using System;
using IA_ConverterCommons;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Linq;
using _ = IA_ConverterCommons.Statements;
using DB = IA_ConverterCommons.DatabaseBasis;

namespace Sias.Loterico.DB2.LT2000B
{
    public class R6020_SELECT_FC_LOTERICO_DB_SELECT_1_Query1 : QueryBasis<R6020_SELECT_FC_LOTERICO_DB_SELECT_1_Query1>
    {
        string GetQuery()
        {
            #region SQL_SOURCE
            /*EXEC SQL
            SELECT VALUE(COD_AGENTE_MASTER, ' ' ),
            VALUE(COD_CGC, '000000000000000' ),
            VALUE(COD_INSCR_ESTAD, ' ' ),
            VALUE(COD_INSCR_MUNIC, ' ' ),
            VALUE(COD_MUNICIPIO, ' ' ),
            VALUE(COD_UF, ' ' ),
            VALUE(DES_EMAIL, ' ' ),
            VALUE(DES_ENDERECO, ' ' ),
            VALUE(DTH_EXCLUSAO,DATE( '9999-12-31' )),
            VALUE(DTH_INCLUSAO,DATE( '9999-12-31' )),
            VALUE(IDE_CONTA_CAUCAO,0),
            VALUE(IDE_CONTA_CPMF,0),
            VALUE(IDE_CONTA_ISENTA,0),
            VALUE(IND_CAT_LOTERICO,0),
            VALUE(IND_STA_LOTERICO,0),
            VALUE(IND_UNIDADE_SUB,0),
            VALUE(NOM_BAIRRO, ' ' ),
            VALUE(NOM_CONSULTOR, ' ' ),
            VALUE(NOM_CONTATO1, ' ' ),
            VALUE(NOM_CONTATO2, ' ' ),
            VALUE(NOM_FANTASIA, ' ' ),
            VALUE(NOM_MUNICIPIO, ' ' ),
            VALUE(NOM_RAZAO_SOCIAL, ' ' ),
            VALUE(NUM_CEP,0),
            VALUE(NUM_ENCEF,0),
            VALUE(NUM_LOTER_ANT,0),
            VALUE(NUM_MATR_CONSULTOR,0),
            VALUE(NUM_PVCEF,0),
            VALUE(NUM_TELEFONE, '000000000000' ),
            VALUE(STA_DADOS_N_CADAST, ' ' ),
            VALUE(STA_LOTERICO, ' ' ),
            VALUE(STA_NIVEL_COMIS, ' ' ),
            VALUE(STA_ULT_ALT_ONLINE, ' ' ),
            VALUE(COD_GARANTIA, ' ' ),
            VALUE(VLR_GARANTIA,0),
            VALUE(DTH_GERACAO,DATE( '9999-12-31' )),
            VALUE(NUM_FAX, '000000000000' ),
            VALUE(NUM_DV_LOTERICO,0),
            VALUE(NUM_SEGURADORA,0)
            INTO :FCLOTERI-COD-AGENTE-MASTER ,
            :FCLOTERI-COD-CGC ,
            :FCLOTERI-COD-INSCR-ESTAD ,
            :FCLOTERI-COD-INSCR-MUNIC ,
            :FCLOTERI-COD-MUNICIPIO ,
            :FCLOTERI-COD-UF ,
            :FCLOTERI-DES-EMAIL ,
            :FCLOTERI-DES-ENDERECO ,
            :FCLOTERI-DTH-EXCLUSAO ,
            :FCLOTERI-DTH-INCLUSAO ,
            :FCLOTERI-IDE-CONTA-CAUCAO ,
            :FCLOTERI-IDE-CONTA-CPMF ,
            :FCLOTERI-IDE-CONTA-ISENTA ,
            :FCLOTERI-IND-CAT-LOTERICO ,
            :FCLOTERI-IND-STA-LOTERICO ,
            :FCLOTERI-IND-UNIDADE-SUB ,
            :FCLOTERI-NOM-BAIRRO ,
            :FCLOTERI-NOM-CONSULTOR ,
            :FCLOTERI-NOM-CONTATO1 ,
            :FCLOTERI-NOM-CONTATO2 ,
            :FCLOTERI-NOM-FANTASIA ,
            :FCLOTERI-NOM-MUNICIPIO ,
            :FCLOTERI-NOM-RAZAO-SOCIAL ,
            :FCLOTERI-NUM-CEP ,
            :FCLOTERI-NUM-ENCEF ,
            :FCLOTERI-NUM-LOTER-ANT ,
            :FCLOTERI-NUM-MATR-CONSULTOR,
            :FCLOTERI-NUM-PVCEF ,
            :FCLOTERI-NUM-TELEFONE ,
            :FCLOTERI-STA-DADOS-N-CADAST,
            :FCLOTERI-STA-LOTERICO ,
            :FCLOTERI-STA-NIVEL-COMIS ,
            :FCLOTERI-STA-ULT-ALT-ONLINE,
            :FCLOTERI-COD-GARANTIA ,
            :FCLOTERI-VLR-GARANTIA ,
            :FCLOTERI-DTH-GERACAO ,
            :FCLOTERI-NUM-FAX ,
            :FCLOTERI-NUM-DV-LOTERICO ,
            :FCLOTERI-NUM-SEGURADORA
            FROM FDRCAP.FC_LOTERICO
            WHERE NUM_LOTERICO = :FCLOTERI-NUM-LOTERICO
            END-EXEC.
            */
            #endregion
            var query = @$"
				SELECT VALUE(COD_AGENTE_MASTER
							, ' ' )
							,
											VALUE(COD_CGC
							, '000000000000000' )
							,
											VALUE(COD_INSCR_ESTAD
							, ' ' )
							,
											VALUE(COD_INSCR_MUNIC
							, ' ' )
							,
											VALUE(COD_MUNICIPIO
							, ' ' )
							,
											VALUE(COD_UF
							, ' ' )
							,
											VALUE(DES_EMAIL
							, ' ' )
							,
											VALUE(DES_ENDERECO
							, ' ' )
							,
											VALUE(DTH_EXCLUSAO
							,DATE( '9999-12-31' ))
							,
											VALUE(DTH_INCLUSAO
							,DATE( '9999-12-31' ))
							,
											VALUE(IDE_CONTA_CAUCAO
							,0)
							,
											VALUE(IDE_CONTA_CPMF
							,0)
							,
											VALUE(IDE_CONTA_ISENTA
							,0)
							,
											VALUE(IND_CAT_LOTERICO
							,0)
							,
											VALUE(IND_STA_LOTERICO
							,0)
							,
											VALUE(IND_UNIDADE_SUB
							,0)
							,
											VALUE(NOM_BAIRRO
							, ' ' )
							,
											VALUE(NOM_CONSULTOR
							, ' ' )
							,
											VALUE(NOM_CONTATO1
							, ' ' )
							,
											VALUE(NOM_CONTATO2
							, ' ' )
							,
											VALUE(NOM_FANTASIA
							, ' ' )
							,
											VALUE(NOM_MUNICIPIO
							, ' ' )
							,
											VALUE(NOM_RAZAO_SOCIAL
							, ' ' )
							,
											VALUE(NUM_CEP
							,0)
							,
											VALUE(NUM_ENCEF
							,0)
							,
											VALUE(NUM_LOTER_ANT
							,0)
							,
											VALUE(NUM_MATR_CONSULTOR
							,0)
							,
											VALUE(NUM_PVCEF
							,0)
							,
											VALUE(NUM_TELEFONE
							, '000000000000' )
							,
											VALUE(STA_DADOS_N_CADAST
							, ' ' )
							,
											VALUE(STA_LOTERICO
							, ' ' )
							,
											VALUE(STA_NIVEL_COMIS
							, ' ' )
							,
											VALUE(STA_ULT_ALT_ONLINE
							, ' ' )
							,
											VALUE(COD_GARANTIA
							, ' ' )
							,
											VALUE(VLR_GARANTIA
							,0)
							,
											VALUE(DTH_GERACAO
							,DATE( '9999-12-31' ))
							,
											VALUE(NUM_FAX
							, '000000000000' )
							,
											VALUE(NUM_DV_LOTERICO
							,0)
							,
											VALUE(NUM_SEGURADORA
							,0)
											FROM FDRCAP.FC_LOTERICO
											WHERE NUM_LOTERICO = '{this.FCLOTERI_NUM_LOTERICO}'";

            return query;
        }
        public string FCLOTERI_COD_AGENTE_MASTER { get; set; }
        public string FCLOTERI_COD_CGC { get; set; }
        public string FCLOTERI_COD_INSCR_ESTAD { get; set; }
        public string FCLOTERI_COD_INSCR_MUNIC { get; set; }
        public string FCLOTERI_COD_MUNICIPIO { get; set; }
        public string FCLOTERI_COD_UF { get; set; }
        public string FCLOTERI_DES_EMAIL { get; set; }
        public string FCLOTERI_DES_ENDERECO { get; set; }
        public string FCLOTERI_DTH_EXCLUSAO { get; set; }
        public string FCLOTERI_DTH_INCLUSAO { get; set; }
        public string FCLOTERI_IDE_CONTA_CAUCAO { get; set; }
        public string FCLOTERI_IDE_CONTA_CPMF { get; set; }
        public string FCLOTERI_IDE_CONTA_ISENTA { get; set; }
        public string FCLOTERI_IND_CAT_LOTERICO { get; set; }
        public string FCLOTERI_IND_STA_LOTERICO { get; set; }
        public string FCLOTERI_IND_UNIDADE_SUB { get; set; }
        public string FCLOTERI_NOM_BAIRRO { get; set; }
        public string FCLOTERI_NOM_CONSULTOR { get; set; }
        public string FCLOTERI_NOM_CONTATO1 { get; set; }
        public string FCLOTERI_NOM_CONTATO2 { get; set; }
        public string FCLOTERI_NOM_FANTASIA { get; set; }
        public string FCLOTERI_NOM_MUNICIPIO { get; set; }
        public string FCLOTERI_NOM_RAZAO_SOCIAL { get; set; }
        public string FCLOTERI_NUM_CEP { get; set; }
        public string FCLOTERI_NUM_ENCEF { get; set; }
        public string FCLOTERI_NUM_LOTER_ANT { get; set; }
        public string FCLOTERI_NUM_MATR_CONSULTOR { get; set; }
        public string FCLOTERI_NUM_PVCEF { get; set; }
        public string FCLOTERI_NUM_TELEFONE { get; set; }
        public string FCLOTERI_STA_DADOS_N_CADAST { get; set; }
        public string FCLOTERI_STA_LOTERICO { get; set; }
        public string FCLOTERI_STA_NIVEL_COMIS { get; set; }
        public string FCLOTERI_STA_ULT_ALT_ONLINE { get; set; }
        public string FCLOTERI_COD_GARANTIA { get; set; }
        public string FCLOTERI_VLR_GARANTIA { get; set; }
        public string FCLOTERI_DTH_GERACAO { get; set; }
        public string FCLOTERI_NUM_FAX { get; set; }
        public string FCLOTERI_NUM_DV_LOTERICO { get; set; }
        public string FCLOTERI_NUM_SEGURADORA { get; set; }
        public string FCLOTERI_NUM_LOTERICO { get; set; }

