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
    public class R6210_INSERT_FC_LOTERICO_DB_INSERT_1_Insert1 : QueryBasis<R6210_INSERT_FC_LOTERICO_DB_INSERT_1_Insert1>
    {
        string GetQuery()
        {
            #region SQL_SOURCE
            /*EXEC SQL INSERT INTO FDRCAP.FC_LOTERICO
            (NUM_LOTERICO,
            COD_AGENTE_MASTER,
            COD_CGC,
            COD_INSCR_ESTAD,
            COD_INSCR_MUNIC,
            COD_MUNICIPIO,
            COD_UF,
            DES_EMAIL,
            DES_ENDERECO,
            DTH_EXCLUSAO,
            DTH_INCLUSAO,
            IDE_CONTA_CAUCAO,
            IDE_CONTA_CPMF,
            IDE_CONTA_ISENTA,
            IND_CAT_LOTERICO,
            IND_STA_LOTERICO,
            IND_UNIDADE_SUB,
            NOM_BAIRRO,
            NOM_CONSULTOR,
            NOM_CONTATO1,
            NOM_CONTATO2,
            NOM_FANTASIA,
            NOM_MUNICIPIO,
            NOM_RAZAO_SOCIAL,
            NUM_CEP,
            NUM_ENCEF,
            NUM_LOTER_ANT,
            NUM_MATR_CONSULTOR,
            NUM_PVCEF,
            NUM_TELEFONE,
            STA_DADOS_N_CADAST,
            STA_LOTERICO,
            STA_NIVEL_COMIS,
            STA_ULT_ALT_ONLINE,
            COD_GARANTIA,
            VLR_GARANTIA,
            DTH_GERACAO,
            NUM_FAX,
            NUM_DV_LOTERICO,
            NUM_SEGURADORA)
            VALUES (:FCLOTERI-NUM-LOTERICO,
            :FCLOTERI-COD-AGENTE-MASTER :VIND-AGENTE-MASTER,
            :FCLOTERI-COD-CGC :VIND-COD-CGC,
            :FCLOTERI-COD-INSCR-ESTAD :VIND-COD-INSCR-ESTAD,
            :FCLOTERI-COD-INSCR-MUNIC :VIND-COD-INSCR-MUNIC,
            :FCLOTERI-COD-MUNICIPIO :VIND-COD-MUNICIPIO,
            :FCLOTERI-COD-UF :VIND-COD-UF,
            :FCLOTERI-DES-EMAIL :VIND-DES-EMAIL,
            :FCLOTERI-DES-ENDERECO :VIND-DES-ENDERECO,
            :FCLOTERI-DTH-EXCLUSAO :VIND-DTH-EXCLUSAO,
            :FCLOTERI-DTH-INCLUSAO :VIND-DTH-INCLUSAO,
            :FCLOTERI-IDE-CONTA-CAUCAO :VIND-IDE-CONTA-CAUCAO,
            :FCLOTERI-IDE-CONTA-CPMF :VIND-IDE-CONTA-CPMF,
            :FCLOTERI-IDE-CONTA-ISENTA :VIND-IDE-CONTA-ISENTA,
            :FCLOTERI-IND-CAT-LOTERICO :VIND-IND-CAT-LOTERICO,
            :FCLOTERI-IND-STA-LOTERICO :VIND-IND-STA-LOTERICO,
            NULL ,
            :FCLOTERI-NOM-BAIRRO :VIND-NOM-BAIRRO,
            :FCLOTERI-NOM-CONSULTOR :VIND-NOM-CONSULTOR,
            :FCLOTERI-NOM-CONTATO1 :VIND-NOM-CONTATO1,
            :FCLOTERI-NOM-CONTATO2 :VIND-NOM-CONTATO2,
            :FCLOTERI-NOM-FANTASIA :VIND-NOM-FANTASIA,
            :FCLOTERI-NOM-MUNICIPIO :VIND-NOM-MUNICIPIO,
            :FCLOTERI-NOM-RAZAO-SOCIAL :VIND-NOM-RAZAO-SOCIAL,
            :FCLOTERI-NUM-CEP :VIND-NUM-CEP,
            :FCLOTERI-NUM-ENCEF :VIND-NUM-ENCEF,
            :FCLOTERI-NUM-LOTER-ANT :VIND-NUM-LOTER-ANT,
            :FCLOTERI-NUM-MATR-CONSULTOR :VIND-NUM-MATR-CON,
            :FCLOTERI-NUM-PVCEF :VIND-NUM-PVCEF,
            :FCLOTERI-NUM-TELEFONE :VIND-NUM-TELEFONE,
            :FCLOTERI-STA-DADOS-N-CADAST :VIND-STA-DADOS-N,
            :FCLOTERI-STA-LOTERICO :VIND-STA-LOTERICO,
            :FCLOTERI-STA-NIVEL-COMIS :VIND-STA-NIVEL-COMIS,
            :FCLOTERI-STA-ULT-ALT-ONLINE :VIND-STA-ULT-ALT,
            :FCLOTERI-COD-GARANTIA :VIND-COD-GARANTIA,
            :FCLOTERI-VLR-GARANTIA :VIND-VLR-GARANTIA,
            :FCLOTERI-DTH-GERACAO :VIND-DTH-GERACAO,
            :FCLOTERI-NUM-FAX :VIND-NUM-FAX,
            :FCLOTERI-NUM-DV-LOTERICO,
            :FCLOTERI-NUM-SEGURADORA)
            END-EXEC.
            */
            #endregion
            var query = @$"
				INSERT INTO FDRCAP.FC_LOTERICO (NUM_LOTERICO, COD_AGENTE_MASTER, COD_CGC, COD_INSCR_ESTAD, COD_INSCR_MUNIC, COD_MUNICIPIO, COD_UF, DES_EMAIL, DES_ENDERECO, DTH_EXCLUSAO, DTH_INCLUSAO, IDE_CONTA_CAUCAO, IDE_CONTA_CPMF, IDE_CONTA_ISENTA, IND_CAT_LOTERICO, IND_STA_LOTERICO, IND_UNIDADE_SUB, NOM_BAIRRO, NOM_CONSULTOR, NOM_CONTATO1, NOM_CONTATO2, NOM_FANTASIA, NOM_MUNICIPIO, NOM_RAZAO_SOCIAL, NUM_CEP, NUM_ENCEF, NUM_LOTER_ANT, NUM_MATR_CONSULTOR, NUM_PVCEF, NUM_TELEFONE, STA_DADOS_N_CADAST, STA_LOTERICO, STA_NIVEL_COMIS, STA_ULT_ALT_ONLINE, COD_GARANTIA, VLR_GARANTIA, DTH_GERACAO, NUM_FAX, NUM_DV_LOTERICO, NUM_SEGURADORA) VALUES ({FieldThreatment(this.FCLOTERI_NUM_LOTERICO)},  {FieldThreatment((this.VIND_AGENTE_MASTER?.ToInt() == -1 ? null : this.FCLOTERI_COD_AGENTE_MASTER))},  {FieldThreatment((this.VIND_COD_CGC?.ToInt() == -1 ? null : this.FCLOTERI_COD_CGC))},  {FieldThreatment((this.VIND_COD_INSCR_ESTAD?.ToInt() == -1 ? null : this.FCLOTERI_COD_INSCR_ESTAD))},  {FieldThreatment((this.VIND_COD_INSCR_MUNIC?.ToInt() == -1 ? null : this.FCLOTERI_COD_INSCR_MUNIC))},  {FieldThreatment((this.VIND_COD_MUNICIPIO?.ToInt() == -1 ? null : this.FCLOTERI_COD_MUNICIPIO))},  {FieldThreatment((this.VIND_COD_UF?.ToInt() == -1 ? null : this.FCLOTERI_COD_UF))},  {FieldThreatment((this.VIND_DES_EMAIL?.ToInt() == -1 ? null : this.FCLOTERI_DES_EMAIL))},  {FieldThreatment((this.VIND_DES_ENDERECO?.ToInt() == -1 ? null : this.FCLOTERI_DES_ENDERECO))},  {FieldThreatment((this.VIND_DTH_EXCLUSAO?.ToInt() == -1 ? null : this.FCLOTERI_DTH_EXCLUSAO))},  {FieldThreatment((this.VIND_DTH_INCLUSAO?.ToInt() == -1 ? null : this.FCLOTERI_DTH_INCLUSAO))},  {FieldThreatment((this.VIND_IDE_CONTA_CAUCAO?.ToInt() == -1 ? null : this.FCLOTERI_IDE_CONTA_CAUCAO))},  {FieldThreatment((this.VIND_IDE_CONTA_CPMF?.ToInt() == -1 ? null : this.FCLOTERI_IDE_CONTA_CPMF))},  {FieldThreatment((this.VIND_IDE_CONTA_ISENTA?.ToInt() == -1 ? null : this.FCLOTERI_IDE_CONTA_ISENTA))},  {FieldThreatment((this.VIND_IND_CAT_LOTERICO?.ToInt() == -1 ? null : this.FCLOTERI_IND_CAT_LOTERICO))},  {FieldThreatment((this.VIND_IND_STA_LOTERICO?.ToInt() == -1 ? null : this.FCLOTERI_IND_STA_LOTERICO))}, NULL ,  {FieldThreatment((this.VIND_NOM_BAIRRO?.ToInt() == -1 ? null : this.FCLOTERI_NOM_BAIRRO))},  {FieldThreatment((this.VIND_NOM_CONSULTOR?.ToInt() == -1 ? null : this.FCLOTERI_NOM_CONSULTOR))},  {FieldThreatment((this.VIND_NOM_CONTATO1?.ToInt() == -1 ? null : this.FCLOTERI_NOM_CONTATO1))},  {FieldThreatment((this.VIND_NOM_CONTATO2?.ToInt() == -1 ? null : this.FCLOTERI_NOM_CONTATO2))},  {FieldThreatment((this.VIND_NOM_FANTASIA?.ToInt() == -1 ? null : this.FCLOTERI_NOM_FANTASIA))},  {FieldThreatment((this.VIND_NOM_MUNICIPIO?.ToInt() == -1 ? null : this.FCLOTERI_NOM_MUNICIPIO))},  {FieldThreatment((this.VIND_NOM_RAZAO_SOCIAL?.ToInt() == -1 ? null : this.FCLOTERI_NOM_RAZAO_SOCIAL))},  {FieldThreatment((this.VIND_NUM_CEP?.ToInt() == -1 ? null : this.FCLOTERI_NUM_CEP))},  {FieldThreatment((this.VIND_NUM_ENCEF?.ToInt() == -1 ? null : this.FCLOTERI_NUM_ENCEF))},  {FieldThreatment((this.VIND_NUM_LOTER_ANT?.ToInt() == -1 ? null : this.FCLOTERI_NUM_LOTER_ANT))},  {FieldThreatment((this.VIND_NUM_MATR_CON?.ToInt() == -1 ? null : this.FCLOTERI_NUM_MATR_CONSULTOR))},  {FieldThreatment((this.VIND_NUM_PVCEF?.ToInt() == -1 ? null : this.FCLOTERI_NUM_PVCEF))},  {FieldThreatment((this.VIND_NUM_TELEFONE?.ToInt() == -1 ? null : this.FCLOTERI_NUM_TELEFONE))},  {FieldThreatment((this.VIND_STA_DADOS_N?.ToInt() == -1 ? null : this.FCLOTERI_STA_DADOS_N_CADAST))},  {FieldThreatment((this.VIND_STA_LOTERICO?.ToInt() == -1 ? null : this.FCLOTERI_STA_LOTERICO))},  {FieldThreatment((this.VIND_STA_NIVEL_COMIS?.ToInt() == -1 ? null : this.FCLOTERI_STA_NIVEL_COMIS))},  {FieldThreatment((this.VIND_STA_ULT_ALT?.ToInt() == -1 ? null : this.FCLOTERI_STA_ULT_ALT_ONLINE))},  {FieldThreatment((this.VIND_COD_GARANTIA?.ToInt() == -1 ? null : this.FCLOTERI_COD_GARANTIA))},  {FieldThreatment((this.VIND_VLR_GARANTIA?.ToInt() == -1 ? null : this.FCLOTERI_VLR_GARANTIA))},  {FieldThreatment((this.VIND_DTH_GERACAO?.ToInt() == -1 ? null : this.FCLOTERI_DTH_GERACAO))},  {FieldThreatment((this.VIND_NUM_FAX?.ToInt() == -1 ? null : this.FCLOTERI_NUM_FAX))}, {FieldThreatment(this.FCLOTERI_NUM_DV_LOTERICO)}, {FieldThreatment(this.FCLOTERI_NUM_SEGURADORA)})";

            return query;
        }
        public string FCLOTERI_NUM_LOTERICO { get; set; }
        public string FCLOTERI_COD_AGENTE_MASTER { get; set; }
        public string VIND_AGENTE_MASTER { get; set; }
        public string FCLOTERI_COD_CGC { get; set; }
        public string VIND_COD_CGC { get; set; }
        public string FCLOTERI_COD_INSCR_ESTAD { get; set; }
        public string VIND_COD_INSCR_ESTAD { get; set; }
        public string FCLOTERI_COD_INSCR_MUNIC { get; set; }
        public string VIND_COD_INSCR_MUNIC { get; set; }
        public string FCLOTERI_COD_MUNICIPIO { get; set; }
        public string VIND_COD_MUNICIPIO { get; set; }
        public string FCLOTERI_COD_UF { get; set; }
        public string VIND_COD_UF { get; set; }
        public string FCLOTERI_DES_EMAIL { get; set; }
        public string VIND_DES_EMAIL { get; set; }
        public string FCLOTERI_DES_ENDERECO { get; set; }
        public string VIND_DES_ENDERECO { get; set; }
        public string FCLOTERI_DTH_EXCLUSAO { get; set; }
        public string VIND_DTH_EXCLUSAO { get; set; }
        public string FCLOTERI_DTH_INCLUSAO { get; set; }
        public string VIND_DTH_INCLUSAO { get; set; }
        public string FCLOTERI_IDE_CONTA_CAUCAO { get; set; }
        public string VIND_IDE_CONTA_CAUCAO { get; set; }
        public string FCLOTERI_IDE_CONTA_CPMF { get; set; }
        public string VIND_IDE_CONTA_CPMF { get; set; }
        public string FCLOTERI_IDE_CONTA_ISENTA { get; set; }
        public string VIND_IDE_CONTA_ISENTA { get; set; }
        public string FCLOTERI_IND_CAT_LOTERICO { get; set; }
        public string VIND_IND_CAT_LOTERICO { get; set; }
        public string FCLOTERI_IND_STA_LOTERICO { get; set; }
        public string VIND_IND_STA_LOTERICO { get; set; }
        public string FCLOTERI_NOM_BAIRRO { get; set; }
        public string VIND_NOM_BAIRRO { get; set; }
        public string FCLOTERI_NOM_CONSULTOR { get; set; }
        public string VIND_NOM_CONSULTOR { get; set; }
        public string FCLOTERI_NOM_CONTATO1 { get; set; }
        public string VIND_NOM_CONTATO1 { get; set; }
        public string FCLOTERI_NOM_CONTATO2 { get; set; }
        public string VIND_NOM_CONTATO2 { get; set; }
        public string FCLOTERI_NOM_FANTASIA { get; set; }
        public string VIND_NOM_FANTASIA { get; set; }
        public string FCLOTERI_NOM_MUNICIPIO { get; set; }
        public string VIND_NOM_MUNICIPIO { get; set; }
        public string FCLOTERI_NOM_RAZAO_SOCIAL { get; set; }
        public string VIND_NOM_RAZAO_SOCIAL { get; set; }
        public string FCLOTERI_NUM_CEP { get; set; }
        public string VIND_NUM_CEP { get; set; }
        public string FCLOTERI_NUM_ENCEF { get; set; }
        public string VIND_NUM_ENCEF { get; set; }
        public string FCLOTERI_NUM_LOTER_ANT { get; set; }
        public string VIND_NUM_LOTER_ANT { get; set; }
        public string FCLOTERI_NUM_MATR_CONSULTOR { get; set; }
        public string VIND_NUM_MATR_CON { get; set; }
        public string FCLOTERI_NUM_PVCEF { get; set; }
        public string VIND_NUM_PVCEF { get; set; }
        public string FCLOTERI_NUM_TELEFONE { get; set; }
        public string VIND_NUM_TELEFONE { get; set; }
        public string FCLOTERI_STA_DADOS_N_CADAST { get; set; }
        public string VIND_STA_DADOS_N { get; set; }
        public string FCLOTERI_STA_LOTERICO { get; set; }
        public string VIND_STA_LOTERICO { get; set; }
        public string FCLOTERI_STA_NIVEL_COMIS { get; set; }
        public string VIND_STA_NIVEL_COMIS { get; set; }
        public string FCLOTERI_STA_ULT_ALT_ONLINE { get; set; }
        public string VIND_STA_ULT_ALT { get; set; }
        public string FCLOTERI_COD_GARANTIA { get; set; }
        public string VIND_COD_GARANTIA { get; set; }
        public string FCLOTERI_VLR_GARANTIA { get; set; }
        public string VIND_VLR_GARANTIA { get; set; }
        public string FCLOTERI_DTH_GERACAO { get; set; }
        public string VIND_DTH_GERACAO { get; set; }
        public string FCLOTERI_NUM_FAX { get; set; }
        public string VIND_NUM_FAX { get; set; }
        public string FCLOTERI_NUM_DV_LOTERICO { get; set; }
        public string FCLOTERI_NUM_SEGURADORA { get; set; }

        public static void Execute(R6210_INSERT_FC_LOTERICO_DB_INSERT_1_Insert1 r6210_INSERT_FC_LOTERICO_DB_INSERT_1_Insert1)
        {
            var ths = r6210_INSERT_FC_LOTERICO_DB_INSERT_1_Insert1;
            ths.SetQuery(ths.GetQuery());
            ths.ExecuteQuery();
        }

        public override R6210_INSERT_FC_LOTERICO_DB_INSERT_1_Insert1 OpenData(List<KeyValuePair<string, object>> result)
        {
            throw new NotImplementedException();
        }

    }
}