using System;
using IA_ConverterCommons;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Linq;
using _ = IA_ConverterCommons.Statements;
using DB = IA_ConverterCommons.DatabaseBasis;

namespace Dclgens
{
    public class LTSOLPAR_LTSOLPAR_PARAM_CHAR05 : VarBasis
    {
        /*"  49 LTSOLPAR-PARAM-CHAR05-LEN          PIC S9(4) USAGE COMP.*/
        public IntBasis LTSOLPAR_PARAM_CHAR05_LEN { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*"  49 LTSOLPAR-PARAM-CHAR05-TEXT          PIC X(200).*/
        public StringBasis LTSOLPAR_PARAM_CHAR05_TEXT { get; set; } = new StringBasis(new PIC("X", "200", "X(200)."), @"");
    }
}