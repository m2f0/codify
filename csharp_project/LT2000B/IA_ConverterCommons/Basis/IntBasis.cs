using System;

namespace IA_ConverterCommons;

public class IntBasis : StructBasis<Int64>
{
    //NUNCA USAR
    public IntBasis() : base(new PIC("", "", ""), 0) { }

    public bool IsNegative => Value < 0;

    public IntBasis(PIC picAttr, Int64 value = 0)
        : base(picAttr, value) { }

    //public static implicit operator IntBasis(Int64 value)
    //{
    //    throw new NotImplementedException();
    //}

    public static implicit operator long(IntBasis intBasis)
    {
        return intBasis.Value;
    }

    //adicionado por causa do programa CT0007S
    public static implicit operator int(IntBasis intBasis)
    {
        return (int)intBasis.Value;
    }

    public static bool operator !=(IntBasis basis, StringBasis comparacao)
    {
        return basis.GetMoveValues() != comparacao.GetMoveValues();
    }

    public static bool operator ==(IntBasis basis, StringBasis comparacao)
    {
        return basis.GetMoveValues() == comparacao.GetMoveValues();
    }

    public static bool operator ==(IntBasis basis, IntBasis comparacao)
    {
        return basis.Value == comparacao.Value;
    }

    public static bool operator !=(IntBasis basis, IntBasis comparacao)
    {
        return basis.Value != comparacao.Value;
    }

    public static bool operator ==(IntBasis basis, string comparacao)
    {
        var moveValues = basis.GetMoveValues();
        return moveValues == comparacao || moveValues.Trim() == comparacao ||
            (
                int.TryParse(moveValues, out var pMove)
                && int.TryParse(comparacao, out var pComp)
                && pMove == pComp
            );
    }

    public static bool operator !=(IntBasis basis, string comparacao)
    {
        return !(basis == comparacao);
    }


    public static bool operator <(IntBasis basis, IntBasis compare)
    {
        //vamos testar se é comparação de strings provaveis datas
        var basisData = basis.GetMoveValues();
        var compareData = compare.GetMoveValues();

        var isBasisNumber = long.TryParse(basisData, out var pNumberBasis);
        var isCompareNumber = long.TryParse(compareData, out var pNumberCompare);

        if (isBasisNumber && isCompareNumber)
            return pNumberBasis < pNumberCompare;

        return basisData?.Length < compareData?.Length;
    }

    public static bool operator >(IntBasis basis, IntBasis compare)
    {
        //vamos testar se é comparação de strings provaveis datas
        var basisData = basis.GetMoveValues();
        var compareData = compare.GetMoveValues();

        var isBasisNumber = long.TryParse(basisData, out var pNumberBasis);
        var isCompareNumber = long.TryParse(compareData, out var pNumberCompare);

        if (isBasisNumber && isCompareNumber)
            return pNumberBasis > pNumberCompare;

        return basisData?.Length > compareData?.Length;
    }

    public static bool operator <(VarBasis basis, IntBasis compare)
    {
        if (int.TryParse(basis.GetMoveValues(), out var pBasis))
            return pBasis < compare;

        return false;
    }

    public static bool operator >(VarBasis basis, IntBasis compare)
    {
        if (int.TryParse(basis.GetMoveValues(), out var pBasis))
            return pBasis > compare;

        return false;
    }

    public override string? ToString()
    {
        return PaddedValue();
    }
}
