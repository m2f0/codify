using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IA_ConverterCommons;

public class AppSettings
{
    public static TestSettings TestSet { get; set; } = new TestSettings();

    public static AppSettings Settings { get; set; }
    public AppSettings(IConfigurationRoot config)
    {
        DB2ConnectionString = config["DB2ConnectionString"];
        SqlConnectionString = config["SqlConnectionString"];
        FileFolderPath = config["FileFolderPath"];
        DbNamesFromTo = config["DbNamesFromTo"];
        MemoryFiles = new List<string>();

        if (!string.IsNullOrEmpty(SqlConnectionString))
        {
            var splits = DbNamesFromTo.Split(";");
            DBNamesList = splits.ToDictionary(
                x =>
                {
                    var split = x.Split("|");
                    return split[0];
                },
                x =>
                {
                    var split = x.Split("|");
                    return split[1];
                }
            );
        }

        TestSet.QueryLimit = int.TryParse(config["LimitQueryToTest"], out var pLimit) ? pLimit : 0;
    }

    public string DB2ConnectionString { get; set; }
    public string SqlConnectionString { get; set; }
    public bool IsSqlServer => !string.IsNullOrEmpty(SqlConnectionString);
    public string FileFolderPath { get; set; }
    public List<string> MemoryFiles { get; set; } = new List<string>();
    public string DbNamesFromTo { get; set; }
    public Dictionary<string, string> DBNamesList { get; set; } = new Dictionary<string, string>();

    public static void Load()
    {
        // Configurar a leitura do arquivo appsettings.json
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        // Criar uma instância da classe de configuração
        var appSettings = new AppSettings(config);
        Settings = appSettings;

        //Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");

        //culture customizado copiando CultureInfo("pt-BR") para definir a data no formato identico ao da base DB2

        var customCulture = (CultureInfo)CultureInfo.CreateSpecificCulture("pt-BR").Clone();

        customCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
        customCulture.DateTimeFormat.LongDatePattern = "yyyy-MM-dd";
        customCulture.DateTimeFormat.DateSeparator = "-";

        Thread.CurrentThread.CurrentCulture = customCulture;
        Thread.CurrentThread.CurrentUICulture = customCulture;
    }
}

public class TestSettings
{
    public readonly object _lock = new object();
    public bool IsTest { get; set; }
    public DB_Test DB_Test = new DB_Test();
    public int QueryLimit { get; set; }
    public Dictionary<string, DynamicData> DynamicData { get; set; } = new Dictionary<string, DynamicData>();
}

public class DB_Test
{
    public bool Is_DB_Test { get; set; }
    public bool LogOnlyError { get; set; } = false;
    public DateTime DbTestTime { get; set; }
    public string DbTestTimeStr => DbTestTime.ToString("yyyy-MM-dd hh-mm-ss");

    public DB_Test()
    {
        DbTestTime = DateTime.UtcNow.AddHours(-3);
    }
}

public class DynamicData
{
    public List<Dictionary<string, string>> DynamicList { get; set; } = new List<Dictionary<string, string>> { };
    public List<Dictionary<string, object>> DynamicObjectList => DynamicList?.Select(x => x.ToDictionary(x => x.Key, x => (object)x.Value)).ToList();

    public List<SQLCA> ChangeSQLCAOpen { get; set; } = new List<SQLCA>();
    public List<SQLCA> ChangeSQLCAFetch { get; set; } = new List<SQLCA>();
    public List<SQLCA> ChangeSQLCAClose { get; set; } = new List<SQLCA>();

    public void AddDynamic(Dictionary<string, string> data, SQLCA? sqlcaOpen = null, SQLCA? sqlcaFetch = null, SQLCA? sqlcaClose = null)
    {
        DynamicList.Add(data);

        ChangeSQLCAOpen.Add(sqlcaOpen);
        ChangeSQLCAFetch.Add(sqlcaFetch);
        ChangeSQLCAClose.Add(sqlcaClose);
    }
}