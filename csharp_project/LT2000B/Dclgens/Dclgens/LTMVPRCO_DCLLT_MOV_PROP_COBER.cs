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
    public class LTMVPRCO_DCLLT_MOV_PROP_COBER : VarBasis
    {
        /*" 10 LTMVPRCO-COD-COBERTURA  PIC S9(4) USAGE COMP.*/
        public IntBasis LTMVPRCO_COD_COBERTURA { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTMVPRCO-COD-EXT-ESTIP  PIC S9(9) USAGE COMP.*/
        public IntBasis LTMVPRCO_COD_EXT_ESTIP { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 LTMVPRCO-COD-EXT-SEGURADO  PIC S9(13)V USAGE COMP-3.*/
        public DoubleBasis LTMVPRCO_COD_EXT_SEGURADO { get; set; } = new DoubleBasis(new PIC("S9", "13", "S9(13)V"), 0);
        /*" 10 LTMVPRCO-DATA-MOVIMENTO  PIC X(10).*/
        public StringBasis LTMVPRCO_DATA_MOVIMENTO { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*" 10 LTMVPRCO-HORA-MOVIMENTO  PIC X(8).*/
        public StringBasis LTMVPRCO_HORA_MOVIMENTO { get; set; } = new StringBasis(new PIC("X", "8", "X(8)."), @"");
        /*" 10 LTMVPRCO-COD-MOVIMENTO  PIC X(1).*/
        public StringBasis LTMVPRCO_COD_MOVIMENTO { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 LTMVPRCO-COD-PRODUTO  PIC S9(4) USAGE COMP.*/
        public IntBasis LTMVPRCO_COD_PRODUTO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTMVPRCO-VAL-IMP-SEGURADA  PIC S9(13)V9(2) USAGE COMP-3.*/
        public DoubleBasis LTMVPRCO_VAL_IMP_SEGURADA { get; set; } = new DoubleBasis(new PIC("S9", "13", "S9(13)V9(2)"), 2);
        /*" 10 LTMVPRCO-VAL-TAXA-PREMIO  PIC S9(3)V9(9) USAGE COMP-3.*/
        public DoubleBasis LTMVPRCO_VAL_TAXA_PREMIO { get; set; } = new DoubleBasis(new PIC("S9", "3", "S9(3)V9(9)"), 9);
        /*"*/
    }
}