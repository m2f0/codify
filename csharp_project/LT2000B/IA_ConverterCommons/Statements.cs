using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
//using com.caixa.backend;
//using static com.caixa.backend.Utils.AppUtils;

namespace IA_ConverterCommons;

public static class Statements
{
    public static string CurrentDateFormat = "yyyyMMddHHmmssfff";
    public static string NormalDatabaseDate = "yyyy-MM-dd";

    public static void Move(this object from, params VarBasis[] tos)
    {
        foreach (var to in tos)
        {
            var values = "";
            if (from is VarBasis)
                values = ((VarBasis)from).GetMoveValues();
            else
                values = from?.ToString();

            to.SetMoveValues(from, values);
        }
    }

    public static void MoveCorr(this VarBasis from, params VarBasis[] tos)
    {
        foreach (var to in tos)
            from.MoveCorr(from, to);
    }
    public static void MoveAll(string param, params VarBasis[] tos)
    {
        foreach (var to in tos)
            to.MoveAll(param);
    }

    public static void Display(this object message)
    {
        if (message is VarBasis)
            ((VarBasis)message).Display();
        else
            VarBasis.Display(message?.ToString());
    }

    public static void Initialize(params VarBasis[] tos)
    {
        foreach (var to in tos)
            to.Initialize();
    }

    public static string InspectConvert(string source, string from, string to)
    {
        var inspectIn0 = "";
        foreach (var c in source)
        {
            var foundedChar = c;
            for (int l = 0; l < from.Length; l++)
            {
                var searchVal = from[l];
                var toVal = to[l];

                if (searchVal == c)
                {
                    foundedChar = toVal;
                    break;
                }
            }

            inspectIn0.Append(foundedChar);
        }
        return inspectIn0;
    }

    public static void Call(string className, params object[] param)
    {
        try
        {
            var methodName = "Execute";

            // Usa a reflexão para carregar a classe e chamar o método
            var tp = TryGetClassName(className);
            if (tp == null)
                throw new Exception($"Erro ao encontrar o programa {className}, talvez não esteja compilado");

            var instance = Activator.CreateInstance(tp);
            var method = instance.GetType().GetMethod(methodName);
            var prop = instance.GetType().GetProperty("IsCall");
            List<object> paramsToSend = new List<object>();

            prop?.SetValue(instance, true);

            // Passa o objeto por referência usando a palavra-chave 'ref'
            var methodArgs = param;


            //parametrização identica
            //caso pular este IF precisa ser revista a quantidade de parametros, não deve ser != 
            if (param.Count() == method.GetParameters().Count())
            {
                for (int i = 0; i < param.Count(); i++)
                {
                    var fromInstance = param[i];
                    var toInstance = Activator.CreateInstance(method.GetParameters()[i].ParameterType.UnderlyingSystemType);

                    //temos um SET de if/else para definir o tipo que será passado por parametro, não entrando neste SET de Ifs, o parametro vai vazio
                    if (toInstance is VarBasis)
                    {
                        try { ((dynamic)toInstance).Pic = ((dynamic)fromInstance).Pic; } catch { }
                        Move(fromInstance, (VarBasis)toInstance);
                    }
                    else if (toInstance is string)
                        toInstance = fromInstance.ToString();

                    paramsToSend.Add(toInstance);
                }
            }

            method.Invoke(instance, paramsToSend.ToArray());

            if (param.Count() == method.GetParameters().Count())
            {
                for (int i = 0; i < paramsToSend.Count(); i++)
                {
                    var paramToReturn = paramsToSend[i];
                    var localParamItem = param[i];
                    //var fromInstance = param[i];
                    //var toInstance = Activator.CreateInstance(method.GetParameters()[i].ParameterType.UnderlyingSystemType);

                    ////temos um SET de if/else para definir o tipo que será passado por parametro, não entrando neste SET de Ifs, o parametro vai vazio
                    if (paramToReturn is VarBasis)
                        Move(paramToReturn, (VarBasis)localParamItem);
                    else if (paramToReturn is string)
                        localParamItem = paramToReturn.ToString();

                    //paramsToSend.Add(toInstance);
                }
            }
        }
        catch (Exception)
        {
        }
        finally
        {
            //EndSession();
        }
    }

