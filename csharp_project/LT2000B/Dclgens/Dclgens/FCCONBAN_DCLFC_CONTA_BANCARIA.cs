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
    public class FCCONBAN_DCLFC_CONTA_BANCARIA : VarBasis
    {
        /*" 10 FCCONBAN-IDE-CONTA-BANCARIA       PIC S9(9) USAGE COMP.*/
        public IntBasis FCCONBAN_IDE_CONTA_BANCARIA { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 FCCONBAN-COD-AGENCIA       PIC S9(9) USAGE COMP.*/
        public IntBasis FCCONBAN_COD_AGENCIA { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 FCCONBAN-COD-BANCO   PIC S9(4) USAGE COMP.*/
        public IntBasis FCCONBAN_COD_BANCO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 FCCONBAN-COD-CONTA   PIC S9(18) USAGE COMP.*/
        public IntBasis FCCONBAN_COD_CONTA { get; set; } = new IntBasis(new PIC("S9", "18", "S9(18)"));
        /*" 10 FCCONBAN-COD-DV-CONTA       PIC X(1).*/
        public StringBasis FCCONBAN_COD_DV_CONTA { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 FCCONBAN-COD-OP-CONTA       PIC S9(4) USAGE COMP.*/
        public IntBasis FCCONBAN_COD_OP_CONTA { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 FCCONBAN-COD-TIPO-CONTA       PIC X(3).*/
        public StringBasis FCCONBAN_COD_TIPO_CONTA { get; set; } = new StringBasis(new PIC("X", "3", "X(3)."), @"");
        /*" 10 FCCONBAN-COD-EMPRESA       PIC S9(9) USAGE COMP.*/
        public IntBasis FCCONBAN_COD_EMPRESA { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 FCCONBAN-IND-OBJETIVO-CONTA       PIC S9(4) USAGE COMP.*/
        public IntBasis FCCONBAN_IND_OBJETIVO_CONTA { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 FCCONBAN-DTH-ATUALIZACAO       PIC X(26).*/
        public StringBasis FCCONBAN_DTH_ATUALIZACAO { get; set; } = new StringBasis(new PIC("X", "26", "X(26)."), @"");
        /*"*/
    }
}