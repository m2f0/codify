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
    public class FCSEQUEN : VarBasis
    {
        /*"01 DCLFC-SEQUENCE.*/
        public FCSEQUEN_DCLFC_SEQUENCE DCLFC_SEQUENCE { get; set; } = new FCSEQUEN_DCLFC_SEQUENCE();

    }
}