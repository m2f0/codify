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
    public class R6070_LER_CONTA_BANCARIA_DB_SELECT_1_Query1 : QueryBasis<R6070_LER_CONTA_BANCARIA_DB_SELECT_1_Query1>
    {
        string GetQuery()
        {
            #region SQL_SOURCE
            /*EXEC SQL
            SELECT
            IDE_CONTA_BANCARIA,
            COD_AGENCIA,
            COD_BANCO,
            COD_CONTA,
            COD_DV_CONTA,
            COD_OP_CONTA,
            COD_TIPO_CONTA
            INTO :FCCONBAN-IDE-CONTA-BANCARIA,
            :FCCONBAN-COD-AGENCIA,
            :FCCONBAN-COD-BANCO,
            :FCCONBAN-COD-CONTA,
            :FCCONBAN-COD-DV-CONTA,
            :FCCONBAN-COD-OP-CONTA,
            :FCCONBAN-COD-TIPO-CONTA
            FROM FDRCAP.FC_CONTA_BANCARIA
            WHERE IDE_CONTA_BANCARIA =:FCCONBAN-IDE-CONTA-BANCARIA
            END-EXEC.
            */
            #endregion
            var query = @$"
				SELECT
											IDE_CONTA_BANCARIA
							,
											COD_AGENCIA
							,
											COD_BANCO
							,
											COD_CONTA
							,
											COD_DV_CONTA
							,
											COD_OP_CONTA
							,
											COD_TIPO_CONTA
											FROM FDRCAP.FC_CONTA_BANCARIA
											WHERE IDE_CONTA_BANCARIA ='{this.FCCONBAN_IDE_CONTA_BANCARIA}'";

            return query;
        }
        public string FCCONBAN_IDE_CONTA_BANCARIA { get; set; }
        public string FCCONBAN_COD_AGENCIA { get; set; }
        public string FCCONBAN_COD_BANCO { get; set; }
        public string FCCONBAN_COD_CONTA { get; set; }
        public string FCCONBAN_COD_DV_CONTA { get; set; }
        public string FCCONBAN_COD_OP_CONTA { get; set; }
        public string FCCONBAN_COD_TIPO_CONTA { get; set; }

        public static R6070_LER_CONTA_BANCARIA_DB_SELECT_1_Query1 Execute(R6070_LER_CONTA_BANCARIA_DB_SELECT_1_Query1 r6070_LER_CONTA_BANCARIA_DB_SELECT_1_Query1)
        {
            var ths = r6070_LER_CONTA_BANCARIA_DB_SELECT_1_Query1;
            ths.SetQuery(ths.GetQuery());

            ths.Open();
            var isFetch = ths.Fetch();

            return isFetch ? ths : null;
        }

        public override R6070_LER_CONTA_BANCARIA_DB_SELECT_1_Query1 OpenData(List<KeyValuePair<string, object>> result)
        {
            var dta = new R6070_LER_CONTA_BANCARIA_DB_SELECT_1_Query1();
            var i = 0;
            dta.FCCONBAN_IDE_CONTA_BANCARIA = result[i++].Value?.ToString();
            dta.FCCONBAN_COD_AGENCIA = result[i++].Value?.ToString();
            dta.FCCONBAN_COD_BANCO = result[i++].Value?.ToString();
            dta.FCCONBAN_COD_CONTA = result[i++].Value?.ToString();
            dta.FCCONBAN_COD_DV_CONTA = result[i++].Value?.ToString();
            dta.FCCONBAN_COD_OP_CONTA = result[i++].Value?.ToString();
            dta.FCCONBAN_COD_TIPO_CONTA = result[i++].Value?.ToString();
            return dta;
        }

    }
}