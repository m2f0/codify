using System.Diagnostics;
using System.Reflection;
using _ = IA_ConverterCommons.Statements;
using System.Globalization;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Numerics;
using System.ComponentModel;

namespace IA_ConverterCommons;

//BaseCopyClass
public class VarBasis
{
    public delegate void ValueChangedEventHandler();
    public bool IsHighValues { get; set; }

    public event ValueChangedEventHandler ValueChanged;

    public void OnValueChanged()
    {
        ValueChanged?.Invoke();
    }

    public static List<Type> VarBasysCommomTypes = new List<Type>
    {
        typeof(IntBasis),
        typeof(DoubleBasis),
        typeof(StringBasis),
        typeof(SelectorBasis),
    };
    /*
        public static implicit operator long(VarBasis basis)
        {
            var basisVal = basis.ToString();
            string testVar = basisVal;
            if (basisVal.Contains(".") || basisVal.Contains(","))
                testVar = string.Join("", basisVal.Split(new[] { ',', '.' }).SkipLast(1));

            return long.TryParse(testVar, out var pBasis) ? pBasis : -9999;
        }
    */
    public static implicit operator long(VarBasis basis)
    {
        var basisVal = basis.ToString();
        if (basis is SelectorBasis)
        {
            basisVal = ((SelectorBasis)basis).Value;
        }

        string testVar = basisVal;
        if (basisVal.Contains(".") || basisVal.Contains(","))
            testVar = string.Join("", basisVal.Split(new[] { ',', '.' }).SkipLast(1));

        return long.TryParse(testVar, out var pBasis) ? pBasis : -9999;
    }

    public static bool operator ==(VarBasis basis, VarBasis comparacao)
    {
        var isNumericBasis = basis is IntBasis || basis is DoubleBasis;
        var isNumericCompare = comparacao is IntBasis || comparacao is DoubleBasis;
        var basisVal = basis?.ToString();
        var compareVal = comparacao?.ToString();

        if (isNumericBasis && isNumericCompare)
        {
            basisVal = basisVal.Replace(".", ",");
            compareVal = compareVal.Replace(".", ",");

            return double.TryParse(basisVal, out var pBVal)
                && double.TryParse(compareVal, out var pCVal)
                && pBVal == pCVal;
        }
        else if (basis is SelectorBasis)
        {
            var selectorBasis = (SelectorBasis)basis;
            return selectorBasis == compareVal;
        }
        else if (basis is StringBasis)
        {
            return basisVal == compareVal;
        }
        else
            return basis?.GetMoveValues() == comparacao?.GetMoveValues();
    }

    public static bool operator !=(VarBasis basis, VarBasis comparacao)
    {
        return !(basis == comparacao);
    }

    //public static bool operator ==(VarBasis basis, int comparacao)
    //{
    //    return int.TryParse(basis.GetMoveValues(), out var pBasis) && pBasis == comparacao;
    //}

    //public static bool operator !=(VarBasis basis, int comparacao)
    //{
    //    return !(basis == comparacao);
    //}

    //public static bool operator ==(VarBasis basis, int comparacao)
    //{
    //    var isInt = int.TryParse(basis.GetMoveValues(), out var pBasis);
    //    return isInt ? pBasis == comparacao : false;
    //}

    //public static bool operator !=(VarBasis basis, int comparacao)
    //{
    //    var isInt = int.TryParse(basis.GetMoveValues(), out var pBasis);
    //    return isInt ? pBasis != comparacao : false;
    //}

    public static bool operator <(VarBasis basis, VarBasis compare)
    {
        //vamos testar se é comparação de strings provaveis datas
        var basisData = basis.GetMoveValues();
        var compareData = compare.GetMoveValues();

        var isBasisDate = DateTime.TryParse(basisData, out var pDateBasis);
        var isCompareDate = DateTime.TryParse(compareData, out var pDateCompare);

        if (isBasisDate && isCompareDate)
            return pDateBasis < pDateCompare;

        var isBasisNumber = long.TryParse(basisData, out var pNumberBasis);
        var isCompareNumber = long.TryParse(compareData, out var pNumberCompare);

        if (isBasisNumber && isCompareNumber)
            return pNumberBasis < pNumberCompare;

        return basisData?.Length < compareData?.Length;
    }

