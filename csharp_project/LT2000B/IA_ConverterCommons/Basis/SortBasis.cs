using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IA_ConverterCommons;

public class SortBasis<T> where T : VarBasis
{
    public T FileLayout { get; set; }
    private int CurrentItem { get; set; } = 0;
    private FileBasis File { get; set; }
    private List<T> AllLines { get; set; } = new List<T>();
    private bool IsOpen { get; set; } = false;

    public SortBasis(T newType)
    {
        FileLayout = newType;
        var len = FileLayout.GetMoveValues().Length.ToString();
        File = new FileBasis(new PIC("X", len, $"X({len})"));
    }

    public void Return(T register, Action atEnd)
    {
        var element = AllLines?.ElementAtOrDefault(CurrentItem++);

        if (register is not null && element is not null)
            Statements.Move(element, register);

        if (atEnd != null && element is null)
            atEnd.Invoke();

        if (element is null)
            throw new GoToException();
    }

    public void SetFile(string fileName) => File.SetFile(fileName);

    public int Sort(string orderBy, Action input, Action output)
    {
        var ret = 0;

        try { input.Invoke(); } catch (GoToException) { ret = 1; }

        var orderKeys = orderBy.Replace("-", "_").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        IOrderedEnumerable<T> order = null;

        try
        {
            foreach (var item in orderKeys)
            {
                if (item == orderKeys.FirstOrDefault())
                {
                    //var it = AllLines[0];
                    //var tp = it.GetType();
                    //var prop = tp.GetProperty(item);

                    order = AllLines.OrderBy(x => x.GetType().GetProperty(item));
                }
                else
                    order = order.ThenBy(x => x.GetType().GetProperty(item));
            }

            AllLines = order.ToList();
        }
        catch (Exception)
        {
            ret = 2;
        }

        try { output.Invoke(); } catch (GoToException) { ret = 3; }

        return ret;
    }

    public void Release(T register)
    {
        if (register is null) return;

        T pass = Activator.CreateInstance<T>();
        Statements.Move(register, pass);
        AllLines.Add(pass);
    }
}