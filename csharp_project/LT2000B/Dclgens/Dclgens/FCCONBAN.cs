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
    public class FCCONBAN : VarBasis
    {
        /*"01 DCLFC-CONTA-BANCARIA.*/
        public FCCONBAN_DCLFC_CONTA_BANCARIA DCLFC_CONTA_BANCARIA { get; set; } = new FCCONBAN_DCLFC_CONTA_BANCARIA();

    }
}