    public static bool operator >(VarBasis basis, VarBasis compare)
    {
        //vamos testar se é comparação de strings provaveis datas
        var basisData = basis.GetMoveValues();
        var compareData = compare.GetMoveValues();

        var isBasisDate = DateTime.TryParse(basisData, out var pDateBasis);
        var isCompareDate = DateTime.TryParse(compareData, out var pDateCompare);

        if (isBasisDate && isCompareDate)
            return pDateBasis > pDateCompare;

        var isBasisNumber = long.TryParse(basisData, out var pNumberBasis);
        var isCompareNumber = long.TryParse(compareData, out var pNumberCompare);

        if (isBasisNumber && isCompareNumber)
            return pNumberBasis > pNumberCompare;

        return basisData?.Length > compareData?.Length;
    }

    public static bool operator <(VarBasis basis, int compare)
    {
        //vamos testar se é comparação de strings provaveis datas
        var basisData = basis.GetMoveValues();
        var compareData = compare;

        var isBasisNumber = long.TryParse(basisData, out var pNumberBasis);

        if (isBasisNumber)
            return pNumberBasis < compare;

        return basisData?.Length < compareData;
    }

    public static bool operator >(VarBasis basis, int compare)
    {
        //vamos testar se é comparação de strings provaveis datas
        var basisData = basis.ToString(); //usado ToString para DoubleBasis o "." sumia
        var compareData = compare;

        if (basis is DoubleBasis)
        {
            if (double.TryParse(basisData.Replace(".", ","), out var pNumberBasis))
                return pNumberBasis > compare;
        }
        else
        {
            var isBasisNumber = long.TryParse(basisData, out var pNumberBasis);

            if (isBasisNumber == true)
                return pNumberBasis > compare;
        }

        return basisData?.Length > compareData;
    }

    #region PUBLIC METHODS


    public delegate void RedefinesDelegateEventHandler(string? newValue);

    public event RedefinesDelegateEventHandler Redefines;

    protected virtual void OnRedefinesChanged(string? newValue)
    {
        Redefines?.Invoke(newValue);
    }

    public static void RedefinePassValue(object value, VarBasis redefines, VarBasis origem)
    {
        void HandleRedefinesChanged(string? newValue) => _.Move(newValue, origem);

        if (redefines.Redefines != null)
            redefines.Redefines -= HandleRedefinesChanged;

        if (value.ToString() != origem.ToString())
            _.Move(value, origem);

        redefines.Redefines += HandleRedefinesChanged;
    }

    public static void Display(string fullValues)
    {
        Console.WriteLine(fullValues);
        Debug.WriteLine(fullValues);
    }

    public string Display()
    {
        var fullValues = GetMoveValues();
        Display(fullValues);
        return fullValues;
    }

    public StringBasis Substring(int initial, int length)
    {
        var fullValues = GetMoveValues();
        var sBasis = new StringBasis(new PIC("X", $"{length}", $"X({length})"), fullValues.Substring(initial - 1, length));
        return sBasis;
    }

    public void Initialize()
    {
        PIC? pic = null;
        var value = string.Empty;
        var currentType = this.GetType();

        if (currentType == typeof(IntBasis))
        {
            pic = ((IntBasis)this).Pic;
            value = GetSecureValue(new string('0', pic.Length), currentType, pic.Length, 0);
            value = value.Substring(0, pic.Length);
            ((IntBasis)this).SetValue(Int64.TryParse(value, out var resInt) ? resInt : 0);
            return;
        }

        if (currentType == typeof(DoubleBasis))
        {
            var me = ((DoubleBasis)this);
            pic = me.Pic;

            value = GetSecureValue(new string('0', pic.Length), currentType, pic.Length, me.Precision);
            value = value.Substring(0, pic.Length);
            ((DoubleBasis)this).SetValue(double.TryParse(value, out var resInt) ? resInt : 0);
            return;
        }

        if (currentType == typeof(StringBasis))
        {
            pic = ((StringBasis)this).Pic;
            value = GetSecureValue(new string(' ', pic.Length), currentType, pic.Length, 0);
            value = value.Substring(0, pic.Length);
            ((StringBasis)this).SetValue(value);
            return;
        }

        if (currentType == typeof(SelectorBasis))
        {
            var selB = ((SelectorBasis)this);
            pic = selB.Pic;
            value = GetSecureValue(selB.Items?.FirstOrDefault()?.Value ?? "", currentType, pic.Length, 0, selB.Items);

            var firstIndex = Math.Max(0, value.Length - pic.Length);

            int substringLength = Math.Min(pic.Length, value.Length - firstIndex);

            value = value.Substring(firstIndex, substringLength);

            ((SelectorBasis)this).SetValue(value);
            return;
        }


        foreach (var property in this.GetType().GetProperties())
        {
            // Verificar se a propriedade é gravável
            if (property.CanWrite)
            {
                var innerValue = property.GetValue(this);
                if (innerValue != null && innerValue is VarBasis)
                    ((VarBasis)innerValue).Initialize();
            }
        }
    }

