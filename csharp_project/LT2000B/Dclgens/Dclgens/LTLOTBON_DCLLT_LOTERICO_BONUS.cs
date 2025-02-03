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
    public class LTLOTBON_DCLLT_LOTERICO_BONUS : VarBasis
    {
        /*" 10 LTLOTBON-NUM-LOTERICO  PIC S9(9) USAGE COMP.*/
        public IntBasis LTLOTBON_NUM_LOTERICO { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 LTLOTBON-COD-BONUS   PIC S9(4) USAGE COMP.*/
        public IntBasis LTLOTBON_COD_BONUS { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*"*/
    }
}