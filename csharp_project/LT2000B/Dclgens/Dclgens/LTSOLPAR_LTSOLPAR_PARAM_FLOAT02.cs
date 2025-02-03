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
    public class LTSOLPAR_LTSOLPAR_PARAM_FLOAT02 : VarBasis
    {
        /*" 10 LTSOLPAR-PARAM-CHAR01       PIC X(60).*/
        public StringBasis LTSOLPAR_PARAM_CHAR01 { get; set; } = new StringBasis(new PIC("X", "60", "X(60)."), @"");
        /*" 10 LTSOLPAR-PARAM-CHAR02       PIC X(30).*/
        public StringBasis LTSOLPAR_PARAM_CHAR02 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)."), @"");
        /*" 10 LTSOLPAR-PARAM-CHAR03       PIC X(15).*/
        public StringBasis LTSOLPAR_PARAM_CHAR03 { get; set; } = new StringBasis(new PIC("X", "15", "X(15)."), @"");
        /*" 10 LTSOLPAR-PARAM-CHAR04       PIC X(15).*/
        public StringBasis LTSOLPAR_PARAM_CHAR04 { get; set; } = new StringBasis(new PIC("X", "15", "X(15)."), @"");
        /*" 10 LTSOLPAR-TIMESTAMP   PIC X(26).*/
        public StringBasis LTSOLPAR_TIMESTAMP { get; set; } = new StringBasis(new PIC("X", "26", "X(26)."), @"");
        /*" 10 LTSOLPAR-DTH-SOLICITACAO       PIC X(8).*/
        public StringBasis LTSOLPAR_DTH_SOLICITACAO { get; set; } = new StringBasis(new PIC("X", "8", "X(8)."), @"");
        /*" 10 LTSOLPAR-PARAM-CHAR05.*/
        public LTSOLPAR_LTSOLPAR_PARAM_CHAR05 LTSOLPAR_PARAM_CHAR05 { get; set; } = new LTSOLPAR_LTSOLPAR_PARAM_CHAR05();

    }
}