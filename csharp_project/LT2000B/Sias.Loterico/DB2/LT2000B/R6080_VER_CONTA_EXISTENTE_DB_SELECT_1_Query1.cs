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
    public class R6080_VER_CONTA_EXISTENTE_DB_SELECT_1_Query1 : QueryBasis<R6080_VER_CONTA_EXISTENTE_DB_SELECT_1_Query1>
    {
        string GetQuery()
        {
            #region SQL_SOURCE
            /*EXEC SQL
            SELECT
            IDE_CONTA_BANCARIA
            INTO :FCCONBAN-IDE-CONTA-BANCARIA
            FROM FDRCAP.FC_CONTA_BANCARIA
            WHERE
            COD_BANCO = :FCCONBAN-COD-BANCO
            AND COD_AGENCIA = :FCCONBAN-COD-AGENCIA
            AND COD_CONTA = :FCCONBAN-COD-CONTA
            AND COD_OP_CONTA = :FCCONBAN-COD-OP-CONTA
            AND COD_DV_CONTA = :FCCONBAN-COD-DV-CONTA
            AND COD_TIPO_CONTA = 'LOT'
            END-EXEC.
            */
            #endregion
            var query = @$"
				SELECT
											IDE_CONTA_BANCARIA
											FROM FDRCAP.FC_CONTA_BANCARIA
											WHERE
											COD_BANCO = '{this.FCCONBAN_COD_BANCO}'
											AND COD_AGENCIA = '{this.FCCONBAN_COD_AGENCIA}'
											AND COD_CONTA = '{this.FCCONBAN_COD_CONTA}'
											AND COD_OP_CONTA = '{this.FCCONBAN_COD_OP_CONTA}'
											AND COD_DV_CONTA = '{this.FCCONBAN_COD_DV_CONTA}'
											AND COD_TIPO_CONTA = 'LOT'";

            return query;
        }
        public string FCCONBAN_IDE_CONTA_BANCARIA { get; set; }
        public string FCCONBAN_COD_OP_CONTA { get; set; }
        public string FCCONBAN_COD_DV_CONTA { get; set; }
        public string FCCONBAN_COD_AGENCIA { get; set; }
        public string FCCONBAN_COD_BANCO { get; set; }
        public string FCCONBAN_COD_CONTA { get; set; }

        public static R6080_VER_CONTA_EXISTENTE_DB_SELECT_1_Query1 Execute(R6080_VER_CONTA_EXISTENTE_DB_SELECT_1_Query1 r6080_VER_CONTA_EXISTENTE_DB_SELECT_1_Query1)
        {
            var ths = r6080_VER_CONTA_EXISTENTE_DB_SELECT_1_Query1;
            ths.SetQuery(ths.GetQuery());

            ths.Open();
            var isFetch = ths.Fetch();

            return isFetch ? ths : null;
        }

        public override R6080_VER_CONTA_EXISTENTE_DB_SELECT_1_Query1 OpenData(List<KeyValuePair<string, object>> result)
        {
            var dta = new R6080_VER_CONTA_EXISTENTE_DB_SELECT_1_Query1();
            var i = 0;
            dta.FCCONBAN_IDE_CONTA_BANCARIA = result[i++].Value?.ToString();
            return dta;
        }

    }
}