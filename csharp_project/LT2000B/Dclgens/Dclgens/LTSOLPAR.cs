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
    public class LTSOLPAR : VarBasis
    {
        /*"01 DCLLT-SOLICITA-PARAM.*/
        public LTSOLPAR_DCLLT_SOLICITA_PARAM DCLLT_SOLICITA_PARAM { get; set; } = new LTSOLPAR_DCLLT_SOLICITA_PARAM();

    }
}