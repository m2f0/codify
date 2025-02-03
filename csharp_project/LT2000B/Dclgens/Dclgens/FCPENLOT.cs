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
    public class FCPENLOT : VarBasis
    {
        /*"01 DCLFC-PEND-LOTERICO.*/
        public FCPENLOT_DCLFC_PEND_LOTERICO DCLFC_PEND_LOTERICO { get; set; } = new FCPENLOT_DCLFC_PEND_LOTERICO();

    }
}