    public static void Unstring(VarBasis from, UnstringDelimitedParameter[] delimitedBys, UnstringIntoParameter[] intos, VarBasis? Pointer = null, VarBasis? Tallyng = null)
    {
        var currentString = from.GetMoveValues();
        var currentInto = 0;
        var delimitadorFounded = false;
        var pointerC = 0;

        for (int i = 0; i < currentString.Length; i++)
        {
            pointerC++;
            var currChar = currentString[i].ToString();

            //se um dos delimitadores for encontrado
            var delimitedBy = delimitedBys.FirstOrDefault(x => x.DelimitedBy == currChar);
            if (delimitedBy?.IsAll == true && delimitadorFounded) continue;

            if (delimitedBy != null)
            {
                delimitadorFounded = true;

                var localStr = string.Join("", currentString.Take(i));
                var localInto = intos[currentInto++];

                Move(localStr, localInto.Field1);

                if (localInto.Field2 is not null && localInto.Type == "COUNT")
                    Move(localStr.Length, localInto.Field2);

                currentString = string.Join("", currentString.Skip(i + 1));
                i = -1;
                continue;
            }
        }

        if (Pointer is not null)
            Move(pointerC, Pointer);

        if (Tallyng is not null)
            Move(currentInto, Tallyng);
    }



    public static long NumValC(StringBasis value)
    {
        var ret = value?.Value?
                    .Trim()
                    .Replace(" ", "")
                    .Replace(".", "")
                    .Replace(",", "")
                    ;

        if (!long.TryParse(ret, out var pRes))
            throw new FormatException("Formato inválido, deveria conter apenas numeros");

        return pRes;
    }

    public static long NumVal(StringBasis value)
    {
        var ret = value?.Value?
                    .Trim()
                    .Replace(" ", "")
                    .Replace(".", "")
                    .Replace(",", "")
                    ;

        if (!long.TryParse(ret, out var pRes))
            throw new FormatException("Formato inválido, deveria conter apenas numeros");

        return pRes;
    }

    public static void MoveAtPosition(this object from, VarBasis to, long start, long length, params VarBasis[] tos)
    {
        string fromValues = "";
        var startIndex = start - 1;
        var isStartLength = startIndex != -1 && length != -1;

        if (!isStartLength) return;
        if (length > from.ToString().Length) fromValues = from.ToString().PadLeft((int)length, '0');

        var destinyValues = to.GetMoveValues().ToArray();

        bool isChar = false;
        if (from is not VarBasis)
        {

            fromValues = from.ToString();
            if (from.ToString().IsEmpty()) isChar = true;
        }

        else
            fromValues = ((VarBasis)from).GetMoveValues();

        for (long i = startIndex, j = 0; i < length; i++, j++)
        {
            if (isChar)
                destinyValues[i] = ' ';
            else
                destinyValues[i] = fromValues[(int)j];
        }


        var destiny = string.Join("", destinyValues);
        to.SetMoveValues(from, destiny);

        if (tos != null)
            Move(from, tos);
    }

    public static void MoveAtPosition(this object from, VarBasis to, Int64 start, int length, params VarBasis[] tos)
    {
        MoveAtPosition(from, to, start, (long)length, tos);
    }


    public static bool IsEmpty(this string value)
    {
        var isEmpty = string.IsNullOrWhiteSpace(value?.Replace("0", " "));
        if (!isEmpty)
            isEmpty = string.IsNullOrEmpty(value);

        return isEmpty;
    }

    public static void Replace(this VarBasis me, string from, string to)
    {
        var str = me.GetMoveValues();
        str = str.Replace(from, to);
        me.SetMoveValues(str, str);
    }

    public static void Divide(double value, double by, VarBasis result, VarBasis? remaining = null)
    {
        if (result is IntBasis)
            ((IntBasis)result).Value = (int)Math.Round(value / by);

        if (result is DoubleBasis)
            ((DoubleBasis)result).Value = value / by;

        if (remaining is not null)
        {
            if (remaining is IntBasis)
                ((IntBasis)remaining).Value = (int)Math.Round(value % by);

            if (remaining is DoubleBasis)
                ((DoubleBasis)remaining).Value = value % by;
        }
    }

    public static void Multiply(double value, double by, VarBasis result)
    {
        double product = value * by; // Calcula o produto

        if (result is IntBasis intResult)
        {
            intResult.Value = (int)product; // Converte para int
        }
        else if (result is DoubleBasis doubleResult)
        {
            doubleResult.Value = product; // Atribui diretamente
        }
        else
        {
            throw new InvalidOperationException("Tipo não suportado.");
        }
        //Problemas para ((dynamic)result).Value = value * by;

        //if (remaining is not null)
        //{
        //    var resVal = ((dynamic)result).Value;
        //    ((dynamic)remaining).Value = resVal % by;
        //}
    }

