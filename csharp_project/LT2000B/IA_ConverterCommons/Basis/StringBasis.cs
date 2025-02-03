using System;

namespace IA_ConverterCommons;

public class StringBasis : StructBasis<string>
{
    //NUNCA USAR
    public StringBasis() : base(new PIC("X", "04", "X(04)"), "") { }

    public StringBasis(PIC picAttr, string value = "")
        : base(picAttr, value) { }

    static string RemovePossibleDateDiactrics(string toRemove)
    {
        return toRemove.Replace("-", "")
                                .Replace("/", "")
                                .Replace(" ", "")
                                .Replace(":", "")
                                .Replace(".", "")
                                .Replace("\\", "");
    }

    public static implicit operator string(StringBasis stringBasis)
    {
        return stringBasis.ToString();
    }

    public static bool operator >(StringBasis stringBasis, string comparacao)
    {
        var strThreated = RemovePossibleDateDiactrics(stringBasis.ToString());
        var compThreated = RemovePossibleDateDiactrics(comparacao.ToString());

        if (Int64.TryParse(strThreated, out var sParse) && Int64.TryParse(compThreated, out var cParse))
            return sParse > cParse;

        return false;
    }

    public static bool operator <(StringBasis stringBasis, string comparacao)
    {
        var strThreated = RemovePossibleDateDiactrics(stringBasis.ToString());
        var compThreated = RemovePossibleDateDiactrics(comparacao.ToString());

        if (Int64.TryParse(strThreated, out var sParse) && Int64.TryParse(compThreated, out var cParse))
            return sParse < cParse;

        return false;
    }

    public static bool operator >(StringBasis stringBasis, StringBasis comparacao)
    {
        var strThreated = RemovePossibleDateDiactrics(stringBasis.ToString());
        var compThreated = RemovePossibleDateDiactrics(comparacao.ToString());

        if (Int64.TryParse(strThreated, out var sParse) && Int64.TryParse(compThreated, out var cParse))
            return sParse > cParse;

        return false;
    }

    public static bool operator <(StringBasis stringBasis, StringBasis comparacao)
    {
        var strThreated = RemovePossibleDateDiactrics(stringBasis.ToString());
        var compThreated = RemovePossibleDateDiactrics(comparacao.ToString());

        if (Int64.TryParse(strThreated, out var sParse) && Int64.TryParse(compThreated, out var cParse))
            return sParse < cParse;

        return false;
    }

    public static bool operator >(VarBasis stringBasis, StringBasis comparacao)
    {
        var strThreated = RemovePossibleDateDiactrics(stringBasis.ToString());
        var compThreated = RemovePossibleDateDiactrics(comparacao.ToString());

        if (Int64.TryParse(strThreated, out var sParse) && Int64.TryParse(compThreated, out var cParse))
            return sParse > cParse;

        return false;
    }

    public static bool operator <(VarBasis stringBasis, StringBasis comparacao)
    {
        var strThreated = RemovePossibleDateDiactrics(stringBasis.ToString());
        var compThreated = RemovePossibleDateDiactrics(comparacao.ToString());

        if (Int64.TryParse(strThreated, out var sParse) && Int64.TryParse(compThreated, out var cParse))
            return sParse < cParse;

        return false;
    }

    public static bool operator >=(StringBasis stringBasis, string comparacao)
    {
        var strThreated = RemovePossibleDateDiactrics(stringBasis.ToString());
        var compThreated = RemovePossibleDateDiactrics(comparacao.ToString());

        if (Int64.TryParse(strThreated, out var sParse) && Int64.TryParse(compThreated, out var cParse))
            return sParse >= cParse;

        return false;
    }

    public static bool operator <=(StringBasis stringBasis, string comparacao)
    {
        var strThreated = RemovePossibleDateDiactrics(stringBasis.ToString());
        var compThreated = RemovePossibleDateDiactrics(comparacao.ToString());

        if (Int64.TryParse(strThreated, out var sParse) && Int64.TryParse(compThreated, out var cParse))
            return sParse <= cParse;

        return false;
    }

    public static bool operator ==(StringBasis basis, StringBasis comparacao)
    {
        return basis.GetMoveValues().Trim() == comparacao.GetMoveValues().Trim();
    }

    public static bool operator !=(StringBasis basis, StringBasis comparacao)
    {
        return !(basis == comparacao);
    }

    public static bool operator !=(StringBasis basis, IntBasis comparacao)
    {
        return basis.GetMoveValues() != comparacao.GetMoveValues();
    }

    public static bool operator ==(StringBasis basis, IntBasis comparacao)
    {
        return basis.GetMoveValues() == comparacao.GetMoveValues();
    }

    public static bool operator ==(StringBasis basis, string comparacao)
    {
        var moveValues = basis.GetMoveValues();
        return moveValues == comparacao || moveValues.Trim() == comparacao;
    }

    public static bool operator !=(StringBasis basis, string comparacao)
    {
        var moveValues = basis.GetMoveValues();
        return !(moveValues == comparacao || moveValues.Trim() == comparacao);
    }

    public override string? ToString()
    {
        return PaddedValue();
    }
}
