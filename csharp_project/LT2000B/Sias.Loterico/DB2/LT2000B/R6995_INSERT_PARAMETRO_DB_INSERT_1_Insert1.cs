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
    public class R6995_INSERT_PARAMETRO_DB_INSERT_1_Insert1 : QueryBasis<R6995_INSERT_PARAMETRO_DB_INSERT_1_Insert1>
    {
        string GetQuery()
        {
            #region SQL_SOURCE
            /*EXEC SQL INSERT INTO SEGUROS.LT_SOLICITA_PARAM
            (COD_PRODUTO ,
            COD_CLIENTE ,
            COD_PROGRAMA ,
            TIPO_SOLICITACAO,
            DATA_SOLICITACAO,
            COD_USUARIO ,
            DATA_PREV_PROC ,
            SIT_SOLICITACAO ,
            TSTMP_SITUACAO ,
            PARAM_DATE01 ,
            PARAM_DATE02 ,
            PARAM_DATE03 ,
            PARAM_SMINT01 ,
            PARAM_SMINT02 ,
            PARAM_SMINT03 ,
            PARAM_INTG01 ,
            PARAM_INTG02 ,
            PARAM_INTG03 ,
            PARAM_DEC01 ,
            PARAM_DEC02 ,
            PARAM_DEC03 ,
            PARAM_FLOAT01 ,
            PARAM_FLOAT02 ,
            PARAM_CHAR01 ,
            PARAM_CHAR02 ,
            PARAM_CHAR03 ,
            PARAM_CHAR04)
            VALUES (:V0SOL-COD-PRODUTO ,
            :V0SOL-COD-CLIENTE ,
            :V0SOL-COD-PROGRAMA ,
            :V0SOL-TIPO-SOLICITACAO ,
            :V0SOL-DATA-SOLICITACAO ,
            :V0SOL-COD-USUARIO ,
            :V0SOL-DATA-PREV-PROC ,
            :V0SOL-SIT-SOLICITACAO ,
            CURRENT TIMESTAMP ,
            :V0SOL-PARAM-DATE01 ,
            :V0SOL-PARAM-DATE02 ,
            :V0SOL-PARAM-DATE03 ,
            :V0SOL-PARAM-SMINT01 ,
            :V0SOL-PARAM-SMINT02 ,
            :V0SOL-PARAM-SMINT03 ,
            :V0SOL-PARAM-INTG01 ,
            :V0SOL-PARAM-INTG02 ,
            :V0SOL-PARAM-INTG03 ,
            :V0SOL-PARAM-DEC01 ,
            :V0SOL-PARAM-DEC02 ,
            :V0SOL-PARAM-DEC03 ,
            :V0SOL-PARAM-FLOAT01 ,
            :V0SOL-PARAM-FLOAT02 ,
            :V0SOL-PARAM-CHAR01 ,
            :V0SOL-PARAM-CHAR02 ,
            :V0SOL-PARAM-CHAR03 ,
            :V0SOL-PARAM-CHAR03)
            END-EXEC.
            */
            #endregion
            var query = @$"
				INSERT INTO SEGUROS.LT_SOLICITA_PARAM (COD_PRODUTO , COD_CLIENTE , COD_PROGRAMA , TIPO_SOLICITACAO, DATA_SOLICITACAO, COD_USUARIO , DATA_PREV_PROC , SIT_SOLICITACAO , TSTMP_SITUACAO , PARAM_DATE01 , PARAM_DATE02 , PARAM_DATE03 , PARAM_SMINT01 , PARAM_SMINT02 , PARAM_SMINT03 , PARAM_INTG01 , PARAM_INTG02 , PARAM_INTG03 , PARAM_DEC01 , PARAM_DEC02 , PARAM_DEC03 , PARAM_FLOAT01 , PARAM_FLOAT02 , PARAM_CHAR01 , PARAM_CHAR02 , PARAM_CHAR03 , PARAM_CHAR04) VALUES ({FieldThreatment(this.V0SOL_COD_PRODUTO)} , {FieldThreatment(this.V0SOL_COD_CLIENTE)} , {FieldThreatment(this.V0SOL_COD_PROGRAMA)} , {FieldThreatment(this.V0SOL_TIPO_SOLICITACAO)} , {FieldThreatment(this.V0SOL_DATA_SOLICITACAO)} , {FieldThreatment(this.V0SOL_COD_USUARIO)} , {FieldThreatment(this.V0SOL_DATA_PREV_PROC)} , {FieldThreatment(this.V0SOL_SIT_SOLICITACAO)} , CURRENT TIMESTAMP , {FieldThreatment(this.V0SOL_PARAM_DATE01)} , {FieldThreatment(this.V0SOL_PARAM_DATE02)} , {FieldThreatment(this.V0SOL_PARAM_DATE03)} , {FieldThreatment(this.V0SOL_PARAM_SMINT01)} , {FieldThreatment(this.V0SOL_PARAM_SMINT02)} , {FieldThreatment(this.V0SOL_PARAM_SMINT03)} , {FieldThreatment(this.V0SOL_PARAM_INTG01)} , {FieldThreatment(this.V0SOL_PARAM_INTG02)} , {FieldThreatment(this.V0SOL_PARAM_INTG03)} , {FieldThreatment(this.V0SOL_PARAM_DEC01)} , {FieldThreatment(this.V0SOL_PARAM_DEC02)} , {FieldThreatment(this.V0SOL_PARAM_DEC03)} , {FieldThreatment(this.V0SOL_PARAM_FLOAT01)} , {FieldThreatment(this.V0SOL_PARAM_FLOAT02)} , {FieldThreatment(this.V0SOL_PARAM_CHAR01)} , {FieldThreatment(this.V0SOL_PARAM_CHAR02)} , {FieldThreatment(this.V0SOL_PARAM_CHAR03)} , {FieldThreatment(this.V0SOL_PARAM_CHAR03)})";

            return query;
        }
        public string V0SOL_COD_PRODUTO { get; set; }
        public string V0SOL_COD_CLIENTE { get; set; }
        public string V0SOL_COD_PROGRAMA { get; set; }
        public string V0SOL_TIPO_SOLICITACAO { get; set; }
        public string V0SOL_DATA_SOLICITACAO { get; set; }
        public string V0SOL_COD_USUARIO { get; set; }
        public string V0SOL_DATA_PREV_PROC { get; set; }
        public string V0SOL_SIT_SOLICITACAO { get; set; }
        public string V0SOL_PARAM_DATE01 { get; set; }
        public string V0SOL_PARAM_DATE02 { get; set; }
        public string V0SOL_PARAM_DATE03 { get; set; }
        public string V0SOL_PARAM_SMINT01 { get; set; }
        public string V0SOL_PARAM_SMINT02 { get; set; }
        public string V0SOL_PARAM_SMINT03 { get; set; }
        public string V0SOL_PARAM_INTG01 { get; set; }
        public string V0SOL_PARAM_INTG02 { get; set; }
        public string V0SOL_PARAM_INTG03 { get; set; }
        public string V0SOL_PARAM_DEC01 { get; set; }
        public string V0SOL_PARAM_DEC02 { get; set; }
        public string V0SOL_PARAM_DEC03 { get; set; }
        public string V0SOL_PARAM_FLOAT01 { get; set; }
        public string V0SOL_PARAM_FLOAT02 { get; set; }
        public string V0SOL_PARAM_CHAR01 { get; set; }
        public string V0SOL_PARAM_CHAR02 { get; set; }
        public string V0SOL_PARAM_CHAR03 { get; set; }

        public static void Execute(R6995_INSERT_PARAMETRO_DB_INSERT_1_Insert1 r6995_INSERT_PARAMETRO_DB_INSERT_1_Insert1)
        {
            var ths = r6995_INSERT_PARAMETRO_DB_INSERT_1_Insert1;
            ths.SetQuery(ths.GetQuery());
            ths.ExecuteQuery();
        }

        public override R6995_INSERT_PARAMETRO_DB_INSERT_1_Insert1 OpenData(List<KeyValuePair<string, object>> result)
        {
            throw new NotImplementedException();
        }

    }
}