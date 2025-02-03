using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using _ = IA_ConverterCommons.Statements;

namespace IA_ConverterCommons;

public class FileBasis : StructBasis<string>
{
    /// <summary>
    /// ex: c:\teste\arquivo.txt
    /// </summary>
    public string FilePath { get; set; }

    public string MemoryFile { get; set; }

    public bool IsOutputOperation { get; set; } = true;

    int CurrentPosition { get; set; } = 0;
    int CurrentLine { get; set; } = 0;
    public List<string> AllLines { get; set; } = new List<string>();

    public IntBasis FileStatus { get; set; } = new IntBasis(new PIC("9", "2", "9(2)"), 0);
    public StructBasis<string>? FileStatusReply { get; set; } = null;

    VarBasis FileRelative { get; set; }

    //NUNCA USAR
    public FileBasis() : base(new PIC("", "", ""), "") { }

    public FileBasis(PIC picAttr)
        : base(picAttr, "")
    {
    }

    public void SetFile(string fileName)
    {
        var isMemFile = string.IsNullOrEmpty(AppSettings.Settings.FileFolderPath)
                        || AppSettings.Settings.MemoryFiles.Contains(fileName);
        ;

        if (!isMemFile)
            FilePath = Path.Combine(AppSettings.Settings.FileFolderPath, fileName);
        else
        {
            //var array = Encoding.UTF8.GetBytes(fileName);
            //MemoryFile = array;
            MemoryFile = fileName;
        }
    }

    public override string? ToString()
    {
        return PaddedValue();
    }

    public void Open(VarBasis fileRelative, StructBasis<string>? fileStatusReply)
    {
        if (fileStatusReply?.Value != null)
            FileStatusReply = fileStatusReply;

        FileRelative = fileRelative;
        FileStatus.Value = 0;

        if (FileStatusReply is not null)
            FileStatusReply.Value = "00";
    }

    public void Open(VarBasis? fileRelative = null, IntBasis? fileStatus = null)
    {
        if (fileStatus?.Value is not null)
        {
            FileStatus = fileStatus;
            FileStatus.Value = 0;
        }

        if (fileRelative is not null)
            FileRelative = fileRelative;
    }

    public void Read(Action atEnd = null, Action notAtEnd = null)
    {
        FileStatus.Value = 0;

        if (FileStatusReply is not null)
            FileStatusReply.Value = "00";

        IsOutputOperation = false;
        var isMemFile = string.IsNullOrEmpty(FilePath);

        try
        {
            if (!isMemFile)
            {
                if (!AllLines.Any())
                    AllLines = File.ReadLines(FilePath).ToList();


                Value = AllLines.ElementAtOrDefault(CurrentLine++);
                if (Value == null)
                    throw new ArgumentOutOfRangeException();

            }




            //    using (var reader = new StreamReader(FilePath))
            //    {
            //        //reader.ReadToEnd.Seek(0, SeekOrigin.Begin);
            //        char[] buffer = new char[Pic.Length];
            //        reader.ReadBlock(buffer, CurrentPosition, Pic.Length);
            //        Value = string.Join("", buffer);
            //    }
            else
            {
                var lastToEnd = MemoryFile.Length - (CurrentPosition - Pic.Length + 1);
                var length = lastToEnd > 0 ? lastToEnd : Pic.Length;
                length = Math.Min(length, MemoryFile.Length);

                Value = MemoryFile.Substring(CurrentPosition, length);
            }

            if (FileRelative is not null)
                _.Move(Value, FileRelative);

            CurrentPosition += Pic.Length - 1;

            if (notAtEnd != null)
                notAtEnd.Invoke();
        }
        catch (ArgumentOutOfRangeException x)
        {
            FileStatus.Value = 10;

            if (FileStatusReply is not null)
                FileStatusReply.Value = "10";
        }
        catch
        {
            Value = "";
            FileStatus.Value = 22;

            if (FileStatusReply is not null)
            {
                FileStatusReply.Value = "22";
                if (FileStatusReply.Value == null)
                    FileStatusReply.Value = "99";
            }
        }

        //AT END
        if (FileStatus.Value == 10)
            if (atEnd != null)
                atEnd.Invoke();

    }

    public void Close()
    {
        FileStatus.Value = 0;

        if (FileStatusReply is not null)
            FileStatusReply.Value = "00";
    }

    // public void Write(string value = null)
    // {
    //     if (string.IsNullOrEmpty(value))
    //         value = FileRelative?.GetMoveValues();
    //
    //     IsOutputOperation = true;
    //
    //     var isMemFile = string.IsNullOrEmpty(FilePath);
    //
    //     if (isMemFile)
    //         using (var writer = new StreamWriter(FilePath, append: true))
    //             writer.WriteLine(value);
    //     else
    //         MemoryFile += value + Environment.NewLine;
    // }
    public void Write(string value = null)
    {
        if (string.IsNullOrEmpty(value))
            value = FileRelative?.GetMoveValues();

        IsOutputOperation = true;
        FileStatus.Value = 0;

        if (FileStatusReply is not null)
            FileStatusReply.Value = "00";

        var isMemFile = string.IsNullOrEmpty(FilePath);

        try
        {
            if (!isMemFile)
                using (var writer = new StreamWriter(FilePath, append: true))
                    writer.WriteLine(value);
            else
            {
                MemoryFile += value + Environment.NewLine;
            }

        }
        catch (ArgumentOutOfRangeException x)
        {
            FileStatus.Value = 10;

            if (FileStatusReply is not null)
                FileStatusReply.Value = "10";
        }
        catch
        {
            Value = "";
            FileStatus.Value = 22;

            if (FileStatusReply is not null)
            {
                FileStatusReply.Value = "22";
                if (FileStatusReply.Value == null)
                    FileStatusReply.Value = "99";
            }
        }


    }

    public List<string> SortFile<T>(string orderBy, T fileRegister) where T : VarBasis
    {
        var retList = new List<string>();

        var orderKeys = orderBy.Replace("-", "_").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        IOrderedEnumerable<T> order = null;

        try
        {
            List<T> fullList = AllLines.Select((x) =>
            {
                var t = Activator.CreateInstance<T>();
                _.Move(x, t);
                return t;
            }).ToList();

            foreach (var item in orderKeys)
            {
                if (item == orderKeys.FirstOrDefault())
                    order = fullList.OrderBy(x => x.GetType().GetProperty(item));
                else if (order != null)
                    order = order.ThenBy(x => x.GetType().GetProperty(item));
            }

            if (order != null)
                retList = order.Select(x => x.GetMoveValues()).ToList();
        }
        catch (Exception ex)
        {
        }

        return retList;
    }
}
