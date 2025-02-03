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
    public class LTMVPRBO : VarBasis
    {
        /*"01 DCLLT-MOV-PROP-BONUS.*/
        public LTMVPRBO_DCLLT_MOV_PROP_BONUS DCLLT_MOV_PROP_BONUS { get; set; } = new LTMVPRBO_DCLLT_MOV_PROP_BONUS();

    }
}