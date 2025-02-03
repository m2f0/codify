namespace IA_ConverterCommons;

public class SQLCA : VarBasis
{
    public SQLCA(int sqlCode = 0)
    {
        SQLCODE.Value = sqlCode;
    }

    public SQLCA(SQLCA sqlca)
    {
        SQLCODE.Value = sqlca.SQLCODE.Value;
        SQLERRMC.Value = sqlca.SQLERRMC.Value;
        SQLSTATE.Value = sqlca.SQLSTATE.Value;
        SQLERRD.Items = sqlca.SQLERRD.Items;
    }

    public IntBasis SQLCODE { get; set; } = new IntBasis(new PIC("9", "4", "9(4)"));
    public StringBasis SQLERRMC { get; set; } = new StringBasis(new PIC("X", "70", "X(70)"));
    public StringBasis SQLSTATE { get; set; } = new StringBasis(new PIC("X", "5", "X(5)"));
    public ListBasis<StringBasis> SQLERRD { get; set; } = new ListBasis<StringBasis>(7);
}