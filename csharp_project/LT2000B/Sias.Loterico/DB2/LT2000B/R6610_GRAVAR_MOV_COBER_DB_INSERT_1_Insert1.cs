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
    public class R6610_GRAVAR_MOV_COBER_DB_INSERT_1_Insert1 : QueryBasis<R6610_GRAVAR_MOV_COBER_DB_INSERT_1_Insert1>
    {
        string GetQuery()
        {
            #region SQL_SOURCE
            /*EXEC SQL INSERT INTO SEGUROS.LT_MOV_PROP_COBER
            (COD_PRODUTO,
            COD_EXT_ESTIP,
            COD_EXT_SEGURADO,
            DATA_MOVIMENTO,
            HORA_MOVIMENTO,
            COD_MOVIMENTO,
            COD_COBERTURA,
            VAL_IMP_SEGURADA,
            VAL_TAXA_PREMIO)
            VALUES (:LTMVPRCO-COD-PRODUTO,
            :LTMVPRCO-COD-EXT-ESTIP,
            :LTMVPRCO-COD-EXT-SEGURADO,
            :LTMVPRCO-DATA-MOVIMENTO,
            :LTMVPRCO-HORA-MOVIMENTO,
            :LTMVPRCO-COD-MOVIMENTO,
            :LTMVPRCO-COD-COBERTURA,
            :LTMVPRCO-VAL-IMP-SEGURADA,
            :LTMVPRCO-VAL-TAXA-PREMIO)
            END-EXEC.
            */
            #endregion
            var query = @$"
				INSERT INTO SEGUROS.LT_MOV_PROP_COBER (COD_PRODUTO, COD_EXT_ESTIP, COD_EXT_SEGURADO, DATA_MOVIMENTO, HORA_MOVIMENTO, COD_MOVIMENTO, COD_COBERTURA, VAL_IMP_SEGURADA, VAL_TAXA_PREMIO) VALUES ({FieldThreatment(this.LTMVPRCO_COD_PRODUTO)}, {FieldThreatment(this.LTMVPRCO_COD_EXT_ESTIP)}, {FieldThreatment(this.LTMVPRCO_COD_EXT_SEGURADO)}, {FieldThreatment(this.LTMVPRCO_DATA_MOVIMENTO)}, {FieldThreatment(this.LTMVPRCO_HORA_MOVIMENTO)}, {FieldThreatment(this.LTMVPRCO_COD_MOVIMENTO)}, {FieldThreatment(this.LTMVPRCO_COD_COBERTURA)}, {FieldThreatment(this.LTMVPRCO_VAL_IMP_SEGURADA)}, {FieldThreatment(this.LTMVPRCO_VAL_TAXA_PREMIO)})";

            return query;
        }
        public string LTMVPRCO_COD_PRODUTO { get; set; }
        public string LTMVPRCO_COD_EXT_ESTIP { get; set; }
        public string LTMVPRCO_COD_EXT_SEGURADO { get; set; }
        public string LTMVPRCO_DATA_MOVIMENTO { get; set; }
        public string LTMVPRCO_HORA_MOVIMENTO { get; set; }
        public string LTMVPRCO_COD_MOVIMENTO { get; set; }
        public string LTMVPRCO_COD_COBERTURA { get; set; }
        public string LTMVPRCO_VAL_IMP_SEGURADA { get; set; }
        public string LTMVPRCO_VAL_TAXA_PREMIO { get; set; }

        public static void Execute(R6610_GRAVAR_MOV_COBER_DB_INSERT_1_Insert1 r6610_GRAVAR_MOV_COBER_DB_INSERT_1_Insert1)
        {
            var ths = r6610_GRAVAR_MOV_COBER_DB_INSERT_1_Insert1;
            ths.SetQuery(ths.GetQuery());
            ths.ExecuteQuery();
        }

        public override R6610_GRAVAR_MOV_COBER_DB_INSERT_1_Insert1 OpenData(List<KeyValuePair<string, object>> result)
        {
            throw new NotImplementedException();
        }

    }
}