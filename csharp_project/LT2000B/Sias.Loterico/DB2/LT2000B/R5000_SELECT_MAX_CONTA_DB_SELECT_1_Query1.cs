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
    public class R5000_SELECT_MAX_CONTA_DB_SELECT_1_Query1 : QueryBasis<R5000_SELECT_MAX_CONTA_DB_SELECT_1_Query1>
    {
        string GetQuery()
        {
            #region SQL_SOURCE
            /*EXEC SQL
            SELECT NUM_SEQ
            INTO :FCSEQUEN-NUM-SEQ
            FROM FDRCAP.FC_SEQUENCE
            WHERE COD_SEQ = 'COB'
            END-EXEC.
            */
            #endregion
            var query = @$"
				SELECT NUM_SEQ
											FROM FDRCAP.FC_SEQUENCE
											WHERE COD_SEQ = 'COB'";

            return query;
        }
        public string FCSEQUEN_NUM_SEQ { get; set; }

        public static R5000_SELECT_MAX_CONTA_DB_SELECT_1_Query1 Execute(R5000_SELECT_MAX_CONTA_DB_SELECT_1_Query1 r5000_SELECT_MAX_CONTA_DB_SELECT_1_Query1)
        {
            var ths = r5000_SELECT_MAX_CONTA_DB_SELECT_1_Query1;
            ths.SetQuery(ths.GetQuery());

            ths.Open();
            var isFetch = ths.Fetch();

            return isFetch ? ths : null;
        }

        public override R5000_SELECT_MAX_CONTA_DB_SELECT_1_Query1 OpenData(List<KeyValuePair<string, object>> result)
        {
            var dta = new R5000_SELECT_MAX_CONTA_DB_SELECT_1_Query1();
            var i = 0;
            dta.FCSEQUEN_NUM_SEQ = result[i++].Value?.ToString();
            return dta;
        }

    }
}