    /// <summary>
    /// Este método serve para obter o valor seguro a partir de um valor qualquer,
    /// aqui se usa da esquerda para a direita idependente do tipo de variavel
    /// </summary>
    private string GetSecureValue(string value, Type type, int length, int precision, List<SelectorItemBasis> selectorItens = null)
    {
        //var currentType = this.GetType();
        //var isPicture =
        //       typeof(IntBasis) == type
        //    || typeof(StringBasis) == type
        //    || typeof(DoubleBasis) == type

        //var type = pic.GetType();   
        if (type == typeof(StringBasis))
            return value?
                .ToString()?
                .PadRight(length, ' ')
                .Substring(0, length);

        if (
           type == typeof(IntBasis)
        //|| type == typeof(DoubleBasis)
        //||  type == typeof(float) 
        //||  type == typeof(decimal)
        )

        {
            var isNegative = false;

            if (!string.IsNullOrEmpty(value))
                isNegative = long.TryParse(value, out var pValue) ? pValue < 0 : false;

            if (isNegative)
                value = value?.Replace("-", "");

            var negativeLength = length - (isNegative ? 1 : 0);
            var paddedValue = $"{(isNegative ? "-" : "")}" +
                    value?
                    .ToString()?
                    .Replace(".", "")?
                    .Replace(",", "")?
                    .PadLeft(negativeLength, '0');

            return paddedValue
                    .Substring(0, length);
        }

        if (
           type == typeof(DoubleBasis)
        //||  type == typeof(float) 
        //||  type == typeof(decimal)
        )
        {
            var isDecimal = precision > 0;
            var lengthToTest = length + precision;
            var localValue = ((value.Length > lengthToTest) ? value.Substring(0, lengthToTest) : value).ToString();
            var cosiderWithDot = localValue.Contains(".") || localValue.Contains(",");
            var isNumber = long.TryParse(localValue, out var isNP);

            if (cosiderWithDot && isNumber)
            {
                lengthToTest++;
                localValue = ((value.Length > lengthToTest) ? value.Substring(value.Length - lengthToTest, lengthToTest) : value).ToString();
            }

            var isMinus = localValue?.Contains("-") == true;
            localValue = localValue.Replace("-", "0");
            var splitedVal = localValue.Split(new[] { ',', '.' });
            var decVal = "";
            var undecVal = "";
            var indexOfSubstring = localValue.Length - (length + precision + (cosiderWithDot ? 1 : 0));

            if (cosiderWithDot && isNumber)
            {
                //pega as casas decimais
                decVal = splitedVal.LastOrDefault();
                if (!string.IsNullOrEmpty(decVal))
                    decVal = double.TryParse($",{decVal}", out var pDecVal) ? pDecVal.ToString().Replace("0,", "") : decVal;


                decVal = decVal.PadRight(precision, '0');
                undecVal = string.Join("", splitedVal.SkipLast(1)).PadLeft(length, '0');
            }
            else
            {
                undecVal = localValue.PadLeft(length, '0').Substring(0, length);
                var localDec = "";
                if (localValue.Length > length)
                {
                    localDec = localValue.Substring(length, localValue.Length - length);
                }
                decVal = localDec.PadLeft(precision, '0').Substring(0, precision);
            }

            localValue = undecVal + decVal;

            if (indexOfSubstring < 0) indexOfSubstring = 0;

            localValue = localValue.Substring(indexOfSubstring);

            //vamos chegar negativo para adicionar o padding
            if (isMinus && isNumber)
                localValue = "-" + localValue.Substring(1);

            if (localValue.Length != (length + precision))
                throw new ValidationException("Campo deve conter o valor correto, aconteceu algum erro de conversão");

            value = localValue;
        }

        if (
           type == typeof(SelectorBasis))
        {
            var isInteger = int.TryParse(value, out var tParseSel);
            if (isInteger)
            {
                value = (selectorItens.FirstOrDefault(x => int.TryParse(x.Value, out var tVal) && tVal == tParseSel)?.Value ?? value).PadLeft(length, '0');
            }
            else
            {
                var compareVal = value.Trim().PadRight(length, ' ').Substring(0, length).Trim();
                value = (selectorItens.FirstOrDefault(x => x.Name.Trim() == compareVal || x.Value.Trim() == compareVal)?.Value ?? value).PadRight(length, ' ');
            }
        }

        return value;
    }

