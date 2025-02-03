using System;
using IA_ConverterCommons;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Linq;
using _ = IA_ConverterCommons.Statements;
using DB = IA_ConverterCommons.DatabaseBasis;
using Dclgens;
using Sias.Loterico.DB2.LT2000B;

[module: StopWatch]
namespace Code
{
    public class LT2000B
    {
        public bool IsCall { get; set; }

        public LT2000B()
        {
            AppSettings.Load();
        }

        #region PROG HEADER
        /*"      *--------------------------------------                                 */
        /*"      *----------------------------------------------------------------*      */
        /*"      *   SISTEMA ................  SISTEMA DE LOTERICO                *      */
        /*"      *   PROGRAMA ...............  LT2000B                            *      */
        /*"      *----------------------------------------------------------------*      */
        /*"      *   ANALISTA ...............                                     *      */
        /*"      *   PROGRAMADOR ............                                     *      */
        /*"      *   DATA CODIFICACAO .......  MAIO     / 2001                    *      */
        /*"      *   VERSAO .................  02012009 17:00HS                   *      */
        /*"      *----------------------------------------------------------------*      */
        /*"      *   FUNCAO :   CRITICAR O ARQUIVO DE MOVIMENTACAO DO SIGEL/CEF   *      */
        /*"      *              E ATUALIZAR O CADASTRO DE LOTERICOS(FC_LOTERICOS) *      */
        /*"      *              E SE O LOTERICO FOR SEGURADO GERAR MOVIMENTO DE   *      */
        /*"      *              ALTERACAO/EXCLUSAO NA TABELA LT_MOV_PROPOSTA PARA *      */
        /*"      *              POSTERIOR ATUALIZACAO DOS CADASTROS DOS LOTERICOS *      */
        /*"      *              SEGURADOS                                         *      */
        /*"      *----------------------------------------------------------------*      */
        /*"      *   ALTERADO PARA APOLICE:                                       *      */
        /*"      *   APOLICE ANTIGA ----------: 0107100057625---------------------*      */
        /*"      *   APOLICE NOVA   ----------:-0107100070673---------------------*      */
        /*"      *----------------------------------------------------------------*      */
        /*"      *   GERA ARQUIVO COM LOTERICOS QUE NAO QUEREM RENOVAR            *      */
        /*"      *       CAD-RENOVAR  = 1 (NAO DESEJA RENOVAR - NO SIGEL)         *      */
        /*"      *       SOMENTE DIA 24/07/2002                                   *      */
        /*"      *----------------------------------------------------------------*      */
        /*"      * EM 10/12/2002 - NAO CONSIDERA NUM-FAX = 99999999  - ALT-K1 ----*      */
        /*"      *----------------------------------------------------------------*      */
        /*"      *----------------------------------------------------------------*      */
        /*"      * EM 18/01/2003 - ALTERACAO SOMENTE PARA TELEFONE,EMAIL E FAX    *      */
        /*"      *                 NAO GERA CERTIFICADO.                          *      */
        /*"      *                 VARIAVEL W-IND-ALT-FAXTELEME                   *      */
        /*"      *                 ALTERA DDD PARA -1. (PROG LT2002B TRATA).      *      */
        /*"      * PROCURAR: OL1801                                               *      */
        /*"      *----------------------------------------------------------------*      */
        /*"      * EM 08/05/2003 - ALTERACAO SOMENTE PARA FAX IGUAL 9999999       *      */
        /*"      *                 NAO GERA CERTIFICADO.                          *      */
        /*"      *                 DDD E EMAIL CONTINUA GERANDO CERTIFICADO       *      */
        /*"      *                 VARIAVEL W-IND-ALT-FAXTELEME                   *      */
        /*"      *----------------------------------------------------------------*      */
        /*"      * EM 09/06/2003 - INCLUIDO TOTALIZADORES DAS INFORMACOES LIDAS   *      */
        /*"      *                 NO SIGEL.                                      *      */
        /*"      * PROCURAR: OL0906                                               *      */
        /*"      *----------------------------------------------------------------*      */
        /*"      * EM 26/02/2005 - RETIRADO EXCLUSAO DA TABELA  FC_PEND_LOTERICO  *      */
        /*"      *             ROTINA = R6750-DELETE-FC-PEND-LOTERICO             *      */
        /*"      *----------------------------------------------------------------*      */
        /*"      * EM 10/11/2006 - RETIRADO ALTERACAO DE IND_UNIDADE SUB          *      */
        /*"      *             DA FC_LOTERICO ( CONTERA O CODIGO DO SOCIO         *      */
        /*"      *                              NA GE-PESSOA-SOCIO        )       *      */
        /*"      *----------------------------------------------------------------*      */
        /*"      * EM 21/03/2015 - ALTERACAO DA CAPITALIZACAO NO SISTEMA LOTERICO *      */
        /*"      *                 E CCA, EM FUNCAO DA ALTERACAO DAS CARACTERISTI-*      */
        /*"      *                 CAS DOS CAMPOS DE CONTA CORRENTE NA TABELA     *      */
        /*"      *                 FC_CONTA_BANCARIA                              *      */
        /*"      *                                                                *      */
        /*"      * COREON                                       PROCURE POR: V.01 *      */
        /*"      *----------------------------------------------------------------*      */
        /*"      *                                                                *      */
        /*"      *   VERSAO 02 -   NSGD - CADMUS 103659.                          *      */
        /*"      *               - NOVA SOLUCAO DE GESTAO DE DEPOSITOS            *      */
        /*"      *                                                                *      */
        /*"      *   EM 06/07/2015 - COREON                                       *      */
        /*"      *                                                                *      */
        /*"      *                                       PROCURE POR V.02         *      */
        /*"      *                                                                *      */
        /*"      *----------------------------------------------------------------*      */
        /*"      *                                                                *      */
        /*"      *   VERSAO V.03 - ABEND - CADMUS 177676                          *      */
        /*"      *                 INSERT FC-CONTA-BANCARIA - 803                 *      */
        /*"      *                                                                *      */
        /*"      *   EM 04/10/2019 - OLIVEIRA                                     *      */
        /*"      *                                                                *      */
        /*"      *            PROCURE POR V.03                                    *      */
        /*"      *                                                                *      */
        /*"      *----------------------------------------------------------------*      */
        #endregion


        #region VARIABLES

        public FileBasis _CADASTRO { get; set; } = new FileBasis(new PIC("X", "700", "X(700)"));

        public FileBasis CADASTRO
        {
            get
            {
                _.Move(REG_CAD, _CADASTRO); VarBasis.RedefinePassValue(REG_CAD, _CADASTRO, REG_CAD); return _CADASTRO;
            }
        }
        public FileBasis _RLT2000B { get; set; } = new FileBasis(new PIC("X", "0", "X(0)"));

        public FileBasis RLT2000B
        {
            get
            {
                _.Move(REG_RLT2000B, _RLT2000B); VarBasis.RedefinePassValue(REG_RLT2000B, _RLT2000B, REG_RLT2000B); return _RLT2000B;
            }
        }
        /*"01 REG-CAD            PIC X(700).*/
        public StringBasis REG_CAD { get; set; } = new StringBasis(new PIC("X", "700", "X(700)."), @"");
        /*"01 REG-RLT2000B.*/
        public LT2000B_REG_RLT2000B REG_RLT2000B { get; set; } = new LT2000B_REG_RLT2000B();
        public class LT2000B_REG_RLT2000B : VarBasis
        {
            /*" 05 REG-LINHA                     PIC X(132).*/
            public StringBasis REG_LINHA { get; set; } = new StringBasis(new PIC("X", "132", "X(132)."), @"");
        }

        /*"*/
        public IntBasis RETURN_CODE { get; set; } = new IntBasis(new PIC("S9", "04", "S9(04)"));
        /*"77 V0ENDO-DTINIVIG             PIC X(10).*/
        public StringBasis V0ENDO_DTINIVIG { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*"77 V0ENDO-DTTERVIG             PIC X(10).*/
        public StringBasis V0ENDO_DTTERVIG { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*"77 V0SIST-DTMOVABE             PIC X(10).*/
        public StringBasis V0SIST_DTMOVABE { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*"77 V0SIST-TIMESTAMP            PIC X(26).*/
        public StringBasis V0SIST_TIMESTAMP { get; set; } = new StringBasis(new PIC("X", "26", "X(26)."), @"");
        /*"77 WS-OBRIGATORIO      PIC  9(001) VALUE 0.*/
        public IntBasis WS_OBRIGATORIO { get; set; } = new IntBasis(new PIC("9", "1", "9(001)"));
        /*"77 WS-NECESSARIO       PIC  9(001) VALUE 0.*/
        public IntBasis WS_NECESSARIO { get; set; } = new IntBasis(new PIC("9", "1", "9(001)"));
        /*"77 WS-TEM-UF           PIC  9(001) VALUE 0.*/
        public IntBasis WS_TEM_UF { get; set; } = new IntBasis(new PIC("9", "1", "9(001)"));
        /*"77 WS-IMPRIMIU         PIC  9(001) VALUE 0.*/
        public IntBasis WS_IMPRIMIU { get; set; } = new IntBasis(new PIC("9", "1", "9(001)"));
        /*"77 WS-IND              PIC  9(003) VALUE 0.*/
        public IntBasis WS_IND { get; set; } = new IntBasis(new PIC("9", "3", "9(003)"));
        /*"77 WS-IDE-CONTA-CPMF   PIC S9(009) VALUE +0  COMP.*/
        public IntBasis WS_IDE_CONTA_CPMF { get; set; } = new IntBasis(new PIC("S9", "9", "S9(009)"));
        /*"77 WS-IDE-CONTA-ISENTA PIC S9(009) VALUE +0  COMP.*/
        public IntBasis WS_IDE_CONTA_ISENTA { get; set; } = new IntBasis(new PIC("S9", "9", "S9(009)"));
        /*"77 WS-IDE-CONTA-CAUCAO PIC S9(009) VALUE +0  COMP.*/
        public IntBasis WS_IDE_CONTA_CAUCAO { get; set; } = new IntBasis(new PIC("S9", "9", "S9(009)"));
        /*"77 MAX-IDE-CONTA-BANCARIA PIC S9(009) VALUE +0  COMP.*/
        public IntBasis MAX_IDE_CONTA_BANCARIA { get; set; } = new IntBasis(new PIC("S9", "9", "S9(009)"));
        /*"77 W-CHAVE-CADASTRADO-SASSE  PIC X(03) VALUE SPACES.*/
        public StringBasis W_CHAVE_CADASTRADO_SASSE { get; set; } = new StringBasis(new PIC("X", "3", "X(03)"), @"");
        /*"77 W-CHAVE-CADASTRADO-SIGEL  PIC X(03) VALUE SPACES.*/
        public StringBasis W_CHAVE_CADASTRADO_SIGEL { get; set; } = new StringBasis(new PIC("X", "3", "X(03)"), @"");
        /*"77 W-CHAVE-HOUVE-ALTERACAO   PIC X(03) VALUE SPACES.*/
        public StringBasis W_CHAVE_HOUVE_ALTERACAO { get; set; } = new StringBasis(new PIC("X", "3", "X(03)"), @"");
        /*"77 W-CHAVE-ALTEROU-SEGURADO  PIC X(03) VALUE SPACES.*/
        public StringBasis W_CHAVE_ALTEROU_SEGURADO { get; set; } = new StringBasis(new PIC("X", "3", "X(03)"), @"");
        /*"77 W-IND-ALT-FAXTELEME       PIC X(01) VALUE SPACES.*/
        public StringBasis W_IND_ALT_FAXTELEME { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
        /*"77 WS-CKT                    PIC 9(01).*/
        public IntBasis WS_CKT { get; set; } = new IntBasis(new PIC("9", "1", "9(01)."));
        /*"77 WS-COFRE                  PIC 9(01).*/
        public IntBasis WS_COFRE { get; set; } = new IntBasis(new PIC("9", "1", "9(01)."));
        /*"77 WS-ALARME                 PIC 9(01).*/
        public IntBasis WS_ALARME { get; set; } = new IntBasis(new PIC("9", "1", "9(01)."));
        /*"77 VIND-DTTERVIG             PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_DTTERVIG { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-DTINIVIG             PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_DTINIVIG { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-AGENTE-MASTER    PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_AGENTE_MASTER { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-COD-CGC          PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_COD_CGC { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-COD-INSCR-ESTAD  PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_COD_INSCR_ESTAD { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-COD-INSCR-MUNIC  PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_COD_INSCR_MUNIC { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-COD-MUNICIPIO    PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_COD_MUNICIPIO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-COD-UF           PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_COD_UF { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-DES-EMAIL        PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_DES_EMAIL { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-DES-ENDERECO     PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_DES_ENDERECO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-DTH-EXCLUSAO     PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_DTH_EXCLUSAO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-DTH-INCLUSAO     PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_DTH_INCLUSAO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-IDE-CONTA-CAUCAO PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_IDE_CONTA_CAUCAO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-IDE-CONTA-CPMF   PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_IDE_CONTA_CPMF { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-IDE-CONTA-ISENTA PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_IDE_CONTA_ISENTA { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-IND-CAT-LOTERICO PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_IND_CAT_LOTERICO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-IND-STA-LOTERICO PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_IND_STA_LOTERICO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-IND-UNIDADE-SUB  PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_IND_UNIDADE_SUB { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-NOM-BAIRRO       PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_NOM_BAIRRO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-NOM-CONSULTOR    PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_NOM_CONSULTOR { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-NOM-CONTATO1     PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_NOM_CONTATO1 { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-NOM-CONTATO2     PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_NOM_CONTATO2 { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-NOM-FANTASIA     PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_NOM_FANTASIA { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-NOM-MUNICIPIO    PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_NOM_MUNICIPIO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-NOM-RAZAO-SOCIAL PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_NOM_RAZAO_SOCIAL { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-NUM-CEP          PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_NUM_CEP { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-NUM-ENCEF        PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_NUM_ENCEF { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-NUM-LOTER-ANT    PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_NUM_LOTER_ANT { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-NUM-MATR-CON     PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_NUM_MATR_CON { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-NUM-PVCEF        PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_NUM_PVCEF { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-NUM-TELEFONE     PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_NUM_TELEFONE { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-STA-DADOS-N      PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_STA_DADOS_N { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-STA-LOTERICO     PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_STA_LOTERICO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-STA-NIVEL-COMIS  PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_STA_NIVEL_COMIS { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-STA-ULT-ALT      PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_STA_ULT_ALT { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-COD-DETALHE      PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_COD_DETALHE { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-DES-MSG-DESTINO  PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_DES_MSG_DESTINO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-DES-MSG-ORIGEM   PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_DES_MSG_ORIGEM { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-COD-GARANTIA     PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_COD_GARANTIA { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-VLR-GARANTIA     PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_VLR_GARANTIA { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-DTH-GERACAO      PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_DTH_GERACAO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-DATA-CANCELAMENTO PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_DATA_CANCELAMENTO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-TIPO-ALTERACAO    PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_TIPO_ALTERACAO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-DATA-ALTERACAO    PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_DATA_ALTERACAO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-COD-AGENTE-MASTER PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_COD_AGENTE_MASTER { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-NUM-TITULO        PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_NUM_TITULO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-DATA-QUITACAO     PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_DATA_QUITACAO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-BONUS-CKT         PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_BONUS_CKT { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-BONUS-COFRE       PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_BONUS_COFRE { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-BONUS-ALARME      PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_BONUS_ALARME { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-NOME-SEGURADORA   PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_NOME_SEGURADORA { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-TIPO-GARANTIA     PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_TIPO_GARANTIA { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-VALOR-GARANTIA    PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_VALOR_GARANTIA { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 VIND-NUM-FAX           PIC S9(004) VALUE +0  COMP.*/
        public IntBasis VIND_NUM_FAX { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 V0SOL-COD-PRODUTO            PIC S9(004)    VALUE +0 COMP.*/
        public IntBasis V0SOL_COD_PRODUTO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 V0SOL-COD-CLIENTE            PIC S9(009)    VALUE +0 COMP.*/
        public IntBasis V0SOL_COD_CLIENTE { get; set; } = new IntBasis(new PIC("S9", "9", "S9(009)"));
        /*"77 V0SOL-COD-PROGRAMA           PIC X(12).*/
        public StringBasis V0SOL_COD_PROGRAMA { get; set; } = new StringBasis(new PIC("X", "12", "X(12)."), @"");
        /*"77 V0SOL-TIPO-SOLICITACAO       PIC S9(004)    VALUE +0 COMP.*/
        public IntBasis V0SOL_TIPO_SOLICITACAO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 V0SOL-COD-MOVIMENTO          PIC S9(004)    VALUE +0 COMP.*/
        public IntBasis V0SOL_COD_MOVIMENTO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 V0SOL-DATA-SOLICITACAO       PIC X(10).*/
        public StringBasis V0SOL_DATA_SOLICITACAO { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*"77 V0SOL-COD-USUARIO            PIC X(08).*/
        public StringBasis V0SOL_COD_USUARIO { get; set; } = new StringBasis(new PIC("X", "8", "X(08)."), @"");
        /*"77 V0SOL-DATA-PREV-PROC         PIC X(10).*/
        public StringBasis V0SOL_DATA_PREV_PROC { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*"77 V0SOL-SIT-SOLICITACAO        PIC X(01).*/
        public StringBasis V0SOL_SIT_SOLICITACAO { get; set; } = new StringBasis(new PIC("X", "1", "X(01)."), @"");
        /*"77 V0SOL-PARAM-DATE01           PIC X(10).*/
        public StringBasis V0SOL_PARAM_DATE01 { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*"77 V0SOL-PARAM-DATE02           PIC X(10).*/
        public StringBasis V0SOL_PARAM_DATE02 { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*"77 V0SOL-PARAM-DATE03           PIC X(10).*/
        public StringBasis V0SOL_PARAM_DATE03 { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*"77 V0SOL-PARAM-SMINT01          PIC S9(004)    VALUE +0 COMP.*/
        public IntBasis V0SOL_PARAM_SMINT01 { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 V0SOL-PARAM-SMINT02          PIC S9(004)    VALUE +0 COMP.*/
        public IntBasis V0SOL_PARAM_SMINT02 { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 V0SOL-PARAM-SMINT03          PIC S9(004)    VALUE +0 COMP.*/
        public IntBasis V0SOL_PARAM_SMINT03 { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
        /*"77 V0SOL-PARAM-INTG01           PIC S9(009)    VALUE +0 COMP.*/
        public IntBasis V0SOL_PARAM_INTG01 { get; set; } = new IntBasis(new PIC("S9", "9", "S9(009)"));
        /*"77 V0SOL-PARAM-INTG02           PIC S9(009)    VALUE +0 COMP.*/
        public IntBasis V0SOL_PARAM_INTG02 { get; set; } = new IntBasis(new PIC("S9", "9", "S9(009)"));
        /*"77 V0SOL-PARAM-INTG03           PIC S9(009)    VALUE +0 COMP.*/
        public IntBasis V0SOL_PARAM_INTG03 { get; set; } = new IntBasis(new PIC("S9", "9", "S9(009)"));
        /*"77 V0SOL-PARAM-DEC01            PIC S9(017)    VALUE +0 COMP-3.*/
        public IntBasis V0SOL_PARAM_DEC01 { get; set; } = new IntBasis(new PIC("S9", "17", "S9(017)"));
        /*"77 V0SOL-PARAM-DEC02            PIC S9(017)    VALUE +0 COMP-3.*/
        public IntBasis V0SOL_PARAM_DEC02 { get; set; } = new IntBasis(new PIC("S9", "17", "S9(017)"));
        /*"77 V0SOL-PARAM-DEC03            PIC S9(017)    VALUE +0 COMP-3.*/
        public IntBasis V0SOL_PARAM_DEC03 { get; set; } = new IntBasis(new PIC("S9", "17", "S9(017)"));
        /*"77 V0SOL-PARAM-FLOAT01          PIC S9(008)    VALUE +0 COMP-3.*/
        public IntBasis V0SOL_PARAM_FLOAT01 { get; set; } = new IntBasis(new PIC("S9", "8", "S9(008)"));
        /*"77 V0SOL-PARAM-FLOAT02          PIC S9(008)    VALUE +0 COMP-3.*/
        public IntBasis V0SOL_PARAM_FLOAT02 { get; set; } = new IntBasis(new PIC("S9", "8", "S9(008)"));
        /*"77 V0SOL-PARAM-CHAR01           PIC X(60).*/
        public StringBasis V0SOL_PARAM_CHAR01 { get; set; } = new StringBasis(new PIC("X", "60", "X(60)."), @"");
        /*"77 V0SOL-PARAM-CHAR02           PIC X(30).*/
        public StringBasis V0SOL_PARAM_CHAR02 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)."), @"");
        /*"77 V0SOL-PARAM-CHAR03           PIC X(15).*/
        public StringBasis V0SOL_PARAM_CHAR03 { get; set; } = new StringBasis(new PIC("X", "15", "X(15)."), @"");
        /*"77 V0SOL-PARAM-CHAR04           PIC X(15).*/
        public StringBasis V0SOL_PARAM_CHAR04 { get; set; } = new StringBasis(new PIC("X", "15", "X(15)."), @"");
        /*"77 V1EMPR-NOM-EMP      PIC  X(040).*/
        public StringBasis V1EMPR_NOM_EMP { get; set; } = new StringBasis(new PIC("X", "40", "X(040)."), @"");
        /*"77 V0LOT-NUM-APOLICE           PIC S9(013) VALUE +0 COMP-3.*/
        public IntBasis V0LOT_NUM_APOLICE { get; set; } = new IntBasis(new PIC("S9", "13", "S9(013)"));
        /*"77 V0LOT-COD-LOT-FENAL         PIC S9(009) VALUE +0 COMP.*/
        public IntBasis V0LOT_COD_LOT_FENAL { get; set; } = new IntBasis(new PIC("S9", "9", "S9(009)"));
        /*"77 V0LOT-COD-LOT-CEF           PIC S9(009) VALUE +0 COMP.*/
        public IntBasis V0LOT_COD_LOT_CEF { get; set; } = new IntBasis(new PIC("S9", "9", "S9(009)"));
        /*"77 WS-NUM-APOLICE             PIC S9(013) VALUE +0 COMP-3.*/
        public IntBasis WS_NUM_APOLICE { get; set; } = new IntBasis(new PIC("S9", "13", "S9(013)"));
        /*"77 WS-CONTADOR                PIC S9(009) VALUE +0 COMP.*/
        public IntBasis WS_CONTADOR { get; set; } = new IntBasis(new PIC("S9", "9", "S9(009)"));
        /*"77 HOST-DATA-INI-1P                 PIC X(10).*/
        public StringBasis HOST_DATA_INI_1P { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*"77 HOST-DATA-FIM-1P                 PIC X(10).*/
        public StringBasis HOST_DATA_FIM_1P { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*"01 AREA-DE-WORK.*/
        public LT2000B_AREA_DE_WORK AREA_DE_WORK { get; set; } = new LT2000B_AREA_DE_WORK();
        public class LT2000B_AREA_DE_WORK : VarBasis
        {
            /*" 05 LK-LINK.*/
            public LT2000B_LK_LINK LK_LINK { get; set; } = new LT2000B_LK_LINK();
            public class LT2000B_LK_LINK : VarBasis
            {
                /*"  10 LK-RTCODE                  PIC S9(004) VALUE +0   COMP*/
                public IntBasis LK_RTCODE { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"));
                /*"  10 LK-TAMANHO                 PIC S9(004) VALUE +40  COMP*/
                public IntBasis LK_TAMANHO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(004)"), +40);
                /*"  10 LK-TITULO                  PIC  X(132) VALUE  SPACES.*/
                public StringBasis LK_TITULO { get; set; } = new StringBasis(new PIC("X", "132", "X(132)"), @"");
                /*" 05 WSL-SQLCODE                   PIC 9(09) VALUE ZEROS.*/
            }
            public IntBasis WSL_SQLCODE { get; set; } = new IntBasis(new PIC("9", "9", "9(09)"));
            /*" 05 W-AC-LINHA                    PIC 9(03) VALUE  80.*/
            public IntBasis W_AC_LINHA { get; set; } = new IntBasis(new PIC("9", "3", "9(03)"), 80);
            /*" 05 W-AC-PAGINA                   PIC 9(04) VALUE  ZEROS.*/
            public IntBasis W_AC_PAGINA { get; set; } = new IntBasis(new PIC("9", "4", "9(04)"));
            /*" 05 WS-RESTO              PIC 9(05) VALUE ZEROS.*/
            public IntBasis WS_RESTO { get; set; } = new IntBasis(new PIC("9", "5", "9(05)"));
            /*" 05 WS-AUX                PIC 9(05) VALUE ZEROS.*/
            public IntBasis WS_AUX { get; set; } = new IntBasis(new PIC("9", "5", "9(05)"));
            /*" 05 WFIM-CADASTRO                 PIC X(03) VALUE SPACES.*/
            public StringBasis WFIM_CADASTRO { get; set; } = new StringBasis(new PIC("X", "3", "X(03)"), @"");
            /*" 05 WFIM-MOVIMENTO                PIC X(03) VALUE SPACES.*/
            public StringBasis WFIM_MOVIMENTO { get; set; } = new StringBasis(new PIC("X", "3", "X(03)"), @"");
            /*" 05 WFIM-RELATORIOS               PIC X(03) VALUE SPACES.*/
            public StringBasis WFIM_RELATORIOS { get; set; } = new StringBasis(new PIC("X", "3", "X(03)"), @"");
            /*" 05 WFIM-PARAM                    PIC X(03) VALUE SPACES.*/
            public StringBasis WFIM_PARAM { get; set; } = new StringBasis(new PIC("X", "3", "X(03)"), @"");
            /*" 05 W-CHAVE-TEM-CRITICA           PIC X(03) VALUE SPACES.*/
            public StringBasis W_CHAVE_TEM_CRITICA { get; set; } = new StringBasis(new PIC("X", "3", "X(03)"), @"");
            /*" 05 WS-NOME-BAIRRO                PIC X(30).*/
            public StringBasis WS_NOME_BAIRRO { get; set; } = new StringBasis(new PIC("X", "30", "X(30)."), @"");
            /*" 05 WS-NOME-BAIRRO-R REDEFINES WS-NOME-BAIRRO.*/
            private _REDEF_LT2000B_WS_NOME_BAIRRO_R _ws_nome_bairro_r { get; set; }
            public _REDEF_LT2000B_WS_NOME_BAIRRO_R WS_NOME_BAIRRO_R
            {
                get { _ws_nome_bairro_r = new _REDEF_LT2000B_WS_NOME_BAIRRO_R(); _.Move(WS_NOME_BAIRRO, _ws_nome_bairro_r); VarBasis.RedefinePassValue(WS_NOME_BAIRRO, _ws_nome_bairro_r, WS_NOME_BAIRRO); _ws_nome_bairro_r.ValueChanged += () => { _.Move(_ws_nome_bairro_r, WS_NOME_BAIRRO); }; return _ws_nome_bairro_r; }
                set { VarBasis.RedefinePassValue(value, _ws_nome_bairro_r, WS_NOME_BAIRRO); }
            }  //Redefines
            public class _REDEF_LT2000B_WS_NOME_BAIRRO_R : VarBasis
            {
                /*"  10 WS-BAIRRO                  PIC X(20).*/
                public StringBasis WS_BAIRRO { get; set; } = new StringBasis(new PIC("X", "20", "X(20)."), @"");
                /*"  10 FILLER                     PIC X(10).*/
                public StringBasis FILLER_0 { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
                /*" 05 WS-NOME-CIDADE                PIC X(30).*/

                public _REDEF_LT2000B_WS_NOME_BAIRRO_R()
                {
                    WS_BAIRRO.ValueChanged += OnValueChanged;
                    FILLER_0.ValueChanged += OnValueChanged;
                }

            }
            public StringBasis WS_NOME_CIDADE { get; set; } = new StringBasis(new PIC("X", "30", "X(30)."), @"");
            /*" 05 WS-NOME-CIDADE-R REDEFINES WS-NOME-CIDADE.*/
            private _REDEF_LT2000B_WS_NOME_CIDADE_R _ws_nome_cidade_r { get; set; }
            public _REDEF_LT2000B_WS_NOME_CIDADE_R WS_NOME_CIDADE_R
            {
                get { _ws_nome_cidade_r = new _REDEF_LT2000B_WS_NOME_CIDADE_R(); _.Move(WS_NOME_CIDADE, _ws_nome_cidade_r); VarBasis.RedefinePassValue(WS_NOME_CIDADE, _ws_nome_cidade_r, WS_NOME_CIDADE); _ws_nome_cidade_r.ValueChanged += () => { _.Move(_ws_nome_cidade_r, WS_NOME_CIDADE); }; return _ws_nome_cidade_r; }
                set { VarBasis.RedefinePassValue(value, _ws_nome_cidade_r, WS_NOME_CIDADE); }
            }  //Redefines
            public class _REDEF_LT2000B_WS_NOME_CIDADE_R : VarBasis
            {
                /*"  10 WS-CIDADE                  PIC X(25).*/
                public StringBasis WS_CIDADE { get; set; } = new StringBasis(new PIC("X", "25", "X(25)."), @"");
                /*"  10 FILLER                     PIC X(05).*/
                public StringBasis FILLER_1 { get; set; } = new StringBasis(new PIC("X", "5", "X(05)."), @"");
                /*" 05 W-MOV-COD-FENAL               PIC 9(07) VALUE  ZEROS.*/

                public _REDEF_LT2000B_WS_NOME_CIDADE_R()
                {
                    WS_CIDADE.ValueChanged += OnValueChanged;
                    FILLER_1.ValueChanged += OnValueChanged;
                }

            }
            public IntBasis W_MOV_COD_FENAL { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 REG-CAD-HEADER.*/
            public LT2000B_REG_CAD_HEADER REG_CAD_HEADER { get; set; } = new LT2000B_REG_CAD_HEADER();
            public class LT2000B_REG_CAD_HEADER : VarBasis
            {
                /*"   15 CAD-TIPO-HEADERX              PIC X(001).*/
                public StringBasis CAD_TIPO_HEADERX { get; set; } = new StringBasis(new PIC("X", "1", "X(001)."), @"");
                /*"   15 CAD-COD-REMESSAX.*/
                public LT2000B_CAD_COD_REMESSAX CAD_COD_REMESSAX { get; set; } = new LT2000B_CAD_COD_REMESSAX();
                public class LT2000B_CAD_COD_REMESSAX : VarBasis
                {
                    /*"    20 CAD-REMESSA                PIC 9(001).*/
                    public IntBasis CAD_REMESSA { get; set; } = new IntBasis(new PIC("9", "1", "9(001)."));
                    /*"   15 CAD-EMPRESA                   PIC X(020).*/
                }
                public StringBasis CAD_EMPRESA { get; set; } = new StringBasis(new PIC("X", "20", "X(020)."), @"");
                /*"   15 CAD-DATA-GERACAOX.*/
                public LT2000B_CAD_DATA_GERACAOX CAD_DATA_GERACAOX { get; set; } = new LT2000B_CAD_DATA_GERACAOX();
                public class LT2000B_CAD_DATA_GERACAOX : VarBasis
                {
                    /*"    20 CAD-DATA-GERACAO           PIC 9(008).*/
                    public IntBasis CAD_DATA_GERACAO { get; set; } = new IntBasis(new PIC("9", "8", "9(008)."));
                    /*"   15 CAD-NUM-SEQX.*/
                }
                public LT2000B_CAD_NUM_SEQX CAD_NUM_SEQX { get; set; } = new LT2000B_CAD_NUM_SEQX();
                public class LT2000B_CAD_NUM_SEQX : VarBasis
                {
                    /*"    20 CAD-NUM-SEQ                PIC 9(006).*/
                    public IntBasis CAD_NUM_SEQ { get; set; } = new IntBasis(new PIC("9", "6", "9(006)."));
                    /*"   15 CAD-VERSAOX.*/
                }
                public LT2000B_CAD_VERSAOX CAD_VERSAOX { get; set; } = new LT2000B_CAD_VERSAOX();
                public class LT2000B_CAD_VERSAOX : VarBasis
                {
                    /*"    20 CAD-VERSAO                 PIC 9(002).*/
                    public IntBasis CAD_VERSAO { get; set; } = new IntBasis(new PIC("9", "2", "9(002)."));
                    /*"    20 CAD-ESPACO                    PIC X(656).*/
                    public StringBasis CAD_ESPACO { get; set; } = new StringBasis(new PIC("X", "656", "X(656)."), @"");
                    /*"   15 CAD-SEQ-REGX.*/
                }
                public LT2000B_CAD_SEQ_REGX CAD_SEQ_REGX { get; set; } = new LT2000B_CAD_SEQ_REGX();
                public class LT2000B_CAD_SEQ_REGX : VarBasis
                {
                    /*"    20 CAD-SEQ-REG                PIC 9(006).*/
                    public IntBasis CAD_SEQ_REG { get; set; } = new IntBasis(new PIC("9", "6", "9(006)."));
                    /*" 05 REG-CAD-TRAILLER.*/
                }
            }
            public LT2000B_REG_CAD_TRAILLER REG_CAD_TRAILLER { get; set; } = new LT2000B_REG_CAD_TRAILLER();
            public class LT2000B_REG_CAD_TRAILLER : VarBasis
            {
                /*"   15 CAD-TIPO-TRAILLERX            PIC X(001).*/
                public StringBasis CAD_TIPO_TRAILLERX { get; set; } = new StringBasis(new PIC("X", "1", "X(001)."), @"");
                /*"   15 CAD-TOTALX.*/
                public LT2000B_CAD_TOTALX CAD_TOTALX { get; set; } = new LT2000B_CAD_TOTALX();
                public class LT2000B_CAD_TOTALX : VarBasis
                {
                    /*"    20 CAD-TOTAL                  PIC 9(006).*/
                    public IntBasis CAD_TOTAL { get; set; } = new IntBasis(new PIC("9", "6", "9(006)."));
                    /*"   15 FILLER                        PIC X(687).*/
                }
                public StringBasis FILLER_2 { get; set; } = new StringBasis(new PIC("X", "687", "X(687)."), @"");
                /*"   15 CAD-NSR-TRAX.*/
                public LT2000B_CAD_NSR_TRAX CAD_NSR_TRAX { get; set; } = new LT2000B_CAD_NSR_TRAX();
                public class LT2000B_CAD_NSR_TRAX : VarBasis
                {
                    /*"    20 CAD-NSR-TRA                PIC 9(006).*/
                    public IntBasis CAD_NSR_TRA { get; set; } = new IntBasis(new PIC("9", "6", "9(006)."));
                    /*" 05 REG-CAD-CADASTRO.*/
                }
            }
            public LT2000B_REG_CAD_CADASTRO REG_CAD_CADASTRO { get; set; } = new LT2000B_REG_CAD_CADASTRO();
            public class LT2000B_REG_CAD_CADASTRO : VarBasis
            {
                /*"   15 CAD-TIPOX.*/
                public LT2000B_CAD_TIPOX CAD_TIPOX { get; set; } = new LT2000B_CAD_TIPOX();
                public class LT2000B_CAD_TIPOX : VarBasis
                {
                    /*"    20 CAD-TIPO                   PIC X(01).*/
                    public StringBasis CAD_TIPO { get; set; } = new StringBasis(new PIC("X", "1", "X(01)."), @"");
                    /*"   15 CAD-COD-CEFX.*/
                }
                public LT2000B_CAD_COD_CEFX CAD_COD_CEFX { get; set; } = new LT2000B_CAD_COD_CEFX();
                public class LT2000B_CAD_COD_CEFX : VarBasis
                {
                    /*"    20 CAD-CODIGO-CEF             PIC 9(09).*/
                    public IntBasis CAD_CODIGO_CEF { get; set; } = new IntBasis(new PIC("9", "9", "9(09)."));
                    /*"    20 CAD-CODIGO-CEF-R REDEFINES CAD-CODIGO-CEF.*/
                    private _REDEF_LT2000B_CAD_CODIGO_CEF_R _cad_codigo_cef_r { get; set; }
                    public _REDEF_LT2000B_CAD_CODIGO_CEF_R CAD_CODIGO_CEF_R
                    {
                        get { _cad_codigo_cef_r = new _REDEF_LT2000B_CAD_CODIGO_CEF_R(); _.Move(CAD_CODIGO_CEF, _cad_codigo_cef_r); VarBasis.RedefinePassValue(CAD_CODIGO_CEF, _cad_codigo_cef_r, CAD_CODIGO_CEF); _cad_codigo_cef_r.ValueChanged += () => { _.Move(_cad_codigo_cef_r, CAD_CODIGO_CEF); }; return _cad_codigo_cef_r; }
                        set { VarBasis.RedefinePassValue(value, _cad_codigo_cef_r, CAD_CODIGO_CEF); }
                    }  //Redefines
                    public class _REDEF_LT2000B_CAD_CODIGO_CEF_R : VarBasis
                    {
                        /*"     25 CAD-COD-CEF             PIC 9(08).*/
                        public IntBasis CAD_COD_CEF { get; set; } = new IntBasis(new PIC("9", "8", "9(08)."));
                        /*"     25 CAD-DV-CEF              PIC 9(01).*/
                        public IntBasis CAD_DV_CEF { get; set; } = new IntBasis(new PIC("9", "1", "9(01)."));
                        /*"   15 CAD-RAZAO-SOCIAL              PIC X(40).*/

                        public _REDEF_LT2000B_CAD_CODIGO_CEF_R()
                        {
                            CAD_COD_CEF.ValueChanged += OnValueChanged;
                            CAD_DV_CEF.ValueChanged += OnValueChanged;
                        }

                    }
                }
                public StringBasis CAD_RAZAO_SOCIAL { get; set; } = new StringBasis(new PIC("X", "40", "X(40)."), @"");
                /*"   15 CAD-NOME-FANTASIA             PIC X(30).*/
                public StringBasis CAD_NOME_FANTASIA { get; set; } = new StringBasis(new PIC("X", "30", "X(30)."), @"");
                /*"   15 CAD-ENDERECO.*/
                public LT2000B_CAD_ENDERECO CAD_ENDERECO { get; set; } = new LT2000B_CAD_ENDERECO();
                public class LT2000B_CAD_ENDERECO : VarBasis
                {
                    /*"    20 CAD-END                    PIC X(40).*/
                    public StringBasis CAD_END { get; set; } = new StringBasis(new PIC("X", "40", "X(40)."), @"");
                    /*"    20 CAD-COMPL-END              PIC X(16).*/
                    public StringBasis CAD_COMPL_END { get; set; } = new StringBasis(new PIC("X", "16", "X(16)."), @"");
                    /*"   15 CAD-BAIRRO                    PIC X(20).*/
                }
                public StringBasis CAD_BAIRRO { get; set; } = new StringBasis(new PIC("X", "20", "X(20)."), @"");
                /*"   15 CAD-COD-MUNICIPIO             PIC X(12).*/
                public StringBasis CAD_COD_MUNICIPIO { get; set; } = new StringBasis(new PIC("X", "12", "X(12)."), @"");
                /*"   15 CAD-CIDADE                    PIC X(25).*/
                public StringBasis CAD_CIDADE { get; set; } = new StringBasis(new PIC("X", "25", "X(25)."), @"");
                /*"   15 CAD-CEPX.*/
                public LT2000B_CAD_CEPX CAD_CEPX { get; set; } = new LT2000B_CAD_CEPX();
                public class LT2000B_CAD_CEPX : VarBasis
                {
                    /*"    20 CAD-CEP                    PIC 9(08).*/
                    public IntBasis CAD_CEP { get; set; } = new IntBasis(new PIC("9", "8", "9(08)."));
                    /*"    20 FILLER                     PIC X(01).*/
                    public StringBasis FILLER_3 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)."), @"");
                    /*"   15 CAD-UF                        PIC X(02).*/
                }
                public StringBasis CAD_UF { get; set; } = new StringBasis(new PIC("X", "2", "X(02)."), @"");
                /*"   15 CAD-TELEFONE.*/
                public LT2000B_CAD_TELEFONE CAD_TELEFONE { get; set; } = new LT2000B_CAD_TELEFONE();
                public class LT2000B_CAD_TELEFONE : VarBasis
                {
                    /*"    20 CAD-DDD-FONE               PIC 9(04).*/
                    public IntBasis CAD_DDD_FONE { get; set; } = new IntBasis(new PIC("9", "4", "9(04)."));
                    /*"    20 CAD-FONE                   PIC 9(08).*/
                    public IntBasis CAD_FONE { get; set; } = new IntBasis(new PIC("9", "8", "9(08)."));
                    /*"   15 FILLER                        PIC X(04).*/
                }
                public StringBasis FILLER_4 { get; set; } = new StringBasis(new PIC("X", "4", "X(04)."), @"");
                /*"   15 CAD-CONTATO1                  PIC X(20).*/
                public StringBasis CAD_CONTATO1 { get; set; } = new StringBasis(new PIC("X", "20", "X(20)."), @"");
                /*"   15 CAD-CONTATO2                  PIC X(20).*/
                public StringBasis CAD_CONTATO2 { get; set; } = new StringBasis(new PIC("X", "20", "X(20)."), @"");
                /*"   15 CAD-CGCX.*/
                public LT2000B_CAD_CGCX CAD_CGCX { get; set; } = new LT2000B_CAD_CGCX();
                public class LT2000B_CAD_CGCX : VarBasis
                {
                    /*"    20 CAD-CGC                    PIC 9(15).*/
                    public IntBasis CAD_CGC { get; set; } = new IntBasis(new PIC("9", "15", "9(15)."));
                    /*"   15 CAD-INSC-MUNX.*/
                }
                public LT2000B_CAD_INSC_MUNX CAD_INSC_MUNX { get; set; } = new LT2000B_CAD_INSC_MUNX();
                public class LT2000B_CAD_INSC_MUNX : VarBasis
                {
                    /*"    20 CAD-INSC-MUN               PIC 9(15).*/
                    public IntBasis CAD_INSC_MUN { get; set; } = new IntBasis(new PIC("9", "15", "9(15)."));
                    /*"   15 CAD-INSC-ESTX.*/
                }
                public LT2000B_CAD_INSC_ESTX CAD_INSC_ESTX { get; set; } = new LT2000B_CAD_INSC_ESTX();
                public class LT2000B_CAD_INSC_ESTX : VarBasis
                {
                    /*"    20 CAD-INSC-EST               PIC 9(15).*/
                    public IntBasis CAD_INSC_EST { get; set; } = new IntBasis(new PIC("9", "15", "9(15)."));
                    /*"   15 CAD-SITUACAOX.*/
                }
                public LT2000B_CAD_SITUACAOX CAD_SITUACAOX { get; set; } = new LT2000B_CAD_SITUACAOX();
                public class LT2000B_CAD_SITUACAOX : VarBasis
                {
                    /*"    20 CAD-SITUACAO               PIC 9(01).*/
                    public IntBasis CAD_SITUACAO { get; set; } = new IntBasis(new PIC("9", "1", "9(01)."));
                    /*"   15 CAD-DATA-INCLUSAOX.*/
                }
                public LT2000B_CAD_DATA_INCLUSAOX CAD_DATA_INCLUSAOX { get; set; } = new LT2000B_CAD_DATA_INCLUSAOX();
                public class LT2000B_CAD_DATA_INCLUSAOX : VarBasis
                {
                    /*"    20 CAD-DATA-INCLUSAO          PIC 9(08).*/
                    public IntBasis CAD_DATA_INCLUSAO { get; set; } = new IntBasis(new PIC("9", "8", "9(08)."));
                    /*"   15 CAD-DATA-EXCLUSAOX.*/
                }
                public LT2000B_CAD_DATA_EXCLUSAOX CAD_DATA_EXCLUSAOX { get; set; } = new LT2000B_CAD_DATA_EXCLUSAOX();
                public class LT2000B_CAD_DATA_EXCLUSAOX : VarBasis
                {
                    /*"    20 CAD-DATA-EXCLUSAO          PIC 9(08).*/
                    public IntBasis CAD_DATA_EXCLUSAO { get; set; } = new IntBasis(new PIC("9", "8", "9(08)."));
                    /*"   15 CAD-NUM-LOT-ANTERIORX.*/
                }
                public LT2000B_CAD_NUM_LOT_ANTERIORX CAD_NUM_LOT_ANTERIORX { get; set; } = new LT2000B_CAD_NUM_LOT_ANTERIORX();
                public class LT2000B_CAD_NUM_LOT_ANTERIORX : VarBasis
                {
                    /*"    20 CAD-NUM-LOT-ANTERIOR       PIC 9(10).*/
                    public IntBasis CAD_NUM_LOT_ANTERIOR { get; set; } = new IntBasis(new PIC("9", "10", "9(10)."));
                    /*"   15 CAD-COD-AG-MASTERX.*/
                }
                public LT2000B_CAD_COD_AG_MASTERX CAD_COD_AG_MASTERX { get; set; } = new LT2000B_CAD_COD_AG_MASTERX();
                public class LT2000B_CAD_COD_AG_MASTERX : VarBasis
                {
                    /*"    20 CAD-COD-AG-MASTER          PIC 9(09).*/
                    public IntBasis CAD_COD_AG_MASTER { get; set; } = new IntBasis(new PIC("9", "9", "9(09)."));
                    /*"   15 CAD-BONUS-CKTX              PIC 9(01).*/
                    public IntBasis CAD_BONUS_CKTX { get; set; } = new IntBasis(new PIC("9", "1", "9(01)."));
                    /*"   15 CAD-BONUS-ALARMEX           PIC 9(01).*/
                    public IntBasis CAD_BONUS_ALARMEX { get; set; } = new IntBasis(new PIC("9", "1", "9(01)."));
                    /*"   15 CAD-BONUS-COFREX            PIC 9(01).*/
                    public IntBasis CAD_BONUS_COFREX { get; set; } = new IntBasis(new PIC("9", "1", "9(01)."));
                    /*"   15 CAD-NOME-SEGURADORA         PIC X(04).*/
                    public StringBasis CAD_NOME_SEGURADORA { get; set; } = new StringBasis(new PIC("X", "4", "X(04)."), @"");
                    /*"   15 CAD-TIPO-GARANTIA           PIC 9(01).*/
                    public IntBasis CAD_TIPO_GARANTIA { get; set; } = new IntBasis(new PIC("9", "1", "9(01)."));
                    /*"   15 CAD-VALOR-GARANTIA          PIC 9(04).*/
                    public IntBasis CAD_VALOR_GARANTIA { get; set; } = new IntBasis(new PIC("9", "4", "9(04)."));
                    /*"   15 CAD-NUM-FAX                 PIC 9(04).*/
                    public IntBasis CAD_NUM_FAX { get; set; } = new IntBasis(new PIC("9", "4", "9(04)."));
                    /*"   15 CAD-VERSAO                  PIC 9(02).*/
                    public IntBasis CAD_VERSAO { get; set; } = new IntBasis(new PIC("9", "2", "9(02)."));
                    /*"   15 CAD-ESPACO                  PIC X(6
                    /*"   15 CAD-BONUS-CKTX              PIC 9(01).*/
                    public IntBasis CAD_BONUS_CKTX { get; set; } = new IntBasis(new PIC("9", "1", "9(01)."));
                    /*"   15 CAD-BONUS-ALARMEX           PIC 9(01).*/
                    public IntBasis CAD_BONUS_ALARMEX { get; set; } = new IntBasis(new PIC("9", "1", "9(01)."));
                    /*"   15 CAD-BONUS-COFREX            PIC 9(01).*/
                    public IntBasis CAD_BONUS_COFREX { get; set; } = new IntBasis(new PIC("9", "1", "9(01)."));
                    /*"   15 CAD-NOME-SEGURADORA         PIC X(04).*/
                    public StringBasis CAD_NOME_SEGURADORA { get; set; } = new StringBasis(new PIC("X", "4", "X(04)."), @"");
                    /*"   15 CAD-TIPO-GARANTIA           PIC 9(01).*/
                    public IntBasis CAD_TIPO_GARANTIA { get; set; } = new IntBasis(new PIC("9", "1", "9(01)."));
                    /*"   15 CAD-VALOR-GARANTIA          PIC 9(04).*/
                    public IntBasis CAD_VALOR_GARANTIA { get; set; } = new IntBasis(new PIC("9", "4", "9(04)."));
                    /*"   15 CAD-NUM-FAX                 PIC 9(04).*/
                    public IntBasis CAD_NUM_FAX { get; set; } = new IntBasis(new PIC("9", "4", "9(04)."));
                    /*"   15 CAD-VERSAO                  PIC 9(02).*/
                    public IntBasis CAD_VERSAO { get; set; } = new IntBasis(new PIC("9", "2", "9(02)."));
                    /*"   15 CAD-ESPACO                  PIC X(6
                    /*"   15 CAD-CAT-LOTERICOX.*/
                }
                public StringBasis CAD_BONUS_CKTX { get; set; } = new StringBasis(new PIC("X", "1", "X(01)."), @"");
                public StringBasis CAD_BONUS_ALARMEX { get; set; } = new StringBasis(new PIC("X", "1", "X(01)."), @"");
                public StringBasis CAD_BONUS_COFREX { get; set; } = new StringBasis(new PIC("X", "1", "X(01)."), @"");
                public StringBasis CAD_NOME_SEGURADORA { get; set; } = new StringBasis(new PIC("X", "40", "X(40)."), @"");
                public StringBasis CAD_TIPO_GARANTIA { get; set; } = new StringBasis(new PIC("X", "4", "X(04)."), @"");
                public StringBasis CAD_VALOR_GARANTIA { get; set; } = new StringBasis(new PIC("X", "15", "X(15)."), @"");
                public StringBasis CAD_NUM_FAX { get; set; } = new StringBasis(new PIC("X", "15", "X(15)."), @"");
                public StringBasis CAD_CGC { get; set; } = new StringBasis(new PIC("X", "15", "X(15)."), @"");
                public StringBasis CAD_INSC_MUN { get; set; } = new StringBasis(new PIC("X", "15", "X(15)."), @"");
                public StringBasis CAD_INSC_EST { get; set; } = new StringBasis(new PIC("X", "15", "X(15)."), @"");
                public StringBasis CAD_SITUACAO { get; set; } = new StringBasis(new PIC("X", "1", "X(01)."), @"");
                public StringBasis CAD_DATA_INCLUSAO { get; set; } = new StringBasis(new PIC("X", "8", "X(08)."), @"");
                public StringBasis CAD_DATA_EXCLUSAO { get; set; } = new StringBasis(new PIC("X", "8", "X(08)."), @"");
                public LT2000B_CAD_CAT_LOTERICOX CAD_CAT_LOTERICOX { get; set; } = new LT2000B_CAD_CAT_LOTERICOX();
                public class LT2000B_CAD_CAT_LOTERICOX : VarBasis
                {
                    /*"    20 CAD-CAT-LOTERICO           PIC 9(02).*/
                    public IntBasis CAD_CAT_LOTERICO { get; set; } = new IntBasis(new PIC("9", "2", "9(02)."));
                    /*"   15 CAD-COD-STATUSX.*/
                }
                public LT2000B_CAD_COD_STATUSX CAD_COD_STATUSX { get; set; } = new LT2000B_CAD_COD_STATUSX();
                public class LT2000B_CAD_COD_STATUSX : VarBasis
                {
                    /*"    20 CAD-COD-STATUS             PIC 9(02).*/
                    public IntBasis CAD_COD_STATUS { get; set; } = new IntBasis(new PIC("9", "2", "9(02)."));
                    /*"   15 CAD-BANCO-DESC-CPMFX.*/
                }
                public LT2000B_CAD_BANCO_DESC_CPMFX CAD_BANCO_DESC_CPMFX { get; set; } = new LT2000B_CAD_BANCO_DESC_CPMFX();
                public class LT2000B_CAD_BANCO_DESC_CPMFX : VarBasis
                {
                    /*"    20 CAD-BANCO-DESC-CPMF        PIC 9(09).*/
                    public IntBasis CAD_BANCO_DESC_CPMF { get; set; } = new IntBasis(new PIC("9", "9", "9(09)."));
                    /*"   15 CAD-AGEN-DESC-CPMFX.*/
                }
                public LT2000B_CAD_AGEN_DESC_CPMFX CAD_AGEN_DESC_CPMFX { get; set; } = new LT2000B_CAD_AGEN_DESC_CPMFX();
                public class LT2000B_CAD_AGEN_DESC_CPMFX : VarBasis
                {
                    /*"    20 CAD-AGEN-DESC-CPMF         PIC 9(05).*/
                    public IntBasis CAD_AGEN_DESC_CPMF { get; set; } = new IntBasis(new PIC("9", "5", "9(05)."));
                    /*"   15 CAD-CONTA-DESC-CPMFX.*/
                }
                public LT2000B_CAD_CONTA_DESC_CPMFX CAD_CONTA_DESC_CPMFX { get; set; } = new LT2000B_CAD_CONTA_DESC_CPMFX();
                public class LT2000B_CAD_CONTA_DESC_CPMFX : VarBasis
                {
                    /*"    20 CAD-CONTA-DESC-CPMF        PIC 9(17).*/
                    public IntBasis CAD_CONTA_DESC_CPMF { get; set; } = new IntBasis(new PIC("9", "17", "9(17)."));
                    /*"   15 CAD-BANCO-ISENTAX.*/
                }
                public LT2000B_CAD_BANCO_ISENTAX CAD_BANCO_ISENTAX { get; set; } = new LT2000B_CAD_BANCO_ISENTAX();
                public class LT2000B_CAD_BANCO_ISENTAX : VarBasis
                {
                    /*"    20 CAD-BANCO-ISENTA             PIC 9(09).*/
                    public IntBasis CAD_BANCO_ISENTA { get; set; } = new IntBasis(new PIC("9", "9", "9(09)."));
                    /*"   15 CAD-AGEN-ISENTAX.*/
                }
                public LT2000B_CAD_AGEN_ISENTAX CAD_AGEN_ISENTAX { get; set; } = new LT2000B_CAD_AGEN_ISENTAX();
                public class LT2000B_CAD_AGEN_ISENTAX : VarBasis
                {
                    /*"    20 CAD-AGEN-ISENTA              PIC 9(05).*/
                    public IntBasis CAD_AGEN_ISENTA { get; set; } = new IntBasis(new PIC("9", "5", "9(05)."));
                    /*"   15 CAD-CONTA-ISENTAX.*/
                }
                public LT2000B_CAD_CONTA_ISENTAX CAD_CONTA_ISENTAX { get; set; } = new LT2000B_CAD_CONTA_ISENTAX();
                public class LT2000B_CAD_CONTA_ISENTAX : VarBasis
                {
                    /*"    20 CAD-CONTA-ISENTA             PIC 9(17).*/
                    public IntBasis CAD_CONTA_ISENTA { get; set; } = new IntBasis(new PIC("9", "17", "9(17)."));
                    /*"   15 CAD-BANCO-CAUCAOX.*/
                }
                public LT2000B_CAD_BANCO_CAUCAOX CAD_BANCO_CAUCAOX { get; set; } = new LT2000B_CAD_BANCO_CAUCAOX();
                public class LT2000B_CAD_BANCO_CAUCAOX : VarBasis
                {
                    /*"    20 CAD-BANCO-CAUCAO             PIC 9(09).*/
                    public IntBasis CAD_BANCO_CAUCAO { get; set; } = new IntBasis(new PIC("9", "9", "9(09)."));
                    /*"   15 CAD-AGEN-CAUCAOX.*/
                }
                public LT2000B_CAD_AGEN_CAUCAOX CAD_AGEN_CAUCAOX { get; set; } = new LT2000B_CAD_AGEN_CAUCAOX();
                public class LT2000B_CAD_AGEN_CAUCAOX : VarBasis
                {
                    /*"    20 CAD-AGEN-CAUCAO              PIC 9(05).*/
                    public IntBasis CAD_AGEN_CAUCAO { get; set; } = new IntBasis(new PIC("9", "5", "9(05)."));
                    /*"   15 CAD-CONTA-CAUCAOX.*/
                }
                public LT2000B_CAD_CONTA_CAUCAOX CAD_CONTA_CAUCAOX { get; set; } = new LT2000B_CAD_CONTA_CAUCAOX();
                public class LT2000B_CAD_CONTA_CAUCAOX : VarBasis
                {
                    /*"    20 CAD-CONTA-CAUCAO             PIC 9(17).*/
                    public IntBasis CAD_CONTA_CAUCAO { get; set; } = new IntBasis(new PIC("9", "17", "9(17)."));
                    /*"   15 CAD-NIVEL-COMISSAOX.*/
                }
                public LT2000B_CAD_NIVEL_COMISSAOX CAD_NIVEL_COMISSAOX { get; set; } = new LT2000B_CAD_NIVEL_COMISSAOX();
                public class LT2000B_CAD_NIVEL_COMISSAOX : VarBasis
                {
                    /*"    20 CAD-NIVEL-COMISSAO           PIC 9(01).*/
                    public IntBasis CAD_NIVEL_COMISSAO { get; set; } = new IntBasis(new PIC("9", "1", "9(01)."));
                    /*"   15 CAD-PV-SUBX.*/
                }
                public LT2000B_CAD_PV_SUBX CAD_PV_SUBX { get; set; } = new LT2000B_CAD_PV_SUBX();
                public class LT2000B_CAD_PV_SUBX : VarBasis
                {
                    /*"    20 CAD-PV-SUB                   PIC 9(04).*/
                    public IntBasis CAD_PV_SUB { get; set; } = new IntBasis(new PIC("9", "4", "9(04)."));
                    /*"   15 CAD-EN-SUBX.*/
                }
                public LT2000B_CAD_EN_SUBX CAD_EN_SUBX { get; set; } = new LT2000B_CAD_EN_SUBX();
                public class LT2000B_CAD_EN_SUBX : VarBasis
                {
                    /*"    20 CAD-EN-SUB                   PIC 9(04).*/
                    public IntBasis CAD_EN_SUB { get; set; } = new IntBasis(new PIC("9", "4", "9(04)."));
                    /*"   15 CAD-UNIDADE-SUBX.*/
                }
                public LT2000B_CAD_UNIDADE_SUBX CAD_UNIDADE_SUBX { get; set; } = new LT2000B_CAD_UNIDADE_SUBX();
                public class LT2000B_CAD_UNIDADE_SUBX : VarBasis
                {
                    /*"    20 CAD-UNIDADE-SUB              PIC 9(04).*/
                    public IntBasis CAD_UNIDADE_SUB { get; set; } = new IntBasis(new PIC("9", "4", "9(04)."));
                    /*"   15 CAD-MATR-CONSULTORX.*/
                }
                public LT2000B_CAD_MATR_CONSULTORX CAD_MATR_CONSULTORX { get; set; } = new LT2000B_CAD_MATR_CONSULTORX();
                public class LT2000B_CAD_MATR_CONSULTORX : VarBasis
                {
                    /*"    20 CAD-MATR-CONSULTOR           PIC 9(07).*/
                    public IntBasis CAD_MATR_CONSULTOR { get; set; } = new IntBasis(new PIC("9", "7", "9(07)."));
                    /*"   15 CAD-NOME-CONSULTORX.*/
                }
                public LT2000B_CAD_NOME_CONSULTORX CAD_NOME_CONSULTORX { get; set; } = new LT2000B_CAD_NOME_CONSULTORX();
                public class LT2000B_CAD_NOME_CONSULTORX : VarBasis
                {
                    /*"    20 CAD-NOME-CONSULTOR           PIC X(40).*/
                    public StringBasis CAD_NOME_CONSULTOR { get; set; } = new StringBasis(new PIC("X", "40", "X(40)."), @"");
                    /*"   15 CAD-EMAILX.*/
                }
                public LT2000B_CAD_EMAILX CAD_EMAILX { get; set; } = new LT2000B_CAD_EMAILX();
                public class LT2000B_CAD_EMAILX : VarBasis
                {
                    /*"    20 CAD-EMAIL                    PIC X(50).*/
                    public StringBasis CAD_EMAIL { get; set; } = new StringBasis(new PIC("X", "50", "X(50)."), @"");
                    /*"   15 CAD-TIPO-GARANTIAX.*/
                }
                public LT2000B_CAD_TIPO_GARANTIAX CAD_TIPO_GARANTIAX { get; set; } = new LT2000B_CAD_TIPO_GARANTIAX();
                public class LT2000B_CAD_TIPO_GARANTIAX : VarBasis
                {
                    /*"    20 CAD-TIPO-GARANTIA            PIC X(01).*/
                    public StringBasis CAD_TIPO_GARANTIA { get; set; } = new StringBasis(new PIC("X", "1", "X(01)."), @"");
                    /*"   15 CAD-VALOR-GARANTIAX.*/
                }
                public LT2000B_CAD_VALOR_GARANTIAX CAD_VALOR_GARANTIAX { get; set; } = new LT2000B_CAD_VALOR_GARANTIAX();
                public class LT2000B_CAD_VALOR_GARANTIAX : VarBasis
                {
                    /*"    20 CAD-VALOR-GARANTIA           PIC 9(07)V99.*/
                    public DoubleBasis CAD_VALOR_GARANTIA { get; set; } = new DoubleBasis(new PIC("9", "7", "9(07)V99."), 2);
                    /*"   15 CAD-NUM-SEGURADORAX.*/
                }
                public LT2000B_CAD_NUM_SEGURADORAX CAD_NUM_SEGURADORAX { get; set; } = new LT2000B_CAD_NUM_SEGURADORAX();
                public class LT2000B_CAD_NUM_SEGURADORAX : VarBasis
                {
                    /*"    20 CAD-NUM-SEGURADORA           PIC 9(03).*/
                    public IntBasis CAD_NUM_SEGURADORA { get; set; } = new IntBasis(new PIC("9", "3", "9(03)."));
                    /*"   15 CAD-BONUS-CKTX.*/
                }
                public LT2000B_CAD_BONUS_CKTX CAD_BONUS_CKTX { get; set; } = new LT2000B_CAD_BONUS_CKTX();
                public class LT2000B_CAD_BONUS_CKTX : VarBasis
                {
                    /*"    20 CAD-BONUS-CKT                PIC 9(01).*/
                    public IntBasis CAD_BONUS_CKT { get; set; } = new IntBasis(new PIC("9", "1", "9(01)."));
                    /*"   15 CAD-BONUS-ALARMEX.*/
                }
                public LT2000B_CAD_BONUS_ALARMEX CAD_BONUS_ALARMEX { get; set; } = new LT2000B_CAD_BONUS_ALARMEX();
                public class LT2000B_CAD_BONUS_ALARMEX : VarBasis
                {
                    /*"    20 CAD-BONUS-ALARME             PIC 9(01).*/
                    public IntBasis CAD_BONUS_ALARME { get; set; } = new IntBasis(new PIC("9", "1", "9(01)."));
                    /*"   15 CAD-BONUS-COFREX.*/
                }
                public LT2000B_CAD_BONUS_COFREX CAD_BONUS_COFREX { get; set; } = new LT2000B_CAD_BONUS_COFREX();
                public class LT2000B_CAD_BONUS_COFREX : VarBasis
                {
                    /*"    20 CAD-BONUS-COFRE              PIC 9(01).*/
                    public IntBasis CAD_BONUS_COFRE { get; set; } = new IntBasis(new PIC("9", "1", "9(01)."));
                    /*"   15 CAD-NUMERO-FAX.*/
                }
                public LT2000B_CAD_NUMERO_FAX CAD_NUMERO_FAX { get; set; } = new LT2000B_CAD_NUMERO_FAX();
                public class LT2000B_CAD_NUMERO_FAX : VarBasis
                {
                    /*"    20 CAD-DDD-FAX                  PIC 9(04).*/
                    public IntBasis CAD_DDD_FAX { get; set; } = new IntBasis(new PIC("9", "4", "9(04)."));
                    /*"    20 CAD-FAX                      PIC 9(08).*/
                    public IntBasis CAD_FAX { get; set; } = new IntBasis(new PIC("9", "8", "9(08)."));
                    /*"   15 CAD-RENOVAR                     PIC 9(01).*/
                }
                public IntBasis CAD_RENOVAR { get; set; } = new IntBasis(new PIC("9", "1", "9(01)."));
                /*"   15 FILLER                          PIC X(117).*/
                public StringBasis FILLER_5 { get; set; } = new StringBasis(new PIC("X", "117", "X(117)."), @"");
                /*"   15 CAD-NSRX.*/
                public LT2000B_CAD_NSRX CAD_NSRX { get; set; } = new LT2000B_CAD_NSRX();
                public class LT2000B_CAD_NSRX : VarBasis
                {
                    /*"    20 CAD-NSR                      PIC 9(06).*/
                    public IntBasis CAD_NSR { get; set; } = new IntBasis(new PIC("9", "6", "9(06)."));
                    /*" 05 W-CGC.*/
                }
            }
            public LT2000B_W_CGC W_CGC { get; set; } = new LT2000B_W_CGC();
            public class LT2000B_W_CGC : VarBasis
            {
                /*"  10 W-CGC-8               PIC X(08) VALUE SPACES.*/
                public StringBasis W_CGC_8 { get; set; } = new StringBasis(new PIC("X", "8", "X(08)"), @"");
                /*"  10 W-CGC-4               PIC X(04) VALUE SPACES.*/
                public StringBasis W_CGC_4 { get; set; } = new StringBasis(new PIC("X", "4", "X(04)"), @"");
                /*"  10 W-CGC-2               PIC X(02) VALUE SPACES.*/
                public StringBasis W_CGC_2 { get; set; } = new StringBasis(new PIC("X", "2", "X(02)"), @"");
                /*" 05 W-CGC-ED.*/
            }
            public LT2000B_W_CGC_ED W_CGC_ED { get; set; } = new LT2000B_W_CGC_ED();
            public class LT2000B_W_CGC_ED : VarBasis
            {
                /*"  10 W-CGC-8-ED            PIC X(08) VALUE SPACES.*/
                public StringBasis W_CGC_8_ED { get; set; } = new StringBasis(new PIC("X", "8", "X(08)"), @"");
                /*"  10 FILLER                PIC X(01) VALUE '/'.*/
                public StringBasis FILLER_6 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"/");
                /*"  10 W-CGC-4-ED            PIC X(04) VALUE SPACES.*/
                public StringBasis W_CGC_4_ED { get; set; } = new StringBasis(new PIC("X", "4", "X(04)"), @"");
                /*"  10 FILLER                PIC X(01) VALUE '-'.*/
                public StringBasis FILLER_7 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"-");
                /*"  10 W-CGC-2-ED            PIC X(02) VALUE SPACES.*/
                public StringBasis W_CGC_2_ED { get; set; } = new StringBasis(new PIC("X", "2", "X(02)"), @"");
                /*" 05 W-AC-CAD-LIDOS                PIC 9(07) VALUE ZEROS.*/
            }
            public IntBasis W_AC_CAD_LIDOS { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 W-AC-MOV-LIDOS                PIC 9(07) VALUE ZEROS.*/
            public IntBasis W_AC_MOV_LIDOS { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 W-AC-LOTERICOS-GRAVADOS       PIC 9(07) VALUE ZEROS.*/
            public IntBasis W_AC_LOTERICOS_GRAVADOS { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 W-AC-LOTERICOS-REJEITADOS     PIC 9(07) VALUE ZEROS.*/
            public IntBasis W_AC_LOTERICOS_REJEITADOS { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 W-AC-LOTERICOS-BLACKLIST      PIC 9(07) VALUE ZEROS.*/
            public IntBasis W_AC_LOTERICOS_BLACKLIST { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 W-AC-MOV-LOTERICOS-BAIXADOS   PIC 9(07) VALUE ZEROS.*/
            public IntBasis W_AC_MOV_LOTERICOS_BAIXADOS { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 W-AC-MOV-LOTERICOS-REJEITADOS PIC 9(07) VALUE ZEROS.*/
            public IntBasis W_AC_MOV_LOTERICOS_REJEITADOS { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 W-AC-ATIVOS                   PIC 9(07) VALUE ZEROS.*/
            public IntBasis W_AC_ATIVOS { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 W-AC-INATIVOS                 PIC 9(07) VALUE ZEROS.*/
            public IntBasis W_AC_INATIVOS { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 W-AC-PRECAD                   PIC 9(07) VALUE ZEROS.*/
            public IntBasis W_AC_PRECAD { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 W-AC-SEGURADORA-CS            PIC 9(07) VALUE ZEROS.*/
            public IntBasis W_AC_SEGURADORA_CS { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 W-AC-SEGURADORA-OUTRAS        PIC 9(07) VALUE ZEROS.*/
            public IntBasis W_AC_SEGURADORA_OUTRAS { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 W-AC-GARANTIA-S               PIC 9(07) VALUE ZEROS.*/
            public IntBasis W_AC_GARANTIA_S { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 W-AC-GARANTIA-C               PIC 9(07) VALUE ZEROS.*/
            public IntBasis W_AC_GARANTIA_C { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 WS-FCPEND-DELETADOS           PIC 9(07) VALUE ZEROS.*/
            public IntBasis WS_FCPEND_DELETADOS { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 WS-FCLOT-TOTAL-INCLUIDOS      PIC 9(07) VALUE ZEROS.*/
            public IntBasis WS_FCLOT_TOTAL_INCLUIDOS { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 WS-FCLOT-TOTAL-ALTERADOS      PIC 9(07) VALUE ZEROS.*/
            public IntBasis WS_FCLOT_TOTAL_ALTERADOS { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 WS-MVPROP-TOTAL-SEGURADOS     PIC 9(07) VALUE ZEROS.*/
            public IntBasis WS_MVPROP_TOTAL_SEGURADOS { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 WS-MVPROP-TOTAL-NSEGURADOS    PIC 9(07) VALUE ZEROS.*/
            public IntBasis WS_MVPROP_TOTAL_NSEGURADOS { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 WS-MVPROP-TOTAL-INCLUIDOS     PIC 9(07) VALUE ZEROS.*/
            public IntBasis WS_MVPROP_TOTAL_INCLUIDOS { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 WS-MVPROP-TOTAL-ALTERADOS     PIC 9(07) VALUE ZEROS.*/
            public IntBasis WS_MVPROP_TOTAL_ALTERADOS { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 WS-MVPROP-TOTAL-CANCELADOS    PIC 9(07) VALUE ZEROS.*/
            public IntBasis WS_MVPROP_TOTAL_CANCELADOS { get; set; } = new IntBasis(new PIC("9", "7", "9(07)"));
            /*" 05 WS-ERRO-DATA                  PIC S9(07) VALUE  ZEROS.*/
            public IntBasis WS_ERRO_DATA { get; set; } = new IntBasis(new PIC("S9", "7", "S9(07)"));
            /*" 05 WS-CONTA-BANCARIA PIC 9(17).*/
            public IntBasis WS_CONTA_BANCARIA { get; set; } = new IntBasis(new PIC("9", "17", "9(17)."));
            /*" 05 WS-CONTA-BANCARIA-R REDEFINES WS-CONTA-BANCARIA.*/
            private _REDEF_LT2000B_WS_CONTA_BANCARIA_R _ws_conta_bancaria_r { get; set; }
            public _REDEF_LT2000B_WS_CONTA_BANCARIA_R WS_CONTA_BANCARIA_R
            {
                get { _ws_conta_bancaria_r = new _REDEF_LT2000B_WS_CONTA_BANCARIA_R(); _.Move(WS_CONTA_BANCARIA, _ws_conta_bancaria_r); VarBasis.RedefinePassValue(WS_CONTA_BANCARIA, _ws_conta_bancaria_r, WS_CONTA_BANCARIA); _ws_conta_bancaria_r.ValueChanged += () => { _.Move(_ws_conta_bancaria_r, WS_CONTA_BANCARIA); }; return _ws_conta_bancaria_r; }
                set { VarBasis.RedefinePassValue(value, _ws_conta_bancaria_r, WS_CONTA_BANCARIA); }
            }  //Redefines
            public class _REDEF_LT2000B_WS_CONTA_BANCARIA_R : VarBasis
            {
                /*"  10 WS-OPERACAO-CONTA PIC  X(004).*/
                public StringBasis WS_OPERACAO_CONTA { get; set; } = new StringBasis(new PIC("X", "4", "X(004)."), @"");
                /*"  10 WS-NUMERO-CONTA   PIC  9(012).*/
                public IntBasis WS_NUMERO_CONTA { get; set; } = new IntBasis(new PIC("9", "12", "9(012)."));
                /*"  10 WS-DV-CONTA       PIC  X(001).*/
                public StringBasis WS_DV_CONTA { get; set; } = new StringBasis(new PIC("X", "1", "X(001)."), @"");
                /*" 05 WS-CONTA-BANCO    PIC 9(017).*/

                public _REDEF_LT2000B_WS_CONTA_BANCARIA_R()
                {
                    WS_OPERACAO_CONTA.ValueChanged += OnValueChanged;
                    WS_NUMERO_CONTA.ValueChanged += OnValueChanged;
                    WS_DV_CONTA.ValueChanged += OnValueChanged;
                }

            }
            public IntBasis WS_CONTA_BANCO { get; set; } = new IntBasis(new PIC("9", "17", "9(017)."));
            /*" 05 WS-CONTA-BANCO-R  REDEFINES   WS-CONTA-BANCO.*/
            private _REDEF_LT2000B_WS_CONTA_BANCO_R _ws_conta_banco_r { get; set; }
            public _REDEF_LT2000B_WS_CONTA_BANCO_R WS_CONTA_BANCO_R
            {
                get { _ws_conta_banco_r = new _REDEF_LT2000B_WS_CONTA_BANCO_R(); _.Move(WS_CONTA_BANCO, _ws_conta_banco_r); VarBasis.RedefinePassValue(WS_CONTA_BANCO, _ws_conta_banco_r, WS_CONTA_BANCO); _ws_conta_banco_r.ValueChanged += () => { _.Move(_ws_conta_banco_r, WS_CONTA_BANCO); }; return _ws_conta_banco_r; }
                set { VarBasis.RedefinePassValue(value, _ws_conta_banco_r, WS_CONTA_BANCO); }
            }  //Redefines
            public class _REDEF_LT2000B_WS_CONTA_BANCO_R : VarBasis
            {
                /*"  10 WS-OPERA-CONTA    PIC 9(004).*/
                public IntBasis WS_OPERA_CONTA { get; set; } = new IntBasis(new PIC("9", "4", "9(004)."));
                /*"  10 WS-FILLER         PIC 9(013).*/
                public IntBasis WS_FILLER { get; set; } = new IntBasis(new PIC("9", "13", "9(013)."));
                /*" 05 WS-CONTA-12POS    PIC 9(12).*/

                public _REDEF_LT2000B_WS_CONTA_BANCO_R()
                {
                    WS_OPERA_CONTA.ValueChanged += OnValueChanged;
                    WS_FILLER.ValueChanged += OnValueChanged;
                }

            }
            public IntBasis WS_CONTA_12POS { get; set; } = new IntBasis(new PIC("9", "12", "9(12)."));
            /*" 05 WIND1             PIC 9(02).*/
            public IntBasis WIND1 { get; set; } = new IntBasis(new PIC("9", "2", "9(02)."));
            /*" 05 WIND2             PIC 9(02).*/
            public IntBasis WIND2 { get; set; } = new IntBasis(new PIC("9", "2", "9(02)."));
            /*" 05 WTAM-TEL          PIC 9(02).*/
            public IntBasis WTAM_TEL { get; set; } = new IntBasis(new PIC("9", "2", "9(02)."));
            /*" 05 WS-NUM-TELEF-ENT  PIC X(12).*/
            public StringBasis WS_NUM_TELEF_ENT { get; set; } = new StringBasis(new PIC("X", "12", "X(12)."), @"");
            /*" 05 FILLER            REDEFINES WS-NUM-TELEF-ENT.*/
            private _REDEF_LT2000B_FILLER_8 _filler_8 { get; set; }
            public _REDEF_LT2000B_FILLER_8 FILLER_8
            {
                get { _filler_8 = new _REDEF_LT2000B_FILLER_8(); _.Move(WS_NUM_TELEF_ENT, _filler_8); VarBasis.RedefinePassValue(WS_NUM_TELEF_ENT, _filler_8, WS_NUM_TELEF_ENT); _filler_8.ValueChanged += () => { _.Move(_filler_8, WS_NUM_TELEF_ENT); }; return _filler_8; }
                set { VarBasis.RedefinePassValue(value, _filler_8, WS_NUM_TELEF_ENT); }
            }  //Redefines
            public class _REDEF_LT2000B_FILLER_8 : VarBasis
            {
                /*"  10 WS-DDD-ENT        PIC  9(005).*/
                public IntBasis WS_DDD_ENT { get; set; } = new IntBasis(new PIC("9", "5", "9(005)."));
                /*"  10 WS-TELEFONE-ENT   PIC  9(007).*/
                public IntBasis WS_TELEFONE_ENT { get; set; } = new IntBasis(new PIC("9", "7", "9(007)."));
                /*" 05 WS-NUM-TELEF-SAI  PIC X(12).*/

                public _REDEF_LT2000B_FILLER_8()
                {
                    WS_DDD_ENT.ValueChanged += OnValueChanged;
                    WS_TELEFONE_ENT.ValueChanged += OnValueChanged;
                }

            }
            public StringBasis WS_NUM_TELEF_SAI { get; set; } = new StringBasis(new PIC("X", "12", "X(12)."), @"");
            /*" 05 FILLER            REDEFINES WS-NUM-TELEF-SAI.*/
            private _REDEF_LT2000B_FILLER_9 _filler_9 { get; set; }
            public _REDEF_LT2000B_FILLER_9 FILLER_9
            {
                get { _filler_9 = new _REDEF_LT2000B_FILLER_9(); _.Move(WS_NUM_TELEF_SAI, _filler_9); VarBasis.RedefinePassValue(WS_NUM_TELEF_SAI, _filler_9, WS_NUM_TELEF_SAI); _filler_9.ValueChanged += () => { _.Move(_filler_9, WS_NUM_TELEF_SAI); }; return _filler_9; }
                set { VarBasis.RedefinePassValue(value, _filler_9, WS_NUM_TELEF_SAI); }
            }  //Redefines
            public class _REDEF_LT2000B_FILLER_9 : VarBasis
            {
                /*"  10 WS-DDD-SAI        PIC  9(004).*/
                public IntBasis WS_DDD_SAI { get; set; } = new IntBasis(new PIC("9", "4", "9(004)."));
                /*"  10 WS-TELEFONE-SAI   PIC  9(008).*/
                public IntBasis WS_TELEFONE_SAI { get; set; } = new IntBasis(new PIC("9", "8", "9(008)."));
                /*" 05 WS-NUMERO-FAX     PIC X(16).*/

                public _REDEF_LT2000B_FILLER_9()
                {
                    WS_DDD_SAI.ValueChanged += OnValueChanged;
                    WS_TELEFONE_SAI.ValueChanged += OnValueChanged;
                }

            }
            public StringBasis WS_NUMERO_FAX { get; set; } = new StringBasis(new PIC("X", "16", "X(16)."), @"");
            /*" 05 WS-NUMERO-FAX-R   REDEFINES WS-NUMERO-FAX.*/
            private _REDEF_LT2000B_WS_NUMERO_FAX_R _ws_numero_fax_r { get; set; }
            public _REDEF_LT2000B_WS_NUMERO_FAX_R WS_NUMERO_FAX_R
            {
                get { _ws_numero_fax_r = new _REDEF_LT2000B_WS_NUMERO_FAX_R(); _.Move(WS_NUMERO_FAX, _ws_numero_fax_r); VarBasis.RedefinePassValue(WS_NUMERO_FAX, _ws_numero_fax_r, WS_NUMERO_FAX); _ws_numero_fax_r.ValueChanged += () => { _.Move(_ws_numero_fax_r, WS_NUMERO_FAX); }; return _ws_numero_fax_r; }
                set { VarBasis.RedefinePassValue(value, _ws_numero_fax_r, WS_NUMERO_FAX); }
            }  //Redefines
            public class _REDEF_LT2000B_WS_NUMERO_FAX_R : VarBasis
            {
                /*"  10 WS-NUMER-FAX.*/
                public LT2000B_WS_NUMER_FAX WS_NUMER_FAX { get; set; } = new LT2000B_WS_NUMER_FAX();
                public class LT2000B_WS_NUMER_FAX : VarBasis
                {
                    /*"    20 WS-DDD-FAX        PIC  9(004).*/
                    public IntBasis WS_DDD_FAX { get; set; } = new IntBasis(new PIC("9", "4", "9(004)."));
                    /*"    20 WS-NUM-FAX        PIC  9(008).*/
                    public IntBasis WS_NUM_FAX { get; set; } = new IntBasis(new PIC("9", "8", "9(008)."));
                    /*"  10 FILLER            PIC  X(004).*/

                    public LT2000B_WS_NUMER_FAX()
                    {
                        WS_DDD_FAX.ValueChanged += OnValueChanged;
                        WS_NUM_FAX.ValueChanged += OnValueChanged;
                    }

                }
                public StringBasis FILLER_10 { get; set; } = new StringBasis(new PIC("X", "4", "X(004)."), @"");
                /*" 05 WS-CODIGO-BANCO   PIC 9(09).*/

                public _REDEF_LT2000B_WS_NUMERO_FAX_R()
                {
                    WS_NUMER_FAX.ValueChanged += OnValueChanged;
                    FILLER_10.ValueChanged += OnValueChanged;
                }

            }
            public IntBasis WS_CODIGO_BANCO { get; set; } = new IntBasis(new PIC("9", "9", "9(09)."));
            /*" 05 WS-CODIGO-BANCO-R REDEFINES WS-CODIGO-BANCO.*/
            private _REDEF_LT2000B_WS_CODIGO_BANCO_R _ws_codigo_banco_r { get; set; }
            public _REDEF_LT2000B_WS_CODIGO_BANCO_R WS_CODIGO_BANCO_R
            {
                get { _ws_codigo_banco_r = new _REDEF_LT2000B_WS_CODIGO_BANCO_R(); _.Move(WS_CODIGO_BANCO, _ws_codigo_banco_r); VarBasis.RedefinePassValue(WS_CODIGO_BANCO, _ws_codigo_banco_r, WS_CODIGO_BANCO); _ws_codigo_banco_r.ValueChanged += () => { _.Move(_ws_codigo_banco_r, WS_CODIGO_BANCO); }; return _ws_codigo_banco_r; }
                set { VarBasis.RedefinePassValue(value, _ws_codigo_banco_r, WS_CODIGO_BANCO); }
            }  //Redefines
            public class _REDEF_LT2000B_WS_CODIGO_BANCO_R : VarBasis
            {
                /*"  10 FILLER            PIC  X(006).*/
                public StringBasis FILLER_11 { get; set; } = new StringBasis(new PIC("X", "6", "X(006)."), @"");
                /*"  10 WS-COD-BANCO      PIC  X(003).*/
                public StringBasis WS_COD_BANCO { get; set; } = new StringBasis(new PIC("X", "3", "X(003)."), @"");
                /*" 05 WS1-CONTA-BANCARIA PIC 9(17).*/

                public _REDEF_LT2000B_WS_CODIGO_BANCO_R()
                {
                    FILLER_11.ValueChanged += OnValueChanged;
                    WS_COD_BANCO.ValueChanged += OnValueChanged;
                }

            }
            public IntBasis WS1_CONTA_BANCARIA { get; set; } = new IntBasis(new PIC("9", "17", "9(17)."));
            /*" 05 WS1-CONTA-BANCARIA-R REDEFINES WS1-CONTA-BANCARIA.*/
            private _REDEF_LT2000B_WS1_CONTA_BANCARIA_R _ws1_conta_bancaria_r { get; set; }
            public _REDEF_LT2000B_WS1_CONTA_BANCARIA_R WS1_CONTA_BANCARIA_R
            {
                get { _ws1_conta_bancaria_r = new _REDEF_LT2000B_WS1_CONTA_BANCARIA_R(); _.Move(WS1_CONTA_BANCARIA, _ws1_conta_bancaria_r); VarBasis.RedefinePassValue(WS1_CONTA_BANCARIA, _ws1_conta_bancaria_r, WS1_CONTA_BANCARIA); _ws1_conta_bancaria_r.ValueChanged += () => { _.Move(_ws1_conta_bancaria_r, WS1_CONTA_BANCARIA); }; return _ws1_conta_bancaria_r; }
                set { VarBasis.RedefinePassValue(value, _ws1_conta_bancaria_r, WS1_CONTA_BANCARIA); }
            }  //Redefines
            public class _REDEF_LT2000B_WS1_CONTA_BANCARIA_R : VarBasis
            {
                /*"  10 WS1-OPERACAO-CONTA PIC  9(004).*/
                public IntBasis WS1_OPERACAO_CONTA { get; set; } = new IntBasis(new PIC("9", "4", "9(004)."));
                /*"  10 WS1-NUMERO-CONTA   PIC  9(012).*/
                public IntBasis WS1_NUMERO_CONTA { get; set; } = new IntBasis(new PIC("9", "12", "9(012)."));
                /*"  10 WS1-DV-CONTA       PIC  9(001).*/
                public IntBasis WS1_DV_CONTA { get; set; } = new IntBasis(new PIC("9", "1", "9(001)."));
                /*" 05 WS-CONBAN-BANCO   PIC 9(04).*/

                public _REDEF_LT2000B_WS1_CONTA_BANCARIA_R()
                {
                    WS1_OPERACAO_CONTA.ValueChanged += OnValueChanged;
                    WS1_NUMERO_CONTA.ValueChanged += OnValueChanged;
                    WS1_DV_CONTA.ValueChanged += OnValueChanged;
                }

            }
            public IntBasis WS_CONBAN_BANCO { get; set; } = new IntBasis(new PIC("9", "4", "9(04)."));
            /*" 05 WS-CONBAN-AGENCIA PIC 9(04).*/
            public IntBasis WS_CONBAN_AGENCIA { get; set; } = new IntBasis(new PIC("9", "4", "9(04)."));
            /*" 05 WS-CONBAN-OP      PIC 9(04).*/
            public IntBasis WS_CONBAN_OP { get; set; } = new IntBasis(new PIC("9", "4", "9(04)."));
            /*" 05 WS-CONBAN-CONTA   PIC 9(12).*/
            public IntBasis WS_CONBAN_CONTA { get; set; } = new IntBasis(new PIC("9", "12", "9(12)."));
            /*" 05 WS-CONBAN-DV      PIC X(01).*/
            public StringBasis WS_CONBAN_DV { get; set; } = new StringBasis(new PIC("X", "1", "X(01)."), @"");
            /*" 05 WS-CODIGO-AGENCIA PIC 9(05).*/
            public IntBasis WS_CODIGO_AGENCIA { get; set; } = new IntBasis(new PIC("9", "5", "9(05)."));
            /*" 05 WS-CODIGO-AGENCIA-R REDEFINES WS-CODIGO-AGENCIA.*/
            private _REDEF_LT2000B_WS_CODIGO_AGENCIA_R _ws_codigo_agencia_r { get; set; }
            public _REDEF_LT2000B_WS_CODIGO_AGENCIA_R WS_CODIGO_AGENCIA_R
            {
                get { _ws_codigo_agencia_r = new _REDEF_LT2000B_WS_CODIGO_AGENCIA_R(); _.Move(WS_CODIGO_AGENCIA, _ws_codigo_agencia_r); VarBasis.RedefinePassValue(WS_CODIGO_AGENCIA, _ws_codigo_agencia_r, WS_CODIGO_AGENCIA); _ws_codigo_agencia_r.ValueChanged += () => { _.Move(_ws_codigo_agencia_r, WS_CODIGO_AGENCIA); }; return _ws_codigo_agencia_r; }
                set { VarBasis.RedefinePassValue(value, _ws_codigo_agencia_r, WS_CODIGO_AGENCIA); }
            }  //Redefines
            public class _REDEF_LT2000B_WS_CODIGO_AGENCIA_R : VarBasis
            {
                /*"  10 FILLER            PIC  X(001).*/
                public StringBasis FILLER_12 { get; set; } = new StringBasis(new PIC("X", "1", "X(001)."), @"");
                /*"  10 WS-COD-AGENCIA    PIC  X(004).*/
                public StringBasis WS_COD_AGENCIA { get; set; } = new StringBasis(new PIC("X", "4", "X(004)."), @"");
                /*" 05 WS-DATA-INIVIG    PIC  X(10).*/

                public _REDEF_LT2000B_WS_CODIGO_AGENCIA_R()
                {
                    FILLER_12.ValueChanged += OnValueChanged;
                    WS_COD_AGENCIA.ValueChanged += OnValueChanged;
                }

            }
            public StringBasis WS_DATA_INIVIG { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
            /*" 05 WS-DATA-TERVIG    PIC  X(10).*/
            public StringBasis WS_DATA_TERVIG { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
            /*" 05 WS-DATA-SEC.*/
            public LT2000B_WS_DATA_SEC WS_DATA_SEC { get; set; } = new LT2000B_WS_DATA_SEC();
            public class LT2000B_WS_DATA_SEC : VarBasis
            {
                /*"  10 WS-SS-DATA-SEC    PIC  9(002)    VALUE  ZEROS.*/
                public IntBasis WS_SS_DATA_SEC { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*"  10 WS-AA-DATA-SEC    PIC  9(002)    VALUE  ZEROS.*/
                public IntBasis WS_AA_DATA_SEC { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*"  10 WS-MM-DATA-SEC    PIC  9(002)    VALUE  ZEROS.*/
                public IntBasis WS_MM_DATA_SEC { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*"  10 WS-DD-DATA-SEC    PIC  9(002)    VALUE  ZEROS.*/
                public IntBasis WS_DD_DATA_SEC { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*" 05 WS-DATA.*/
            }
            public LT2000B_WS_DATA WS_DATA { get; set; } = new LT2000B_WS_DATA();
            public class LT2000B_WS_DATA : VarBasis
            {
                /*"  10 WS-DD-DATA        PIC  9(002)    VALUE  ZEROS.*/
                public IntBasis WS_DD_DATA { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*"  10 WS-MM-DATA        PIC  9(002)    VALUE  ZEROS.*/
                public IntBasis WS_MM_DATA { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*"  10 WS-AA-DATA        PIC  9(002)    VALUE  ZEROS.*/
                public IntBasis WS_AA_DATA { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*" 05 WS-DATE.*/
            }
            public LT2000B_WS_DATE WS_DATE { get; set; } = new LT2000B_WS_DATE();
            public class LT2000B_WS_DATE : VarBasis
            {
                /*"  10 WS-AA-DATE        PIC  9(002)    VALUE  ZEROS.*/
                public IntBasis WS_AA_DATE { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*"  10 WS-MM-DATE        PIC  9(002)    VALUE  ZEROS.*/
                public IntBasis WS_MM_DATE { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*"  10 WS-DD-DATE        PIC  9(002)    VALUE  ZEROS.*/
                public IntBasis WS_DD_DATE { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*" 05 WS-DT-ANT.*/
            }
            public LT2000B_WS_DT_ANT WS_DT_ANT { get; set; } = new LT2000B_WS_DT_ANT();
            public class LT2000B_WS_DT_ANT : VarBasis
            {
                /*"  10 WS-AA-ANT         PIC  9(002)    VALUE  ZEROS.*/
                public IntBasis WS_AA_ANT { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*"  10 WS-MM-ANT         PIC  9(002)    VALUE  ZEROS.*/
                public IntBasis WS_MM_ANT { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*"  10 WS-DD-ANT         PIC  9(002)    VALUE  ZEROS.*/
                public IntBasis WS_DD_ANT { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*" 05 WS-DT-POS.*/
            }
            public LT2000B_WS_DT_POS WS_DT_POS { get; set; } = new LT2000B_WS_DT_POS();
            public class LT2000B_WS_DT_POS : VarBasis
            {
                /*"  10 WS-AA-POS         PIC  9(002)    VALUE  ZEROS.*/
                public IntBasis WS_AA_POS { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*"  10 WS-MM-POS         PIC  9(002)    VALUE  ZEROS.*/
                public IntBasis WS_MM_POS { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*"  10 WS-DD-POS         PIC  9(002)    VALUE  ZEROS.*/
                public IntBasis WS_DD_POS { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*" 05 WS-TIME.*/
            }
            public LT2000B_WS_TIME WS_TIME { get; set; } = new LT2000B_WS_TIME();
            public class LT2000B_WS_TIME : VarBasis
            {
                /*"  10 WS-HH-TIME        PIC  9(002)    VALUE ZEROS.*/
                public IntBasis WS_HH_TIME { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*"  10 WS-MM-TIME        PIC  9(002)    VALUE ZEROS.*/
                public IntBasis WS_MM_TIME { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*"  10 WS-SS-TIME        PIC  9(002)    VALUE ZEROS.*/
                public IntBasis WS_SS_TIME { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*"  10 WS-CC-TIME        PIC  9(002)    VALUE ZEROS.*/
                public IntBasis WS_CC_TIME { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*" 05 WTIME-DAY          PIC  99.99.99.99.*/
            }
            public IntBasis WTIME_DAY { get; set; } = new IntBasis(new PIC("9", "8", "99.99.99.99."));
            /*" 05 FILLER             REDEFINES      WTIME-DAY.*/
            private _REDEF_LT2000B_FILLER_13 _filler_13 { get; set; }
            public _REDEF_LT2000B_FILLER_13 FILLER_13
            {
                get { _filler_13 = new _REDEF_LT2000B_FILLER_13(); _.Move(WTIME_DAY, _filler_13); VarBasis.RedefinePassValue(WTIME_DAY, _filler_13, WTIME_DAY); _filler_13.ValueChanged += () => { _.Move(_filler_13, WTIME_DAY); }; return _filler_13; }
                set { VarBasis.RedefinePassValue(value, _filler_13, WTIME_DAY); }
            }  //Redefines
            public class _REDEF_LT2000B_FILLER_13 : VarBasis
            {
                /*"  10 WTIME-DAYR.*/
                public LT2000B_WTIME_DAYR WTIME_DAYR { get; set; } = new LT2000B_WTIME_DAYR();
                public class LT2000B_WTIME_DAYR : VarBasis
                {
                    /*"    20 WTIME-HORA         PIC  X(002).*/
                    public StringBasis WTIME_HORA { get; set; } = new StringBasis(new PIC("X", "2", "X(002)."), @"");
                    /*"    20 WTIME-2PT1         PIC  X(001).*/
                    public StringBasis WTIME_2PT1 { get; set; } = new StringBasis(new PIC("X", "1", "X(001)."), @"");
                    /*"    20 WTIME-MINU         PIC  X(002).*/
                    public StringBasis WTIME_MINU { get; set; } = new StringBasis(new PIC("X", "2", "X(002)."), @"");
                    /*"    20 WTIME-2PT2         PIC  X(001).*/
                    public StringBasis WTIME_2PT2 { get; set; } = new StringBasis(new PIC("X", "1", "X(001)."), @"");
                    /*"    20 WTIME-SEGU         PIC  X(002).*/
                    public StringBasis WTIME_SEGU { get; set; } = new StringBasis(new PIC("X", "2", "X(002)."), @"");
                    /*"  10 WTIME-2PT3         PIC  X(001).*/

                    public LT2000B_WTIME_DAYR()
                    {
                        WTIME_HORA.ValueChanged += OnValueChanged;
                        WTIME_2PT1.ValueChanged += OnValueChanged;
                        WTIME_MINU.ValueChanged += OnValueChanged;
                        WTIME_2PT2.ValueChanged += OnValueChanged;
                        WTIME_SEGU.ValueChanged += OnValueChanged;
                    }

                }
                public StringBasis WTIME_2PT3 { get; set; } = new StringBasis(new PIC("X", "1", "X(001)."), @"");
                /*"  10 WTIME-CCSE         PIC  X(002).*/
                public StringBasis WTIME_CCSE { get; set; } = new StringBasis(new PIC("X", "2", "X(002)."), @"");
                /*" 05 WDATA-CABEC.*/

                public _REDEF_LT2000B_FILLER_13()
                {
                    WTIME_DAYR.ValueChanged += OnValueChanged;
                    WTIME_2PT3.ValueChanged += OnValueChanged;
                    WTIME_CCSE.ValueChanged += OnValueChanged;
                }

            }
            public LT2000B_WDATA_CABEC WDATA_CABEC { get; set; } = new LT2000B_WDATA_CABEC();
            public class LT2000B_WDATA_CABEC : VarBasis
            {
                /*"  10 WDATA-DD-CABEC    PIC  9(002)    VALUE ZEROS.*/
                public IntBasis WDATA_DD_CABEC { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*"  10 FILLER            PIC  X(001)    VALUE '/'.*/
                public StringBasis FILLER_14 { get; set; } = new StringBasis(new PIC("X", "1", "X(001)"), @"/");
                /*"  10 WDATA-MM-CABEC    PIC  9(002)    VALUE ZEROS.*/
                public IntBasis WDATA_MM_CABEC { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*"  10 FILLER            PIC  X(001)    VALUE '/'.*/
                public StringBasis FILLER_15 { get; set; } = new StringBasis(new PIC("X", "1", "X(001)"), @"/");
                /*"  10 WDATA-AA-CABEC    PIC  9(002)    VALUE ZEROS.*/
                public IntBasis WDATA_AA_CABEC { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*" 05 WHORA-CABEC.*/
            }
            public LT2000B_WHORA_CABEC WHORA_CABEC { get; set; } = new LT2000B_WHORA_CABEC();
            public class LT2000B_WHORA_CABEC : VarBasis
            {
                /*"  10 WHORA-HH-CABEC    PIC  9(002)    VALUE ZEROS.*/
                public IntBasis WHORA_HH_CABEC { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*"  10 FILLER            PIC  X(001)    VALUE ':'.*/
                public StringBasis FILLER_16 { get; set; } = new StringBasis(new PIC("X", "1", "X(001)"), @":");
                /*"  10 WHORA-MM-CABEC    PIC  9(002)    VALUE ZEROS.*/
                public IntBasis WHORA_MM_CABEC { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*"  10 FILLER            PIC  X(001)    VALUE ':'.*/
                public StringBasis FILLER_17 { get; set; } = new StringBasis(new PIC("X", "1", "X(001)"), @":");
                /*"  10 WHORA-SS-CABEC    PIC  9(002)    VALUE ZEROS.*/
                public IntBasis WHORA_SS_CABEC { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*" 05 WDATA-MOVABE.*/
            }
            public LT2000B_WDATA_MOVABE WDATA_MOVABE { get; set; } = new LT2000B_WDATA_MOVABE();
            public class LT2000B_WDATA_MOVABE : VarBasis
            {
                /*"  10 WDATA-SC-MOVABE   PIC  9(002)    VALUE ZEROS.*/
                public IntBasis WDATA_SC_MOVABE { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*"  10 WDATA-AA-MOVABE   PIC  9(002)    VALUE ZEROS.*/
                public IntBasis WDATA_AA_MOVABE { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*"  10 FILLER            PIC  X(001)    VALUE '-'.*/
                public StringBasis FILLER_18 { get; set; } = new StringBasis(new PIC("X", "1", "X(001)"), @"-");
                /*"  10 WDATA-MM-MOVABE   PIC  9(002)    VALUE ZEROS.*/
                public IntBasis WDATA_MM_MOVABE { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*"  10 FILLER            PIC  X(001)    VALUE '-'.*/
                public StringBasis FILLER_19 { get; set; } = new StringBasis(new PIC("X", "1", "X(001)"), @"-");
                /*"  10 WDATA-DD-MOVABE   PIC  9(002)    VALUE ZEROS.*/
                public IntBasis WDATA_DD_MOVABE { get; set; } = new IntBasis(new PIC("9", "2", "9(002)"));
                /*" 05 TAB-UF.*/
            }
            public LT2000B_TAB_UF TAB_UF { get; set; } = new LT2000B_TAB_UF();
            public class LT2000B_TAB_UF : VarBasis
            {
                /*"  10 FILLER PIC X(30) VALUE 'AC ACRE'.*/
                public StringBasis FILLER_20 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"AC ACRE");
                /*"  10 FILLER PIC X(30) VALUE 'AL ALAGOAS'.*/
                public StringBasis FILLER_21 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"AL ALAGOAS");
                /*"  10 FILLER PIC X(30) VALUE 'AM AMAZONAS'.*/
                public StringBasis FILLER_22 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"AM AMAZONAS");
                /*"  10 FILLER PIC X(30) VALUE 'AP AMAPA   '.*/
                public StringBasis FILLER_23 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"AP AMAPA   ");
                /*"  10 FILLER PIC X(30) VALUE 'BA BAHIA'.*/
                public StringBasis FILLER_24 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"BA BAHIA");
                /*"  10 FILLER PIC X(30) VALUE 'CE CEARA'.*/
                public StringBasis FILLER_25 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"CE CEARA");
                /*"  10 FILLER PIC X(30) VALUE 'DF DISTRITO FEDERAL'.*/
                public StringBasis FILLER_26 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"DF DISTRITO FEDERAL");
                /*"  10 FILLER PIC X(30) VALUE 'ES ESPIRITO SANTO'.*/
                public StringBasis FILLER_27 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"ES ESPIRITO SANTO");
                /*"  10 FILLER PIC X(30) VALUE 'GO GOIANIA'.*/
                public StringBasis FILLER_28 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"GO GOIANIA");
                /*"  10 FILLER PIC X(30) VALUE 'MA MARANHAO'.*/
                public StringBasis FILLER_29 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"MA MARANHAO");
                /*"  10 FILLER PIC X(30) VALUE 'MG BELO HORIZONTE'.*/
                public StringBasis FILLER_30 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"MG BELO HORIZONTE");
                /*"  10 FILLER PIC X(30) VALUE 'MS MATO GROSSO DO SUL'.*/
                public StringBasis FILLER_31 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"MS MATO GROSSO DO SUL");
                /*"  10 FILLER PIC X(30) VALUE 'MT MATO GROSSO'.*/
                public StringBasis FILLER_32 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"MT MATO GROSSO");
                /*"  10 FILLER PIC X(30) VALUE 'PA PARA'.*/
                public StringBasis FILLER_33 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"PA PARA");
                /*"  10 FILLER PIC X(30) VALUE 'PB PARAIBA'.*/
                public StringBasis FILLER_34 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"PB PARAIBA");
                /*"  10 FILLER PIC X(30) VALUE 'PE PERNAMBUCO'.*/
                public StringBasis FILLER_35 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"PE PERNAMBUCO");
                /*"  10 FILLER PIC X(30) VALUE 'PI PIAUI'.*/
                public StringBasis FILLER_36 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"PI PIAUI");
                /*"  10 FILLER PIC X(30) VALUE 'PR PARANA'.*/
                public StringBasis FILLER_37 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"PR PARANA");
                /*"  10 FILLER PIC X(30) VALUE 'RJ RIO DE JANEIRO'.*/
                public StringBasis FILLER_38 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"RJ RIO DE JANEIRO");
                /*"  10 FILLER PIC X(30) VALUE 'RN RIO G DO NORTE'.*/
                public StringBasis FILLER_39 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"RN RIO G DO NORTE");
                /*"  10 FILLER PIC X(30) VALUE 'RO RONDONIA'.*/
                public StringBasis FILLER_40 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"RO RONDONIA");
                /*"  10 FILLER PIC X(30) VALUE 'RR RORAIMA'.*/
                public StringBasis FILLER_41 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"RR RORAIMA");
                /*"  10 FILLER PIC X(30) VALUE 'RS RIO G DO SUL'.*/
                public StringBasis FILLER_42 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"RS RIO G DO SUL");
                /*"  10 FILLER PIC X(30) VALUE 'SC SANTA CATARINA'.*/
                public StringBasis FILLER_43 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"SC SANTA CATARINA");
                /*"  10 FILLER PIC X(30) VALUE 'SE SERGIPE'.*/
                public StringBasis FILLER_44 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"SE SERGIPE");
                /*"  10 FILLER PIC X(30) VALUE 'SP SAO PAULO'.*/
                public StringBasis FILLER_45 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"SP SAO PAULO");
                /*"  10 FILLER PIC X(30) VALUE 'TO TOCANTINS'.*/
                public StringBasis FILLER_46 { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"TO TOCANTINS");
                /*" 05 TAB-UF-R REDEFINES TAB-UF OCCURS 27 TIMES.*/
            }
            private ListBasis<_REDEF_LT2000B_TAB_UF_R> _tab_uf_r { get; set; }
            public ListBasis<_REDEF_LT2000B_TAB_UF_R> TAB_UF_R
            {
                get { _tab_uf_r = new ListBasis<_REDEF_LT2000B_TAB_UF_R>(27); _.Move(TAB_UF, _tab_uf_r); VarBasis.RedefinePassValue(TAB_UF, _tab_uf_r, TAB_UF); _tab_uf_r.ValueChanged += () => { _.Move(_tab_uf_r, TAB_UF); }; return _tab_uf_r; }
                set { VarBasis.RedefinePassValue(value, _tab_uf_r, TAB_UF); }
            }  //Redefines
            public class _REDEF_LT2000B_TAB_UF_R : VarBasis
            {
                /*"  10 TB-UF   PIC X(02).*/
                public StringBasis TB_UF { get; set; } = new StringBasis(new PIC("X", "2", "X(02)."), @"");
                /*"  10 FILLER  PIC X(01).*/
                public StringBasis FILLER_47 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)."), @"");
                /*"  10 TB-EST  PIC X(27).*/
                public StringBasis TB_EST { get; set; } = new StringBasis(new PIC("X", "27", "X(27)."), @"");
                /*" 05 W-CAD-DATA-INI-VIG            PIC X(10) VALUE ZERO.*/

                public _REDEF_LT2000B_TAB_UF_R()
                {
                    TB_UF.ValueChanged += OnValueChanged;
                    FILLER_47.ValueChanged += OnValueChanged;
                    TB_EST.ValueChanged += OnValueChanged;
                }

            }
            public StringBasis W_CAD_DATA_INI_VIG { get; set; } = new StringBasis(new PIC("X", "10", "X(10)"), @"");
            /*" 05 W-CAD-DATA-EXCLUSAO           PIC X(10) VALUE ZERO.*/
            public StringBasis W_CAD_DATA_EXCLUSAO { get; set; } = new StringBasis(new PIC("X", "10", "X(10)"), @"");
            /*" 05 W-CAD-DATA-TER-VIG            PIC X(10) VALUE ZERO.*/
            public StringBasis W_CAD_DATA_TER_VIG { get; set; } = new StringBasis(new PIC("X", "10", "X(10)"), @"");
            /*" 05 W-CAD-DATA-INI-1P             PIC X(10) VALUE ZERO.*/
            public StringBasis W_CAD_DATA_INI_1P { get; set; } = new StringBasis(new PIC("X", "10", "X(10)"), @"");
            /*" 05 W-CAD-DATA-FIM-1P             PIC X(10) VALUE ZERO.*/
            public StringBasis W_CAD_DATA_FIM_1P { get; set; } = new StringBasis(new PIC("X", "10", "X(10)"), @"");
            /*" 05 W-CAD-DATA-GERACAO            PIC X(10) VALUE SPACES.*/
            public StringBasis W_CAD_DATA_GERACAO { get; set; } = new StringBasis(new PIC("X", "10", "X(10)"), @"");
            /*" 05 W-CAD-DATA-DEB.*/
            public LT2000B_W_CAD_DATA_DEB W_CAD_DATA_DEB { get; set; } = new LT2000B_W_CAD_DATA_DEB();
            public class LT2000B_W_CAD_DATA_DEB : VarBasis
            {
                /*"  10 FILLER                     PIC X(08) VALUE SPACES.*/
                public StringBasis FILLER_48 { get; set; } = new StringBasis(new PIC("X", "8", "X(08)"), @"");
                /*"  10 W-CAD-DATA-DEB-DD          PIC 9(02) VALUE ZERO.*/
                public IntBasis W_CAD_DATA_DEB_DD { get; set; } = new IntBasis(new PIC("9", "2", "9(02)"));
                /*" 05 W-DATA-DDMMAAAA.*/
            }
            public LT2000B_W_DATA_DDMMAAAA W_DATA_DDMMAAAA { get; set; } = new LT2000B_W_DATA_DDMMAAAA();
            public class LT2000B_W_DATA_DDMMAAAA : VarBasis
            {
                /*"  10 W-DATA-DDMMAAAA-DD         PIC 9(02) VALUE ZERO.*/
                public IntBasis W_DATA_DDMMAAAA_DD { get; set; } = new IntBasis(new PIC("9", "2", "9(02)"));
                /*"  10 W-DATA-DDMMAAAA-MM         PIC 9(02) VALUE ZERO.*/
                public IntBasis W_DATA_DDMMAAAA_MM { get; set; } = new IntBasis(new PIC("9", "2", "9(02)"));
                /*"  10 W-DATA-DDMMAAAA-AAAA       PIC 9(04) VALUE ZERO.*/
                public IntBasis W_DATA_DDMMAAAA_AAAA { get; set; } = new IntBasis(new PIC("9", "4", "9(04)"));
                /*" 05 W-DATA-AAAAMMDD.*/
            }
            public LT2000B_W_DATA_AAAAMMDD W_DATA_AAAAMMDD { get; set; } = new LT2000B_W_DATA_AAAAMMDD();
            public class LT2000B_W_DATA_AAAAMMDD : VarBasis
            {
                /*"  10 W-DATA-AAAAMMDD-AAAA       PIC 9(04) VALUE ZERO.*/
                public IntBasis W_DATA_AAAAMMDD_AAAA { get; set; } = new IntBasis(new PIC("9", "4", "9(04)"));
                /*"  10 W-DATA-AAAAMMDD-MM         PIC 9(02) VALUE ZERO.*/
                public IntBasis W_DATA_AAAAMMDD_MM { get; set; } = new IntBasis(new PIC("9", "2", "9(02)"));
                /*"  10 W-DATA-AAAAMMDD-DD         PIC 9(02) VALUE ZERO.*/
                public IntBasis W_DATA_AAAAMMDD_DD { get; set; } = new IntBasis(new PIC("9", "2", "9(02)"));
                /*" 05 W-DATA-AAAA-MM-DD.*/
            }
            public LT2000B_W_DATA_AAAA_MM_DD W_DATA_AAAA_MM_DD { get; set; } = new LT2000B_W_DATA_AAAA_MM_DD();
            public class LT2000B_W_DATA_AAAA_MM_DD : VarBasis
            {
                /*"  10 W-DATA-AAAA-MM-DD-AAAA     PIC 9(04) VALUE ZERO.*/
                public IntBasis W_DATA_AAAA_MM_DD_AAAA { get; set; } = new IntBasis(new PIC("9", "4", "9(04)"));
                /*"  10 FILLER                     PIC X(01) VALUE '-'.*/
                public StringBasis FILLER_49 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"-");
                /*"  10 W-DATA-AAAA-MM-DD-MM       PIC 9(02) VALUE ZERO.*/
                public IntBasis W_DATA_AAAA_MM_DD_MM { get; set; } = new IntBasis(new PIC("9", "2", "9(02)"));
                /*"  10 FILLER                     PIC X(01) VALUE '-'.*/
                public StringBasis FILLER_50 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"-");
                /*"  10 W-DATA-AAAA-MM-DD-DD       PIC 9(02) VALUE ZERO.*/
                public IntBasis W_DATA_AAAA_MM_DD_DD { get; set; } = new IntBasis(new PIC("9", "2", "9(02)"));
                /*" 05 LC01.*/
            }
            public LT2000B_LC01 LC01 { get; set; } = new LT2000B_LC01();
            public class LT2000B_LC01 : VarBasis
            {
                /*"  10 FILLER                     PIC X(43) VALUE 'LT2000B'.*/
                public StringBasis FILLER_51 { get; set; } = new StringBasis(new PIC("X", "43", "X(43)"), @"LT2000B");
                /*"  10 LC01-EMPRESA               PIC X(040) VALUE  SPACES.*/
                public StringBasis LC01_EMPRESA { get; set; } = new StringBasis(new PIC("X", "40", "X(040)"), @"");
                /*"  10 FILLER                     PIC X(036) VALUE  SPACES.*/
                public StringBasis FILLER_52 { get; set; } = new StringBasis(new PIC("X", "36", "X(036)"), @"");
                /*"  10 FILLER                     PIC X(009) VALUE 'PAGINA'.*/
                public StringBasis FILLER_53 { get; set; } = new StringBasis(new PIC("X", "9", "X(009)"), @"PAGINA");
                /*"  10 LC01-PAGINA                PIC ZZZ9.*/
                public IntBasis LC01_PAGINA { get; set; } = new IntBasis(new PIC("9", "4", "ZZZ9."));
                /*" 05 LC02.*/
            }
            public LT2000B_LC02 LC02 { get; set; } = new LT2000B_LC02();
            public class LT2000B_LC02 : VarBasis
            {
                /*"  10 FILLER                     PIC X(119) VALUE  SPACES.*/
                public StringBasis FILLER_54 { get; set; } = new StringBasis(new PIC("X", "119", "X(119)"), @"");
                /*"  10 FILLER                     PIC X(05) VALUE 'DATA'.*/
                public StringBasis FILLER_55 { get; set; } = new StringBasis(new PIC("X", "5", "X(05)"), @"DATA");
                /*"  10 LC02-DATA                  PIC X(08) VALUE  SPACES.*/
                public StringBasis LC02_DATA { get; set; } = new StringBasis(new PIC("X", "8", "X(08)"), @"");
                /*" 05 LC03.*/
            }
            public LT2000B_LC03 LC03 { get; set; } = new LT2000B_LC03();
            public class LT2000B_LC03 : VarBasis
            {
                /*"  10 FILLER                     PIC X(33) VALUE  SPACES.*/
                public StringBasis FILLER_56 { get; set; } = new StringBasis(new PIC("X", "33", "X(33)"), @"");
                /*"  10 FILLER                     PIC X(86) VALUE          ' RELATORIO DE INCONSISTENCIAS NO MOVIMENTO DE          'LOTERICOS - SIGEL'.*/
                public StringBasis FILLER_57 { get; set; } = new StringBasis(new PIC("X", "86", "X(86)"), @" RELATORIO DE INCONSISTENCIAS NO MOVIMENTO DE          ");
                /*"  10 FILLER                     PIC X(05) VALUE 'HORA'.*/
                public StringBasis FILLER_58 { get; set; } = new StringBasis(new PIC("X", "5", "X(05)"), @"HORA");
                /*"  10 LC03-HORA                  PIC X(08) VALUE  SPACES.*/
                public StringBasis LC03_HORA { get; set; } = new StringBasis(new PIC("X", "8", "X(08)"), @"");
                /*" 05 LC05.*/
            }
            public LT2000B_LC05 LC05 { get; set; } = new LT2000B_LC05();
            public class LT2000B_LC05 : VarBasis
            {
                /*"  10 FILLER                     PIC X(132) VALUE ALL '-'.*/
                public StringBasis FILLER_59 { get; set; } = new StringBasis(new PIC("X", "132", "X(132)"), @"ALL");
                /*" 05 LC06.*/
            }
            public LT2000B_LC06 LC06 { get; set; } = new LT2000B_LC06();
            public class LT2000B_LC06 : VarBasis
            {
                /*"  10 FILLER                     PIC X(132) VALUE ALL ' '.*/
                public StringBasis FILLER_60 { get; set; } = new StringBasis(new PIC("X", "132", "X(132)"), @"ALL");
                /*" 05 LD01-CAD.*/
            }
            public LT2000B_LD01_CAD LD01_CAD { get; set; } = new LT2000B_LD01_CAD();
            public class LT2000B_LD01_CAD : VarBasis
            {
                /*"  10 FILLER                     PIC X(11) VALUE          'COD. CEF:'.*/
                public StringBasis FILLER_61 { get; set; } = new StringBasis(new PIC("X", "11", "X(11)"), @"COD. CEF:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_62 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD01-CAD-COD-CEF           PIC X(09) VALUE SPACES.*/
                public StringBasis LD01_CAD_COD_CEF { get; set; } = new StringBasis(new PIC("X", "9", "X(09)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_63 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(13) VALUE          'RAZAO SOCIAL:'.*/
                public StringBasis FILLER_64 { get; set; } = new StringBasis(new PIC("X", "13", "X(13)"), @"RAZAO SOCIAL:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_65 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD01-CAD-RAZAO-SOCIAL      PIC X(56) VALUE SPACES.*/
                public StringBasis LD01_CAD_RAZAO_SOCIAL { get; set; } = new StringBasis(new PIC("X", "56", "X(56)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_66 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(09) VALUE          'SITUACAO:'.*/
                public StringBasis FILLER_67 { get; set; } = new StringBasis(new PIC("X", "9", "X(09)"), @"SITUACAO:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_68 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD01-SITUACAO              PIC X(01) VALUE SPACES.*/
                public StringBasis LD01_SITUACAO { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*" 05 LD02-CAD.*/
            }
            public LT2000B_LD02_CAD LD02_CAD { get; set; } = new LT2000B_LD02_CAD();
            public class LT2000B_LD02_CAD : VarBasis
            {
                /*"  10 FILLER                     PIC X(08) VALUE          'C.G.C.:'.*/
                public StringBasis FILLER_69 { get; set; } = new StringBasis(new PIC("X", "8", "X(08)"), @"C.G.C.:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_70 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD02-CAD-CGC               PIC X(16) VALUE SPACES.*/
                public StringBasis LD02_CAD_CGC { get; set; } = new StringBasis(new PIC("X", "16", "X(16)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_71 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(05) VALUE          'END.:'.*/
                public StringBasis FILLER_72 { get; set; } = new StringBasis(new PIC("X", "5", "X(05)"), @"END.:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_73 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD02-CAD-END               PIC X(50) VALUE SPACES.*/
                public StringBasis LD02_CAD_END { get; set; } = new StringBasis(new PIC("X", "50", "X(50)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_74 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(07) VALUE          'BAIRRO:'.*/
                public StringBasis FILLER_75 { get; set; } = new StringBasis(new PIC("X", "7", "X(07)"), @"BAIRRO:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_76 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD02-CAD-BAIRRO            PIC X(20) VALUE SPACES.*/
                public StringBasis LD02_CAD_BAIRRO { get; set; } = new StringBasis(new PIC("X", "20", "X(20)"), @"");
                /*" 05 LD02A-CAD.*/
            }
            public LT2000B_LD02A_CAD LD02A_CAD { get; set; } = new LT2000B_LD02A_CAD();
            public class LT2000B_LD02A_CAD : VarBasis
            {
                /*"  10 FILLER                     PIC X(09) VALUE          'INSC MUN:'.*/
                public StringBasis FILLER_77 { get; set; } = new StringBasis(new PIC("X", "9", "X(09)"), @"INSC MUN:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_78 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD02A-CAD-INSC-MUN         PIC X(16) VALUE SPACES.*/
                public StringBasis LD02A_CAD_INSC_MUN { get; set; } = new StringBasis(new PIC("X", "16", "X(16)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_79 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(09) VALUE          'INSC EST:'.*/
                public StringBasis FILLER_80 { get; set; } = new StringBasis(new PIC("X", "9", "X(09)"), @"INSC EST:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_81 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD02A-CAD-INSC-EST         PIC X(16) VALUE SPACES.*/
                public StringBasis LD02A_CAD_INSC_EST { get; set; } = new StringBasis(new PIC("X", "16", "X(16)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_82 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*" 05 LD02B-CAD.*/
            }
            public LT2000B_LD02B_CAD LD02B_CAD { get; set; } = new LT2000B_LD02B_CAD();
            public class LT2000B_LD02B_CAD : VarBasis
            {
                /*"  10 FILLER                     PIC X(09) VALUE          'LOT-ANT :'.*/
                public StringBasis FILLER_83 { get; set; } = new StringBasis(new PIC("X", "9", "X(09)"), @"LOT-ANT :");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_84 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD02B-LOT-ANT              PIC X(09) VALUE SPACES.*/
                public StringBasis LD02B_LOT_ANT { get; set; } = new StringBasis(new PIC("X", "9", "X(09)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_85 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(12) VALUE          'AGEN MASTER:'.*/
                public StringBasis FILLER_86 { get; set; } = new StringBasis(new PIC("X", "12", "X(12)"), @"AGEN MASTER:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_87 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD02B-AGE-MASTER           PIC X(09) VALUE SPACES.*/
                public StringBasis LD02B_AGE_MASTER { get; set; } = new StringBasis(new PIC("X", "9", "X(09)"), @"");
                /*"  10 FILLER                     PIC X(10) VALUE SPACES.*/
                public StringBasis FILLER_88 { get; set; } = new StringBasis(new PIC("X", "10", "X(10)"), @"");
                /*"  10 FILLER                     PIC X(10) VALUE          'CATEGORIA:'.*/
                public StringBasis FILLER_89 { get; set; } = new StringBasis(new PIC("X", "10", "X(10)"), @"CATEGORIA:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_90 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD02B-CATEGORIA            PIC X(02) VALUE SPACES.*/
                public StringBasis LD02B_CATEGORIA { get; set; } = new StringBasis(new PIC("X", "2", "X(02)"), @"");
                /*"  10 FILLER                     PIC X(03) VALUE SPACES.*/
                public StringBasis FILLER_91 { get; set; } = new StringBasis(new PIC("X", "3", "X(03)"), @"");
                /*"  10 FILLER                     PIC X(07) VALUE          'STATUS:'.*/
                public StringBasis FILLER_92 { get; set; } = new StringBasis(new PIC("X", "7", "X(07)"), @"STATUS:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_93 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD02B-STATUS               PIC X(02) VALUE SPACES.*/
                public StringBasis LD02B_STATUS { get; set; } = new StringBasis(new PIC("X", "2", "X(02)"), @"");
                /*" 05 LD03-CAD.*/
            }
            public LT2000B_LD03_CAD LD03_CAD { get; set; } = new LT2000B_LD03_CAD();
            public class LT2000B_LD03_CAD : VarBasis
            {
                /*"  10 FILLER                     PIC X(04) VALUE          'CEP:'.*/
                public StringBasis FILLER_94 { get; set; } = new StringBasis(new PIC("X", "4", "X(04)"), @"CEP:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_95 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD03-CAD-CEP-1             PIC 9(08) VALUE 0.*/
                public IntBasis LD03_CAD_CEP_1 { get; set; } = new IntBasis(new PIC("9", "8", "9(08)"));
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_96 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(07) VALUE          'CIDADE:'.*/
                public StringBasis FILLER_97 { get; set; } = new StringBasis(new PIC("X", "7", "X(07)"), @"CIDADE:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_98 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD03-CAD-CIDADE            PIC X(25) VALUE SPACES.*/
                public StringBasis LD03_CAD_CIDADE { get; set; } = new StringBasis(new PIC("X", "25", "X(25)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_99 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(03) VALUE          'UF:'.*/
                public StringBasis FILLER_100 { get; set; } = new StringBasis(new PIC("X", "3", "X(03)"), @"UF:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_101 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD03-CAD-UF                PIC X(02) VALUE SPACES.*/
                public StringBasis LD03_CAD_UF { get; set; } = new StringBasis(new PIC("X", "2", "X(02)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_102 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(05) VALUE          'FONE:'.*/
                public StringBasis FILLER_103 { get; set; } = new StringBasis(new PIC("X", "5", "X(05)"), @"FONE:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_104 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD03-CAD-DDD-FONE          PIC 9(03) VALUE 0.*/
                public IntBasis LD03_CAD_DDD_FONE { get; set; } = new IntBasis(new PIC("9", "3", "9(03)"));
                /*"  10 FILLER                     PIC X(01) VALUE '-'.*/
                public StringBasis FILLER_105 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"-");
                /*"  10 LD03-CAD-FONE              PIC 9(08) VALUE 0.*/
                public IntBasis LD03_CAD_FONE { get; set; } = new IntBasis(new PIC("9", "8", "9(08)"));
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_106 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(06) VALUE          ' FAX:'.*/
                public StringBasis FILLER_107 { get; set; } = new StringBasis(new PIC("X", "6", "X(06)"), @" FAX:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_108 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD03-CAD-DDD-FAX           PIC 9(04) VALUE 0.*/
                public IntBasis LD03_CAD_DDD_FAX { get; set; } = new IntBasis(new PIC("9", "4", "9(04)"));
                /*"  10 FILLER                     PIC X(01) VALUE '-'.*/
                public StringBasis FILLER_109 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"-");
                /*"  10 LD03-CAD-FAX               PIC 9(08) VALUE 0.*/
                public IntBasis LD03_CAD_FAX { get; set; } = new IntBasis(new PIC("9", "8", "9(08)"));
                /*" 05 LD04-CAD.*/
            }
            public LT2000B_LD04_CAD LD04_CAD { get; set; } = new LT2000B_LD04_CAD();
            public class LT2000B_LD04_CAD : VarBasis
            {
                /*"  10 FILLER                     PIC X(12) VALUE          'IS.INCENDIO:'.*/
                public StringBasis FILLER_110 { get; set; } = new StringBasis(new PIC("X", "12", "X(12)"), @"IS.INCENDIO:");
                /*"  10 LD04-CAD-INCENDIO          PIC ZZZ.ZZZ.ZZ9,99.*/
                public DoubleBasis LD04_CAD_INCENDIO { get; set; } = new DoubleBasis(new PIC("9", "9", "ZZZ.ZZZ.ZZ9V99."), 2);
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_111 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(07) VALUE          'TX INC:'.*/
                public StringBasis FILLER_112 { get; set; } = new StringBasis(new PIC("X", "7", "X(07)"), @"TX INC:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_113 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD04-CAD-TAXA-IS-INCENDIO  PIC ZZ9,999999999.*/
                public DoubleBasis LD04_CAD_TAXA_IS_INCENDIO { get; set; } = new DoubleBasis(new PIC("9", "3", "ZZ9V999999999."), 9);
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_114 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(12) VALUE          'DANOS ELE:'.*/
                public StringBasis FILLER_115 { get; set; } = new StringBasis(new PIC("X", "12", "X(12)"), @"DANOS ELE:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_116 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD04-CAD-DANOS             PIC ZZZ.ZZZ.ZZ9,99.*/
                public DoubleBasis LD04_CAD_DANOS { get; set; } = new DoubleBasis(new PIC("9", "9", "ZZZ.ZZZ.ZZ9V99."), 2);
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_117 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(07) VALUE          'TX DEL:'.*/
                public StringBasis FILLER_118 { get; set; } = new StringBasis(new PIC("X", "7", "X(07)"), @"TX DEL:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_119 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD04-CAD-TAXA-IS-DANOS     PIC ZZ9,999999999.*/
                public DoubleBasis LD04_CAD_TAXA_IS_DANOS { get; set; } = new DoubleBasis(new PIC("9", "3", "ZZ9V999999999."), 9);
                /*" 05 LD04-CAD-A.*/
            }
            public LT2000B_LD04_CAD_A LD04_CAD_A { get; set; } = new LT2000B_LD04_CAD_A();
            public class LT2000B_LD04_CAD_A : VarBasis
            {
                /*"  10 FILLER                     PIC X(12) VALUE          'IS ACIDENTE:'.*/
                public StringBasis FILLER_120 { get; set; } = new StringBasis(new PIC("X", "12", "X(12)"), @"IS ACIDENTE:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_121 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD04-CAD-ACIDENTE          PIC ZZZ.ZZZ.ZZ9,99.*/
                public DoubleBasis LD04_CAD_ACIDENTE { get; set; } = new DoubleBasis(new PIC("9", "9", "ZZZ.ZZZ.ZZ9V99."), 2);
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_122 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(07) VALUE          'TX ACI:'.*/
                public StringBasis FILLER_123 { get; set; } = new StringBasis(new PIC("X", "7", "X(07)"), @"TX ACI:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_124 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD04-CAD-TAXA-IS-ACIDENTE  PIC ZZ9,999999999.*/
                public DoubleBasis LD04_CAD_TAXA_IS_ACIDENTE { get; set; } = new DoubleBasis(new PIC("9", "3", "ZZ9V999999999."), 9);
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_125 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(12) VALUE          'IS VALORES:'.*/
                public StringBasis FILLER_126 { get; set; } = new StringBasis(new PIC("X", "12", "X(12)"), @"IS VALORES:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_127 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD04-CAD-VALORES           PIC ZZZ.ZZZ.ZZ9,99.*/
                public DoubleBasis LD04_CAD_VALORES { get; set; } = new DoubleBasis(new PIC("9", "9", "ZZZ.ZZZ.ZZ9V99."), 2);
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_128 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(07) VALUE          'TX VAL:'.*/
                public StringBasis FILLER_129 { get; set; } = new StringBasis(new PIC("X", "7", "X(07)"), @"TX VAL:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_130 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD04-CAD-TAXA-IS-VALORES   PIC ZZ9,999999999.*/
                public DoubleBasis LD04_CAD_TAXA_IS_VALORES { get; set; } = new DoubleBasis(new PIC("9", "3", "ZZ9V999999999."), 9);
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_131 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(09) VALUE          'TIPO GAR:'.*/
                public StringBasis FILLER_132 { get; set; } = new StringBasis(new PIC("X", "9", "X(09)"), @"TIPO GAR:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_133 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD04-CAD-TIPO-GARANTIA     PIC X(01) VALUE SPACES.*/
                public StringBasis LD04_CAD_TIPO_GARANTIA { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*" 05 LD04-CAD-B.*/
            }
            public LT2000B_LD04_CAD_B LD04_CAD_B { get; set; } = new LT2000B_LD04_CAD_B();
            public class LT2000B_LD04_CAD_B : VarBasis
            {
                /*"  10 FILLER                     PIC X(08) VALUE          'B. SIN.:'.*/
                public StringBasis FILLER_134 { get; set; } = new StringBasis(new PIC("X", "8", "X(08)"), @"B. SIN.:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_135 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD04-CAD-BONUS-SIN         PIC X(01) VALUE SPACES.*/
                public StringBasis LD04_CAD_BONUS_SIN { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_136 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(08) VALUE          'B. ALA.:'.*/
                public StringBasis FILLER_137 { get; set; } = new StringBasis(new PIC("X", "8", "X(08)"), @"B. ALA.:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_138 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD04-CAD-BONUS-ALA         PIC X(02) VALUE SPACES.*/
                public StringBasis LD04_CAD_BONUS_ALA { get; set; } = new StringBasis(new PIC("X", "2", "X(02)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_139 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(08) VALUE          'B. CKT.:'.*/
                public StringBasis FILLER_140 { get; set; } = new StringBasis(new PIC("X", "8", "X(08)"), @"B. CKT.:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_141 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD04-CAD-BONUS-CKT         PIC X(02) VALUE SPACES.*/
                public StringBasis LD04_CAD_BONUS_CKT { get; set; } = new StringBasis(new PIC("X", "2", "X(02)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_142 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(08) VALUE          'B. COF.:'.*/
                public StringBasis FILLER_143 { get; set; } = new StringBasis(new PIC("X", "8", "X(08)"), @"B. COF.:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_144 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD04-CAD-BONUS-COF         PIC X(02) VALUE SPACES.*/
                public StringBasis LD04_CAD_BONUS_COF { get; set; } = new StringBasis(new PIC("X", "2", "X(02)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_145 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*" 05 LD05-CAD.*/
            }
            public LT2000B_LD05_CAD LD05_CAD { get; set; } = new LT2000B_LD05_CAD();
            public class LT2000B_LD05_CAD : VarBasis
            {
                /*"  10 FILLER                     PIC X(15) VALUE          'DATA INCLUSAO:'.*/
                public StringBasis FILLER_146 { get; set; } = new StringBasis(new PIC("X", "15", "X(15)"), @"DATA INCLUSAO:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_147 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD05-CAD-DATA-INCLUSAO     PIC X(10) VALUE SPACES.*/
                public StringBasis LD05_CAD_DATA_INCLUSAO { get; set; } = new StringBasis(new PIC("X", "10", "X(10)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_148 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(15) VALUE          'DATA EXCLUSAO :'.*/
                public StringBasis FILLER_149 { get; set; } = new StringBasis(new PIC("X", "15", "X(15)"), @"DATA EXCLUSAO :");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_150 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD05-CAD-DATA-EXCLUSAO     PIC X(10) VALUE SPACES.*/
                public StringBasis LD05_CAD_DATA_EXCLUSAO { get; set; } = new StringBasis(new PIC("X", "10", "X(10)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_151 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(15) VALUE          'DATA GERACAO  :'.*/
                public StringBasis FILLER_152 { get; set; } = new StringBasis(new PIC("X", "15", "X(15)"), @"DATA GERACAO  :");
                /*"  10 LD05-CAD-DATA-GERACAO      PIC X(10) VALUE SPACES.*/
                public StringBasis LD05_CAD_DATA_GERACAO { get; set; } = new StringBasis(new PIC("X", "10", "X(10)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_153 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(11) VALUE          'NR.SEQ REG:'.*/
                public StringBasis FILLER_154 { get; set; } = new StringBasis(new PIC("X", "11", "X(11)"), @"NR.SEQ REG:");
                /*"  10 LD05-NSR                   PIC X(06) VALUE SPACES.*/
                public StringBasis LD05_NSR { get; set; } = new StringBasis(new PIC("X", "6", "X(06)"), @"");
                /*" 05 LD06-CAD.*/
            }
            public LT2000B_LD06_CAD LD06_CAD { get; set; } = new LT2000B_LD06_CAD();
            public class LT2000B_LD06_CAD : VarBasis
            {
                /*"  10 FILLER                     PIC X(09) VALUE          'MAT.CONS:'.*/
                public StringBasis FILLER_155 { get; set; } = new StringBasis(new PIC("X", "9", "X(09)"), @"MAT.CONS:");
                /*"  10 LD06-MAT-CONSULTOR         PIC X(07) VALUE SPACES.*/
                public StringBasis LD06_MAT_CONSULTOR { get; set; } = new StringBasis(new PIC("X", "7", "X(07)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_156 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(10) VALUE          'NOME CONS:'.*/
                public StringBasis FILLER_157 { get; set; } = new StringBasis(new PIC("X", "10", "X(10)"), @"NOME CONS:");
                /*"  10 LD06-NOME-CONSULTOR        PIC X(40).*/
                public StringBasis LD06_NOME_CONSULTOR { get; set; } = new StringBasis(new PIC("X", "40", "X(40)."), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_158 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(06) VALUE          'EMAIL:'.*/
                public StringBasis FILLER_159 { get; set; } = new StringBasis(new PIC("X", "6", "X(06)"), @"EMAIL:");
                /*"  10 LD06-EMAIL                 PIC X(50).*/
                public StringBasis LD06_EMAIL { get; set; } = new StringBasis(new PIC("X", "50", "X(50)."), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_160 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*" 05 LD06A-CAD.*/
            }
            public LT2000B_LD06A_CAD LD06A_CAD { get; set; } = new LT2000B_LD06A_CAD();
            public class LT2000B_LD06A_CAD : VarBasis
            {
                /*"  10 FILLER                     PIC X(07) VALUE          'PV-SUB:'.*/
                public StringBasis FILLER_161 { get; set; } = new StringBasis(new PIC("X", "7", "X(07)"), @"PV-SUB:");
                /*"  10 LD06A-PV-SUB               PIC X(04) VALUE SPACES.*/
                public StringBasis LD06A_PV_SUB { get; set; } = new StringBasis(new PIC("X", "4", "X(04)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_162 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(07) VALUE          'EN SUB:'.*/
                public StringBasis FILLER_163 { get; set; } = new StringBasis(new PIC("X", "7", "X(07)"), @"EN SUB:");
                /*"  10 LD06A-EN-SUB               PIC X(04).*/
                public StringBasis LD06A_EN_SUB { get; set; } = new StringBasis(new PIC("X", "4", "X(04)."), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_164 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(09) VALUE          'UNID SUB:'.*/
                public StringBasis FILLER_165 { get; set; } = new StringBasis(new PIC("X", "9", "X(09)"), @"UNID SUB:");
                /*"  10 LD06A-UNIDADE-SUB          PIC X(04).*/
                public StringBasis LD06A_UNIDADE_SUB { get; set; } = new StringBasis(new PIC("X", "4", "X(04)."), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_166 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(12) VALUE          'NIVEL COMIS:'.*/
                public StringBasis FILLER_167 { get; set; } = new StringBasis(new PIC("X", "12", "X(12)"), @"NIVEL COMIS:");
                /*"  10 LD06A-NIVEL-COMISSAO       PIC X(01).*/
                public StringBasis LD06A_NIVEL_COMISSAO { get; set; } = new StringBasis(new PIC("X", "1", "X(01)."), @"");
                /*" 05 LD06B-CAD.*/
            }
            public LT2000B_LD06B_CAD LD06B_CAD { get; set; } = new LT2000B_LD06B_CAD();
            public class LT2000B_LD06B_CAD : VarBasis
            {
                /*"  10 FILLER                     PIC X(11) VALUE          'N.FANTASIA:'.*/
                public StringBasis FILLER_168 { get; set; } = new StringBasis(new PIC("X", "11", "X(11)"), @"N.FANTASIA:");
                /*"  10 LD06B-NOME-FANTASIA        PIC X(30) VALUE SPACES.*/
                public StringBasis LD06B_NOME_FANTASIA { get; set; } = new StringBasis(new PIC("X", "30", "X(30)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_169 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(09) VALUE          'CONTATO1:'.*/
                public StringBasis FILLER_170 { get; set; } = new StringBasis(new PIC("X", "9", "X(09)"), @"CONTATO1:");
                /*"  10 LD06B-CONTATO1             PIC X(20).*/
                public StringBasis LD06B_CONTATO1 { get; set; } = new StringBasis(new PIC("X", "20", "X(20)."), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_171 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(09) VALUE          'CONTATO2:'.*/
                public StringBasis FILLER_172 { get; set; } = new StringBasis(new PIC("X", "9", "X(09)"), @"CONTATO2:");
                /*"  10 LD06B-CONTATO2             PIC X(20).*/
                public StringBasis LD06B_CONTATO2 { get; set; } = new StringBasis(new PIC("X", "20", "X(20)."), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_173 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(10) VALUE          'COD.MUNIC:'.*/
                public StringBasis FILLER_174 { get; set; } = new StringBasis(new PIC("X", "10", "X(10)"), @"COD.MUNIC:");
                /*"  10 LD06B-COD-MUNICIPIO        PIC X(12).*/
                public StringBasis LD06B_COD_MUNICIPIO { get; set; } = new StringBasis(new PIC("X", "12", "X(12)."), @"");
                /*" 05 LD06C-CAD.*/
            }
            public LT2000B_LD06C_CAD LD06C_CAD { get; set; } = new LT2000B_LD06C_CAD();
            public class LT2000B_LD06C_CAD : VarBasis
            {
                /*"  10 FILLER                     PIC X(17) VALUE          'NUM. SEGURADORA:'.*/
                public StringBasis FILLER_175 { get; set; } = new StringBasis(new PIC("X", "17", "X(17)"), @"NUM. SEGURADORA:");
                /*"  10 LD06C-NUM-SEGURADORA      PIC X(50) VALUE SPACES.*/
                public StringBasis LD06C_NUM_SEGURADORA { get; set; } = new StringBasis(new PIC("X", "50", "X(50)"), @"");
                /*" 05 LD07-CAD.*/
            }
            public LT2000B_LD07_CAD LD07_CAD { get; set; } = new LT2000B_LD07_CAD();
            public class LT2000B_LD07_CAD : VarBasis
            {
                /*"  10 FILLER                     PIC X(17) VALUE          'C/C CPMF   BANCO:'.*/
                public StringBasis FILLER_176 { get; set; } = new StringBasis(new PIC("X", "17", "X(17)"), @"C/C CPMF   BANCO:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_177 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD07-CAD-BANCO             PIC 9(03) VALUE 0.*/
                public IntBasis LD07_CAD_BANCO { get; set; } = new IntBasis(new PIC("9", "3", "9(03)"));
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_178 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(08) VALUE          'AGENCIA:'.*/
                public StringBasis FILLER_179 { get; set; } = new StringBasis(new PIC("X", "8", "X(08)"), @"AGENCIA:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_180 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD07-CAD-AGENCIA           PIC 9(04) VALUE 0.*/
                public IntBasis LD07_CAD_AGENCIA { get; set; } = new IntBasis(new PIC("9", "4", "9(04)"));
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_181 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(09) VALUE          'OPERACAO:'.*/
                public StringBasis FILLER_182 { get; set; } = new StringBasis(new PIC("X", "9", "X(09)"), @"OPERACAO:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_183 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD07-CAD-OPERACAO          PIC X(04) VALUE SPACES.*/
                public StringBasis LD07_CAD_OPERACAO { get; set; } = new StringBasis(new PIC("X", "4", "X(04)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_184 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(06) VALUE          'CONTA:'.*/
                public StringBasis FILLER_185 { get; set; } = new StringBasis(new PIC("X", "6", "X(06)"), @"CONTA:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_186 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD07-CAD-CONTA             PIC X(12) VALUE SPACES.*/
                public StringBasis LD07_CAD_CONTA { get; set; } = new StringBasis(new PIC("X", "12", "X(12)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_187 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(03) VALUE          'DV:'.*/
                public StringBasis FILLER_188 { get; set; } = new StringBasis(new PIC("X", "3", "X(03)"), @"DV:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_189 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD07-CAD-DV-CONTA          PIC X(01) VALUE SPACES.*/
                public StringBasis LD07_CAD_DV_CONTA { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(02) VALUE SPACES.*/
                public StringBasis FILLER_190 { get; set; } = new StringBasis(new PIC("X", "2", "X(02)"), @"");
                /*" 05 LD07A-CAD.*/
            }
            public LT2000B_LD07A_CAD LD07A_CAD { get; set; } = new LT2000B_LD07A_CAD();
            public class LT2000B_LD07A_CAD : VarBasis
            {
                /*"  10 FILLER                     PIC X(17) VALUE          'C/C ISENTA BANCO:'.*/
                public StringBasis FILLER_191 { get; set; } = new StringBasis(new PIC("X", "17", "X(17)"), @"C/C ISENTA BANCO:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_192 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD07A-CAD-BANCO            PIC 9(03) VALUE 0.*/
                public IntBasis LD07A_CAD_BANCO { get; set; } = new IntBasis(new PIC("9", "3", "9(03)"));
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_193 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(08) VALUE          'AGENCIA:'.*/
                public StringBasis FILLER_194 { get; set; } = new StringBasis(new PIC("X", "8", "X(08)"), @"AGENCIA:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_195 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD07A-CAD-AGENCIA          PIC 9(04) VALUE 0.*/
                public IntBasis LD07A_CAD_AGENCIA { get; set; } = new IntBasis(new PIC("9", "4", "9(04)"));
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_196 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(09) VALUE          'OPERACAO:'.*/
                public StringBasis FILLER_197 { get; set; } = new StringBasis(new PIC("X", "9", "X(09)"), @"OPERACAO:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_198 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD07A-CAD-OPERACAO         PIC X(04) VALUE SPACES.*/
                public StringBasis LD07A_CAD_OPERACAO { get; set; } = new StringBasis(new PIC("X", "4", "X(04)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_199 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(06) VALUE          'CONTA:'.*/
                public StringBasis FILLER_200 { get; set; } = new StringBasis(new PIC("X", "6", "X(06)"), @"CONTA:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_201 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD07A-CAD-CONTA            PIC X(12) VALUE SPACES.*/
                public StringBasis LD07A_CAD_CONTA { get; set; } = new StringBasis(new PIC("X", "12", "X(12)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_202 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(03) VALUE          'DV:'.*/
                public StringBasis FILLER_203 { get; set; } = new StringBasis(new PIC("X", "3", "X(03)"), @"DV:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_204 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD07A-CAD-DV-CONTA         PIC X(01) VALUE SPACES.*/
                public StringBasis LD07A_CAD_DV_CONTA { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(02) VALUE SPACES.*/
                public StringBasis FILLER_205 { get; set; } = new StringBasis(new PIC("X", "2", "X(02)"), @"");
                /*" 05 LD07B-CAD.*/
            }
            public LT2000B_LD07B_CAD LD07B_CAD { get; set; } = new LT2000B_LD07B_CAD();
            public class LT2000B_LD07B_CAD : VarBasis
            {
                /*"  10 FILLER                     PIC X(17) VALUE          'C/C CAUCAO BANCO:'.*/
                public StringBasis FILLER_206 { get; set; } = new StringBasis(new PIC("X", "17", "X(17)"), @"C/C CAUCAO BANCO:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_207 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD07B-CAD-BANCO            PIC 9(03) VALUE 0.*/
                public IntBasis LD07B_CAD_BANCO { get; set; } = new IntBasis(new PIC("9", "3", "9(03)"));
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_208 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(08) VALUE          'AGENCIA:'.*/
                public StringBasis FILLER_209 { get; set; } = new StringBasis(new PIC("X", "8", "X(08)"), @"AGENCIA:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_210 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD07B-CAD-AGENCIA          PIC 9(04) VALUE 0.*/
                public IntBasis LD07B_CAD_AGENCIA { get; set; } = new IntBasis(new PIC("9", "4", "9(04)"));
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_211 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(09) VALUE          'OPERACAO:'.*/
                public StringBasis FILLER_212 { get; set; } = new StringBasis(new PIC("X", "9", "X(09)"), @"OPERACAO:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_213 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD07B-CAD-OPERACAO         PIC X(04) VALUE SPACES.*/
                public StringBasis LD07B_CAD_OPERACAO { get; set; } = new StringBasis(new PIC("X", "4", "X(04)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_214 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(06) VALUE          'CONTA:'.*/
                public StringBasis FILLER_215 { get; set; } = new StringBasis(new PIC("X", "6", "X(06)"), @"CONTA:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_216 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD07B-CAD-CONTA            PIC X(12) VALUE SPACES.*/
                public StringBasis LD07B_CAD_CONTA { get; set; } = new StringBasis(new PIC("X", "12", "X(12)"), @"");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_217 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(03) VALUE          'DV:'.*/
                public StringBasis FILLER_218 { get; set; } = new StringBasis(new PIC("X", "3", "X(03)"), @"DV:");
                /*"  10 FILLER                     PIC X(01) VALUE SPACES.*/
                public StringBasis FILLER_219 { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 LD07B-CAD-DV-CONTA         PIC X(01) VALUE SPACES.*/
                public StringBasis LD07B_CAD_DV_CONTA { get; set; } = new StringBasis(new PIC("X", "1", "X(01)"), @"");
                /*"  10 FILLER                     PIC X(02) VALUE SPACES.*/
                public StringBasis FILLER_220 { get; set; } = new StringBasis(new PIC("X", "2", "X(02)"), @"");
                /*" 05 LD00.*/
            }
            public LT2000B_LD00 LD00 { get; set; } = new LT2000B_LD00();
            public class LT2000B_LD00 : VarBasis
            {
                /*"  10 FILLER                     PIC X(010) VALUE SPACES.*/
                public StringBasis FILLER_221 { get; set; } = new StringBasis(new PIC("X", "10", "X(010)"), @"");
                /*"  10 LD00-MSG1                  PIC X(122) VALUE SPACES.*/
                public StringBasis LD00_MSG1 { get; set; } = new StringBasis(new PIC("X", "122", "X(122)"), @"");
                /*" 05 LT00.*/
            }
            public LT2000B_LT00 LT00 { get; set; } = new LT2000B_LT00();
            public class LT2000B_LT00 : VarBasis
            {
                /*"  10 FILLER                     PIC X(10) VALUE SPACES.*/
                public StringBasis FILLER_222 { get; set; } = new StringBasis(new PIC("X", "10", "X(10)"), @"");
                /*"  10 LT00-TEXTO                 PIC X(50) VALUE SPACES.*/
                public StringBasis LT00_TEXTO { get; set; } = new StringBasis(new PIC("X", "50", "X(50)"), @"");
                /*"  10 FILLER                     PIC X(10) VALUE ALL '.'.*/
                public StringBasis FILLER_223 { get; set; } = new StringBasis(new PIC("X", "10", "X(10)"), @"ALL");
                /*"  10 FILLER                     PIC X(03) VALUE SPACES.*/
                public StringBasis FILLER_224 { get; set; } = new StringBasis(new PIC("X", "3", "X(03)"), @"");
                /*"  10 LT00-TOTAIS                PIC ZZZ.ZZZ.ZZ9.*/
                public IntBasis LT00_TOTAIS { get; set; } = new IntBasis(new PIC("9", "9", "ZZZ.ZZZ.ZZ9."));
                /*" 05 LT01.*/
            }
            public LT2000B_LT01 LT01 { get; set; } = new LT2000B_LT01();
            public class LT2000B_LT01 : VarBasis
            {
                /*"  10 FILLER                     PIC X(10) VALUE SPACES.*/
                public StringBasis FILLER_225 { get; set; } = new StringBasis(new PIC("X", "10", "X(10)"), @"");
                /*"  10 LT01-TEXTO                 PIC X(50) VALUE SPACES.*/
                public StringBasis LT01_TEXTO { get; set; } = new StringBasis(new PIC("X", "50", "X(50)"), @"");
                /*"  10 FILLER                     PIC X(10) VALUE ALL '.'.*/
                public StringBasis FILLER_226 { get; set; } = new StringBasis(new PIC("X", "10", "X(10)"), @"ALL");
                /*"  10 LT01-TOTAIS                PIC ZZZ.ZZZ.ZZ9,99.*/
                public DoubleBasis LT01_TOTAIS { get; set; } = new DoubleBasis(new PIC("9", "9", "ZZZ.ZZZ.ZZ9V99."), 2);
                /*"01 LK-CONVERSAO.*/
            }
        }
        public LT2000B_LK_CONVERSAO LK_CONVERSAO { get; set; } = new LT2000B_LK_CONVERSAO();
        public class LT2000B_LK_CONVERSAO : VarBasis
        {
            /*" 05 LK-TAM-CAMPO             PIC 9(003)  VALUE ZEROS.*/
            public IntBasis LK_TAM_CAMPO { get; set; } = new IntBasis(new PIC("9", "3", "9(003)"));
            /*" 05 LK-CAMPO-ENTRADA         PIC X(100)  VALUE SPACES.*/
            public StringBasis LK_CAMPO_ENTRADA { get; set; } = new StringBasis(new PIC("X", "100", "X(100)"), @"");
            /*" 05 LK-CAMPO-SAIDA           PIC X(100)  VALUE SPACES.*/
            public StringBasis LK_CAMPO_SAIDA { get; set; } = new StringBasis(new PIC("X", "100", "X(100)"), @"");
            /*" 05 LK-CONVER-MAIUSCULA      PIC X(001)  VALUE SPACES.*/
            public StringBasis LK_CONVER_MAIUSCULA { get; set; } = new StringBasis(new PIC("X", "1", "X(001)"), @"");
            /*" 05 WABEND.*/
            public LT2000B_WABEND WABEND { get; set; } = new LT2000B_WABEND();
            public class LT2000B_WABEND : VarBasis
            {
                /*"  10 FILLER              PIC  X(008) VALUE           'LT2000B '.*/
                public StringBasis FILLER_227 { get; set; } = new StringBasis(new PIC("X", "8", "X(008)"), @"LT2000B ");
                /*"  10 FILLER              PIC  X(025) VALUE           '*** ERRO EXEC SQL NUMERO '.*/
                public StringBasis FILLER_228 { get; set; } = new StringBasis(new PIC("X", "25", "X(025)"), @"*** ERRO EXEC SQL NUMERO ");
                /*"  10 WNR-EXEC-SQL        PIC  X(004) VALUE '0000'.*/
                public StringBasis WNR_EXEC_SQL { get; set; } = new StringBasis(new PIC("X", "4", "X(004)"), @"0000");
                /*"  10 FILLER              PIC  X(013) VALUE           ' *** SQLCODE '.*/
                public StringBasis FILLER_229 { get; set; } = new StringBasis(new PIC("X", "13", "X(013)"), @" *** SQLCODE ");
                /*"  10 WSQLCODE            PIC  ZZZZZ999- VALUE ZEROS.*/
                public IntBasis WSQLCODE { get; set; } = new IntBasis(new PIC("9", "9", "ZZZZZ999-"));
                /*"01 LK-AREA-LINK2.*/
            }
        }
        public LT2000B_LK_AREA_LINK2 LK_AREA_LINK2 { get; set; } = new LT2000B_LK_AREA_LINK2();
        public class LT2000B_LK_AREA_LINK2 : VarBasis
        {
            /*" 05 LK-DATA-ATUAL     PIC S9(08)      VALUE +0 COMP.*/
            public IntBasis LK_DATA_ATUAL { get; set; } = new IntBasis(new PIC("S9", "8", "S9(08)"));
            /*" 05 LK-NUM-APOLICE    PIC S9(13)      VALUE +0 COMP-3.*/
            public IntBasis LK_NUM_APOLICE { get; set; } = new IntBasis(new PIC("S9", "13", "S9(13)"));
            /*" 05 LK-COD-RETORNO    PIC S9(04)      VALUE +0 COMP.*/
            public IntBasis LK_COD_RETORNO { get; set; } = new IntBasis(new PIC("S9", "4", "S9(04)"));
        }


        public Dclgens.FCSEQUEN FCSEQUEN { get; set; } = new Dclgens.FCSEQUEN();
        public Dclgens.FCLOTERI FCLOTERI { get; set; } = new Dclgens.FCLOTERI();
        public Dclgens.FCPENLOT FCPENLOT { get; set; } = new Dclgens.FCPENLOT();
        public Dclgens.FCTPENLT FCTPENLT { get; set; } = new Dclgens.FCTPENLT();
        public Dclgens.FCCONBAN FCCONBAN { get; set; } = new Dclgens.FCCONBAN();
        public Dclgens.FCHISLOT FCHISLOT { get; set; } = new Dclgens.FCHISLOT();
        public Dclgens.LTLOTBON LTLOTBON { get; set; } = new Dclgens.LTLOTBON();
        public Dclgens.LTMVPROP LTMVPROP { get; set; } = new Dclgens.LTMVPROP();
        public Dclgens.LTMVPRBO LTMVPRBO { get; set; } = new Dclgens.LTMVPRBO();
        public Dclgens.LTMVPRCO LTMVPRCO { get; set; } = new Dclgens.LTMVPRCO();
        public Dclgens.LTSOLPAR LTSOLPAR { get; set; } = new Dclgens.LTSOLPAR();
        public dynamic Result { get; set; }

        #endregion

        [StopWatch]
        /*" Execute */
        public dynamic Execute(string CADASTRO_FILE_NAME_P, string RLT2000B_FILE_NAME_P) //PROCEDURE DIVISION 
        /**/
        {
            try
            {
                CADASTRO.SetFile(CADASTRO_FILE_NAME_P);
                RLT2000B.SetFile(RLT2000B_FILE_NAME_P);

                /*" -1135- EXEC SQL WHENEVER SQLWARNING CONTINUE END-EXEC. */

                /*" -1137- EXEC SQL WHENEVER SQLERROR CONTINUE END-EXEC. */

                /*" -1139- EXEC SQL WHENEVER NOT FOUND CONTINUE END-EXEC. */

                /*" -1143- PERFORM R0100-SELECT-V1SISTEMA */

                R0100_SELECT_V1SISTEMA_SECTION();

                /*" -1145- PERFORM R0110-SELECT-APOLICE */

                R0110_SELECT_APOLICE_SECTION();

                /*" -1147- PERFORM R9000-OPEN-ARQUIVOS. */

                R9000_OPEN_ARQUIVOS_SECTION();

                /*" -1149- PERFORM R7510-MONTA-CABECALHO. */

                R7510_MONTA_CABECALHO_SECTION();

                /*" -1151- PERFORM R0900-LE-CADASTRO. */

                R0900_LE_CADASTRO_SECTION();

                /*" -1152- IF WFIM-CADASTRO NOT EQUAL SPACES */

                if (!AREA_DE_WORK.WFIM_CADASTRO.IsEmpty())
                {

                    /*" -1153- DISPLAY 'NENHUM REGISTRO ENCONTRADO NO ARQUIVO SIGEL ' */
                    _.Display($"NENHUM REGISTRO ENCONTRADO NO ARQUIVO SIGEL ");

                    /*" -1157- GO TO R0000-90-FINALIZA. */

                    R0000_90_FINALIZA(); //GOTO
                    return Result;
                }


                /*" -1162- PERFORM R1000-PROCESSA-CADASTRO UNTIL WFIM-CADASTRO NOT EQUAL SPACES. */

                while (!(!AREA_DE_WORK.WFIM_CADASTRO.IsEmpty()))
                {

                    R1000_PROCESSA_CADASTRO_SECTION();
                }

                /*" -1162- EXEC SQL COMMIT WORK END-EXEC. */

                DatabaseConnection.Instance.CommitTransaction();

            }
            catch (GoBack ex)
            {
            }

            if (!IsCall) DatabaseConnection.Instance.EndTransaction();

            Result = new { };
            return Result;
        }

        [StopWatch]
        /*" R0000-90-FINALIZA */
        private void R0000_90_FINALIZA(bool isPerform = false)
        {
            /*" -1167- ADD 99 TO W-AC-LINHA. */
            AREA_DE_WORK.W_AC_LINHA.Value = AREA_DE_WORK.W_AC_LINHA + 99;

            /*" -1169- PERFORM R7520-CABECALHO. */

            R7520_CABECALHO_SECTION();

            /*" -1170- MOVE 'TOTAL DE REGISTROS LIDOS            ' TO LT00-TEXTO. */
            _.Move("TOTAL DE REGISTROS LIDOS            ", AREA_DE_WORK.LT00.LT00_TEXTO);

            /*" -1171- MOVE W-AC-CAD-LIDOS TO LT00-TOTAIS */
            _.Move(AREA_DE_WORK.W_AC_CAD_LIDOS, AREA_DE_WORK.LT00.LT00_TOTAIS);

            /*" -1173- WRITE REG-RLT2000B FROM LT00 AFTER 2. */
            _.Move(AREA_DE_WORK.LT00.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -1174- MOVE 'TOTAL DE REGISTROS GRAVADOS   ' TO LT00-TEXTO. */
            _.Move("TOTAL DE REGISTROS GRAVADOS   ", AREA_DE_WORK.LT00.LT00_TEXTO);

            /*" -1175- MOVE W-AC-LOTERICOS-GRAVADOS TO LT00-TOTAIS */
            _.Move(AREA_DE_WORK.W_AC_LOTERICOS_GRAVADOS, AREA_DE_WORK.LT00.LT00_TOTAIS);

            /*" -1177- WRITE REG-RLT2000B FROM LT00 AFTER 2. */
            _.Move(AREA_DE_WORK.LT00.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -1178- MOVE 'TOTAL DE REGISTROS REJEITADOS' TO LT00-TEXTO. */
            _.Move("TOTAL DE REGISTROS REJEITADOS", AREA_DE_WORK.LT00.LT00_TEXTO);

            /*" -1179- MOVE W-AC-LOTERICOS-REJEITADOS TO LT00-TOTAIS */
            _.Move(AREA_DE_WORK.W_AC_LOTERICOS_REJEITADOS, AREA_DE_WORK.LT00.LT00_TOTAIS);

            /*" -1183- WRITE REG-RLT2000B FROM LT00 AFTER 2. */
            _.Move(AREA_DE_WORK.LT00.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -1184- MOVE 'TOTAL DE REGISTROS BLACKLIST ' TO LT00-TEXTO. */
            _.Move("TOTAL DE REGISTROS BLACKLIST ", AREA_DE_WORK.LT00.LT00_TEXTO);

            /*" -1185- MOVE W-AC-LOTERICOS-BLACKLIST TO LT00-TOTAIS */
            _.Move(AREA_DE_WORK.W_AC_LOTERICOS_BLACKLIST, AREA_DE_WORK.LT00.LT00_TOTAIS);

            /*" -1188- WRITE REG-RLT2000B FROM LT00 AFTER 2. */
            _.Move(AREA_DE_WORK.LT00.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -1190- MOVE '  TOTAL DE REGISTROS INCLUIDOS NA FC-LOTERICO ..=' TO LT00-TEXTO. */
            _.Move("  TOTAL DE REGISTROS INCLUIDOS NA FC-LOTERICO ..=", AREA_DE_WORK.LT00.LT00_TEXTO);

            /*" -1191- MOVE WS-FCLOT-TOTAL-INCLUIDOS TO LT00-TOTAIS */
            _.Move(AREA_DE_WORK.WS_FCLOT_TOTAL_INCLUIDOS, AREA_DE_WORK.LT00.LT00_TOTAIS);

            /*" -1193- WRITE REG-RLT2000B FROM LT00 AFTER 2. */
            _.Move(AREA_DE_WORK.LT00.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -1195- MOVE '  TOTAL DE REGISTROS ALTERADOS NA FC-LOTERICO ..=' TO LT00-TEXTO. */
            _.Move("  TOTAL DE REGISTROS ALTERADOS NA FC-LOTERICO ..=", AREA_DE_WORK.LT00.LT00_TEXTO);

            /*" -1196- MOVE WS-FCLOT-TOTAL-ALTERADOS TO LT00-TOTAIS. */
            _.Move(AREA_DE_WORK.WS_FCLOT_TOTAL_ALTERADOS, AREA_DE_WORK.LT00.LT00_TOTAIS);

            /*" -1198- WRITE REG-RLT2000B FROM LT00 AFTER 2. */
            _.Move(AREA_DE_WORK.LT00.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -1200- MOVE '  TOTAL DE LOTERICOS JA SEGURADOS ..............=' TO LT00-TEXTO. */
            _.Move("  TOTAL DE LOTERICOS JA SEGURADOS ..............=", AREA_DE_WORK.LT00.LT00_TEXTO);

            /*" -1201- MOVE WS-MVPROP-TOTAL-SEGURADOS TO LT00-TOTAIS. */
            _.Move(AREA_DE_WORK.WS_MVPROP_TOTAL_SEGURADOS, AREA_DE_WORK.LT00.LT00_TOTAIS);

            /*" -1203- WRITE REG-RLT2000B FROM LT00 AFTER 2. */
            _.Move(AREA_DE_WORK.LT00.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -1205- MOVE '  TOTAL DE LOTERICOS NAO SEGURADOS .............=' TO LT00-TEXTO. */
            _.Move("  TOTAL DE LOTERICOS NAO SEGURADOS .............=", AREA_DE_WORK.LT00.LT00_TEXTO);

            /*" -1206- MOVE WS-MVPROP-TOTAL-NSEGURADOS TO LT00-TOTAIS. */
            _.Move(AREA_DE_WORK.WS_MVPROP_TOTAL_NSEGURADOS, AREA_DE_WORK.LT00.LT00_TOTAIS);

            /*" -1209- WRITE REG-RLT2000B FROM LT00 AFTER 2. */
            _.Move(AREA_DE_WORK.LT00.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -1211- MOVE '  TOTAL DE LOTERICOS ATIVOS NO SIGEL............=' TO LT00-TEXTO. */
            _.Move("  TOTAL DE LOTERICOS ATIVOS NO SIGEL............=", AREA_DE_WORK.LT00.LT00_TEXTO);

            /*" -1212- MOVE W-AC-ATIVOS TO LT00-TOTAIS. */
            _.Move(AREA_DE_WORK.W_AC_ATIVOS, AREA_DE_WORK.LT00.LT00_TOTAIS);

            /*" -1214- WRITE REG-RLT2000B FROM LT00 AFTER 2. */
            _.Move(AREA_DE_WORK.LT00.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -1216- MOVE '  TOTAL DE LOTERICOS INATIVOS NO SIGEL..........=' TO LT00-TEXTO. */
            _.Move("  TOTAL DE LOTERICOS INATIVOS NO SIGEL..........=", AREA_DE_WORK.LT00.LT00_TEXTO);

            /*" -1217- MOVE W-AC-INATIVOS TO LT00-TOTAIS. */
            _.Move(AREA_DE_WORK.W_AC_INATIVOS, AREA_DE_WORK.LT00.LT00_TOTAIS);

            /*" -1219- WRITE REG-RLT2000B FROM LT00 AFTER 2. */
            _.Move(AREA_DE_WORK.LT00.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -1221- MOVE '  TOTAL DE LOTERICOS EM PRE-CADASTRO NO SIGEL...=' TO LT00-TEXTO. */
            _.Move("  TOTAL DE LOTERICOS EM PRE-CADASTRO NO SIGEL...=", AREA_DE_WORK.LT00.LT00_TEXTO);

            /*" -1222- MOVE W-AC-PRECAD TO LT00-TOTAIS. */
            _.Move(AREA_DE_WORK.W_AC_PRECAD, AREA_DE_WORK.LT00.LT00_TOTAIS);

            /*" -1224- WRITE REG-RLT2000B FROM LT00 AFTER 2. */
            _.Move(AREA_DE_WORK.LT00.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -1226- MOVE '  TOTAL DE LOTERICOS DA SEGURADORA 001(CS)......=' TO LT00-TEXTO. */
            _.Move("  TOTAL DE LOTERICOS DA SEGURADORA 001(CS)......=", AREA_DE_WORK.LT00.LT00_TEXTO);

            /*" -1227- MOVE W-AC-SEGURADORA-CS TO LT00-TOTAIS. */
            _.Move(AREA_DE_WORK.W_AC_SEGURADORA_CS, AREA_DE_WORK.LT00.LT00_TOTAIS);

            /*" -1229- WRITE REG-RLT2000B FROM LT00 AFTER 2. */
            _.Move(AREA_DE_WORK.LT00.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -1231- MOVE '  TOTAL DE LOTERICOS DE OUTRAS SEGURADOARAS.....=' TO LT00-TEXTO. */
            _.Move("  TOTAL DE LOTERICOS DE OUTRAS SEGURADOARAS.....=", AREA_DE_WORK.LT00.LT00_TEXTO);

            /*" -1232- MOVE W-AC-SEGURADORA-OUTRAS TO LT00-TOTAIS. */
            _.Move(AREA_DE_WORK.W_AC_SEGURADORA_OUTRAS, AREA_DE_WORK.LT00.LT00_TOTAIS);

            /*" -1234- WRITE REG-RLT2000B FROM LT00 AFTER 2. */
            _.Move(AREA_DE_WORK.LT00.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -1236- MOVE '  TOTAL DE LOTERICOS COM GARANTIA = S (SEGURO)..=' TO LT00-TEXTO. */
            _.Move("  TOTAL DE LOTERICOS COM GARANTIA = S (SEGURO)..=", AREA_DE_WORK.LT00.LT00_TEXTO);

            /*" -1237- MOVE W-AC-GARANTIA-S TO LT00-TOTAIS. */
            _.Move(AREA_DE_WORK.W_AC_GARANTIA_S, AREA_DE_WORK.LT00.LT00_TOTAIS);

            /*" -1239- WRITE REG-RLT2000B FROM LT00 AFTER 2. */
            _.Move(AREA_DE_WORK.LT00.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -1241- MOVE '  TOTAL DE LOTERICOS COM GARANTIA = C (CAUCAO)..=' TO LT00-TEXTO. */
            _.Move("  TOTAL DE LOTERICOS COM GARANTIA = C (CAUCAO)..=", AREA_DE_WORK.LT00.LT00_TEXTO);

            /*" -1242- MOVE W-AC-GARANTIA-C TO LT00-TOTAIS. */
            _.Move(AREA_DE_WORK.W_AC_GARANTIA_C, AREA_DE_WORK.LT00.LT00_TOTAIS);

            /*" -1245- WRITE REG-RLT2000B FROM LT00 AFTER 2. */
            _.Move(AREA_DE_WORK.LT00.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -1247- MOVE '  TOTAL DE LOTERICOS - INCLUSOES NO MOVIMENTO   =' TO LT00-TEXTO. */
            _.Move("  TOTAL DE LOTERICOS - INCLUSOES NO MOVIMENTO   =", AREA_DE_WORK.LT00.LT00_TEXTO);

            /*" -1248- MOVE WS-MVPROP-TOTAL-INCLUIDOS TO LT00-TOTAIS. */
            _.Move(AREA_DE_WORK.WS_MVPROP_TOTAL_INCLUIDOS, AREA_DE_WORK.LT00.LT00_TOTAIS);

            /*" -1250- WRITE REG-RLT2000B FROM LT00 AFTER 2. */
            _.Move(AREA_DE_WORK.LT00.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -1252- MOVE '  TOTAL DE LOTERICOS - MOV. DE ALTERACOES    - 3=' TO LT00-TEXTO. */
            _.Move("  TOTAL DE LOTERICOS - MOV. DE ALTERACOES    - 3=", AREA_DE_WORK.LT00.LT00_TEXTO);

            /*" -1253- MOVE WS-MVPROP-TOTAL-ALTERADOS TO LT00-TOTAIS. */
            _.Move(AREA_DE_WORK.WS_MVPROP_TOTAL_ALTERADOS, AREA_DE_WORK.LT00.LT00_TOTAIS);

            /*" -1255- WRITE REG-RLT2000B FROM LT00 AFTER 2. */
            _.Move(AREA_DE_WORK.LT00.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -1257- MOVE '  TOTAL DE LOTERICOS - MOV DE CANCELAMENTOS - 7 =' TO LT00-TEXTO. */
            _.Move("  TOTAL DE LOTERICOS - MOV DE CANCELAMENTOS - 7 =", AREA_DE_WORK.LT00.LT00_TEXTO);

            /*" -1258- MOVE WS-MVPROP-TOTAL-CANCELADOS TO LT00-TOTAIS. */
            _.Move(AREA_DE_WORK.WS_MVPROP_TOTAL_CANCELADOS, AREA_DE_WORK.LT00.LT00_TOTAIS);

            /*" -1261- WRITE REG-RLT2000B FROM LT00 AFTER 2. */
            _.Move(AREA_DE_WORK.LT00.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -1264- PERFORM R9100-CLOSE-ARQUIVOS. */

            R9100_CLOSE_ARQUIVOS_SECTION();

            /*" -1265- DISPLAY '              PROGRAMA - LT2000B                  ' */
            _.Display($"              PROGRAMA - LT2000B                  ");

            /*" -1266- DISPLAY '                                                  ' */
            _.Display($"                                                  ");

            /*" -1267- DISPLAY '                TERMINO NORMAL                    ' */
            _.Display($"                TERMINO NORMAL                    ");

            /*" -1268- DISPLAY '                                                  ' */
            _.Display($"                                                  ");

            /*" -1270- DISPLAY '  TOTAL DE REGISTROS LIDOS NO ARQ SIGEL ....... =' W-AC-CAD-LIDOS. */
            _.Display($"  TOTAL DE REGISTROS LIDOS NO ARQ SIGEL ....... ={AREA_DE_WORK.W_AC_CAD_LIDOS}");

            /*" -1272- DISPLAY '  TOTAL DE REGISTROS REJEITADOS ............... =' W-AC-LOTERICOS-REJEITADOS. */
            _.Display($"  TOTAL DE REGISTROS REJEITADOS ............... ={AREA_DE_WORK.W_AC_LOTERICOS_REJEITADOS}");

            /*" -1274- DISPLAY '  TOTAL DE REGISTROS GRAVADOS ................. =' W-AC-LOTERICOS-GRAVADOS. */
            _.Display($"  TOTAL DE REGISTROS GRAVADOS ................. ={AREA_DE_WORK.W_AC_LOTERICOS_GRAVADOS}");

            /*" -1275- DISPLAY '                  ' . */
            _.Display($"                  ");

            /*" -1276- DISPLAY '                  ' . */
            _.Display($"                  ");

            /*" -1278- DISPLAY '  TOTAL DE REGISTROS INCLUIDOS NA FC-LOTERICO ..=' WS-FCLOT-TOTAL-INCLUIDOS. */
            _.Display($"  TOTAL DE REGISTROS INCLUIDOS NA FC-LOTERICO ..={AREA_DE_WORK.WS_FCLOT_TOTAL_INCLUIDOS}");

            /*" -1280- DISPLAY '  TOTAL DE REGISTROS ALTERADOS NA FC-LOTERICO ..=' WS-FCLOT-TOTAL-ALTERADOS. */
            _.Display($"  TOTAL DE REGISTROS ALTERADOS NA FC-LOTERICO ..={AREA_DE_WORK.WS_FCLOT_TOTAL_ALTERADOS}");

            /*" -1281- DISPLAY '             ' . */
            _.Display($"             ");

            /*" -1282- DISPLAY '             ' . */
            _.Display($"             ");

            /*" -1284- DISPLAY '  TOTAL DE LOTERICOS JA SEGURADOS ..............=' WS-MVPROP-TOTAL-SEGURADOS. */
            _.Display($"  TOTAL DE LOTERICOS JA SEGURADOS ..............={AREA_DE_WORK.WS_MVPROP_TOTAL_SEGURADOS}");

            /*" -1286- DISPLAY '  TOTAL DE LOTERICOS NAO SEGURADOS .............=' WS-MVPROP-TOTAL-NSEGURADOS. */
            _.Display($"  TOTAL DE LOTERICOS NAO SEGURADOS .............={AREA_DE_WORK.WS_MVPROP_TOTAL_NSEGURADOS}");

            /*" -1288- DISPLAY '  TOTAL DE LOTERICOS - INCLUSOES NO MOVIMENTO   =' WS-MVPROP-TOTAL-INCLUIDOS. */
            _.Display($"  TOTAL DE LOTERICOS - INCLUSOES NO MOVIMENTO   ={AREA_DE_WORK.WS_MVPROP_TOTAL_INCLUIDOS}");

            /*" -1290- DISPLAY '  TOTAL DE LOTERICOS - MOV DE ALTERACOES    - 3 =' WS-MVPROP-TOTAL-ALTERADOS. */
            _.Display($"  TOTAL DE LOTERICOS - MOV DE ALTERACOES    - 3 ={AREA_DE_WORK.WS_MVPROP_TOTAL_ALTERADOS}");

            /*" -1292- DISPLAY '  TOTAL DE LOTERICOS - MOV DE CANCELAMENTOS - 7 =' WS-MVPROP-TOTAL-CANCELADOS. */
            _.Display($"  TOTAL DE LOTERICOS - MOV DE CANCELAMENTOS - 7 ={AREA_DE_WORK.WS_MVPROP_TOTAL_CANCELADOS}");

            /*" -1295- DISPLAY '  TOTAL DE DELECOES NA FC_PEND_LOTERICOS .......=' WS-FCPEND-DELETADOS. */
            _.Display($"  TOTAL DE DELECOES NA FC_PEND_LOTERICOS .......={AREA_DE_WORK.WS_FCPEND_DELETADOS}");

            /*" -1296- DISPLAY '                                                  ' . */
            _.Display($"                                                  ");

            /*" -1298- DISPLAY '**************************************************' . */
            _.Display($"**************************************************");

            /*" -1300- MOVE ZEROS TO RETURN-CODE */
            _.Move(0, RETURN_CODE);

            /*" -1300- STOP RUN. */

            throw new GoBack();   // => STOP RUN.

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R0000_SAIDA*/

        [StopWatch]
        /*" R0100-SELECT-V1SISTEMA-SECTION */
        private void R0100_SELECT_V1SISTEMA_SECTION()
        {
            /*" -1309- MOVE '0001' TO WNR-EXEC-SQL. */
            _.Move("0001", LK_CONVERSAO.WABEND.WNR_EXEC_SQL);

            /*" -1314- PERFORM R0100_SELECT_V1SISTEMA_DB_SELECT_1 */

            R0100_SELECT_V1SISTEMA_DB_SELECT_1();

            /*" -1318- IF (SQLCODE NOT EQUAL ZEROS) AND (SQLCODE NOT EQUAL 100) */

            if ((DB.SQLCODE != 00) && (DB.SQLCODE != 100))
            {

                /*" -1319- DISPLAY 'PROBLEMA SELECT V1SISTEMA' */
                _.Display($"PROBLEMA SELECT V1SISTEMA");

                /*" -1321- GO TO R9999-ROT-ERRO. */

                R9999_ROT_ERRO_SECTION(); //GOTO
                return;
            }


            /*" -1322- MOVE V0SIST-DTMOVABE TO WDATA-MOVABE. */
            _.Move(V0SIST_DTMOVABE, AREA_DE_WORK.WDATA_MOVABE);

            /*" -1323- MOVE WDATA-AA-MOVABE TO WS-AA-ANT. */
            _.Move(AREA_DE_WORK.WDATA_MOVABE.WDATA_AA_MOVABE, AREA_DE_WORK.WS_DT_ANT.WS_AA_ANT);

            /*" -1324- MOVE WDATA-MM-MOVABE TO WS-MM-ANT. */
            _.Move(AREA_DE_WORK.WDATA_MOVABE.WDATA_MM_MOVABE, AREA_DE_WORK.WS_DT_ANT.WS_MM_ANT);

            /*" -1325- MOVE 01 TO WS-DD-ANT. */
            _.Move(01, AREA_DE_WORK.WS_DT_ANT.WS_DD_ANT);

            /*" -1326- MOVE WDATA-AA-MOVABE TO WS-AA-POS. */
            _.Move(AREA_DE_WORK.WDATA_MOVABE.WDATA_AA_MOVABE, AREA_DE_WORK.WS_DT_POS.WS_AA_POS);

            /*" -1327- MOVE WDATA-MM-MOVABE TO WS-MM-POS. */
            _.Move(AREA_DE_WORK.WDATA_MOVABE.WDATA_MM_MOVABE, AREA_DE_WORK.WS_DT_POS.WS_MM_POS);

            /*" -1329- MOVE WDATA-DD-MOVABE TO WS-DD-POS. */
            _.Move(AREA_DE_WORK.WDATA_MOVABE.WDATA_DD_MOVABE, AREA_DE_WORK.WS_DT_POS.WS_DD_POS);

            /*" -1329- DISPLAY 'LT2000B - DTMOVABE :' V0SIST-DTMOVABE. */
            _.Display($"LT2000B - DTMOVABE :{V0SIST_DTMOVABE}");

        }

        [StopWatch]
        /*" R0100-SELECT-V1SISTEMA-DB-SELECT-1 */
        public void R0100_SELECT_V1SISTEMA_DB_SELECT_1()
        {
            /*" -1314- EXEC SQL SELECT DTMOVABE INTO :V0SIST-DTMOVABE FROM SEGUROS.V0SISTEMA WHERE IDSISTEM = 'LT' END-EXEC. */

            var r0100_SELECT_V1SISTEMA_DB_SELECT_1_Query1 = new R0100_SELECT_V1SISTEMA_DB_SELECT_1_Query1()
            {
            };

            var executed_1 = R0100_SELECT_V1SISTEMA_DB_SELECT_1_Query1.Execute(r0100_SELECT_V1SISTEMA_DB_SELECT_1_Query1);
            if (executed_1 != null)
            {
                _.Move(executed_1.V0SIST_DTMOVABE, V0SIST_DTMOVABE);
            }


        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R0100_SAIDA*/

        [StopWatch]
        /*" R0110-SELECT-APOLICE-SECTION */
        private void R0110_SELECT_APOLICE_SECTION()
        {
            /*" -1338- MOVE '0110' TO WNR-EXEC-SQL. */
            _.Move("0110", LK_CONVERSAO.WABEND.WNR_EXEC_SQL);

            /*" -1339- MOVE 0 TO LK-DATA-ATUAL. */
            _.Move(0, LK_AREA_LINK2.LK_DATA_ATUAL);

            /*" -1341- CALL 'LT2100S' USING LK-AREA-LINK2. */
            _.Call("LT2100S", LK_AREA_LINK2);

            /*" -1342- MOVE LK-NUM-APOLICE TO WS-NUM-APOLICE LTSOLPAR-PARAM-DEC01. */
            _.Move(LK_AREA_LINK2.LK_NUM_APOLICE, WS_NUM_APOLICE, LTSOLPAR.DCLLT_SOLICITA_PARAM.LTSOLPAR_PARAM_DEC01);

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R0110_SAIDA*/

        [StopWatch]
        /*" R0900-LE-CADASTRO-SECTION */
        private void R0900_LE_CADASTRO_SECTION()
        {
            /*" -1353- READ CADASTRO AT END */
            try
            {
                CADASTRO.Read(() =>
                {

                    /*" -1355- MOVE 'SIM' TO WFIM-CADASTRO */
                    _.Move("SIM", AREA_DE_WORK.WFIM_CADASTRO);

                    /*" -1357- GO TO R0900-SAIDA. */
                    /*Método Suprimido por falta de linha ou apenas EXIT nome: R0900_SAIDA*/ //GOTO
                    return;
                });


            }
            catch (GoToException ex)
            {
                return;
            }
            /*" -1359- ADD 1 TO W-AC-CAD-LIDOS. */
            AREA_DE_WORK.W_AC_CAD_LIDOS.Value = AREA_DE_WORK.W_AC_CAD_LIDOS + 1;

            /*" -1361- MOVE REG-CAD TO REG-CAD-CADASTRO. */
            _.Move(CADASTRO?.Value, AREA_DE_WORK.REG_CAD_CADASTRO);

            /*" -1362- IF CAD-TIPO = 'H' */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TIPOX.CAD_TIPO == "H")
            {

                /*" -1363- MOVE REG-CAD TO REG-CAD-HEADER */
                _.Move(CADASTRO?.Value, AREA_DE_WORK.REG_CAD_HEADER);

                /*" -1364- PERFORM R0950-TRATA-HEADER */

                R0950_TRATA_HEADER_SECTION();

                /*" -1366- GO TO R0900-LE-CADASTRO. */
                new Task(() => R0900_LE_CADASTRO_SECTION()).RunSynchronously(); //GOTO
                return;//Recursividade detectada, cuidado...
            }


            /*" -1367- IF CAD-TIPO = 'T' */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TIPOX.CAD_TIPO == "T")
            {

                /*" -1368- MOVE REG-CAD TO REG-CAD-TRAILLER */
                _.Move(CADASTRO?.Value, AREA_DE_WORK.REG_CAD_TRAILLER);

                /*" -1376- GO TO R0900-LE-CADASTRO. */
                new Task(() => R0900_LE_CADASTRO_SECTION()).RunSynchronously(); //GOTO
                return;//Recursividade detectada, cuidado...
            }


            /*" -1376- . */

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R0900_SAIDA*/

        [StopWatch]
        /*" R0950-TRATA-HEADER-SECTION */
        private void R0950_TRATA_HEADER_SECTION()
        {
            /*" -1384- MOVE SPACES TO W-CAD-DATA-GERACAO */
            _.Move("", AREA_DE_WORK.W_CAD_DATA_GERACAO);

            /*" -1385- IF CAD-DATA-GERACAO IS NUMERIC */

            if (AREA_DE_WORK.REG_CAD_HEADER.CAD_DATA_GERACAOX.CAD_DATA_GERACAO.IsNumeric())
            {

                /*" -1387- MOVE CAD-DATA-GERACAO TO W-DATA-AAAAMMDD WS-DATA-SEC */
                _.Move(AREA_DE_WORK.REG_CAD_HEADER.CAD_DATA_GERACAOX.CAD_DATA_GERACAO, AREA_DE_WORK.W_DATA_AAAAMMDD, AREA_DE_WORK.WS_DATA_SEC);

                /*" -1388- MOVE W-DATA-AAAAMMDD-DD TO W-DATA-AAAA-MM-DD-DD */
                _.Move(AREA_DE_WORK.W_DATA_AAAAMMDD.W_DATA_AAAAMMDD_DD, AREA_DE_WORK.W_DATA_AAAA_MM_DD.W_DATA_AAAA_MM_DD_DD);

                /*" -1389- MOVE W-DATA-AAAAMMDD-MM TO W-DATA-AAAA-MM-DD-MM */
                _.Move(AREA_DE_WORK.W_DATA_AAAAMMDD.W_DATA_AAAAMMDD_MM, AREA_DE_WORK.W_DATA_AAAA_MM_DD.W_DATA_AAAA_MM_DD_MM);

                /*" -1390- MOVE W-DATA-AAAAMMDD-AAAA TO W-DATA-AAAA-MM-DD-AAAA */
                _.Move(AREA_DE_WORK.W_DATA_AAAAMMDD.W_DATA_AAAAMMDD_AAAA, AREA_DE_WORK.W_DATA_AAAA_MM_DD.W_DATA_AAAA_MM_DD_AAAA);

                /*" -1391- MOVE W-DATA-AAAA-MM-DD TO W-CAD-DATA-GERACAO */
                _.Move(AREA_DE_WORK.W_DATA_AAAA_MM_DD, AREA_DE_WORK.W_CAD_DATA_GERACAO);

                /*" -1392- PERFORM R1055-CRITICA-DATA */

                R1055_CRITICA_DATA_SECTION();

                /*" -1393- ELSE */
            }
            else
            {


                /*" -1395- MOVE 1 TO WS-ERRO-DATA. */
                _.Move(1, AREA_DE_WORK.WS_ERRO_DATA);
            }


            /*" -1396- IF WS-ERRO-DATA EQUAL 1 */

            if (AREA_DE_WORK.WS_ERRO_DATA == 1)
            {

                /*" -1397- DISPLAY 'R0950-ERRO DATA GERACAO =' CAD-DATA-GERACAO */
                _.Display($"R0950-ERRO DATA GERACAO ={AREA_DE_WORK.REG_CAD_HEADER.CAD_DATA_GERACAOX.CAD_DATA_GERACAO}");

                /*" -1397- GO TO R9999-ROT-ERRO. */

                R9999_ROT_ERRO_SECTION(); //GOTO
                return;
            }


        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R0950_SAIDA*/

        [StopWatch]
        /*" R1000-PROCESSA-CADASTRO-SECTION */
        private void R1000_PROCESSA_CADASTRO_SECTION()
        {
            /*" -1406- PERFORM R7650-CONVERTE-CARACTER. */

            R7650_CONVERTE_CARACTER_SECTION();

            /*" -1408- PERFORM R1050-CRITICA-CADASTRO. */

            R1050_CRITICA_CADASTRO_SECTION();

            /*" -1409- IF WS-OBRIGATORIO = 1 */

            if (WS_OBRIGATORIO == 1)
            {

                /*" -1410- ADD 1 TO W-AC-LOTERICOS-REJEITADOS */
                AREA_DE_WORK.W_AC_LOTERICOS_REJEITADOS.Value = AREA_DE_WORK.W_AC_LOTERICOS_REJEITADOS + 1;

                /*" -1412- GO TO R1000-LER-CADASTRO. */

                R1000_LER_CADASTRO(); //GOTO
                return;
            }


            /*" -1413- MOVE SPACES TO LD00-MSG1 */
            _.Move("", AREA_DE_WORK.LD00.LD00_MSG1);

            /*" -1415- MOVE 'LT2000B' TO LTMVPROP-COD-USUARIO. */
            _.Move("LT2000B", LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_USUARIO);

            /*" -1417- PERFORM R6020-SELECT-FC-LOTERICO. */

            R6020_SELECT_FC_LOTERICO_SECTION();

            /*" -1419- PERFORM R6030-SELECT-V0LOTERICO01. */

            R6030_SELECT_V0LOTERICO01_SECTION();

            /*" -1420- IF W-CHAVE-CADASTRADO-SIGEL EQUAL 'NAO' */

            if (W_CHAVE_CADASTRADO_SIGEL == "NAO")
            {

                /*" -1421- PERFORM R6220-GRAVAR-FC-CONTA */

                R6220_GRAVAR_FC_CONTA_SECTION();

                /*" -1422- PERFORM R6200-MONTAR-FC-LOTERICO */

                R6200_MONTAR_FC_LOTERICO_SECTION();

                /*" -1423- PERFORM R6210-INSERT-FC-LOTERICO */

                R6210_INSERT_FC_LOTERICO_SECTION();

                /*" -1424- GO TO R1000-LER-CADASTRO */

                R1000_LER_CADASTRO(); //GOTO
                return;

                /*" -1426- END-IF */
            }


            /*" -1427- PERFORM R6000-VER-ALTERACAO-LOTERICO. */

            R6000_VER_ALTERACAO_LOTERICO_SECTION();

            /*" -1428- PERFORM R6060-VER-ALTERACAO-FC-CONTA. */

            R6060_VER_ALTERACAO_FC_CONTA_SECTION();

            /*" -1430- PERFORM R6850-VER-ALTERACAO-BONUS. */

            R6850_VER_ALTERACAO_BONUS_SECTION();

            /*" -1431- IF W-CHAVE-HOUVE-ALTERACAO = 'SIM' */

            if (W_CHAVE_HOUVE_ALTERACAO == "SIM")
            {

                /*" -1432- PERFORM R6200-MONTAR-FC-LOTERICO */

                R6200_MONTAR_FC_LOTERICO_SECTION();

                /*" -1433- PERFORM R6700-UPDATE-FC-LOTERICO */

                R6700_UPDATE_FC_LOTERICO_SECTION();

                /*" -1434- END-IF */
            }


            /*" -1434- . */

            /*" -0- FLUXCONTROL_PERFORM R1000_LER_CADASTRO */

            R1000_LER_CADASTRO();

        }

        [StopWatch]
        /*" R1000-LER-CADASTRO */
        private void R1000_LER_CADASTRO(bool isPerform = false)
        {
            /*" -1438- PERFORM R0900-LE-CADASTRO. */

            R0900_LE_CADASTRO_SECTION();

            /*" -1438- . */

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R1000_SAIDA*/

        [StopWatch]
        /*" R1010-VERIFICA-UF-SECTION */
        private void R1010_VERIFICA_UF_SECTION()
        {
            /*" -1447- IF CAD-UF = TB-UF(WS-IND) */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_UF == AREA_DE_WORK.TAB_UF_R[WS_IND].TB_UF)
            {

                /*" -1447- MOVE 1 TO WS-TEM-UF. */
                _.Move(1, WS_TEM_UF);
            }


        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R1010_SAIDA*/

        [StopWatch]
        /*" R1050-CRITICA-CADASTRO-SECTION */
        private void R1050_CRITICA_CADASTRO_SECTION()
        {
            /*" -1454- MOVE SPACES TO LD00-MSG1. */
            _.Move("", AREA_DE_WORK.LD00.LD00_MSG1);

            /*" -1458- MOVE 0 TO WS-OBRIGATORIO WS-NECESSARIO WS-IMPRIMIU. */
            _.Move(0, WS_OBRIGATORIO, WS_NECESSARIO, WS_IMPRIMIU);

            /*" -1459- IF CAD-TIPO NOT EQUAL 'C' */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TIPOX.CAD_TIPO != "C")
            {

                /*" -1460- MOVE 'TIPO DE REGISTRO INVALIDO             ' TO LD00-MSG1 */
                _.Move("TIPO DE REGISTRO INVALIDO             ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1461- MOVE 1 TO WS-OBRIGATORIO */
                _.Move(1, WS_OBRIGATORIO);

                /*" -1462- MOVE SPACES TO CAD-TIPO */
                _.Move("", AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TIPOX.CAD_TIPO);

                /*" -1464- PERFORM R7600-IMPRIME-LD00-MSG1. */

                R7600_IMPRIME_LD00_MSG1_SECTION();
            }


            /*" -1466- IF CAD-COD-CEF NOT NUMERIC OR CAD-COD-CEF EQUAL ZEROS */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF_R.CAD_COD_CEF.IsNumeric() || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF_R.CAD_COD_CEF == 00)
            {

                /*" -1467- MOVE 'CODIGO LOTERICO INVALIDO              ' TO LD00-MSG1 */
                _.Move("CODIGO LOTERICO INVALIDO              ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1468- MOVE 1 TO WS-OBRIGATORIO */
                _.Move(1, WS_OBRIGATORIO);

                /*" -1469- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1471- MOVE 0 TO CAD-COD-CEF. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF_R.CAD_COD_CEF);
            }


            /*" -1472- IF CAD-DV-CEF NOT NUMERIC */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF_R.CAD_DV_CEF.IsNumeric())
            {

                /*" -1473- MOVE 'DIGITO DO LOTERICO INVALIDO         ' TO LD00-MSG1 */
                _.Move("DIGITO DO LOTERICO INVALIDO         ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1474- MOVE 1 TO WS-OBRIGATORIO */
                _.Move(1, WS_OBRIGATORIO);

                /*" -1475- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1477- MOVE 0 TO CAD-COD-CEF. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF_R.CAD_COD_CEF);
            }


            /*" -1478- IF CAD-RAZAO-SOCIAL EQUAL SPACES */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_RAZAO_SOCIAL.IsEmpty())
            {

                /*" -1479- MOVE 'FALTA RAZAO SOCIAL                ' TO LD00-MSG1 */
                _.Move("FALTA RAZAO SOCIAL                ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1480- MOVE 1 TO WS-NECESSARIO */
                _.Move(1, WS_NECESSARIO);

                /*" -1482- PERFORM R7600-IMPRIME-LD00-MSG1. */

                R7600_IMPRIME_LD00_MSG1_SECTION();
            }


            /*" -1483- IF CAD-ENDERECO EQUAL SPACES */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_ENDERECO.IsEmpty())
            {

                /*" -1484- MOVE 'FALTA ENDERECO                    ' TO LD00-MSG1 */
                _.Move("FALTA ENDERECO                    ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1485- MOVE 1 TO WS-NECESSARIO */
                _.Move(1, WS_NECESSARIO);

                /*" -1487- PERFORM R7600-IMPRIME-LD00-MSG1. */

                R7600_IMPRIME_LD00_MSG1_SECTION();
            }


            /*" -1488- IF CAD-BAIRRO EQUAL SPACES */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BAIRRO.IsEmpty())
            {

                /*" -1489- MOVE 'FALTA BAIRRO                      ' TO LD00-MSG1 */
                _.Move("FALTA BAIRRO                      ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1490- MOVE 1 TO WS-NECESSARIO */
                _.Move(1, WS_NECESSARIO);

                /*" -1492- PERFORM R7600-IMPRIME-LD00-MSG1. */

                R7600_IMPRIME_LD00_MSG1_SECTION();
            }


            /*" -1493- IF CAD-COD-MUNICIPIO EQUAL SPACES */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_MUNICIPIO.IsEmpty())
            {

                /*" -1494- MOVE 'FALTA CODIGO DO MUNICIPIO         ' TO LD00-MSG1 */
                _.Move("FALTA CODIGO DO MUNICIPIO         ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1496- PERFORM R7600-IMPRIME-LD00-MSG1. */

                R7600_IMPRIME_LD00_MSG1_SECTION();
            }


            /*" -1497- IF CAD-CIDADE EQUAL SPACES */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CIDADE.IsEmpty())
            {

                /*" -1498- MOVE 'FALTA CIDADE                      ' TO LD00-MSG1 */
                _.Move("FALTA CIDADE                      ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1499- MOVE 1 TO WS-NECESSARIO */
                _.Move(1, WS_NECESSARIO);

                /*" -1501- PERFORM R7600-IMPRIME-LD00-MSG1. */

                R7600_IMPRIME_LD00_MSG1_SECTION();
            }


            /*" -1503- IF CAD-CEP NOT NUMERIC OR CAD-CEP EQUAL ZEROS */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CEPX.CAD_CEP.IsNumeric() || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CEPX.CAD_CEP == 00)
            {

                /*" -1504- MOVE 'CEP INVALIDO                      ' TO LD00-MSG1 */
                _.Move("CEP INVALIDO                      ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1505- MOVE 1 TO WS-NECESSARIO */
                _.Move(1, WS_NECESSARIO);

                /*" -1506- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1508- MOVE 0 TO CAD-CEP. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CEPX.CAD_CEP);
            }


            /*" -1509- IF CAD-UF EQUAL SPACES */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_UF.IsEmpty())
            {

                /*" -1510- MOVE 'FALTA UF                    ' TO LD00-MSG1 */
                _.Move("FALTA UF                    ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1511- MOVE 1 TO WS-NECESSARIO */
                _.Move(1, WS_NECESSARIO);

                /*" -1512- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1513- ELSE */
            }
            else
            {


                /*" -1514- MOVE 0 TO WS-TEM-UF */
                _.Move(0, WS_TEM_UF);

                /*" -1516- PERFORM R1010-VERIFICA-UF VARYING WS-IND FROM 1 BY 1 UNTIL WS-IND > 27 */

                for (WS_IND.Value = 1; !(WS_IND > 27); WS_IND.Value += 1)
                {

                    R1010_VERIFICA_UF_SECTION();
                }

                /*" -1517- IF WS-TEM-UF = 0 */

                if (WS_TEM_UF == 0)
                {

                    /*" -1518- MOVE SPACES TO CAD-UF */
                    _.Move("", AREA_DE_WORK.REG_CAD_CADASTRO.CAD_UF);

                    /*" -1519- MOVE 1 TO WS-NECESSARIO */
                    _.Move(1, WS_NECESSARIO);

                    /*" -1520- MOVE 'UF NAO CADASTRADA ' TO LD00-MSG1 */
                    _.Move("UF NAO CADASTRADA ", AREA_DE_WORK.LD00.LD00_MSG1);

                    /*" -1522- PERFORM R7600-IMPRIME-LD00-MSG1. */

                    R7600_IMPRIME_LD00_MSG1_SECTION();
                }

            }


            /*" -1523- IF CAD-TELEFONE EQUAL SPACES */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TELEFONE.IsEmpty())
            {

                /*" -1524- MOVE ZEROS TO CAD-TELEFONE */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TELEFONE);

                /*" -1525- ELSE */
            }
            else
            {


                /*" -1526- MOVE CAD-TELEFONE TO WS-NUM-TELEF-ENT */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TELEFONE, AREA_DE_WORK.WS_NUM_TELEF_ENT);

                /*" -1527- PERFORM R1100-TRATAR-TELEF-FAX */

                R1100_TRATAR_TELEF_FAX_SECTION();

                /*" -1529- MOVE WS-NUM-TELEF-SAI TO CAD-TELEFONE. */
                _.Move(AREA_DE_WORK.WS_NUM_TELEF_SAI, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TELEFONE);
            }


            /*" -1530- IF CAD-DDD-FONE NOT NUMERIC */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TELEFONE.CAD_DDD_FONE.IsNumeric())
            {

                /*" -1531- MOVE 'DDD INVALIDO       ' TO LD00-MSG1 */
                _.Move("DDD INVALIDO       ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1532- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1534- MOVE ZEROS TO CAD-DDD-FONE. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TELEFONE.CAD_DDD_FONE);
            }


            /*" -1535- IF CAD-FONE NOT NUMERIC */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TELEFONE.CAD_FONE.IsNumeric())
            {

                /*" -1536- MOVE 'TELEFONE INVALIDO       ' TO LD00-MSG1 */
                _.Move("TELEFONE INVALIDO       ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1537- MOVE ZEROS TO CAD-TELEFONE */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TELEFONE);

                /*" -1539- PERFORM R7600-IMPRIME-LD00-MSG1. */

                R7600_IMPRIME_LD00_MSG1_SECTION();
            }


            /*" -1540- IF CAD-CGC NOT NUMERIC OR CAD-CGC EQUAL ZEROS */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CGCX.CAD_CGC.IsNumeric() || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CGCX.CAD_CGC == 00)
            {

                /*" -1541- MOVE 'CGC INVALIDO                     ' TO LD00-MSG1 */
                _.Move("CGC INVALIDO                     ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1542- MOVE 1 TO WS-NECESSARIO */
                _.Move(1, WS_NECESSARIO);

                /*" -1543- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1545- MOVE 0 TO CAD-CGC. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CGCX.CAD_CGC);
            }


            /*" -1547- IF CAD-INSC-MUN NOT NUMERIC OR CAD-INSC-MUNX = ZEROS */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_INSC_MUNX.CAD_INSC_MUN.IsNumeric() || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_INSC_MUNX == 00)
            {

                /*" -1552- MOVE SPACES TO CAD-INSC-MUNX. */
                _.Move("", AREA_DE_WORK.REG_CAD_CADASTRO.CAD_INSC_MUNX);
            }


            /*" -1554- IF CAD-INSC-EST NOT NUMERIC OR CAD-INSC-ESTX = ZEROS */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_INSC_ESTX.CAD_INSC_EST.IsNumeric() || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_INSC_ESTX == 00)
            {

                /*" -1559- MOVE SPACES TO CAD-INSC-ESTX. */
                _.Move("", AREA_DE_WORK.REG_CAD_CADASTRO.CAD_INSC_ESTX);
            }


            /*" -1562- IF CAD-SITUACAO NOT EQUAL 0 AND CAD-SITUACAO NOT EQUAL 1 AND CAD-SITUACAO NOT EQUAL 2 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_SITUACAOX.CAD_SITUACAO != 0 && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_SITUACAOX.CAD_SITUACAO != 1 && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_SITUACAOX.CAD_SITUACAO != 2)
            {

                /*" -1563- MOVE 'SITUACAO INVALIDA       ' TO LD00-MSG1 */
                _.Move("SITUACAO INVALIDA       ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1564- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1566- MOVE 2 TO CAD-SITUACAO. */
                _.Move(2, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_SITUACAOX.CAD_SITUACAO);
            }


            /*" -1570- MOVE SPACES TO W-CAD-DATA-INI-VIG W-CAD-DATA-TER-VIG W-CAD-DATA-EXCLUSAO. */
            _.Move("", AREA_DE_WORK.W_CAD_DATA_INI_VIG, AREA_DE_WORK.W_CAD_DATA_TER_VIG, AREA_DE_WORK.W_CAD_DATA_EXCLUSAO);

            /*" -1572- IF CAD-DATA-INCLUSAO NOT NUMERIC OR CAD-DATA-INCLUSAO EQUAL ZEROS */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_DATA_INCLUSAOX.CAD_DATA_INCLUSAO.IsNumeric() || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_DATA_INCLUSAOX.CAD_DATA_INCLUSAO == 00)
            {

                /*" -1573- MOVE 'DATA DE INCLUSAO INVALIDA       ' TO LD00-MSG1 */
                _.Move("DATA DE INCLUSAO INVALIDA       ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1574- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1575- MOVE 0 TO CAD-DATA-INCLUSAO */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_DATA_INCLUSAOX.CAD_DATA_INCLUSAO);

                /*" -1576- ELSE */
            }
            else
            {


                /*" -1578- MOVE CAD-DATA-INCLUSAO TO W-DATA-AAAAMMDD WS-DATA-SEC */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_DATA_INCLUSAOX.CAD_DATA_INCLUSAO, AREA_DE_WORK.W_DATA_AAAAMMDD, AREA_DE_WORK.WS_DATA_SEC);

                /*" -1579- MOVE W-DATA-AAAAMMDD-DD TO W-DATA-AAAA-MM-DD-DD */
                _.Move(AREA_DE_WORK.W_DATA_AAAAMMDD.W_DATA_AAAAMMDD_DD, AREA_DE_WORK.W_DATA_AAAA_MM_DD.W_DATA_AAAA_MM_DD_DD);

                /*" -1580- MOVE W-DATA-AAAAMMDD-MM TO W-DATA-AAAA-MM-DD-MM */
                _.Move(AREA_DE_WORK.W_DATA_AAAAMMDD.W_DATA_AAAAMMDD_MM, AREA_DE_WORK.W_DATA_AAAA_MM_DD.W_DATA_AAAA_MM_DD_MM);

                /*" -1581- MOVE W-DATA-AAAAMMDD-AAAA TO W-DATA-AAAA-MM-DD-AAAA */
                _.Move(AREA_DE_WORK.W_DATA_AAAAMMDD.W_DATA_AAAAMMDD_AAAA, AREA_DE_WORK.W_DATA_AAAA_MM_DD.W_DATA_AAAA_MM_DD_AAAA);

                /*" -1582- MOVE W-DATA-AAAA-MM-DD TO W-CAD-DATA-INI-VIG */
                _.Move(AREA_DE_WORK.W_DATA_AAAA_MM_DD, AREA_DE_WORK.W_CAD_DATA_INI_VIG);

                /*" -1583- PERFORM R1055-CRITICA-DATA */

                R1055_CRITICA_DATA_SECTION();

                /*" -1584- IF WS-ERRO-DATA EQUAL 1 */

                if (AREA_DE_WORK.WS_ERRO_DATA == 1)
                {

                    /*" -1585- MOVE 'DATA DE INCLUSAO INVALIDA       ' TO LD00-MSG1 */
                    _.Move("DATA DE INCLUSAO INVALIDA       ", AREA_DE_WORK.LD00.LD00_MSG1);

                    /*" -1586- PERFORM R7600-IMPRIME-LD00-MSG1 */

                    R7600_IMPRIME_LD00_MSG1_SECTION();

                    /*" -1589- MOVE 0 TO CAD-DATA-INCLUSAO W-CAD-DATA-INI-VIG. */
                    _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_DATA_INCLUSAOX.CAD_DATA_INCLUSAO, AREA_DE_WORK.W_CAD_DATA_INI_VIG);
                }

            }


            /*" -1590- IF CAD-DATA-EXCLUSAO NOT NUMERIC */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_DATA_EXCLUSAOX.CAD_DATA_EXCLUSAO.IsNumeric())
            {

                /*" -1591- MOVE 'DATA DE EXCLUSAO NAO NUMERICA   ' TO LD00-MSG1 */
                _.Move("DATA DE EXCLUSAO NAO NUMERICA   ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1592- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1593- MOVE 0 TO CAD-DATA-EXCLUSAO */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_DATA_EXCLUSAOX.CAD_DATA_EXCLUSAO);

                /*" -1594- ELSE */
            }
            else
            {


                /*" -1595- IF CAD-DATA-EXCLUSAO > ZEROS */

                if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_DATA_EXCLUSAOX.CAD_DATA_EXCLUSAO > 00)
                {

                    /*" -1597- MOVE CAD-DATA-EXCLUSAO TO W-DATA-AAAAMMDD WS-DATA-SEC */
                    _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_DATA_EXCLUSAOX.CAD_DATA_EXCLUSAO, AREA_DE_WORK.W_DATA_AAAAMMDD, AREA_DE_WORK.WS_DATA_SEC);

                    /*" -1598- MOVE W-DATA-AAAAMMDD-DD TO W-DATA-AAAA-MM-DD-DD */
                    _.Move(AREA_DE_WORK.W_DATA_AAAAMMDD.W_DATA_AAAAMMDD_DD, AREA_DE_WORK.W_DATA_AAAA_MM_DD.W_DATA_AAAA_MM_DD_DD);

                    /*" -1599- MOVE W-DATA-AAAAMMDD-MM TO W-DATA-AAAA-MM-DD-MM */
                    _.Move(AREA_DE_WORK.W_DATA_AAAAMMDD.W_DATA_AAAAMMDD_MM, AREA_DE_WORK.W_DATA_AAAA_MM_DD.W_DATA_AAAA_MM_DD_MM);

                    /*" -1600- MOVE W-DATA-AAAAMMDD-AAAA TO W-DATA-AAAA-MM-DD-AAAA */
                    _.Move(AREA_DE_WORK.W_DATA_AAAAMMDD.W_DATA_AAAAMMDD_AAAA, AREA_DE_WORK.W_DATA_AAAA_MM_DD.W_DATA_AAAA_MM_DD_AAAA);

                    /*" -1601- MOVE W-DATA-AAAA-MM-DD TO W-CAD-DATA-TER-VIG */
                    _.Move(AREA_DE_WORK.W_DATA_AAAA_MM_DD, AREA_DE_WORK.W_CAD_DATA_TER_VIG);

                    /*" -1602- MOVE W-DATA-AAAA-MM-DD TO W-CAD-DATA-EXCLUSAO */
                    _.Move(AREA_DE_WORK.W_DATA_AAAA_MM_DD, AREA_DE_WORK.W_CAD_DATA_EXCLUSAO);

                    /*" -1603- PERFORM R1055-CRITICA-DATA */

                    R1055_CRITICA_DATA_SECTION();

                    /*" -1604- IF WS-ERRO-DATA EQUAL 1 */

                    if (AREA_DE_WORK.WS_ERRO_DATA == 1)
                    {

                        /*" -1605- MOVE 'DATA DE EXCLUSAO INVALIDA       ' TO LD00-MSG1 */
                        _.Move("DATA DE EXCLUSAO INVALIDA       ", AREA_DE_WORK.LD00.LD00_MSG1);

                        /*" -1606- PERFORM R7600-IMPRIME-LD00-MSG1 */

                        R7600_IMPRIME_LD00_MSG1_SECTION();

                        /*" -1610- MOVE 0 TO CAD-DATA-EXCLUSAO W-CAD-DATA-TER-VIG W-CAD-DATA-EXCLUSAO. */
                        _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_DATA_EXCLUSAOX.CAD_DATA_EXCLUSAO, AREA_DE_WORK.W_CAD_DATA_TER_VIG, AREA_DE_WORK.W_CAD_DATA_EXCLUSAO);
                    }

                }

            }


            /*" -1611- IF CAD-NUM-LOT-ANTERIOR NOT NUMERIC */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_LOT_ANTERIORX.CAD_NUM_LOT_ANTERIOR.IsNumeric())
            {

                /*" -1612- MOVE 'COD. LOTERICO ANTERIOR INVALIDO     ' TO LD00-MSG1 */
                _.Move("COD. LOTERICO ANTERIOR INVALIDO     ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1613- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1614- MOVE 0 TO CAD-NUM-LOT-ANTERIOR */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_LOT_ANTERIORX.CAD_NUM_LOT_ANTERIOR);

                /*" -1615- ELSE */
            }
            else
            {


                /*" -1616- IF CAD-NUM-LOT-ANTERIOR EQUAL CAD-COD-CEF */

                if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_LOT_ANTERIORX.CAD_NUM_LOT_ANTERIOR == AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF_R.CAD_COD_CEF)
                {

                    /*" -1618- MOVE 0 TO CAD-NUM-LOT-ANTERIOR. */
                    _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_LOT_ANTERIORX.CAD_NUM_LOT_ANTERIOR);
                }

            }


            /*" -1619- IF CAD-COD-AG-MASTER NOT NUMERIC */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_AG_MASTERX.CAD_COD_AG_MASTER.IsNumeric())
            {

                /*" -1620- MOVE 'CODIGO AGENTE MASTER INVALIDO     ' TO LD00-MSG1 */
                _.Move("CODIGO AGENTE MASTER INVALIDO     ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1621- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1623- MOVE 0 TO CAD-COD-AG-MASTER. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_AG_MASTERX.CAD_COD_AG_MASTER);
            }


            /*" -1625- IF CAD-CAT-LOTERICO NOT NUMERIC OR CAD-CAT-LOTERICO EQUAL ZEROS */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CAT_LOTERICOX.CAD_CAT_LOTERICO.IsNumeric() || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CAT_LOTERICOX.CAD_CAT_LOTERICO == 00)
            {

                /*" -1626- MOVE 'CATEGORIA INVALIDA                ' TO LD00-MSG1 */
                _.Move("CATEGORIA INVALIDA                ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1627- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1629- MOVE 0 TO CAD-CAT-LOTERICO. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CAT_LOTERICOX.CAD_CAT_LOTERICO);
            }


            /*" -1631- IF CAD-COD-STATUS NOT NUMERIC OR CAD-COD-STATUS EQUAL ZEROS */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_STATUSX.CAD_COD_STATUS.IsNumeric() || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_STATUSX.CAD_COD_STATUS == 00)
            {

                /*" -1632- MOVE 'CODIGO DO STATUS INVALIDO         ' TO LD00-MSG1 */
                _.Move("CODIGO DO STATUS INVALIDO         ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1633- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1637- MOVE 0 TO CAD-COD-STATUS. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_STATUSX.CAD_COD_STATUS);
            }


            /*" -1639- IF CAD-BANCO-DESC-CPMF NOT NUMERIC OR CAD-BANCO-DESC-CPMF EQUAL ZEROS */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_DESC_CPMFX.CAD_BANCO_DESC_CPMF.IsNumeric() || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_DESC_CPMFX.CAD_BANCO_DESC_CPMF == 00)
            {

                /*" -1640- MOVE 'BANCO COM DESC CPMF INVALIDO          ' TO LD00-MSG1 */
                _.Move("BANCO COM DESC CPMF INVALIDO          ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1641- MOVE 1 TO WS-NECESSARIO */
                _.Move(1, WS_NECESSARIO);

                /*" -1642- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1644- MOVE 0 TO CAD-BANCO-DESC-CPMF. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_DESC_CPMFX.CAD_BANCO_DESC_CPMF);
            }


            /*" -1646- IF CAD-AGEN-DESC-CPMF NOT NUMERIC OR CAD-AGEN-DESC-CPMF EQUAL ZEROS */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_DESC_CPMFX.CAD_AGEN_DESC_CPMF.IsNumeric() || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_DESC_CPMFX.CAD_AGEN_DESC_CPMF == 00)
            {

                /*" -1647- MOVE 'AGENCIA COM DESC CPMF INVALIDA       ' TO LD00-MSG1 */
                _.Move("AGENCIA COM DESC CPMF INVALIDA       ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1648- MOVE 1 TO WS-NECESSARIO */
                _.Move(1, WS_NECESSARIO);

                /*" -1649- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1651- MOVE 0 TO CAD-AGEN-DESC-CPMF. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_DESC_CPMFX.CAD_AGEN_DESC_CPMF);
            }


            /*" -1653- IF CAD-CONTA-DESC-CPMF NOT NUMERIC OR CAD-CONTA-DESC-CPMF EQUAL ZEROS */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_DESC_CPMFX.CAD_CONTA_DESC_CPMF.IsNumeric() || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_DESC_CPMFX.CAD_CONTA_DESC_CPMF == 00)
            {

                /*" -1654- MOVE 'CONTA COM DESC CPMF INVALIDA       ' TO LD00-MSG1 */
                _.Move("CONTA COM DESC CPMF INVALIDA       ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1655- MOVE 1 TO WS-NECESSARIO */
                _.Move(1, WS_NECESSARIO);

                /*" -1656- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1660- MOVE 0 TO CAD-CONTA-DESC-CPMF. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_DESC_CPMFX.CAD_CONTA_DESC_CPMF);
            }


            /*" -1662- IF CAD-BANCO-ISENTA NOT NUMERIC OR CAD-BANCO-ISENTA EQUAL ZEROS */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_ISENTAX.CAD_BANCO_ISENTA.IsNumeric() || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_ISENTAX.CAD_BANCO_ISENTA == 00)
            {

                /*" -1663- MOVE 'BANCO ISENTA  INVALIDO          ' TO LD00-MSG1 */
                _.Move("BANCO ISENTA  INVALIDO          ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1664- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1666- MOVE 0 TO CAD-BANCO-ISENTA. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_ISENTAX.CAD_BANCO_ISENTA);
            }


            /*" -1668- IF CAD-AGEN-ISENTA NOT NUMERIC OR CAD-AGEN-ISENTA EQUAL ZEROS */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_ISENTAX.CAD_AGEN_ISENTA.IsNumeric() || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_ISENTAX.CAD_AGEN_ISENTA == 00)
            {

                /*" -1669- MOVE 'AGENCIA ISENTA INVALIDA       ' TO LD00-MSG1 */
                _.Move("AGENCIA ISENTA INVALIDA       ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1670- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1672- MOVE 0 TO CAD-AGEN-ISENTA. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_ISENTAX.CAD_AGEN_ISENTA);
            }


            /*" -1674- IF CAD-CONTA-ISENTA NOT NUMERIC OR CAD-CONTA-ISENTA EQUAL ZEROS */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_ISENTAX.CAD_CONTA_ISENTA.IsNumeric() || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_ISENTAX.CAD_CONTA_ISENTA == 00)
            {

                /*" -1675- MOVE 'CONTA ISENTA  INVALIDA       ' TO LD00-MSG1 */
                _.Move("CONTA ISENTA  INVALIDA       ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1676- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1680- MOVE 0 TO CAD-CONTA-ISENTA. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_ISENTAX.CAD_CONTA_ISENTA);
            }


            /*" -1684- IF CAD-BANCO-CAUCAO NOT NUMERIC OR CAD-BANCO-CAUCAO EQUAL ZEROS */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_CAUCAOX.CAD_BANCO_CAUCAO.IsNumeric() || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_CAUCAOX.CAD_BANCO_CAUCAO == 00)
            {

                /*" -1686- MOVE 0 TO CAD-BANCO-CAUCAO. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_CAUCAOX.CAD_BANCO_CAUCAO);
            }


            /*" -1690- IF CAD-AGEN-CAUCAO NOT NUMERIC OR CAD-AGEN-CAUCAO EQUAL ZEROS */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_CAUCAOX.CAD_AGEN_CAUCAO.IsNumeric() || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_CAUCAOX.CAD_AGEN_CAUCAO == 00)
            {

                /*" -1692- MOVE 0 TO CAD-AGEN-CAUCAO. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_CAUCAOX.CAD_AGEN_CAUCAO);
            }


            /*" -1696- IF CAD-CONTA-CAUCAO NOT NUMERIC OR CAD-CONTA-CAUCAO EQUAL ZEROS */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_CAUCAOX.CAD_CONTA_CAUCAO.IsNumeric() || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_CAUCAOX.CAD_CONTA_CAUCAO == 00)
            {

                /*" -1700- MOVE 0 TO CAD-CONTA-CAUCAO. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_CAUCAOX.CAD_CONTA_CAUCAO);
            }


            /*" -1701- IF CAD-NIVEL-COMISSAO NOT NUMERIC */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NIVEL_COMISSAOX.CAD_NIVEL_COMISSAO.IsNumeric())
            {

                /*" -1702- MOVE 'NIVEL DE COMISSAO INVALIDA       ' TO LD00-MSG1 */
                _.Move("NIVEL DE COMISSAO INVALIDA       ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1703- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1705- MOVE 0 TO CAD-NIVEL-COMISSAO. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NIVEL_COMISSAOX.CAD_NIVEL_COMISSAO);
            }


            /*" -1706- IF CAD-PV-SUB NOT NUMERIC */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_PV_SUBX.CAD_PV_SUB.IsNumeric())
            {

                /*" -1707- MOVE 'CODIGO DO PV INVALIDO       ' TO LD00-MSG1 */
                _.Move("CODIGO DO PV INVALIDO       ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1708- MOVE 1 TO WS-NECESSARIO */
                _.Move(1, WS_NECESSARIO);

                /*" -1709- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1711- MOVE 0 TO CAD-PV-SUB. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_PV_SUBX.CAD_PV_SUB);
            }


            /*" -1712- IF CAD-EN-SUB NOT NUMERIC */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_EN_SUBX.CAD_EN_SUB.IsNumeric())
            {

                /*" -1713- MOVE 'CODIGO DO EN INVALIDO       ' TO LD00-MSG1 */
                _.Move("CODIGO DO EN INVALIDO       ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1714- MOVE 1 TO WS-NECESSARIO */
                _.Move(1, WS_NECESSARIO);

                /*" -1715- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1717- MOVE 0 TO CAD-EN-SUB. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_EN_SUBX.CAD_EN_SUB);
            }


            /*" -1718- IF CAD-UNIDADE-SUB NOT NUMERIC */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_UNIDADE_SUBX.CAD_UNIDADE_SUB.IsNumeric())
            {

                /*" -1719- MOVE 'CODIGO DA UNIDADE-SUB INVALIDO    ' TO LD00-MSG1 */
                _.Move("CODIGO DA UNIDADE-SUB INVALIDO    ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1720- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1722- MOVE 0 TO CAD-UNIDADE-SUB. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_UNIDADE_SUBX.CAD_UNIDADE_SUB);
            }


            /*" -1723- IF CAD-MATR-CONSULTOR NOT NUMERIC */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_MATR_CONSULTORX.CAD_MATR_CONSULTOR.IsNumeric())
            {

                /*" -1724- MOVE 'MATRICULA DO CONSULTOR INVALIDA  ' TO LD00-MSG1 */
                _.Move("MATRICULA DO CONSULTOR INVALIDA  ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1725- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1727- MOVE 0 TO CAD-MATR-CONSULTOR. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_MATR_CONSULTORX.CAD_MATR_CONSULTOR);
            }


            /*" -1730- IF CAD-TIPO-GARANTIA NOT EQUAL ' ' AND CAD-TIPO-GARANTIA NOT EQUAL 'C' AND CAD-TIPO-GARANTIA NOT EQUAL 'S' */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TIPO_GARANTIAX.CAD_TIPO_GARANTIA != " " && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TIPO_GARANTIAX.CAD_TIPO_GARANTIA != "C" && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TIPO_GARANTIAX.CAD_TIPO_GARANTIA != "S")
            {

                /*" -1731- MOVE 'TIPO DE GARANTIA INVALIDA       ' TO LD00-MSG1 */
                _.Move("TIPO DE GARANTIA INVALIDA       ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1732- MOVE 1 TO WS-NECESSARIO */
                _.Move(1, WS_NECESSARIO);

                /*" -1733- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1735- MOVE SPACES TO CAD-TIPO-GARANTIA. */
                _.Move("", AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TIPO_GARANTIAX.CAD_TIPO_GARANTIA);
            }


            /*" -1736- IF CAD-TIPO-GARANTIA EQUAL ' ' */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TIPO_GARANTIAX.CAD_TIPO_GARANTIA == " ")
            {

                /*" -1737- MOVE 0 TO CAD-VALOR-GARANTIA */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_VALOR_GARANTIAX.CAD_VALOR_GARANTIA);

                /*" -1738- ELSE */
            }
            else
            {


                /*" -1740- IF CAD-VALOR-GARANTIA NOT NUMERIC OR CAD-VALOR-GARANTIA EQUAL ZEROS */

                if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_VALOR_GARANTIAX.CAD_VALOR_GARANTIA.IsNumeric() || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_VALOR_GARANTIAX.CAD_VALOR_GARANTIA == 00)
                {

                    /*" -1741- MOVE 'VALOR DE GARANTIA INVALIDO       ' TO LD00-MSG1 */
                    _.Move("VALOR DE GARANTIA INVALIDO       ", AREA_DE_WORK.LD00.LD00_MSG1);

                    /*" -1742- MOVE 1 TO WS-NECESSARIO */
                    _.Move(1, WS_NECESSARIO);

                    /*" -1743- PERFORM R7600-IMPRIME-LD00-MSG1 */

                    R7600_IMPRIME_LD00_MSG1_SECTION();

                    /*" -1745- MOVE 0 TO CAD-VALOR-GARANTIA. */
                    _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_VALOR_GARANTIAX.CAD_VALOR_GARANTIA);
                }

            }


            /*" -1746- IF CAD-NUM-SEGURADORAX = SPACES */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_SEGURADORAX.IsEmpty())
            {

                /*" -1748- MOVE ZEROS TO CAD-NUM-SEGURADORA. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_SEGURADORAX.CAD_NUM_SEGURADORA);
            }


            /*" -1749- IF CAD-TIPO-GARANTIA NOT = 'S' */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TIPO_GARANTIAX.CAD_TIPO_GARANTIA != "S")
            {

                /*" -1750- MOVE 0 TO CAD-NUM-SEGURADORA */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_SEGURADORAX.CAD_NUM_SEGURADORA);

                /*" -1751- ELSE */
            }
            else
            {


                /*" -1753- IF CAD-NUM-SEGURADORA NOT NUMERIC OR CAD-NUM-SEGURADORA = ZEROS */

                if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_SEGURADORAX.CAD_NUM_SEGURADORA.IsNumeric() || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_SEGURADORAX.CAD_NUM_SEGURADORA == 00)
                {

                    /*" -1754- MOVE 'NUM.SEGURADORA INVALIDO       ' TO LD00-MSG1 */
                    _.Move("NUM.SEGURADORA INVALIDO       ", AREA_DE_WORK.LD00.LD00_MSG1);

                    /*" -1755- MOVE 1 TO WS-NECESSARIO */
                    _.Move(1, WS_NECESSARIO);

                    /*" -1756- PERFORM R7600-IMPRIME-LD00-MSG1 */

                    R7600_IMPRIME_LD00_MSG1_SECTION();

                    /*" -1758- MOVE 0 TO CAD-NUM-SEGURADORA. */
                    _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_SEGURADORAX.CAD_NUM_SEGURADORA);
                }

            }


            /*" -1761- IF CAD-BONUS-CKTX NOT EQUAL '0' AND CAD-BONUS-CKTX NOT EQUAL '1' AND CAD-BONUS-CKTX NOT EQUAL '2' */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_CKTX != "0" && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_CKTX != "1" && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_CKTX != "2")
            {

                /*" -1762- MOVE 'BONUS DE CKT-TV INVALIDO              ' TO LD00-MSG1 */
                _.Move("BONUS DE CKT-TV INVALIDO              ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1763- MOVE 1 TO WS-NECESSARIO */
                _.Move(1, WS_NECESSARIO);

                /*" -1764- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1766- MOVE 0 TO CAD-BONUS-CKT. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_CKTX.CAD_BONUS_CKT);
            }


            /*" -1769- IF CAD-BONUS-ALARMEX NOT EQUAL '0' AND CAD-BONUS-ALARMEX NOT EQUAL '1' AND CAD-BONUS-ALARMEX NOT EQUAL '2' */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_ALARMEX != "0" && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_ALARMEX != "1" && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_ALARMEX != "2")
            {

                /*" -1770- MOVE 'BONUS DE ALARME INVALIDO              ' TO LD00-MSG1 */
                _.Move("BONUS DE ALARME INVALIDO              ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1771- MOVE 1 TO WS-NECESSARIO */
                _.Move(1, WS_NECESSARIO);

                /*" -1772- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1774- MOVE 0 TO CAD-BONUS-ALARME. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_ALARMEX.CAD_BONUS_ALARME);
            }


            /*" -1777- IF CAD-BONUS-COFREX NOT EQUAL '0' AND CAD-BONUS-COFREX NOT EQUAL '1' AND CAD-BONUS-COFREX NOT EQUAL '2' */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_COFREX != "0" && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_COFREX != "1" && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_COFREX != "2")
            {

                /*" -1778- MOVE 'BONUS DE COFRE  INVALIDO              ' TO LD00-MSG1 */
                _.Move("BONUS DE COFRE  INVALIDO              ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1779- MOVE 1 TO WS-NECESSARIO */
                _.Move(1, WS_NECESSARIO);

                /*" -1780- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1782- MOVE 0 TO CAD-BONUS-COFRE. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_COFREX.CAD_BONUS_COFRE);
            }


            /*" -1783- IF CAD-NUMERO-FAX EQUAL SPACES */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUMERO_FAX.IsEmpty())
            {

                /*" -1784- MOVE ZEROS TO CAD-NUMERO-FAX */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUMERO_FAX);

                /*" -1785- ELSE */
            }
            else
            {


                /*" -1786- MOVE CAD-NUMERO-FAX TO WS-NUM-TELEF-ENT */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUMERO_FAX, AREA_DE_WORK.WS_NUM_TELEF_ENT);

                /*" -1787- PERFORM R1100-TRATAR-TELEF-FAX */

                R1100_TRATAR_TELEF_FAX_SECTION();

                /*" -1789- MOVE WS-NUM-TELEF-SAI TO CAD-NUMERO-FAX. */
                _.Move(AREA_DE_WORK.WS_NUM_TELEF_SAI, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUMERO_FAX);
            }


            /*" -1790- IF CAD-FAX NOT NUMERIC */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUMERO_FAX.CAD_FAX.IsNumeric())
            {

                /*" -1791- MOVE 'FAX NAO NUMERICO   ' TO LD00-MSG1 */
                _.Move("FAX NAO NUMERICO   ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1792- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1794- MOVE ZEROS TO CAD-NUMERO-FAX. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUMERO_FAX);
            }


            /*" -1795- IF CAD-DDD-FAX NOT NUMERIC */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUMERO_FAX.CAD_DDD_FAX.IsNumeric())
            {

                /*" -1796- MOVE 'DDD FAX NAO NUMERICO   ' TO LD00-MSG1 */
                _.Move("DDD FAX NAO NUMERICO   ", AREA_DE_WORK.LD00.LD00_MSG1);

                /*" -1797- PERFORM R7600-IMPRIME-LD00-MSG1 */

                R7600_IMPRIME_LD00_MSG1_SECTION();

                /*" -1800- MOVE ZEROS TO CAD-DDD-FAX. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUMERO_FAX.CAD_DDD_FAX);
            }


            /*" -1801- IF CAD-SITUACAO EQUAL 0 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_SITUACAOX.CAD_SITUACAO == 0)
            {

                /*" -1803- ADD 1 TO W-AC-INATIVOS. */
                AREA_DE_WORK.W_AC_INATIVOS.Value = AREA_DE_WORK.W_AC_INATIVOS + 1;
            }


            /*" -1804- IF CAD-SITUACAO EQUAL 1 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_SITUACAOX.CAD_SITUACAO == 1)
            {

                /*" -1806- ADD 1 TO W-AC-ATIVOS. */
                AREA_DE_WORK.W_AC_ATIVOS.Value = AREA_DE_WORK.W_AC_ATIVOS + 1;
            }


            /*" -1807- IF CAD-SITUACAO EQUAL 2 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_SITUACAOX.CAD_SITUACAO == 2)
            {

                /*" -1809- ADD 1 TO W-AC-PRECAD. */
                AREA_DE_WORK.W_AC_PRECAD.Value = AREA_DE_WORK.W_AC_PRECAD + 1;
            }


            /*" -1810- IF CAD-TIPO-GARANTIA EQUAL 'C' */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TIPO_GARANTIAX.CAD_TIPO_GARANTIA == "C")
            {

                /*" -1812- ADD 1 TO W-AC-GARANTIA-C. */
                AREA_DE_WORK.W_AC_GARANTIA_C.Value = AREA_DE_WORK.W_AC_GARANTIA_C + 1;
            }


            /*" -1813- IF CAD-TIPO-GARANTIA EQUAL 'S' */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TIPO_GARANTIAX.CAD_TIPO_GARANTIA == "S")
            {

                /*" -1815- ADD 1 TO W-AC-GARANTIA-S. */
                AREA_DE_WORK.W_AC_GARANTIA_S.Value = AREA_DE_WORK.W_AC_GARANTIA_S + 1;
            }


            /*" -1816- IF CAD-NUM-SEGURADORA = 1 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_SEGURADORAX.CAD_NUM_SEGURADORA == 1)
            {

                /*" -1817- ADD 1 TO W-AC-SEGURADORA-CS */
                AREA_DE_WORK.W_AC_SEGURADORA_CS.Value = AREA_DE_WORK.W_AC_SEGURADORA_CS + 1;

                /*" -1818- ELSE */
            }
            else
            {


                /*" -1819- IF CAD-NUM-SEGURADORA > 1 */

                if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_SEGURADORAX.CAD_NUM_SEGURADORA > 1)
                {

                    /*" -1819- ADD 1 TO W-AC-SEGURADORA-OUTRAS. */
                    AREA_DE_WORK.W_AC_SEGURADORA_OUTRAS.Value = AREA_DE_WORK.W_AC_SEGURADORA_OUTRAS + 1;
                }

            }


        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R1050_SAIDA*/

        [StopWatch]
        /*" R1055-CRITICA-DATA-SECTION */
        private void R1055_CRITICA_DATA_SECTION()
        {
            /*" -1830- MOVE 0 TO WS-ERRO-DATA. */
            _.Move(0, AREA_DE_WORK.WS_ERRO_DATA);

            /*" -1831- IF WS-DATA-SEC NOT NUMERIC */

            if (!AREA_DE_WORK.WS_DATA_SEC.IsNumeric())
            {

                /*" -1832- MOVE 1 TO WS-ERRO-DATA */
                _.Move(1, AREA_DE_WORK.WS_ERRO_DATA);

                /*" -1833- ELSE */
            }
            else
            {


                /*" -1837- IF WS-MM-DATA-SEC GREATER 12 OR WS-MM-DATA-SEC EQUAL 0 OR WS-DD-DATA-SEC EQUAL 0 OR WS-SS-DATA-SEC EQUAL 0 */

                if (AREA_DE_WORK.WS_DATA_SEC.WS_MM_DATA_SEC > 12 || AREA_DE_WORK.WS_DATA_SEC.WS_MM_DATA_SEC == 0 || AREA_DE_WORK.WS_DATA_SEC.WS_DD_DATA_SEC == 0 || AREA_DE_WORK.WS_DATA_SEC.WS_SS_DATA_SEC == 0)
                {

                    /*" -1838- MOVE 1 TO WS-ERRO-DATA */
                    _.Move(1, AREA_DE_WORK.WS_ERRO_DATA);

                    /*" -1839- ELSE */
                }
                else
                {


                    /*" -1843- IF WS-MM-DATA-SEC EQUAL 04 OR WS-MM-DATA-SEC EQUAL 06 OR WS-MM-DATA-SEC EQUAL 09 OR WS-MM-DATA-SEC EQUAL 11 */

                    if (AREA_DE_WORK.WS_DATA_SEC.WS_MM_DATA_SEC == 04 || AREA_DE_WORK.WS_DATA_SEC.WS_MM_DATA_SEC == 06 || AREA_DE_WORK.WS_DATA_SEC.WS_MM_DATA_SEC == 09 || AREA_DE_WORK.WS_DATA_SEC.WS_MM_DATA_SEC == 11)
                    {

                        /*" -1844- IF WS-DD-DATA-SEC GREATER 30 */

                        if (AREA_DE_WORK.WS_DATA_SEC.WS_DD_DATA_SEC > 30)
                        {

                            /*" -1845- MOVE 1 TO WS-ERRO-DATA */
                            _.Move(1, AREA_DE_WORK.WS_ERRO_DATA);

                            /*" -1847- ELSE NEXT SENTENCE */
                        }
                        else
                        {


                            /*" -1848- ELSE */
                        }

                    }
                    else
                    {


                        /*" -1849- IF WS-MM-DATA-SEC EQUAL 02 */

                        if (AREA_DE_WORK.WS_DATA_SEC.WS_MM_DATA_SEC == 02)
                        {

                            /*" -1851- DIVIDE WS-AA-DATA-SEC BY 4 GIVING WS-AUX REMAINDER WS-RESTO */
                            _.Divide(AREA_DE_WORK.WS_DATA_SEC.WS_AA_DATA_SEC, 4, AREA_DE_WORK.WS_AUX, AREA_DE_WORK.WS_RESTO);

                            /*" -1852- IF WS-RESTO GREATER 0 */

                            if (AREA_DE_WORK.WS_RESTO > 0)
                            {

                                /*" -1853- IF WS-DD-DATA-SEC GREATER 28 */

                                if (AREA_DE_WORK.WS_DATA_SEC.WS_DD_DATA_SEC > 28)
                                {

                                    /*" -1854- MOVE 1 TO WS-ERRO-DATA */
                                    _.Move(1, AREA_DE_WORK.WS_ERRO_DATA);

                                    /*" -1856- ELSE NEXT SENTENCE */
                                }
                                else
                                {


                                    /*" -1857- ELSE */
                                }

                            }
                            else
                            {


                                /*" -1858- IF WS-DD-DATA-SEC GREATER 29 */

                                if (AREA_DE_WORK.WS_DATA_SEC.WS_DD_DATA_SEC > 29)
                                {

                                    /*" -1859- MOVE 1 TO WS-ERRO-DATA */
                                    _.Move(1, AREA_DE_WORK.WS_ERRO_DATA);

                                    /*" -1861- ELSE NEXT SENTENCE */
                                }
                                else
                                {


                                    /*" -1862- ELSE */
                                }

                            }

                        }
                        else
                        {


                            /*" -1863- IF WS-DD-DATA-SEC GREATER 31 */

                            if (AREA_DE_WORK.WS_DATA_SEC.WS_DD_DATA_SEC > 31)
                            {

                                /*" -1863- MOVE 1 TO WS-ERRO-DATA. */
                                _.Move(1, AREA_DE_WORK.WS_ERRO_DATA);
                            }

                        }

                    }

                }

            }


        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R1055_SAIDA*/

        [StopWatch]
        /*" R1100-TRATAR-TELEF-FAX-SECTION */
        private void R1100_TRATAR_TELEF_FAX_SECTION()
        {
            /*" -1871- IF WS-NUM-TELEF-ENT NUMERIC */

            if (AREA_DE_WORK.WS_NUM_TELEF_ENT.IsNumeric())
            {

                /*" -1872- MOVE WS-DDD-ENT TO WS-DDD-SAI */
                _.Move(AREA_DE_WORK.FILLER_8.WS_DDD_ENT, AREA_DE_WORK.FILLER_9.WS_DDD_SAI);

                /*" -1873- MOVE WS-TELEFONE-ENT TO WS-TELEFONE-SAI */
                _.Move(AREA_DE_WORK.FILLER_8.WS_TELEFONE_ENT, AREA_DE_WORK.FILLER_9.WS_TELEFONE_SAI);

                /*" -1875- GO TO R1100-SAIDA. */
                /*Método Suprimido por falta de linha ou apenas EXIT nome: R1100_SAIDA*/ //GOTO
                return;
            }


            /*" -1877- MOVE ZEROS TO WS-NUM-TELEF-SAI. */
            _.Move(0, AREA_DE_WORK.WS_NUM_TELEF_SAI);

            /*" -1878- IF WS-DDD-ENT NUMERIC */

            if (AREA_DE_WORK.FILLER_8.WS_DDD_ENT.IsNumeric())
            {

                /*" -1879- MOVE WS-DDD-ENT TO WS-DDD-SAI */
                _.Move(AREA_DE_WORK.FILLER_8.WS_DDD_ENT, AREA_DE_WORK.FILLER_9.WS_DDD_SAI);

                /*" -1880- MOVE 7 TO WTAM-TEL */
                _.Move(7, AREA_DE_WORK.WTAM_TEL);

                /*" -1884- GO TO R1100-MONTA-TELEF. */

                R1100_MONTA_TELEF(); //GOTO
                return;
            }


            /*" -1886- MOVE 4 TO WIND1 WIND2. */
            _.Move(4, AREA_DE_WORK.WIND1, AREA_DE_WORK.WIND2);

            /*" -1888- PERFORM R1110-MOVIMENTA-TEL WIND2 TIMES. */

            for (int i = 0; i < AREA_DE_WORK.WIND2; i++)
            {

                R1110_MOVIMENTA_TEL_SECTION();

            }

            /*" -1890- MOVE 8 TO WTAM-TEL. */
            _.Move(8, AREA_DE_WORK.WTAM_TEL);

            /*" -1891- IF WS-NUM-TELEF-ENT(5:8) NUMERIC */

            if (AREA_DE_WORK.WS_NUM_TELEF_ENT.Substring(5, 8).IsNumeric())
            {

                /*" -1892- MOVE WS-NUM-TELEF-ENT(5:8) TO WS-TELEFONE-SAI */
                _.Move(AREA_DE_WORK.WS_NUM_TELEF_ENT.Substring(5, 8), AREA_DE_WORK.FILLER_9.WS_TELEFONE_SAI);

                /*" -1892- GO TO R1100-SAIDA. */
                /*Método Suprimido por falta de linha ou apenas EXIT nome: R1100_SAIDA*/ //GOTO
                return;
            }


            /*" -0- FLUXCONTROL_PERFORM R1100_MONTA_TELEF */

            R1100_MONTA_TELEF();

        }

        [StopWatch]
        /*" R1100-MONTA-TELEF */
        private void R1100_MONTA_TELEF(bool isPerform = false)
        {
            /*" -1900- MOVE 12 TO WIND1 WIND2. */
            _.Move(12, AREA_DE_WORK.WIND1, AREA_DE_WORK.WIND2);

            /*" -1900- PERFORM R1110-MOVIMENTA-TEL WTAM-TEL TIMES. */

            for (int i = 0; i < AREA_DE_WORK.WTAM_TEL; i++)
            {

                R1110_MOVIMENTA_TEL_SECTION();

            }

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R1100_SAIDA*/

        [StopWatch]
        /*" R1110-MOVIMENTA-TEL-SECTION */
        private void R1110_MOVIMENTA_TEL_SECTION()
        {
            /*" -1907- IF WS-NUM-TELEF-ENT(WIND1:1) NUMERIC */

            if (AREA_DE_WORK.WS_NUM_TELEF_ENT.Substring(AREA_DE_WORK.WIND1, 1).IsNumeric())
            {

                /*" -1909- MOVE WS-NUM-TELEF-ENT(WIND1:1) TO WS-NUM-TELEF-SAI(WIND2:1) */
                _.MoveAtPosition(AREA_DE_WORK.WS_NUM_TELEF_ENT.Substring(AREA_DE_WORK.WIND1, 1), AREA_DE_WORK.WS_NUM_TELEF_SAI, AREA_DE_WORK.WIND2, 1);

                /*" -1911- COMPUTE WIND2 = WIND2 - 1. */
                AREA_DE_WORK.WIND2.Value = AREA_DE_WORK.WIND2 - 1;
            }


            /*" -1911- COMPUTE WIND1 = WIND1 - 1. */
            AREA_DE_WORK.WIND1.Value = AREA_DE_WORK.WIND1 - 1;

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R1110_SAIDA*/

        [StopWatch]
        /*" R5000-SELECT-MAX-CONTA-SECTION */
        private void R5000_SELECT_MAX_CONTA_SECTION()
        {
            /*" -1920- MOVE '0002' TO WNR-EXEC-SQL. */
            _.Move("0002", LK_CONVERSAO.WABEND.WNR_EXEC_SQL);

            /*" -1925- PERFORM R5000_SELECT_MAX_CONTA_DB_SELECT_1 */

            R5000_SELECT_MAX_CONTA_DB_SELECT_1();

            /*" -1928- IF SQLCODE NOT EQUAL ZEROS */

            if (DB.SQLCODE != 00)
            {

                /*" -1929- DISPLAY 'ERRO SELECT FC_SEQUENCE ' */
                _.Display($"ERRO SELECT FC_SEQUENCE ");

                /*" -1931- GO TO R9999-ROT-ERRO. */

                R9999_ROT_ERRO_SECTION(); //GOTO
                return;
            }


            /*" -1933- DISPLAY 'R5000-MAX IDE-CONTA-BAN=' FCSEQUEN-NUM-SEQ */
            _.Display($"R5000-MAX IDE-CONTA-BAN={FCSEQUEN.DCLFC_SEQUENCE.FCSEQUEN_NUM_SEQ}");

            /*" -1933- MOVE FCSEQUEN-NUM-SEQ TO MAX-IDE-CONTA-BANCARIA. */
            _.Move(FCSEQUEN.DCLFC_SEQUENCE.FCSEQUEN_NUM_SEQ, MAX_IDE_CONTA_BANCARIA);

        }

        [StopWatch]
        /*" R5000-SELECT-MAX-CONTA-DB-SELECT-1 */
        public void R5000_SELECT_MAX_CONTA_DB_SELECT_1()
        {
            /*" -1925- EXEC SQL SELECT NUM_SEQ INTO :FCSEQUEN-NUM-SEQ FROM FDRCAP.FC_SEQUENCE WHERE COD_SEQ = 'COB' END-EXEC. */

            var r5000_SELECT_MAX_CONTA_DB_SELECT_1_Query1 = new R5000_SELECT_MAX_CONTA_DB_SELECT_1_Query1()
            {
            };

            var executed_1 = R5000_SELECT_MAX_CONTA_DB_SELECT_1_Query1.Execute(r5000_SELECT_MAX_CONTA_DB_SELECT_1_Query1);
            if (executed_1 != null)
            {
                _.Move(executed_1.FCSEQUEN_NUM_SEQ, FCSEQUEN.DCLFC_SEQUENCE.FCSEQUEN_NUM_SEQ);
            }


        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R5000_EXIT*/

        [StopWatch]
        /*" R5100-UPDATE-MAX-CONTA-SECTION */
        private void R5100_UPDATE_MAX_CONTA_SECTION()
        {
            /*" -1942- MOVE '0002' TO WNR-EXEC-SQL. */
            _.Move("0002", LK_CONVERSAO.WABEND.WNR_EXEC_SQL);

            /*" -1944- MOVE MAX-IDE-CONTA-BANCARIA TO FCSEQUEN-NUM-SEQ */
            _.Move(MAX_IDE_CONTA_BANCARIA, FCSEQUEN.DCLFC_SEQUENCE.FCSEQUEN_NUM_SEQ);

            /*" -1948- PERFORM R5100_UPDATE_MAX_CONTA_DB_UPDATE_1 */

            R5100_UPDATE_MAX_CONTA_DB_UPDATE_1();

            /*" -1951- IF SQLCODE NOT EQUAL ZEROS */

            if (DB.SQLCODE != 00)
            {

                /*" -1952- DISPLAY 'ERRO SELECT FC_SEQUENCE ' */
                _.Display($"ERRO SELECT FC_SEQUENCE ");

                /*" -1952- GO TO R9999-ROT-ERRO. */

                R9999_ROT_ERRO_SECTION(); //GOTO
                return;
            }


        }

        [StopWatch]
        /*" R5100-UPDATE-MAX-CONTA-DB-UPDATE-1 */
        public void R5100_UPDATE_MAX_CONTA_DB_UPDATE_1()
        {
            /*" -1948- EXEC SQL UPDATE FDRCAP.FC_SEQUENCE SET NUM_SEQ = :FCSEQUEN-NUM-SEQ WHERE COD_SEQ = 'COB' END-EXEC. */

            var r5100_UPDATE_MAX_CONTA_DB_UPDATE_1_Update1 = new R5100_UPDATE_MAX_CONTA_DB_UPDATE_1_Update1()
            {
                FCSEQUEN_NUM_SEQ = FCSEQUEN.DCLFC_SEQUENCE.FCSEQUEN_NUM_SEQ.ToString(),
            };

            R5100_UPDATE_MAX_CONTA_DB_UPDATE_1_Update1.Execute(r5100_UPDATE_MAX_CONTA_DB_UPDATE_1_Update1);

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R5100_EXIT*/

        [StopWatch]
        /*" R6000-VER-ALTERACAO-LOTERICO-SECTION */
        private void R6000_VER_ALTERACAO_LOTERICO_SECTION()
        {
            /*" -1961- MOVE SPACES TO LD00-MSG1. */
            _.Move("", AREA_DE_WORK.LD00.LD00_MSG1);

            /*" -1964- MOVE 'NAO' TO W-CHAVE-HOUVE-ALTERACAO W-CHAVE-ALTEROU-SEGURADO */
            _.Move("NAO", W_CHAVE_HOUVE_ALTERACAO, W_CHAVE_ALTEROU_SEGURADO);

            /*" -1969- MOVE 'N' TO LTMVPROP-IND-ALT-DADOS-PES LTMVPROP-IND-ALT-ENDER LTMVPROP-IND-ALT-COBER LTMVPROP-IND-ALT-BONUS. */
            _.Move("N", LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_DADOS_PES, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_ENDER, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_COBER, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_BONUS);

            /*" -1971- MOVE 'N' TO W-IND-ALT-FAXTELEME. */
            _.Move("N", W_IND_ALT_FAXTELEME);

            /*" -1973- MOVE 3 TO LTMVPROP-COD-MOVIMENTO. */
            _.Move(3, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_MOVIMENTO);

            /*" -1977- IF CAD-CGCX NOT EQUAL FCLOTERI-COD-CGC OR CAD-NOME-FANTASIA NOT EQUAL FCLOTERI-NOM-FANTASIA OR CAD-RAZAO-SOCIAL NOT EQUAL FCLOTERI-NOM-RAZAO-SOCIAL */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CGCX != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_CGC || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NOME_FANTASIA != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_FANTASIA || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_RAZAO_SOCIAL != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_RAZAO_SOCIAL)
            {

                /*" -1978- MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO */
                _.Move("SIM", W_CHAVE_HOUVE_ALTERACAO);

                /*" -1980- MOVE 'S' TO LTMVPROP-IND-ALT-DADOS-PES. */
                _.Move("S", LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_DADOS_PES);
            }


            /*" -1985- IF CAD-EN-SUB NOT EQUAL FCLOTERI-NUM-ENCEF OR CAD-NUM-LOT-ANTERIOR NOT EQUAL FCLOTERI-NUM-LOTER-ANT OR CAD-DV-CEF NOT EQUAL FCLOTERI-NUM-DV-LOTERICO OR CAD-PV-SUB NOT EQUAL FCLOTERI-NUM-PVCEF */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_EN_SUBX.CAD_EN_SUB != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_ENCEF || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_LOT_ANTERIORX.CAD_NUM_LOT_ANTERIOR != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_LOTER_ANT || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF_R.CAD_DV_CEF != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_DV_LOTERICO || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_PV_SUBX.CAD_PV_SUB != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_PVCEF)
            {

                /*" -1986- MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO */
                _.Move("SIM", W_CHAVE_HOUVE_ALTERACAO);

                /*" -1988- MOVE 'S' TO LTMVPROP-IND-ALT-DADOS-PES. */
                _.Move("S", LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_DADOS_PES);
            }


            /*" -1989- MOVE FCLOTERI-NOM-BAIRRO TO WS-NOME-BAIRRO. */
            _.Move(FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_BAIRRO, AREA_DE_WORK.WS_NOME_BAIRRO);

            /*" -1991- MOVE FCLOTERI-NOM-MUNICIPIO TO WS-NOME-CIDADE. */
            _.Move(FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_MUNICIPIO, AREA_DE_WORK.WS_NOME_CIDADE);

            /*" -1998- IF CAD-UF NOT EQUAL FCLOTERI-COD-UF OR CAD-EMAIL NOT EQUAL FCLOTERI-DES-EMAIL OR CAD-ENDERECO NOT EQUAL FCLOTERI-DES-ENDERECO OR CAD-BAIRRO NOT EQUAL WS-BAIRRO OR CAD-CIDADE NOT EQUAL WS-CIDADE OR CAD-CEP NOT EQUAL FCLOTERI-NUM-CEP */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_UF != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_UF || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_EMAILX.CAD_EMAIL != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DES_EMAIL || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_ENDERECO != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DES_ENDERECO || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BAIRRO != AREA_DE_WORK.WS_NOME_BAIRRO_R.WS_BAIRRO || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CIDADE != AREA_DE_WORK.WS_NOME_CIDADE_R.WS_CIDADE || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CEPX.CAD_CEP != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_CEP)
            {

                /*" -1999- MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO */
                _.Move("SIM", W_CHAVE_HOUVE_ALTERACAO);

                /*" -2009- MOVE 'S' TO LTMVPROP-IND-ALT-ENDER. */
                _.Move("S", LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_ENDER);
            }


            /*" -2010- MOVE FCLOTERI-NUM-TELEFONE TO WS-NUM-TELEF-SAI */
            _.Move(FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_TELEFONE, AREA_DE_WORK.WS_NUM_TELEF_SAI);

            /*" -2013- MOVE FCLOTERI-NUM-FAX TO WS-NUMERO-FAX. */
            _.Move(FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_FAX, AREA_DE_WORK.WS_NUMERO_FAX);

            /*" -2020- IF CAD-TELEFONE NOT EQUAL WS-NUM-TELEF-SAI */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TELEFONE != AREA_DE_WORK.WS_NUM_TELEF_SAI)
            {

                /*" -2021- MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO */
                _.Move("SIM", W_CHAVE_HOUVE_ALTERACAO);

                /*" -2025- MOVE 'S' TO LTMVPROP-IND-ALT-ENDER . */
                _.Move("S", LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_ENDER);
            }


            /*" -2027- IF CAD-NUMERO-FAX NOT EQUAL WS-NUMER-FAX AND CAD-FAX NOT EQUAL 99999999 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUMERO_FAX != AREA_DE_WORK.WS_NUMERO_FAX_R.WS_NUMER_FAX && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUMERO_FAX.CAD_FAX != 99999999)
            {

                /*" -2028- DISPLAY 'ALTER 4 = ' CAD-CODIGO-CEF */
                _.Display($"ALTER 4 = {AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF}");

                /*" -2030- DISPLAY 'CAD.FAX = ' CAD-NUMERO-FAX ' FC_FAX = ' WS-NUMER-FAX */

                $"CAD.FAX = {AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUMERO_FAX} FC_FAX = {AREA_DE_WORK.WS_NUMERO_FAX_R.WS_NUMER_FAX}"
                .Display();

                /*" -2031- MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO */
                _.Move("SIM", W_CHAVE_HOUVE_ALTERACAO);

                /*" -2035- MOVE 'S' TO LTMVPROP-IND-ALT-ENDER. */
                _.Move("S", LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_ENDER);
            }


            /*" -2037- IF CAD-VALOR-GARANTIA NOT EQUAL FCLOTERI-VLR-GARANTIA */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_VALOR_GARANTIAX.CAD_VALOR_GARANTIA != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_VLR_GARANTIA)
            {

                /*" -2038- MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO */
                _.Move("SIM", W_CHAVE_HOUVE_ALTERACAO);

                /*" -2040- MOVE 'S' TO LTMVPROP-IND-ALT-COBER. */
                _.Move("S", LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_COBER);
            }


            /*" -2045- IF CAD-COD-MUNICIPIO NOT = FCLOTERI-COD-MUNICIPIO OR CAD-INSC-MUNX NOT = FCLOTERI-COD-INSCR-MUNIC OR CAD-INSC-ESTX NOT = FCLOTERI-COD-INSCR-ESTAD OR CAD-SITUACAOX NOT = FCLOTERI-STA-LOTERICO */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_MUNICIPIO != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_MUNICIPIO || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_INSC_MUNX != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_INSCR_MUNIC || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_INSC_ESTX != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_INSCR_ESTAD || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_SITUACAOX != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_STA_LOTERICO)
            {

                /*" -2047- MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO. */
                _.Move("SIM", W_CHAVE_HOUVE_ALTERACAO);
            }


            /*" -2053- IF CAD-COD-AG-MASTERX NOT = FCLOTERI-COD-AGENTE-MASTER OR CAD-CAT-LOTERICO NOT = FCLOTERI-IND-CAT-LOTERICO OR CAD-COD-STATUS NOT = FCLOTERI-IND-STA-LOTERICO OR CAD-UNIDADE-SUB NOT = FCLOTERI-IND-UNIDADE-SUB OR W-CAD-DATA-INI-VIG NOT = FCLOTERI-DTH-INCLUSAO */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_AG_MASTERX != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_AGENTE_MASTER || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CAT_LOTERICOX.CAD_CAT_LOTERICO != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IND_CAT_LOTERICO || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_STATUSX.CAD_COD_STATUS != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IND_STA_LOTERICO || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_UNIDADE_SUBX.CAD_UNIDADE_SUB != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IND_UNIDADE_SUB || AREA_DE_WORK.W_CAD_DATA_INI_VIG != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DTH_INCLUSAO)
            {

                /*" -2055- MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO. */
                _.Move("SIM", W_CHAVE_HOUVE_ALTERACAO);
            }


            /*" -2062- IF CAD-TIPO-GARANTIA NOT = FCLOTERI-COD-GARANTIA OR CAD-MATR-CONSULTOR NOT = FCLOTERI-NUM-MATR-CONSULTOR OR CAD-NOME-CONSULTOR NOT = FCLOTERI-NOM-CONSULTOR OR CAD-CONTATO1 NOT = FCLOTERI-NOM-CONTATO1 OR CAD-CONTATO2 NOT = FCLOTERI-NOM-CONTATO2 OR CAD-NIVEL-COMISSAOX NOT = FCLOTERI-STA-NIVEL-COMIS */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TIPO_GARANTIAX.CAD_TIPO_GARANTIA != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_GARANTIA || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_MATR_CONSULTORX.CAD_MATR_CONSULTOR != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_MATR_CONSULTOR || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NOME_CONSULTORX.CAD_NOME_CONSULTOR != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_CONSULTOR || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTATO1 != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_CONTATO1 || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTATO2 != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_CONTATO2 || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NIVEL_COMISSAOX != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_STA_NIVEL_COMIS)
            {

                /*" -2064- MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO. */
                _.Move("SIM", W_CHAVE_HOUVE_ALTERACAO);
            }


            /*" -2067- IF W-CAD-DATA-EXCLUSAO NOT = SPACES AND W-CAD-DATA-EXCLUSAO NOT = FCLOTERI-DTH-EXCLUSAO */

            if (!AREA_DE_WORK.W_CAD_DATA_EXCLUSAO.IsEmpty() && AREA_DE_WORK.W_CAD_DATA_EXCLUSAO != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DTH_EXCLUSAO)
            {

                /*" -2069- MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO. */
                _.Move("SIM", W_CHAVE_HOUVE_ALTERACAO);
            }


            /*" -2070- IF CAD-NUM-SEGURADORA NOT = FCLOTERI-NUM-SEGURADORA */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_SEGURADORAX.CAD_NUM_SEGURADORA != FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_SEGURADORA)
            {

                /*" -2074- MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO. */
                _.Move("SIM", W_CHAVE_HOUVE_ALTERACAO);
            }


            /*" -2075- IF W-CHAVE-CADASTRADO-SASSE NOT = 'SIM' */

            if (W_CHAVE_CADASTRADO_SASSE != "SIM")
            {

                /*" -2077- GO TO R6000-PULA. */

                R6000_PULA(); //GOTO
                return;
            }


            /*" -2078- IF CAD-SITUACAO EQUAL '0' */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_SITUACAOX.CAD_SITUACAO == "0")
            {

                /*" -2079- MOVE 7 TO LTMVPROP-COD-MOVIMENTO */
                _.Move(7, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_MOVIMENTO);

                /*" -2080- MOVE 'SIM' TO W-CHAVE-ALTEROU-SEGURADO */
                _.Move("SIM", W_CHAVE_ALTEROU_SEGURADO);

                /*" -2082- GO TO R6000-SAIDA. */
                /*Método Suprimido por falta de linha ou apenas EXIT nome: R6000_SAIDA*/ //GOTO
                return;
            }


            /*" -2083- IF CAD-TIPO-GARANTIA NOT = 'S' */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TIPO_GARANTIAX.CAD_TIPO_GARANTIA != "S")
            {

                /*" -2084- MOVE 7 TO LTMVPROP-COD-MOVIMENTO */
                _.Move(7, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_MOVIMENTO);

                /*" -2085- MOVE 'SIM' TO W-CHAVE-ALTEROU-SEGURADO */
                _.Move("SIM", W_CHAVE_ALTEROU_SEGURADO);

                /*" -2087- GO TO R6000-SAIDA. */
                /*Método Suprimido por falta de linha ou apenas EXIT nome: R6000_SAIDA*/ //GOTO
                return;
            }


            /*" -2088- IF CAD-NUM-SEGURADORA NOT EQUAL 001 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_SEGURADORAX.CAD_NUM_SEGURADORA != 001)
            {

                /*" -2089- MOVE 7 TO LTMVPROP-COD-MOVIMENTO */
                _.Move(7, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_MOVIMENTO);

                /*" -2090- MOVE 'SIM' TO W-CHAVE-ALTEROU-SEGURADO */
                _.Move("SIM", W_CHAVE_ALTEROU_SEGURADO);

                /*" -2092- GO TO R6000-SAIDA. */
                /*Método Suprimido por falta de linha ou apenas EXIT nome: R6000_SAIDA*/ //GOTO
                return;
            }


            /*" -2097- IF LTMVPROP-IND-ALT-DADOS-PES = 'S' OR LTMVPROP-IND-ALT-ENDER = 'S' OR W-IND-ALT-FAXTELEME = 'S' OR LTMVPROP-IND-ALT-COBER = 'S' OR LTMVPROP-IND-ALT-BONUS = 'S' */

            if (LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_DADOS_PES == "S" || LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_ENDER == "S" || W_IND_ALT_FAXTELEME == "S" || LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_COBER == "S" || LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_BONUS == "S")
            {

                /*" -2097- MOVE 'SIM' TO W-CHAVE-ALTEROU-SEGURADO. */
                _.Move("SIM", W_CHAVE_ALTEROU_SEGURADO);
            }


            /*" -0- FLUXCONTROL_PERFORM R6000_PULA */

            R6000_PULA();

        }

        [StopWatch]
        /*" R6000-PULA */
        private void R6000_PULA(bool isPerform = false)
        {
            /*" -2101- IF WS-NECESSARIO = 1 */

            if (WS_NECESSARIO == 1)
            {

                /*" -2101- MOVE 'NAO' TO W-CHAVE-ALTEROU-SEGURADO. */
                _.Move("NAO", W_CHAVE_ALTEROU_SEGURADO);
            }


        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6000_SAIDA*/

        [StopWatch]
        /*" R6020-SELECT-FC-LOTERICO-SECTION */
        private void R6020_SELECT_FC_LOTERICO_SECTION()
        {
            /*" -2110- MOVE '6020' TO WNR-EXEC-SQL. */
            _.Move("6020", LK_CONVERSAO.WABEND.WNR_EXEC_SQL);

            /*" -2111- MOVE CAD-COD-CEF TO FCLOTERI-NUM-LOTERICO. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF_R.CAD_COD_CEF, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_LOTERICO);

            /*" -2113- MOVE 'NAO' TO W-CHAVE-CADASTRADO-SIGEL. */
            _.Move("NAO", W_CHAVE_CADASTRADO_SIGEL);

            /*" -2194- PERFORM R6020_SELECT_FC_LOTERICO_DB_SELECT_1 */

            R6020_SELECT_FC_LOTERICO_DB_SELECT_1();

            /*" -2198- IF SQLCODE EQUAL ZEROS */

            if (DB.SQLCODE == 00)
            {

                /*" -2199- MOVE 'SIM' TO W-CHAVE-CADASTRADO-SIGEL */
                _.Move("SIM", W_CHAVE_CADASTRADO_SIGEL);

                /*" -2200- ELSE */
            }
            else
            {


                /*" -2201- IF SQLCODE NOT EQUAL 100 */

                if (DB.SQLCODE != 100)
                {

                    /*" -2202- DISPLAY ' 6020-ERRO SELECT FC_LOTERICO SIGEL =' */
                    _.Display($" 6020-ERRO SELECT FC_LOTERICO SIGEL =");

                    /*" -2203- DISPLAY ' NUM-LOTERICO=' FCLOTERI-NUM-LOTERICO */
                    _.Display($" NUM-LOTERICO={FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_LOTERICO}");

                    /*" -2203- GO TO R9999-ROT-ERRO. */

                    R9999_ROT_ERRO_SECTION(); //GOTO
                    return;
                }

            }


        }

        [StopWatch]
        /*" R6020-SELECT-FC-LOTERICO-DB-SELECT-1 */
        public void R6020_SELECT_FC_LOTERICO_DB_SELECT_1()
        {
            /*" -2194- EXEC SQL SELECT VALUE(COD_AGENTE_MASTER, ' ' ), VALUE(COD_CGC, '000000000000000' ), VALUE(COD_INSCR_ESTAD, ' ' ), VALUE(COD_INSCR_MUNIC, ' ' ), VALUE(COD_MUNICIPIO, ' ' ), VALUE(COD_UF, ' ' ), VALUE(DES_EMAIL, ' ' ), VALUE(DES_ENDERECO, ' ' ), VALUE(DTH_EXCLUSAO,DATE( '9999-12-31' )), VALUE(DTH_INCLUSAO,DATE( '9999-12-31' )), VALUE(IDE_CONTA_CAUCAO,0), VALUE(IDE_CONTA_CPMF,0), VALUE(IDE_CONTA_ISENTA,0), VALUE(IND_CAT_LOTERICO,0), VALUE(IND_STA_LOTERICO,0), VALUE(IND_UNIDADE_SUB,0), VALUE(NOM_BAIRRO, ' ' ), VALUE(NOM_CONSULTOR, ' ' ), VALUE(NOM_CONTATO1, ' ' ), VALUE(NOM_CONTATO2, ' ' ), VALUE(NOM_FANTASIA, ' ' ), VALUE(NOM_MUNICIPIO, ' ' ), VALUE(NOM_RAZAO_SOCIAL, ' ' ), VALUE(NUM_CEP,0), VALUE(NUM_ENCEF,0), VALUE(NUM_LOTER_ANT,0), VALUE(NUM_MATR_CONSULTOR,0), VALUE(NUM_PVCEF,0), VALUE(NUM_TELEFONE, '000000000000' ), VALUE(STA_DADOS_N_CADAST, ' ' ), VALUE(STA_LOTERICO, ' ' ), VALUE(STA_NIVEL_COMIS, ' ' ), VALUE(STA_ULT_ALT_ONLINE, ' ' ), VALUE(COD_GARANTIA, ' ' ), VALUE(VLR_GARANTIA,0), VALUE(DTH_GERACAO,DATE( '9999-12-31' )), VALUE(NUM_FAX, '000000000000' ), VALUE(NUM_DV_LOTERICO,0), VALUE(NUM_SEGURADORA,0) INTO :FCLOTERI-COD-AGENTE-MASTER , :FCLOTERI-COD-CGC , :FCLOTERI-COD-INSCR-ESTAD , :FCLOTERI-COD-INSCR-MUNIC , :FCLOTERI-COD-MUNICIPIO , :FCLOTERI-COD-UF , :FCLOTERI-DES-EMAIL , :FCLOTERI-DES-ENDERECO , :FCLOTERI-DTH-EXCLUSAO , :FCLOTERI-DTH-INCLUSAO , :FCLOTERI-IDE-CONTA-CAUCAO , :FCLOTERI-IDE-CONTA-CPMF , :FCLOTERI-IDE-CONTA-ISENTA , :FCLOTERI-IND-CAT-LOTERICO , :FCLOTERI-IND-STA-LOTERICO , :FCLOTERI-IND-UNIDADE-SUB , :FCLOTERI-NOM-BAIRRO , :FCLOTERI-NOM-CONSULTOR , :FCLOTERI-NOM-CONTATO1 , :FCLOTERI-NOM-CONTATO2 , :FCLOTERI-NOM-FANTASIA , :FCLOTERI-NOM-MUNICIPIO , :FCLOTERI-NOM-RAZAO-SOCIAL , :FCLOTERI-NUM-CEP , :FCLOTERI-NUM-ENCEF , :FCLOTERI-NUM-LOTER-ANT , :FCLOTERI-NUM-MATR-CONSULTOR, :FCLOTERI-NUM-PVCEF , :FCLOTERI-NUM-TELEFONE , :FCLOTERI-STA-DADOS-N-CADAST, :FCLOTERI-STA-LOTERICO , :FCLOTERI-STA-NIVEL-COMIS , :FCLOTERI-STA-ULT-ALT-ONLINE, :FCLOTERI-COD-GARANTIA , :FCLOTERI-VLR-GARANTIA , :FCLOTERI-DTH-GERACAO , :FCLOTERI-NUM-FAX , :FCLOTERI-NUM-DV-LOTERICO , :FCLOTERI-NUM-SEGURADORA FROM FDRCAP.FC_LOTERICO WHERE NUM_LOTERICO = :FCLOTERI-NUM-LOTERICO END-EXEC. */

            var r6020_SELECT_FC_LOTERICO_DB_SELECT_1_Query1 = new R6020_SELECT_FC_LOTERICO_DB_SELECT_1_Query1()
            {
                FCLOTERI_NUM_LOTERICO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_LOTERICO.ToString(),
            };

            var executed_1 = R6020_SELECT_FC_LOTERICO_DB_SELECT_1_Query1.Execute(r6020_SELECT_FC_LOTERICO_DB_SELECT_1_Query1);
            if (executed_1 != null)
            {
                _.Move(executed_1.FCLOTERI_COD_AGENTE_MASTER, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_AGENTE_MASTER);
                _.Move(executed_1.FCLOTERI_COD_CGC, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_CGC);
                _.Move(executed_1.FCLOTERI_COD_INSCR_ESTAD, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_INSCR_ESTAD);
                _.Move(executed_1.FCLOTERI_COD_INSCR_MUNIC, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_INSCR_MUNIC);
                _.Move(executed_1.FCLOTERI_COD_MUNICIPIO, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_MUNICIPIO);
                _.Move(executed_1.FCLOTERI_COD_UF, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_UF);
                _.Move(executed_1.FCLOTERI_DES_EMAIL, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DES_EMAIL);
                _.Move(executed_1.FCLOTERI_DES_ENDERECO, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DES_ENDERECO);
                _.Move(executed_1.FCLOTERI_DTH_EXCLUSAO, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DTH_EXCLUSAO);
                _.Move(executed_1.FCLOTERI_DTH_INCLUSAO, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DTH_INCLUSAO);
                _.Move(executed_1.FCLOTERI_IDE_CONTA_CAUCAO, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IDE_CONTA_CAUCAO);
                _.Move(executed_1.FCLOTERI_IDE_CONTA_CPMF, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IDE_CONTA_CPMF);
                _.Move(executed_1.FCLOTERI_IDE_CONTA_ISENTA, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IDE_CONTA_ISENTA);
                _.Move(executed_1.FCLOTERI_IND_CAT_LOTERICO, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IND_CAT_LOTERICO);
                _.Move(executed_1.FCLOTERI_IND_STA_LOTERICO, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IND_STA_LOTERICO);
                _.Move(executed_1.FCLOTERI_IND_UNIDADE_SUB, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IND_UNIDADE_SUB);
                _.Move(executed_1.FCLOTERI_NOM_BAIRRO, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_BAIRRO);
                _.Move(executed_1.FCLOTERI_NOM_CONSULTOR, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_CONSULTOR);
                _.Move(executed_1.FCLOTERI_NOM_CONTATO1, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_CONTATO1);
                _.Move(executed_1.FCLOTERI_NOM_CONTATO2, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_CONTATO2);
                _.Move(executed_1.FCLOTERI_NOM_FANTASIA, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_FANTASIA);
                _.Move(executed_1.FCLOTERI_NOM_MUNICIPIO, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_MUNICIPIO);
                _.Move(executed_1.FCLOTERI_NOM_RAZAO_SOCIAL, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_RAZAO_SOCIAL);
                _.Move(executed_1.FCLOTERI_NUM_CEP, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_CEP);
                _.Move(executed_1.FCLOTERI_NUM_ENCEF, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_ENCEF);
                _.Move(executed_1.FCLOTERI_NUM_LOTER_ANT, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_LOTER_ANT);
                _.Move(executed_1.FCLOTERI_NUM_MATR_CONSULTOR, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_MATR_CONSULTOR);
                _.Move(executed_1.FCLOTERI_NUM_PVCEF, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_PVCEF);
                _.Move(executed_1.FCLOTERI_NUM_TELEFONE, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_TELEFONE);
                _.Move(executed_1.FCLOTERI_STA_DADOS_N_CADAST, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_STA_DADOS_N_CADAST);
                _.Move(executed_1.FCLOTERI_STA_LOTERICO, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_STA_LOTERICO);
                _.Move(executed_1.FCLOTERI_STA_NIVEL_COMIS, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_STA_NIVEL_COMIS);
                _.Move(executed_1.FCLOTERI_STA_ULT_ALT_ONLINE, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_STA_ULT_ALT_ONLINE);
                _.Move(executed_1.FCLOTERI_COD_GARANTIA, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_GARANTIA);
                _.Move(executed_1.FCLOTERI_VLR_GARANTIA, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_VLR_GARANTIA);
                _.Move(executed_1.FCLOTERI_DTH_GERACAO, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DTH_GERACAO);
                _.Move(executed_1.FCLOTERI_NUM_FAX, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_FAX);
                _.Move(executed_1.FCLOTERI_NUM_DV_LOTERICO, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_DV_LOTERICO);
                _.Move(executed_1.FCLOTERI_NUM_SEGURADORA, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_SEGURADORA);
            }


        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6020_SAIDA*/

        [StopWatch]
        /*" R6030-SELECT-V0LOTERICO01-SECTION */
        private void R6030_SELECT_V0LOTERICO01_SECTION()
        {
            /*" -2212- MOVE '6030' TO WNR-EXEC-SQL. */
            _.Move("6030", LK_CONVERSAO.WABEND.WNR_EXEC_SQL);

            /*" -2214- MOVE CAD-CODIGO-CEF TO V0LOT-COD-LOT-CEF. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF, V0LOT_COD_LOT_CEF);

            /*" -2216- MOVE 'NAO' TO W-CHAVE-CADASTRADO-SASSE */
            _.Move("NAO", W_CHAVE_CADASTRADO_SASSE);

            /*" -2223- PERFORM R6030_SELECT_V0LOTERICO01_DB_SELECT_1 */

            R6030_SELECT_V0LOTERICO01_DB_SELECT_1();

            /*" -2228- IF SQLCODE EQUAL ZEROS */

            if (DB.SQLCODE == 00)
            {

                /*" -2229- MOVE 'SIM' TO W-CHAVE-CADASTRADO-SASSE */
                _.Move("SIM", W_CHAVE_CADASTRADO_SASSE);

                /*" -2230- ADD 1 TO WS-MVPROP-TOTAL-SEGURADOS */
                AREA_DE_WORK.WS_MVPROP_TOTAL_SEGURADOS.Value = AREA_DE_WORK.WS_MVPROP_TOTAL_SEGURADOS + 1;

                /*" -2231- ELSE */
            }
            else
            {


                /*" -2232- IF SQLCODE EQUAL 100 */

                if (DB.SQLCODE == 100)
                {

                    /*" -2233- ADD 1 TO WS-MVPROP-TOTAL-NSEGURADOS */
                    AREA_DE_WORK.WS_MVPROP_TOTAL_NSEGURADOS.Value = AREA_DE_WORK.WS_MVPROP_TOTAL_NSEGURADOS + 1;

                    /*" -2234- ELSE */
                }
                else
                {


                    /*" -2235- DISPLAY 'R6030-ERRO DE SELECT V0LOTERICO01 ' */
                    _.Display($"R6030-ERRO DE SELECT V0LOTERICO01 ");

                    /*" -2236- DISPLAY 'COD-CEF    = ' V0LOT-COD-LOT-CEF */
                    _.Display($"COD-CEF    = {V0LOT_COD_LOT_CEF}");

                    /*" -2238- GO TO R9999-ROT-ERRO. */

                    R9999_ROT_ERRO_SECTION(); //GOTO
                    return;
                }

            }


            /*" -2239- IF W-CHAVE-CADASTRADO-SIGEL = 'NAO' */

            if (W_CHAVE_CADASTRADO_SIGEL == "NAO")
            {

                /*" -2240- IF W-CHAVE-CADASTRADO-SASSE = 'SIM' */

                if (W_CHAVE_CADASTRADO_SASSE == "SIM")
                {

                    /*" -2241- DISPLAY ' LOTERICO SEGURADO SEM CADASTRO NO SIGEL = ' */
                    _.Display($" LOTERICO SEGURADO SEM CADASTRO NO SIGEL = ");

                    /*" -2242- DISPLAY ' NUM-LOTERICO=' FCLOTERI-NUM-LOTERICO */
                    _.Display($" NUM-LOTERICO={FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_LOTERICO}");

                    /*" -2242- GO TO R9999-ROT-ERRO. */

                    R9999_ROT_ERRO_SECTION(); //GOTO
                    return;
                }

            }


        }

        [StopWatch]
        /*" R6030-SELECT-V0LOTERICO01-DB-SELECT-1 */
        public void R6030_SELECT_V0LOTERICO01_DB_SELECT_1()
        {
            /*" -2223- EXEC SQL SELECT COD_LOT_FENAL INTO :V0LOT-COD-LOT-FENAL FROM SEGUROS.V0LOTERICO01 WHERE NUM_APOLICE = :WS-NUM-APOLICE AND COD_LOT_FENAL = :V0LOT-COD-LOT-CEF AND SITUACAO < '1' END-EXEC. */

            var r6030_SELECT_V0LOTERICO01_DB_SELECT_1_Query1 = new R6030_SELECT_V0LOTERICO01_DB_SELECT_1_Query1()
            {
                V0LOT_COD_LOT_CEF = V0LOT_COD_LOT_CEF.ToString(),
                WS_NUM_APOLICE = WS_NUM_APOLICE.ToString(),
            };

            var executed_1 = R6030_SELECT_V0LOTERICO01_DB_SELECT_1_Query1.Execute(r6030_SELECT_V0LOTERICO01_DB_SELECT_1_Query1);
            if (executed_1 != null)
            {
                _.Move(executed_1.V0LOT_COD_LOT_FENAL, V0LOT_COD_LOT_FENAL);
            }


        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6030_SAIDA*/

        [StopWatch]
        /*" R6060-VER-ALTERACAO-FC-CONTA-SECTION */
        private void R6060_VER_ALTERACAO_FC_CONTA_SECTION()
        {
            /*" -2250- MOVE '0008' TO WNR-EXEC-SQL. */
            _.Move("0008", LK_CONVERSAO.WABEND.WNR_EXEC_SQL);

            /*" -2254- MOVE ZEROS TO WS-IDE-CONTA-CPMF WS-IDE-CONTA-ISENTA WS-IDE-CONTA-CAUCAO. */
            _.Move(0, WS_IDE_CONTA_CPMF, WS_IDE_CONTA_ISENTA, WS_IDE_CONTA_CAUCAO);

            /*" -2257- IF CAD-BANCO-DESC-CPMF EQUAL ZEROS OR CAD-AGEN-DESC-CPMF EQUAL ZEROS OR CAD-CONTA-DESC-CPMF EQUAL ZEROS */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_DESC_CPMFX.CAD_BANCO_DESC_CPMF == 00 || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_DESC_CPMFX.CAD_AGEN_DESC_CPMF == 00 || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_DESC_CPMFX.CAD_CONTA_DESC_CPMF == 00)
            {

                /*" -2258- GO TO R6060-CONTA-ISENTA */

                R6060_CONTA_ISENTA(); //GOTO
                return;

                /*" -2260- END-IF */
            }


            /*" -2262- MOVE FCLOTERI-IDE-CONTA-CPMF TO FCCONBAN-IDE-CONTA-BANCARIA WS-IDE-CONTA-CPMF. */
            _.Move(FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IDE_CONTA_CPMF, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA, WS_IDE_CONTA_CPMF);

            /*" -2264- PERFORM R6070-LER-CONTA-BANCARIA. */

            R6070_LER_CONTA_BANCARIA_SECTION();

            /*" -2270- MOVE CAD-CONTA-DESC-CPMF TO WS1-CONTA-BANCARIA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_DESC_CPMFX.CAD_CONTA_DESC_CPMF, AREA_DE_WORK.WS1_CONTA_BANCARIA);

            /*" -2275- IF CAD-BANCO-DESC-CPMF EQUAL WS-CONBAN-BANCO AND CAD-AGEN-DESC-CPMF EQUAL WS-CONBAN-AGENCIA AND WS1-OPERACAO-CONTA EQUAL WS-CONBAN-OP AND WS1-NUMERO-CONTA EQUAL WS-CONBAN-CONTA AND WS1-DV-CONTA EQUAL WS-CONBAN-DV */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_DESC_CPMFX.CAD_BANCO_DESC_CPMF == AREA_DE_WORK.WS_CONBAN_BANCO && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_DESC_CPMFX.CAD_AGEN_DESC_CPMF == AREA_DE_WORK.WS_CONBAN_AGENCIA && AREA_DE_WORK.WS1_CONTA_BANCARIA_R.WS1_OPERACAO_CONTA == AREA_DE_WORK.WS_CONBAN_OP && AREA_DE_WORK.WS1_CONTA_BANCARIA_R.WS1_NUMERO_CONTA == AREA_DE_WORK.WS_CONBAN_CONTA && AREA_DE_WORK.WS1_CONTA_BANCARIA_R.WS1_DV_CONTA == AREA_DE_WORK.WS_CONBAN_DV)
            {

                /*" -2276- GO TO R6060-CONTA-ISENTA */

                R6060_CONTA_ISENTA(); //GOTO
                return;

                /*" -2278- END-IF */
            }


            /*" -2280- IF W-CHAVE-CADASTRADO-SASSE EQUAL 'SIM' AND WS-NECESSARIO = 0 */

            if (W_CHAVE_CADASTRADO_SASSE == "SIM" && WS_NECESSARIO == 0)
            {

                /*" -2281- MOVE 'SIM' TO W-CHAVE-ALTEROU-SEGURADO */
                _.Move("SIM", W_CHAVE_ALTEROU_SEGURADO);

                /*" -2282- MOVE 'S' TO LTMVPROP-IND-ALT-DADOS-PES */
                _.Move("S", LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_DADOS_PES);

                /*" -2284- END-IF */
            }


            /*" -2285- MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO */
            _.Move("SIM", W_CHAVE_HOUVE_ALTERACAO);

            /*" -2286- MOVE CAD-BANCO-DESC-CPMF TO WS-CODIGO-BANCO */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_DESC_CPMFX.CAD_BANCO_DESC_CPMF, AREA_DE_WORK.WS_CODIGO_BANCO);

            /*" -2287- MOVE CAD-AGEN-DESC-CPMF TO WS-CODIGO-AGENCIA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_DESC_CPMFX.CAD_AGEN_DESC_CPMF, AREA_DE_WORK.WS_CODIGO_AGENCIA);

            /*" -2290- MOVE CAD-CONTA-DESC-CPMF TO WS-CONTA-BANCARIA WS-CONTA-BANCO */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_DESC_CPMFX.CAD_CONTA_DESC_CPMF, AREA_DE_WORK.WS_CONTA_BANCARIA, AREA_DE_WORK.WS_CONTA_BANCO);

            /*" -2293- MOVE WS-NUMERO-CONTA TO WS-CONTA-12POS. */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_NUMERO_CONTA, AREA_DE_WORK.WS_CONTA_12POS);

            /*" -2295- MOVE WS-CODIGO-BANCO TO FCCONBAN-COD-BANCO */
            _.Move(AREA_DE_WORK.WS_CODIGO_BANCO, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_BANCO);

            /*" -2297- MOVE WS-CODIGO-AGENCIA TO FCCONBAN-COD-AGENCIA */
            _.Move(AREA_DE_WORK.WS_CODIGO_AGENCIA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_AGENCIA);

            /*" -2299- MOVE WS-CONTA-12POS TO FCCONBAN-COD-CONTA */
            _.Move(AREA_DE_WORK.WS_CONTA_12POS, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_CONTA);

            /*" -2300- MOVE WS-OPERA-CONTA TO FCCONBAN-COD-OP-CONTA */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCO_R.WS_OPERA_CONTA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_OP_CONTA);

            /*" -2301- MOVE WS-DV-CONTA TO FCCONBAN-COD-DV-CONTA */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_DV_CONTA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_DV_CONTA);

            /*" -2303- PERFORM R6080-VER-CONTA-EXISTENTE. */

            R6080_VER_CONTA_EXISTENTE_SECTION();

            /*" -2304- IF FCCONBAN-IDE-CONTA-BANCARIA > ZEROS */

            if (FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA > 00)
            {

                /*" -2305- MOVE FCCONBAN-IDE-CONTA-BANCARIA TO WS-IDE-CONTA-CPMF */
                _.Move(FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA, WS_IDE_CONTA_CPMF);

                /*" -2306- ELSE */
            }
            else
            {


                /*" -2307- PERFORM R6230-INSERT-FC-CONTA */

                R6230_INSERT_FC_CONTA_SECTION();

                /*" -2308- MOVE MAX-IDE-CONTA-BANCARIA TO WS-IDE-CONTA-CPMF */
                _.Move(MAX_IDE_CONTA_BANCARIA, WS_IDE_CONTA_CPMF);

                /*" -2309- END-IF */
            }


            /*" -2309- . */

            /*" -0- FLUXCONTROL_PERFORM R6060_CONTA_ISENTA */

            R6060_CONTA_ISENTA();

        }

        [StopWatch]
        /*" R6060-CONTA-ISENTA */
        private void R6060_CONTA_ISENTA(bool isPerform = false)
        {
            /*" -2315- IF CAD-BANCO-ISENTA EQUAL ZEROS OR CAD-AGEN-ISENTA EQUAL ZEROS OR CAD-CONTA-ISENTA EQUAL ZEROS */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_ISENTAX.CAD_BANCO_ISENTA == 00 || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_ISENTAX.CAD_AGEN_ISENTA == 00 || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_ISENTAX.CAD_CONTA_ISENTA == 00)
            {

                /*" -2316- GO TO R6060-CONTA-CAUCAO */

                R6060_CONTA_CAUCAO(); //GOTO
                return;

                /*" -2318- END-IF */
            }


            /*" -2320- MOVE FCLOTERI-IDE-CONTA-ISENTA TO FCCONBAN-IDE-CONTA-BANCARIA WS-IDE-CONTA-ISENTA. */
            _.Move(FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IDE_CONTA_ISENTA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA, WS_IDE_CONTA_ISENTA);

            /*" -2322- PERFORM R6070-LER-CONTA-BANCARIA. */

            R6070_LER_CONTA_BANCARIA_SECTION();

            /*" -2328- MOVE CAD-CONTA-ISENTA TO WS1-CONTA-BANCARIA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_ISENTAX.CAD_CONTA_ISENTA, AREA_DE_WORK.WS1_CONTA_BANCARIA);

            /*" -2333- IF CAD-BANCO-ISENTA EQUAL WS-CONBAN-BANCO AND CAD-AGEN-ISENTA EQUAL WS-CONBAN-AGENCIA AND WS1-OPERACAO-CONTA EQUAL WS-CONBAN-OP AND WS1-NUMERO-CONTA EQUAL WS-CONBAN-CONTA AND WS1-DV-CONTA EQUAL WS-CONBAN-DV */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_ISENTAX.CAD_BANCO_ISENTA == AREA_DE_WORK.WS_CONBAN_BANCO && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_ISENTAX.CAD_AGEN_ISENTA == AREA_DE_WORK.WS_CONBAN_AGENCIA && AREA_DE_WORK.WS1_CONTA_BANCARIA_R.WS1_OPERACAO_CONTA == AREA_DE_WORK.WS_CONBAN_OP && AREA_DE_WORK.WS1_CONTA_BANCARIA_R.WS1_NUMERO_CONTA == AREA_DE_WORK.WS_CONBAN_CONTA && AREA_DE_WORK.WS1_CONTA_BANCARIA_R.WS1_DV_CONTA == AREA_DE_WORK.WS_CONBAN_DV)
            {

                /*" -2334- GO TO R6060-CONTA-CAUCAO */

                R6060_CONTA_CAUCAO(); //GOTO
                return;

                /*" -2336- END-IF */
            }


            /*" -2337- MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO */
            _.Move("SIM", W_CHAVE_HOUVE_ALTERACAO);

            /*" -2338- MOVE CAD-BANCO-ISENTA TO WS-CODIGO-BANCO */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_ISENTAX.CAD_BANCO_ISENTA, AREA_DE_WORK.WS_CODIGO_BANCO);

            /*" -2339- MOVE CAD-AGEN-ISENTA TO WS-CODIGO-AGENCIA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_ISENTAX.CAD_AGEN_ISENTA, AREA_DE_WORK.WS_CODIGO_AGENCIA);

            /*" -2342- MOVE CAD-CONTA-ISENTA TO WS-CONTA-BANCARIA WS-CONTA-BANCO */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_ISENTAX.CAD_CONTA_ISENTA, AREA_DE_WORK.WS_CONTA_BANCARIA, AREA_DE_WORK.WS_CONTA_BANCO);

            /*" -2345- MOVE WS-NUMERO-CONTA TO WS-CONTA-12POS. */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_NUMERO_CONTA, AREA_DE_WORK.WS_CONTA_12POS);

            /*" -2347- MOVE WS-CODIGO-BANCO TO FCCONBAN-COD-BANCO */
            _.Move(AREA_DE_WORK.WS_CODIGO_BANCO, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_BANCO);

            /*" -2349- MOVE WS-CODIGO-AGENCIA TO FCCONBAN-COD-AGENCIA */
            _.Move(AREA_DE_WORK.WS_CODIGO_AGENCIA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_AGENCIA);

            /*" -2351- MOVE WS-CONTA-12POS TO FCCONBAN-COD-CONTA */
            _.Move(AREA_DE_WORK.WS_CONTA_12POS, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_CONTA);

            /*" -2352- MOVE WS-OPERA-CONTA TO FCCONBAN-COD-OP-CONTA */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCO_R.WS_OPERA_CONTA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_OP_CONTA);

            /*" -2353- MOVE WS-DV-CONTA TO FCCONBAN-COD-DV-CONTA */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_DV_CONTA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_DV_CONTA);

            /*" -2355- PERFORM R6080-VER-CONTA-EXISTENTE. */

            R6080_VER_CONTA_EXISTENTE_SECTION();

            /*" -2356- IF FCCONBAN-IDE-CONTA-BANCARIA > ZEROS */

            if (FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA > 00)
            {

                /*" -2357- MOVE FCCONBAN-IDE-CONTA-BANCARIA TO WS-IDE-CONTA-ISENTA */
                _.Move(FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA, WS_IDE_CONTA_ISENTA);

                /*" -2358- ELSE */
            }
            else
            {


                /*" -2359- PERFORM R6230-INSERT-FC-CONTA */

                R6230_INSERT_FC_CONTA_SECTION();

                /*" -2360- MOVE MAX-IDE-CONTA-BANCARIA TO WS-IDE-CONTA-ISENTA */
                _.Move(MAX_IDE_CONTA_BANCARIA, WS_IDE_CONTA_ISENTA);

                /*" -2361- END-IF */
            }


            /*" -2361- . */

        }

        [StopWatch]
        /*" R6060-CONTA-CAUCAO */
        private void R6060_CONTA_CAUCAO(bool isPerform = false)
        {
            /*" -2367- IF CAD-BANCO-CAUCAO EQUAL ZEROS OR CAD-AGEN-CAUCAO EQUAL ZEROS OR CAD-CONTA-CAUCAO EQUAL ZEROS */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_CAUCAOX.CAD_BANCO_CAUCAO == 00 || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_CAUCAOX.CAD_AGEN_CAUCAO == 00 || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_CAUCAOX.CAD_CONTA_CAUCAO == 00)
            {

                /*" -2369- GO TO R6060-SAIDA. */
                /*Método Suprimido por falta de linha ou apenas EXIT nome: R6060_SAIDA*/ //GOTO
                return;
            }


            /*" -2371- MOVE FCLOTERI-IDE-CONTA-CAUCAO TO FCCONBAN-IDE-CONTA-BANCARIA WS-IDE-CONTA-CAUCAO. */
            _.Move(FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IDE_CONTA_CAUCAO, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA, WS_IDE_CONTA_CAUCAO);

            /*" -2373- PERFORM R6070-LER-CONTA-BANCARIA. */

            R6070_LER_CONTA_BANCARIA_SECTION();

            /*" -2379- MOVE CAD-CONTA-CAUCAO TO WS1-CONTA-BANCARIA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_CAUCAOX.CAD_CONTA_CAUCAO, AREA_DE_WORK.WS1_CONTA_BANCARIA);

            /*" -2384- IF CAD-BANCO-CAUCAO EQUAL WS-CONBAN-BANCO AND CAD-AGEN-CAUCAO EQUAL WS-CONBAN-AGENCIA AND WS1-OPERACAO-CONTA EQUAL WS-CONBAN-OP AND WS1-NUMERO-CONTA EQUAL WS-CONBAN-CONTA AND WS1-DV-CONTA EQUAL WS-CONBAN-DV */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_CAUCAOX.CAD_BANCO_CAUCAO == AREA_DE_WORK.WS_CONBAN_BANCO && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_CAUCAOX.CAD_AGEN_CAUCAO == AREA_DE_WORK.WS_CONBAN_AGENCIA && AREA_DE_WORK.WS1_CONTA_BANCARIA_R.WS1_OPERACAO_CONTA == AREA_DE_WORK.WS_CONBAN_OP && AREA_DE_WORK.WS1_CONTA_BANCARIA_R.WS1_NUMERO_CONTA == AREA_DE_WORK.WS_CONBAN_CONTA && AREA_DE_WORK.WS1_CONTA_BANCARIA_R.WS1_DV_CONTA == AREA_DE_WORK.WS_CONBAN_DV)
            {

                /*" -2385- GO TO R6060-SAIDA */
                /*Método Suprimido por falta de linha ou apenas EXIT nome: R6060_SAIDA*/ //GOTO
                return;

                /*" -2387- END-IF */
            }


            /*" -2388- MOVE CAD-BANCO-CAUCAO TO WS-CODIGO-BANCO */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_CAUCAOX.CAD_BANCO_CAUCAO, AREA_DE_WORK.WS_CODIGO_BANCO);

            /*" -2389- MOVE CAD-AGEN-CAUCAO TO WS-CODIGO-AGENCIA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_CAUCAOX.CAD_AGEN_CAUCAO, AREA_DE_WORK.WS_CODIGO_AGENCIA);

            /*" -2392- MOVE CAD-CONTA-CAUCAO TO WS-CONTA-BANCARIA WS-CONTA-BANCO */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_CAUCAOX.CAD_CONTA_CAUCAO, AREA_DE_WORK.WS_CONTA_BANCARIA, AREA_DE_WORK.WS_CONTA_BANCO);

            /*" -2394- MOVE WS-NUMERO-CONTA TO WS-CONTA-12POS. */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_NUMERO_CONTA, AREA_DE_WORK.WS_CONTA_12POS);

            /*" -2396- MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO */
            _.Move("SIM", W_CHAVE_HOUVE_ALTERACAO);

            /*" -2398- MOVE WS-CODIGO-BANCO TO FCCONBAN-COD-BANCO */
            _.Move(AREA_DE_WORK.WS_CODIGO_BANCO, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_BANCO);

            /*" -2400- MOVE WS-CODIGO-AGENCIA TO FCCONBAN-COD-AGENCIA */
            _.Move(AREA_DE_WORK.WS_CODIGO_AGENCIA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_AGENCIA);

            /*" -2402- MOVE WS-CONTA-12POS TO FCCONBAN-COD-CONTA */
            _.Move(AREA_DE_WORK.WS_CONTA_12POS, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_CONTA);

            /*" -2403- MOVE WS-OPERA-CONTA TO FCCONBAN-COD-OP-CONTA */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCO_R.WS_OPERA_CONTA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_OP_CONTA);

            /*" -2404- MOVE WS-DV-CONTA TO FCCONBAN-COD-DV-CONTA */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_DV_CONTA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_DV_CONTA);

            /*" -2406- PERFORM R6080-VER-CONTA-EXISTENTE. */

            R6080_VER_CONTA_EXISTENTE_SECTION();

            /*" -2407- IF FCCONBAN-IDE-CONTA-BANCARIA > ZEROS */

            if (FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA > 00)
            {

                /*" -2408- MOVE FCCONBAN-IDE-CONTA-BANCARIA TO WS-IDE-CONTA-CAUCAO */
                _.Move(FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA, WS_IDE_CONTA_CAUCAO);

                /*" -2409- ELSE */
            }
            else
            {


                /*" -2410- PERFORM R6230-INSERT-FC-CONTA */

                R6230_INSERT_FC_CONTA_SECTION();

                /*" -2411- MOVE MAX-IDE-CONTA-BANCARIA TO WS-IDE-CONTA-CAUCAO */
                _.Move(MAX_IDE_CONTA_BANCARIA, WS_IDE_CONTA_CAUCAO);

                /*" -2412- END-IF */
            }


            /*" -2412- . */

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6060_SAIDA*/

        [StopWatch]
        /*" R6070-LER-CONTA-BANCARIA-SECTION */
        private void R6070_LER_CONTA_BANCARIA_SECTION()
        {
            /*" -2425- MOVE ZEROS TO WS-CONBAN-BANCO WS-CONBAN-AGENCIA WS-CONBAN-OP WS-CONBAN-CONTA. */
            _.Move(0, AREA_DE_WORK.WS_CONBAN_BANCO, AREA_DE_WORK.WS_CONBAN_AGENCIA, AREA_DE_WORK.WS_CONBAN_OP, AREA_DE_WORK.WS_CONBAN_CONTA);

            /*" -2427- MOVE SPACES TO WS-CONBAN-DV . */
            _.Move("", AREA_DE_WORK.WS_CONBAN_DV);

            /*" -2428- IF FCCONBAN-IDE-CONTA-BANCARIA = ZEROS */

            if (FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA == 00)
            {

                /*" -2433- MOVE ZEROS TO FCCONBAN-COD-AGENCIA FCCONBAN-COD-BANCO FCCONBAN-COD-CONTA FCCONBAN-COD-DV-CONTA FCCONBAN-COD-OP-CONTA */
                _.Move(0, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_AGENCIA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_BANCO, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_CONTA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_DV_CONTA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_OP_CONTA);

                /*" -2435- GO TO R6070-SAIDA. */
                /*Método Suprimido por falta de linha ou apenas EXIT nome: R6070_SAIDA*/ //GOTO
                return;
            }


            /*" -2453- PERFORM R6070_LER_CONTA_BANCARIA_DB_SELECT_1 */

            R6070_LER_CONTA_BANCARIA_DB_SELECT_1();

            /*" -2456- IF SQLCODE NOT EQUAL ZEROS */

            if (DB.SQLCODE != 00)
            {

                /*" -2457- DISPLAY ' 6070-ERRO LER CONTA BAN LOT=' CAD-COD-CEF */
                _.Display($" 6070-ERRO LER CONTA BAN LOT={AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF_R.CAD_COD_CEF}");

                /*" -2463- DISPLAY ' AGEN=' FCCONBAN-COD-AGENCIA ' BANCO=' FCCONBAN-COD-BANCO ' CONTA=' FCCONBAN-COD-CONTA ' DV=' FCCONBAN-COD-DV-CONTA ' OP=' FCCONBAN-COD-OP-CONTA ' IDE= ' FCCONBAN-IDE-CONTA-BANCARIA */

                $" AGEN={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_AGENCIA} BANCO={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_BANCO} CONTA={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_CONTA} DV={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_DV_CONTA} OP={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_OP_CONTA} IDE= {FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA}"
                .Display();

                /*" -2465- GO TO R9999-ROT-ERRO. */

                R9999_ROT_ERRO_SECTION(); //GOTO
                return;
            }


            /*" -2466- IF FCCONBAN-COD-TIPO-CONTA NOT = 'LOT' */

            if (FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_TIPO_CONTA != "LOT")
            {

                /*" -2469- DISPLAY ' 6070-ENCONTRADO TIPO CTA BANCARIA DIF. LOT ' FCCONBAN-IDE-CONTA-BANCARIA '  ' CAD-COD-CEF */

                $" 6070-ENCONTRADO TIPO CTA BANCARIA DIF. LOT {FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA}  {AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF_R.CAD_COD_CEF}"
                .Display();

                /*" -2476- MOVE ZEROS TO FCCONBAN-COD-AGENCIA FCCONBAN-COD-BANCO FCCONBAN-COD-CONTA FCCONBAN-COD-DV-CONTA FCCONBAN-COD-OP-CONTA. */
                _.Move(0, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_AGENCIA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_BANCO, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_CONTA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_DV_CONTA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_OP_CONTA);
            }


            /*" -2477- MOVE FCCONBAN-COD-BANCO TO WS-CONBAN-BANCO */
            _.Move(FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_BANCO, AREA_DE_WORK.WS_CONBAN_BANCO);

            /*" -2478- MOVE FCCONBAN-COD-AGENCIA TO WS-CONBAN-AGENCIA */
            _.Move(FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_AGENCIA, AREA_DE_WORK.WS_CONBAN_AGENCIA);

            /*" -2479- MOVE FCCONBAN-COD-OP-CONTA TO WS-CONBAN-OP */
            _.Move(FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_OP_CONTA, AREA_DE_WORK.WS_CONBAN_OP);

            /*" -2480- MOVE FCCONBAN-COD-CONTA TO WS-CONBAN-CONTA */
            _.Move(FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_CONTA, AREA_DE_WORK.WS_CONBAN_CONTA);

            /*" -2481- MOVE FCCONBAN-COD-DV-CONTA TO WS-CONBAN-DV */
            _.Move(FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_DV_CONTA, AREA_DE_WORK.WS_CONBAN_DV);

            /*" -2481- . */

        }

        [StopWatch]
        /*" R6070-LER-CONTA-BANCARIA-DB-SELECT-1 */
        public void R6070_LER_CONTA_BANCARIA_DB_SELECT_1()
        {
            /*" -2453- EXEC SQL SELECT IDE_CONTA_BANCARIA, COD_AGENCIA, COD_BANCO, COD_CONTA, COD_DV_CONTA, COD_OP_CONTA, COD_TIPO_CONTA INTO :FCCONBAN-IDE-CONTA-BANCARIA, :FCCONBAN-COD-AGENCIA, :FCCONBAN-COD-BANCO, :FCCONBAN-COD-CONTA, :FCCONBAN-COD-DV-CONTA, :FCCONBAN-COD-OP-CONTA, :FCCONBAN-COD-TIPO-CONTA FROM FDRCAP.FC_CONTA_BANCARIA WHERE IDE_CONTA_BANCARIA =:FCCONBAN-IDE-CONTA-BANCARIA END-EXEC. */

            var r6070_LER_CONTA_BANCARIA_DB_SELECT_1_Query1 = new R6070_LER_CONTA_BANCARIA_DB_SELECT_1_Query1()
            {
                FCCONBAN_IDE_CONTA_BANCARIA = FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA.ToString(),
            };

            var executed_1 = R6070_LER_CONTA_BANCARIA_DB_SELECT_1_Query1.Execute(r6070_LER_CONTA_BANCARIA_DB_SELECT_1_Query1);
            if (executed_1 != null)
            {
                _.Move(executed_1.FCCONBAN_IDE_CONTA_BANCARIA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA);
                _.Move(executed_1.FCCONBAN_COD_AGENCIA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_AGENCIA);
                _.Move(executed_1.FCCONBAN_COD_BANCO, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_BANCO);
                _.Move(executed_1.FCCONBAN_COD_CONTA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_CONTA);
                _.Move(executed_1.FCCONBAN_COD_DV_CONTA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_DV_CONTA);
                _.Move(executed_1.FCCONBAN_COD_OP_CONTA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_OP_CONTA);
                _.Move(executed_1.FCCONBAN_COD_TIPO_CONTA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_TIPO_CONTA);
            }


        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6070_SAIDA*/

        [StopWatch]
        /*" R6080-VER-CONTA-EXISTENTE-SECTION */
        private void R6080_VER_CONTA_EXISTENTE_SECTION()
        {
            /*" -2500- PERFORM R6080_VER_CONTA_EXISTENTE_DB_SELECT_1 */

            R6080_VER_CONTA_EXISTENTE_DB_SELECT_1();

            /*" -2503- IF SQLCODE NOT EQUAL ZEROS */

            if (DB.SQLCODE != 00)
            {

                /*" -2504- IF SQLCODE EQUAL 100 */

                if (DB.SQLCODE == 100)
                {

                    /*" -2505- MOVE ZEROS TO FCCONBAN-IDE-CONTA-BANCARIA */
                    _.Move(0, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA);

                    /*" -2506- ELSE */
                }
                else
                {


                    /*" -2507- DISPLAY ' 6070-ERRO LER CONTA BAN LOT=' CAD-COD-CEF */
                    _.Display($" 6070-ERRO LER CONTA BAN LOT={AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF_R.CAD_COD_CEF}");

                    /*" -2513- DISPLAY ' AGEN=' FCCONBAN-COD-AGENCIA ' BANCO=' FCCONBAN-COD-BANCO ' CONTA=' FCCONBAN-COD-CONTA ' DV=' FCCONBAN-COD-DV-CONTA ' OP=' FCCONBAN-COD-OP-CONTA ' IDE= ' FCCONBAN-IDE-CONTA-BANCARIA */

                    $" AGEN={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_AGENCIA} BANCO={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_BANCO} CONTA={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_CONTA} DV={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_DV_CONTA} OP={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_OP_CONTA} IDE= {FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA}"
                    .Display();

                    /*" -2513- GO TO R9999-ROT-ERRO. */

                    R9999_ROT_ERRO_SECTION(); //GOTO
                    return;
                }

            }


        }

        [StopWatch]
        /*" R6080-VER-CONTA-EXISTENTE-DB-SELECT-1 */
        public void R6080_VER_CONTA_EXISTENTE_DB_SELECT_1()
        {
            /*" -2500- EXEC SQL SELECT IDE_CONTA_BANCARIA INTO :FCCONBAN-IDE-CONTA-BANCARIA FROM FDRCAP.FC_CONTA_BANCARIA WHERE COD_BANCO = :FCCONBAN-COD-BANCO AND COD_AGENCIA = :FCCONBAN-COD-AGENCIA AND COD_CONTA = :FCCONBAN-COD-CONTA AND COD_OP_CONTA = :FCCONBAN-COD-OP-CONTA AND COD_DV_CONTA = :FCCONBAN-COD-DV-CONTA AND COD_TIPO_CONTA = 'LOT' END-EXEC. */

            var r6080_VER_CONTA_EXISTENTE_DB_SELECT_1_Query1 = new R6080_VER_CONTA_EXISTENTE_DB_SELECT_1_Query1()
            {
                FCCONBAN_COD_OP_CONTA = FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_OP_CONTA.ToString(),
                FCCONBAN_COD_DV_CONTA = FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_DV_CONTA.ToString(),
                FCCONBAN_COD_AGENCIA = FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_AGENCIA.ToString(),
                FCCONBAN_COD_BANCO = FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_BANCO.ToString(),
                FCCONBAN_COD_CONTA = FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_CONTA.ToString(),
            };

            var executed_1 = R6080_VER_CONTA_EXISTENTE_DB_SELECT_1_Query1.Execute(r6080_VER_CONTA_EXISTENTE_DB_SELECT_1_Query1);
            if (executed_1 != null)
            {
                _.Move(executed_1.FCCONBAN_IDE_CONTA_BANCARIA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA);
            }


        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6080_SAIDA*/

        [StopWatch]
        /*" R6200-MONTAR-FC-LOTERICO-SECTION */
        private void R6200_MONTAR_FC_LOTERICO_SECTION()
        {
            /*" -2559- MOVE 0 TO VIND-AGENTE-MASTER VIND-COD-CGC VIND-COD-INSCR-ESTAD VIND-COD-INSCR-MUNIC VIND-COD-MUNICIPIO VIND-COD-UF VIND-DES-EMAIL VIND-DES-ENDERECO VIND-DTH-EXCLUSAO VIND-DTH-INCLUSAO VIND-IDE-CONTA-CAUCAO VIND-IDE-CONTA-CPMF VIND-IDE-CONTA-ISENTA VIND-IND-CAT-LOTERICO VIND-IND-STA-LOTERICO VIND-IND-UNIDADE-SUB VIND-NOM-BAIRRO VIND-NOM-CONSULTOR VIND-NOM-CONTATO1 VIND-NOM-CONTATO2 VIND-NOM-FANTASIA VIND-NOM-MUNICIPIO VIND-NOM-RAZAO-SOCIAL VIND-NUM-CEP VIND-NUM-ENCEF VIND-NUM-LOTER-ANT VIND-NUM-MATR-CON VIND-NUM-PVCEF VIND-NUM-TELEFONE VIND-STA-DADOS-N VIND-STA-LOTERICO VIND-STA-NIVEL-COMIS VIND-STA-ULT-ALT VIND-COD-GARANTIA VIND-VLR-GARANTIA VIND-NUM-FAX. */
            _.Move(0, VIND_AGENTE_MASTER, VIND_COD_CGC, VIND_COD_INSCR_ESTAD, VIND_COD_INSCR_MUNIC, VIND_COD_MUNICIPIO, VIND_COD_UF, VIND_DES_EMAIL, VIND_DES_ENDERECO, VIND_DTH_EXCLUSAO, VIND_DTH_INCLUSAO, VIND_IDE_CONTA_CAUCAO, VIND_IDE_CONTA_CPMF, VIND_IDE_CONTA_ISENTA, VIND_IND_CAT_LOTERICO, VIND_IND_STA_LOTERICO, VIND_IND_UNIDADE_SUB, VIND_NOM_BAIRRO, VIND_NOM_CONSULTOR, VIND_NOM_CONTATO1, VIND_NOM_CONTATO2, VIND_NOM_FANTASIA, VIND_NOM_MUNICIPIO, VIND_NOM_RAZAO_SOCIAL, VIND_NUM_CEP, VIND_NUM_ENCEF, VIND_NUM_LOTER_ANT, VIND_NUM_MATR_CON, VIND_NUM_PVCEF, VIND_NUM_TELEFONE, VIND_STA_DADOS_N, VIND_STA_LOTERICO, VIND_STA_NIVEL_COMIS, VIND_STA_ULT_ALT, VIND_COD_GARANTIA, VIND_VLR_GARANTIA, VIND_NUM_FAX);

            /*" -2560- MOVE CAD-COD-CEF TO FCLOTERI-NUM-LOTERICO. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF_R.CAD_COD_CEF, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_LOTERICO);

            /*" -2562- MOVE CAD-DV-CEF TO FCLOTERI-NUM-DV-LOTERICO. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF_R.CAD_DV_CEF, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_DV_LOTERICO);

            /*" -2563- IF CAD-COD-AG-MASTER IS NUMERIC AND CAD-COD-AG-MASTER > 0 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_AG_MASTERX.CAD_COD_AG_MASTER.IsNumeric() && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_AG_MASTERX.CAD_COD_AG_MASTER > 0)
            {

                /*" -2564- MOVE CAD-COD-AG-MASTERX TO FCLOTERI-COD-AGENTE-MASTER */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_AG_MASTERX, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_AGENTE_MASTER);

                /*" -2565- ELSE */
            }
            else
            {


                /*" -2567- MOVE -1 TO VIND-AGENTE-MASTER. */
                _.Move(-1, VIND_AGENTE_MASTER);
            }


            /*" -2568- IF CAD-CGC IS NUMERIC AND CAD-CGC > 0 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CGCX.CAD_CGC.IsNumeric() && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CGCX.CAD_CGC > 0)
            {

                /*" -2569- MOVE CAD-CGCX TO FCLOTERI-COD-CGC */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CGCX, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_CGC);

                /*" -2570- ELSE */
            }
            else
            {


                /*" -2572- MOVE -1 TO VIND-COD-CGC. */
                _.Move(-1, VIND_COD_CGC);
            }


            /*" -2573- IF CAD-INSC-ESTX NOT = SPACES */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_INSC_ESTX.IsEmpty())
            {

                /*" -2574- MOVE CAD-INSC-ESTX TO FCLOTERI-COD-INSCR-ESTAD */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_INSC_ESTX, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_INSCR_ESTAD);

                /*" -2575- ELSE */
            }
            else
            {


                /*" -2577- MOVE -1 TO VIND-COD-INSCR-ESTAD. */
                _.Move(-1, VIND_COD_INSCR_ESTAD);
            }


            /*" -2578- IF CAD-INSC-MUNX NOT = SPACES */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_INSC_MUNX.IsEmpty())
            {

                /*" -2579- MOVE CAD-INSC-MUNX TO FCLOTERI-COD-INSCR-MUNIC */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_INSC_MUNX, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_INSCR_MUNIC);

                /*" -2580- ELSE */
            }
            else
            {


                /*" -2582- MOVE -1 TO VIND-COD-INSCR-MUNIC. */
                _.Move(-1, VIND_COD_INSCR_MUNIC);
            }


            /*" -2583- IF CAD-COD-MUNICIPIO NOT EQUAL SPACES */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_MUNICIPIO.IsEmpty())
            {

                /*" -2584- MOVE CAD-COD-MUNICIPIO TO FCLOTERI-COD-MUNICIPIO */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_MUNICIPIO, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_MUNICIPIO);

                /*" -2585- ELSE */
            }
            else
            {


                /*" -2587- MOVE -1 TO VIND-COD-MUNICIPIO. */
                _.Move(-1, VIND_COD_MUNICIPIO);
            }


            /*" -2588- IF CAD-UF NOT EQUAL SPACES */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_UF.IsEmpty())
            {

                /*" -2589- MOVE CAD-UF TO FCLOTERI-COD-UF */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_UF, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_UF);

                /*" -2590- ELSE */
            }
            else
            {


                /*" -2592- MOVE -1 TO VIND-COD-UF. */
                _.Move(-1, VIND_COD_UF);
            }


            /*" -2593- IF CAD-EMAIL NOT EQUAL SPACES */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_EMAILX.CAD_EMAIL.IsEmpty())
            {

                /*" -2594- MOVE CAD-EMAIL TO FCLOTERI-DES-EMAIL */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_EMAILX.CAD_EMAIL, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DES_EMAIL);

                /*" -2595- ELSE */
            }
            else
            {


                /*" -2597- MOVE -1 TO VIND-DES-EMAIL. */
                _.Move(-1, VIND_DES_EMAIL);
            }


            /*" -2598- IF CAD-ENDERECO NOT EQUAL SPACES */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_ENDERECO.IsEmpty())
            {

                /*" -2599- MOVE CAD-ENDERECO TO FCLOTERI-DES-ENDERECO */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_ENDERECO, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DES_ENDERECO);

                /*" -2600- ELSE */
            }
            else
            {


                /*" -2602- MOVE -1 TO VIND-DES-ENDERECO. */
                _.Move(-1, VIND_DES_ENDERECO);
            }


            /*" -2603- IF CAD-DATA-INCLUSAO > 0 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_DATA_INCLUSAOX.CAD_DATA_INCLUSAO > 0)
            {

                /*" -2604- MOVE W-CAD-DATA-INI-VIG TO FCLOTERI-DTH-INCLUSAO */
                _.Move(AREA_DE_WORK.W_CAD_DATA_INI_VIG, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DTH_INCLUSAO);

                /*" -2605- ELSE */
            }
            else
            {


                /*" -2606- MOVE SPACES TO FCLOTERI-DTH-INCLUSAO */
                _.Move("", FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DTH_INCLUSAO);

                /*" -2608- MOVE -1 TO VIND-DTH-INCLUSAO. */
                _.Move(-1, VIND_DTH_INCLUSAO);
            }


            /*" -2609- IF CAD-DATA-EXCLUSAO > 0 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_DATA_EXCLUSAOX.CAD_DATA_EXCLUSAO > 0)
            {

                /*" -2610- MOVE W-CAD-DATA-TER-VIG TO FCLOTERI-DTH-EXCLUSAO */
                _.Move(AREA_DE_WORK.W_CAD_DATA_TER_VIG, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DTH_EXCLUSAO);

                /*" -2611- ELSE */
            }
            else
            {


                /*" -2612- MOVE SPACES TO FCLOTERI-DTH-EXCLUSAO */
                _.Move("", FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DTH_EXCLUSAO);

                /*" -2614- MOVE -1 TO VIND-DTH-EXCLUSAO. */
                _.Move(-1, VIND_DTH_EXCLUSAO);
            }


            /*" -2615- IF WS-IDE-CONTA-CAUCAO > 0 */

            if (WS_IDE_CONTA_CAUCAO > 0)
            {

                /*" -2616- MOVE WS-IDE-CONTA-CAUCAO TO FCLOTERI-IDE-CONTA-CAUCAO */
                _.Move(WS_IDE_CONTA_CAUCAO, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IDE_CONTA_CAUCAO);

                /*" -2617- ELSE */
            }
            else
            {


                /*" -2619- MOVE -1 TO VIND-IDE-CONTA-CAUCAO. */
                _.Move(-1, VIND_IDE_CONTA_CAUCAO);
            }


            /*" -2620- IF WS-IDE-CONTA-CPMF > 0 */

            if (WS_IDE_CONTA_CPMF > 0)
            {

                /*" -2621- MOVE WS-IDE-CONTA-CPMF TO FCLOTERI-IDE-CONTA-CPMF */
                _.Move(WS_IDE_CONTA_CPMF, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IDE_CONTA_CPMF);

                /*" -2622- ELSE */
            }
            else
            {


                /*" -2624- MOVE -1 TO VIND-IDE-CONTA-CPMF. */
                _.Move(-1, VIND_IDE_CONTA_CPMF);
            }


            /*" -2625- IF WS-IDE-CONTA-ISENTA > 0 */

            if (WS_IDE_CONTA_ISENTA > 0)
            {

                /*" -2626- MOVE WS-IDE-CONTA-ISENTA TO FCLOTERI-IDE-CONTA-ISENTA */
                _.Move(WS_IDE_CONTA_ISENTA, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IDE_CONTA_ISENTA);

                /*" -2627- ELSE */
            }
            else
            {


                /*" -2629- MOVE -1 TO VIND-IDE-CONTA-ISENTA. */
                _.Move(-1, VIND_IDE_CONTA_ISENTA);
            }


            /*" -2631- IF CAD-CAT-LOTERICO IS NUMERIC AND CAD-CAT-LOTERICO > 0 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CAT_LOTERICOX.CAD_CAT_LOTERICO.IsNumeric() && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CAT_LOTERICOX.CAD_CAT_LOTERICO > 0)
            {

                /*" -2632- MOVE CAD-CAT-LOTERICO TO FCLOTERI-IND-CAT-LOTERICO */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CAT_LOTERICOX.CAD_CAT_LOTERICO, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IND_CAT_LOTERICO);

                /*" -2633- ELSE */
            }
            else
            {


                /*" -2635- MOVE -1 TO VIND-IND-CAT-LOTERICO. */
                _.Move(-1, VIND_IND_CAT_LOTERICO);
            }


            /*" -2637- IF CAD-COD-STATUS IS NUMERIC AND CAD-COD-STATUS > 0 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_STATUSX.CAD_COD_STATUS.IsNumeric() && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_STATUSX.CAD_COD_STATUS > 0)
            {

                /*" -2638- MOVE CAD-COD-STATUS TO FCLOTERI-IND-STA-LOTERICO */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_STATUSX.CAD_COD_STATUS, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IND_STA_LOTERICO);

                /*" -2639- ELSE */
            }
            else
            {


                /*" -2641- MOVE -1 TO VIND-IND-STA-LOTERICO. */
                _.Move(-1, VIND_IND_STA_LOTERICO);
            }


            /*" -2643- IF CAD-UNIDADE-SUB IS NUMERIC AND CAD-UNIDADE-SUB > 0 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_UNIDADE_SUBX.CAD_UNIDADE_SUB.IsNumeric() && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_UNIDADE_SUBX.CAD_UNIDADE_SUB > 0)
            {

                /*" -2644- MOVE CAD-UNIDADE-SUB TO FCLOTERI-IND-UNIDADE-SUB */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_UNIDADE_SUBX.CAD_UNIDADE_SUB, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IND_UNIDADE_SUB);

                /*" -2645- ELSE */
            }
            else
            {


                /*" -2647- MOVE -1 TO VIND-IND-UNIDADE-SUB. */
                _.Move(-1, VIND_IND_UNIDADE_SUB);
            }


            /*" -2648- IF CAD-BAIRRO NOT EQUAL SPACES */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BAIRRO.IsEmpty())
            {

                /*" -2649- MOVE CAD-BAIRRO TO FCLOTERI-NOM-BAIRRO */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BAIRRO, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_BAIRRO);

                /*" -2650- ELSE */
            }
            else
            {


                /*" -2652- MOVE -1 TO VIND-NOM-BAIRRO. */
                _.Move(-1, VIND_NOM_BAIRRO);
            }


            /*" -2653- IF CAD-NOME-CONSULTOR NOT EQUAL SPACES */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NOME_CONSULTORX.CAD_NOME_CONSULTOR.IsEmpty())
            {

                /*" -2654- MOVE CAD-NOME-CONSULTOR TO FCLOTERI-NOM-CONSULTOR */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NOME_CONSULTORX.CAD_NOME_CONSULTOR, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_CONSULTOR);

                /*" -2655- ELSE */
            }
            else
            {


                /*" -2657- MOVE -1 TO VIND-NOM-CONSULTOR. */
                _.Move(-1, VIND_NOM_CONSULTOR);
            }


            /*" -2658- IF CAD-CONTATO1 NOT EQUAL SPACES */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTATO1.IsEmpty())
            {

                /*" -2659- MOVE CAD-CONTATO1 TO FCLOTERI-NOM-CONTATO1 */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTATO1, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_CONTATO1);

                /*" -2660- ELSE */
            }
            else
            {


                /*" -2662- MOVE -1 TO VIND-NOM-CONTATO1. */
                _.Move(-1, VIND_NOM_CONTATO1);
            }


            /*" -2663- IF CAD-CONTATO2 NOT EQUAL SPACES */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTATO2.IsEmpty())
            {

                /*" -2664- MOVE CAD-CONTATO2 TO FCLOTERI-NOM-CONTATO2 */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTATO2, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_CONTATO2);

                /*" -2665- ELSE */
            }
            else
            {


                /*" -2667- MOVE -1 TO VIND-NOM-CONTATO2. */
                _.Move(-1, VIND_NOM_CONTATO2);
            }


            /*" -2668- IF CAD-NOME-FANTASIA NOT EQUAL SPACES */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NOME_FANTASIA.IsEmpty())
            {

                /*" -2669- MOVE CAD-NOME-FANTASIA TO FCLOTERI-NOM-FANTASIA */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NOME_FANTASIA, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_FANTASIA);

                /*" -2670- ELSE */
            }
            else
            {


                /*" -2672- MOVE -1 TO VIND-NOM-FANTASIA. */
                _.Move(-1, VIND_NOM_FANTASIA);
            }


            /*" -2673- IF CAD-CIDADE NOT EQUAL SPACES */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CIDADE.IsEmpty())
            {

                /*" -2674- MOVE CAD-CIDADE TO FCLOTERI-NOM-MUNICIPIO */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CIDADE, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_MUNICIPIO);

                /*" -2675- ELSE */
            }
            else
            {


                /*" -2677- MOVE -1 TO VIND-NOM-MUNICIPIO. */
                _.Move(-1, VIND_NOM_MUNICIPIO);
            }


            /*" -2678- IF CAD-RAZAO-SOCIAL NOT EQUAL SPACES */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_RAZAO_SOCIAL.IsEmpty())
            {

                /*" -2679- MOVE CAD-RAZAO-SOCIAL TO FCLOTERI-NOM-RAZAO-SOCIAL */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_RAZAO_SOCIAL, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_RAZAO_SOCIAL);

                /*" -2680- ELSE */
            }
            else
            {


                /*" -2682- MOVE -1 TO VIND-NOM-RAZAO-SOCIAL. */
                _.Move(-1, VIND_NOM_RAZAO_SOCIAL);
            }


            /*" -2684- IF CAD-CEP IS NUMERIC AND CAD-CEP > 0 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CEPX.CAD_CEP.IsNumeric() && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CEPX.CAD_CEP > 0)
            {

                /*" -2685- MOVE CAD-CEP TO FCLOTERI-NUM-CEP */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CEPX.CAD_CEP, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_CEP);

                /*" -2686- ELSE */
            }
            else
            {


                /*" -2688- MOVE -1 TO VIND-NUM-CEP. */
                _.Move(-1, VIND_NUM_CEP);
            }


            /*" -2690- IF CAD-EN-SUB IS NUMERIC AND CAD-EN-SUB > 0 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_EN_SUBX.CAD_EN_SUB.IsNumeric() && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_EN_SUBX.CAD_EN_SUB > 0)
            {

                /*" -2691- MOVE CAD-EN-SUB TO FCLOTERI-NUM-ENCEF */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_EN_SUBX.CAD_EN_SUB, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_ENCEF);

                /*" -2692- ELSE */
            }
            else
            {


                /*" -2694- MOVE -1 TO VIND-NUM-ENCEF. */
                _.Move(-1, VIND_NUM_ENCEF);
            }


            /*" -2696- IF CAD-NUM-LOT-ANTERIOR IS NUMERIC AND CAD-NUM-LOT-ANTERIOR > 0 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_LOT_ANTERIORX.CAD_NUM_LOT_ANTERIOR.IsNumeric() && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_LOT_ANTERIORX.CAD_NUM_LOT_ANTERIOR > 0)
            {

                /*" -2697- MOVE CAD-NUM-LOT-ANTERIOR TO FCLOTERI-NUM-LOTER-ANT */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_LOT_ANTERIORX.CAD_NUM_LOT_ANTERIOR, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_LOTER_ANT);

                /*" -2698- ELSE */
            }
            else
            {


                /*" -2700- MOVE -1 TO VIND-NUM-LOTER-ANT. */
                _.Move(-1, VIND_NUM_LOTER_ANT);
            }


            /*" -2702- IF CAD-MATR-CONSULTOR IS NUMERIC AND CAD-MATR-CONSULTOR > 0 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_MATR_CONSULTORX.CAD_MATR_CONSULTOR.IsNumeric() && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_MATR_CONSULTORX.CAD_MATR_CONSULTOR > 0)
            {

                /*" -2704- MOVE CAD-MATR-CONSULTOR TO FCLOTERI-NUM-MATR-CONSULTOR */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_MATR_CONSULTORX.CAD_MATR_CONSULTOR, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_MATR_CONSULTOR);

                /*" -2705- ELSE */
            }
            else
            {


                /*" -2707- MOVE -1 TO VIND-NUM-MATR-CON. */
                _.Move(-1, VIND_NUM_MATR_CON);
            }


            /*" -2709- IF CAD-PV-SUB IS NUMERIC AND CAD-PV-SUB > 0 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_PV_SUBX.CAD_PV_SUB.IsNumeric() && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_PV_SUBX.CAD_PV_SUB > 0)
            {

                /*" -2710- MOVE CAD-PV-SUB TO FCLOTERI-NUM-PVCEF */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_PV_SUBX.CAD_PV_SUB, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_PVCEF);

                /*" -2711- ELSE */
            }
            else
            {


                /*" -2713- MOVE -1 TO VIND-NUM-PVCEF. */
                _.Move(-1, VIND_NUM_PVCEF);
            }


            /*" -2714- IF CAD-TELEFONE NOT = SPACES AND ZEROS */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TELEFONE.In(string.Empty, "00"))
            {

                /*" -2715- MOVE CAD-TELEFONE TO FCLOTERI-NUM-TELEFONE */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TELEFONE, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_TELEFONE);

                /*" -2716- ELSE */
            }
            else
            {


                /*" -2718- MOVE -1 TO VIND-NUM-TELEFONE. */
                _.Move(-1, VIND_NUM_TELEFONE);
            }


            /*" -2719- IF CAD-NIVEL-COMISSAO > 0 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NIVEL_COMISSAOX.CAD_NIVEL_COMISSAO > 0)
            {

                /*" -2720- MOVE CAD-NIVEL-COMISSAOX TO FCLOTERI-STA-NIVEL-COMIS */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NIVEL_COMISSAOX, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_STA_NIVEL_COMIS);

                /*" -2721- ELSE */
            }
            else
            {


                /*" -2723- MOVE -1 TO VIND-STA-NIVEL-COMIS. */
                _.Move(-1, VIND_STA_NIVEL_COMIS);
            }


            /*" -2725- MOVE CAD-SITUACAO TO FCLOTERI-STA-LOTERICO. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_SITUACAOX.CAD_SITUACAO, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_STA_LOTERICO);

            /*" -2726- MOVE ' ' TO FCLOTERI-STA-ULT-ALT-ONLINE */
            _.Move(" ", FCLOTERI.DCLFC_LOTERICO.FCLOTERI_STA_ULT_ALT_ONLINE);

            /*" -2728- MOVE -1 TO VIND-STA-ULT-ALT. */
            _.Move(-1, VIND_STA_ULT_ALT);

            /*" -2730- MOVE CAD-TIPO-GARANTIA TO FCLOTERI-COD-GARANTIA. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TIPO_GARANTIAX.CAD_TIPO_GARANTIA, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_GARANTIA);

            /*" -2732- IF CAD-VALOR-GARANTIA IS NUMERIC AND CAD-VALOR-GARANTIA > 0 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_VALOR_GARANTIAX.CAD_VALOR_GARANTIA.IsNumeric() && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_VALOR_GARANTIAX.CAD_VALOR_GARANTIA > 0)
            {

                /*" -2733- MOVE CAD-VALOR-GARANTIA TO FCLOTERI-VLR-GARANTIA */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_VALOR_GARANTIAX.CAD_VALOR_GARANTIA, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_VLR_GARANTIA);

                /*" -2734- ELSE */
            }
            else
            {


                /*" -2736- MOVE -1 TO VIND-VLR-GARANTIA. */
                _.Move(-1, VIND_VLR_GARANTIA);
            }


            /*" -2737- MOVE W-CAD-DATA-GERACAO TO FCLOTERI-DTH-GERACAO. */
            _.Move(AREA_DE_WORK.W_CAD_DATA_GERACAO, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DTH_GERACAO);

            /*" -2739- MOVE CAD-NUM-SEGURADORA TO FCLOTERI-NUM-SEGURADORA. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_SEGURADORAX.CAD_NUM_SEGURADORA, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_SEGURADORA);

            /*" -2740- IF CAD-NUMERO-FAX NOT = SPACES AND ZEROS */

            if (!AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUMERO_FAX.In(string.Empty, "00"))
            {

                /*" -2741- MOVE CAD-NUMERO-FAX TO FCLOTERI-NUM-FAX */
                _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUMERO_FAX, FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_FAX);

                /*" -2742- ELSE */
            }
            else
            {


                /*" -2744- MOVE -1 TO VIND-NUM-FAX. */
                _.Move(-1, VIND_NUM_FAX);
            }


            /*" -2745- IF WS-NECESSARIO = 1 */

            if (WS_NECESSARIO == 1)
            {

                /*" -2746- MOVE 'S' TO FCLOTERI-STA-DADOS-N-CADAST */
                _.Move("S", FCLOTERI.DCLFC_LOTERICO.FCLOTERI_STA_DADOS_N_CADAST);

                /*" -2747- ELSE */
            }
            else
            {


                /*" -2747- MOVE 'N' TO FCLOTERI-STA-DADOS-N-CADAST. */
                _.Move("N", FCLOTERI.DCLFC_LOTERICO.FCLOTERI_STA_DADOS_N_CADAST);
            }


        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6200_SAIDA*/

        [StopWatch]
        /*" R6210-INSERT-FC-LOTERICO-SECTION */
        private void R6210_INSERT_FC_LOTERICO_SECTION()
        {
            /*" -2757- MOVE '0008' TO WNR-EXEC-SQL. */
            _.Move("0008", LK_CONVERSAO.WABEND.WNR_EXEC_SQL);

            /*" -2839- PERFORM R6210_INSERT_FC_LOTERICO_DB_INSERT_1 */

            R6210_INSERT_FC_LOTERICO_DB_INSERT_1();

            /*" -2842- IF SQLCODE EQUAL ZEROS */

            if (DB.SQLCODE == 00)
            {

                /*" -2843- ADD 1 TO WS-FCLOT-TOTAL-INCLUIDOS */
                AREA_DE_WORK.WS_FCLOT_TOTAL_INCLUIDOS.Value = AREA_DE_WORK.WS_FCLOT_TOTAL_INCLUIDOS + 1;

                /*" -2844- ADD 1 TO W-AC-LOTERICOS-GRAVADOS */
                AREA_DE_WORK.W_AC_LOTERICOS_GRAVADOS.Value = AREA_DE_WORK.W_AC_LOTERICOS_GRAVADOS + 1;

                /*" -2845- ELSE */
            }
            else
            {


                /*" -2846- DISPLAY 'ERRO INSERT FCLOTERICO................... ' */
                _.Display($"ERRO INSERT FCLOTERICO................... ");

                /*" -2847- DISPLAY 'COD. LOTERICO   = ' FCLOTERI-NUM-LOTERICO */
                _.Display($"COD. LOTERICO   = {FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_LOTERICO}");

                /*" -2848- DISPLAY ' DV =' FCLOTERI-NUM-DV-LOTERICO */
                _.Display($" DV ={FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_DV_LOTERICO}");

                /*" -2849- DISPLAY 'DTH-EXC=' FCLOTERI-DTH-EXCLUSAO */
                _.Display($"DTH-EXC={FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DTH_EXCLUSAO}");

                /*" -2850- DISPLAY 'DTH-INC=' FCLOTERI-DTH-INCLUSAO */
                _.Display($"DTH-INC={FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DTH_INCLUSAO}");

                /*" -2851- DISPLAY 'IDE-CONTA-CAUCAO=' FCLOTERI-IDE-CONTA-CAUCAO */
                _.Display($"IDE-CONTA-CAUCAO={FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IDE_CONTA_CAUCAO}");

                /*" -2852- DISPLAY 'IDE-CONTA-CPMF=' FCLOTERI-IDE-CONTA-CPMF */
                _.Display($"IDE-CONTA-CPMF={FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IDE_CONTA_CPMF}");

                /*" -2853- DISPLAY 'IDE-CONTA-ISENTA=' FCLOTERI-IDE-CONTA-ISENTA */
                _.Display($"IDE-CONTA-ISENTA={FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IDE_CONTA_ISENTA}");

                /*" -2854- DISPLAY 'VIND-DTH-EXC=' VIND-DTH-EXCLUSAO */
                _.Display($"VIND-DTH-EXC={VIND_DTH_EXCLUSAO}");

                /*" -2855- DISPLAY 'VIND-DTH-INC=' VIND-DTH-INCLUSAO */
                _.Display($"VIND-DTH-INC={VIND_DTH_INCLUSAO}");

                /*" -2856- DISPLAY 'VIND-DTH-CONTA-CAUCAO=' VIND-IDE-CONTA-CAUCAO */
                _.Display($"VIND-DTH-CONTA-CAUCAO={VIND_IDE_CONTA_CAUCAO}");

                /*" -2857- DISPLAY 'VIND-DTH-CONTA-CPMF=' VIND-IDE-CONTA-CPMF */
                _.Display($"VIND-DTH-CONTA-CPMF={VIND_IDE_CONTA_CPMF}");

                /*" -2858- DISPLAY 'VIND-IDE-CONTA-ISENTA=' VIND-IDE-CONTA-ISENTA */
                _.Display($"VIND-IDE-CONTA-ISENTA={VIND_IDE_CONTA_ISENTA}");

                /*" -2858- GO TO R9999-ROT-ERRO. */

                R9999_ROT_ERRO_SECTION(); //GOTO
                return;
            }


        }

        [StopWatch]
        /*" R6210-INSERT-FC-LOTERICO-DB-INSERT-1 */
        public void R6210_INSERT_FC_LOTERICO_DB_INSERT_1()
        {
            /*" -2839- EXEC SQL INSERT INTO FDRCAP.FC_LOTERICO (NUM_LOTERICO, COD_AGENTE_MASTER, COD_CGC, COD_INSCR_ESTAD, COD_INSCR_MUNIC, COD_MUNICIPIO, COD_UF, DES_EMAIL, DES_ENDERECO, DTH_EXCLUSAO, DTH_INCLUSAO, IDE_CONTA_CAUCAO, IDE_CONTA_CPMF, IDE_CONTA_ISENTA, IND_CAT_LOTERICO, IND_STA_LOTERICO, IND_UNIDADE_SUB, NOM_BAIRRO, NOM_CONSULTOR, NOM_CONTATO1, NOM_CONTATO2, NOM_FANTASIA, NOM_MUNICIPIO, NOM_RAZAO_SOCIAL, NUM_CEP, NUM_ENCEF, NUM_LOTER_ANT, NUM_MATR_CONSULTOR, NUM_PVCEF, NUM_TELEFONE, STA_DADOS_N_CADAST, STA_LOTERICO, STA_NIVEL_COMIS, STA_ULT_ALT_ONLINE, COD_GARANTIA, VLR_GARANTIA, DTH_GERACAO, NUM_FAX, NUM_DV_LOTERICO, NUM_SEGURADORA) VALUES (:FCLOTERI-NUM-LOTERICO, :FCLOTERI-COD-AGENTE-MASTER :VIND-AGENTE-MASTER, :FCLOTERI-COD-CGC :VIND-COD-CGC, :FCLOTERI-COD-INSCR-ESTAD :VIND-COD-INSCR-ESTAD, :FCLOTERI-COD-INSCR-MUNIC :VIND-COD-INSCR-MUNIC, :FCLOTERI-COD-MUNICIPIO :VIND-COD-MUNICIPIO, :FCLOTERI-COD-UF :VIND-COD-UF, :FCLOTERI-DES-EMAIL :VIND-DES-EMAIL, :FCLOTERI-DES-ENDERECO :VIND-DES-ENDERECO, :FCLOTERI-DTH-EXCLUSAO :VIND-DTH-EXCLUSAO, :FCLOTERI-DTH-INCLUSAO :VIND-DTH-INCLUSAO, :FCLOTERI-IDE-CONTA-CAUCAO :VIND-IDE-CONTA-CAUCAO, :FCLOTERI-IDE-CONTA-CPMF :VIND-IDE-CONTA-CPMF, :FCLOTERI-IDE-CONTA-ISENTA :VIND-IDE-CONTA-ISENTA, :FCLOTERI-IND-CAT-LOTERICO :VIND-IND-CAT-LOTERICO, :FCLOTERI-IND-STA-LOTERICO :VIND-IND-STA-LOTERICO, NULL , :FCLOTERI-NOM-BAIRRO :VIND-NOM-BAIRRO, :FCLOTERI-NOM-CONSULTOR :VIND-NOM-CONSULTOR, :FCLOTERI-NOM-CONTATO1 :VIND-NOM-CONTATO1, :FCLOTERI-NOM-CONTATO2 :VIND-NOM-CONTATO2, :FCLOTERI-NOM-FANTASIA :VIND-NOM-FANTASIA, :FCLOTERI-NOM-MUNICIPIO :VIND-NOM-MUNICIPIO, :FCLOTERI-NOM-RAZAO-SOCIAL :VIND-NOM-RAZAO-SOCIAL, :FCLOTERI-NUM-CEP :VIND-NUM-CEP, :FCLOTERI-NUM-ENCEF :VIND-NUM-ENCEF, :FCLOTERI-NUM-LOTER-ANT :VIND-NUM-LOTER-ANT, :FCLOTERI-NUM-MATR-CONSULTOR :VIND-NUM-MATR-CON, :FCLOTERI-NUM-PVCEF :VIND-NUM-PVCEF, :FCLOTERI-NUM-TELEFONE :VIND-NUM-TELEFONE, :FCLOTERI-STA-DADOS-N-CADAST :VIND-STA-DADOS-N, :FCLOTERI-STA-LOTERICO :VIND-STA-LOTERICO, :FCLOTERI-STA-NIVEL-COMIS :VIND-STA-NIVEL-COMIS, :FCLOTERI-STA-ULT-ALT-ONLINE :VIND-STA-ULT-ALT, :FCLOTERI-COD-GARANTIA :VIND-COD-GARANTIA, :FCLOTERI-VLR-GARANTIA :VIND-VLR-GARANTIA, :FCLOTERI-DTH-GERACAO :VIND-DTH-GERACAO, :FCLOTERI-NUM-FAX :VIND-NUM-FAX, :FCLOTERI-NUM-DV-LOTERICO, :FCLOTERI-NUM-SEGURADORA) END-EXEC. */

            var r6210_INSERT_FC_LOTERICO_DB_INSERT_1_Insert1 = new R6210_INSERT_FC_LOTERICO_DB_INSERT_1_Insert1()
            {
                FCLOTERI_NUM_LOTERICO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_LOTERICO.ToString(),
                FCLOTERI_COD_AGENTE_MASTER = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_AGENTE_MASTER.ToString(),
                VIND_AGENTE_MASTER = VIND_AGENTE_MASTER.ToString(),
                FCLOTERI_COD_CGC = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_CGC.ToString(),
                VIND_COD_CGC = VIND_COD_CGC.ToString(),
                FCLOTERI_COD_INSCR_ESTAD = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_INSCR_ESTAD.ToString(),
                VIND_COD_INSCR_ESTAD = VIND_COD_INSCR_ESTAD.ToString(),
                FCLOTERI_COD_INSCR_MUNIC = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_INSCR_MUNIC.ToString(),
                VIND_COD_INSCR_MUNIC = VIND_COD_INSCR_MUNIC.ToString(),
                FCLOTERI_COD_MUNICIPIO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_MUNICIPIO.ToString(),
                VIND_COD_MUNICIPIO = VIND_COD_MUNICIPIO.ToString(),
                FCLOTERI_COD_UF = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_UF.ToString(),
                VIND_COD_UF = VIND_COD_UF.ToString(),
                FCLOTERI_DES_EMAIL = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DES_EMAIL.ToString(),
                VIND_DES_EMAIL = VIND_DES_EMAIL.ToString(),
                FCLOTERI_DES_ENDERECO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DES_ENDERECO.ToString(),
                VIND_DES_ENDERECO = VIND_DES_ENDERECO.ToString(),
                FCLOTERI_DTH_EXCLUSAO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DTH_EXCLUSAO.ToString(),
                VIND_DTH_EXCLUSAO = VIND_DTH_EXCLUSAO.ToString(),
                FCLOTERI_DTH_INCLUSAO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DTH_INCLUSAO.ToString(),
                VIND_DTH_INCLUSAO = VIND_DTH_INCLUSAO.ToString(),
                FCLOTERI_IDE_CONTA_CAUCAO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IDE_CONTA_CAUCAO.ToString(),
                VIND_IDE_CONTA_CAUCAO = VIND_IDE_CONTA_CAUCAO.ToString(),
                FCLOTERI_IDE_CONTA_CPMF = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IDE_CONTA_CPMF.ToString(),
                VIND_IDE_CONTA_CPMF = VIND_IDE_CONTA_CPMF.ToString(),
                FCLOTERI_IDE_CONTA_ISENTA = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IDE_CONTA_ISENTA.ToString(),
                VIND_IDE_CONTA_ISENTA = VIND_IDE_CONTA_ISENTA.ToString(),
                FCLOTERI_IND_CAT_LOTERICO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IND_CAT_LOTERICO.ToString(),
                VIND_IND_CAT_LOTERICO = VIND_IND_CAT_LOTERICO.ToString(),
                FCLOTERI_IND_STA_LOTERICO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IND_STA_LOTERICO.ToString(),
                VIND_IND_STA_LOTERICO = VIND_IND_STA_LOTERICO.ToString(),
                FCLOTERI_NOM_BAIRRO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_BAIRRO.ToString(),
                VIND_NOM_BAIRRO = VIND_NOM_BAIRRO.ToString(),
                FCLOTERI_NOM_CONSULTOR = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_CONSULTOR.ToString(),
                VIND_NOM_CONSULTOR = VIND_NOM_CONSULTOR.ToString(),
                FCLOTERI_NOM_CONTATO1 = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_CONTATO1.ToString(),
                VIND_NOM_CONTATO1 = VIND_NOM_CONTATO1.ToString(),
                FCLOTERI_NOM_CONTATO2 = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_CONTATO2.ToString(),
                VIND_NOM_CONTATO2 = VIND_NOM_CONTATO2.ToString(),
                FCLOTERI_NOM_FANTASIA = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_FANTASIA.ToString(),
                VIND_NOM_FANTASIA = VIND_NOM_FANTASIA.ToString(),
                FCLOTERI_NOM_MUNICIPIO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_MUNICIPIO.ToString(),
                VIND_NOM_MUNICIPIO = VIND_NOM_MUNICIPIO.ToString(),
                FCLOTERI_NOM_RAZAO_SOCIAL = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_RAZAO_SOCIAL.ToString(),
                VIND_NOM_RAZAO_SOCIAL = VIND_NOM_RAZAO_SOCIAL.ToString(),
                FCLOTERI_NUM_CEP = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_CEP.ToString(),
                VIND_NUM_CEP = VIND_NUM_CEP.ToString(),
                FCLOTERI_NUM_ENCEF = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_ENCEF.ToString(),
                VIND_NUM_ENCEF = VIND_NUM_ENCEF.ToString(),
                FCLOTERI_NUM_LOTER_ANT = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_LOTER_ANT.ToString(),
                VIND_NUM_LOTER_ANT = VIND_NUM_LOTER_ANT.ToString(),
                FCLOTERI_NUM_MATR_CONSULTOR = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_MATR_CONSULTOR.ToString(),
                VIND_NUM_MATR_CON = VIND_NUM_MATR_CON.ToString(),
                FCLOTERI_NUM_PVCEF = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_PVCEF.ToString(),
                VIND_NUM_PVCEF = VIND_NUM_PVCEF.ToString(),
                FCLOTERI_NUM_TELEFONE = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_TELEFONE.ToString(),
                VIND_NUM_TELEFONE = VIND_NUM_TELEFONE.ToString(),
                FCLOTERI_STA_DADOS_N_CADAST = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_STA_DADOS_N_CADAST.ToString(),
                VIND_STA_DADOS_N = VIND_STA_DADOS_N.ToString(),
                FCLOTERI_STA_LOTERICO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_STA_LOTERICO.ToString(),
                VIND_STA_LOTERICO = VIND_STA_LOTERICO.ToString(),
                FCLOTERI_STA_NIVEL_COMIS = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_STA_NIVEL_COMIS.ToString(),
                VIND_STA_NIVEL_COMIS = VIND_STA_NIVEL_COMIS.ToString(),
                FCLOTERI_STA_ULT_ALT_ONLINE = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_STA_ULT_ALT_ONLINE.ToString(),
                VIND_STA_ULT_ALT = VIND_STA_ULT_ALT.ToString(),
                FCLOTERI_COD_GARANTIA = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_GARANTIA.ToString(),
                VIND_COD_GARANTIA = VIND_COD_GARANTIA.ToString(),
                FCLOTERI_VLR_GARANTIA = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_VLR_GARANTIA.ToString(),
                VIND_VLR_GARANTIA = VIND_VLR_GARANTIA.ToString(),
                FCLOTERI_DTH_GERACAO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DTH_GERACAO.ToString(),
                VIND_DTH_GERACAO = VIND_DTH_GERACAO.ToString(),
                FCLOTERI_NUM_FAX = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_FAX.ToString(),
                VIND_NUM_FAX = VIND_NUM_FAX.ToString(),
                FCLOTERI_NUM_DV_LOTERICO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_DV_LOTERICO.ToString(),
                FCLOTERI_NUM_SEGURADORA = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_SEGURADORA.ToString(),
            };

            R6210_INSERT_FC_LOTERICO_DB_INSERT_1_Insert1.Execute(r6210_INSERT_FC_LOTERICO_DB_INSERT_1_Insert1);

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6210_SAIDA*/

        [StopWatch]
        /*" R6220-GRAVAR-FC-CONTA-SECTION */
        private void R6220_GRAVAR_FC_CONTA_SECTION()
        {
            /*" -2869- MOVE ZEROS TO WS-IDE-CONTA-CPMF WS-IDE-CONTA-ISENTA WS-IDE-CONTA-CAUCAO. */
            _.Move(0, WS_IDE_CONTA_CPMF, WS_IDE_CONTA_ISENTA, WS_IDE_CONTA_CAUCAO);

            /*" -2872- IF CAD-BANCO-DESC-CPMF EQUAL ZEROS OR CAD-AGEN-DESC-CPMF EQUAL ZEROS OR CAD-CONTA-DESC-CPMF EQUAL ZEROS */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_DESC_CPMFX.CAD_BANCO_DESC_CPMF == 00 || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_DESC_CPMFX.CAD_AGEN_DESC_CPMF == 00 || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_DESC_CPMFX.CAD_CONTA_DESC_CPMF == 00)
            {

                /*" -2873- GO TO R6220-CONTA-ISENTA */

                R6220_CONTA_ISENTA(); //GOTO
                return;

                /*" -2875- END-IF */
            }


            /*" -2876- MOVE CAD-BANCO-DESC-CPMF TO WS-CODIGO-BANCO */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_DESC_CPMFX.CAD_BANCO_DESC_CPMF, AREA_DE_WORK.WS_CODIGO_BANCO);

            /*" -2877- MOVE CAD-AGEN-DESC-CPMF TO WS-CODIGO-AGENCIA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_DESC_CPMFX.CAD_AGEN_DESC_CPMF, AREA_DE_WORK.WS_CODIGO_AGENCIA);

            /*" -2880- MOVE CAD-CONTA-DESC-CPMF TO WS-CONTA-BANCARIA WS-CONTA-BANCO */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_DESC_CPMFX.CAD_CONTA_DESC_CPMF, AREA_DE_WORK.WS_CONTA_BANCARIA, AREA_DE_WORK.WS_CONTA_BANCO);

            /*" -2883- MOVE WS-NUMERO-CONTA TO WS-CONTA-12POS. */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_NUMERO_CONTA, AREA_DE_WORK.WS_CONTA_12POS);

            /*" -2885- MOVE WS-CODIGO-BANCO TO FCCONBAN-COD-BANCO */
            _.Move(AREA_DE_WORK.WS_CODIGO_BANCO, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_BANCO);

            /*" -2887- MOVE WS-CODIGO-AGENCIA TO FCCONBAN-COD-AGENCIA */
            _.Move(AREA_DE_WORK.WS_CODIGO_AGENCIA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_AGENCIA);

            /*" -2889- MOVE WS-CONTA-12POS TO FCCONBAN-COD-CONTA */
            _.Move(AREA_DE_WORK.WS_CONTA_12POS, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_CONTA);

            /*" -2890- MOVE WS-OPERA-CONTA TO FCCONBAN-COD-OP-CONTA */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCO_R.WS_OPERA_CONTA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_OP_CONTA);

            /*" -2892- MOVE WS-DV-CONTA TO FCCONBAN-COD-DV-CONTA */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_DV_CONTA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_DV_CONTA);

            /*" -2894- PERFORM R6080-VER-CONTA-EXISTENTE. */

            R6080_VER_CONTA_EXISTENTE_SECTION();

            /*" -2895- IF FCCONBAN-IDE-CONTA-BANCARIA > ZEROS */

            if (FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA > 00)
            {

                /*" -2896- MOVE FCCONBAN-IDE-CONTA-BANCARIA TO WS-IDE-CONTA-CPMF */
                _.Move(FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA, WS_IDE_CONTA_CPMF);

                /*" -2897- ELSE */
            }
            else
            {


                /*" -2898- PERFORM R6230-INSERT-FC-CONTA */

                R6230_INSERT_FC_CONTA_SECTION();

                /*" -2899- MOVE MAX-IDE-CONTA-BANCARIA TO WS-IDE-CONTA-CPMF */
                _.Move(MAX_IDE_CONTA_BANCARIA, WS_IDE_CONTA_CPMF);

                /*" -2900- END-IF */
            }


            /*" -2900- . */

            /*" -0- FLUXCONTROL_PERFORM R6220_CONTA_ISENTA */

            R6220_CONTA_ISENTA();

        }

        [StopWatch]
        /*" R6220-CONTA-ISENTA */
        private void R6220_CONTA_ISENTA(bool isPerform = false)
        {
            /*" -2906- IF CAD-BANCO-ISENTA EQUAL ZEROS OR CAD-AGEN-ISENTA EQUAL ZEROS OR CAD-CONTA-ISENTA EQUAL ZEROS */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_ISENTAX.CAD_BANCO_ISENTA == 00 || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_ISENTAX.CAD_AGEN_ISENTA == 00 || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_ISENTAX.CAD_CONTA_ISENTA == 00)
            {

                /*" -2908- GO TO R6220-CONTA-CAUCAO. */

                R6220_CONTA_CAUCAO(); //GOTO
                return;
            }


            /*" -2909- MOVE CAD-BANCO-ISENTA TO WS-CODIGO-BANCO */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_ISENTAX.CAD_BANCO_ISENTA, AREA_DE_WORK.WS_CODIGO_BANCO);

            /*" -2910- MOVE CAD-AGEN-ISENTA TO WS-CODIGO-AGENCIA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_ISENTAX.CAD_AGEN_ISENTA, AREA_DE_WORK.WS_CODIGO_AGENCIA);

            /*" -2913- MOVE CAD-CONTA-ISENTA TO WS-CONTA-BANCARIA WS-CONTA-BANCO */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_ISENTAX.CAD_CONTA_ISENTA, AREA_DE_WORK.WS_CONTA_BANCARIA, AREA_DE_WORK.WS_CONTA_BANCO);

            /*" -2916- MOVE WS-NUMERO-CONTA TO WS-CONTA-12POS. */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_NUMERO_CONTA, AREA_DE_WORK.WS_CONTA_12POS);

            /*" -2918- MOVE WS-CODIGO-BANCO TO FCCONBAN-COD-BANCO */
            _.Move(AREA_DE_WORK.WS_CODIGO_BANCO, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_BANCO);

            /*" -2920- MOVE WS-CODIGO-AGENCIA TO FCCONBAN-COD-AGENCIA */
            _.Move(AREA_DE_WORK.WS_CODIGO_AGENCIA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_AGENCIA);

            /*" -2922- MOVE WS-CONTA-12POS TO FCCONBAN-COD-CONTA */
            _.Move(AREA_DE_WORK.WS_CONTA_12POS, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_CONTA);

            /*" -2923- MOVE WS-OPERA-CONTA TO FCCONBAN-COD-OP-CONTA */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCO_R.WS_OPERA_CONTA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_OP_CONTA);

            /*" -2925- MOVE WS-DV-CONTA TO FCCONBAN-COD-DV-CONTA */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_DV_CONTA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_DV_CONTA);

            /*" -2927- PERFORM R6080-VER-CONTA-EXISTENTE. */

            R6080_VER_CONTA_EXISTENTE_SECTION();

            /*" -2928- IF FCCONBAN-IDE-CONTA-BANCARIA > ZEROS */

            if (FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA > 00)
            {

                /*" -2929- MOVE FCCONBAN-IDE-CONTA-BANCARIA TO WS-IDE-CONTA-ISENTA */
                _.Move(FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA, WS_IDE_CONTA_ISENTA);

                /*" -2930- ELSE */
            }
            else
            {


                /*" -2931- PERFORM R6230-INSERT-FC-CONTA */

                R6230_INSERT_FC_CONTA_SECTION();

                /*" -2932- MOVE MAX-IDE-CONTA-BANCARIA TO WS-IDE-CONTA-ISENTA */
                _.Move(MAX_IDE_CONTA_BANCARIA, WS_IDE_CONTA_ISENTA);

                /*" -2933- END-IF */
            }


            /*" -2933- . */

        }

        [StopWatch]
        /*" R6220-CONTA-CAUCAO */
        private void R6220_CONTA_CAUCAO(bool isPerform = false)
        {
            /*" -2939- IF CAD-BANCO-CAUCAO EQUAL ZEROS OR CAD-AGEN-CAUCAO EQUAL ZEROS OR CAD-CONTA-CAUCAO EQUAL ZEROS */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_CAUCAOX.CAD_BANCO_CAUCAO == 00 || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_CAUCAOX.CAD_AGEN_CAUCAO == 00 || AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_CAUCAOX.CAD_CONTA_CAUCAO == 00)
            {

                /*" -2941- GO TO R6220-SAIDA. */
                /*Método Suprimido por falta de linha ou apenas EXIT nome: R6220_SAIDA*/ //GOTO
                return;
            }


            /*" -2942- MOVE CAD-BANCO-CAUCAO TO WS-CODIGO-BANCO */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_CAUCAOX.CAD_BANCO_CAUCAO, AREA_DE_WORK.WS_CODIGO_BANCO);

            /*" -2943- MOVE CAD-AGEN-CAUCAO TO WS-CODIGO-AGENCIA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_CAUCAOX.CAD_AGEN_CAUCAO, AREA_DE_WORK.WS_CODIGO_AGENCIA);

            /*" -2946- MOVE CAD-CONTA-CAUCAO TO WS-CONTA-BANCARIA WS-CONTA-BANCO */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_CAUCAOX.CAD_CONTA_CAUCAO, AREA_DE_WORK.WS_CONTA_BANCARIA, AREA_DE_WORK.WS_CONTA_BANCO);

            /*" -2949- MOVE WS-NUMERO-CONTA TO WS-CONTA-12POS. */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_NUMERO_CONTA, AREA_DE_WORK.WS_CONTA_12POS);

            /*" -2951- MOVE WS-CODIGO-BANCO TO FCCONBAN-COD-BANCO */
            _.Move(AREA_DE_WORK.WS_CODIGO_BANCO, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_BANCO);

            /*" -2953- MOVE WS-CODIGO-AGENCIA TO FCCONBAN-COD-AGENCIA */
            _.Move(AREA_DE_WORK.WS_CODIGO_AGENCIA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_AGENCIA);

            /*" -2955- MOVE WS-CONTA-12POS TO FCCONBAN-COD-CONTA */
            _.Move(AREA_DE_WORK.WS_CONTA_12POS, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_CONTA);

            /*" -2956- MOVE WS-OPERA-CONTA TO FCCONBAN-COD-OP-CONTA */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCO_R.WS_OPERA_CONTA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_OP_CONTA);

            /*" -2958- MOVE WS-DV-CONTA TO FCCONBAN-COD-DV-CONTA */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_DV_CONTA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_DV_CONTA);

            /*" -2960- PERFORM R6080-VER-CONTA-EXISTENTE. */

            R6080_VER_CONTA_EXISTENTE_SECTION();

            /*" -2961- IF FCCONBAN-IDE-CONTA-BANCARIA > ZEROS */

            if (FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA > 00)
            {

                /*" -2962- MOVE FCCONBAN-IDE-CONTA-BANCARIA TO WS-IDE-CONTA-CAUCAO */
                _.Move(FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA, WS_IDE_CONTA_CAUCAO);

                /*" -2963- ELSE */
            }
            else
            {


                /*" -2964- PERFORM R6230-INSERT-FC-CONTA */

                R6230_INSERT_FC_CONTA_SECTION();

                /*" -2965- MOVE MAX-IDE-CONTA-BANCARIA TO WS-IDE-CONTA-CAUCAO */
                _.Move(MAX_IDE_CONTA_BANCARIA, WS_IDE_CONTA_CAUCAO);

                /*" -2966- END-IF */
            }


            /*" -2966- . */

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6220_SAIDA*/

        [StopWatch]
        /*" R6230-INSERT-FC-CONTA-SECTION */
        private void R6230_INSERT_FC_CONTA_SECTION()
        {
            /*" -2975- MOVE '0008' TO WNR-EXEC-SQL. */
            _.Move("0008", LK_CONVERSAO.WABEND.WNR_EXEC_SQL);

            /*" -2976- PERFORM R5000-SELECT-MAX-CONTA */

            R5000_SELECT_MAX_CONTA_SECTION();

            /*" -2977- COMPUTE MAX-IDE-CONTA-BANCARIA = MAX-IDE-CONTA-BANCARIA + 1 */
            MAX_IDE_CONTA_BANCARIA.Value = MAX_IDE_CONTA_BANCARIA + 1;

            /*" -2978- MOVE MAX-IDE-CONTA-BANCARIA TO FCCONBAN-IDE-CONTA-BANCARIA */
            _.Move(MAX_IDE_CONTA_BANCARIA, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA);

            /*" -2980- MOVE 20 TO FCCONBAN-COD-EMPRESA */
            _.Move(20, FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_EMPRESA);

            /*" -2997- PERFORM R6230_INSERT_FC_CONTA_DB_INSERT_1 */

            R6230_INSERT_FC_CONTA_DB_INSERT_1();

            /*" -3000- IF SQLCODE NOT EQUAL ZEROS */

            if (DB.SQLCODE != 00)
            {

                /*" -3001- IF SQLCODE EQUAL -803 */

                if (DB.SQLCODE == -803)
                {

                    /*" -3002- DISPLAY ' R6230-ERRO -803 INSERT FC-CONTA BANCARIA' */
                    _.Display($" R6230-ERRO -803 INSERT FC-CONTA BANCARIA");

                    /*" -3011- DISPLAY ' AGEN=' FCCONBAN-COD-AGENCIA ' BANCO=' FCCONBAN-COD-BANCO ' CONTA=' FCCONBAN-COD-CONTA ' DV=' FCCONBAN-COD-DV-CONTA ' OP=' FCCONBAN-COD-OP-CONTA ' IDE= ' FCCONBAN-IDE-CONTA-BANCARIA ' CODLOT= ' CAD-COD-CEF ' EMP=' FCCONBAN-COD-EMPRESA '  CONTA JA CADASTRADA ' */

                    $" AGEN={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_AGENCIA} BANCO={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_BANCO} CONTA={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_CONTA} DV={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_DV_CONTA} OP={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_OP_CONTA} IDE= {FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA} CODLOT= {AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF_R.CAD_COD_CEF} EMP={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_EMPRESA}  CONTA JA CADASTRADA "
                    .Display();

                    /*" -3012- GO TO R6230-SAIDA */
                    /*Método Suprimido por falta de linha ou apenas EXIT nome: R6230_SAIDA*/ //GOTO
                    return;

                    /*" -3013- ELSE */
                }
                else
                {


                    /*" -3014- DISPLAY ' R6230-ERRO INSERT FC-CONTA BANCARIA' */
                    _.Display($" R6230-ERRO INSERT FC-CONTA BANCARIA");

                    /*" -3022- DISPLAY ' AGEN=' FCCONBAN-COD-AGENCIA ' BANCO=' FCCONBAN-COD-BANCO ' CONTA=' FCCONBAN-COD-CONTA ' DV=' FCCONBAN-COD-DV-CONTA ' OP=' FCCONBAN-COD-OP-CONTA ' IDE= ' FCCONBAN-IDE-CONTA-BANCARIA ' CODLOT= ' CAD-COD-CEF ' EMP=' FCCONBAN-COD-EMPRESA */

                    $" AGEN={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_AGENCIA} BANCO={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_BANCO} CONTA={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_CONTA} DV={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_DV_CONTA} OP={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_OP_CONTA} IDE= {FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA} CODLOT= {AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF_R.CAD_COD_CEF} EMP={FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_EMPRESA}"
                    .Display();

                    /*" -3023- GO TO R9999-ROT-ERRO */

                    R9999_ROT_ERRO_SECTION(); //GOTO
                    return;

                    /*" -3024- END-IF */
                }


                /*" -3026- END-IF. */
            }


            /*" -3027- PERFORM R5100-UPDATE-MAX-CONTA. */

            R5100_UPDATE_MAX_CONTA_SECTION();

            /*" -3027- . */

        }

        [StopWatch]
        /*" R6230-INSERT-FC-CONTA-DB-INSERT-1 */
        public void R6230_INSERT_FC_CONTA_DB_INSERT_1()
        {
            /*" -2997- EXEC SQL INSERT INTO FDRCAP.FC_CONTA_BANCARIA (IDE_CONTA_BANCARIA, COD_AGENCIA, COD_BANCO, COD_CONTA, COD_DV_CONTA, COD_OP_CONTA, COD_TIPO_CONTA, COD_EMPRESA) VALUES (:FCCONBAN-IDE-CONTA-BANCARIA, :FCCONBAN-COD-AGENCIA, :FCCONBAN-COD-BANCO, :FCCONBAN-COD-CONTA, :FCCONBAN-COD-DV-CONTA, :FCCONBAN-COD-OP-CONTA, 'LOT' , :FCCONBAN-COD-EMPRESA) END-EXEC. */

            var r6230_INSERT_FC_CONTA_DB_INSERT_1_Insert1 = new R6230_INSERT_FC_CONTA_DB_INSERT_1_Insert1()
            {
                FCCONBAN_IDE_CONTA_BANCARIA = FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_IDE_CONTA_BANCARIA.ToString(),
                FCCONBAN_COD_AGENCIA = FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_AGENCIA.ToString(),
                FCCONBAN_COD_BANCO = FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_BANCO.ToString(),
                FCCONBAN_COD_CONTA = FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_CONTA.ToString(),
                FCCONBAN_COD_DV_CONTA = FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_DV_CONTA.ToString(),
                FCCONBAN_COD_OP_CONTA = FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_OP_CONTA.ToString(),
                FCCONBAN_COD_EMPRESA = FCCONBAN.DCLFC_CONTA_BANCARIA.FCCONBAN_COD_EMPRESA.ToString(),
            };

            R6230_INSERT_FC_CONTA_DB_INSERT_1_Insert1.Execute(r6230_INSERT_FC_CONTA_DB_INSERT_1_Insert1);

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6230_SAIDA*/

        [StopWatch]
        /*" R6600-GRAVAR-MOVIMENTO-SECTION */
        private void R6600_GRAVAR_MOVIMENTO_SECTION()
        {
            /*" -3043- MOVE 7105 TO LTMVPROP-COD-PRODUTO */
            _.Move(7105, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_PRODUTO);

            /*" -3044- MOVE 6204 TO LTMVPROP-COD-EXT-ESTIP */
            _.Move(6204, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_EXT_ESTIP);

            /*" -3045- MOVE CAD-CODIGO-CEF TO LTMVPROP-COD-EXT-SEGURADO */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_EXT_SEGURADO);

            /*" -3047- MOVE W-CAD-DATA-GERACAO TO LTMVPROP-DATA-MOVIMENTO. */
            _.Move(AREA_DE_WORK.W_CAD_DATA_GERACAO, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_DATA_MOVIMENTO);

            /*" -3048- MOVE WS-HH-TIME TO WTIME-HORA */
            _.Move(AREA_DE_WORK.WS_TIME.WS_HH_TIME, AREA_DE_WORK.FILLER_13.WTIME_DAYR.WTIME_HORA);

            /*" -3049- MOVE '.' TO WTIME-2PT1 */
            _.Move(".", AREA_DE_WORK.FILLER_13.WTIME_DAYR.WTIME_2PT1);

            /*" -3050- MOVE WS-MM-TIME TO WTIME-MINU */
            _.Move(AREA_DE_WORK.WS_TIME.WS_MM_TIME, AREA_DE_WORK.FILLER_13.WTIME_DAYR.WTIME_MINU);

            /*" -3051- MOVE '.' TO WTIME-2PT2 */
            _.Move(".", AREA_DE_WORK.FILLER_13.WTIME_DAYR.WTIME_2PT2);

            /*" -3052- MOVE WS-SS-TIME TO WTIME-SEGU */
            _.Move(AREA_DE_WORK.WS_TIME.WS_SS_TIME, AREA_DE_WORK.FILLER_13.WTIME_DAYR.WTIME_SEGU);

            /*" -3054- MOVE WTIME-DAYR TO LTMVPROP-HORA-MOVIMENTO. */
            _.Move(AREA_DE_WORK.FILLER_13.WTIME_DAYR, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_HORA_MOVIMENTO);

            /*" -3055- MOVE WS-NUM-APOLICE TO LTMVPROP-NUM-APOLICE. */
            _.Move(WS_NUM_APOLICE, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_NUM_APOLICE);

            /*" -3057- MOVE SPACES TO LTMVPROP-COD-USUARIO-CANCEL. */
            _.Move("", LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_USUARIO_CANCEL);

            /*" -3058- MOVE CAD-RAZAO-SOCIAL TO LTMVPROP-NOM-RAZAO-SOCIAL */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_RAZAO_SOCIAL, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_NOM_RAZAO_SOCIAL);

            /*" -3059- MOVE CAD-NOME-FANTASIA TO LTMVPROP-NOME-FANTASIA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NOME_FANTASIA, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_NOME_FANTASIA);

            /*" -3060- MOVE CAD-CGC TO LTMVPROP-CGCCPF */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CGCX.CAD_CGC, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_CGCCPF);

            /*" -3061- MOVE CAD-END TO LTMVPROP-ENDERECO */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_ENDERECO.CAD_END, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_ENDERECO);

            /*" -3062- MOVE CAD-COMPL-END TO LTMVPROP-COMPL-ENDER */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_ENDERECO.CAD_COMPL_END, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COMPL_ENDER);

            /*" -3063- MOVE CAD-BAIRRO TO LTMVPROP-BAIRRO */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BAIRRO, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_BAIRRO);

            /*" -3064- MOVE CAD-CEP TO LTMVPROP-CEP */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CEPX.CAD_CEP, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_CEP);

            /*" -3065- MOVE CAD-CIDADE TO LTMVPROP-CIDADE */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CIDADE, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_CIDADE);

            /*" -3066- MOVE CAD-UF TO LTMVPROP-SIGLA-UF */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_UF, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_SIGLA_UF);

            /*" -3067- MOVE CAD-DDD-FONE TO LTMVPROP-DDD */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TELEFONE.CAD_DDD_FONE, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_DDD);

            /*" -3068- MOVE CAD-FONE TO LTMVPROP-NUM-FONE */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TELEFONE.CAD_FONE, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_NUM_FONE);

            /*" -3069- MOVE CAD-FAX TO LTMVPROP-NUM-FAX */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUMERO_FAX.CAD_FAX, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_NUM_FAX);

            /*" -3071- MOVE CAD-EMAIL TO LTMVPROP-EMAIL */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_EMAILX.CAD_EMAIL, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_EMAIL);

            /*" -3072- MOVE CAD-EN-SUB TO LTMVPROP-COD-DIVISAO */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_EN_SUBX.CAD_EN_SUB, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_DIVISAO);

            /*" -3074- MOVE CAD-PV-SUB TO LTMVPROP-COD-SUBDIVISAO */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_PV_SUBX.CAD_PV_SUB, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_SUBDIVISAO);

            /*" -3075- MOVE CAD-BANCO-DESC-CPMF TO WS-CODIGO-BANCO */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_DESC_CPMFX.CAD_BANCO_DESC_CPMF, AREA_DE_WORK.WS_CODIGO_BANCO);

            /*" -3077- MOVE WS-COD-BANCO TO LTMVPROP-COD-BANCO */
            _.Move(AREA_DE_WORK.WS_CODIGO_BANCO_R.WS_COD_BANCO, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_BANCO);

            /*" -3078- MOVE CAD-AGEN-DESC-CPMF TO WS-CODIGO-AGENCIA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_DESC_CPMFX.CAD_AGEN_DESC_CPMF, AREA_DE_WORK.WS_CODIGO_AGENCIA);

            /*" -3080- MOVE WS-COD-AGENCIA TO LTMVPROP-COD-AGENCIA */
            _.Move(AREA_DE_WORK.WS_CODIGO_AGENCIA_R.WS_COD_AGENCIA, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_AGENCIA);

            /*" -3082- MOVE CAD-CONTA-DESC-CPMF TO WS-CONTA-BANCARIA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_DESC_CPMFX.CAD_CONTA_DESC_CPMF, AREA_DE_WORK.WS_CONTA_BANCARIA);

            /*" -3084- MOVE WS-NUMERO-CONTA TO WS-CONTA-12POS */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_NUMERO_CONTA, AREA_DE_WORK.WS_CONTA_12POS);

            /*" -3085- MOVE WS-CONTA-12POS TO LTMVPROP-COD-CONTA */
            _.Move(AREA_DE_WORK.WS_CONTA_12POS, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_CONTA);

            /*" -3086- MOVE WS-DV-CONTA TO LTMVPROP-COD-DV-CONTA */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_DV_CONTA, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_DV_CONTA);

            /*" -3088- MOVE WS-OPERACAO-CONTA TO LTMVPROP-COD-OP-CONTA. */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_OPERACAO_CONTA, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_OP_CONTA);

            /*" -3093- MOVE CAD-NUM-LOT-ANTERIOR TO LTMVPROP-COD-EXT-SEG-ANT */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_LOT_ANTERIORX.CAD_NUM_LOT_ANTERIOR, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_EXT_SEG_ANT);

            /*" -3098- IF LTMVPROP-IND-ALT-DADOS-PES = 'N' AND LTMVPROP-IND-ALT-ENDER = 'N' AND W-IND-ALT-FAXTELEME = 'S' AND LTMVPROP-IND-ALT-COBER = 'N' AND LTMVPROP-IND-ALT-BONUS = 'N' */

            if (LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_DADOS_PES == "N" && LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_ENDER == "N" && W_IND_ALT_FAXTELEME == "S" && LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_COBER == "N" && LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_BONUS == "N")
            {

                /*" -3099- MOVE 'S' TO LTMVPROP-IND-ALT-ENDER */
                _.Move("S", LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_ENDER);

                /*" -3100- IF LTMVPROP-DDD > 0 */

                if (LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_DDD > 0)
                {

                    /*" -3101- COMPUTE LTMVPROP-DDD = LTMVPROP-DDD * - 1 */
                    LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_DDD.Value = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_DDD * -1;

                    /*" -3102- ELSE */
                }
                else
                {


                    /*" -3107- COMPUTE LTMVPROP-DDD = - 1 . */
                    LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_DDD.Value = -1;
                }

            }


            /*" -3109- MOVE '3' TO LTMVPROP-SIT-MOVIMENTO. */
            _.Move("3", LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_SIT_MOVIMENTO);

            /*" -3111- MOVE W-CAD-DATA-GERACAO TO LTMVPROP-DT-INIVIG-PROPOSTA. */
            _.Move(AREA_DE_WORK.W_CAD_DATA_GERACAO, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_DT_INIVIG_PROPOSTA);

            /*" -3113- MOVE 0 TO LTMVPROP-VAL-PREMIO. */
            _.Move(0, LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_VAL_PREMIO);

            /*" -3115- MOVE '6600' TO WNR-EXEC-SQL. */
            _.Move("6600", LK_CONVERSAO.WABEND.WNR_EXEC_SQL);

            /*" -3192- PERFORM R6600_GRAVAR_MOVIMENTO_DB_INSERT_1 */

            R6600_GRAVAR_MOVIMENTO_DB_INSERT_1();

            /*" -3195- IF SQLCODE NOT EQUAL ZEROS */

            if (DB.SQLCODE != 00)
            {

                /*" -3196- IF SQLCODE EQUAL - 803 */

                if (DB.SQLCODE == -803)
                {

                    /*" -3197- DISPLAY 'R6600-ERRO INSERT MOVIMENTO SQLCODE -803....' */
                    _.Display($"R6600-ERRO INSERT MOVIMENTO SQLCODE -803....");

                    /*" -3198- DISPLAY ' COD. LOT = ' LTMVPROP-COD-EXT-SEGURADO */
                    _.Display($" COD. LOT = {LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_EXT_SEGURADO}");

                    /*" -3199- DISPLAY ' RAZAO =' LTMVPROP-NOM-RAZAO-SOCIAL */
                    _.Display($" RAZAO ={LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_NOM_RAZAO_SOCIAL}");

                    /*" -3200- DISPLAY ' CGC =' LTMVPROP-CGCCPF */
                    _.Display($" CGC ={LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_CGCCPF}");

                    /*" -3201- DISPLAY ' COD.MOV =' LTMVPROP-COD-MOVIMENTO */
                    _.Display($" COD.MOV ={LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_MOVIMENTO}");

                    /*" -3202- GO TO R9999-ROT-ERRO */

                    R9999_ROT_ERRO_SECTION(); //GOTO
                    return;

                    /*" -3203- ELSE */
                }
                else
                {


                    /*" -3204- DISPLAY 'R6600-ERRO INSERT MOVIMENTO.............. ' */
                    _.Display($"R6600-ERRO INSERT MOVIMENTO.............. ");

                    /*" -3205- DISPLAY 'COD. LOT= ' LTMVPROP-COD-EXT-SEGURADO */
                    _.Display($"COD. LOT= {LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_EXT_SEGURADO}");

                    /*" -3206- DISPLAY ' RAZAO  =' LTMVPROP-NOM-RAZAO-SOCIAL */
                    _.Display($" RAZAO  ={LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_NOM_RAZAO_SOCIAL}");

                    /*" -3207- DISPLAY ' CGC    =' LTMVPROP-CGCCPF */
                    _.Display($" CGC    ={LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_CGCCPF}");

                    /*" -3212- DISPLAY ' COD.MOV=' LTMVPROP-COD-MOVIMENTO ' PRODUTO=' LTMVPROP-COD-PRODUTO ' ESTIP=' LTMVPROP-COD-EXT-ESTIP ' DATA=' LTMVPROP-DATA-MOVIMENTO ' HORA=' LTMVPROP-HORA-MOVIMENTO */

                    $" COD.MOV={LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_MOVIMENTO} PRODUTO={LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_PRODUTO} ESTIP={LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_EXT_ESTIP} DATA={LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_DATA_MOVIMENTO} HORA={LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_HORA_MOVIMENTO}"
                    .Display();

                    /*" -3214- GO TO R9999-ROT-ERRO. */

                    R9999_ROT_ERRO_SECTION(); //GOTO
                    return;
                }

            }


            /*" -3216- ADD 1 TO WS-MVPROP-TOTAL-INCLUIDOS. */
            AREA_DE_WORK.WS_MVPROP_TOTAL_INCLUIDOS.Value = AREA_DE_WORK.WS_MVPROP_TOTAL_INCLUIDOS + 1;

            /*" -3217- IF LTMVPROP-COD-MOVIMENTO = 3 */

            if (LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_MOVIMENTO == 3)
            {

                /*" -3218- ADD 1 TO WS-MVPROP-TOTAL-ALTERADOS */
                AREA_DE_WORK.WS_MVPROP_TOTAL_ALTERADOS.Value = AREA_DE_WORK.WS_MVPROP_TOTAL_ALTERADOS + 1;

                /*" -3219- ELSE */
            }
            else
            {


                /*" -3220- IF LTMVPROP-COD-MOVIMENTO = 7 */

                if (LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_MOVIMENTO == 7)
                {

                    /*" -3220- ADD 1 TO WS-MVPROP-TOTAL-CANCELADOS. */
                    AREA_DE_WORK.WS_MVPROP_TOTAL_CANCELADOS.Value = AREA_DE_WORK.WS_MVPROP_TOTAL_CANCELADOS + 1;
                }

            }


        }

        [StopWatch]
        /*" R6600-GRAVAR-MOVIMENTO-DB-INSERT-1 */
        public void R6600_GRAVAR_MOVIMENTO_DB_INSERT_1()
        {
            /*" -3192- EXEC SQL INSERT INTO SEGUROS.LT_MOV_PROPOSTA (COD_PRODUTO, COD_EXT_ESTIP, COD_EXT_SEGURADO, DATA_MOVIMENTO, HORA_MOVIMENTO, COD_MOVIMENTO, NOM_RAZAO_SOCIAL, NOME_FANTASIA, CGCCPF, ENDERECO, COMPL_ENDER, BAIRRO, CEP, CIDADE, SIGLA_UF, DDD, NUM_FONE, NUM_FAX, EMAIL, COD_DIVISAO, COD_SUBDIVISAO, COD_BANCO, COD_AGENCIA, COD_CONTA, COD_DV_CONTA, COD_OP_CONTA, COD_EXT_SEG_ANT, SIT_MOVIMENTO, DT_INIVIG_PROPOSTA, IND_ALT_DADOS_PES, IND_ALT_ENDER, IND_ALT_COBER, IND_ALT_BONUS, COD_USUARIO, TIMESTAMP, VAL_PREMIO, NUM_APOLICE, COD_USUARIO_CANCEL) VALUES (:LTMVPROP-COD-PRODUTO, :LTMVPROP-COD-EXT-ESTIP, :LTMVPROP-COD-EXT-SEGURADO, :LTMVPROP-DATA-MOVIMENTO, :LTMVPROP-HORA-MOVIMENTO, :LTMVPROP-COD-MOVIMENTO, :LTMVPROP-NOM-RAZAO-SOCIAL, :LTMVPROP-NOME-FANTASIA, :LTMVPROP-CGCCPF, :LTMVPROP-ENDERECO, :LTMVPROP-COMPL-ENDER, :LTMVPROP-BAIRRO, :LTMVPROP-CEP, :LTMVPROP-CIDADE, :LTMVPROP-SIGLA-UF, :LTMVPROP-DDD, :LTMVPROP-NUM-FONE, :LTMVPROP-NUM-FAX, :LTMVPROP-EMAIL, :LTMVPROP-COD-DIVISAO, :LTMVPROP-COD-SUBDIVISAO, :LTMVPROP-COD-BANCO, :LTMVPROP-COD-AGENCIA, :LTMVPROP-COD-CONTA, :LTMVPROP-COD-DV-CONTA, :LTMVPROP-COD-OP-CONTA, :LTMVPROP-COD-EXT-SEG-ANT, :LTMVPROP-SIT-MOVIMENTO, :LTMVPROP-DT-INIVIG-PROPOSTA, :LTMVPROP-IND-ALT-DADOS-PES, :LTMVPROP-IND-ALT-ENDER, :LTMVPROP-IND-ALT-COBER, :LTMVPROP-IND-ALT-BONUS, :LTMVPROP-COD-USUARIO, CURRENT TIMESTAMP, :LTMVPROP-VAL-PREMIO, :LTMVPROP-NUM-APOLICE, :LTMVPROP-COD-USUARIO-CANCEL) END-EXEC. */

            var r6600_GRAVAR_MOVIMENTO_DB_INSERT_1_Insert1 = new R6600_GRAVAR_MOVIMENTO_DB_INSERT_1_Insert1()
            {
                LTMVPROP_COD_PRODUTO = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_PRODUTO.ToString(),
                LTMVPROP_COD_EXT_ESTIP = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_EXT_ESTIP.ToString(),
                LTMVPROP_COD_EXT_SEGURADO = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_EXT_SEGURADO.ToString(),
                LTMVPROP_DATA_MOVIMENTO = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_DATA_MOVIMENTO.ToString(),
                LTMVPROP_HORA_MOVIMENTO = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_HORA_MOVIMENTO.ToString(),
                LTMVPROP_COD_MOVIMENTO = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_MOVIMENTO.ToString(),
                LTMVPROP_NOM_RAZAO_SOCIAL = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_NOM_RAZAO_SOCIAL.ToString(),
                LTMVPROP_NOME_FANTASIA = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_NOME_FANTASIA.ToString(),
                LTMVPROP_CGCCPF = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_CGCCPF.ToString(),
                LTMVPROP_ENDERECO = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_ENDERECO.ToString(),
                LTMVPROP_COMPL_ENDER = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COMPL_ENDER.ToString(),
                LTMVPROP_BAIRRO = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_BAIRRO.ToString(),
                LTMVPROP_CEP = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_CEP.ToString(),
                LTMVPROP_CIDADE = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_CIDADE.ToString(),
                LTMVPROP_SIGLA_UF = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_SIGLA_UF.ToString(),
                LTMVPROP_DDD = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_DDD.ToString(),
                LTMVPROP_NUM_FONE = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_NUM_FONE.ToString(),
                LTMVPROP_NUM_FAX = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_NUM_FAX.ToString(),
                LTMVPROP_EMAIL = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_EMAIL.ToString(),
                LTMVPROP_COD_DIVISAO = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_DIVISAO.ToString(),
                LTMVPROP_COD_SUBDIVISAO = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_SUBDIVISAO.ToString(),
                LTMVPROP_COD_BANCO = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_BANCO.ToString(),
                LTMVPROP_COD_AGENCIA = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_AGENCIA.ToString(),
                LTMVPROP_COD_CONTA = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_CONTA.ToString(),
                LTMVPROP_COD_DV_CONTA = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_DV_CONTA.ToString(),
                LTMVPROP_COD_OP_CONTA = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_OP_CONTA.ToString(),
                LTMVPROP_COD_EXT_SEG_ANT = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_EXT_SEG_ANT.ToString(),
                LTMVPROP_SIT_MOVIMENTO = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_SIT_MOVIMENTO.ToString(),
                LTMVPROP_DT_INIVIG_PROPOSTA = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_DT_INIVIG_PROPOSTA.ToString(),
                LTMVPROP_IND_ALT_DADOS_PES = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_DADOS_PES.ToString(),
                LTMVPROP_IND_ALT_ENDER = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_ENDER.ToString(),
                LTMVPROP_IND_ALT_COBER = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_COBER.ToString(),
                LTMVPROP_IND_ALT_BONUS = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_BONUS.ToString(),
                LTMVPROP_COD_USUARIO = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_USUARIO.ToString(),
                LTMVPROP_VAL_PREMIO = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_VAL_PREMIO.ToString(),
                LTMVPROP_NUM_APOLICE = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_NUM_APOLICE.ToString(),
                LTMVPROP_COD_USUARIO_CANCEL = LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_USUARIO_CANCEL.ToString(),
            };

            R6600_GRAVAR_MOVIMENTO_DB_INSERT_1_Insert1.Execute(r6600_GRAVAR_MOVIMENTO_DB_INSERT_1_Insert1);

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6600_SAIDA*/

        [StopWatch]
        /*" R6610-GRAVAR-MOV-COBER-SECTION */
        private void R6610_GRAVAR_MOV_COBER_SECTION()
        {
            /*" -3232- IF LTMVPROP-COD-MOVIMENTO = 7 OR (LTMVPROP-IND-ALT-COBER = 'N' AND LTMVPROP-IND-ALT-BONUS = 'N' ) */

            if (LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_MOVIMENTO == 7 || (LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_COBER == "N" && LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_BONUS == "N"))
            {

                /*" -3234- GO TO R6610-SAIDA. */
                /*Método Suprimido por falta de linha ou apenas EXIT nome: R6610_SAIDA*/ //GOTO
                return;
            }


            /*" -3235- MOVE LTMVPROP-COD-PRODUTO TO LTMVPRCO-COD-PRODUTO */
            _.Move(LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_PRODUTO, LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_COD_PRODUTO);

            /*" -3236- MOVE LTMVPROP-COD-EXT-ESTIP TO LTMVPRCO-COD-EXT-ESTIP */
            _.Move(LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_EXT_ESTIP, LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_COD_EXT_ESTIP);

            /*" -3237- MOVE LTMVPROP-COD-EXT-SEGURADO TO LTMVPRCO-COD-EXT-SEGURADO */
            _.Move(LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_EXT_SEGURADO, LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_COD_EXT_SEGURADO);

            /*" -3238- MOVE LTMVPROP-DATA-MOVIMENTO TO LTMVPRCO-DATA-MOVIMENTO */
            _.Move(LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_DATA_MOVIMENTO, LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_DATA_MOVIMENTO);

            /*" -3239- MOVE LTMVPROP-COD-MOVIMENTO TO LTMVPRCO-COD-MOVIMENTO */
            _.Move(LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_MOVIMENTO, LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_COD_MOVIMENTO);

            /*" -3240- MOVE LTMVPROP-HORA-MOVIMENTO TO LTMVPRCO-HORA-MOVIMENTO */
            _.Move(LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_HORA_MOVIMENTO, LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_HORA_MOVIMENTO);

            /*" -3242- MOVE 4 TO LTMVPRCO-COD-COBERTURA */
            _.Move(4, LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_COD_COBERTURA);

            /*" -3243- MOVE CAD-VALOR-GARANTIA TO LTMVPRCO-VAL-IMP-SEGURADA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_VALOR_GARANTIAX.CAD_VALOR_GARANTIA, LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_VAL_IMP_SEGURADA);

            /*" -3246- MOVE 0 TO LTMVPRCO-VAL-TAXA-PREMIO. */
            _.Move(0, LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_VAL_TAXA_PREMIO);

            /*" -3248- MOVE '6610' TO WNR-EXEC-SQL. */
            _.Move("6610", LK_CONVERSAO.WABEND.WNR_EXEC_SQL);

            /*" -3267- PERFORM R6610_GRAVAR_MOV_COBER_DB_INSERT_1 */

            R6610_GRAVAR_MOV_COBER_DB_INSERT_1();

            /*" -3270- IF SQLCODE NOT EQUAL ZEROS */

            if (DB.SQLCODE != 00)
            {

                /*" -3271- DISPLAY 'R6610-ERRO INSERT MOVIMENTO COBERTURA ' */
                _.Display($"R6610-ERRO INSERT MOVIMENTO COBERTURA ");

                /*" -3272- DISPLAY 'COD. LOTERICO   = ' LTMVPRCO-COD-EXT-SEGURADO */
                _.Display($"COD. LOTERICO   = {LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_COD_EXT_SEGURADO}");

                /*" -3272- GO TO R9999-ROT-ERRO. */

                R9999_ROT_ERRO_SECTION(); //GOTO
                return;
            }


        }

        [StopWatch]
        /*" R6610-GRAVAR-MOV-COBER-DB-INSERT-1 */
        public void R6610_GRAVAR_MOV_COBER_DB_INSERT_1()
        {
            /*" -3267- EXEC SQL INSERT INTO SEGUROS.LT_MOV_PROP_COBER (COD_PRODUTO, COD_EXT_ESTIP, COD_EXT_SEGURADO, DATA_MOVIMENTO, HORA_MOVIMENTO, COD_MOVIMENTO, COD_COBERTURA, VAL_IMP_SEGURADA, VAL_TAXA_PREMIO) VALUES (:LTMVPRCO-COD-PRODUTO, :LTMVPRCO-COD-EXT-ESTIP, :LTMVPRCO-COD-EXT-SEGURADO, :LTMVPRCO-DATA-MOVIMENTO, :LTMVPRCO-HORA-MOVIMENTO, :LTMVPRCO-COD-MOVIMENTO, :LTMVPRCO-COD-COBERTURA, :LTMVPRCO-VAL-IMP-SEGURADA, :LTMVPRCO-VAL-TAXA-PREMIO) END-EXEC. */

            var r6610_GRAVAR_MOV_COBER_DB_INSERT_1_Insert1 = new R6610_GRAVAR_MOV_COBER_DB_INSERT_1_Insert1()
            {
                LTMVPRCO_COD_PRODUTO = LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_COD_PRODUTO.ToString(),
                LTMVPRCO_COD_EXT_ESTIP = LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_COD_EXT_ESTIP.ToString(),
                LTMVPRCO_COD_EXT_SEGURADO = LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_COD_EXT_SEGURADO.ToString(),
                LTMVPRCO_DATA_MOVIMENTO = LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_DATA_MOVIMENTO.ToString(),
                LTMVPRCO_HORA_MOVIMENTO = LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_HORA_MOVIMENTO.ToString(),
                LTMVPRCO_COD_MOVIMENTO = LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_COD_MOVIMENTO.ToString(),
                LTMVPRCO_COD_COBERTURA = LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_COD_COBERTURA.ToString(),
                LTMVPRCO_VAL_IMP_SEGURADA = LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_VAL_IMP_SEGURADA.ToString(),
                LTMVPRCO_VAL_TAXA_PREMIO = LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_VAL_TAXA_PREMIO.ToString(),
            };

            R6610_GRAVAR_MOV_COBER_DB_INSERT_1_Insert1.Execute(r6610_GRAVAR_MOV_COBER_DB_INSERT_1_Insert1);

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6610_SAIDA*/

        [StopWatch]
        /*" R6620-GRAVAR-MOV-BONUS-SECTION */
        private void R6620_GRAVAR_MOV_BONUS_SECTION()
        {
            /*" -3284- IF LTMVPROP-COD-MOVIMENTO = 7 OR LTMVPROP-IND-ALT-BONUS = 'N' */

            if (LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_COD_MOVIMENTO == 7 || LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_BONUS == "N")
            {

                /*" -3286- GO TO R6620-SAIDA. */
                /*Método Suprimido por falta de linha ou apenas EXIT nome: R6620_SAIDA*/ //GOTO
                return;
            }


            /*" -3287- MOVE LTMVPRCO-COD-PRODUTO TO LTMVPRBO-COD-PRODUTO */
            _.Move(LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_COD_PRODUTO, LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_COD_PRODUTO);

            /*" -3288- MOVE LTMVPRCO-COD-EXT-ESTIP TO LTMVPRBO-COD-EXT-ESTIP */
            _.Move(LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_COD_EXT_ESTIP, LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_COD_EXT_ESTIP);

            /*" -3289- MOVE LTMVPRCO-COD-EXT-SEGURADO TO LTMVPRBO-COD-EXT-SEGURADO */
            _.Move(LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_COD_EXT_SEGURADO, LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_COD_EXT_SEGURADO);

            /*" -3290- MOVE LTMVPRCO-DATA-MOVIMENTO TO LTMVPRBO-DATA-MOVIMENTO. */
            _.Move(LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_DATA_MOVIMENTO, LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_DATA_MOVIMENTO);

            /*" -3291- MOVE LTMVPRCO-HORA-MOVIMENTO TO LTMVPRBO-HORA-MOVIMENTO. */
            _.Move(LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_HORA_MOVIMENTO, LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_HORA_MOVIMENTO);

            /*" -3292- MOVE LTMVPRCO-COD-MOVIMENTO TO LTMVPRBO-COD-MOVIMENTO. */
            _.Move(LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_COD_MOVIMENTO, LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_COD_MOVIMENTO);

            /*" -3297- MOVE LTMVPRCO-COD-COBERTURA TO LTMVPRBO-COD-COBERTURA. */
            _.Move(LTMVPRCO.DCLLT_MOV_PROP_COBER.LTMVPRCO_COD_COBERTURA, LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_COD_COBERTURA);

            /*" -3298- IF CAD-BONUS-ALARME EQUAL 1 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_ALARMEX.CAD_BONUS_ALARME == 1)
            {

                /*" -3299- MOVE 2 TO LTMVPRBO-COD-BONUS */
                _.Move(2, LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_COD_BONUS);

                /*" -3303- PERFORM R6630-GRAVAR-MOV-BONUS. */

                R6630_GRAVAR_MOV_BONUS_SECTION();
            }


            /*" -3304- IF CAD-BONUS-CKT EQUAL 1 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_CKTX.CAD_BONUS_CKT == 1)
            {

                /*" -3305- MOVE 3 TO LTMVPRBO-COD-BONUS */
                _.Move(3, LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_COD_BONUS);

                /*" -3309- PERFORM R6630-GRAVAR-MOV-BONUS. */

                R6630_GRAVAR_MOV_BONUS_SECTION();
            }


            /*" -3310- IF CAD-BONUS-COFRE EQUAL 1 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_COFREX.CAD_BONUS_COFRE == 1)
            {

                /*" -3311- MOVE 4 TO LTMVPRBO-COD-BONUS */
                _.Move(4, LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_COD_BONUS);

                /*" -3311- PERFORM R6630-GRAVAR-MOV-BONUS. */

                R6630_GRAVAR_MOV_BONUS_SECTION();
            }


        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6620_SAIDA*/

        [StopWatch]
        /*" R6630-GRAVAR-MOV-BONUS-SECTION */
        private void R6630_GRAVAR_MOV_BONUS_SECTION()
        {
            /*" -3319- MOVE '6630' TO WNR-EXEC-SQL. */
            _.Move("6630", LK_CONVERSAO.WABEND.WNR_EXEC_SQL);

            /*" -3336- PERFORM R6630_GRAVAR_MOV_BONUS_DB_INSERT_1 */

            R6630_GRAVAR_MOV_BONUS_DB_INSERT_1();

            /*" -3339- IF SQLCODE NOT EQUAL ZEROS */

            if (DB.SQLCODE != 00)
            {

                /*" -3340- DISPLAY 'R6630-ERRO INSERT MOVIMENTO BONUS     ' */
                _.Display($"R6630-ERRO INSERT MOVIMENTO BONUS     ");

                /*" -3341- DISPLAY 'COD. LOTERICO   = ' LTMVPRBO-COD-EXT-SEGURADO */
                _.Display($"COD. LOTERICO   = {LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_COD_EXT_SEGURADO}");

                /*" -3342- DISPLAY 'DATA MOVIMENTO  = ' LTMVPRBO-DATA-MOVIMENTO */
                _.Display($"DATA MOVIMENTO  = {LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_DATA_MOVIMENTO}");

                /*" -3343- DISPLAY 'HORA MOVIMENTO  = ' LTMVPRBO-HORA-MOVIMENTO */
                _.Display($"HORA MOVIMENTO  = {LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_HORA_MOVIMENTO}");

                /*" -3344- DISPLAY 'COD. COBERTURA  = ' LTMVPRBO-COD-COBERTURA */
                _.Display($"COD. COBERTURA  = {LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_COD_COBERTURA}");

                /*" -3344- DISPLAY 'COD. BONUS      = ' LTMVPRBO-COD-BONUS. */
                _.Display($"COD. BONUS      = {LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_COD_BONUS}");
            }


        }

        [StopWatch]
        /*" R6630-GRAVAR-MOV-BONUS-DB-INSERT-1 */
        public void R6630_GRAVAR_MOV_BONUS_DB_INSERT_1()
        {
            /*" -3336- EXEC SQL INSERT INTO SEGUROS.LT_MOV_PROP_BONUS (COD_PRODUTO, COD_EXT_ESTIP, COD_EXT_SEGURADO, DATA_MOVIMENTO, HORA_MOVIMENTO, COD_MOVIMENTO, COD_COBERTURA, COD_BONUS) VALUES (:LTMVPRBO-COD-PRODUTO, :LTMVPRBO-COD-EXT-ESTIP, :LTMVPRBO-COD-EXT-SEGURADO, :LTMVPRBO-DATA-MOVIMENTO, :LTMVPRBO-HORA-MOVIMENTO, :LTMVPRBO-COD-MOVIMENTO, :LTMVPRBO-COD-COBERTURA, :LTMVPRBO-COD-BONUS) END-EXEC. */

            var r6630_GRAVAR_MOV_BONUS_DB_INSERT_1_Insert1 = new R6630_GRAVAR_MOV_BONUS_DB_INSERT_1_Insert1()
            {
                LTMVPRBO_COD_PRODUTO = LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_COD_PRODUTO.ToString(),
                LTMVPRBO_COD_EXT_ESTIP = LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_COD_EXT_ESTIP.ToString(),
                LTMVPRBO_COD_EXT_SEGURADO = LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_COD_EXT_SEGURADO.ToString(),
                LTMVPRBO_DATA_MOVIMENTO = LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_DATA_MOVIMENTO.ToString(),
                LTMVPRBO_HORA_MOVIMENTO = LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_HORA_MOVIMENTO.ToString(),
                LTMVPRBO_COD_MOVIMENTO = LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_COD_MOVIMENTO.ToString(),
                LTMVPRBO_COD_COBERTURA = LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_COD_COBERTURA.ToString(),
                LTMVPRBO_COD_BONUS = LTMVPRBO.DCLLT_MOV_PROP_BONUS.LTMVPRBO_COD_BONUS.ToString(),
            };

            R6630_GRAVAR_MOV_BONUS_DB_INSERT_1_Insert1.Execute(r6630_GRAVAR_MOV_BONUS_DB_INSERT_1_Insert1);

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6630_SAIDA*/

        [StopWatch]
        /*" R6700-UPDATE-FC-LOTERICO-SECTION */
        private void R6700_UPDATE_FC_LOTERICO_SECTION()
        {
            /*" -3355- MOVE '0008' TO WNR-EXEC-SQL. */
            _.Move("0008", LK_CONVERSAO.WABEND.WNR_EXEC_SQL);

            /*" -3435- PERFORM R6700_UPDATE_FC_LOTERICO_DB_UPDATE_1 */

            R6700_UPDATE_FC_LOTERICO_DB_UPDATE_1();

            /*" -3439- IF SQLCODE EQUAL ZEROS */

            if (DB.SQLCODE == 00)
            {

                /*" -3440- ADD 1 TO WS-FCLOT-TOTAL-ALTERADOS */
                AREA_DE_WORK.WS_FCLOT_TOTAL_ALTERADOS.Value = AREA_DE_WORK.WS_FCLOT_TOTAL_ALTERADOS + 1;

                /*" -3441- ADD 1 TO W-AC-LOTERICOS-GRAVADOS */
                AREA_DE_WORK.W_AC_LOTERICOS_GRAVADOS.Value = AREA_DE_WORK.W_AC_LOTERICOS_GRAVADOS + 1;

                /*" -3442- ELSE */
            }
            else
            {


                /*" -3443- DISPLAY '6700-ERRO UPDATE FCLOTERICO................... ' */
                _.Display($"6700-ERRO UPDATE FCLOTERICO................... ");

                /*" -3444- DISPLAY 'COD. LOTERICO   = ' FCLOTERI-NUM-LOTERICO */
                _.Display($"COD. LOTERICO   = {FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_LOTERICO}");

                /*" -3445- DISPLAY 'DTH-EXC=' FCLOTERI-DTH-EXCLUSAO */
                _.Display($"DTH-EXC={FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DTH_EXCLUSAO}");

                /*" -3446- DISPLAY 'DTH-INC=' FCLOTERI-DTH-INCLUSAO */
                _.Display($"DTH-INC={FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DTH_INCLUSAO}");

                /*" -3447- DISPLAY 'IDE-CONTA-CAUCAO=' FCLOTERI-IDE-CONTA-CAUCAO */
                _.Display($"IDE-CONTA-CAUCAO={FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IDE_CONTA_CAUCAO}");

                /*" -3448- DISPLAY 'IDE-CONTA-CPMF=' FCLOTERI-IDE-CONTA-CPMF */
                _.Display($"IDE-CONTA-CPMF={FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IDE_CONTA_CPMF}");

                /*" -3449- DISPLAY 'IDE-CONTA-ISENTA=' FCLOTERI-IDE-CONTA-ISENTA */
                _.Display($"IDE-CONTA-ISENTA={FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IDE_CONTA_ISENTA}");

                /*" -3450- DISPLAY 'VIND-DTH-EXC=' VIND-DTH-EXCLUSAO */
                _.Display($"VIND-DTH-EXC={VIND_DTH_EXCLUSAO}");

                /*" -3451- DISPLAY 'VIND-DTH-INC=' VIND-DTH-INCLUSAO */
                _.Display($"VIND-DTH-INC={VIND_DTH_INCLUSAO}");

                /*" -3452- DISPLAY 'VIND-DTH-CONTA-CAUCAO=' VIND-IDE-CONTA-CAUCAO */
                _.Display($"VIND-DTH-CONTA-CAUCAO={VIND_IDE_CONTA_CAUCAO}");

                /*" -3453- DISPLAY 'VIND-DTH-CONTA-CPMF=' VIND-IDE-CONTA-CPMF */
                _.Display($"VIND-DTH-CONTA-CPMF={VIND_IDE_CONTA_CPMF}");

                /*" -3454- DISPLAY 'VIND-IDE-CONTA-ISENTA=' VIND-IDE-CONTA-ISENTA */
                _.Display($"VIND-IDE-CONTA-ISENTA={VIND_IDE_CONTA_ISENTA}");

                /*" -3454- GO TO R9999-ROT-ERRO. */

                R9999_ROT_ERRO_SECTION(); //GOTO
                return;
            }


        }

        [StopWatch]
        /*" R6700-UPDATE-FC-LOTERICO-DB-UPDATE-1 */
        public void R6700_UPDATE_FC_LOTERICO_DB_UPDATE_1()
        {
            /*" -3435- EXEC SQL UPDATE FDRCAP.FC_LOTERICO SET COD_AGENTE_MASTER =:FCLOTERI-COD-AGENTE-MASTER :VIND-COD-AGENTE-MASTER, COD_CGC =:FCLOTERI-COD-CGC :VIND-COD-CGC, COD_INSCR_ESTAD =:FCLOTERI-COD-INSCR-ESTAD :VIND-COD-INSCR-ESTAD , COD_INSCR_MUNIC =:FCLOTERI-COD-INSCR-MUNIC :VIND-COD-INSCR-MUNIC, COD_MUNICIPIO =:FCLOTERI-COD-MUNICIPIO :VIND-COD-MUNICIPIO, COD_UF =:FCLOTERI-COD-UF :VIND-COD-UF, DES_EMAIL =:FCLOTERI-DES-EMAIL :VIND-DES-EMAIL, DES_ENDERECO =:FCLOTERI-DES-ENDERECO :VIND-DES-ENDERECO, DTH_EXCLUSAO =:FCLOTERI-DTH-EXCLUSAO :VIND-DTH-EXCLUSAO, DTH_INCLUSAO =:FCLOTERI-DTH-INCLUSAO :VIND-DTH-INCLUSAO, IDE_CONTA_CAUCAO =:FCLOTERI-IDE-CONTA-CAUCAO :VIND-IDE-CONTA-CAUCAO, IDE_CONTA_CPMF =:FCLOTERI-IDE-CONTA-CPMF :VIND-IDE-CONTA-CPMF, IDE_CONTA_ISENTA =:FCLOTERI-IDE-CONTA-ISENTA :VIND-IDE-CONTA-ISENTA, IND_CAT_LOTERICO =:FCLOTERI-IND-CAT-LOTERICO :VIND-IND-CAT-LOTERICO, IND_STA_LOTERICO =:FCLOTERI-IND-STA-LOTERICO :VIND-IND-STA-LOTERICO, NOM_BAIRRO =:FCLOTERI-NOM-BAIRRO :VIND-NOM-BAIRRO, NOM_CONSULTOR =:FCLOTERI-NOM-CONSULTOR :VIND-NOM-CONSULTOR, NOM_CONTATO1 =:FCLOTERI-NOM-CONTATO1 :VIND-NOM-CONTATO1, NOM_CONTATO2 =:FCLOTERI-NOM-CONTATO2 :VIND-NOM-CONTATO2, NOM_FANTASIA =:FCLOTERI-NOM-FANTASIA :VIND-NOM-FANTASIA, NOM_MUNICIPIO =:FCLOTERI-NOM-MUNICIPIO :VIND-NOM-MUNICIPIO, NOM_RAZAO_SOCIAL =:FCLOTERI-NOM-RAZAO-SOCIAL :VIND-NOM-RAZAO-SOCIAL, NUM_CEP =:FCLOTERI-NUM-CEP :VIND-NUM-CEP, NUM_ENCEF =:FCLOTERI-NUM-ENCEF :VIND-NUM-ENCEF, NUM_LOTER_ANT =:FCLOTERI-NUM-LOTER-ANT :VIND-NUM-LOTER-ANT, NUM_MATR_CONSULTOR =:FCLOTERI-NUM-MATR-CONSULTOR :VIND-NUM-MATR-CON, NUM_PVCEF =:FCLOTERI-NUM-PVCEF :VIND-NUM-PVCEF, NUM_TELEFONE =:FCLOTERI-NUM-TELEFONE :VIND-NUM-TELEFONE, STA_DADOS_N_CADAST =:FCLOTERI-STA-DADOS-N-CADAST :VIND-STA-DADOS-N, STA_LOTERICO =:FCLOTERI-STA-LOTERICO :VIND-STA-LOTERICO, STA_NIVEL_COMIS =:FCLOTERI-STA-NIVEL-COMIS :VIND-STA-NIVEL-COMIS, STA_ULT_ALT_ONLINE =:FCLOTERI-STA-ULT-ALT-ONLINE :VIND-STA-ULT-ALT, COD_GARANTIA =:FCLOTERI-COD-GARANTIA :VIND-COD-GARANTIA, VLR_GARANTIA =:FCLOTERI-VLR-GARANTIA :VIND-VLR-GARANTIA, DTH_GERACAO =:FCLOTERI-DTH-GERACAO :VIND-DTH-GERACAO, NUM_DV_LOTERICO =:FCLOTERI-NUM-DV-LOTERICO, NUM_FAX =:FCLOTERI-NUM-FAX :VIND-NUM-FAX, NUM_SEGURADORA =:FCLOTERI-NUM-SEGURADORA WHERE NUM_LOTERICO =:FCLOTERI-NUM-LOTERICO END-EXEC. */

            var r6700_UPDATE_FC_LOTERICO_DB_UPDATE_1_Update1 = new R6700_UPDATE_FC_LOTERICO_DB_UPDATE_1_Update1()
            {
                FCLOTERI_COD_AGENTE_MASTER = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_AGENTE_MASTER.ToString(),
                VIND_COD_AGENTE_MASTER = VIND_COD_AGENTE_MASTER.ToString(),
                FCLOTERI_IDE_CONTA_CAUCAO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IDE_CONTA_CAUCAO.ToString(),
                VIND_IDE_CONTA_CAUCAO = VIND_IDE_CONTA_CAUCAO.ToString(),
                FCLOTERI_IDE_CONTA_ISENTA = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IDE_CONTA_ISENTA.ToString(),
                VIND_IDE_CONTA_ISENTA = VIND_IDE_CONTA_ISENTA.ToString(),
                FCLOTERI_IND_CAT_LOTERICO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IND_CAT_LOTERICO.ToString(),
                VIND_IND_CAT_LOTERICO = VIND_IND_CAT_LOTERICO.ToString(),
                FCLOTERI_IND_STA_LOTERICO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IND_STA_LOTERICO.ToString(),
                VIND_IND_STA_LOTERICO = VIND_IND_STA_LOTERICO.ToString(),
                FCLOTERI_NOM_RAZAO_SOCIAL = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_RAZAO_SOCIAL.ToString(),
                VIND_NOM_RAZAO_SOCIAL = VIND_NOM_RAZAO_SOCIAL.ToString(),
                FCLOTERI_COD_INSCR_ESTAD = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_INSCR_ESTAD.ToString(),
                VIND_COD_INSCR_ESTAD = VIND_COD_INSCR_ESTAD.ToString(),
                FCLOTERI_COD_INSCR_MUNIC = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_INSCR_MUNIC.ToString(),
                VIND_COD_INSCR_MUNIC = VIND_COD_INSCR_MUNIC.ToString(),
                FCLOTERI_NUM_MATR_CONSULTOR = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_MATR_CONSULTOR.ToString(),
                VIND_NUM_MATR_CON = VIND_NUM_MATR_CON.ToString(),
                FCLOTERI_STA_NIVEL_COMIS = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_STA_NIVEL_COMIS.ToString(),
                VIND_STA_NIVEL_COMIS = VIND_STA_NIVEL_COMIS.ToString(),
                FCLOTERI_STA_DADOS_N_CADAST = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_STA_DADOS_N_CADAST.ToString(),
                VIND_STA_DADOS_N = VIND_STA_DADOS_N.ToString(),
                FCLOTERI_STA_ULT_ALT_ONLINE = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_STA_ULT_ALT_ONLINE.ToString(),
                VIND_STA_ULT_ALT = VIND_STA_ULT_ALT.ToString(),
                FCLOTERI_IDE_CONTA_CPMF = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_IDE_CONTA_CPMF.ToString(),
                VIND_IDE_CONTA_CPMF = VIND_IDE_CONTA_CPMF.ToString(),
                FCLOTERI_COD_MUNICIPIO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_MUNICIPIO.ToString(),
                VIND_COD_MUNICIPIO = VIND_COD_MUNICIPIO.ToString(),
                FCLOTERI_NOM_CONSULTOR = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_CONSULTOR.ToString(),
                VIND_NOM_CONSULTOR = VIND_NOM_CONSULTOR.ToString(),
                FCLOTERI_NOM_MUNICIPIO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_MUNICIPIO.ToString(),
                VIND_NOM_MUNICIPIO = VIND_NOM_MUNICIPIO.ToString(),
                FCLOTERI_NUM_LOTER_ANT = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_LOTER_ANT.ToString(),
                VIND_NUM_LOTER_ANT = VIND_NUM_LOTER_ANT.ToString(),
                FCLOTERI_DES_ENDERECO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DES_ENDERECO.ToString(),
                VIND_DES_ENDERECO = VIND_DES_ENDERECO.ToString(),
                FCLOTERI_DTH_EXCLUSAO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DTH_EXCLUSAO.ToString(),
                VIND_DTH_EXCLUSAO = VIND_DTH_EXCLUSAO.ToString(),
                FCLOTERI_DTH_INCLUSAO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DTH_INCLUSAO.ToString(),
                VIND_DTH_INCLUSAO = VIND_DTH_INCLUSAO.ToString(),
                FCLOTERI_NOM_CONTATO1 = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_CONTATO1.ToString(),
                VIND_NOM_CONTATO1 = VIND_NOM_CONTATO1.ToString(),
                FCLOTERI_NOM_CONTATO2 = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_CONTATO2.ToString(),
                VIND_NOM_CONTATO2 = VIND_NOM_CONTATO2.ToString(),
                FCLOTERI_NOM_FANTASIA = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_FANTASIA.ToString(),
                VIND_NOM_FANTASIA = VIND_NOM_FANTASIA.ToString(),
                FCLOTERI_NUM_TELEFONE = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_TELEFONE.ToString(),
                VIND_NUM_TELEFONE = VIND_NUM_TELEFONE.ToString(),
                FCLOTERI_STA_LOTERICO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_STA_LOTERICO.ToString(),
                VIND_STA_LOTERICO = VIND_STA_LOTERICO.ToString(),
                FCLOTERI_COD_GARANTIA = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_GARANTIA.ToString(),
                VIND_COD_GARANTIA = VIND_COD_GARANTIA.ToString(),
                FCLOTERI_VLR_GARANTIA = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_VLR_GARANTIA.ToString(),
                VIND_VLR_GARANTIA = VIND_VLR_GARANTIA.ToString(),
                FCLOTERI_DTH_GERACAO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DTH_GERACAO.ToString(),
                VIND_DTH_GERACAO = VIND_DTH_GERACAO.ToString(),
                FCLOTERI_NOM_BAIRRO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NOM_BAIRRO.ToString(),
                VIND_NOM_BAIRRO = VIND_NOM_BAIRRO.ToString(),
                FCLOTERI_DES_EMAIL = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_DES_EMAIL.ToString(),
                VIND_DES_EMAIL = VIND_DES_EMAIL.ToString(),
                FCLOTERI_NUM_ENCEF = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_ENCEF.ToString(),
                VIND_NUM_ENCEF = VIND_NUM_ENCEF.ToString(),
                FCLOTERI_NUM_PVCEF = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_PVCEF.ToString(),
                VIND_NUM_PVCEF = VIND_NUM_PVCEF.ToString(),
                FCLOTERI_COD_CGC = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_CGC.ToString(),
                VIND_COD_CGC = VIND_COD_CGC.ToString(),
                FCLOTERI_NUM_CEP = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_CEP.ToString(),
                VIND_NUM_CEP = VIND_NUM_CEP.ToString(),
                FCLOTERI_NUM_FAX = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_FAX.ToString(),
                VIND_NUM_FAX = VIND_NUM_FAX.ToString(),
                FCLOTERI_COD_UF = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_COD_UF.ToString(),
                VIND_COD_UF = VIND_COD_UF.ToString(),
                FCLOTERI_NUM_DV_LOTERICO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_DV_LOTERICO.ToString(),
                FCLOTERI_NUM_SEGURADORA = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_SEGURADORA.ToString(),
                FCLOTERI_NUM_LOTERICO = FCLOTERI.DCLFC_LOTERICO.FCLOTERI_NUM_LOTERICO.ToString(),
            };

            R6700_UPDATE_FC_LOTERICO_DB_UPDATE_1_Update1.Execute(r6700_UPDATE_FC_LOTERICO_DB_UPDATE_1_Update1);

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6700_SAIDA*/

        [StopWatch]
        /*" R6750-DELETE-FC-PEND-LOTERICO-SECTION */
        private void R6750_DELETE_FC_PEND_LOTERICO_SECTION()
        {
            /*" -3462- MOVE '0008' TO WNR-EXEC-SQL. */
            _.Move("0008", LK_CONVERSAO.WABEND.WNR_EXEC_SQL);

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6750_SAIDA*/

        [StopWatch]
        /*" R6800-SELECT-BONUS-SECTION */
        private void R6800_SELECT_BONUS_SECTION()
        {
            /*" -3498- MOVE '6800' TO WNR-EXEC-SQL. */
            _.Move("6800", LK_CONVERSAO.WABEND.WNR_EXEC_SQL);

            /*" -3506- PERFORM R6800_SELECT_BONUS_DB_SELECT_1 */

            R6800_SELECT_BONUS_DB_SELECT_1();

            /*" -3509- IF SQLCODE NOT EQUAL ZEROS */

            if (DB.SQLCODE != 00)
            {

                /*" -3510- IF SQLCODE NOT EQUAL 100 */

                if (DB.SQLCODE != 100)
                {

                    /*" -3511- DISPLAY 'ERRO SELECT LT_LOTERICO_BONUS....' */
                    _.Display($"ERRO SELECT LT_LOTERICO_BONUS....");

                    /*" -3512- DISPLAY 'COD. LOTERICO   = ' CAD-COD-CEF */
                    _.Display($"COD. LOTERICO   = {AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF_R.CAD_COD_CEF}");

                    /*" -3513- DISPLAY 'BONUS COM ERRO  = ' LTLOTBON-COD-BONUS */
                    _.Display($"BONUS COM ERRO  = {LTLOTBON.DCLLT_LOTERICO_BONUS.LTLOTBON_COD_BONUS}");

                    /*" -3513- GO TO R9999-ROT-ERRO. */

                    R9999_ROT_ERRO_SECTION(); //GOTO
                    return;
                }

            }


        }

        [StopWatch]
        /*" R6800-SELECT-BONUS-DB-SELECT-1 */
        public void R6800_SELECT_BONUS_DB_SELECT_1()
        {
            /*" -3506- EXEC SQL SELECT NUM_LOTERICO , COD_BONUS INTO :LTLOTBON-NUM-LOTERICO, :LTLOTBON-COD-BONUS FROM SEGUROS.LT_LOTERICO_BONUS WHERE NUM_LOTERICO = :LTLOTBON-NUM-LOTERICO AND COD_BONUS = :LTLOTBON-COD-BONUS END-EXEC. */

            var r6800_SELECT_BONUS_DB_SELECT_1_Query1 = new R6800_SELECT_BONUS_DB_SELECT_1_Query1()
            {
                LTLOTBON_NUM_LOTERICO = LTLOTBON.DCLLT_LOTERICO_BONUS.LTLOTBON_NUM_LOTERICO.ToString(),
                LTLOTBON_COD_BONUS = LTLOTBON.DCLLT_LOTERICO_BONUS.LTLOTBON_COD_BONUS.ToString(),
            };

            var executed_1 = R6800_SELECT_BONUS_DB_SELECT_1_Query1.Execute(r6800_SELECT_BONUS_DB_SELECT_1_Query1);
            if (executed_1 != null)
            {
                _.Move(executed_1.LTLOTBON_NUM_LOTERICO, LTLOTBON.DCLLT_LOTERICO_BONUS.LTLOTBON_NUM_LOTERICO);
                _.Move(executed_1.LTLOTBON_COD_BONUS, LTLOTBON.DCLLT_LOTERICO_BONUS.LTLOTBON_COD_BONUS);
            }


        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6800_SAIDA*/

        [StopWatch]
        /*" R6810-GRAVAR-LOTERICO-BONUS-SECTION */
        private void R6810_GRAVAR_LOTERICO_BONUS_SECTION()
        {
            /*" -3525- MOVE CAD-COD-CEF TO LTLOTBON-NUM-LOTERICO. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF_R.CAD_COD_CEF, LTLOTBON.DCLLT_LOTERICO_BONUS.LTLOTBON_NUM_LOTERICO);

            /*" -3526- IF CAD-BONUS-ALARME EQUAL 1 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_ALARMEX.CAD_BONUS_ALARME == 1)
            {

                /*" -3527- MOVE 2 TO LTLOTBON-COD-BONUS */
                _.Move(2, LTLOTBON.DCLLT_LOTERICO_BONUS.LTLOTBON_COD_BONUS);

                /*" -3531- PERFORM R6830-INSERT-BONUS. */

                R6830_INSERT_BONUS_SECTION();
            }


            /*" -3532- IF CAD-BONUS-CKT EQUAL 1 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_CKTX.CAD_BONUS_CKT == 1)
            {

                /*" -3533- MOVE 3 TO LTLOTBON-COD-BONUS */
                _.Move(3, LTLOTBON.DCLLT_LOTERICO_BONUS.LTLOTBON_COD_BONUS);

                /*" -3537- PERFORM R6830-INSERT-BONUS. */

                R6830_INSERT_BONUS_SECTION();
            }


            /*" -3538- IF CAD-BONUS-COFRE EQUAL 1 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_COFREX.CAD_BONUS_COFRE == 1)
            {

                /*" -3539- MOVE 4 TO LTLOTBON-COD-BONUS */
                _.Move(4, LTLOTBON.DCLLT_LOTERICO_BONUS.LTLOTBON_COD_BONUS);

                /*" -3539- PERFORM R6830-INSERT-BONUS. */

                R6830_INSERT_BONUS_SECTION();
            }


        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6810_SAIDA*/

        [StopWatch]
        /*" R6820-DELETE-BONUS-SECTION */
        private void R6820_DELETE_BONUS_SECTION()
        {
            /*" -3547- MOVE '6820' TO WNR-EXEC-SQL. */
            _.Move("6820", LK_CONVERSAO.WABEND.WNR_EXEC_SQL);

            /*" -3551- PERFORM R6820_DELETE_BONUS_DB_DELETE_1 */

            R6820_DELETE_BONUS_DB_DELETE_1();

            /*" -3555- IF SQLCODE NOT EQUAL ZEROS AND SQLCODE NOT EQUAL 100 */

            if (DB.SQLCODE != 00 && DB.SQLCODE != 100)
            {

                /*" -3557- DISPLAY ' R6820-ERRO DELETE LT_LOTERICO_BONUS COD-CEF:' CAD-COD-CEF */
                _.Display($" R6820-ERRO DELETE LT_LOTERICO_BONUS COD-CEF:{AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF_R.CAD_COD_CEF}");

                /*" -3557- GO TO R9999-ROT-ERRO. */

                R9999_ROT_ERRO_SECTION(); //GOTO
                return;
            }


        }

        [StopWatch]
        /*" R6820-DELETE-BONUS-DB-DELETE-1 */
        public void R6820_DELETE_BONUS_DB_DELETE_1()
        {
            /*" -3551- EXEC SQL DELETE FROM SEGUROS.LT_LOTERICO_BONUS WHERE NUM_LOTERICO =:LTLOTBON-NUM-LOTERICO END-EXEC. */

            var r6820_DELETE_BONUS_DB_DELETE_1_Delete1 = new R6820_DELETE_BONUS_DB_DELETE_1_Delete1()
            {
                LTLOTBON_NUM_LOTERICO = LTLOTBON.DCLLT_LOTERICO_BONUS.LTLOTBON_NUM_LOTERICO.ToString(),
            };

            R6820_DELETE_BONUS_DB_DELETE_1_Delete1.Execute(r6820_DELETE_BONUS_DB_DELETE_1_Delete1);

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6820_SAIDA*/

        [StopWatch]
        /*" R6830-INSERT-BONUS-SECTION */
        private void R6830_INSERT_BONUS_SECTION()
        {
            /*" -3565- MOVE '0010' TO WNR-EXEC-SQL. */
            _.Move("0010", LK_CONVERSAO.WABEND.WNR_EXEC_SQL);

            /*" -3571- PERFORM R6830_INSERT_BONUS_DB_INSERT_1 */

            R6830_INSERT_BONUS_DB_INSERT_1();

            /*" -3574- IF SQLCODE NOT EQUAL ZEROS */

            if (DB.SQLCODE != 00)
            {

                /*" -3576- DISPLAY ' R6830-ERRO INSERT LT_LOTERICO_BONUS' CAD-COD-CEF */
                _.Display($" R6830-ERRO INSERT LT_LOTERICO_BONUS{AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF_R.CAD_COD_CEF}");

                /*" -3576- GO TO R9999-ROT-ERRO. */

                R9999_ROT_ERRO_SECTION(); //GOTO
                return;
            }


        }

        [StopWatch]
        /*" R6830-INSERT-BONUS-DB-INSERT-1 */
        public void R6830_INSERT_BONUS_DB_INSERT_1()
        {
            /*" -3571- EXEC SQL INSERT INTO SEGUROS.LT_LOTERICO_BONUS (NUM_LOTERICO , COD_BONUS) VALUES (:LTLOTBON-NUM-LOTERICO , :LTLOTBON-COD-BONUS) END-EXEC. */

            var r6830_INSERT_BONUS_DB_INSERT_1_Insert1 = new R6830_INSERT_BONUS_DB_INSERT_1_Insert1()
            {
                LTLOTBON_NUM_LOTERICO = LTLOTBON.DCLLT_LOTERICO_BONUS.LTLOTBON_NUM_LOTERICO.ToString(),
                LTLOTBON_COD_BONUS = LTLOTBON.DCLLT_LOTERICO_BONUS.LTLOTBON_COD_BONUS.ToString(),
            };

            R6830_INSERT_BONUS_DB_INSERT_1_Insert1.Execute(r6830_INSERT_BONUS_DB_INSERT_1_Insert1);

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6830_SAIDA*/

        [StopWatch]
        /*" R6850-VER-ALTERACAO-BONUS-SECTION */
        private void R6850_VER_ALTERACAO_BONUS_SECTION()
        {
            /*" -3587- MOVE 0 TO WS-ALARME WS-CKT WS-COFRE. */
            _.Move(0, WS_ALARME, WS_CKT, WS_COFRE);

            /*" -3588- MOVE CAD-COD-CEF TO LTLOTBON-NUM-LOTERICO. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF_R.CAD_COD_CEF, LTLOTBON.DCLLT_LOTERICO_BONUS.LTLOTBON_NUM_LOTERICO);

            /*" -3589- MOVE 2 TO LTLOTBON-COD-BONUS. */
            _.Move(2, LTLOTBON.DCLLT_LOTERICO_BONUS.LTLOTBON_COD_BONUS);

            /*" -3590- PERFORM R6800-SELECT-BONUS. */

            R6800_SELECT_BONUS_SECTION();

            /*" -3591- IF SQLCODE = ZEROS */

            if (DB.SQLCODE == 00)
            {

                /*" -3593- MOVE 1 TO WS-ALARME. */
                _.Move(1, WS_ALARME);
            }


            /*" -3594- MOVE 3 TO LTLOTBON-COD-BONUS. */
            _.Move(3, LTLOTBON.DCLLT_LOTERICO_BONUS.LTLOTBON_COD_BONUS);

            /*" -3595- PERFORM R6800-SELECT-BONUS. */

            R6800_SELECT_BONUS_SECTION();

            /*" -3596- IF SQLCODE = ZEROS */

            if (DB.SQLCODE == 00)
            {

                /*" -3598- MOVE 1 TO WS-CKT. */
                _.Move(1, WS_CKT);
            }


            /*" -3599- MOVE 4 TO LTLOTBON-COD-BONUS. */
            _.Move(4, LTLOTBON.DCLLT_LOTERICO_BONUS.LTLOTBON_COD_BONUS);

            /*" -3600- PERFORM R6800-SELECT-BONUS. */

            R6800_SELECT_BONUS_SECTION();

            /*" -3601- IF SQLCODE = ZEROS */

            if (DB.SQLCODE == 00)
            {

                /*" -3603- MOVE 1 TO WS-COFRE. */
                _.Move(1, WS_COFRE);
            }


            /*" -3604- IF CAD-BONUS-ALARME NOT EQUAL 1 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_ALARMEX.CAD_BONUS_ALARME != 1)
            {

                /*" -3605- MOVE ZEROS TO CAD-BONUS-ALARME. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_ALARMEX.CAD_BONUS_ALARME);
            }


            /*" -3606- IF CAD-BONUS-CKT NOT EQUAL 1 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_CKTX.CAD_BONUS_CKT != 1)
            {

                /*" -3607- MOVE ZEROS TO CAD-BONUS-CKT. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_CKTX.CAD_BONUS_CKT);
            }


            /*" -3608- IF CAD-BONUS-COFRE NOT EQUAL 1 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_COFREX.CAD_BONUS_COFRE != 1)
            {

                /*" -3610- MOVE ZEROS TO CAD-BONUS-COFRE. */
                _.Move(0, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_COFREX.CAD_BONUS_COFRE);
            }


            /*" -3613- IF CAD-BONUS-ALARME EQUAL WS-ALARME AND CAD-BONUS-CKT EQUAL WS-CKT AND CAD-BONUS-COFRE EQUAL WS-COFRE */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_ALARMEX.CAD_BONUS_ALARME == WS_ALARME && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_CKTX.CAD_BONUS_CKT == WS_CKT && AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_COFREX.CAD_BONUS_COFRE == WS_COFRE)
            {

                /*" -3615- GO TO R6850-SAIDA. */
                /*Método Suprimido por falta de linha ou apenas EXIT nome: R6850_SAIDA*/ //GOTO
                return;
            }


            /*" -3617- DISPLAY ' ALT BONUS =' CAD-CODIGO-CEF. */
            _.Display($" ALT BONUS ={AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF}");

            /*" -3619- MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO W-CHAVE-ALTEROU-SEGURADO */
            _.Move("SIM", W_CHAVE_HOUVE_ALTERACAO, W_CHAVE_ALTEROU_SEGURADO);

            /*" -3619- MOVE 'S' TO LTMVPROP-IND-ALT-BONUS. */
            _.Move("S", LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_BONUS);

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6850_SAIDA*/

        [StopWatch]
        /*" R6900-ALTERAR-BONUS-SECTION */
        private void R6900_ALTERAR_BONUS_SECTION()
        {
            /*" -3631- IF LTMVPROP-IND-ALT-BONUS = 'N' */

            if (LTMVPROP.DCLLT_MOV_PROPOSTA.LTMVPROP_IND_ALT_BONUS == "N")
            {

                /*" -3633- GO TO R6900-SAIDA. */
                /*Método Suprimido por falta de linha ou apenas EXIT nome: R6900_SAIDA*/ //GOTO
                return;
            }


            /*" -3638- PERFORM R6820-DELETE-BONUS. */

            R6820_DELETE_BONUS_SECTION();

            /*" -3639- IF CAD-BONUS-ALARME EQUAL 1 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_ALARMEX.CAD_BONUS_ALARME == 1)
            {

                /*" -3640- MOVE 2 TO LTLOTBON-COD-BONUS */
                _.Move(2, LTLOTBON.DCLLT_LOTERICO_BONUS.LTLOTBON_COD_BONUS);

                /*" -3644- PERFORM R6830-INSERT-BONUS. */

                R6830_INSERT_BONUS_SECTION();
            }


            /*" -3645- IF CAD-BONUS-CKT EQUAL 1 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_CKTX.CAD_BONUS_CKT == 1)
            {

                /*" -3646- MOVE 3 TO LTLOTBON-COD-BONUS */
                _.Move(3, LTLOTBON.DCLLT_LOTERICO_BONUS.LTLOTBON_COD_BONUS);

                /*" -3650- PERFORM R6830-INSERT-BONUS. */

                R6830_INSERT_BONUS_SECTION();
            }


            /*" -3651- IF CAD-BONUS-COFRE EQUAL 1 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_COFREX.CAD_BONUS_COFRE == 1)
            {

                /*" -3652- MOVE 4 TO LTLOTBON-COD-BONUS */
                _.Move(4, LTLOTBON.DCLLT_LOTERICO_BONUS.LTLOTBON_COD_BONUS);

                /*" -3652- PERFORM R6830-INSERT-BONUS. */

                R6830_INSERT_BONUS_SECTION();
            }


        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6900_SAIDA*/

        [StopWatch]
        /*" R7500-IMPRIME-CADASTRO-SECTION */
        private void R7500_IMPRIME_CADASTRO_SECTION()
        {
            /*" -3663- MOVE CAD-COD-CEFX TO LD01-CAD-COD-CEF */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX, AREA_DE_WORK.LD01_CAD.LD01_CAD_COD_CEF);

            /*" -3664- MOVE CAD-RAZAO-SOCIAL TO LD01-CAD-RAZAO-SOCIAL */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_RAZAO_SOCIAL, AREA_DE_WORK.LD01_CAD.LD01_CAD_RAZAO_SOCIAL);

            /*" -3666- MOVE CAD-SITUACAOX TO LD01-SITUACAO */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_SITUACAOX, AREA_DE_WORK.LD01_CAD.LD01_SITUACAO);

            /*" -3667- MOVE CAD-CGC TO W-CGC. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CGCX.CAD_CGC, AREA_DE_WORK.W_CGC);

            /*" -3668- MOVE W-CGC-8 TO W-CGC-8-ED */
            _.Move(AREA_DE_WORK.W_CGC.W_CGC_8, AREA_DE_WORK.W_CGC_ED.W_CGC_8_ED);

            /*" -3669- MOVE W-CGC-4 TO W-CGC-4-ED */
            _.Move(AREA_DE_WORK.W_CGC.W_CGC_4, AREA_DE_WORK.W_CGC_ED.W_CGC_4_ED);

            /*" -3670- MOVE W-CGC-2 TO W-CGC-2-ED */
            _.Move(AREA_DE_WORK.W_CGC.W_CGC_2, AREA_DE_WORK.W_CGC_ED.W_CGC_2_ED);

            /*" -3671- MOVE W-CGC-ED TO LD02-CAD-CGC */
            _.Move(AREA_DE_WORK.W_CGC_ED, AREA_DE_WORK.LD02_CAD.LD02_CAD_CGC);

            /*" -3672- MOVE CAD-ENDERECO TO LD02-CAD-END */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_ENDERECO, AREA_DE_WORK.LD02_CAD.LD02_CAD_END);

            /*" -3674- MOVE CAD-BAIRRO TO LD02-CAD-BAIRRO */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BAIRRO, AREA_DE_WORK.LD02_CAD.LD02_CAD_BAIRRO);

            /*" -3675- MOVE CAD-INSC-MUNX TO LD02A-CAD-INSC-MUN */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_INSC_MUNX, AREA_DE_WORK.LD02A_CAD.LD02A_CAD_INSC_MUN);

            /*" -3676- MOVE CAD-INSC-ESTX TO LD02A-CAD-INSC-EST */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_INSC_ESTX, AREA_DE_WORK.LD02A_CAD.LD02A_CAD_INSC_EST);

            /*" -3677- MOVE CAD-NUM-LOT-ANTERIORX TO LD02B-LOT-ANT */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_LOT_ANTERIORX, AREA_DE_WORK.LD02B_CAD.LD02B_LOT_ANT);

            /*" -3678- MOVE CAD-COD-AG-MASTERX TO LD02B-AGE-MASTER */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_AG_MASTERX, AREA_DE_WORK.LD02B_CAD.LD02B_AGE_MASTER);

            /*" -3679- MOVE CAD-CAT-LOTERICOX TO LD02B-CATEGORIA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CAT_LOTERICOX, AREA_DE_WORK.LD02B_CAD.LD02B_CATEGORIA);

            /*" -3681- MOVE CAD-COD-STATUSX TO LD02B-STATUS */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_STATUSX, AREA_DE_WORK.LD02B_CAD.LD02B_STATUS);

            /*" -3682- MOVE CAD-CEP TO LD03-CAD-CEP-1. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CEPX.CAD_CEP, AREA_DE_WORK.LD03_CAD.LD03_CAD_CEP_1);

            /*" -3683- MOVE CAD-CIDADE TO LD03-CAD-CIDADE */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CIDADE, AREA_DE_WORK.LD03_CAD.LD03_CAD_CIDADE);

            /*" -3684- MOVE CAD-UF TO LD03-CAD-UF */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_UF, AREA_DE_WORK.LD03_CAD.LD03_CAD_UF);

            /*" -3685- MOVE CAD-DDD-FONE TO LD03-CAD-DDD-FONE */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TELEFONE.CAD_DDD_FONE, AREA_DE_WORK.LD03_CAD.LD03_CAD_DDD_FONE);

            /*" -3686- MOVE CAD-FONE TO LD03-CAD-FONE */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TELEFONE.CAD_FONE, AREA_DE_WORK.LD03_CAD.LD03_CAD_FONE);

            /*" -3687- MOVE CAD-DDD-FAX TO LD03-CAD-DDD-FAX */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUMERO_FAX.CAD_DDD_FAX, AREA_DE_WORK.LD03_CAD.LD03_CAD_DDD_FAX);

            /*" -3689- MOVE CAD-FAX TO LD03-CAD-FAX */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUMERO_FAX.CAD_FAX, AREA_DE_WORK.LD03_CAD.LD03_CAD_FAX);

            /*" -3690- MOVE CAD-TIPO-GARANTIAX TO LD04-CAD-TIPO-GARANTIA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_TIPO_GARANTIAX, AREA_DE_WORK.LD04_CAD_A.LD04_CAD_TIPO_GARANTIA);

            /*" -3691- MOVE CAD-VALOR-GARANTIA TO LD04-CAD-VALORES */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_VALOR_GARANTIAX.CAD_VALOR_GARANTIA, AREA_DE_WORK.LD04_CAD_A.LD04_CAD_VALORES);

            /*" -3692- MOVE CAD-BONUS-ALARMEX TO LD04-CAD-BONUS-ALA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_ALARMEX, AREA_DE_WORK.LD04_CAD_B.LD04_CAD_BONUS_ALA);

            /*" -3693- MOVE CAD-BONUS-CKTX TO LD04-CAD-BONUS-CKT */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_CKTX, AREA_DE_WORK.LD04_CAD_B.LD04_CAD_BONUS_CKT);

            /*" -3695- MOVE CAD-BONUS-COFREX TO LD04-CAD-BONUS-COF */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BONUS_COFREX, AREA_DE_WORK.LD04_CAD_B.LD04_CAD_BONUS_COF);

            /*" -3696- MOVE CAD-DATA-INCLUSAO TO W-DATA-AAAAMMDD */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_DATA_INCLUSAOX.CAD_DATA_INCLUSAO, AREA_DE_WORK.W_DATA_AAAAMMDD);

            /*" -3697- MOVE W-DATA-AAAAMMDD-DD TO W-DATA-AAAA-MM-DD-DD */
            _.Move(AREA_DE_WORK.W_DATA_AAAAMMDD.W_DATA_AAAAMMDD_DD, AREA_DE_WORK.W_DATA_AAAA_MM_DD.W_DATA_AAAA_MM_DD_DD);

            /*" -3698- MOVE W-DATA-AAAAMMDD-MM TO W-DATA-AAAA-MM-DD-MM */
            _.Move(AREA_DE_WORK.W_DATA_AAAAMMDD.W_DATA_AAAAMMDD_MM, AREA_DE_WORK.W_DATA_AAAA_MM_DD.W_DATA_AAAA_MM_DD_MM);

            /*" -3699- MOVE W-DATA-AAAAMMDD-AAAA TO W-DATA-AAAA-MM-DD-AAAA */
            _.Move(AREA_DE_WORK.W_DATA_AAAAMMDD.W_DATA_AAAAMMDD_AAAA, AREA_DE_WORK.W_DATA_AAAA_MM_DD.W_DATA_AAAA_MM_DD_AAAA);

            /*" -3701- MOVE W-DATA-AAAA-MM-DD TO LD05-CAD-DATA-INCLUSAO. */
            _.Move(AREA_DE_WORK.W_DATA_AAAA_MM_DD, AREA_DE_WORK.LD05_CAD.LD05_CAD_DATA_INCLUSAO);

            /*" -3702- MOVE CAD-DATA-EXCLUSAO TO W-DATA-AAAAMMDD */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_DATA_EXCLUSAOX.CAD_DATA_EXCLUSAO, AREA_DE_WORK.W_DATA_AAAAMMDD);

            /*" -3703- MOVE W-DATA-AAAAMMDD-DD TO W-DATA-AAAA-MM-DD-DD */
            _.Move(AREA_DE_WORK.W_DATA_AAAAMMDD.W_DATA_AAAAMMDD_DD, AREA_DE_WORK.W_DATA_AAAA_MM_DD.W_DATA_AAAA_MM_DD_DD);

            /*" -3704- MOVE W-DATA-AAAAMMDD-MM TO W-DATA-AAAA-MM-DD-MM */
            _.Move(AREA_DE_WORK.W_DATA_AAAAMMDD.W_DATA_AAAAMMDD_MM, AREA_DE_WORK.W_DATA_AAAA_MM_DD.W_DATA_AAAA_MM_DD_MM);

            /*" -3705- MOVE W-DATA-AAAAMMDD-AAAA TO W-DATA-AAAA-MM-DD-AAAA */
            _.Move(AREA_DE_WORK.W_DATA_AAAAMMDD.W_DATA_AAAAMMDD_AAAA, AREA_DE_WORK.W_DATA_AAAA_MM_DD.W_DATA_AAAA_MM_DD_AAAA);

            /*" -3707- MOVE W-DATA-AAAA-MM-DD TO LD05-CAD-DATA-EXCLUSAO. */
            _.Move(AREA_DE_WORK.W_DATA_AAAA_MM_DD, AREA_DE_WORK.LD05_CAD.LD05_CAD_DATA_EXCLUSAO);

            /*" -3712- MOVE W-CAD-DATA-GERACAO TO LD05-CAD-DATA-GERACAO. */
            _.Move(AREA_DE_WORK.W_CAD_DATA_GERACAO, AREA_DE_WORK.LD05_CAD.LD05_CAD_DATA_GERACAO);

            /*" -3714- MOVE CAD-NSRX TO LD05-NSR. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NSRX, AREA_DE_WORK.LD05_CAD.LD05_NSR);

            /*" -3715- MOVE CAD-MATR-CONSULTORX TO LD06-MAT-CONSULTOR. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_MATR_CONSULTORX, AREA_DE_WORK.LD06_CAD.LD06_MAT_CONSULTOR);

            /*" -3716- MOVE CAD-NOME-CONSULTOR TO LD06-NOME-CONSULTOR */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NOME_CONSULTORX.CAD_NOME_CONSULTOR, AREA_DE_WORK.LD06_CAD.LD06_NOME_CONSULTOR);

            /*" -3718- MOVE CAD-EMAIL TO LD06-EMAIL. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_EMAILX.CAD_EMAIL, AREA_DE_WORK.LD06_CAD.LD06_EMAIL);

            /*" -3719- MOVE CAD-PV-SUBX TO LD06A-PV-SUB. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_PV_SUBX, AREA_DE_WORK.LD06A_CAD.LD06A_PV_SUB);

            /*" -3720- MOVE CAD-EN-SUBX TO LD06A-EN-SUB. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_EN_SUBX, AREA_DE_WORK.LD06A_CAD.LD06A_EN_SUB);

            /*" -3721- MOVE CAD-UNIDADE-SUBX TO LD06A-UNIDADE-SUB */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_UNIDADE_SUBX, AREA_DE_WORK.LD06A_CAD.LD06A_UNIDADE_SUB);

            /*" -3723- MOVE CAD-NIVEL-COMISSAOX TO LD06A-NIVEL-COMISSAO. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NIVEL_COMISSAOX, AREA_DE_WORK.LD06A_CAD.LD06A_NIVEL_COMISSAO);

            /*" -3724- MOVE CAD-NOME-FANTASIA TO LD06B-NOME-FANTASIA. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NOME_FANTASIA, AREA_DE_WORK.LD06B_CAD.LD06B_NOME_FANTASIA);

            /*" -3725- MOVE CAD-CONTATO1 TO LD06B-CONTATO1. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTATO1, AREA_DE_WORK.LD06B_CAD.LD06B_CONTATO1);

            /*" -3726- MOVE CAD-CONTATO2 TO LD06B-CONTATO2. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTATO2, AREA_DE_WORK.LD06B_CAD.LD06B_CONTATO2);

            /*" -3728- MOVE CAD-COD-MUNICIPIO TO LD06B-COD-MUNICIPIO. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_MUNICIPIO, AREA_DE_WORK.LD06B_CAD.LD06B_COD_MUNICIPIO);

            /*" -3730- MOVE CAD-NUM-SEGURADORA TO LD06C-NUM-SEGURADORA. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NUM_SEGURADORAX.CAD_NUM_SEGURADORA, AREA_DE_WORK.LD06C_CAD.LD06C_NUM_SEGURADORA);

            /*" -3731- MOVE CAD-BANCO-DESC-CPMF TO LD07-CAD-BANCO. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_DESC_CPMFX.CAD_BANCO_DESC_CPMF, AREA_DE_WORK.LD07_CAD.LD07_CAD_BANCO);

            /*" -3732- MOVE CAD-AGEN-DESC-CPMF TO LD07-CAD-AGENCIA. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_DESC_CPMFX.CAD_AGEN_DESC_CPMF, AREA_DE_WORK.LD07_CAD.LD07_CAD_AGENCIA);

            /*" -3733- MOVE CAD-CONTA-DESC-CPMF TO WS-CONTA-BANCARIA. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_DESC_CPMFX.CAD_CONTA_DESC_CPMF, AREA_DE_WORK.WS_CONTA_BANCARIA);

            /*" -3734- MOVE WS-OPERACAO-CONTA TO LD07-CAD-OPERACAO. */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_OPERACAO_CONTA, AREA_DE_WORK.LD07_CAD.LD07_CAD_OPERACAO);

            /*" -3735- MOVE WS-NUMERO-CONTA TO LD07-CAD-CONTA. */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_NUMERO_CONTA, AREA_DE_WORK.LD07_CAD.LD07_CAD_CONTA);

            /*" -3737- MOVE WS-DV-CONTA TO LD07-CAD-DV-CONTA. */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_DV_CONTA, AREA_DE_WORK.LD07_CAD.LD07_CAD_DV_CONTA);

            /*" -3738- MOVE CAD-BANCO-ISENTA TO LD07A-CAD-BANCO. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_ISENTAX.CAD_BANCO_ISENTA, AREA_DE_WORK.LD07A_CAD.LD07A_CAD_BANCO);

            /*" -3739- MOVE CAD-AGEN-ISENTA TO LD07A-CAD-AGENCIA. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_ISENTAX.CAD_AGEN_ISENTA, AREA_DE_WORK.LD07A_CAD.LD07A_CAD_AGENCIA);

            /*" -3740- MOVE CAD-CONTA-ISENTA TO WS-CONTA-BANCARIA. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_ISENTAX.CAD_CONTA_ISENTA, AREA_DE_WORK.WS_CONTA_BANCARIA);

            /*" -3741- MOVE WS-OPERACAO-CONTA TO LD07A-CAD-OPERACAO. */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_OPERACAO_CONTA, AREA_DE_WORK.LD07A_CAD.LD07A_CAD_OPERACAO);

            /*" -3742- MOVE WS-NUMERO-CONTA TO LD07A-CAD-CONTA. */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_NUMERO_CONTA, AREA_DE_WORK.LD07A_CAD.LD07A_CAD_CONTA);

            /*" -3744- MOVE WS-DV-CONTA TO LD07A-CAD-DV-CONTA. */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_DV_CONTA, AREA_DE_WORK.LD07A_CAD.LD07A_CAD_DV_CONTA);

            /*" -3745- MOVE CAD-BANCO-CAUCAO TO LD07B-CAD-BANCO. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BANCO_CAUCAOX.CAD_BANCO_CAUCAO, AREA_DE_WORK.LD07B_CAD.LD07B_CAD_BANCO);

            /*" -3746- MOVE CAD-AGEN-CAUCAO TO LD07B-CAD-AGENCIA. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_AGEN_CAUCAOX.CAD_AGEN_CAUCAO, AREA_DE_WORK.LD07B_CAD.LD07B_CAD_AGENCIA);

            /*" -3747- MOVE CAD-CONTA-CAUCAO TO WS-CONTA-BANCARIA. */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CONTA_CAUCAOX.CAD_CONTA_CAUCAO, AREA_DE_WORK.WS_CONTA_BANCARIA);

            /*" -3748- MOVE WS-OPERACAO-CONTA TO LD07B-CAD-OPERACAO. */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_OPERACAO_CONTA, AREA_DE_WORK.LD07B_CAD.LD07B_CAD_OPERACAO);

            /*" -3749- MOVE WS-NUMERO-CONTA TO LD07B-CAD-CONTA. */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_NUMERO_CONTA, AREA_DE_WORK.LD07B_CAD.LD07B_CAD_CONTA);

            /*" -3751- MOVE WS-DV-CONTA TO LD07B-CAD-DV-CONTA. */
            _.Move(AREA_DE_WORK.WS_CONTA_BANCARIA_R.WS_DV_CONTA, AREA_DE_WORK.LD07B_CAD.LD07B_CAD_DV_CONTA);

            /*" -3752- ADD 1 TO W-AC-LINHA. */
            AREA_DE_WORK.W_AC_LINHA.Value = AREA_DE_WORK.W_AC_LINHA + 1;

            /*" -3754- WRITE REG-RLT2000B FROM LC05 AFTER 1. */
            _.Move(AREA_DE_WORK.LC05.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -3755- ADD 2 TO W-AC-LINHA. */
            AREA_DE_WORK.W_AC_LINHA.Value = AREA_DE_WORK.W_AC_LINHA + 2;

            /*" -3756- PERFORM R7520-CABECALHO. */

            R7520_CABECALHO_SECTION();

            /*" -3758- WRITE REG-RLT2000B FROM LD01-CAD AFTER 2. */
            _.Move(AREA_DE_WORK.LD01_CAD.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -3759- ADD 1 TO W-AC-LINHA. */
            AREA_DE_WORK.W_AC_LINHA.Value = AREA_DE_WORK.W_AC_LINHA + 1;

            /*" -3760- PERFORM R7520-CABECALHO. */

            R7520_CABECALHO_SECTION();

            /*" -3762- WRITE REG-RLT2000B FROM LD02-CAD AFTER 1. */
            _.Move(AREA_DE_WORK.LD02_CAD.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -3763- ADD 1 TO W-AC-LINHA. */
            AREA_DE_WORK.W_AC_LINHA.Value = AREA_DE_WORK.W_AC_LINHA + 1;

            /*" -3764- PERFORM R7520-CABECALHO. */

            R7520_CABECALHO_SECTION();

            /*" -3766- WRITE REG-RLT2000B FROM LD02A-CAD AFTER 1. */
            _.Move(AREA_DE_WORK.LD02A_CAD.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -3767- ADD 1 TO W-AC-LINHA. */
            AREA_DE_WORK.W_AC_LINHA.Value = AREA_DE_WORK.W_AC_LINHA + 1;

            /*" -3768- PERFORM R7520-CABECALHO. */

            R7520_CABECALHO_SECTION();

            /*" -3770- WRITE REG-RLT2000B FROM LD02B-CAD AFTER 1. */
            _.Move(AREA_DE_WORK.LD02B_CAD.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -3771- ADD 1 TO W-AC-LINHA. */
            AREA_DE_WORK.W_AC_LINHA.Value = AREA_DE_WORK.W_AC_LINHA + 1;

            /*" -3772- PERFORM R7520-CABECALHO. */

            R7520_CABECALHO_SECTION();

            /*" -3774- WRITE REG-RLT2000B FROM LD03-CAD AFTER 1. */
            _.Move(AREA_DE_WORK.LD03_CAD.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -3775- ADD 3 TO W-AC-LINHA. */
            AREA_DE_WORK.W_AC_LINHA.Value = AREA_DE_WORK.W_AC_LINHA + 3;

            /*" -3776- PERFORM R7520-CABECALHO. */

            R7520_CABECALHO_SECTION();

            /*" -3777- WRITE REG-RLT2000B FROM LD04-CAD AFTER 1. */
            _.Move(AREA_DE_WORK.LD04_CAD.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -3778- WRITE REG-RLT2000B FROM LD04-CAD-A AFTER 1. */
            _.Move(AREA_DE_WORK.LD04_CAD_A.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -3780- WRITE REG-RLT2000B FROM LD04-CAD-B AFTER 1. */
            _.Move(AREA_DE_WORK.LD04_CAD_B.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -3781- ADD 1 TO W-AC-LINHA. */
            AREA_DE_WORK.W_AC_LINHA.Value = AREA_DE_WORK.W_AC_LINHA + 1;

            /*" -3782- PERFORM R7520-CABECALHO. */

            R7520_CABECALHO_SECTION();

            /*" -3784- WRITE REG-RLT2000B FROM LD05-CAD AFTER 1. */
            _.Move(AREA_DE_WORK.LD05_CAD.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -3785- ADD 1 TO W-AC-LINHA. */
            AREA_DE_WORK.W_AC_LINHA.Value = AREA_DE_WORK.W_AC_LINHA + 1;

            /*" -3786- PERFORM R7520-CABECALHO. */

            R7520_CABECALHO_SECTION();

            /*" -3788- WRITE REG-RLT2000B FROM LD06-CAD AFTER 1. */
            _.Move(AREA_DE_WORK.LD06_CAD.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -3789- ADD 1 TO W-AC-LINHA. */
            AREA_DE_WORK.W_AC_LINHA.Value = AREA_DE_WORK.W_AC_LINHA + 1;

            /*" -3790- PERFORM R7520-CABECALHO. */

            R7520_CABECALHO_SECTION();

            /*" -3792- WRITE REG-RLT2000B FROM LD06A-CAD AFTER 1. */
            _.Move(AREA_DE_WORK.LD06A_CAD.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -3793- ADD 1 TO W-AC-LINHA. */
            AREA_DE_WORK.W_AC_LINHA.Value = AREA_DE_WORK.W_AC_LINHA + 1;

            /*" -3794- PERFORM R7520-CABECALHO. */

            R7520_CABECALHO_SECTION();

            /*" -3796- WRITE REG-RLT2000B FROM LD06B-CAD AFTER 1. */
            _.Move(AREA_DE_WORK.LD06B_CAD.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -3797- ADD 1 TO W-AC-LINHA. */
            AREA_DE_WORK.W_AC_LINHA.Value = AREA_DE_WORK.W_AC_LINHA + 1;

            /*" -3798- PERFORM R7520-CABECALHO. */

            R7520_CABECALHO_SECTION();

            /*" -3800- WRITE REG-RLT2000B FROM LD06C-CAD AFTER 1. */
            _.Move(AREA_DE_WORK.LD06C_CAD.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -3801- ADD 1 TO W-AC-LINHA. */
            AREA_DE_WORK.W_AC_LINHA.Value = AREA_DE_WORK.W_AC_LINHA + 1;

            /*" -3802- PERFORM R7520-CABECALHO. */

            R7520_CABECALHO_SECTION();

            /*" -3804- WRITE REG-RLT2000B FROM LD07-CAD AFTER 1. */
            _.Move(AREA_DE_WORK.LD07_CAD.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -3805- ADD 1 TO W-AC-LINHA. */
            AREA_DE_WORK.W_AC_LINHA.Value = AREA_DE_WORK.W_AC_LINHA + 1;

            /*" -3806- PERFORM R7520-CABECALHO. */

            R7520_CABECALHO_SECTION();

            /*" -3808- WRITE REG-RLT2000B FROM LD07A-CAD AFTER 1. */
            _.Move(AREA_DE_WORK.LD07A_CAD.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -3809- ADD 1 TO W-AC-LINHA. */
            AREA_DE_WORK.W_AC_LINHA.Value = AREA_DE_WORK.W_AC_LINHA + 1;

            /*" -3810- PERFORM R7520-CABECALHO. */

            R7520_CABECALHO_SECTION();

            /*" -3812- WRITE REG-RLT2000B FROM LD07B-CAD AFTER 1. */
            _.Move(AREA_DE_WORK.LD07B_CAD.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

            /*" -3813- ADD 1 TO W-AC-LINHA. */
            AREA_DE_WORK.W_AC_LINHA.Value = AREA_DE_WORK.W_AC_LINHA + 1;

            /*" -3814- PERFORM R7520-CABECALHO. */

            R7520_CABECALHO_SECTION();

            /*" -3814- WRITE REG-RLT2000B FROM LC06 AFTER 1. */
            _.Move(AREA_DE_WORK.LC06.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R7500_SAIDA*/

        [StopWatch]
        /*" R7510-MONTA-CABECALHO-SECTION */
        private void R7510_MONTA_CABECALHO_SECTION()
        {
            /*" -3822- ACCEPT WS-DATE FROM DATE */
            _.Move(_.AcceptDate("DATE"), AREA_DE_WORK.WS_DATE);

            /*" -3823- MOVE WS-DD-DATE TO WDATA-DD-CABEC */
            _.Move(AREA_DE_WORK.WS_DATE.WS_DD_DATE, AREA_DE_WORK.WDATA_CABEC.WDATA_DD_CABEC);

            /*" -3824- MOVE WS-MM-DATE TO WDATA-MM-CABEC */
            _.Move(AREA_DE_WORK.WS_DATE.WS_MM_DATE, AREA_DE_WORK.WDATA_CABEC.WDATA_MM_CABEC);

            /*" -3825- MOVE WS-AA-DATE TO WDATA-AA-CABEC */
            _.Move(AREA_DE_WORK.WS_DATE.WS_AA_DATE, AREA_DE_WORK.WDATA_CABEC.WDATA_AA_CABEC);

            /*" -3827- MOVE WDATA-CABEC TO LC02-DATA */
            _.Move(AREA_DE_WORK.WDATA_CABEC, AREA_DE_WORK.LC02.LC02_DATA);

            /*" -3828- ACCEPT WS-TIME FROM TIME */
            _.Move(_.AcceptDate("TIME"), AREA_DE_WORK.WS_TIME);

            /*" -3829- MOVE WS-HH-TIME TO WHORA-HH-CABEC */
            _.Move(AREA_DE_WORK.WS_TIME.WS_HH_TIME, AREA_DE_WORK.WHORA_CABEC.WHORA_HH_CABEC);

            /*" -3830- MOVE WS-MM-TIME TO WHORA-MM-CABEC */
            _.Move(AREA_DE_WORK.WS_TIME.WS_MM_TIME, AREA_DE_WORK.WHORA_CABEC.WHORA_MM_CABEC);

            /*" -3831- MOVE WS-SS-TIME TO WHORA-SS-CABEC */
            _.Move(AREA_DE_WORK.WS_TIME.WS_SS_TIME, AREA_DE_WORK.WHORA_CABEC.WHORA_SS_CABEC);

            /*" -3833- MOVE WHORA-CABEC TO LC03-HORA */
            _.Move(AREA_DE_WORK.WHORA_CABEC, AREA_DE_WORK.LC03.LC03_HORA);

            /*" -3837- PERFORM R7510_MONTA_CABECALHO_DB_SELECT_1 */

            R7510_MONTA_CABECALHO_DB_SELECT_1();

            /*" -3840- MOVE V1EMPR-NOM-EMP TO LK-TITULO */
            _.Move(V1EMPR_NOM_EMP, AREA_DE_WORK.LK_LINK.LK_TITULO);

            /*" -3842- CALL 'PROALN01' USING LK-LINK */
            _.Call("PROALN01", AREA_DE_WORK.LK_LINK);

            /*" -3843- IF LK-RTCODE EQUAL ZEROS */

            if (AREA_DE_WORK.LK_LINK.LK_RTCODE == 00)
            {

                /*" -3844- MOVE LK-TITULO TO LC01-EMPRESA */
                _.Move(AREA_DE_WORK.LK_LINK.LK_TITULO, AREA_DE_WORK.LC01.LC01_EMPRESA);

                /*" -3845- ELSE */
            }
            else
            {


                /*" -3846- DISPLAY 'PROBLEMA CALL V1EMPRESA' */
                _.Display($"PROBLEMA CALL V1EMPRESA");

                /*" -3848- STOP RUN. */

                throw new GoBack();   // => STOP RUN.
            }


            /*" -3848- MOVE 99 TO W-AC-LINHA. */
            _.Move(99, AREA_DE_WORK.W_AC_LINHA);

        }

        [StopWatch]
        /*" R7510-MONTA-CABECALHO-DB-SELECT-1 */
        public void R7510_MONTA_CABECALHO_DB_SELECT_1()
        {
            /*" -3837- EXEC SQL SELECT NOME_EMPRESA INTO :V1EMPR-NOM-EMP FROM SEGUROS.V1EMPRESA WHERE COD_EMPRESA = 0 END-EXEC. */

            var r7510_MONTA_CABECALHO_DB_SELECT_1_Query1 = new R7510_MONTA_CABECALHO_DB_SELECT_1_Query1()
            {
            };

            var executed_1 = R7510_MONTA_CABECALHO_DB_SELECT_1_Query1.Execute(r7510_MONTA_CABECALHO_DB_SELECT_1_Query1);
            if (executed_1 != null)
            {
                _.Move(executed_1.V1EMPR_NOM_EMP, V1EMPR_NOM_EMP);
            }


        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R7510_SAIDA*/

        [StopWatch]
        /*" R7520-CABECALHO-SECTION */
        private void R7520_CABECALHO_SECTION()
        {
            /*" -3856- IF W-AC-LINHA GREATER 60 */

            if (AREA_DE_WORK.W_AC_LINHA > 60)
            {

                /*" -3857- ADD 1 TO W-AC-PAGINA */
                AREA_DE_WORK.W_AC_PAGINA.Value = AREA_DE_WORK.W_AC_PAGINA + 1;

                /*" -3858- MOVE W-AC-PAGINA TO LC01-PAGINA */
                _.Move(AREA_DE_WORK.W_AC_PAGINA, AREA_DE_WORK.LC01.LC01_PAGINA);

                /*" -3859- WRITE REG-RLT2000B FROM LC01 AFTER PAGE */
                _.Move(AREA_DE_WORK.LC01.GetMoveValues(), REG_RLT2000B);

                RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

                /*" -3860- WRITE REG-RLT2000B FROM LC02 AFTER 1 */
                _.Move(AREA_DE_WORK.LC02.GetMoveValues(), REG_RLT2000B);

                RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

                /*" -3861- WRITE REG-RLT2000B FROM LC03 AFTER 1 */
                _.Move(AREA_DE_WORK.LC03.GetMoveValues(), REG_RLT2000B);

                RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

                /*" -3862- WRITE REG-RLT2000B FROM LC06 AFTER 1 */
                _.Move(AREA_DE_WORK.LC06.GetMoveValues(), REG_RLT2000B);

                RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

                /*" -3862- MOVE 4 TO W-AC-LINHA. */
                _.Move(4, AREA_DE_WORK.W_AC_LINHA);
            }


        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R7520_SAIDA*/

        [StopWatch]
        /*" R7600-IMPRIME-LD00-MSG1-SECTION */
        private void R7600_IMPRIME_LD00_MSG1_SECTION()
        {
            /*" -3870- IF CAD-SITUACAOX = ZEROS OR 2 */

            if (AREA_DE_WORK.REG_CAD_CADASTRO.CAD_SITUACAOX.In("00", "2"))
            {

                /*" -3872- GO TO R7600-SAIDA. */
                /*Método Suprimido por falta de linha ou apenas EXIT nome: R7600_SAIDA*/ //GOTO
                return;
            }


            /*" -3873- IF WS-IMPRIMIU EQUAL ZEROS */

            if (WS_IMPRIMIU == 00)
            {

                /*" -3874- MOVE 1 TO WS-IMPRIMIU */
                _.Move(1, WS_IMPRIMIU);

                /*" -3876- PERFORM R7500-IMPRIME-CADASTRO. */

                R7500_IMPRIME_CADASTRO_SECTION();
            }


            /*" -3877- ADD 1 TO W-AC-LINHA */
            AREA_DE_WORK.W_AC_LINHA.Value = AREA_DE_WORK.W_AC_LINHA + 1;

            /*" -3878- PERFORM R7520-CABECALHO */

            R7520_CABECALHO_SECTION();

            /*" -3878- WRITE REG-RLT2000B FROM LD00 AFTER 1. */
            _.Move(AREA_DE_WORK.LD00.GetMoveValues(), REG_RLT2000B);

            RLT2000B.Write(REG_RLT2000B.GetMoveValues().ToString());

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R7600_SAIDA*/

        [StopWatch]
        /*" R7650-CONVERTE-CARACTER-SECTION */
        private void R7650_CONVERTE_CARACTER_SECTION()
        {
            /*" -3886- MOVE 'S' TO LK-CONVER-MAIUSCULA */
            _.Move("S", LK_CONVERSAO.LK_CONVER_MAIUSCULA);

            /*" -3887- MOVE 40 TO LK-TAM-CAMPO */
            _.Move(40, LK_CONVERSAO.LK_TAM_CAMPO);

            /*" -3888- MOVE CAD-RAZAO-SOCIAL TO LK-CAMPO-ENTRADA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_RAZAO_SOCIAL, LK_CONVERSAO.LK_CAMPO_ENTRADA);

            /*" -3889- CALL 'PROCONV' USING LK-CONVERSAO */
            _.Call("PROCONV", LK_CONVERSAO);

            /*" -3891- MOVE LK-CAMPO-SAIDA TO CAD-RAZAO-SOCIAL. */
            _.Move(LK_CONVERSAO.LK_CAMPO_SAIDA, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_RAZAO_SOCIAL);

            /*" -3892- MOVE 'S' TO LK-CONVER-MAIUSCULA */
            _.Move("S", LK_CONVERSAO.LK_CONVER_MAIUSCULA);

            /*" -3893- MOVE 30 TO LK-TAM-CAMPO */
            _.Move(30, LK_CONVERSAO.LK_TAM_CAMPO);

            /*" -3894- MOVE CAD-NOME-FANTASIA TO LK-CAMPO-ENTRADA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NOME_FANTASIA, LK_CONVERSAO.LK_CAMPO_ENTRADA);

            /*" -3895- CALL 'PROCONV' USING LK-CONVERSAO */
            _.Call("PROCONV", LK_CONVERSAO);

            /*" -3897- MOVE LK-CAMPO-SAIDA TO CAD-NOME-FANTASIA. */
            _.Move(LK_CONVERSAO.LK_CAMPO_SAIDA, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NOME_FANTASIA);

            /*" -3898- MOVE 'S' TO LK-CONVER-MAIUSCULA */
            _.Move("S", LK_CONVERSAO.LK_CONVER_MAIUSCULA);

            /*" -3899- MOVE 40 TO LK-TAM-CAMPO */
            _.Move(40, LK_CONVERSAO.LK_TAM_CAMPO);

            /*" -3900- MOVE CAD-END TO LK-CAMPO-ENTRADA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_ENDERECO.CAD_END, LK_CONVERSAO.LK_CAMPO_ENTRADA);

            /*" -3901- CALL 'PROCONV' USING LK-CONVERSAO */
            _.Call("PROCONV", LK_CONVERSAO);

            /*" -3903- MOVE LK-CAMPO-SAIDA TO CAD-END. */
            _.Move(LK_CONVERSAO.LK_CAMPO_SAIDA, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_ENDERECO.CAD_END);

            /*" -3904- MOVE 'S' TO LK-CONVER-MAIUSCULA */
            _.Move("S", LK_CONVERSAO.LK_CONVER_MAIUSCULA);

            /*" -3905- MOVE 16 TO LK-TAM-CAMPO */
            _.Move(16, LK_CONVERSAO.LK_TAM_CAMPO);

            /*" -3906- MOVE CAD-COMPL-END TO LK-CAMPO-ENTRADA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_ENDERECO.CAD_COMPL_END, LK_CONVERSAO.LK_CAMPO_ENTRADA);

            /*" -3907- CALL 'PROCONV' USING LK-CONVERSAO */
            _.Call("PROCONV", LK_CONVERSAO);

            /*" -3909- MOVE LK-CAMPO-SAIDA TO CAD-COMPL-END. */
            _.Move(LK_CONVERSAO.LK_CAMPO_SAIDA, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_ENDERECO.CAD_COMPL_END);

            /*" -3910- MOVE 'S' TO LK-CONVER-MAIUSCULA */
            _.Move("S", LK_CONVERSAO.LK_CONVER_MAIUSCULA);

            /*" -3911- MOVE 20 TO LK-TAM-CAMPO */
            _.Move(20, LK_CONVERSAO.LK_TAM_CAMPO);

            /*" -3912- MOVE CAD-BAIRRO TO LK-CAMPO-ENTRADA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BAIRRO, LK_CONVERSAO.LK_CAMPO_ENTRADA);

            /*" -3913- CALL 'PROCONV' USING LK-CONVERSAO */
            _.Call("PROCONV", LK_CONVERSAO);

            /*" -3915- MOVE LK-CAMPO-SAIDA TO CAD-BAIRRO. */
            _.Move(LK_CONVERSAO.LK_CAMPO_SAIDA, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_BAIRRO);

            /*" -3916- MOVE 'S' TO LK-CONVER-MAIUSCULA */
            _.Move("S", LK_CONVERSAO.LK_CONVER_MAIUSCULA);

            /*" -3917- MOVE 12 TO LK-TAM-CAMPO */
            _.Move(12, LK_CONVERSAO.LK_TAM_CAMPO);

            /*" -3918- MOVE CAD-COD-MUNICIPIO TO LK-CAMPO-ENTRADA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_MUNICIPIO, LK_CONVERSAO.LK_CAMPO_ENTRADA);

            /*" -3919- CALL 'PROCONV' USING LK-CONVERSAO */
            _.Call("PROCONV", LK_CONVERSAO);

            /*" -3921- MOVE LK-CAMPO-SAIDA TO CAD-COD-MUNICIPIO. */
            _.Move(LK_CONVERSAO.LK_CAMPO_SAIDA, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_MUNICIPIO);

            /*" -3922- MOVE 'S' TO LK-CONVER-MAIUSCULA */
            _.Move("S", LK_CONVERSAO.LK_CONVER_MAIUSCULA);

            /*" -3923- MOVE 25 TO LK-TAM-CAMPO */
            _.Move(25, LK_CONVERSAO.LK_TAM_CAMPO);

            /*" -3924- MOVE CAD-CIDADE TO LK-CAMPO-ENTRADA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CIDADE, LK_CONVERSAO.LK_CAMPO_ENTRADA);

            /*" -3925- CALL 'PROCONV' USING LK-CONVERSAO */
            _.Call("PROCONV", LK_CONVERSAO);

            /*" -3927- MOVE LK-CAMPO-SAIDA TO CAD-CIDADE. */
            _.Move(LK_CONVERSAO.LK_CAMPO_SAIDA, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_CIDADE);

            /*" -3928- MOVE 'S' TO LK-CONVER-MAIUSCULA */
            _.Move("S", LK_CONVERSAO.LK_CONVER_MAIUSCULA);

            /*" -3929- MOVE 02 TO LK-TAM-CAMPO */
            _.Move(02, LK_CONVERSAO.LK_TAM_CAMPO);

            /*" -3930- MOVE CAD-UF TO LK-CAMPO-ENTRADA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_UF, LK_CONVERSAO.LK_CAMPO_ENTRADA);

            /*" -3931- CALL 'PROCONV' USING LK-CONVERSAO */
            _.Call("PROCONV", LK_CONVERSAO);

            /*" -3933- MOVE LK-CAMPO-SAIDA TO CAD-UF. */
            _.Move(LK_CONVERSAO.LK_CAMPO_SAIDA, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_UF);

            /*" -3934- MOVE 'S' TO LK-CONVER-MAIUSCULA */
            _.Move("S", LK_CONVERSAO.LK_CONVER_MAIUSCULA);

            /*" -3935- MOVE 40 TO LK-TAM-CAMPO */
            _.Move(40, LK_CONVERSAO.LK_TAM_CAMPO);

            /*" -3936- MOVE CAD-NOME-CONSULTOR TO LK-CAMPO-ENTRADA */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NOME_CONSULTORX.CAD_NOME_CONSULTOR, LK_CONVERSAO.LK_CAMPO_ENTRADA);

            /*" -3937- CALL 'PROCONV' USING LK-CONVERSAO */
            _.Call("PROCONV", LK_CONVERSAO);

            /*" -3937- MOVE LK-CAMPO-SAIDA TO CAD-NOME-CONSULTOR. */
            _.Move(LK_CONVERSAO.LK_CAMPO_SAIDA, AREA_DE_WORK.REG_CAD_CADASTRO.CAD_NOME_CONSULTORX.CAD_NOME_CONSULTOR);

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R7650_SAIDA*/

        [StopWatch]
        /*" R6990-GRAVAR-PARAM-RENOVAR-SECTION */
        private void R6990_GRAVAR_PARAM_RENOVAR_SECTION()
        {
            /*" -3948- MOVE 7105 TO V0SOL-COD-PRODUTO */
            _.Move(7105, V0SOL_COD_PRODUTO);

            /*" -3949- MOVE CAD-CODIGO-CEF TO V0SOL-COD-CLIENTE */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF, V0SOL_COD_CLIENTE);

            /*" -3950- MOVE V0SIST-DTMOVABE TO V0SOL-DATA-SOLICITACAO */
            _.Move(V0SIST_DTMOVABE, V0SOL_DATA_SOLICITACAO);

            /*" -3951- MOVE 'LT2000B' TO V0SOL-COD-USUARIO */
            _.Move("LT2000B", V0SOL_COD_USUARIO);

            /*" -3952- MOVE 'LT2018B' TO V0SOL-COD-PROGRAMA */
            _.Move("LT2018B", V0SOL_COD_PROGRAMA);

            /*" -3953- MOVE V0SIST-DTMOVABE TO V0SOL-DATA-PREV-PROC */
            _.Move(V0SIST_DTMOVABE, V0SOL_DATA_PREV_PROC);

            /*" -3954- MOVE 0 TO V0SOL-COD-MOVIMENTO */
            _.Move(0, V0SOL_COD_MOVIMENTO);

            /*" -3955- MOVE '0' TO V0SOL-SIT-SOLICITACAO */
            _.Move("0", V0SOL_SIT_SOLICITACAO);

            /*" -3956- MOVE CAD-RENOVAR TO V0SOL-TIPO-SOLICITACAO */
            _.Move(AREA_DE_WORK.REG_CAD_CADASTRO.CAD_RENOVAR, V0SOL_TIPO_SOLICITACAO);

            /*" -3957- MOVE V0SIST-DTMOVABE TO V0SOL-PARAM-DATE01 */
            _.Move(V0SIST_DTMOVABE, V0SOL_PARAM_DATE01);

            /*" -3958- MOVE V0SIST-DTMOVABE TO V0SOL-PARAM-DATE02 */
            _.Move(V0SIST_DTMOVABE, V0SOL_PARAM_DATE02);

            /*" -3959- MOVE V0SIST-DTMOVABE TO V0SOL-PARAM-DATE03 */
            _.Move(V0SIST_DTMOVABE, V0SOL_PARAM_DATE03);

            /*" -3960- MOVE 0 TO V0SOL-PARAM-SMINT01 */
            _.Move(0, V0SOL_PARAM_SMINT01);

            /*" -3961- MOVE 0 TO V0SOL-PARAM-SMINT02 */
            _.Move(0, V0SOL_PARAM_SMINT02);

            /*" -3962- MOVE 0 TO V0SOL-PARAM-SMINT03 */
            _.Move(0, V0SOL_PARAM_SMINT03);

            /*" -3963- MOVE 0 TO V0SOL-PARAM-INTG01. */
            _.Move(0, V0SOL_PARAM_INTG01);

            /*" -3964- MOVE 0 TO V0SOL-PARAM-INTG02 */
            _.Move(0, V0SOL_PARAM_INTG02);

            /*" -3965- MOVE 0 TO V0SOL-PARAM-INTG03 */
            _.Move(0, V0SOL_PARAM_INTG03);

            /*" -3966- MOVE 0107100070673 TO V0SOL-PARAM-DEC01 */
            _.Move(0107100070673, V0SOL_PARAM_DEC01);

            /*" -3967- MOVE 0 TO V0SOL-PARAM-DEC02 */
            _.Move(0, V0SOL_PARAM_DEC02);

            /*" -3968- MOVE 0 TO V0SOL-PARAM-DEC03 */
            _.Move(0, V0SOL_PARAM_DEC03);

            /*" -3969- MOVE 0 TO V0SOL-PARAM-FLOAT01 */
            _.Move(0, V0SOL_PARAM_FLOAT01);

            /*" -3970- MOVE 0 TO V0SOL-PARAM-FLOAT02 */
            _.Move(0, V0SOL_PARAM_FLOAT02);

            /*" -3971- MOVE ' ' TO V0SOL-PARAM-CHAR01 */
            _.Move(" ", V0SOL_PARAM_CHAR01);

            /*" -3972- MOVE ' ' TO V0SOL-PARAM-CHAR02 */
            _.Move(" ", V0SOL_PARAM_CHAR02);

            /*" -3973- MOVE ' ' TO V0SOL-PARAM-CHAR03 */
            _.Move(" ", V0SOL_PARAM_CHAR03);

            /*" -3975- MOVE ' ' TO V0SOL-PARAM-CHAR04 */
            _.Move(" ", V0SOL_PARAM_CHAR04);

            /*" -3977- PERFORM R6995-INSERT-PARAMETRO. */

            R6995_INSERT_PARAMETRO_SECTION();

            /*" -3978- ADD 1 TO W-AC-LOTERICOS-GRAVADOS. */
            AREA_DE_WORK.W_AC_LOTERICOS_GRAVADOS.Value = AREA_DE_WORK.W_AC_LOTERICOS_GRAVADOS + 1;

            /*" -3978- DISPLAY 'LOTERICO NAO DESEJA RENOVAR=' CAD-CODIGO-CEF. */
            _.Display($"LOTERICO NAO DESEJA RENOVAR={AREA_DE_WORK.REG_CAD_CADASTRO.CAD_COD_CEFX.CAD_CODIGO_CEF}");

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6990_SAIDA*/

        [StopWatch]
        /*" R6995-INSERT-PARAMETRO-SECTION */
        private void R6995_INSERT_PARAMETRO_SECTION()
        {
            /*" -3987- MOVE '0008' TO WNR-EXEC-SQL. */
            _.Move("0008", LK_CONVERSAO.WABEND.WNR_EXEC_SQL);

            /*" -4042- PERFORM R6995_INSERT_PARAMETRO_DB_INSERT_1 */

            R6995_INSERT_PARAMETRO_DB_INSERT_1();

            /*" -4045- IF SQLCODE NOT EQUAL ZEROS */

            if (DB.SQLCODE != 00)
            {

                /*" -4046- DISPLAY ' R6990-ERRO INSERT LT-SOLICITA_PARAM ' */
                _.Display($" R6990-ERRO INSERT LT-SOLICITA_PARAM ");

                /*" -4047- DISPLAY ' COD. PROGRAMA   = ' V0SOL-COD-PROGRAMA */
                _.Display($" COD. PROGRAMA   = {V0SOL_COD_PROGRAMA}");

                /*" -4048- DISPLAY ' DATA-PREV-PROC  =' V0SOL-DATA-SOLICITACAO */
                _.Display($" DATA-PREV-PROC  ={V0SOL_DATA_SOLICITACAO}");

                /*" -4049- DISPLAY ' SEGURADO        =' V0SOL-COD-CLIENTE */
                _.Display($" SEGURADO        ={V0SOL_COD_CLIENTE}");

                /*" -4049- GO TO R9999-ROT-ERRO. */

                R9999_ROT_ERRO_SECTION(); //GOTO
                return;
            }


        }

        [StopWatch]
        /*" R6995-INSERT-PARAMETRO-DB-INSERT-1 */
        public void R6995_INSERT_PARAMETRO_DB_INSERT_1()
        {
            /*" -4042- EXEC SQL INSERT INTO SEGUROS.LT_SOLICITA_PARAM (COD_PRODUTO , COD_CLIENTE , COD_PROGRAMA , TIPO_SOLICITACAO, DATA_SOLICITACAO, COD_USUARIO , DATA_PREV_PROC , SIT_SOLICITACAO , TSTMP_SITUACAO , PARAM_DATE01 , PARAM_DATE02 , PARAM_DATE03 , PARAM_SMINT01 , PARAM_SMINT02 , PARAM_SMINT03 , PARAM_INTG01 , PARAM_INTG02 , PARAM_INTG03 , PARAM_DEC01 , PARAM_DEC02 , PARAM_DEC03 , PARAM_FLOAT01 , PARAM_FLOAT02 , PARAM_CHAR01 , PARAM_CHAR02 , PARAM_CHAR03 , PARAM_CHAR04) VALUES (:V0SOL-COD-PRODUTO , :V0SOL-COD-CLIENTE , :V0SOL-COD-PROGRAMA , :V0SOL-TIPO-SOLICITACAO , :V0SOL-DATA-SOLICITACAO , :V0SOL-COD-USUARIO , :V0SOL-DATA-PREV-PROC , :V0SOL-SIT-SOLICITACAO , CURRENT TIMESTAMP , :V0SOL-PARAM-DATE01 , :V0SOL-PARAM-DATE02 , :V0SOL-PARAM-DATE03 , :V0SOL-PARAM-SMINT01 , :V0SOL-PARAM-SMINT02 , :V0SOL-PARAM-SMINT03 , :V0SOL-PARAM-INTG01 , :V0SOL-PARAM-INTG02 , :V0SOL-PARAM-INTG03 , :V0SOL-PARAM-DEC01 , :V0SOL-PARAM-DEC02 , :V0SOL-PARAM-DEC03 , :V0SOL-PARAM-FLOAT01 , :V0SOL-PARAM-FLOAT02 , :V0SOL-PARAM-CHAR01 , :V0SOL-PARAM-CHAR02 , :V0SOL-PARAM-CHAR03 , :V0SOL-PARAM-CHAR03) END-EXEC. */

            var r6995_INSERT_PARAMETRO_DB_INSERT_1_Insert1 = new R6995_INSERT_PARAMETRO_DB_INSERT_1_Insert1()
            {
                V0SOL_COD_PRODUTO = V0SOL_COD_PRODUTO.ToString(),
                V0SOL_COD_CLIENTE = V0SOL_COD_CLIENTE.ToString(),
                V0SOL_COD_PROGRAMA = V0SOL_COD_PROGRAMA.ToString(),
                V0SOL_TIPO_SOLICITACAO = V0SOL_TIPO_SOLICITACAO.ToString(),
                V0SOL_DATA_SOLICITACAO = V0SOL_DATA_SOLICITACAO.ToString(),
                V0SOL_COD_USUARIO = V0SOL_COD_USUARIO.ToString(),
                V0SOL_DATA_PREV_PROC = V0SOL_DATA_PREV_PROC.ToString(),
                V0SOL_SIT_SOLICITACAO = V0SOL_SIT_SOLICITACAO.ToString(),
                V0SOL_PARAM_DATE01 = V0SOL_PARAM_DATE01.ToString(),
                V0SOL_PARAM_DATE02 = V0SOL_PARAM_DATE02.ToString(),
                V0SOL_PARAM_DATE03 = V0SOL_PARAM_DATE03.ToString(),
                V0SOL_PARAM_SMINT01 = V0SOL_PARAM_SMINT01.ToString(),
                V0SOL_PARAM_SMINT02 = V0SOL_PARAM_SMINT02.ToString(),
                V0SOL_PARAM_SMINT03 = V0SOL_PARAM_SMINT03.ToString(),
                V0SOL_PARAM_INTG01 = V0SOL_PARAM_INTG01.ToString(),
                V0SOL_PARAM_INTG02 = V0SOL_PARAM_INTG02.ToString(),
                V0SOL_PARAM_INTG03 = V0SOL_PARAM_INTG03.ToString(),
                V0SOL_PARAM_DEC01 = V0SOL_PARAM_DEC01.ToString(),
                V0SOL_PARAM_DEC02 = V0SOL_PARAM_DEC02.ToString(),
                V0SOL_PARAM_DEC03 = V0SOL_PARAM_DEC03.ToString(),
                V0SOL_PARAM_FLOAT01 = V0SOL_PARAM_FLOAT01.ToString(),
                V0SOL_PARAM_FLOAT02 = V0SOL_PARAM_FLOAT02.ToString(),
                V0SOL_PARAM_CHAR01 = V0SOL_PARAM_CHAR01.ToString(),
                V0SOL_PARAM_CHAR02 = V0SOL_PARAM_CHAR02.ToString(),
                V0SOL_PARAM_CHAR03 = V0SOL_PARAM_CHAR03.ToString(),
            };

            R6995_INSERT_PARAMETRO_DB_INSERT_1_Insert1.Execute(r6995_INSERT_PARAMETRO_DB_INSERT_1_Insert1);

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R6995_SAIDA*/

        [StopWatch]
        /*" R9000-OPEN-ARQUIVOS-SECTION */
        private void R9000_OPEN_ARQUIVOS_SECTION()
        {
            /*" -4059- OPEN INPUT CADASTRO. */
            CADASTRO.Open(REG_CAD);

            /*" -4059- OPEN OUTPUT RLT2000B. */
            RLT2000B.Open(REG_RLT2000B);

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R9000_SAIDA*/

        [StopWatch]
        /*" R9100-CLOSE-ARQUIVOS-SECTION */
        private void R9100_CLOSE_ARQUIVOS_SECTION()
        {
            /*" -4068- CLOSE CADASTRO RLT2000B. */
            CADASTRO.Close();
            RLT2000B.Close();

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R9100_SAIDA*/

        [StopWatch]
        /*" R9999-ROT-ERRO-SECTION */
        private void R9999_ROT_ERRO_SECTION()
        {
            /*" -4079- MOVE SQLCODE TO WSQLCODE. */
            _.Move(DB.SQLCODE, LK_CONVERSAO.WABEND.WSQLCODE);

            /*" -4081- DISPLAY WABEND */
            _.Display(LK_CONVERSAO.WABEND);

            /*" -4083- CLOSE CADASTRO RLT2000B. */
            CADASTRO.Close();
            RLT2000B.Close();

            /*" -4083- EXEC SQL WHENEVER SQLWARNING CONTINUE END-EXEC. */

            /*" -4085- EXEC SQL ROLLBACK WORK END-EXEC. */

            DatabaseConnection.Instance.RollbackTransaction();

            /*" -4089- MOVE 99 TO RETURN-CODE. */
            _.Move(99, RETURN_CODE);

            /*" -4089- STOP RUN. */

            throw new GoBack();   // => STOP RUN.

        }
        /*Método Suprimido por falta de linha ou apenas EXIT nome: R9999_SAIDA*/
    }
}
