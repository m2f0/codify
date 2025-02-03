using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace IA_ConverterCommons;

public class DoubleBasis : StructBasis<double>
{
    private int _precision = -1;
    public int Precision
    {
        get
        {
            if (_precision != -1) return _precision;

            _precision = DataTypeModel.GetDataType(Pic.FullPic).Precision;

            return _precision;
        }
    }

    public bool IsPrecisionValue => Value.ToString().Contains(".") || Value.ToString().Contains(",");
    public bool IsNegative => Value < 0;

    //NUNCA USAR
    public DoubleBasis() : base(new PIC("", "", ""), 0) { }

    public DoubleBasis(PIC picAttr, int precision, double value = 0)
        : base(picAttr, value)
    {
        //Precision = precision;
    }

    //public static implicit operator DoubleBasis(double value)
    //{
    //    return new DoubleBasis(Pic, value);
    //}

    public static implicit operator double(DoubleBasis doubleBasis)
    {
        return doubleBasis.Value;
    }

    public static double operator /(DoubleBasis basis, int comparacao)
    {
        var basisVal = basis.ToString();
        basisVal = basisVal.Replace(".", ",");

        double.TryParse(basisVal, out var pBVal);
        return (pBVal / comparacao);
    }

    public static double operator -(DoubleBasis basis, DoubleBasis comparacao)
    {
        return basis.Value - comparacao.Value;
    }

    public static double operator +(DoubleBasis basis, DoubleBasis comparacao)
    {
        return basis.Value + comparacao.Value;
    }

    public override string? ToString()
    {
        var value = "";
        value = Value.ToString();
        if (IsNegative)
            value = value.Replace("-", "0");

        var splitedSeparator = value.Split(new[] { ',', '.' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var beforeSeparator = string.Join("", splitedSeparator.SkipLast(1));
        var afterSeparator = splitedSeparator?.LastOrDefault();
        if (string.IsNullOrEmpty(beforeSeparator))
        {
            beforeSeparator = afterSeparator;
            afterSeparator = "";
        }
        var currLeng = Pic?.Length ?? afterSeparator.Length;
        value = beforeSeparator.PadLeft(currLeng, '0').Substring(0, currLeng) + (Precision > 0 ? "." : "") + (afterSeparator ?? "").PadRight(Precision, '0').Substring(0, Precision);

        if (IsNegative)
            value = "-" + value.Substring(1);

        if (value.Length != (Pic?.Length + Precision + (Precision > 0 ? 1 : 0)))
            throw new ValidationException("no ToString só é permitido o 'MAX' do tamanho, não deve ser diferente ");

        return value;
    }
}
