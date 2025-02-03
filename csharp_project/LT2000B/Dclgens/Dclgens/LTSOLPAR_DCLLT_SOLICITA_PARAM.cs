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
    public class LTSOLPAR_DCLLT_SOLICITA_PARAM : VarBasis
    {
        /*" 10 LTSOLPAR-COD-PRODUTO       PIC S9(4) USAGE COMP.*/
        public IntBasis LTSOLPAR_COD_PRODUTO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTSOLPAR-COD-CLIENTE       PIC S9(9) USAGE COMP.*/
        public IntBasis LTSOLPAR_COD_CLIENTE { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 LTSOLPAR-COD-PROGRAMA       PIC X(12).*/
        public StringBasis LTSOLPAR_COD_PROGRAMA { get; set; } = new StringBasis(new PIC("X", "12", "X(12)."), @"");
        /*" 10 LTSOLPAR-TIPO-SOLICITACAO       PIC S9(4) USAGE COMP.*/
        public IntBasis LTSOLPAR_TIPO_SOLICITACAO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTSOLPAR-DATA-SOLICITACAO       PIC X(10).*/
        public StringBasis LTSOLPAR_DATA_SOLICITACAO { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*" 10 LTSOLPAR-COD-USUARIO       PIC X(8).*/
        public StringBasis LTSOLPAR_COD_USUARIO { get; set; } = new StringBasis(new PIC("X", "8", "X(8)."), @"");
        /*" 10 LTSOLPAR-DATA-PREV-PROC       PIC X(10).*/
        public StringBasis LTSOLPAR_DATA_PREV_PROC { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*" 10 LTSOLPAR-SIT-SOLICITACAO       PIC X(1).*/
        public StringBasis LTSOLPAR_SIT_SOLICITACAO { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 LTSOLPAR-TSTMP-SITUACAO       PIC X(26).*/
        public StringBasis LTSOLPAR_TSTMP_SITUACAO { get; set; } = new StringBasis(new PIC("X", "26", "X(26)."), @"");
        /*" 10 LTSOLPAR-PARAM-DATE01       PIC X(10).*/
        public StringBasis LTSOLPAR_PARAM_DATE01 { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*" 10 LTSOLPAR-PARAM-DATE02       PIC X(10).*/
        public StringBasis LTSOLPAR_PARAM_DATE02 { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*" 10 LTSOLPAR-PARAM-DATE03       PIC X(10).*/
        public StringBasis LTSOLPAR_PARAM_DATE03 { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*" 10 LTSOLPAR-PARAM-SMINT01       PIC S9(4) USAGE COMP.*/
        public IntBasis LTSOLPAR_PARAM_SMINT01 { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTSOLPAR-PARAM-SMINT02       PIC S9(4) USAGE COMP.*/
        public IntBasis LTSOLPAR_PARAM_SMINT02 { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTSOLPAR-PARAM-SMINT03       PIC S9(4) USAGE COMP.*/
        public IntBasis LTSOLPAR_PARAM_SMINT03 { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTSOLPAR-PARAM-INTG01       PIC S9(9) USAGE COMP.*/
        public IntBasis LTSOLPAR_PARAM_INTG01 { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 LTSOLPAR-PARAM-INTG02       PIC S9(9) USAGE COMP.*/
        public IntBasis LTSOLPAR_PARAM_INTG02 { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 LTSOLPAR-PARAM-INTG03       PIC S9(9) USAGE COMP.*/
        public IntBasis LTSOLPAR_PARAM_INTG03 { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 LTSOLPAR-PARAM-DEC01       PIC S9(17)V USAGE COMP-3.*/
        public DoubleBasis LTSOLPAR_PARAM_DEC01 { get; set; } = new DoubleBasis(new PIC("S9", "17", "S9(17)V"), 0);
        /*" 10 LTSOLPAR-PARAM-DEC02       PIC S9(17)V USAGE COMP-3.*/
        public DoubleBasis LTSOLPAR_PARAM_DEC02 { get; set; } = new DoubleBasis(new PIC("S9", "17", "S9(17)V"), 0);
        /*" 10 LTSOLPAR-PARAM-DEC03       PIC S9(17)V USAGE COMP-3.*/
        public DoubleBasis LTSOLPAR_PARAM_DEC03 { get; set; } = new DoubleBasis(new PIC("S9", "17", "S9(17)V"), 0);
        /*" 10 LTSOLPAR-PARAM-FLOAT01       USAGE COMP-2.*/
        public LTSOLPAR_LTSOLPAR_PARAM_FLOAT01 LTSOLPAR_PARAM_FLOAT01 { get; set; } = new LTSOLPAR_LTSOLPAR_PARAM_FLOAT01();

    }
}