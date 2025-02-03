using Dapper;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text.Json.Serialization;
using System.Diagnostics;
using System.Data.SqlClient;

namespace IA_ConverterCommons;

public enum ThreatmentType
{
    Query,
    InsertWhereField
}

public abstract class QueryBasis<T> : DatabaseBasis where T : class
{
    public List<T> FetchData { get; set; } = new List<T>();
    [JsonIgnore]
    public List<T> AllData => FetchData.ToList();
    protected int CurrentIndex { get; set; } = -1;
    protected bool IsCursor { get; set; } = false;
    protected bool IsProcedure { get; set; } = false;
    protected dynamic Params { get; set; }

    private string _query;
    private string Query => TranslateQuery(_query);

    private Guid guid = Guid.NewGuid();

    private int RowsCount { get; set; } = 0;

    private string TranslateQuery(string query)
    {
        if (IsProcedure)
            if (!AppSettings.Settings.IsSqlServer)
            {
                var shadowToLog = $"CALL {query} (";
                shadowToLog += string.Join($",{Environment.NewLine}", ((PropertyInfo[])Params.GetType().GetProperties()).Select(x => $"'{x.GetValue(Params)}'"));
                shadowToLog += $"\n)";

                return shadowToLog;
            }

        return query;

        //var match = Regex.Match(query, @"SELECT\s+NEXT\s+VALUE\s+FOR\s+SEGUROS.SZ014SQ");
        //if (match.Success)
        //{
        //    query = query.Replace(match.Value, "SELECT MAX(SEQ_RECEBIMENTO) + 1");
        //    query.Replace("SYSIBM.SYSDUMMY1", "SEGUROS.SZ_MOV_CONTROLE");
        //}

        ////IsSqlServer
        //if (!AppSettings.Settings.IsDb2)
        //{
        //    query = Regex.Replace(query, @"VALUE\s*\(", $"COALESCE(");
        //    query = Regex.Replace(query, @"WITH\s+UR", $"");
        //    query = Regex.Replace(query, @"DIGITS\s*\(", $"CONVERT(DECIMAL(18, 2),");
        //    query = query.Replace("SEGUROS.", $"{AppSettings.Settings.DatabasePrefix}.");
        //}

        //return query;
    }
    public string FieldThreatment(string value, ThreatmentType type = ThreatmentType.Query)
    {
        var ret = value;

        if (value == null)
        {
            if (type == ThreatmentType.InsertWhereField)
                return "IS NULL";

            return "null";
        }

        if (value.Length == 10 || value.Length == 19)
            if (value.Count(x => x == '/') == 2 || value.Count(x => x == '-') == 2)
                if (DateTime.TryParse(value, out DateTime pDate))
                    ret = pDate.ToString("yyyy-MM-dd");

        if (value.Contains(","))
            if (double.TryParse(value, out double pDouble))
                ret = value.Replace(",", ".");

        return (type == ThreatmentType.InsertWhereField ? " = " : "") + $"'{ret}'";
    }
    public int FieldTreatmentInteger(string value, ThreatmentType type = ThreatmentType.Query)
    {
        string ret = value;

        if (value == null)
        {
            if (type == ThreatmentType.InsertWhereField)
                return 0;
            return 0;
        }
        return Convert.ToInt16(ret);
    }
    public void Allocate(string procName)
    {
        SQLCA_Internal.SQLCODE.SetValue(0);
        SQLCA_Internal.SQLERRMC.SetValue("");

        IsProcedure = true;
        _query = procName;
    }

    //use wisely
    public QueryBasis() { }

    public QueryBasis(string query)
    {
        this._query = query;
    }

    public void SetQuery(string query)
    {
        _query = query;
    }

