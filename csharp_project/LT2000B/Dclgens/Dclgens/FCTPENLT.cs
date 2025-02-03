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
    public class FCTPENLT : VarBasis
    {
        /*"01 DCLFC-TIPO-PEND-LOTER.*/
        public FCTPENLT_DCLFC_TIPO_PEND_LOTER DCLFC_TIPO_PEND_LOTER { get; set; } = new FCTPENLT_DCLFC_TIPO_PEND_LOTER();

    }
}