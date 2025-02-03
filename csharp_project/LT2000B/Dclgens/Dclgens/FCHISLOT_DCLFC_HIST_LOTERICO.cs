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
    public class FCHISLOT_DCLFC_HIST_LOTERICO : VarBasis
    {
        /*" 10 FCHISLOT-IDE-HIST-LOTERICO  PIC S9(9) USAGE COMP.*/
        public IntBasis FCHISLOT_IDE_HIST_LOTERICO { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 FCHISLOT-COD-OPERACAO  PIC X(4).*/
        public StringBasis FCHISLOT_COD_OPERACAO { get; set; } = new StringBasis(new PIC("X", "4", "X(4)."), @"");
        /*" 10 FCHISLOT-DTH-REGISTRO  PIC X(26).*/
        public StringBasis FCHISLOT_DTH_REGISTRO { get; set; } = new StringBasis(new PIC("X", "26", "X(26)."), @"");
        /*" 10 FCHISLOT-IDE-USUARIO  PIC S9(4) USAGE COMP.*/
        public IntBasis FCHISLOT_IDE_USUARIO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 FCHISLOT-NUM-LOTERICO  PIC S9(9) USAGE COMP.*/
        public IntBasis FCHISLOT_NUM_LOTERICO { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 FCHISLOT-COD-DETALHE  PIC X(3).*/
        public StringBasis FCHISLOT_COD_DETALHE { get; set; } = new StringBasis(new PIC("X", "3", "X(3)."), @"");
        /*" 10 FCHISLOT-DES-MSG-DESTINO  PIC X(56).*/
        public StringBasis FCHISLOT_DES_MSG_DESTINO { get; set; } = new StringBasis(new PIC("X", "56", "X(56)."), @"");
        /*" 10 FCHISLOT-DES-MSG-ORIGEM  PIC X(56).*/
        public StringBasis FCHISLOT_DES_MSG_ORIGEM { get; set; } = new StringBasis(new PIC("X", "56", "X(56)."), @"");
        /*"*/
    }
}