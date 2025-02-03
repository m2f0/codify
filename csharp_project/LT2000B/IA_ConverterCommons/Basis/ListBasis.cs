using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace IA_ConverterCommons;



public class ListBasis<T, TT> : VarBasis where T : StructBasis<TT>, new()
{
    public int Times { get; set; } = 0;
    public ReadOnlyCollection<T> Items { get; set; } = new ReadOnlyCollection<T>(new List<T>());
    public PIC? Pic { get; set; }

    public T this[int index]
    {
        get
        {
            var idx = index - 1;
            if (idx < 0 || idx >= Items.Count)
                //throw new IndexOutOfRangeException();
                return new T();

            return Items[idx];
        }
    }

    public T this[IntBasis index]
    {
        get
        {
            if (!int.TryParse(index.ToString(), out var intP))
                throw new IndexOutOfRangeException();

            return this[intP];
        }
    }

    public ListBasis(PIC picAttr, int times, TT defValue = default)
    {
        Pic = picAttr;
        Times = times;

        var initialListValues = new List<T>();
        for (int i = 0; i < times; i++)
        {
            var t = new T();
            t.Pic = picAttr;
            t.ValueChanged += OnValueChanged;
            initialListValues.Add(t);
        }

        if (defValue != null)
            initialListValues.ForEach(x => { x.SetValue(defValue); });

        Items = new ReadOnlyCollection<T>(initialListValues);
    }

    public override string? ToString()
    {
        return string.Join("", Items.Select(x => x.GetMoveValues()));
    }
}

public class ListBasis<T> : VarBasis where T : VarBasis, new()
{
    public int Times { get; set; } = 0;
    public ReadOnlyCollection<T> Items { get; set; } = new ReadOnlyCollection<T>(new List<T>());

    public T this[int index]
    {
        get
        {
            index = index - 1;
            if (index < 0 || index >= Items.Count)
            {
                T fakeItem = Activator.CreateInstance<T>();
                return fakeItem;

                //throw new IndexOutOfRangeException();
            }

            return Items[index];
        }
    }

    public T this[IntBasis index]
    {
        get
        {
            if (!int.TryParse(index.Value.ToString(), out var intP))
                throw new IndexOutOfRangeException();

            return this[intP];
        }
    }

    public ListBasis(int times)
    {
        Times = times;

        var initialListValues = new List<T>();
        for (int i = 0; i < times; i++)
            initialListValues.Add(new T());

        Items = new ReadOnlyCollection<T>(initialListValues);
    }

    public override string? ToString()
    {
        return string.Join("", Items.Select(x => x.GetMoveValues()));
    }
}