    [StopWatch]
    public void ExecuteQuery()
    {
        CurrentIndex = 0;
        SQLCA_Internal.SQLCODE.SetValue(0);
        SQLCA_Internal.SQLSTATE.SetValue("");
        SQLCA_Internal.SQLERRMC.SetValue("");
        SQLCA_Internal.SQLERRD = new ListBasis<StringBasis>(7);

        try { LogHelper.Log(MethodBase.GetCurrentMethod(), Query); } catch { }

        try
        {
            int lines = 0;
            if (AppSettings.TestSet.IsTest)
            {
                if (AppSettings.TestSet.DynamicData.ContainsKey(GetType().Name))
                {
                    var dynData = AppSettings.TestSet.DynamicData[GetType().Name];
                    var dta = new Dictionary<string, string>();
                    var considerInserted = false;
                    var considerUpdateNoParameters = false;
                    foreach (var prop in GetType().GetProperties())
                    {
                        if (dynData.DynamicList.Any(x => x.Keys.Contains(prop.Name)))
                        {
                            dta.Add(prop.Name, prop.GetValue(this).ToString());
                            considerInserted = true;
                        }
                        if (!dynData.DynamicList.Any(x => x.Keys.Contains(prop.Name)) && Query.Contains("UPDATE"))
                        {
                            considerUpdateNoParameters = true;
                        }
                    }

                    if (considerInserted)
                        dynData.AddDynamic(dta);

                    if (considerUpdateNoParameters)
                        dta.Add("UpdateCheck", "UpdateCheck");
                    dynData.AddDynamic(dta);

                    if (GetType().Name.Contains("Delete") && !GetType().Name.Contains("Select") && !GetType().Name.Contains("Insert") && !GetType().Name.Contains("Update"))
                    {
                        dynData.DynamicList.Clear();
                    }

                    //AppSettings.TestSet.DynamicData[GetType().Name].AddDynamic();
                    lines = dynData.DynamicList.Count - 1;

                    var sqlca = dynData?.ChangeSQLCAOpen;
                    if (sqlca?.Count > 0)
                    {
                        SQLCA_Internal = sqlca?.FirstOrDefault() ?? SQLCA_Internal;
                    }
                }
            }
            else
            {
                using (var dbConnection = DatabaseConnection.Instance)
                    lines = dbConnection.Connection.Execute(Query, transaction: dbConnection.GetTransaction());
            }

            RowsCount = lines;
            SQLCA_Internal.SQLERRD[3].Value = lines.ToString();
        }
        catch (Exception ex)
        {
            SQLCA_Internal.SQLCODE.SetValue(999);
            SQLCA_Internal.SQLSTATE.SetValue("01S01");
            SQLCA_Internal.SQLERRMC.SetValue($"Erro: " + ex.Message);
            SQLCA_Internal.SQLERRD[3].Value = "0";

            if (AppSettings.TestSet.DB_Test.Is_DB_Test) throw;
        }
        finally
        {
            DatabaseBasis.SQLCODE = SQLCA_Internal.SQLCODE;
            DatabaseBasis.SQLERRMC = SQLCA_Internal.SQLERRMC;
            DatabaseBasis.SQLSTATE = SQLCA_Internal.SQLSTATE;
            DatabaseBasis.SQLERRD = SQLCA_Internal.SQLERRD;

            try { LogHelper.Log(MethodBase.GetCurrentMethod(), this.GetType().Namespace.Split(".").LastOrDefault(), Query, SQLCA_Internal.SQLERRMC, SQLCODE.Value, guid, RowsCount); } catch { }
        }
    }

