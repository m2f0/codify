using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IA_ConverterCommons;

public class GoBack : Exception
{
    public GoBack() : base("Programa Finalizado com sucesso")
    {
    }
}

public class GoToException : Exception
{
    public GoToException() : base("Esta linha não pode ser executada, após o GOTO o programa toma 'outro rumo'")
    {
    }
}
