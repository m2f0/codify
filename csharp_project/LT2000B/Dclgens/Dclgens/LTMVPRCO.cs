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
    public class LTMVPRCO : VarBasis
    {
        /*"01 DCLLT-MOV-PROP-COBER.*/
        public LTMVPRCO_DCLLT_MOV_PROP_COBER DCLLT_MOV_PROP_COBER { get; set; } = new LTMVPRCO_DCLLT_MOV_PROP_COBER();

    }
}