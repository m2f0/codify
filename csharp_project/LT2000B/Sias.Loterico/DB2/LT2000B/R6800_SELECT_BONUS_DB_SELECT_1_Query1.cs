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
    public class R6800_SELECT_BONUS_DB_SELECT_1_Query1 : QueryBasis<R6800_SELECT_BONUS_DB_SELECT_1_Query1>
    {
        string GetQuery()
        {
            #region SQL_SOURCE
            /*EXEC SQL
            SELECT NUM_LOTERICO ,
            COD_BONUS
            INTO :LTLOTBON-NUM-LOTERICO,
            :LTLOTBON-COD-BONUS
            FROM SEGUROS.LT_LOTERICO_BONUS
            WHERE NUM_LOTERICO = :LTLOTBON-NUM-LOTERICO
            AND COD_BONUS = :LTLOTBON-COD-BONUS
            END-EXEC.
            */
            #endregion
            var query = @$"
				SELECT NUM_LOTERICO 
							,
											COD_BONUS
											FROM SEGUROS.LT_LOTERICO_BONUS
											WHERE NUM_LOTERICO = '{this.LTLOTBON_NUM_LOTERICO}'
											AND COD_BONUS = '{this.LTLOTBON_COD_BONUS}'";

            return query;
        }
        public string LTLOTBON_NUM_LOTERICO { get; set; }
        public string LTLOTBON_COD_BONUS { get; set; }

        public static R6800_SELECT_BONUS_DB_SELECT_1_Query1 Execute(R6800_SELECT_BONUS_DB_SELECT_1_Query1 r6800_SELECT_BONUS_DB_SELECT_1_Query1)
        {
            var ths = r6800_SELECT_BONUS_DB_SELECT_1_Query1;
            ths.SetQuery(ths.GetQuery());

            ths.Open();
            var isFetch = ths.Fetch();

            return isFetch ? ths : null;
        }

        public override R6800_SELECT_BONUS_DB_SELECT_1_Query1 OpenData(List<KeyValuePair<string, object>> result)
        {
            var dta = new R6800_SELECT_BONUS_DB_SELECT_1_Query1();
            var i = 0;
            dta.LTLOTBON_NUM_LOTERICO = result[i++].Value?.ToString();
            dta.LTLOTBON_COD_BONUS = result[i++].Value?.ToString();
            return dta;
        }

    }
}