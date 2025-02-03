using IA_ConverterCommons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA_ConverterCommons;

public class UnstringIntoParameter
{
    public VarBasis Field1 { get; set; }
    public string? Type { get; set; } //este campo pode ser COUNT como pode ser DELIMITER 
    public VarBasis? Field2 { get; set; }

    public UnstringIntoParameter(VarBasis field1, string? type = null, VarBasis? field2 = null)
    {
        Field1 = field1;
        Type = type;
        Field2 = field2;
    }
}
public class UnstringDelimitedParameter
{
    public bool IsAll { get; set; }
    public string DelimitedBy { get; set; }

    public UnstringDelimitedParameter(string delimitedBy, bool isAll = false)
    {
        IsAll = isAll;
        DelimitedBy = delimitedBy;
    }
}