    public string SetMoveValues(object from, string localStringValues, bool? isToConsiderMoveFromLeftToRigth = null)
    {
        var currentType = this.GetType();
        var isPicture = VarBasysCommomTypes.Contains(currentType);

        if (isToConsiderMoveFromLeftToRigth == null && !isPicture)
            isToConsiderMoveFromLeftToRigth = true;

        if (isPicture)
        {
            PIC? pic = null;
            PIC? fromPic = null;
            var value = string.Empty;
            var precision = 0;
            var fromPrecision = 0;
            var localStrLength = localStringValues?.Length ?? 0;

            if (from.GetType() == typeof(IntBasis))
                fromPic = ((IntBasis?)from)?.Pic;

            if (from.GetType() == typeof(DoubleBasis))
            {
                fromPic = ((DoubleBasis?)from)?.Pic;
                fromPrecision = ((DoubleBasis?)from)?.Precision ?? 0;
            }

            if (currentType == typeof(IntBasis))
            {
                dynamic me = null;
                me = ((IntBasis)this);
                pic = me.Pic;

                //TODO:> considerando apenas o int
                //teste de valor vazio, neste caso retorna apenas o valor default
                if (localStrLength == 0)
                    value = GetSecureValue(localStringValues ?? "", currentType, pic.Length, precision);
                else if (fromPic == null)
                {
                    //#V1
                    //se o fromPic for null, significa que o valor passado é um inteiro ou uma string ** MOVE '' ou MOVE 0
                    var fromLen = Math.Min(pic.Length, localStringValues?.Length ?? 0);
                    value = localStringValues.Substring(0, fromLen);
                    //#V1
                }
                else
                {
                    //#V1
                    //se o fromPic for null, significa que o valor passado é um inteiro ou uma string ** MOVE '' ou MOVE 0
                    //temos problema quando chega "01012020" em um PIC de 2 com left true
                    //temos problema quando chega "01" em um pic de 10
                    var fromLen = Math.Min(pic.Length, fromPic?.Length ?? 0);
                    if (fromLen > localStringValues?.Length)
                        fromLen = localStringValues.Length;

                    var dif = (fromPic?.Length - pic.Length) ?? 0;
                    if (dif < 0)
                        dif = 0;

                    var firstIdx = 0;
                    if (isToConsiderMoveFromLeftToRigth != true)
                        firstIdx = dif;

                    value = localStringValues.Substring(firstIdx, fromLen);
                    //#V1
                }

                if (DateTime.TryParseExact(value, Statements.NormalDatabaseDate, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var pDate))
                    value = pDate.ToString(Statements.NormalDatabaseDate.Replace(" ", "").Replace(":", "").Replace("-", "").Replace("/", ""));

                me.SetValue(Int64.TryParse(value, out var resInt) ? resInt : 0);
            }


            if (currentType == typeof(DoubleBasis))
            {
                dynamic me = null;
                var isDouble = currentType == typeof(DoubleBasis);

                me = ((DoubleBasis)this);
                precision = me.Precision;
                pic = me.Pic;

                //TODO:> considerando apenas o int
                //teste de valor vazio, neste caso retorna apenas o valor default
                if (localStrLength == 0)
                    value = GetSecureValue(localStringValues ?? "", currentType, pic.Length, precision);
                else
                {
                    //se o fromPic for null, significa que o valor passado é um inteiro ou uma string ** MOVE '' ou MOVE 0
                    //dando foco sempre no fromPic, de forma que o valor nunca estoure a tring atual
                    var fromLen = fromPic?.Length ?? pic.Length;
                    value = GetSecureValue(localStringValues ?? "", currentType, fromLen, precision);
                    value = value.Insert(value.Length - precision, ",");
                }

                /*
                var lengthToSendGreater = pic.Length;//Math.Max(pic.Length, fromPic?.Length ?? pic.Length);
                value = GetSecureValue(localStringValues, currentType, lengthToSendGreater, precision);

                var dif = (fromPic?.Length - pic.Length) ?? 0;
                if (dif < 0)
                    dif = 0;


                //para o considerLeft funcionar, foi necessário colocar as variaveis em modelo flutuantes e declaradas, por isso o FirstI e LastI
                var firstIndex = dif;
                var lastIndex = pic.Length + precision;
                if (isToConsiderMoveFromLeftToRigth == true)
                {
                    if (dif > 0)
                        firstIndex = 0;
                }
                else
                {
                    if (dif > 0)
                        lastIndex -= dif;
                }

                value = value.Substring(firstIndex, lastIndex);
                */

                me.SetValue(double.TryParse(value, out double resDoub) ? resDoub : 0);
                //me.SetValue(double.TryParse(value.Insert(value.Length - precision, ","), out double resDoub) ? resDoub : 0);
            }


            if (currentType == typeof(StringBasis))
            {
                pic = ((StringBasis)this).Pic;
                value = GetSecureValue(localStringValues, currentType, pic.Length, 0);
                value = value.Substring(0, pic.Length);
                ((StringBasis)this).SetValue(value);
            }

            if (currentType == typeof(SelectorBasis))
            {
                pic = ((SelectorBasis)this).Pic;
                value = GetSecureValue(localStringValues, currentType, pic.Length, 0, ((SelectorBasis)this).Items);

                var firstIndex = Math.Max(0, value.Length - pic.Length);

                int substringLength = Math.Min(pic.Length, value.Length - firstIndex);

                value = value.Substring(firstIndex, substringLength);

                ((SelectorBasis)this).SetValue(value);
            }

            //localStringValues é o que sobra para a proxima interação, essa string vai sendo "cortada" até o fim
            if (localStringValues?.Length >= (pic.Length + precision))
                //a ordem foi alterada em 28-08-2024, caso de problema verificar
                //localStringValues = localStringValues.Substring((fromPic?.Length ?? pic.Length) + precision);
                localStringValues = localStringValues.Substring(((pic?.Length ?? fromPic?.Length) ?? 0) + precision);
            else
                localStringValues = string.Empty;

            return localStringValues;
        }

        OnRedefinesChanged(localStringValues);


        //para pegar valores dentro de redefines e classe dentro de classe
        if (currentType.Name.Contains("ListBas"))
        {
            var items = ((dynamic)this).Items;
            ////var items = property.GetValue(this).GetType().GetProperties();
            //var items = (ICollection<VarBasis>)propertyValue;
            foreach (var item in items)
            {
                if (item is VarBasis)
                {
                    localStringValues = ((VarBasis)item).SetMoveValues(from, localStringValues, isToConsiderMoveFromLeftToRigth);
                }
            }
        }

        foreach (var property in currentType.GetProperties())
        {
            if (property.PropertyType.Name.Contains("_REDEF_")) continue;
            if (currentType.Name.Equals("SelectorBasis") && (property.Name.Equals("Items") || property.Name.Equals("Item"))) continue;

            if (property.PropertyType.Name.Contains("ListBas"))
            {
                var propertyValue = property.GetValue(this);
                var items = ((dynamic)propertyValue).Items;
                foreach (var item in items)
                {
                    if (item is VarBasis)
                    {
                        localStringValues = ((VarBasis)item).SetMoveValues(from, localStringValues, isToConsiderMoveFromLeftToRigth);
                    }
                }

                //fullValues += ret;

                continue;
            }

            // Verificar se a propriedade é gravável
            if (property.CanWrite)
            {
                var innerValue = property.GetValue(this);
                if (innerValue != null && innerValue is VarBasis)
                    localStringValues = ((VarBasis)innerValue).SetMoveValues(from, localStringValues, isToConsiderMoveFromLeftToRigth);
            }
        }

        return localStringValues;
    }

