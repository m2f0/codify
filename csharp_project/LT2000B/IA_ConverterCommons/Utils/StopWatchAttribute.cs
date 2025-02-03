using MethodDecorator.Fody.Interfaces;
using System.Diagnostics;
using System.Reflection;
using System;
using Newtonsoft.Json;
using IA_ConverterCommons;

//[module: StopWatch]
namespace IA_ConverterCommons
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly | AttributeTargets.Module)]
    public class StopWatchAttribute : Attribute, IMethodDecorator
    {
        private LogFormat log = new LogFormat();
        public static Guid LogID = Guid.NewGuid();
        public static string LastMethod = "";
        public static int LastCounter = 1;

        public StopWatchAttribute() { }

        // instance, method and args can be captured here and stored in attribute instance fields
        // for future usage in OnEntry/OnExit/OnException
        public void Init(object instance, MethodBase method, object[] args)
        {
            log.timer = Stopwatch.StartNew();
            log.Method = method.Name;
            log.Class = method.DeclaringType?.Name;
            log.MethodHeader = $"log: {LogID}";
            log.Message = $"Starting Method";
            Log(log);
        }

        public void OnEntry()
        {
        }

        public void OnExit()
        {
            log.timer.Stop();
            log.Message = $"Ending Method [OK]";
            Log(log);
        }

        public void OnException(Exception exception)
        {
            if (exception is GoBack)
            {
                OnExit();
                return;
            }

            log.timer.Stop();
            log.Message = $"Ending Method [EXCEPTION]";
            log.Error = $"{exception.Message}";
            Log(log);
        }

        private void Log(LogFormat logF)
        {
            if (log.Method == LastMethod)
            {
                LastCounter++;
            }
            else
                LastCounter = 1;

            logF.Counter = LastCounter;
            LastMethod = log.Method;

            LogHelper.Log(logF);
        }
    }

    public class LogFormat
    {
        [JsonIgnore]
        public Stopwatch? timer;

        public int? Counter { get; set; } = 1;
        public string? Class { get; set; } = "";
        public string? MethodHeader { get; set; } = "";
        public string? Method { get; set; } = "";
        public string? Time => $"{timer?.ElapsedMilliseconds} ms";
        public string? Message { get; set; } = "";
        public string? Error { get; set; } = "";
    }
    public class LogFormatSQL 
    {
        [JsonIgnore]
        public Stopwatch? timer;

        public Guid? DBCallId { get; set; } = null;
        
        public DateTime? TimeStamp { get; set; } = DateTime.Now;
        public string? Query { get; set; } = "";
        public long? sqlCode { get; set; } = null;
        public int? rowsCount { get; set; } = null;
        public string? Time => $"{timer?.ElapsedMilliseconds} ms";        
        public string? Error { get; set; } = "";

    }
}
