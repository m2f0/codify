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
    public class LTMVPRBO_DCLLT_MOV_PROP_BONUS : VarBasis
    {
        /*" 10 LTMVPRBO-COD-PRODUTO  PIC S9(4) USAGE COMP.*/
        public IntBasis LTMVPRBO_COD_PRODUTO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTMVPRBO-COD-EXT-ESTIP  PIC S9(9) USAGE COMP.*/
        public IntBasis LTMVPRBO_COD_EXT_ESTIP { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 LTMVPRBO-COD-EXT-SEGURADO  PIC S9(13)V USAGE COMP-3.*/
        public DoubleBasis LTMVPRBO_COD_EXT_SEGURADO { get; set; } = new DoubleBasis(new PIC("S9", "13", "S9(13)V"), 0);
        /*" 10 LTMVPRBO-DATA-MOVIMENTO  PIC X(10).*/
        public StringBasis LTMVPRBO_DATA_MOVIMENTO { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*" 10 LTMVPRBO-HORA-MOVIMENTO  PIC X(8).*/
        public StringBasis LTMVPRBO_HORA_MOVIMENTO { get; set; } = new StringBasis(new PIC("X", "8", "X(8)."), @"");
        /*" 10 LTMVPRBO-COD-MOVIMENTO  PIC X(1).*/
        public StringBasis LTMVPRBO_COD_MOVIMENTO { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 LTMVPRBO-COD-COBERTURA  PIC S9(4) USAGE COMP.*/
        public IntBasis LTMVPRBO_COD_COBERTURA { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTMVPRBO-COD-BONUS   PIC S9(4) USAGE COMP.*/
        public IntBasis LTMVPRBO_COD_BONUS { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTMVPRBO-DES-ESPEC-BONUS  PIC X(60).*/
        public StringBasis LTMVPRBO_DES_ESPEC_BONUS { get; set; } = new StringBasis(new PIC("X", "60", "X(60)."), @"");
        /*"*/
    }
}