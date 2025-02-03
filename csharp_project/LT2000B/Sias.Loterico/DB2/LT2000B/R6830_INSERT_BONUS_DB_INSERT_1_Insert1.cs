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
    public class R6830_INSERT_BONUS_DB_INSERT_1_Insert1 : QueryBasis<R6830_INSERT_BONUS_DB_INSERT_1_Insert1>
    {
        string GetQuery()
        {
            #region SQL_SOURCE
            /*EXEC SQL
            INSERT INTO SEGUROS.LT_LOTERICO_BONUS
            (NUM_LOTERICO ,
            COD_BONUS)
            VALUES (:LTLOTBON-NUM-LOTERICO ,
            :LTLOTBON-COD-BONUS)
            END-EXEC.
            */
            #endregion
            var query = @$"
				INSERT INTO SEGUROS.LT_LOTERICO_BONUS (NUM_LOTERICO , COD_BONUS) VALUES ({FieldThreatment(this.LTLOTBON_NUM_LOTERICO)} , {FieldThreatment(this.LTLOTBON_COD_BONUS)})";

            return query;
        }
        public string LTLOTBON_NUM_LOTERICO { get; set; }
        public string LTLOTBON_COD_BONUS { get; set; }

        public static void Execute(R6830_INSERT_BONUS_DB_INSERT_1_Insert1 r6830_INSERT_BONUS_DB_INSERT_1_Insert1)
        {
            var ths = r6830_INSERT_BONUS_DB_INSERT_1_Insert1;
            ths.SetQuery(ths.GetQuery());
            ths.ExecuteQuery();
        }

        public override R6830_INSERT_BONUS_DB_INSERT_1_Insert1 OpenData(List<KeyValuePair<string, object>> result)
        {
            throw new NotImplementedException();
        }

    }
}