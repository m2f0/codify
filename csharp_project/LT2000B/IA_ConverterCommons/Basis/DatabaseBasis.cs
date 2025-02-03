using Dapper;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;

namespace IA_ConverterCommons;

public class DatabaseBasis
{
    public static IntBasis SQLCODE { get; set; } = new IntBasis(new PIC("9", "4", "9(4)"));
    public static StringBasis SQLERRMC { get; set; } = new StringBasis(new PIC("X", "70", "X(70)"));
    public static StringBasis SQLSTATE { get; set; } = new StringBasis(new PIC("X", "5", "X(5)"));
    public static ListBasis<StringBasis> SQLERRD { get; set; } = new ListBasis<StringBasis>(7);

    [JsonIgnore]
    public SQLCA SQLCA_Internal { get; set; } = new SQLCA();

    public static string SQLCA
    {
        get
        {
            return
                SQLCODE.GetMoveValues() +
                SQLERRMC.GetMoveValues() +
                SQLSTATE.GetMoveValues() +
                SQLERRD.GetMoveValues();
        }
    }
}
