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
    public class FCLOTERI_DCLFC_LOTERICO : VarBasis
    {
        /*" 10 FCLOTERI-NUM-LOTERICO  PIC S9(9) USAGE COMP.*/
        public IntBasis FCLOTERI_NUM_LOTERICO { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 FCLOTERI-COD-AGENTE-MASTER  PIC X(9).*/
        public StringBasis FCLOTERI_COD_AGENTE_MASTER { get; set; } = new StringBasis(new PIC("X", "9", "X(9)."), @"");
        /*" 10 FCLOTERI-COD-CGC     PIC X(15).*/
        public StringBasis FCLOTERI_COD_CGC { get; set; } = new StringBasis(new PIC("X", "15", "X(15)."), @"");
        /*" 10 FCLOTERI-COD-INSCR-ESTAD  PIC X(15).*/
        public StringBasis FCLOTERI_COD_INSCR_ESTAD { get; set; } = new StringBasis(new PIC("X", "15", "X(15)."), @"");
        /*" 10 FCLOTERI-COD-INSCR-MUNIC  PIC X(15).*/
        public StringBasis FCLOTERI_COD_INSCR_MUNIC { get; set; } = new StringBasis(new PIC("X", "15", "X(15)."), @"");
        /*" 10 FCLOTERI-COD-MUNICIPIO  PIC X(12).*/
        public StringBasis FCLOTERI_COD_MUNICIPIO { get; set; } = new StringBasis(new PIC("X", "12", "X(12)."), @"");
        /*" 10 FCLOTERI-COD-UF      PIC X(2).*/
        public StringBasis FCLOTERI_COD_UF { get; set; } = new StringBasis(new PIC("X", "2", "X(2)."), @"");
        /*" 10 FCLOTERI-DES-EMAIL   PIC X(50).*/
        public StringBasis FCLOTERI_DES_EMAIL { get; set; } = new StringBasis(new PIC("X", "50", "X(50)."), @"");
        /*" 10 FCLOTERI-DES-ENDERECO  PIC X(56).*/
        public StringBasis FCLOTERI_DES_ENDERECO { get; set; } = new StringBasis(new PIC("X", "56", "X(56)."), @"");
        /*" 10 FCLOTERI-DTH-EXCLUSAO  PIC X(10).*/
        public StringBasis FCLOTERI_DTH_EXCLUSAO { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*" 10 FCLOTERI-DTH-INCLUSAO  PIC X(10).*/
        public StringBasis FCLOTERI_DTH_INCLUSAO { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*" 10 FCLOTERI-IDE-CONTA-CAUCAO  PIC S9(9) USAGE COMP.*/
        public IntBasis FCLOTERI_IDE_CONTA_CAUCAO { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 FCLOTERI-IDE-CONTA-CPMF  PIC S9(9) USAGE COMP.*/
        public IntBasis FCLOTERI_IDE_CONTA_CPMF { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 FCLOTERI-IDE-CONTA-ISENTA  PIC S9(9) USAGE COMP.*/
        public IntBasis FCLOTERI_IDE_CONTA_ISENTA { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 FCLOTERI-IND-CAT-LOTERICO  PIC S9(4) USAGE COMP.*/
        public IntBasis FCLOTERI_IND_CAT_LOTERICO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 FCLOTERI-IND-STA-LOTERICO  PIC S9(4) USAGE COMP.*/
        public IntBasis FCLOTERI_IND_STA_LOTERICO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 FCLOTERI-IND-UNIDADE-SUB  PIC S9(9) USAGE COMP.*/
        public IntBasis FCLOTERI_IND_UNIDADE_SUB { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 FCLOTERI-NOM-BAIRRO  PIC X(30).*/
        public StringBasis FCLOTERI_NOM_BAIRRO { get; set; } = new StringBasis(new PIC("X", "30", "X(30)."), @"");
        /*" 10 FCLOTERI-NOM-CONSULTOR  PIC X(40).*/
        public StringBasis FCLOTERI_NOM_CONSULTOR { get; set; } = new StringBasis(new PIC("X", "40", "X(40)."), @"");
        /*" 10 FCLOTERI-NOM-CONTATO1  PIC X(20).*/
        public StringBasis FCLOTERI_NOM_CONTATO1 { get; set; } = new StringBasis(new PIC("X", "20", "X(20)."), @"");
        /*" 10 FCLOTERI-NOM-CONTATO2  PIC X(20).*/
        public StringBasis FCLOTERI_NOM_CONTATO2 { get; set; } = new StringBasis(new PIC("X", "20", "X(20)."), @"");
        /*" 10 FCLOTERI-NOM-FANTASIA  PIC X(30).*/
        public StringBasis FCLOTERI_NOM_FANTASIA { get; set; } = new StringBasis(new PIC("X", "30", "X(30)."), @"");
        /*" 10 FCLOTERI-NOM-MUNICIPIO  PIC X(30).*/
        public StringBasis FCLOTERI_NOM_MUNICIPIO { get; set; } = new StringBasis(new PIC("X", "30", "X(30)."), @"");
        /*" 10 FCLOTERI-NOM-RAZAO-SOCIAL  PIC X(40).*/
        public StringBasis FCLOTERI_NOM_RAZAO_SOCIAL { get; set; } = new StringBasis(new PIC("X", "40", "X(40)."), @"");
        /*" 10 FCLOTERI-NUM-CEP     PIC S9(9) USAGE COMP.*/
        public IntBasis FCLOTERI_NUM_CEP { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 FCLOTERI-NUM-ENCEF   PIC S9(4) USAGE COMP.*/
        public IntBasis FCLOTERI_NUM_ENCEF { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 FCLOTERI-NUM-LOTER-ANT  PIC S9(9) USAGE COMP.*/
        public IntBasis FCLOTERI_NUM_LOTER_ANT { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 FCLOTERI-NUM-MATR-CONSULTOR  PIC S9(9) USAGE COMP.*/
        public IntBasis FCLOTERI_NUM_MATR_CONSULTOR { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 FCLOTERI-NUM-PVCEF   PIC S9(4) USAGE COMP.*/
        public IntBasis FCLOTERI_NUM_PVCEF { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 FCLOTERI-NUM-TELEFONE  PIC X(16).*/
        public StringBasis FCLOTERI_NUM_TELEFONE { get; set; } = new StringBasis(new PIC("X", "16", "X(16)."), @"");
        /*" 10 FCLOTERI-STA-DADOS-N-CADAST  PIC X(1).*/
        public StringBasis FCLOTERI_STA_DADOS_N_CADAST { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 FCLOTERI-STA-LOTERICO  PIC X(1).*/
        public StringBasis FCLOTERI_STA_LOTERICO { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 FCLOTERI-STA-NIVEL-COMIS  PIC X(1).*/
        public StringBasis FCLOTERI_STA_NIVEL_COMIS { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 FCLOTERI-STA-ULT-ALT-ONLINE  PIC X(1).*/
        public StringBasis FCLOTERI_STA_ULT_ALT_ONLINE { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 FCLOTERI-COD-GARANTIA  PIC X(1).*/
        public StringBasis FCLOTERI_COD_GARANTIA { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 FCLOTERI-VLR-GARANTIA  PIC S9(13)V9(2) USAGE COMP-3.*/
        public DoubleBasis FCLOTERI_VLR_GARANTIA { get; set; } = new DoubleBasis(new PIC("S9", "13", "S9(13)V9(2)"), 2);
        /*" 10 FCLOTERI-DTH-GERACAO  PIC X(10).*/
        public StringBasis FCLOTERI_DTH_GERACAO { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*" 10 FCLOTERI-NUM-FAX     PIC X(16).*/
        public StringBasis FCLOTERI_NUM_FAX { get; set; } = new StringBasis(new PIC("X", "16", "X(16)."), @"");
        /*" 10 FCLOTERI-NUM-DV-LOTERICO  PIC S9(4) USAGE COMP.*/
        public IntBasis FCLOTERI_NUM_DV_LOTERICO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 FCLOTERI-NUM-SEGURADORA  PIC S9(9) USAGE COMP.*/
        public IntBasis FCLOTERI_NUM_SEGURADORA { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*"*/
    }
}