    public string GetMoveValues()
    {
        var fullValues = string.Empty;
        var type = this.GetType();
        PIC? pic = null;

        if (VarBasysCommomTypes.Contains(type))
        {
            pic = (PIC)((dynamic)this).Pic;
            var precision = 0;
            if (type == typeof(DoubleBasis))
                precision = ((DoubleBasis)this).Precision;

            var passValue = this.ToString();
            List<SelectorItemBasis> basisItens = null;
            if (type == typeof(SelectorBasis))
            {
                passValue = ((SelectorBasis)this).Value;
                basisItens = ((SelectorBasis)this).Items;
            }

            var value = GetSecureValue(passValue, type, pic.Length, precision, basisItens);

            //try
            //{
            //    if (type == typeof(IntBasis))
            //    {
            //        var me = ((IntBasis)this);
            //        if (me.IsNegative && )
            //            value = $"-{value.Replace("-", "0")}";
            //    }
            //}
            //catch { }

            return value;
        }

        foreach (var property in this.GetType().GetProperties())
        {
            if (property.PropertyType.Name.Contains("ReadOnlyCollection") && (property.Name == "Items")) continue;
            if (property.PropertyType.Name.Contains("Int32") && (property.Name == "Times")) continue;
            if (property.PropertyType.Name.Contains("Boolean") && (property.Name == "IsHighValues")) continue;
            if (property.DeclaringType.Name.Contains("ListBas") && property.Name == "Item") continue;
            if (property.PropertyType.Name.Contains("_REDEF_")) continue;
            if (property.PropertyType.Name.Contains("ListBas"))
            {
                var propertyValue = property.GetValue(this);
                var items = ((dynamic)propertyValue).Items;
                var ret = "";
                foreach (var item in items)
                {
                    if (item is VarBasis)
                        ret += ((VarBasis)item).GetMoveValues();

                }

                fullValues += ret;

                continue;
            }

            // Verificar se a propriedade é gravável
            if (property.CanRead)
            {
                var value = GetProperValue(property);
                fullValues += value;
            }
        }

        return fullValues;
    }

