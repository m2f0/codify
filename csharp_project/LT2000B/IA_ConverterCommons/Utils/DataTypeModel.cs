using MethodDecorator.Fody.Interfaces;
using System.Diagnostics;
using System.Reflection;
using System;
using Newtonsoft.Json;
using IA_ConverterCommons;
using System.Text.RegularExpressions;
using System.Linq;

//[module: StopWatch]
namespace IA_ConverterCommons
{
    public class DataTypeModel
    {
        public string Name { get; set; }
        public string DefaultValue { get; set; }
        public string CSharpType { get; set; }
        public int Precision { get; set; } = 0;
        public int Length { get; set; } = 0;

        public static DataTypeModel GetDataType(string fullPicDecl, string forceType = "")
        {
            var ret = new DataTypeModel();
            var trimmedData = fullPicDecl.Replace(",", "V").Replace(".", "");
            var isSignal = false;

            if (trimmedData[0] == 'S' || (trimmedData[0] == '-' && trimmedData[1] != '-'))
            {
                isSignal = true;
                trimmedData = trimmedData.Substring(1);
            }
            trimmedData = trimmedData.Replace("-", "Z");

            var splitedPicDecimal = trimmedData.Split('V');
            var picData = splitedPicDecimal[0];
            var decimalData = splitedPicDecimal.Length > 1 ? splitedPicDecimal[1] : "";

            var isNumeric = picData.Contains("9");
            var isDecimal = trimmedData.Contains("V");
            var length = 0;
            var precision = 0;

            if (picData.Contains("X"))
            {
                ret.Name = "StringBasis";
                ret.DefaultValue = "\"\"";
                ret.CSharpType = "string";

                var rex = Regex.Matches(picData, @"X\((?<numWval>\d+)\)");
                foreach (var match in rex.Where(x => x.Success))
                {
                    length += int.Parse(match.Groups["numWval"].Value);
                    picData = picData.Replace(match.Value, "");
                }

                length += picData.Count(x => x == 'X');
            }
            else if (isNumeric)
            {
                ret.Name = isDecimal ? "DoubleBasis" : "IntBasis";
                ret.DefaultValue = "0";
                ret.CSharpType = isDecimal ? "double" : "Int64";

                var rex = Regex.Matches(picData, @"9\((?<numWval>\d+)\)");
                foreach (var match in rex.Where(x => x.Success))
                {
                    length += int.Parse(match.Groups["numWval"].Value);
                    picData = picData.Replace(match.Value, "");
                }

                length += picData.Count(x => x == '9' || x == 'Z');

                if (isDecimal)
                {
                    rex = Regex.Matches(decimalData, @"9\((?<numWval>\d+)\)");
                    foreach (var match in rex.Where(x => x.Success))
                    {
                        precision += int.Parse(match.Groups["numWval"].Value);
                        decimalData = decimalData.Replace(match.Value, "");
                    }

                    precision += decimalData.Count(x => x == '9' || x == 'Z');
                }
            }
            else
            {
                var resolved = false;
                if (!string.IsNullOrEmpty(forceType))
                {
                    if (forceType.Contains("9"))
                    {
                        ret.Name = isDecimal ? "DoubleBasis" : "IntBasis";
                        ret.DefaultValue = "0";
                        ret.CSharpType = isDecimal ? "double" : "Int64";

                        resolved = true;
                    }
                    else if (forceType.Contains("X"))
                    {
                        ret.Name = "StringBasis";
                        ret.DefaultValue = "\"\"";
                        ret.CSharpType = "string";

                        resolved = true;
                    }
                }

                if (!resolved)
                {
                    ret.Name = "object"; // Tipo padrão se não reconhecermos o tipo
                    ret.DefaultValue = "NOTHING";
                    ret.CSharpType = "object";
                }

            }

            ret.Length = length;
            ret.Precision = precision;

            return ret;
        }
    }
}
