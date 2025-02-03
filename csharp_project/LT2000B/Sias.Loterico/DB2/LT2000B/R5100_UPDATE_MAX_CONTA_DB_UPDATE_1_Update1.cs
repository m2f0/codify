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
    public class R5100_UPDATE_MAX_CONTA_DB_UPDATE_1_Update1 : QueryBasis<R5100_UPDATE_MAX_CONTA_DB_UPDATE_1_Update1>
    {
        string GetQuery()
        {
            var query = @$"
				UPDATE FDRCAP.FC_SEQUENCE
				SET NUM_SEQ =  '{this.FCSEQUEN_NUM_SEQ}'
				WHERE COD_SEQ = 'COB'";

            return query;
        }
        public string FCSEQUEN_NUM_SEQ { get; set; }

        public static void Execute(R5100_UPDATE_MAX_CONTA_DB_UPDATE_1_Update1 r5100_UPDATE_MAX_CONTA_DB_UPDATE_1_Update1)
        {
            var ths = r5100_UPDATE_MAX_CONTA_DB_UPDATE_1_Update1;
            ths.SetQuery(ths.GetQuery());
            ths.ExecuteQuery();
        }

        public override R5100_UPDATE_MAX_CONTA_DB_UPDATE_1_Update1 OpenData(List<KeyValuePair<string, object>> result)
        {
            throw new NotImplementedException();
        }

    }
}