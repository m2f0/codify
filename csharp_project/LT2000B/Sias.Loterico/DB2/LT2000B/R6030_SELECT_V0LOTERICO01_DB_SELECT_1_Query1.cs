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
    public class R6030_SELECT_V0LOTERICO01_DB_SELECT_1_Query1 : QueryBasis<R6030_SELECT_V0LOTERICO01_DB_SELECT_1_Query1>
    {
        string GetQuery()
        {
            #region SQL_SOURCE
            /*EXEC SQL
            SELECT COD_LOT_FENAL
            INTO :V0LOT-COD-LOT-FENAL
            FROM SEGUROS.V0LOTERICO01
            WHERE NUM_APOLICE = :WS-NUM-APOLICE
            AND COD_LOT_FENAL = :V0LOT-COD-LOT-CEF
            AND SITUACAO < '1'
            END-EXEC.
            */
            #endregion
            var query = @$"
				SELECT COD_LOT_FENAL
											FROM SEGUROS.V0LOTERICO01
											WHERE NUM_APOLICE = '{this.WS_NUM_APOLICE}'
											AND COD_LOT_FENAL = '{this.V0LOT_COD_LOT_CEF}'
											AND SITUACAO < '1'";

            return query;
        }
        public string V0LOT_COD_LOT_FENAL { get; set; }
        public string V0LOT_COD_LOT_CEF { get; set; }
        public string WS_NUM_APOLICE { get; set; }

        public static R6030_SELECT_V0LOTERICO01_DB_SELECT_1_Query1 Execute(R6030_SELECT_V0LOTERICO01_DB_SELECT_1_Query1 r6030_SELECT_V0LOTERICO01_DB_SELECT_1_Query1)
        {
            var ths = r6030_SELECT_V0LOTERICO01_DB_SELECT_1_Query1;
            ths.SetQuery(ths.GetQuery());

            ths.Open();
            var isFetch = ths.Fetch();

            return isFetch ? ths : null;
        }

        public override R6030_SELECT_V0LOTERICO01_DB_SELECT_1_Query1 OpenData(List<KeyValuePair<string, object>> result)
        {
            var dta = new R6030_SELECT_V0LOTERICO01_DB_SELECT_1_Query1();
            var i = 0;
            dta.V0LOT_COD_LOT_FENAL = result[i++].Value?.ToString();
            return dta;
        }

    }
}