    public bool IsLowValues()
    {
        var fullValue = GetMoveValues();

        var fullCompare = string.Empty;
        var type = this.GetType();
        PIC? pic = null;

        if (VarBasysCommomTypes.Contains(type))
        {
            pic = (PIC)((dynamic)this).Pic;

            var precision = 0;
            if (type == typeof(DoubleBasis))
                precision = ((DoubleBasis)this).Precision;

            fullCompare = GetSecureValue("", type, pic.Length, precision);
            return fullCompare == fullValue;
        }

        foreach (var property in this.GetType().GetProperties())
        {
            // Verificar se a propriedade é gravável
            if (property.CanRead)
            {
                var value = GetProperValue(property);
                fullCompare += value;
            }
        }

        return fullCompare == fullValue;
    }

    public bool All(string all)
    {
        return GetMoveValues().All(x => x.ToString() == all);
    }

    public void Trim()
    {
        var getMoveTrimmed = GetMoveValues().Trim();
        SetMoveValues(null, getMoveTrimmed);
    }

    public bool In(params string[] args)
    {
        var value = GetMoveValues().Trim();
        var isInt = int.TryParse(value, out var pVal);
        if (isInt)
            return args.Any(x => int.TryParse(x, out var pX) && pX == pVal);

        return args.Any(x => x.Trim() == value.Trim());
    }
    public bool In(params Double[] args)
    {
        if (!Double.TryParse(GetMoveValues().Trim(), out var pVal)) return false;

        return args.ToList().Contains(pVal);
    }

    public void MoveCorr(VarBasis from, VarBasis to)
    {
        var fromList = GetValidProperties(from);
        var toList = GetValidProperties(to);

        foreach (var fromItem in fromList)
        {
            foreach (var toItem in toList)
            {
                if (!IsCompatible(fromItem, toItem)) continue;

                if (VarBasysCommomTypes.Contains(fromItem.PropertyType) && VarBasysCommomTypes.Contains(toItem.PropertyType))
                {
                    var fromVal = ((dynamic)fromItem.GetValue(this)).Value;
                    ((dynamic)toItem.GetValue(to)).Value = fromVal;
                }
            }
        }
    }

