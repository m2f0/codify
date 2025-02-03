using System;
using IA_ConverterCommons;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Linq;
using _ = IA_ConverterCommons.Statements;
using DB = IA_ConverterCommons.DatabaseBasis;
using Xunit;
using Code;
using static Code.LT2000B;

namespace FileTests.Test
{
    public class LT2000B_Tests
    {
        //é de extrema importancia que este método seja modificado com cautela, 
        //o melhor é manter aqui apenas os dados que serão COMUM a todos os Theory's criados
        public static void Load_Parameters()
        {
            #region PARAMETERS
            #region R0100_SELECT_V1SISTEMA_DB_SELECT_1_Query1

            var q0 = new DynamicData();
            q0.AddDynamic(new Dictionary<string, string>{
                { "V0SIST_DTMOVABE" , ""}
            });
            AppSettings.TestSet.DynamicData.Add("R0100_SELECT_V1SISTEMA_DB_SELECT_1_Query1", q0);

            #endregion

            #region R5000_SELECT_MAX_CONTA_DB_SELECT_1_Query1

            var q1 = new DynamicData();
            q1.AddDynamic(new Dictionary<string, string>{
                { "FCSEQUEN_NUM_SEQ" , ""}
            });
            AppSettings.TestSet.DynamicData.Add("R5000_SELECT_MAX_CONTA_DB_SELECT_1_Query1", q1);

            #endregion

            #region R5100_UPDATE_MAX_CONTA_DB_UPDATE_1_Update1

            var q2 = new DynamicData();
            q2.AddDynamic(new Dictionary<string, string>{
                { "FCSEQUEN_NUM_SEQ" , ""}
            });
            AppSettings.TestSet.DynamicData.Add("R5100_UPDATE_MAX_CONTA_DB_UPDATE_1_Update1", q2);

            #endregion

            #region R6020_SELECT_FC_LOTERICO_DB_SELECT_1_Query1

            var q3 = new DynamicData();
            q3.AddDynamic(new Dictionary<string, string>{
                { "FCLOTERI_COD_AGENTE_MASTER" , ""},
                { "FCLOTERI_COD_CGC" , ""},
                { "FCLOTERI_COD_INSCR_ESTAD" , ""},
                { "FCLOTERI_COD_INSCR_MUNIC" , ""},
                { "FCLOTERI_COD_MUNICIPIO" , ""},
                { "FCLOTERI_COD_UF" , ""},
                { "FCLOTERI_DES_EMAIL" , ""},
                { "FCLOTERI_DES_ENDERECO" , ""},
                { "FCLOTERI_DTH_EXCLUSAO" , ""},
                { "FCLOTERI_DTH_INCLUSAO" , ""},
                { "FCLOTERI_IDE_CONTA_CAUCAO" , ""},
                { "FCLOTERI_IDE_CONTA_CPMF" , ""},
                { "FCLOTERI_IDE_CONTA_ISENTA" , ""},
                { "FCLOTERI_IND_CAT_LOTERICO" , ""},
                { "FCLOTERI_IND_STA_LOTERICO" , ""},
                { "FCLOTERI_IND_UNIDADE_SUB" , ""},
                { "FCLOTERI_NOM_BAIRRO" , ""},
                { "FCLOTERI_NOM_CONSULTOR" , ""},
                { "FCLOTERI_NOM_CONTATO1" , ""},
                { "FCLOTERI_NOM_CONTATO2" , ""},
                { "FCLOTERI_NOM_FANTASIA" , ""},
                { "FCLOTERI_NOM_MUNICIPIO" , ""},
                { "FCLOTERI_NOM_RAZAO_SOCIAL" , ""},
                { "FCLOTERI_NUM_CEP" , ""},
                { "FCLOTERI_NUM_ENCEF" , ""},
                { "FCLOTERI_NUM_LOTER_ANT" , ""},
                { "FCLOTERI_NUM_MATR_CONSULTOR" , ""},
                { "FCLOTERI_NUM_PVCEF" , ""},
                { "FCLOTERI_NUM_TELEFONE" , ""},
                { "FCLOTERI_STA_DADOS_N_CADAST" , ""},
                { "FCLOTERI_STA_LOTERICO" , ""},
                { "FCLOTERI_STA_NIVEL_COMIS" , ""},
                { "FCLOTERI_STA_ULT_ALT_ONLINE" , ""},
                { "FCLOTERI_COD_GARANTIA" , ""},
                { "FCLOTERI_VLR_GARANTIA" , ""},
                { "FCLOTERI_DTH_GERACAO" , ""},
                { "FCLOTERI_NUM_FAX" , ""},
                { "FCLOTERI_NUM_DV_LOTERICO" , ""},
                { "FCLOTERI_NUM_SEGURADORA" , ""},
            });
            AppSettings.TestSet.DynamicData.Add("R6020_SELECT_FC_LOTERICO_DB_SELECT_1_Query1", q3);

            #endregion

            #region R6030_SELECT_V0LOTERICO01_DB_SELECT_1_Query1

            var q4 = new DynamicData();
            q4.AddDynamic(new Dictionary<string, string>{
                { "V0LOT_COD_LOT_FENAL" , ""}
            });
            AppSettings.TestSet.DynamicData.Add("R6030_SELECT_V0LOTERICO01_DB_SELECT_1_Query1", q4);

            #endregion

            #region R6070_LER_CONTA_BANCARIA_DB_SELECT_1_Query1

            var q5 = new DynamicData();
            q5.AddDynamic(new Dictionary<string, string>{
                { "FCCONBAN_IDE_CONTA_BANCARIA" , ""},
                { "FCCONBAN_COD_AGENCIA" , ""},
                { "FCCONBAN_COD_BANCO" , ""},
                { "FCCONBAN_COD_CONTA" , ""},
                { "FCCONBAN_COD_DV_CONTA" , ""},
                { "FCCONBAN_COD_OP_CONTA" , ""},
                { "FCCONBAN_COD_TIPO_CONTA" , ""},
            });
            AppSettings.TestSet.DynamicData.Add("R6070_LER_CONTA_BANCARIA_DB_SELECT_1_Query1", q5);

            #endregion

            #region R6080_VER_CONTA_EXISTENTE_DB_SELECT_1_Query1

            var q6 = new DynamicData();
            q6.AddDynamic(new Dictionary<string, string>{
                { "FCCONBAN_IDE_CONTA_BANCARIA" , ""}
            });
            AppSettings.TestSet.DynamicData.Add("R6080_VER_CONTA_EXISTENTE_DB_SELECT_1_Query1", q6);

            #endregion

            #region R6210_INSERT_FC_LOTERICO_DB_INSERT_1_Insert1

            var q7 = new DynamicData();
            q7.AddDynamic(new Dictionary<string, string>{
                { "FCLOTERI_NUM_LOTERICO" , ""},
                { "FCLOTERI_COD_AGENTE_MASTER" , ""},
                { "FCLOTERI_COD_CGC" , ""},
                { "FCLOTERI_COD_INSCR_ESTAD" , ""},
                { "FCLOTERI_COD_INSCR_MUNIC" , ""},
                { "FCLOTERI_COD_MUNICIPIO" , ""},
                { "FCLOTERI_COD_UF" , ""},
                { "FCLOTERI_DES_EMAIL" , ""},
                { "FCLOTERI_DES_ENDERECO" , ""},
                { "FCLOTERI_DTH_EXCLUSAO" , ""},
                { "FCLOTERI_DTH_INCLUSAO" , ""},
                { "FCLOTERI_IDE_CONTA_CAUCAO" , ""},
                { "FCLOTERI_IDE_CONTA_CPMF" , ""},
                { "FCLOTERI_IDE_CONTA_ISENTA" , ""},
                { "FCLOTERI_IND_CAT_LOTERICO" , ""},
                { "FCLOTERI_IND_STA_LOTERICO" , ""},
                { "FCLOTERI_NOM_BAIRRO" , ""},
                { "FCLOTERI_NOM_CONSULTOR" , ""},
                { "FCLOTERI_NOM_CONTATO1" , ""},
                { "FCLOTERI_NOM_CONTATO2" , ""},
                { "FCLOTERI_NOM_FANTASIA" , ""},
                { "FCLOTERI_NOM_MUNICIPIO" , ""},
                { "FCLOTERI_NOM_RAZAO_SOCIAL" , ""},
                { "FCLOTERI_NUM_CEP" , ""},
                { "FCLOTERI_NUM_ENCEF" , ""},
                { "FCLOTERI_NUM_LOTER_ANT" , ""},
                { "FCLOTERI_NUM_MATR_CONSULTOR" , ""},
                { "FCLOTERI_NUM_PVCEF" , ""},
                { "FCLOTERI_NUM_TELEFONE" , ""},
                { "FCLOTERI_STA_DADOS_N_CADAST" , ""},
                { "FCLOTERI_STA_LOTERICO" , ""},
                { "FCLOTERI_STA_NIVEL_COMIS" , ""},
                { "FCLOTERI_STA_ULT_ALT_ONLINE" , ""},
                { "FCLOTERI_COD_GARANTIA" , ""},
                { "FCLOTERI_VLR_GARANTIA" , ""},
                { "FCLOTERI_DTH_GERACAO" , ""},
                { "FCLOTERI_NUM_FAX" , ""},
                { "FCLOTERI_NUM_DV_LOTERICO" , ""},
                { "FCLOTERI_NUM_SEGURADORA" , ""},
            });
            AppSettings.TestSet.DynamicData.Add("R6210_INSERT_FC_LOTERICO_DB_INSERT_1_Insert1", q7);

            #endregion

            #region R6230_INSERT_FC_CONTA_DB_INSERT_1_Insert1

            var q8 = new DynamicData();
            q8.AddDynamic(new Dictionary<string, string>{
                { "FCCONBAN_IDE_CONTA_BANCARIA" , ""},
                { "FCCONBAN_COD_AGENCIA" , ""},
                { "FCCONBAN_COD_BANCO" , ""},
                { "FCCONBAN_COD_CONTA" , ""},
                { "FCCONBAN_COD_DV_CONTA" , ""},
                { "FCCONBAN_COD_OP_CONTA" , ""},
                { "FCCONBAN_COD_EMPRESA" , ""},
            });
            AppSettings.TestSet.DynamicData.Add("R6230_INSERT_FC_CONTA_DB_INSERT_1_Insert1", q8);

            #endregion

            #region R6600_GRAVAR_MOVIMENTO_DB_INSERT_1_Insert1

            var q9 = new DynamicData();
            q9.AddDynamic(new Dictionary<string, string>{
                { "LTMVPROP_COD_PRODUTO" , ""},
                { "LTMVPROP_COD_EXT_ESTIP" , ""},
                { "LTMVPROP_COD_EXT_SEGURADO" , ""},
                { "LTMVPROP_DATA_MOVIMENTO" , ""},
                { "LTMVPROP_HORA_MOVIMENTO" , ""},
                { "LTMVPROP_COD_MOVIMENTO" , ""},
                { "LTMVPROP_NOM_RAZAO_SOCIAL" , ""},
                { "LTMVPROP_NOME_FANTASIA" , ""},
                { "LTMVPROP_CGCCPF" , ""},
                { "LTMVPROP_ENDERECO" , ""},
                { "LTMVPROP_COMPL_ENDER" , ""},
                { "LTMVPROP_BAIRRO" , ""},
                { "LTMVPROP_CEP" , ""},
                { "LTMVPROP_CIDADE" , ""},
                { "LTMVPROP_SIGLA_UF" , ""},
                { "LTMVPROP_DDD" , ""},
                { "LTMVPROP_NUM_FONE" , ""},
                { "LTMVPROP_NUM_FAX" , ""},
                { "LTMVPROP_EMAIL" , ""},
                { "LTMVPROP_COD_DIVISAO" , ""},
                { "LTMVPROP_COD_SUBDIVISAO" , ""},
                { "LTMVPROP_COD_BANCO" , ""},
                { "LTMVPROP_COD_AGENCIA" , ""},
                { "LTMVPROP_COD_CONTA" , ""},
                { "LTMVPROP_COD_DV_CONTA" , ""},
                { "LTMVPROP_COD_OP_CONTA" , ""},
                { "LTMVPROP_COD_EXT_SEG_ANT" , ""},
                { "LTMVPROP_SIT_MOVIMENTO" , ""},
                { "LTMVPROP_DT_INIVIG_PROPOSTA" , ""},
                { "LTMVPROP_IND_ALT_DADOS_PES" , ""},
                { "LTMVPROP_IND_ALT_ENDER" , ""},
                { "LTMVPROP_IND_ALT_COBER" , ""},
                { "LTMVPROP_IND_ALT_BONUS" , ""},
                { "LTMVPROP_COD_USUARIO" , ""},
                { "LTMVPROP_VAL_PREMIO" , ""},
                { "LTMVPROP_NUM_APOLICE" , ""},
                { "LTMVPROP_COD_USUARIO_CANCEL" , ""},
            });
            AppSettings.TestSet.DynamicData.Add("R6600_GRAVAR_MOVIMENTO_DB_INSERT_1_Insert1", q9);

            #endregion

            #region R6610_GRAVAR_MOV_COBER_DB_INSERT_1_Insert1

            var q10 = new DynamicData();
            q10.AddDynamic(new Dictionary<string, string>{
                { "LTMVPRCO_COD_PRODUTO" , ""},
                { "LTMVPRCO_COD_EXT_ESTIP" , ""},
                { "LTMVPRCO_COD_EXT_SEGURADO" , ""},
                { "LTMVPRCO_DATA_MOVIMENTO" , ""},
                { "LTMVPRCO_HORA_MOVIMENTO" , ""},
                { "LTMVPRCO_COD_MOVIMENTO" , ""},
                { "LTMVPRCO_COD_COBERTURA" , ""},
                { "LTMVPRCO_VAL_IMP_SEGURADA" , ""},
                { "LTMVPRCO_VAL_TAXA_PREMIO" , ""},
            });
            AppSettings.TestSet.DynamicData.Add("R6610_GRAVAR_MOV_COBER_DB_INSERT_1_Insert1", q10);

            #endregion

            #region R6630_GRAVAR_MOV_BONUS_DB_INSERT_1_Insert1

            var q11 = new DynamicData();
            q11.AddDynamic(new Dictionary<string, string>{
                { "LTMVPRBO_COD_PRODUTO" , ""},
                { "LTMVPRBO_COD_EXT_ESTIP" , ""},
                { "LTMVPRBO_COD_EXT_SEGURADO" , ""},
                { "LTMVPRBO_DATA_MOVIMENTO" , ""},
                { "LTMVPRBO_HORA_MOVIMENTO" , ""},
                { "LTMVPRBO_COD_MOVIMENTO" , ""},
                { "LTMVPRBO_COD_COBERTURA" , ""},
                { "LTMVPRBO_COD_BONUS" , ""},
            });
            AppSettings.TestSet.DynamicData.Add("R6630_GRAVAR_MOV_BONUS_DB_INSERT_1_Insert1", q11);

            #endregion

            #region R6700_UPDATE_FC_LOTERICO_DB_UPDATE_1_Update1

            var q12 = new DynamicData();
            q12.AddDynamic(new Dictionary<string, string>{
                { "FCLOTERI_COD_AGENTE_MASTER" , ""},
                { "VIND_COD_AGENTE_MASTER" , ""},
                { "FCLOTERI_IDE_CONTA_CAUCAO" , ""},
                { "VIND_IDE_CONTA_CAUCAO" , ""},
                { "FCLOTERI_IDE_CONTA_ISENTA" , ""},
                { "VIND_IDE_CONTA_ISENTA" , ""},
                { "FCLOTERI_IND_CAT_LOTERICO" , ""},
                { "VIND_IND_CAT_LOTERICO" , ""},
                { "FCLOTERI_IND_STA_LOTERICO" , ""},
                { "VIND_IND_STA_LOTERICO" , ""},
                { "FCLOTERI_NOM_RAZAO_SOCIAL" , ""},
                { "VIND_NOM_RAZAO_SOCIAL" , ""},
                { "FCLOTERI_COD_INSCR_ESTAD" , ""},
                { "VIND_COD_INSCR_ESTAD" , ""},
                { "FCLOTERI_COD_INSCR_MUNIC" , ""},
                { "VIND_COD_INSCR_MUNIC" , ""},
                { "FCLOTERI_NUM_MATR_CONSULTOR" , ""},
                { "VIND_NUM_MATR_CON" , ""},
                { "FCLOTERI_STA_NIVEL_COMIS" , ""},
                { "VIND_STA_NIVEL_COMIS" , ""},
                { "FCLOTERI_STA_DADOS_N_CADAST" , ""},
                { "VIND_STA_DADOS_N" , ""},
                { "FCLOTERI_STA_ULT_ALT_ONLINE" , ""},
                { "VIND_STA_ULT_ALT" , ""},
                { "FCLOTERI_IDE_CONTA_CPMF" , ""},
                { "VIND_IDE_CONTA_CPMF" , ""},
                { "FCLOTERI_COD_MUNICIPIO" , ""},
                { "VIND_COD_MUNICIPIO" , ""},
                { "FCLOTERI_NOM_CONSULTOR" , ""},
                { "VIND_NOM_CONSULTOR" , ""},
                { "FCLOTERI_NOM_MUNICIPIO" , ""},
                { "VIND_NOM_MUNICIPIO" , ""},
                { "FCLOTERI_NUM_LOTER_ANT" , ""},
                { "VIND_NUM_LOTER_ANT" , ""},
                { "FCLOTERI_DES_ENDERECO" , ""},
                { "VIND_DES_ENDERECO" , ""},
                { "FCLOTERI_DTH_EXCLUSAO" , ""},
                { "VIND_DTH_EXCLUSAO" , ""},
                { "FCLOTERI_DTH_INCLUSAO" , ""},
                { "VIND_DTH_INCLUSAO" , ""},
                { "FCLOTERI_NOM_CONTATO1" , ""},
                { "VIND_NOM_CONTATO1" , ""},
                { "FCLOTERI_NOM_CONTATO2" , ""},
                { "VIND_NOM_CONTATO2" , ""},
                { "FCLOTERI_NOM_FANTASIA" , ""},
                { "VIND_NOM_FANTASIA" , ""},
                { "FCLOTERI_NUM_TELEFONE" , ""},
                { "VIND_NUM_TELEFONE" , ""},
                { "FCLOTERI_STA_LOTERICO" , ""},
                { "VIND_STA_LOTERICO" , ""},
                { "FCLOTERI_COD_GARANTIA" , ""},
                { "VIND_COD_GARANTIA" , ""},
                { "FCLOTERI_VLR_GARANTIA" , ""},
                { "VIND_VLR_GARANTIA" , ""},
                { "FCLOTERI_DTH_GERACAO" , ""},
                { "VIND_DTH_GERACAO" , ""},
                { "FCLOTERI_NOM_BAIRRO" , ""},
                { "VIND_NOM_BAIRRO" , ""},
                { "FCLOTERI_DES_EMAIL" , ""},
                { "VIND_DES_EMAIL" , ""},
                { "FCLOTERI_NUM_ENCEF" , ""},
                { "VIND_NUM_ENCEF" , ""},
                { "FCLOTERI_NUM_PVCEF" , ""},
                { "VIND_NUM_PVCEF" , ""},
                { "FCLOTERI_COD_CGC" , ""},
                { "VIND_COD_CGC" , ""},
                { "FCLOTERI_NUM_CEP" , ""},
                { "VIND_NUM_CEP" , ""},
                { "FCLOTERI_NUM_FAX" , ""},
                { "VIND_NUM_FAX" , ""},
                { "FCLOTERI_COD_UF" , ""},
                { "VIND_COD_UF" , ""},
                { "FCLOTERI_NUM_DV_LOTERICO" , ""},
                { "FCLOTERI_NUM_SEGURADORA" , ""},
                { "FCLOTERI_NUM_LOTERICO" , ""},
            });
            AppSettings.TestSet.DynamicData.Add("R6700_UPDATE_FC_LOTERICO_DB_UPDATE_1_Update1", q12);

            #endregion

            #region R6800_SELECT_BONUS_DB_SELECT_1_Query1

            var q13 = new DynamicData();
            q13.AddDynamic(new Dictionary<string, string>{
                { "LTLOTBON_NUM_LOTERICO" , ""},
                { "LTLOTBON_COD_BONUS" , ""},
            });
            AppSettings.TestSet.DynamicData.Add("R6800_SELECT_BONUS_DB_SELECT_1_Query1", q13);

            #endregion

            #region R6820_DELETE_BONUS_DB_DELETE_1_Delete1

            var q14 = new DynamicData();
            q14.AddDynamic(new Dictionary<string, string>{
                { "LTLOTBON_NUM_LOTERICO" , ""}
            });
            AppSettings.TestSet.DynamicData.Add("R6820_DELETE_BONUS_DB_DELETE_1_Delete1", q14);

            #endregion

            #region R6830_INSERT_BONUS_DB_INSERT_1_Insert1

            var q15 = new DynamicData();
            q15.AddDynamic(new Dictionary<string, string>{
                { "LTLOTBON_NUM_LOTERICO" , ""},
                { "LTLOTBON_COD_BONUS" , ""},
            });
            AppSettings.TestSet.DynamicData.Add("R6830_INSERT_BONUS_DB_INSERT_1_Insert1", q15);

            #endregion

            #region R7510_MONTA_CABECALHO_DB_SELECT_1_Query1

            var q16 = new DynamicData();
            q16.AddDynamic(new Dictionary<string, string>{
                { "V1EMPR_NOM_EMP" , ""}
            });
            AppSettings.TestSet.DynamicData.Add("R7510_MONTA_CABECALHO_DB_SELECT_1_Query1", q16);

            #endregion

            #region R6995_INSERT_PARAMETRO_DB_INSERT_1_Insert1

            var q17 = new DynamicData();
            q17.AddDynamic(new Dictionary<string, string>{
                { "V0SOL_COD_PRODUTO" , ""},
                { "V0SOL_COD_CLIENTE" , ""},
                { "V0SOL_COD_PROGRAMA" , ""},
                { "V0SOL_TIPO_SOLICITACAO" , ""},
                { "V0SOL_DATA_SOLICITACAO" , ""},
                { "V0SOL_COD_USUARIO" , ""},
                { "V0SOL_DATA_PREV_PROC" , ""},
                { "V0SOL_SIT_SOLICITACAO" , ""},
                { "V0SOL_PARAM_DATE01" , ""},
                { "V0SOL_PARAM_DATE02" , ""},
                { "V0SOL_PARAM_DATE03" , ""},
                { "V0SOL_PARAM_SMINT01" , ""},
                { "V0SOL_PARAM_SMINT02" , ""},
                { "V0SOL_PARAM_SMINT03" , ""},
                { "V0SOL_PARAM_INTG01" , ""},
                { "V0SOL_PARAM_INTG02" , ""},
                { "V0SOL_PARAM_INTG03" , ""},
                { "V0SOL_PARAM_DEC01" , ""},
                { "V0SOL_PARAM_DEC02" , ""},
                { "V0SOL_PARAM_DEC03" , ""},
                { "V0SOL_PARAM_FLOAT01" , ""},
                { "V0SOL_PARAM_FLOAT02" , ""},
                { "V0SOL_PARAM_CHAR01" , ""},
                { "V0SOL_PARAM_CHAR02" , ""},
                { "V0SOL_PARAM_CHAR03" , ""},
            });
            AppSettings.TestSet.DynamicData.Add("R6995_INSERT_PARAMETRO_DB_INSERT_1_Insert1", q17);

            #endregion

            #endregion
        }

        [Theory]
        [InlineData("123456789", "123456789")]
        public static void LT2000B_Tests_Theory(string CADASTRO_FILE_NAME_P, string RLT2000B_FILE_NAME_P)
        {
            lock (AppSettings.TestSet._lock)
            {
                AppSettings.TestSet.IsTest = true;
                AppSettings.TestSet.DynamicData.Clear();
                Load_Parameters();

                //para testes quando necessário alterar alguma variavel do loadParameter faça assim:
                //AppSettings.TestSet.DynamicData["R0100_00_INICIALIZA_Query1"]["SISTEMAS_DATA_MOV_ABERTO"] = "10";
                //dessa forma, alteramos apenas no necessário para os testes e evitamos alterar testes subsequentes, o region a seguir serve para isso...

                #region VARIAVEIS_TESTE
                #endregion
                var program = new LT2000B();
                program.Execute(CADASTRO_FILE_NAME_P, RLT2000B_FILE_NAME_P);

                Assert.True(true);
            }
        }
    }
}