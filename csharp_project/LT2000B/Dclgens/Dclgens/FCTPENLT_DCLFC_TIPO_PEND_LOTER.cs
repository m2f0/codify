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
    public class FCTPENLT_DCLFC_TIPO_PEND_LOTER : VarBasis
    {
        /*" 10 FCTPENLT-NUM-LOTERICO  PIC S9(9) USAGE COMP.*/
        public IntBasis FCTPENLT_NUM_LOTERICO { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 FCTPENLT-COD-TIPO-PEND-LOT  PIC X(3).*/
        public StringBasis FCTPENLT_COD_TIPO_PEND_LOT { get; set; } = new StringBasis(new PIC("X", "3", "X(3)."), @"");
        /*"*/
    }
}