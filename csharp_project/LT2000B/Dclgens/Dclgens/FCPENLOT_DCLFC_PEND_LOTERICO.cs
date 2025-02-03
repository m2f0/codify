using System;
using IA_ConverterCommons;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Linq;
using _ = IA_ConverterCommons.Statements;
using DB = IA_ConverterCommons.DatabaseBasis;

namespace Dclgens
{
    public class FCPENLOT_DCLFC_PEND_LOTERICO : VarBasis
    {
        /*" 10 FCPENLOT-NUM-LOTERICO  PIC S9(9) USAGE COMP.*/
        public IntBasis FCPENLOT_NUM_LOTERICO { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 FCPENLOT-NUM-NSA-LOTERICO  PIC S9(9) USAGE COMP.*/
        public IntBasis FCPENLOT_NUM_NSA_LOTERICO { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 FCPENLOT-NUM-NSR     PIC S9(9) USAGE COMP.*/
        public IntBasis FCPENLOT_NUM_NSR { get; set; } = new IntBasis(new PIC("S9", "9", "S9(9)"));
        /*" 10 FCPENLOT-COD-CGC     PIC X(15).*/
        public StringBasis FCPENLOT_COD_CGC { get; set; } = new StringBasis(new PIC("X", "15", "X(15)."), @"");
        /*" 10 FCPENLOT-COD-UF      PIC X(2).*/
        public StringBasis FCPENLOT_COD_UF { get; set; } = new StringBasis(new PIC("X", "2", "X(2)."), @"");
        /*" 10 FCPENLOT-DES-EMAIL   PIC X(50).*/
        public StringBasis FCPENLOT_DES_EMAIL { get; set; } = new StringBasis(new PIC("X", "50", "X(50)."), @"");
        /*" 10 FCPENLOT-DES-ENDERECO  PIC X(56).*/
        public StringBasis FCPENLOT_DES_ENDERECO { get; set; } = new StringBasis(new PIC("X", "56", "X(56)."), @"");
        /*" 10 FCPENLOT-DES-NUM-CEP  PIC X(8).*/
        public StringBasis FCPENLOT_DES_NUM_CEP { get; set; } = new StringBasis(new PIC("X", "8", "X(8)."), @"");
        /*" 10 FCPENLOT-NOM-BAIRRO  PIC X(30).*/
        public StringBasis FCPENLOT_NOM_BAIRRO { get; set; } = new StringBasis(new PIC("X", "30", "X(30)."), @"");
        /*" 10 FCPENLOT-NOM-MUNICIPIO  PIC X(30).*/
        public StringBasis FCPENLOT_NOM_MUNICIPIO { get; set; } = new StringBasis(new PIC("X", "30", "X(30)."), @"");
        /*" 10 FCPENLOT-NOM-RAZAO-SOCIAL  PIC X(40).*/
        public StringBasis FCPENLOT_NOM_RAZAO_SOCIAL { get; set; } = new StringBasis(new PIC("X", "40", "X(40)."), @"");
        /*" 10 FCPENLOT-NUM-AG-CAUCAO  PIC X(4).*/
        public StringBasis FCPENLOT_NUM_AG_CAUCAO { get; set; } = new StringBasis(new PIC("X", "4", "X(4)."), @"");
        /*" 10 FCPENLOT-NUM-AG-DESC-CPMF  PIC X(4).*/
        public StringBasis FCPENLOT_NUM_AG_DESC_CPMF { get; set; } = new StringBasis(new PIC("X", "4", "X(4)."), @"");
        /*" 10 FCPENLOT-NUM-AG-ISENTO-CPMF  PIC X(4).*/
        public StringBasis FCPENLOT_NUM_AG_ISENTO_CPMF { get; set; } = new StringBasis(new PIC("X", "4", "X(4)."), @"");
        /*" 10 FCPENLOT-NUM-BC-CAUCAO  PIC X(9).*/
        public StringBasis FCPENLOT_NUM_BC_CAUCAO { get; set; } = new StringBasis(new PIC("X", "9", "X(9)."), @"");
        /*" 10 FCPENLOT-NUM-BC-DESC-CPMF  PIC X(9).*/
        public StringBasis FCPENLOT_NUM_BC_DESC_CPMF { get; set; } = new StringBasis(new PIC("X", "9", "X(9)."), @"");
        /*" 10 FCPENLOT-NUM-BC-ISENTO-CPMF  PIC X(9).*/
        public StringBasis FCPENLOT_NUM_BC_ISENTO_CPMF { get; set; } = new StringBasis(new PIC("X", "9", "X(9)."), @"");
        /*" 10 FCPENLOT-NUM-CT-CAUCAO  PIC X(10).*/
        public StringBasis FCPENLOT_NUM_CT_CAUCAO { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*" 10 FCPENLOT-NUM-CT-DESC-CPMF  PIC X(10).*/
        public StringBasis FCPENLOT_NUM_CT_DESC_CPMF { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*" 10 FCPENLOT-NUM-CT-ISENTO-CPMF  PIC X(10).*/
        public StringBasis FCPENLOT_NUM_CT_ISENTO_CPMF { get; set; } = new StringBasis(new PIC("X", "10", "X(10)."), @"");
        /*" 10 FCPENLOT-NUM-DV-CAUCAO  PIC X(1).*/
        public StringBasis FCPENLOT_NUM_DV_CAUCAO { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 FCPENLOT-NUM-DV-DESC-CPMF  PIC X(1).*/
        public StringBasis FCPENLOT_NUM_DV_DESC_CPMF { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 FCPENLOT-NUM-DV-ISENTO-CPMF  PIC X(1).*/
        public StringBasis FCPENLOT_NUM_DV_ISENTO_CPMF { get; set; } = new StringBasis(new PIC("X", "1", "X(1)."), @"");
        /*" 10 FCPENLOT-NUM-OP-CAUCAO  PIC X(3).*/
        public StringBasis FCPENLOT_NUM_OP_CAUCAO { get; set; } = new StringBasis(new PIC("X", "3", "X(3)."), @"");
        /*" 10 FCPENLOT-NUM-OP-DESC-CPMF  PIC X(3).*/
        public StringBasis FCPENLOT_NUM_OP_DESC_CPMF { get; set; } = new StringBasis(new PIC("X", "3", "X(3)."), @"");
        /*" 10 FCPENLOT-NUM-OP-ISENTO-CPMF  PIC X(3).*/
        public StringBasis FCPENLOT_NUM_OP_ISENTO_CPMF { get; set; } = new StringBasis(new PIC("X", "3", "X(3)."), @"");
        /*" 10 FCPENLOT-NUM-TELEFONE  PIC X(16).*/
        public StringBasis FCPENLOT_NUM_TELEFONE { get; set; } = new StringBasis(new PIC("X", "16", "X(16)."), @"");
        /*"*/
    }
}