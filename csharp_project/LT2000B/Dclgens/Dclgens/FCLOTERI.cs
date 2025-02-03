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
    public class FCLOTERI : VarBasis
    {
        /*"01 DCLFC-LOTERICO.*/
        public FCLOTERI_DCLFC_LOTERICO DCLFC_LOTERICO { get; set; } = new FCLOTERI_DCLFC_LOTERICO();

    }
}