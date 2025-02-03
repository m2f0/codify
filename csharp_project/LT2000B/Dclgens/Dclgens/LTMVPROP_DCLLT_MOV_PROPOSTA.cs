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
    public class LTMVPROP_DCLLT_MOV_PROPOSTA : VarBasis
    {
        /*" 10 LTMVPROP-COD-PRODUTO       PIC S9(4) USAGE COMP.*/
        public IntBasis LTMVPROP_COD_PRODUTO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTMVPROP-COD-EXT-ESTIP       PIC S9(9) USAGE COMP.*/
        public IntBasis LTMVPROP_COD_EXT_ESTIP { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 LTMVPROP-COD-EXT-SEGURADO       PIC S9(13)V USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_COD_EXT_SEGURADO { get; set; } = new DoubleBasis(new PIC("S9", "13", "S9(13)V"), 0);
        /*" 10 LTMVPROP-DATA-MOVIMENTO       PIC X(10).*/
        public StringBasis LTMVPROP_DATA_MOVIMENTO { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*" 10 LTMVPROP-HORA-MOVIMENTO       PIC X(8).*/
        public StringBasis LTMVPROP_HORA_MOVIMENTO { get; set; } = new StringBasis(new PIC("X", "8", "X(8)."), @"");
        /*" 10 LTMVPROP-COD-MOVIMENTO       PIC X(1).*/
        public StringBasis LTMVPROP_COD_MOVIMENTO { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 LTMVPROP-NOM-RAZAO-SOCIAL       PIC X(40).*/
        public StringBasis LTMVPROP_NOM_RAZAO_SOCIAL { get; set; } = new StringBasis(new PIC("X", "40", "X(40)."), @"");
        /*" 10 LTMVPROP-NOME-FANTASIA       PIC X(30).*/
        public StringBasis LTMVPROP_NOME_FANTASIA { get; set; } = new StringBasis(new PIC("X", "30", "X(30)."), @"");
        /*" 10 LTMVPROP-CGCCPF      PIC S9(15)V USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_CGCCPF { get; set; } = new DoubleBasis(new PIC("S9", "15", "S9(15)V"), 0);
        /*" 10 LTMVPROP-ENDERECO    PIC X(40).*/
        public StringBasis LTMVPROP_ENDERECO { get; set; } = new StringBasis(new PIC("X", "40", "X(40)."), @"");
        /*" 10 LTMVPROP-COMPL-ENDER       PIC X(15).*/
        public StringBasis LTMVPROP_COMPL_ENDER { get; set; } = new StringBasis(new PIC("X", "15", "X(15)."), @"");
        /*" 10 LTMVPROP-BAIRRO      PIC X(20).*/
        public StringBasis LTMVPROP_BAIRRO { get; set; } = new StringBasis(new PIC("X", "20", "X(20)."), @"");
        /*" 10 LTMVPROP-CEP         PIC S9(9) USAGE COMP.*/
        public IntBasis LTMVPROP_CEP { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 LTMVPROP-CIDADE      PIC X(20).*/
        public StringBasis LTMVPROP_CIDADE { get; set; } = new StringBasis(new PIC("X", "20", "X(20)."), @"");
        /*" 10 LTMVPROP-SIGLA-UF    PIC X(2).*/
        public StringBasis LTMVPROP_SIGLA_UF { get; set; } = new StringBasis(new PIC("X", "2", "X(2)."), @"");
        /*" 10 LTMVPROP-DDD         PIC S9(4) USAGE COMP.*/
        public IntBasis LTMVPROP_DDD { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTMVPROP-NUM-FONE    PIC S9(11)V USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_NUM_FONE { get; set; } = new DoubleBasis(new PIC("S9", "11", "S9(11)V"), 0);
        /*" 10 LTMVPROP-NUM-FAX     PIC S9(11)V USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_NUM_FAX { get; set; } = new DoubleBasis(new PIC("S9", "11", "S9(11)V"), 0);
        /*" 10 LTMVPROP-EMAIL       PIC X(40).*/
        public StringBasis LTMVPROP_EMAIL { get; set; } = new StringBasis(new PIC("X", "40", "X(40)."), @"");
        /*" 10 LTMVPROP-COD-DIVISAO       PIC S9(4) USAGE COMP.*/
        public IntBasis LTMVPROP_COD_DIVISAO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTMVPROP-COD-SUBDIVISAO       PIC S9(4) USAGE COMP.*/
        public IntBasis LTMVPROP_COD_SUBDIVISAO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTMVPROP-COD-BANCO   PIC X(3).*/
        public StringBasis LTMVPROP_COD_BANCO { get; set; } = new StringBasis(new PIC("X", "3", "X(3)."), @"");
        /*" 10 LTMVPROP-COD-AGENCIA       PIC X(4).*/
        public StringBasis LTMVPROP_COD_AGENCIA { get; set; } = new StringBasis(new PIC("X", "4", "X(4)."), @"");
        /*" 10 LTMVPROP-COD-CONTA   PIC X(12).*/
        public StringBasis LTMVPROP_COD_CONTA { get; set; } = new StringBasis(new PIC("X", "12", "X(12)."), @"");
        /*" 10 LTMVPROP-COD-DV-CONTA       PIC X(1).*/
        public StringBasis LTMVPROP_COD_DV_CONTA { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 LTMVPROP-COD-OP-CONTA       PIC X(4).*/
        public StringBasis LTMVPROP_COD_OP_CONTA { get; set; } = new StringBasis(new PIC("X", "4", "X(4)."), @"");
        /*" 10 LTMVPROP-COD-EXT-SEG-ANT       PIC S9(13)V USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_COD_EXT_SEG_ANT { get; set; } = new DoubleBasis(new PIC("S9", "13", "S9(13)V"), 0);
        /*" 10 LTMVPROP-SIT-MOVIMENTO       PIC X(1).*/
        public StringBasis LTMVPROP_SIT_MOVIMENTO { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 LTMVPROP-DT-INIVIG-PROPOSTA       PIC X(10).*/
        public StringBasis LTMVPROP_DT_INIVIG_PROPOSTA { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*" 10 LTMVPROP-IND-ALT-DADOS-PES       PIC X(1).*/
        public StringBasis LTMVPROP_IND_ALT_DADOS_PES { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 LTMVPROP-IND-ALT-ENDER       PIC X(1).*/
        public StringBasis LTMVPROP_IND_ALT_ENDER { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 LTMVPROP-IND-ALT-COBER       PIC X(1).*/
        public StringBasis LTMVPROP_IND_ALT_COBER { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 LTMVPROP-IND-ALT-BONUS       PIC X(1).*/
        public StringBasis LTMVPROP_IND_ALT_BONUS { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 LTMVPROP-COD-USUARIO       PIC X(8).*/
        public StringBasis LTMVPROP_COD_USUARIO { get; set; } = new StringBasis(new PIC("X", "8", "X(8)."), @"");
        /*" 10 LTMVPROP-TIMESTAMP   PIC X(26).*/
        public StringBasis LTMVPROP_TIMESTAMP { get; set; } = new StringBasis(new PIC("X", "26", "X(26)."), @"");
        /*" 10 LTMVPROP-VAL-PREMIO  PIC S9(13)V9(2) USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_VAL_PREMIO { get; set; } = new DoubleBasis(new PIC("S9", "13", "S9(13)V9(2)"), 2);
        /*" 10 LTMVPROP-NUM-APOLICE       PIC S9(13)V USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_NUM_APOLICE { get; set; } = new DoubleBasis(new PIC("S9", "13", "S9(13)V"), 0);
        /*" 10 LTMVPROP-COD-USUARIO-CANCEL       PIC X(8).*/
        public StringBasis LTMVPROP_COD_USUARIO_CANCEL { get; set; } = new StringBasis(new PIC("X", "8", "X(8)."), @"");
        /*" 10 LTMVPROP-COD-FONTE   PIC S9(4) USAGE COMP.*/
        public IntBasis LTMVPROP_COD_FONTE { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTMVPROP-NUM-PROPOSTA       PIC S9(9) USAGE COMP.*/
        public IntBasis LTMVPROP_NUM_PROPOSTA { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 LTMVPROP-COD-CLASSE-ADESAO       PIC X(1).*/
        public StringBasis LTMVPROP_COD_CLASSE_ADESAO { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 LTMVPROP-COD-MUNICIPIO       PIC S9(9) USAGE COMP.*/
        public IntBasis LTMVPROP_COD_MUNICIPIO { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 LTMVPROP-QTD-PARCELAS       PIC S9(4) USAGE COMP.*/
        public IntBasis LTMVPROP_QTD_PARCELAS { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTMVPROP-PCT-JUROS   PIC S9(3)V9(4) USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_PCT_JUROS { get; set; } = new DoubleBasis(new PIC("S9", "3", "S9(3)V9(4)"), 4);
        /*" 10 LTMVPROP-SEQ-PROPOSTA       PIC S9(13)V USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_SEQ_PROPOSTA { get; set; } = new DoubleBasis(new PIC("S9", "13", "S9(13)V"), 0);
        /*" 10 LTMVPROP-VLR-CUSTO-APOLICE       PIC S9(13)V9(2) USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_VLR_CUSTO_APOLICE { get; set; } = new DoubleBasis(new PIC("S9", "13", "S9(13)V9(2)"), 2);
        /*" 10 LTMVPROP-VLR-ADICIONAL-FRAC       PIC S9(13)V9(2) USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_VLR_ADICIONAL_FRAC { get; set; } = new DoubleBasis(new PIC("S9", "13", "S9(13)V9(2)"), 2);
        /*" 10 LTMVPROP-VLR-JUROS-MENSAL       PIC S9(13)V9(2) USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_VLR_JUROS_MENSAL { get; set; } = new DoubleBasis(new PIC("S9", "13", "S9(13)V9(2)"), 2);
        /*" 10 LTMVPROP-NUM-ENDOSSO       PIC S9(9) USAGE COMP.*/
        public IntBasis LTMVPROP_NUM_ENDOSSO { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 LTMVPROP-COD-ANT-LOTERICO       PIC S9(13)V USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_COD_ANT_LOTERICO { get; set; } = new DoubleBasis(new PIC("S9", "13", "S9(13)V"), 0);
        /*" 10 LTMVPROP-STA-CANCELAMENTO       PIC X(1).*/
        public StringBasis LTMVPROP_STA_CANCELAMENTO { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 LTMVPROP-DTH-FIM-PROPOSTA       PIC X(10).*/
        public StringBasis LTMVPROP_DTH_FIM_PROPOSTA { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*" 10 LTMVPROP-NUM-TITULO  PIC S9(13)V USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_NUM_TITULO { get; set; } = new DoubleBasis(new PIC("S9", "13", "S9(13)V"), 0);
        /*" 10 LTMVPROP-NUM-CLASSE-ADESAO       PIC S9(4) USAGE COMP.*/
        public IntBasis LTMVPROP_NUM_CLASSE_ADESAO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTMVPROP-COD-PESSOA-SOCIO       PIC S9(9) USAGE COMP.*/
        public IntBasis LTMVPROP_COD_PESSOA_SOCIO { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 LTMVPROP-PCT-DESC-EQUIP       PIC S9(4)V9(3) USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_PCT_DESC_EQUIP { get; set; } = new DoubleBasis(new PIC("S9", "4", "S9(4)V9(3)"), 3);
        /*" 10 LTMVPROP-PCT-DESC-FIDEL       PIC S9(4)V9(3) USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_PCT_DESC_FIDEL { get; set; } = new DoubleBasis(new PIC("S9", "4", "S9(4)V9(3)"), 3);
        /*" 10 LTMVPROP-PCT-DESC-EXP       PIC S9(4)V9(3) USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_PCT_DESC_EXP { get; set; } = new DoubleBasis(new PIC("S9", "4", "S9(4)V9(3)"), 3);
        /*" 10 LTMVPROP-PCT-DESC-AGRUP       PIC S9(4)V9(3) USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_PCT_DESC_AGRUP { get; set; } = new DoubleBasis(new PIC("S9", "4", "S9(4)V9(3)"), 3);
        /*" 10 LTMVPROP-PCT-IOF     PIC S9(4)V9(3) USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_PCT_IOF { get; set; } = new DoubleBasis(new PIC("S9", "4", "S9(4)V9(3)"), 3);
        /*" 10 LTMVPROP-IND-TIPO-ADESAO       PIC S9(9) USAGE COMP.*/
        public IntBasis LTMVPROP_IND_TIPO_ADESAO { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 LTMVPROP-STA-ALT-CONTA       PIC X(1).*/
        public StringBasis LTMVPROP_STA_ALT_CONTA { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 LTMVPROP-IND-REGIAO  PIC S9(9) USAGE COMP.*/
        public IntBasis LTMVPROP_IND_REGIAO { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 LTMVPROP-IND-TIPO-ENDOSSO       PIC S9(4) USAGE COMP.*/
        public IntBasis LTMVPROP_IND_TIPO_ENDOSSO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTMVPROP-STA-ALT-BENEFICIARIO       PIC X(1).*/
        public StringBasis LTMVPROP_STA_ALT_BENEFICIARIO { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 LTMVPROP-VLR-FATUR-ANUAL       PIC S9(8)V9(2) USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_VLR_FATUR_ANUAL { get; set; } = new DoubleBasis(new PIC("S9", "8", "S9(8)V9(2)"), 2);
        /*" 10 LTMVPROP-IND-MOTIVO-ENDOSSO       PIC S9(4) USAGE COMP.*/
        public IntBasis LTMVPROP_IND_MOTIVO_ENDOSSO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTMVPROP-PCT-COM-CORRETOR       PIC S9(4)V9(3) USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_PCT_COM_CORRETOR { get; set; } = new DoubleBasis(new PIC("S9", "4", "S9(4)V9(3)"), 3);
        /*" 10 LTMVPROP-PCT-COM-INDICADOR       PIC S9(4)V9(3) USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_PCT_COM_INDICADOR { get; set; } = new DoubleBasis(new PIC("S9", "4", "S9(4)V9(3)"), 3);
        /*" 10 LTMVPROP-PCT-COM-BALCAO       PIC S9(4)V9(3) USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_PCT_COM_BALCAO { get; set; } = new DoubleBasis(new PIC("S9", "4", "S9(4)V9(3)"), 3);
        /*" 10 LTMVPROP-PCT-DESC-BLINDAGEM       PIC S9(4)V9(3) USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_PCT_DESC_BLINDAGEM { get; set; } = new DoubleBasis(new PIC("S9", "4", "S9(4)V9(3)"), 3);
        /*" 10 LTMVPROP-VLR-PRM-REFERENCIA       PIC S9(8)V9(2) USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_VLR_PRM_REFERENCIA { get; set; } = new DoubleBasis(new PIC("S9", "8", "S9(8)V9(2)"), 2);
        /*" 10 LTMVPROP-DTH-ENVIO-PROPOSTA       PIC X(10).*/
        public StringBasis LTMVPROP_DTH_ENVIO_PROPOSTA { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*" 10 LTMVPROP-COD-CANAL   PIC S9(4) USAGE COMP.*/
        public IntBasis LTMVPROP_COD_CANAL { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTMVPROP-STA-SEGURADO       PIC S9(4) USAGE COMP.*/
        public IntBasis LTMVPROP_STA_SEGURADO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTMVPROP-QTD-REN-SEM-SINI       PIC S9(4) USAGE COMP.*/
        public IntBasis LTMVPROP_QTD_REN_SEM_SINI { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTMVPROP-QTD-REN-SEM-SINI-INF       PIC S9(4) USAGE COMP.*/
        public IntBasis LTMVPROP_QTD_REN_SEM_SINI_INF { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTMVPROP-NUM-PROPOSTA-SIM       PIC S9(18) USAGE COMP.*/
        public IntBasis LTMVPROP_NUM_PROPOSTA_SIM { get; set; } = new IntBasis(new PIC("S9", "18", "S9(18)"));
        /*" 10 LTMVPROP-IND-TIPO-VIGENCIA       PIC S9(4) USAGE COMP.*/
        public IntBasis LTMVPROP_IND_TIPO_VIGENCIA { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTMVPROP-PCT-DESC-PLURIANUAL       PIC S9(4)V9(3) USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_PCT_DESC_PLURIANUAL { get; set; } = new DoubleBasis(new PIC("S9", "4", "S9(4)V9(3)"), 3);
        /*" 10 LTMVPROP-PCT-DESC-COFRE       PIC S9(4)V9(3) USAGE COMP-3.*/
        public DoubleBasis LTMVPROP_PCT_DESC_COFRE { get; set; } = new DoubleBasis(new PIC("S9", "4", "S9(4)V9(3)"), 3);
        /*" 10 LTMVPROP-IND-TIPO-CORRESP       PIC S9(4) USAGE COMP.*/
        public IntBasis LTMVPROP_IND_TIPO_CORRESP { get; set; } = new IntBasis(new PIC("S9", "4", "S9(4)"));
        /*" 10 LTMVPROP-COD-PROGRAMA       PIC X(10).*/
        public StringBasis LTMVPROP_COD_PROGRAMA { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*"*/
    }
}