    public static string WhenCompiled(int initial = 0, int final = 0)
    {
        // Obter o Assembly atual
        var assembly = Assembly.GetExecutingAssembly();

        // A informação de compilação contém a data em formato de string
        var dataCompilacao = File.GetCreationTime(assembly.Location);
        if (initial == 0 && final == 0)
            return dataCompilacao.ToString(CurrentDateFormat);

        return dataCompilacao.ToString(CurrentDateFormat.Substring(initial - 1, final));
    }

    public static DateTime DateNow() => DateTime.UtcNow.AddHours(-3);

    public static string AcceptDate(string model = "TIME", string format = "")
    {
        //obtidas em https://www.cadcobol.com.br/accept.htm
        var date = DateNow();

        if (model == "TIME")
            return date.ToString("HHmmssff");

        if (model == "DATE")
            return date.ToString("yyMMdd");

        throw new NotImplementedException($"DEVERIA TER UM TRATAMENTO IGUAL OS ANTERIORES COM O TIPO >{model}<");
    }

    public static DateTime CurrentDateAsDate()
    {
        var date = DateNow();

        return date;
    }

    public static string CurrentDate(string format = "")
    {
        var date = DateNow();

        if (string.IsNullOrWhiteSpace(format))
            format = CurrentDateFormat;

        return date.ToString(format);
    }

    public static string CurrentDate(int startOrLength, int length = 0)
    {
        var date = DateNow();
        if (length == 0)
        {
            length = startOrLength;
            startOrLength = 0;
        }

        return date.ToString(CurrentDateFormat.Substring(startOrLength, length));
    }

    static Type TryGetClassName(string className)
    {
        //Type.GetType($"com.caixa.backend.{className}, Claim")
        var projectList = new List<string>
        {
            "Sias.Bilhetes"
            ,"Sias.Cobranca"
            ,"Sias.Comissao"
            ,"Sias.Cosseguro"
            ,"Sias.Emissao"
            ,"Sias.Geral"
            ,"Sias.Loterico"
            ,"Sias.Outros"
            ,"Sias.PessoaFisica"
            ,"Sias.Sinistro"
            ,"Sias.VidaAzul"
            ,"Sias.VidaEmGrupo"
        };
        var sharedFolder = SearchShareLibFolder();

        foreach (var project in projectList)
        {
            try
            {
                var assembly = Assembly.LoadFrom(Path.Combine(sharedFolder, $"{project}.dll"));
                var name = className;
                var retStr = $"Code.{name}";

                var tp = assembly.GetType($"{retStr}");
                if (tp != null) return tp;
            }
            catch (Exception)
            {

            }
        }

        return null;
    }

    public static string SearchShareLibFolder()
    {
        var currentFolder = Environment.CurrentDirectory;
        for (int i = 0; i < 10; i++)
        {
            if (Directory.Exists(Path.Combine(currentFolder, "SharedLibs")))
                return Path.Combine(currentFolder, "SharedLibs");

            currentFolder = Directory.GetParent(currentFolder).FullName;
        }

        return null;
    }

    public static long ToInt(this string str)
    {
        if (string.IsNullOrEmpty(str)) return -1;

        if (!long.TryParse(str, out var pStr))
            throw new Exception("o valor do parametro STR em ToInt deve ser preenchido");

        return pStr;
    }

    public static DateTime ToDateTime(this StringBasis value)
    {
        var pic = value.Pic.Length;
        pic = pic > CurrentDateFormat.Length ? CurrentDateFormat.Length : pic;

        var formatToPass = pic == 10 ? NormalDatabaseDate : CurrentDateFormat.Substring(0, pic);

        return DateTime.ParseExact(value.ToString().Trim(), formatToPass, null);
    }

    public static DateTime ToFirstDayDateTime(this StringBasis value)
    {
        var pic = value.Pic.Length;

        var dt = DateTime.ParseExact(value.ToString(), CurrentDateFormat.Substring(0, pic), null);

        return new DateTime(dt.Year, dt.Month, 1);
    }

    public static bool IsEqual(VarBasis varBasis, string value)
    {
        return varBasis.GetMoveValues().Trim() == value.Trim();
    }

    public static void ThreatableTestError(Exception ex)
    {
        if (ex.Message.Contains("The syntax of the string representation of a datetime value is incorrect")) throw ex;
        if (ex.Message.Contains("Invalid SQL syntax")) throw ex;
    }
}