    public void MoveAll(string param)
    {
        var localPar = param.Replace("'", "").Replace("SPACES", " ");
        //var toList = new List<PropertyInfo>();

        var type = GetType();
        if (VarBasysCommomTypes.Contains(type))
        {
            var length = ((dynamic)this).Pic.Length;
            var precision = 0;

            if (type == typeof(DoubleBasis))
                precision = ((dynamic)this)?.Precision ?? 0;

            var updatedValue = $"{new string(localPar.FirstOrDefault(), length)}{(precision > 0 ? $",{new string(localPar.FirstOrDefault(), precision)}" : "")}";

            _.Move(updatedValue, this);
        }
    }

    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(GetMoveValues());
    }

    public bool In(params Int64[] args)
    {
        return args.Contains(Int64.Parse(GetMoveValues()));
    }
    public bool IsNumeric()
    {
        var _value = GetMoveValues();
        return !string.IsNullOrWhiteSpace(_value?.ToString())
            && (
                Int64.TryParse(_value.ToString(), out var pInt64) ||
                double.TryParse(_value.ToString(), out var pDbl) ||
                float.TryParse(_value.ToString(), out var pFloat) ||
                decimal.TryParse(_value.ToString(), out var pDec)
            );
    }
    #endregion

    #region MOVE

    private string? GetProperValue(PropertyInfo property)
    {
        var type = property.PropertyType;
        if (property.PropertyType.Name.Contains("_REDEF_")) return "";
        var propertyValue = property.GetValue(this);
        var value = "";
        PIC? pic = null;

        if (VarBasysCommomTypes.Contains(type))
        {
            pic = ((dynamic)propertyValue).Pic;
            value = ((VarBasis)propertyValue).GetMoveValues();
            return value;
        }

        if (property.PropertyType.Name.Contains("ListBas"))
        {
            var items = ((dynamic)propertyValue).Items;
            var ret = "";
            foreach (var item in items)
            {
                if (item is VarBasis)
                    ret += ((VarBasis)item).GetMoveValues();

            }
            return ret;
        }

        //gambi para pegar valores dentro de redefines e classe dentro de classe
        if (property.ReflectedType.BaseType.Name.Contains("VarBasis"))
        {
            var items = propertyValue.GetType().GetProperties();
            var ret = "";
            foreach (var item in items)
            {
                if (item.CanRead)
                {
                    if (item.PropertyType.BaseType != typeof(VarBasis) && item.PropertyType.BaseType.BaseType != typeof(VarBasis)) continue;

                    var varVal = ((object)item.GetValue(property.GetValue(this)));
                    if (item.PropertyType.Name.Contains("ListBasis"))
                        value = varVal.ToString();
                    else
                    {
                        if (varVal.GetType().GetProperty("Pic") != null)
                            pic = ((dynamic)item.GetValue(property.GetValue(this)))?.Pic;

                        value = ((VarBasis)varVal).GetMoveValues();
                    }
                    ret += value;
                    continue;
                }
            }

            return ret;
        }

        if (property.GetType().Name == "RuntimePropertyInfo") return "";

        foreach (var proper in property.GetType().GetProperties())
        {
            // Verificar se a propriedade é gravável
            if (proper.CanRead)
            {
                var properValue = GetProperValue(proper);
                value += properValue;
            }
        }

        return value;
    }

    private bool IsCompatible(PropertyInfo from, PropertyInfo to)
    {
        //no momento da criação das variaveis, caso haja variavel identica,
        //é adicionado o _NUMERO ou seja, uma variavel chamada ANO pode estar como ANO_0 em ambos os lados da comparação
        //inclusive elas podem conter numeros diferentes, exemplo ANO_0 e ANO_1
        if (from.Name == to.Name) return true;
        if (Regex.Replace(from.Name, @"_\d+$", @"") == Regex.Replace(to.Name, @"_\d+$", @"")) return true;

        return false;
    }

    private List<PropertyInfo> GetValidProperties(VarBasis from)
    {
        var ignoreList = new List<string>
        {
            "IsHighValues"
        };

        return from
            .GetType()
            .GetProperties()
            .Where(x => !ignoreList.Contains(x.Name))
            .ToList();
    }
    #endregion
}
