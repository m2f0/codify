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
    public class R6230_INSERT_FC_CONTA_DB_INSERT_1_Insert1 : QueryBasis<R6230_INSERT_FC_CONTA_DB_INSERT_1_Insert1>
    {
        string GetQuery()
        {
            #region SQL_SOURCE
            /*EXEC SQL INSERT INTO FDRCAP.FC_CONTA_BANCARIA
            (IDE_CONTA_BANCARIA,
            COD_AGENCIA,
            COD_BANCO,
            COD_CONTA,
            COD_DV_CONTA,
            COD_OP_CONTA,
            COD_TIPO_CONTA,
            COD_EMPRESA)
            VALUES (:FCCONBAN-IDE-CONTA-BANCARIA,
            :FCCONBAN-COD-AGENCIA,
            :FCCONBAN-COD-BANCO,
            :FCCONBAN-COD-CONTA,
            :FCCONBAN-COD-DV-CONTA,
            :FCCONBAN-COD-OP-CONTA,
            'LOT' ,
            :FCCONBAN-COD-EMPRESA)
            END-EXEC.
            */
            #endregion
            var query = @$"
				INSERT INTO FDRCAP.FC_CONTA_BANCARIA (IDE_CONTA_BANCARIA, COD_AGENCIA, COD_BANCO, COD_CONTA, COD_DV_CONTA, COD_OP_CONTA, COD_TIPO_CONTA, COD_EMPRESA) VALUES ({FieldThreatment(this.FCCONBAN_IDE_CONTA_BANCARIA)}, {FieldThreatment(this.FCCONBAN_COD_AGENCIA)}, {FieldThreatment(this.FCCONBAN_COD_BANCO)}, {FieldThreatment(this.FCCONBAN_COD_CONTA)}, {FieldThreatment(this.FCCONBAN_COD_DV_CONTA)}, {FieldThreatment(this.FCCONBAN_COD_OP_CONTA)}, 'LOT' , {FieldThreatment(this.FCCONBAN_COD_EMPRESA)})";

            return query;
        }
        public string FCCONBAN_IDE_CONTA_BANCARIA { get; set; }
        public string FCCONBAN_COD_AGENCIA { get; set; }
        public string FCCONBAN_COD_BANCO { get; set; }
        public string FCCONBAN_COD_CONTA { get; set; }
        public string FCCONBAN_COD_DV_CONTA { get; set; }
        public string FCCONBAN_COD_OP_CONTA { get; set; }
        public string FCCONBAN_COD_EMPRESA { get; set; }

        public static void Execute(R6230_INSERT_FC_CONTA_DB_INSERT_1_Insert1 r6230_INSERT_FC_CONTA_DB_INSERT_1_Insert1)
        {
            var ths = r6230_INSERT_FC_CONTA_DB_INSERT_1_Insert1;
            ths.SetQuery(ths.GetQuery());
            ths.ExecuteQuery();
        }

        public override R6230_INSERT_FC_CONTA_DB_INSERT_1_Insert1 OpenData(List<KeyValuePair<string, object>> result)
        {
            throw new NotImplementedException();
        }

    }
}