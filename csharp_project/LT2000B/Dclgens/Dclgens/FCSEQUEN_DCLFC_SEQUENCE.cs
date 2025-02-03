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
    public class FCSEQUEN_DCLFC_SEQUENCE : VarBasis
    {
        /*" 10 FCSEQUEN-COD-SEQ     PIC X(5).*/
        public StringBasis FCSEQUEN_COD_SEQ { get; set; } = new StringBasis(new PIC("X", "5", "X(5)."), @"");
        /*" 10 FCSEQUEN-NUM-SEQ     PIC S9(9) USAGE COMP.*/
        public IntBasis FCSEQUEN_NUM_SEQ { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 FCSEQUEN-DES-SEQUENCIA  PIC X(60).*/
        public StringBasis FCSEQUEN_DES_SEQUENCIA { get; set; } = new StringBasis(new PIC("X", "60", "X(60)."), @"");
        /*"*/
    }
}