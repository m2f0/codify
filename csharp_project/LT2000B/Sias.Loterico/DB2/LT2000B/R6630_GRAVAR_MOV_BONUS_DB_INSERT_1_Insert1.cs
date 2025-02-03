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
    public class R6630_GRAVAR_MOV_BONUS_DB_INSERT_1_Insert1 : QueryBasis<R6630_GRAVAR_MOV_BONUS_DB_INSERT_1_Insert1>
    {
        string GetQuery()
        {
            #region SQL_SOURCE
            /*EXEC SQL INSERT INTO SEGUROS.LT_MOV_PROP_BONUS
            (COD_PRODUTO,
            COD_EXT_ESTIP,
            COD_EXT_SEGURADO,
            DATA_MOVIMENTO,
            HORA_MOVIMENTO,
            COD_MOVIMENTO,
            COD_COBERTURA,
            COD_BONUS)
            VALUES (:LTMVPRBO-COD-PRODUTO,
            :LTMVPRBO-COD-EXT-ESTIP,
            :LTMVPRBO-COD-EXT-SEGURADO,
            :LTMVPRBO-DATA-MOVIMENTO,
            :LTMVPRBO-HORA-MOVIMENTO,
            :LTMVPRBO-COD-MOVIMENTO,
            :LTMVPRBO-COD-COBERTURA,
            :LTMVPRBO-COD-BONUS)
            END-EXEC.
            */
            #endregion
            var query = @$"
				INSERT INTO SEGUROS.LT_MOV_PROP_BONUS (COD_PRODUTO, COD_EXT_ESTIP, COD_EXT_SEGURADO, DATA_MOVIMENTO, HORA_MOVIMENTO, COD_MOVIMENTO, COD_COBERTURA, COD_BONUS) VALUES ({FieldThreatment(this.LTMVPRBO_COD_PRODUTO)}, {FieldThreatment(this.LTMVPRBO_COD_EXT_ESTIP)}, {FieldThreatment(this.LTMVPRBO_COD_EXT_SEGURADO)}, {FieldThreatment(this.LTMVPRBO_DATA_MOVIMENTO)}, {FieldThreatment(this.LTMVPRBO_HORA_MOVIMENTO)}, {FieldThreatment(this.LTMVPRBO_COD_MOVIMENTO)}, {FieldThreatment(this.LTMVPRBO_COD_COBERTURA)}, {FieldThreatment(this.LTMVPRBO_COD_BONUS)})";

            return query;
        }
        public string LTMVPRBO_COD_PRODUTO { get; set; }
        public string LTMVPRBO_COD_EXT_ESTIP { get; set; }
        public string LTMVPRBO_COD_EXT_SEGURADO { get; set; }
        public string LTMVPRBO_DATA_MOVIMENTO { get; set; }
        public string LTMVPRBO_HORA_MOVIMENTO { get; set; }
        public string LTMVPRBO_COD_MOVIMENTO { get; set; }
        public string LTMVPRBO_COD_COBERTURA { get; set; }
        public string LTMVPRBO_COD_BONUS { get; set; }

        public static void Execute(R6630_GRAVAR_MOV_BONUS_DB_INSERT_1_Insert1 r6630_GRAVAR_MOV_BONUS_DB_INSERT_1_Insert1)
        {
            var ths = r6630_GRAVAR_MOV_BONUS_DB_INSERT_1_Insert1;
            ths.SetQuery(ths.GetQuery());
            ths.ExecuteQuery();
        }

        public override R6630_GRAVAR_MOV_BONUS_DB_INSERT_1_Insert1 OpenData(List<KeyValuePair<string, object>> result)
        {
            throw new NotImplementedException();
        }

    }
}