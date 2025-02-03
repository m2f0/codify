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
    public class LTMVPROP : VarBasis
    {
        /*"01 DCLLT-MOV-PROPOSTA.*/
        public LTMVPROP_DCLLT_MOV_PROPOSTA DCLLT_MOV_PROPOSTA { get; set; } = new LTMVPROP_DCLLT_MOV_PROPOSTA();

    }
}