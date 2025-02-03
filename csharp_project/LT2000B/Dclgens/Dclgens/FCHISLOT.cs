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
    public class FCHISLOT : VarBasis
    {
        /*"01 DCLFC-HIST-LOTERICO.*/
        public FCHISLOT_DCLFC_HIST_LOTERICO DCLFC_HIST_LOTERICO { get; set; } = new FCHISLOT_DCLFC_HIST_LOTERICO();

    }
}