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
    public class R6820_DELETE_BONUS_DB_DELETE_1_Delete1 : QueryBasis<R6820_DELETE_BONUS_DB_DELETE_1_Delete1>
    {
        string GetQuery()
        {
            #region SQL_SOURCE
            /*EXEC SQL
            DELETE FROM SEGUROS.LT_LOTERICO_BONUS
            WHERE
            NUM_LOTERICO =:LTLOTBON-NUM-LOTERICO
            END-EXEC.
            */
            #endregion
            var query = @$"
				DELETE FROM SEGUROS.LT_LOTERICO_BONUS
				WHERE
				NUM_LOTERICO ='{this.LTLOTBON_NUM_LOTERICO}'";

            return query;
        }
        public string LTLOTBON_NUM_LOTERICO { get; set; }

        public static void Execute(R6820_DELETE_BONUS_DB_DELETE_1_Delete1 r6820_DELETE_BONUS_DB_DELETE_1_Delete1)
        {
            var ths = r6820_DELETE_BONUS_DB_DELETE_1_Delete1;
            ths.SetQuery(ths.GetQuery());
            ths.ExecuteQuery();
        }

        public override R6820_DELETE_BONUS_DB_DELETE_1_Delete1 OpenData(List<KeyValuePair<string, object>> result)
        {
            throw new NotImplementedException();
        }

    }
}