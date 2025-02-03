using Newtonsoft.Json.Linq;
using System;
using System.Text.Json.Serialization;

namespace IA_ConverterCommons;

public abstract class StructBasis<T> : VarBasis
{
    //private static bool firstRedefines = false;

    private T? _value { get; set; }
    public T? Value
    {
        get => _value;
        set
        {
            _value = value;

            OnValueChanged();

            //if (!firstRedefines) {
            //    firstRedefines = true;
            //    OnRedefinesChanged(value.ToString());
            //    firstRedefines = false;
            //}
        }
    }

    [JsonIgnore]
    public PIC? Pic { get; set; }

    public StructBasis(PIC pic, T value)
    {
        Pic = pic;
        _value = value;
    }

    public bool IsSuccess()
    {
        if (typeof(long) == typeof(T) && _value != null)
            return _value as long? == 0;

        return false;
    }

    //public void Add(int count)
    //{
    //    if (typeof(long) == typeof(T) && Value != null)
    //        Value = (T?)(object)(Convert.ToInt64(Value) + count);
    //}

    public bool IsEmpty()
    {
        return PaddedValue(false) == PaddedValue(true);
    }

    public bool IsNumeric()
    {
        return !string.IsNullOrWhiteSpace(_value?.ToString())
            && (
                Int64.TryParse(_value.ToString(), out var pInt64) ||
                double.TryParse(_value.ToString(), out var pDbl) ||
                float.TryParse(_value.ToString(), out var pFloat) ||
                decimal.TryParse(_value.ToString(), out var pDec)
            );
    }

    public string? PaddedValue(bool considerEmpty = false)
    {
        var paddingLeft = false;
        var paddingChar = ' ';
        var precision = 0;
        if (this is DoubleBasis)
        {
            precision = ((DoubleBasis)(VarBasis)this).Precision;
        }

        if (typeof(string) != typeof(T))
        {
            paddingLeft = true;
            paddingChar = '0';
        }

        var value = _value?.ToString();
        if (value == null || considerEmpty)
            value = "";

        if (paddingLeft)
        {
            var isMinus = false;
            var isPlus = false;

            if (value.Contains("-") && paddingChar == '0')
            {
                isMinus = true;
                value = value.Replace("-", "");
            }

            if (value.Contains("+") && paddingChar == '0')
            {
                isPlus = true;
                value = value.Replace("+", "");
            }

            var ret = value?
                    .PadLeft((Pic?.Length + precision) ?? value.Length, paddingChar);

            if (isMinus)
                ret = "-" + ret.Substring(1, ret.Length - 1);

            if (isPlus)
                ret = "+" + ret.Substring(1, ret.Length - 1);

            return ret;
        }
        else
            return value?
                    .PadRight((Pic?.Length + precision) ?? value.Length, paddingChar);
    }

    public StringBasis Substring(long initial, long length)
    {
        if (!int.TryParse(initial.ToString(), out var pIni) || !int.TryParse(length.ToString(), out var pLen))
            throw new Exception("Erro de conversão de dados, Substring em long error.");

        return Substring(pIni, pLen);
    }

    public StringBasis Substring(int initial, int length)
    {
        var paddingChar = ' ';
        if (typeof(string) != typeof(T))
            paddingChar = '0';

        var value = _value?.ToString();
        if (value == null)
            value = "";

        var ret = value
                .ToString()
                .PadRight(Pic?.Length ?? value.Length, paddingChar)
                .Substring(initial - 1, length);

        var t = new StringBasis(new PIC(Pic.CobolType, Pic.CobolLength, $"{Pic.CobolType}({Pic.CobolLength})"), ret);
        return t;
    }

    public void SetValue(T value) => Value = value;

    public void SetValue(string value)
    {
        if (typeof(T) == typeof(double))
            SetValue((T)(object)Int64.Parse(value));

        if (typeof(T) == typeof(double))
            SetValue((T)(object)double.Parse(value));

        if (typeof(T) == typeof(string))
            SetValue((T)(object)value?.ToString());
    }
}

//TODO: todos os métodos genéricos devem ficar em VARBASIS, pois todas as variaveis de cobol podem testar EMPTY ou LOWVALUES