    [StopWatch]
    public void Open(dynamic? procParams = null)
    {
        CurrentIndex = 0;
        FetchData.Clear();
        SQLCA_Internal.SQLCODE.SetValue(0);
        SQLCA_Internal.SQLERRMC.SetValue("");
        SQLCA_Internal.SQLSTATE.SetValue("");

        try { LogHelper.Log(MethodBase.GetCurrentMethod(), Query); } catch { }

        try
        {
            IEnumerable<dynamic> lines = null;

            //aqui ocorre a passagem de dados para o projeto de teste, em execução comum não deve entrar no IF e sim no ELSE
            if (AppSettings.TestSet.IsTest)
            {
                if (AppSettings.TestSet.DynamicData.ContainsKey(GetType().Name))
                {
                    var dynData = AppSettings.TestSet.DynamicData[GetType().Name];
                    lines = dynData.DynamicObjectList;
                    var sqlca = dynData?.ChangeSQLCAOpen;

                    if (sqlca?.Count > 0)
                        SQLCA_Internal = sqlca?.FirstOrDefault() ?? SQLCA_Internal;
                }
            }
            else
            {
                using (var dbConnection = DatabaseConnection.Instance)
                {
                    if (IsProcedure)
                    {
                        lines = dbConnection.Connection.Query(_query, (object)procParams, commandType: System.Data.CommandType.StoredProcedure, transaction: dbConnection.GetTransaction());
                    }
                    else
                    {
                        lines = dbConnection.Connection.Query(Query, transaction: dbConnection.GetTransaction());
                    }
                }
            }

            if (lines != null)
                // Imprimir os resultados
                foreach (var line in lines)
                {
                    var result = ((IReadOnlyDictionary<string, object>)line)?.AsList();
                    if (result == null) break;

                    var item = OpenData(result);

                    FetchData.Add(item);
                }

            if (AppSettings.TestSet.QueryLimit > 0)
                if (FetchData.Count > 0)
                    FetchData = FetchData.Take(AppSettings.TestSet.QueryLimit).ToList();

            RowsCount = lines?.Count() ?? 0;

            //if (lines?.Any() != true)
            //{
            //    SQLCA.SQLCODE.SetValue(100);
            //    SQLCA.SQLSTATE.SetValue("02000");
            //}
        }
        catch (Exception ex)
        {
            SQLCA_Internal.SQLCODE.SetValue(999);
            SQLCA_Internal.SQLSTATE.SetValue("01S01");
            SQLCA_Internal.SQLERRMC.SetValue($"Erro: " + ex.Message);

            if (AppSettings.TestSet.DB_Test.Is_DB_Test) throw;
        }
        finally
        {
            DatabaseBasis.SQLCODE = SQLCA_Internal.SQLCODE;
            DatabaseBasis.SQLERRMC = SQLCA_Internal.SQLERRMC;
            DatabaseBasis.SQLSTATE = SQLCA_Internal.SQLSTATE;

            try { LogHelper.Log(MethodBase.GetCurrentMethod(), this.GetType().Namespace.Split(".").LastOrDefault(), Query, SQLCA_Internal.SQLERRMC, SQLCODE.Value, guid, RowsCount); } catch { }
        }
    }

    //classe para varrer os dados
    public abstract T OpenData(List<KeyValuePair<string, object>> result);

    [StopWatch]
    public bool Fetch()
    {
        var isEOF = CurrentIndex >= FetchData?.Count || FetchData?.Count == 0;
        var data = isEOF ? null : FetchData[CurrentIndex++];
        var properties = GetType().GetProperties();

        if (isEOF && SQLCA_Internal.SQLCODE != 999)
        {
            SQLCA_Internal.SQLCODE.SetValue(100);
            SQLCA_Internal.SQLERRMC.SetValue("Fim de linhas");
        }
        else if (SQLCA_Internal.SQLCODE == 999)
        {

        }
        else
        {
            SQLCA_Internal.SQLCODE.SetValue(0);
            SQLCA_Internal.SQLERRMC.SetValue($"{CurrentIndex}/{FetchData?.Count} encontrada(s)");
        }

        foreach (var property in properties)
        {
            if (property.CanWrite)
            {
                var ignoreList = new List<string> {
                    "FetchData",
                    "AllData",
                    "SQLCA",
                    "IsCursor",
                    "JustACursor",
                    "SQLCA_Internal",
                };

                object dta = null;
                if (!isEOF)
                    dta = data.GetType().GetProperty(property.Name).GetValue(data);

                if (!ignoreList.Contains(property.Name))
                    property.SetValue(this, dta ?? "", null);
            }
        }

        if (AppSettings.TestSet.IsTest)
        {
            if (AppSettings.TestSet.DynamicData.ContainsKey(GetType().Name))
            {
                var dynData = AppSettings.TestSet.DynamicData[GetType().Name];

                var sqlca = dynData?.ChangeSQLCAFetch;
                if (sqlca?.Count > 0)
                {
                    SQLCA_Internal = sqlca?.FirstOrDefault() ?? SQLCA_Internal;
                }

                if (dynData.DynamicList.Count > 0)
                {
                    dynData.DynamicList.RemoveAt(0);
                    if (!IsCursor)
                        CurrentIndex = 0;
                }
            }
        }

        DatabaseBasis.SQLCODE = SQLCA_Internal.SQLCODE;
        DatabaseBasis.SQLERRMC = SQLCA_Internal.SQLERRMC;
        DatabaseBasis.SQLSTATE = SQLCA_Internal.SQLSTATE;
        return DatabaseBasis.SQLCODE == 0;
    }

    [StopWatch]
    public void Close()
    {
        CurrentIndex = -1;
        FetchData = new List<T>();

        DatabaseBasis.SQLCODE.SetValue(0);
        DatabaseBasis.SQLERRMC.SetValue("Cursor Fechado");
    }
}
