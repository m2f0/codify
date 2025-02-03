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
    public class R6600_GRAVAR_MOVIMENTO_DB_INSERT_1_Insert1 : QueryBasis<R6600_GRAVAR_MOVIMENTO_DB_INSERT_1_Insert1>
    {
        string GetQuery()
        {
            #region SQL_SOURCE
            /*EXEC SQL INSERT INTO SEGUROS.LT_MOV_PROPOSTA
            (COD_PRODUTO,
            COD_EXT_ESTIP,
            COD_EXT_SEGURADO,
            DATA_MOVIMENTO,
            HORA_MOVIMENTO,
            COD_MOVIMENTO,
            NOM_RAZAO_SOCIAL,
            NOME_FANTASIA,
            CGCCPF,
            ENDERECO,
            COMPL_ENDER,
            BAIRRO,
            CEP,
            CIDADE,
            SIGLA_UF,
            DDD,
            NUM_FONE,
            NUM_FAX,
            EMAIL,
            COD_DIVISAO,
            COD_SUBDIVISAO,
            COD_BANCO,
            COD_AGENCIA,
            COD_CONTA,
            COD_DV_CONTA,
            COD_OP_CONTA,
            COD_EXT_SEG_ANT,
            SIT_MOVIMENTO,
            DT_INIVIG_PROPOSTA,
            IND_ALT_DADOS_PES,
            IND_ALT_ENDER,
            IND_ALT_COBER,
            IND_ALT_BONUS,
            COD_USUARIO,
            TIMESTAMP,
            VAL_PREMIO,
            NUM_APOLICE,
            COD_USUARIO_CANCEL)
            VALUES (:LTMVPROP-COD-PRODUTO,
            :LTMVPROP-COD-EXT-ESTIP,
            :LTMVPROP-COD-EXT-SEGURADO,
            :LTMVPROP-DATA-MOVIMENTO,
            :LTMVPROP-HORA-MOVIMENTO,
            :LTMVPROP-COD-MOVIMENTO,
            :LTMVPROP-NOM-RAZAO-SOCIAL,
            :LTMVPROP-NOME-FANTASIA,
            :LTMVPROP-CGCCPF,
            :LTMVPROP-ENDERECO,
            :LTMVPROP-COMPL-ENDER,
            :LTMVPROP-BAIRRO,
            :LTMVPROP-CEP,
            :LTMVPROP-CIDADE,
            :LTMVPROP-SIGLA-UF,
            :LTMVPROP-DDD,
            :LTMVPROP-NUM-FONE,
            :LTMVPROP-NUM-FAX,
            :LTMVPROP-EMAIL,
            :LTMVPROP-COD-DIVISAO,
            :LTMVPROP-COD-SUBDIVISAO,
            :LTMVPROP-COD-BANCO,
            :LTMVPROP-COD-AGENCIA,
            :LTMVPROP-COD-CONTA,
            :LTMVPROP-COD-DV-CONTA,
            :LTMVPROP-COD-OP-CONTA,
            :LTMVPROP-COD-EXT-SEG-ANT,
            :LTMVPROP-SIT-MOVIMENTO,
            :LTMVPROP-DT-INIVIG-PROPOSTA,
            :LTMVPROP-IND-ALT-DADOS-PES,
            :LTMVPROP-IND-ALT-ENDER,
            :LTMVPROP-IND-ALT-COBER,
            :LTMVPROP-IND-ALT-BONUS,
            :LTMVPROP-COD-USUARIO,
            CURRENT TIMESTAMP,
            :LTMVPROP-VAL-PREMIO,
            :LTMVPROP-NUM-APOLICE,
            :LTMVPROP-COD-USUARIO-CANCEL)
            END-EXEC.
            */
            #endregion
            var query = @$"
				INSERT INTO SEGUROS.LT_MOV_PROPOSTA (COD_PRODUTO, COD_EXT_ESTIP, COD_EXT_SEGURADO, DATA_MOVIMENTO, HORA_MOVIMENTO, COD_MOVIMENTO, NOM_RAZAO_SOCIAL, NOME_FANTASIA, CGCCPF, ENDERECO, COMPL_ENDER, BAIRRO, CEP, CIDADE, SIGLA_UF, DDD, NUM_FONE, NUM_FAX, EMAIL, COD_DIVISAO, COD_SUBDIVISAO, COD_BANCO, COD_AGENCIA, COD_CONTA, COD_DV_CONTA, COD_OP_CONTA, COD_EXT_SEG_ANT, SIT_MOVIMENTO, DT_INIVIG_PROPOSTA, IND_ALT_DADOS_PES, IND_ALT_ENDER, IND_ALT_COBER, IND_ALT_BONUS, COD_USUARIO, TIMESTAMP, VAL_PREMIO, NUM_APOLICE, COD_USUARIO_CANCEL) VALUES ({FieldThreatment(this.LTMVPROP_COD_PRODUTO)}, {FieldThreatment(this.LTMVPROP_COD_EXT_ESTIP)}, {FieldThreatment(this.LTMVPROP_COD_EXT_SEGURADO)}, {FieldThreatment(this.LTMVPROP_DATA_MOVIMENTO)}, {FieldThreatment(this.LTMVPROP_HORA_MOVIMENTO)}, {FieldThreatment(this.LTMVPROP_COD_MOVIMENTO)}, {FieldThreatment(this.LTMVPROP_NOM_RAZAO_SOCIAL)}, {FieldThreatment(this.LTMVPROP_NOME_FANTASIA)}, {FieldThreatment(this.LTMVPROP_CGCCPF)}, {FieldThreatment(this.LTMVPROP_ENDERECO)}, {FieldThreatment(this.LTMVPROP_COMPL_ENDER)}, {FieldThreatment(this.LTMVPROP_BAIRRO)}, {FieldThreatment(this.LTMVPROP_CEP)}, {FieldThreatment(this.LTMVPROP_CIDADE)}, {FieldThreatment(this.LTMVPROP_SIGLA_UF)}, {FieldThreatment(this.LTMVPROP_DDD)}, {FieldThreatment(this.LTMVPROP_NUM_FONE)}, {FieldThreatment(this.LTMVPROP_NUM_FAX)}, {FieldThreatment(this.LTMVPROP_EMAIL)}, {FieldThreatment(this.LTMVPROP_COD_DIVISAO)}, {FieldThreatment(this.LTMVPROP_COD_SUBDIVISAO)}, {FieldThreatment(this.LTMVPROP_COD_BANCO)}, {FieldThreatment(this.LTMVPROP_COD_AGENCIA)}, {FieldThreatment(this.LTMVPROP_COD_CONTA)}, {FieldThreatment(this.LTMVPROP_COD_DV_CONTA)}, {FieldThreatment(this.LTMVPROP_COD_OP_CONTA)}, {FieldThreatment(this.LTMVPROP_COD_EXT_SEG_ANT)}, {FieldThreatment(this.LTMVPROP_SIT_MOVIMENTO)}, {FieldThreatment(this.LTMVPROP_DT_INIVIG_PROPOSTA)}, {FieldThreatment(this.LTMVPROP_IND_ALT_DADOS_PES)}, {FieldThreatment(this.LTMVPROP_IND_ALT_ENDER)}, {FieldThreatment(this.LTMVPROP_IND_ALT_COBER)}, {FieldThreatment(this.LTMVPROP_IND_ALT_BONUS)}, {FieldThreatment(this.LTMVPROP_COD_USUARIO)}, CURRENT TIMESTAMP, {FieldThreatment(this.LTMVPROP_VAL_PREMIO)}, {FieldThreatment(this.LTMVPROP_NUM_APOLICE)}, {FieldThreatment(this.LTMVPROP_COD_USUARIO_CANCEL)})";

            return query;
        }
        public string LTMVPROP_COD_PRODUTO { get; set; }
        public string LTMVPROP_COD_EXT_ESTIP { get; set; }
        public string LTMVPROP_COD_EXT_SEGURADO { get; set; }
        public string LTMVPROP_DATA_MOVIMENTO { get; set; }
        public string LTMVPROP_HORA_MOVIMENTO { get; set; }
        public string LTMVPROP_COD_MOVIMENTO { get; set; }
        public string LTMVPROP_NOM_RAZAO_SOCIAL { get; set; }
        public string LTMVPROP_NOME_FANTASIA { get; set; }
        public string LTMVPROP_CGCCPF { get; set; }
        public string LTMVPROP_ENDERECO { get; set; }
        public string LTMVPROP_COMPL_ENDER { get; set; }
        public string LTMVPROP_BAIRRO { get; set; }
        public string LTMVPROP_CEP { get; set; }
        public string LTMVPROP_CIDADE { get; set; }
        public string LTMVPROP_SIGLA_UF { get; set; }
        public string LTMVPROP_DDD { get; set; }
        public string LTMVPROP_NUM_FONE { get; set; }
        public string LTMVPROP_NUM_FAX { get; set; }
        public string LTMVPROP_EMAIL { get; set; }
        public string LTMVPROP_COD_DIVISAO { get; set; }
        public string LTMVPROP_COD_SUBDIVISAO { get; set; }
        public string LTMVPROP_COD_BANCO { get; set; }
        public string LTMVPROP_COD_AGENCIA { get; set; }
        public string LTMVPROP_COD_CONTA { get; set; }
        public string LTMVPROP_COD_DV_CONTA { get; set; }
        public string LTMVPROP_COD_OP_CONTA { get; set; }
        public string LTMVPROP_COD_EXT_SEG_ANT { get; set; }
        public string LTMVPROP_SIT_MOVIMENTO { get; set; }
        public string LTMVPROP_DT_INIVIG_PROPOSTA { get; set; }
        public string LTMVPROP_IND_ALT_DADOS_PES { get; set; }
        public string LTMVPROP_IND_ALT_ENDER { get; set; }
        public string LTMVPROP_IND_ALT_COBER { get; set; }
        public string LTMVPROP_IND_ALT_BONUS { get; set; }
        public string LTMVPROP_COD_USUARIO { get; set; }
        public string LTMVPROP_VAL_PREMIO { get; set; }
        public string LTMVPROP_NUM_APOLICE { get; set; }
        public string LTMVPROP_COD_USUARIO_CANCEL { get; set; }

        public static void Execute(R6600_GRAVAR_MOVIMENTO_DB_INSERT_1_Insert1 r6600_GRAVAR_MOVIMENTO_DB_INSERT_1_Insert1)
        {
            var ths = r6600_GRAVAR_MOVIMENTO_DB_INSERT_1_Insert1;
            ths.SetQuery(ths.GetQuery());
            ths.ExecuteQuery();
        }

        public override R6600_GRAVAR_MOVIMENTO_DB_INSERT_1_Insert1 OpenData(List<KeyValuePair<string, object>> result)
        {
            throw new NotImplementedException();
        }

    }
}