        public static R6020_SELECT_FC_LOTERICO_DB_SELECT_1_Query1 Execute(R6020_SELECT_FC_LOTERICO_DB_SELECT_1_Query1 r6020_SELECT_FC_LOTERICO_DB_SELECT_1_Query1)
        {
            var ths = r6020_SELECT_FC_LOTERICO_DB_SELECT_1_Query1;
            ths.SetQuery(ths.GetQuery());

            ths.Open();
            var isFetch = ths.Fetch();

            return isFetch ? ths : null;
        }

        public override R6020_SELECT_FC_LOTERICO_DB_SELECT_1_Query1 OpenData(List<KeyValuePair<string, object>> result)
        {
            var dta = new R6020_SELECT_FC_LOTERICO_DB_SELECT_1_Query1();
            var i = 0;
            dta.FCLOTERI_COD_AGENTE_MASTER = result[i++].Value?.ToString();
            dta.FCLOTERI_COD_CGC = result[i++].Value?.ToString();
            dta.FCLOTERI_COD_INSCR_ESTAD = result[i++].Value?.ToString();
            dta.FCLOTERI_COD_INSCR_MUNIC = result[i++].Value?.ToString();
            dta.FCLOTERI_COD_MUNICIPIO = result[i++].Value?.ToString();
            dta.FCLOTERI_COD_UF = result[i++].Value?.ToString();
            dta.FCLOTERI_DES_EMAIL = result[i++].Value?.ToString();
            dta.FCLOTERI_DES_ENDERECO = result[i++].Value?.ToString();
            dta.FCLOTERI_DTH_EXCLUSAO = result[i++].Value?.ToString();
            dta.FCLOTERI_DTH_INCLUSAO = result[i++].Value?.ToString();
            dta.FCLOTERI_IDE_CONTA_CAUCAO = result[i++].Value?.ToString();
            dta.FCLOTERI_IDE_CONTA_CPMF = result[i++].Value?.ToString();
            dta.FCLOTERI_IDE_CONTA_ISENTA = result[i++].Value?.ToString();
            dta.FCLOTERI_IND_CAT_LOTERICO = result[i++].Value?.ToString();
            dta.FCLOTERI_IND_STA_LOTERICO = result[i++].Value?.ToString();
            dta.FCLOTERI_IND_UNIDADE_SUB = result[i++].Value?.ToString();
            dta.FCLOTERI_NOM_BAIRRO = result[i++].Value?.ToString();
            dta.FCLOTERI_NOM_CONSULTOR = result[i++].Value?.ToString();
            dta.FCLOTERI_NOM_CONTATO1 = result[i++].Value?.ToString();
            dta.FCLOTERI_NOM_CONTATO2 = result[i++].Value?.ToString();
            dta.FCLOTERI_NOM_FANTASIA = result[i++].Value?.ToString();
            dta.FCLOTERI_NOM_MUNICIPIO = result[i++].Value?.ToString();
            dta.FCLOTERI_NOM_RAZAO_SOCIAL = result[i++].Value?.ToString();
            dta.FCLOTERI_NUM_CEP = result[i++].Value?.ToString();
            dta.FCLOTERI_NUM_ENCEF = result[i++].Value?.ToString();
            dta.FCLOTERI_NUM_LOTER_ANT = result[i++].Value?.ToString();
            dta.FCLOTERI_NUM_MATR_CONSULTOR = result[i++].Value?.ToString();
            dta.FCLOTERI_NUM_PVCEF = result[i++].Value?.ToString();
            dta.FCLOTERI_NUM_TELEFONE = result[i++].Value?.ToString();
            dta.FCLOTERI_STA_DADOS_N_CADAST = result[i++].Value?.ToString();
            dta.FCLOTERI_STA_LOTERICO = result[i++].Value?.ToString();
            dta.FCLOTERI_STA_NIVEL_COMIS = result[i++].Value?.ToString();
            dta.FCLOTERI_STA_ULT_ALT_ONLINE = result[i++].Value?.ToString();
            dta.FCLOTERI_COD_GARANTIA = result[i++].Value?.ToString();
            dta.FCLOTERI_VLR_GARANTIA = result[i++].Value?.ToString();
            dta.FCLOTERI_DTH_GERACAO = result[i++].Value?.ToString();
            dta.FCLOTERI_NUM_FAX = result[i++].Value?.ToString();
            dta.FCLOTERI_NUM_DV_LOTERICO = result[i++].Value?.ToString();
            dta.FCLOTERI_NUM_SEGURADORA = result[i++].Value?.ToString();
            return dta;
        }

    }
}