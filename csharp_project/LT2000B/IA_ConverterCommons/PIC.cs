using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IA_ConverterCommons;

public class PIC
{
    public string CobolType { get; set; }
    public string CobolLength { get; set; }
    public string FullPic { get; set; }
    public int _length = -1;
    public int Length
    {
        get
        {
            if (_length != -1) return _length;

            _length = DataTypeModel.GetDataType(FullPic).Length;

            return _length;
        }
    }

    public PIC(string type, string length, string fullPic)
    {
        CobolType = type;
        CobolLength = length;
        FullPic = fullPic;
    }
}