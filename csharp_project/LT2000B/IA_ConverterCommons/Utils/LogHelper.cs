using MethodDecorator.Fody.Interfaces;
using System.Diagnostics;
using System.Reflection;
using System;
using Newtonsoft.Json;
using IA_ConverterCommons;
using System.Text.RegularExpressions;
using System.IO;
using System.Text.Json.Serialization;
using System.Text.Json;

//[module: StopWatch]
namespace IA_ConverterCommons
{
    public class LogHelper
    {
        public static void Log(MethodBase method, string message)
        {
            var log = new LogFormat();
            log.timer = Stopwatch.StartNew();
            log.Method = method.Name;
            log.Class = method.DeclaringType?.Name;
            log.MethodHeader = $"log: {StopWatchAttribute.LogID}";

            var mss = message.Replace("\t", " ");
            mss = message.Replace("\n", " ");
            mss = Regex.Replace(message, @"\s+", @" ");

            log.Message = mss;

            Log(log);
        }

        public static void Log(MethodBase method, string fileName, string query, string error, long sqlCode, Guid dBCallId, int rowsCount)
        {
            var log = new LogFormatSQL();
            log.timer = Stopwatch.StartNew();

            log.DBCallId = dBCallId;
            log.sqlCode = sqlCode;
            log.rowsCount = rowsCount;

            var quer = query.Replace("\t", " ");
            quer = query.Replace("\n", " ");
            quer = Regex.Replace(query, @"\s+", @" ");

            log.Query = quer;

            var err = error.Replace("\t", " ");
            err = error.Replace("\n", " ");
            err = Regex.Replace(error, @"\s+", @" ");

            log.Error = err.Trim();
            var isError = !string.IsNullOrEmpty(log.Error);
            if (isError)
                log.Error = ">>> " + log.Error;

            Log(log);

            if (AppSettings.TestSet.DB_Test.Is_DB_Test && (!AppSettings.TestSet.DB_Test.LogOnlyError || (AppSettings.TestSet.DB_Test.LogOnlyError && isError)))
            {
                var outDirectory = Path.Combine(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.ToString(), "TESTSET_LOG");
                if (!Directory.Exists(outDirectory)) Directory.CreateDirectory(outDirectory);

                var fullFilePath = Path.Combine(outDirectory, $"{fileName}.dbd");
                var jumpLine = File.Exists(fullFilePath) ? "\n" : "";
                File.AppendAllText(fullFilePath, jumpLine + JsonConvert.SerializeObject(log, Formatting.None));
            }
        }

        public static void Log(LogFormat log)
        {
            var message = JsonConvert.SerializeObject(log);
            Log(message);
        }
        public static void Log(LogFormatSQL log)
        {
            var message = JsonConvert.SerializeObject(log);
            Log(message);
        }

        public static void Log(string message)
        {
            Console.WriteLine(message);
            Debug.WriteLine(message);
        }
    }
}
