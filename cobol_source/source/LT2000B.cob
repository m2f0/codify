       IDENTIFICATION               DIVISION.                                   
      *--------------------------------------                                   
       PROGRAM-ID.                  LT2000B.                                    
      *----------------------------------------------------------------*        
      *   SISTEMA ................  SISTEMA DE LOTERICO                *        
      *   PROGRAMA ...............  LT2000B                            *        
      *----------------------------------------------------------------*        
      *   ANALISTA ...............                                     *        
      *   PROGRAMADOR ............                                     *        
      *   DATA CODIFICACAO .......  MAIO     / 2001                    *        
      *   VERSAO .................  02012009 17:00HS                   *        
      *----------------------------------------------------------------*        
      *   FUNCAO :   CRITICAR O ARQUIVO DE MOVIMENTACAO DO SIGEL/CEF   *        
      *              E ATUALIZAR O CADASTRO DE LOTERICOS(FC_LOTERICOS) *        
      *              E SE O LOTERICO FOR SEGURADO GERAR MOVIMENTO DE   *        
      *              ALTERACAO/EXCLUSAO NA TABELA LT_MOV_PROPOSTA PARA *        
      *              POSTERIOR ATUALIZACAO DOS CADASTROS DOS LOTERICOS *        
      *              SEGURADOS                                         *        
      *----------------------------------------------------------------*        
      *   ALTERADO PARA APOLICE:                                       *        
      *   APOLICE ANTIGA ----------: 0107100057625---------------------*        
      *   APOLICE NOVA   ----------:-0107100070673---------------------*        
      *----------------------------------------------------------------*        
      *   GERA ARQUIVO COM LOTERICOS QUE NAO QUEREM RENOVAR            *        
      *       CAD-RENOVAR  = 1 (NAO DESEJA RENOVAR - NO SIGEL)         *        
      *       SOMENTE DIA 24/07/2002                                   *        
      *----------------------------------------------------------------*        
      * EM 10/12/2002 - NAO CONSIDERA NUM-FAX = 99999999  - ALT-K1 ----*        
      *----------------------------------------------------------------*        
      *----------------------------------------------------------------*        
      * EM 18/01/2003 - ALTERACAO SOMENTE PARA TELEFONE,EMAIL E FAX    *        
      *                 NAO GERA CERTIFICADO.                          *        
      *                 VARIAVEL W-IND-ALT-FAXTELEME                   *        
      *                 ALTERA DDD PARA -1. (PROG LT2002B TRATA).      *        
      * PROCURAR: OL1801                                               *        
      *----------------------------------------------------------------*        
      * EM 08/05/2003 - ALTERACAO SOMENTE PARA FAX IGUAL 9999999       *        
      *                 NAO GERA CERTIFICADO.                          *        
      *                 DDD E EMAIL CONTINUA GERANDO CERTIFICADO       *        
      *                 VARIAVEL W-IND-ALT-FAXTELEME                   *        
      *----------------------------------------------------------------*        
      * EM 09/06/2003 - INCLUIDO TOTALIZADORES DAS INFORMACOES LIDAS   *        
      *                 NO SIGEL.                                      *        
      * PROCURAR: OL0906                                               *        
      *----------------------------------------------------------------*        
      * EM 26/02/2005 - RETIRADO EXCLUSAO DA TABELA  FC_PEND_LOTERICO  *        
      *             ROTINA = R6750-DELETE-FC-PEND-LOTERICO             *        
      *----------------------------------------------------------------*        
      * EM 10/11/2006 - RETIRADO ALTERACAO DE IND_UNIDADE SUB          *        
      *             DA FC_LOTERICO ( CONTERA O CODIGO DO SOCIO         *        
      *                              NA GE-PESSOA-SOCIO        )       *        
      *----------------------------------------------------------------*        
      * EM 21/03/2015 - ALTERACAO DA CAPITALIZACAO NO SISTEMA LOTERICO *        
      *                 E CCA, EM FUNCAO DA ALTERACAO DAS CARACTERISTI-*        
      *                 CAS DOS CAMPOS DE CONTA CORRENTE NA TABELA     *        
      *                 FC_CONTA_BANCARIA                              *        
      *                                                                *        
      * COREON                                       PROCURE POR: V.01 *        
      *----------------------------------------------------------------*        
      *                                                                *        
      *   VERSAO 02 -   NSGD - CADMUS 103659.                          *        
      *               - NOVA SOLUCAO DE GESTAO DE DEPOSITOS            *        
      *                                                                *        
      *   EM 06/07/2015 - COREON                                       *        
      *                                                                *        
      *                                       PROCURE POR V.02         *        
      *                                                                *        
      *----------------------------------------------------------------*        
      *                                                                *        
      *   VERSAO V.03 - ABEND - CADMUS 177676                          *        
      *                 INSERT FC-CONTA-BANCARIA - 803                 *        
      *                                                                *        
      *   EM 04/10/2019 - OLIVEIRA                                     *        
      *                                                                *        
      *            PROCURE POR V.03                                    *        
      *                                                                *        
      *----------------------------------------------------------------*        
       ENVIRONMENT                  DIVISION.                                   
      *--------------------------------------                                   
       CONFIGURATION                SECTION.                                    
      *--------------------------------------                                   
       SPECIAL-NAMES.                                                           
      *                                                                         
           DECIMAL-POINT      IS    COMMA.                                      
      *----------------------------------------------------------------*        
       INPUT-OUTPUT                  SECTION.                                   
      *-------------------------------------                                    
       FILE-CONTROL.                                                            
      *                                                                         
           SELECT      CADASTRO                                                 
                       ASSIGN      TO    MOV2000B.                              
      *                                                                         
           SELECT      RLT2000B                                                 
                       ASSIGN      TO    RLT2000B.                              
      *                                                                         
      *----------------------------------------------------------------*        
       DATA DIVISION.                                                           
      *--------------                                                           
       FILE SECTION.                                                            
      *-------------                                                            
      *                                                                         
      *  ARQUIVO SIGEL                                                          
      *                                                                         
       FD  CADASTRO                                                             
           RECORD    700                                                        
           BLOCK       0                                                        
           RECORDING MODE IS F                                                  
           LABEL RECORD IS OMITTED.                                             
      *                                                                         
       01  REG-CAD            PIC X(700).                                       
                                                                                
     *                                                                          
       FD  RLT2000B                                                             
           RECORDING MODE  F                                                    
           LABEL RECORD IS OMITTED.                                             
      *                                                                         
       01  REG-RLT2000B.                                                        
           05 REG-LINHA                     PIC X(132).                         
      *                                                                         
      *----------------------------------------------------------------*        
       WORKING-STORAGE              SECTION.                                    
                                                                                
           EXEC SQL BEGIN DECLARE SECTION END-EXEC.                             
                                                                                
      *----------------------------------------------------------------*        
      *                 DEFINICAO DAS VARIAVEIS HOST                   *        
      *----------------------------------------------------------------*        
      *--* TABELA DE ENDOSSO (V0ENDOSSO)                                        
      *---------------------------------                                        
      *                                                                         
       77  V0ENDO-DTINIVIG             PIC X(10).                               
       77  V0ENDO-DTTERVIG             PIC X(10).                               
      *                                                                         
      *--* TABELA DE SISTEMAS (V1SISTEMA)                                       
      *---------------------------------                                        
      *                                                                         
       77  V0SIST-DTMOVABE             PIC X(10).                               
       77  V0SIST-TIMESTAMP            PIC X(26).                               
      *                                                                         
      *--* VARIAVEIS AUXILIARES                                                 
      *---------------------------------                                        
      *                                                                         
       77         WS-OBRIGATORIO      PIC  9(001) VALUE 0.                      
       77         WS-NECESSARIO       PIC  9(001) VALUE 0.                      
       77         WS-TEM-UF           PIC  9(001) VALUE 0.                      
       77         WS-IMPRIMIU         PIC  9(001) VALUE 0.                      
       77         WS-IND              PIC  9(003) VALUE 0.                      
       77         WS-IDE-CONTA-CPMF   PIC S9(009) VALUE +0  COMP.               
       77         WS-IDE-CONTA-ISENTA PIC S9(009) VALUE +0  COMP.               
       77         WS-IDE-CONTA-CAUCAO PIC S9(009) VALUE +0  COMP.               
       77         MAX-IDE-CONTA-BANCARIA PIC S9(009) VALUE +0  COMP.            
       77         W-CHAVE-CADASTRADO-SASSE  PIC X(03) VALUE SPACES.             
       77         W-CHAVE-CADASTRADO-SIGEL  PIC X(03) VALUE SPACES.             
       77         W-CHAVE-HOUVE-ALTERACAO   PIC X(03) VALUE SPACES.             
       77         W-CHAVE-ALTEROU-SEGURADO  PIC X(03) VALUE SPACES.             
       77         W-IND-ALT-FAXTELEME       PIC X(01) VALUE SPACES.             
       77         WS-CKT                    PIC 9(01).                          
       77         WS-COFRE                  PIC 9(01).                          
       77         WS-ALARME                 PIC 9(01).                          
      *                                                                         
       77         VIND-DTTERVIG             PIC S9(004) VALUE +0  COMP.         
       77         VIND-DTINIVIG             PIC S9(004) VALUE +0  COMP.         
                                                                                
      * VARIAVEIS INDICADORAS                                                   
                                                                                
       77            VIND-AGENTE-MASTER    PIC S9(004) VALUE +0  COMP.          
       77            VIND-COD-CGC          PIC S9(004) VALUE +0  COMP.          
       77            VIND-COD-INSCR-ESTAD  PIC S9(004) VALUE +0  COMP.          
       77            VIND-COD-INSCR-MUNIC  PIC S9(004) VALUE +0  COMP.          
       77            VIND-COD-MUNICIPIO    PIC S9(004) VALUE +0  COMP.          
       77            VIND-COD-UF           PIC S9(004) VALUE +0  COMP.          
       77            VIND-DES-EMAIL        PIC S9(004) VALUE +0  COMP.          
       77            VIND-DES-ENDERECO     PIC S9(004) VALUE +0  COMP.          
       77            VIND-DTH-EXCLUSAO     PIC S9(004) VALUE +0  COMP.          
       77            VIND-DTH-INCLUSAO     PIC S9(004) VALUE +0  COMP.          
       77            VIND-IDE-CONTA-CAUCAO PIC S9(004) VALUE +0  COMP.          
       77            VIND-IDE-CONTA-CPMF   PIC S9(004) VALUE +0  COMP.          
       77            VIND-IDE-CONTA-ISENTA PIC S9(004) VALUE +0  COMP.          
       77            VIND-IND-CAT-LOTERICO PIC S9(004) VALUE +0  COMP.          
       77            VIND-IND-STA-LOTERICO PIC S9(004) VALUE +0  COMP.          
       77            VIND-IND-UNIDADE-SUB  PIC S9(004) VALUE +0  COMP.          
       77            VIND-NOM-BAIRRO       PIC S9(004) VALUE +0  COMP.          
       77            VIND-NOM-CONSULTOR    PIC S9(004) VALUE +0  COMP.          
       77            VIND-NOM-CONTATO1     PIC S9(004) VALUE +0  COMP.          
       77            VIND-NOM-CONTATO2     PIC S9(004) VALUE +0  COMP.          
       77            VIND-NOM-FANTASIA     PIC S9(004) VALUE +0  COMP.          
       77            VIND-NOM-MUNICIPIO    PIC S9(004) VALUE +0  COMP.          
       77            VIND-NOM-RAZAO-SOCIAL PIC S9(004) VALUE +0  COMP.          
       77            VIND-NUM-CEP          PIC S9(004) VALUE +0  COMP.          
       77            VIND-NUM-ENCEF        PIC S9(004) VALUE +0  COMP.          
       77            VIND-NUM-LOTER-ANT    PIC S9(004) VALUE +0  COMP.          
       77            VIND-NUM-MATR-CON     PIC S9(004) VALUE +0  COMP.          
       77            VIND-NUM-PVCEF        PIC S9(004) VALUE +0  COMP.          
       77            VIND-NUM-TELEFONE     PIC S9(004) VALUE +0  COMP.          
       77            VIND-STA-DADOS-N      PIC S9(004) VALUE +0  COMP.          
       77            VIND-STA-LOTERICO     PIC S9(004) VALUE +0  COMP.          
       77            VIND-STA-NIVEL-COMIS  PIC S9(004) VALUE +0  COMP.          
       77            VIND-STA-ULT-ALT      PIC S9(004) VALUE +0  COMP.          
       77            VIND-COD-DETALHE      PIC S9(004) VALUE +0  COMP.          
       77            VIND-DES-MSG-DESTINO  PIC S9(004) VALUE +0  COMP.          
       77            VIND-DES-MSG-ORIGEM   PIC S9(004) VALUE +0  COMP.          
       77            VIND-COD-GARANTIA     PIC S9(004) VALUE +0  COMP.          
       77            VIND-VLR-GARANTIA     PIC S9(004) VALUE +0  COMP.          
       77            VIND-DTH-GERACAO      PIC S9(004) VALUE +0  COMP.          
       77            VIND-DATA-CANCELAMENTO PIC S9(004) VALUE +0  COMP.         
       77            VIND-TIPO-ALTERACAO    PIC S9(004) VALUE +0  COMP.         
       77            VIND-DATA-ALTERACAO    PIC S9(004) VALUE +0  COMP.         
       77            VIND-COD-AGENTE-MASTER PIC S9(004) VALUE +0  COMP.         
       77            VIND-NUM-TITULO        PIC S9(004) VALUE +0  COMP.         
       77            VIND-DATA-QUITACAO     PIC S9(004) VALUE +0  COMP.         
       77            VIND-BONUS-CKT         PIC S9(004) VALUE +0  COMP.         
       77            VIND-BONUS-COFRE       PIC S9(004) VALUE +0  COMP.         
       77            VIND-BONUS-ALARME      PIC S9(004) VALUE +0  COMP.         
       77            VIND-NOME-SEGURADORA   PIC S9(004) VALUE +0  COMP.         
       77            VIND-TIPO-GARANTIA     PIC S9(004) VALUE +0  COMP.         
       77            VIND-VALOR-GARANTIA    PIC S9(004) VALUE +0  COMP.         
       77            VIND-NUM-FAX           PIC S9(004) VALUE +0  COMP.         
                                                                                
       77 V0SOL-COD-PRODUTO            PIC S9(004)    VALUE +0 COMP.            
       77 V0SOL-COD-CLIENTE            PIC S9(009)    VALUE +0 COMP.            
       77 V0SOL-COD-PROGRAMA           PIC X(12).                               
       77 V0SOL-TIPO-SOLICITACAO       PIC S9(004)    VALUE +0 COMP.            
       77 V0SOL-COD-MOVIMENTO          PIC S9(004)    VALUE +0 COMP.            
       77 V0SOL-DATA-SOLICITACAO       PIC X(10).                               
       77 V0SOL-COD-USUARIO            PIC X(08).                               
       77 V0SOL-DATA-PREV-PROC         PIC X(10).                               
       77 V0SOL-SIT-SOLICITACAO        PIC X(01).                               
       77 V0SOL-PARAM-DATE01           PIC X(10).                               
       77 V0SOL-PARAM-DATE02           PIC X(10).                               
       77 V0SOL-PARAM-DATE03           PIC X(10).                               
       77 V0SOL-PARAM-SMINT01          PIC S9(004)    VALUE +0 COMP.            
       77 V0SOL-PARAM-SMINT02          PIC S9(004)    VALUE +0 COMP.            
       77 V0SOL-PARAM-SMINT03          PIC S9(004)    VALUE +0 COMP.            
       77 V0SOL-PARAM-INTG01           PIC S9(009)    VALUE +0 COMP.            
       77 V0SOL-PARAM-INTG02           PIC S9(009)    VALUE +0 COMP.            
       77 V0SOL-PARAM-INTG03           PIC S9(009)    VALUE +0 COMP.            
       77 V0SOL-PARAM-DEC01            PIC S9(017)    VALUE +0 COMP-3.          
       77 V0SOL-PARAM-DEC02            PIC S9(017)    VALUE +0 COMP-3.          
       77 V0SOL-PARAM-DEC03            PIC S9(017)    VALUE +0 COMP-3.          
       77 V0SOL-PARAM-FLOAT01          PIC S9(008)    VALUE +0 COMP-3.          
       77 V0SOL-PARAM-FLOAT02          PIC S9(008)    VALUE +0 COMP-3.          
       77 V0SOL-PARAM-CHAR01           PIC X(60).                               
       77 V0SOL-PARAM-CHAR02           PIC X(30).                               
       77 V0SOL-PARAM-CHAR03           PIC X(15).                               
       77 V0SOL-PARAM-CHAR04           PIC X(15).                               
      *                                                                         
      *--* TABELA DE EMPRESAS (V1EMPRESA)                                       
      *---------------------------------                                        
      *                                                                         
       77         V1EMPR-NOM-EMP      PIC  X(040).                              
      *                                                                         
      *--* TABELA LOTERICOS (V0LOTERICO01)                                      
      *-----------------------------------                                      
      *                                                                         
       77  V0LOT-NUM-APOLICE           PIC S9(013) VALUE +0 COMP-3.             
       77  V0LOT-COD-LOT-FENAL         PIC S9(009) VALUE +0 COMP.               
       77  V0LOT-COD-LOT-CEF           PIC S9(009) VALUE +0 COMP.               
      *                                                                         
       77   WS-NUM-APOLICE             PIC S9(013) VALUE +0 COMP-3.             
       77   WS-CONTADOR                PIC S9(009) VALUE +0 COMP.               
      *                                                                         
       77  HOST-DATA-INI-1P                 PIC X(10).                          
       77  HOST-DATA-FIM-1P                 PIC X(10).                          
      *                                                                         
      *----------------------------------------------------------------*        
           EXEC SQL END DECLARE SECTION END-EXEC.                               
      *----------------------------------------------------------------*        
           EXEC SQL INCLUDE FCSEQUEN   END-EXEC.                                
           EXEC SQL INCLUDE FCLOTERI   END-EXEC.                                
           EXEC SQL INCLUDE FCPENLOT   END-EXEC.                                
           EXEC SQL INCLUDE FCTPENLT   END-EXEC.                                
           EXEC SQL INCLUDE FCCONBAN   END-EXEC.                                
           EXEC SQL INCLUDE FCHISLOT   END-EXEC.                                
           EXEC SQL INCLUDE LTLOTBON   END-EXEC.                                
           EXEC SQL INCLUDE LTMVPROP   END-EXEC.                                
           EXEC SQL INCLUDE LTMVPRBO   END-EXEC.                                
           EXEC SQL INCLUDE LTMVPRCO   END-EXEC.                                
           EXEC SQL INCLUDE LTSOLPAR   END-EXEC.                                
           EXEC SQL INCLUDE SQLCA      END-EXEC.                                
      *----------------------------------------------------------------*        
      *                     CAMPOS   DE   TRABALHO                     *        
      *----------------------------------------------------------------*        
      *                                                                         
       01           AREA-DE-WORK.                                               
      *                                                                         
           05 LK-LINK.                                                          
              10 LK-RTCODE                  PIC S9(004) VALUE +0   COMP.        
              10 LK-TAMANHO                 PIC S9(004) VALUE +40  COMP.        
              10 LK-TITULO                  PIC  X(132) VALUE  SPACES.          
      *                                                                         
      *--* CAMPOS PARA RETORNO DE CODIGOS DE ERRO                               
      *                                                                         
           05 WSL-SQLCODE                   PIC 9(09) VALUE ZEROS.              
      *                                                                         
      *--* CAMPOS PARA CALCULO                                                  
      *                                                                         
           05 W-AC-LINHA                    PIC 9(03) VALUE  80.                
           05 W-AC-PAGINA                   PIC 9(04) VALUE  ZEROS.             
      *                                                                         
           05         WS-RESTO              PIC 9(05) VALUE ZEROS.              
           05         WS-AUX                PIC 9(05) VALUE ZEROS.              
      *                                                                         
      *--* CAMPOS PARA TRATAMENTO DE ARQUIVOS/TABELAS                           
      *                                                                         
           05 WFIM-CADASTRO                 PIC X(03) VALUE SPACES.             
           05 WFIM-MOVIMENTO                PIC X(03) VALUE SPACES.             
           05 WFIM-RELATORIOS               PIC X(03) VALUE SPACES.             
           05 WFIM-PARAM                    PIC X(03) VALUE SPACES.             
      *                                                                         
      *--* CHAVES AUXILIARES                                                    
      *                                                                         
           05 W-CHAVE-TEM-CRITICA           PIC X(03) VALUE SPACES.             
      *                                                                         
           05 WS-NOME-BAIRRO                PIC X(30).                          
           05 WS-NOME-BAIRRO-R REDEFINES WS-NOME-BAIRRO.                        
              10 WS-BAIRRO                  PIC X(20).                          
              10 FILLER                     PIC X(10).                          
      *                                                                         
           05 WS-NOME-CIDADE                PIC X(30).                          
           05 WS-NOME-CIDADE-R REDEFINES WS-NOME-CIDADE.                        
              10 WS-CIDADE                  PIC X(25).                          
              10 FILLER                     PIC X(05).                          
      *                                                                         
      *--* CAMPO DE CONTROLE DE QUEBRA                                          
      *                                                                         
           05 W-MOV-COD-FENAL               PIC 9(07) VALUE  ZEROS.             
      *                                                                         
           05  REG-CAD-HEADER.                                                  
             15 CAD-TIPO-HEADERX              PIC X(001).                       
             15 CAD-COD-REMESSAX.                                               
                20 CAD-REMESSA                PIC 9(001).                       
             15 CAD-EMPRESA                   PIC X(020).                       
             15 CAD-DATA-GERACAOX.                                              
                20 CAD-DATA-GERACAO           PIC 9(008).                       
             15 CAD-NUM-SEQX.                                                   
                20 CAD-NUM-SEQ                PIC 9(006).                       
             15 CAD-VERSAOX.                                                    
                20 CAD-VERSAO                 PIC 9(002).                       
             20 CAD-ESPACO                    PIC X(656).                       
             15 CAD-SEQ-REGX.                                                   
                20 CAD-SEQ-REG                PIC 9(006).                       
      *                                                                         
           05  REG-CAD-TRAILLER.                                                
             15 CAD-TIPO-TRAILLERX            PIC X(001).                       
             15 CAD-TOTALX.                                                     
                20 CAD-TOTAL                  PIC 9(006).                       
             15 FILLER                        PIC X(687).                       
             15 CAD-NSR-TRAX.                                                   
                20 CAD-NSR-TRA                PIC 9(006).                       
      *                                                                         
           05  REG-CAD-CADASTRO.                                                
             15 CAD-TIPOX.                                                      
                20 CAD-TIPO                   PIC X(01).                        
             15 CAD-COD-CEFX.                                                   
                20 CAD-CODIGO-CEF             PIC 9(09).                        
                20 CAD-CODIGO-CEF-R REDEFINES CAD-CODIGO-CEF.                   
                   25 CAD-COD-CEF             PIC 9(08).                        
                   25 CAD-DV-CEF              PIC 9(01).                        
             15 CAD-RAZAO-SOCIAL              PIC X(40).                        
             15 CAD-NOME-FANTASIA             PIC X(30).                        
             15 CAD-ENDERECO.                                                   
                20 CAD-END                    PIC X(40).                        
                20 CAD-COMPL-END              PIC X(16).                        
             15 CAD-BAIRRO                    PIC X(20).                        
             15 CAD-COD-MUNICIPIO             PIC X(12).                        
             15 CAD-CIDADE                    PIC X(25).                        
             15 CAD-CEPX.                                                       
                20 CAD-CEP                    PIC 9(08).                        
                20 FILLER                     PIC X(01).                        
             15 CAD-UF                        PIC X(02).                        
             15 CAD-TELEFONE.                                                   
                20 CAD-DDD-FONE               PIC 9(04).                        
                20 CAD-FONE                   PIC 9(08).                        
             15 FILLER                        PIC X(04).                        
             15 CAD-CONTATO1                  PIC X(20).                        
             15 CAD-CONTATO2                  PIC X(20).                        
             15 CAD-CGCX.                                                       
                20 CAD-CGC                    PIC 9(15).                        
             15 CAD-INSC-MUNX.                                                  
                20 CAD-INSC-MUN               PIC 9(15).                        
             15 CAD-INSC-ESTX.                                                  
                20 CAD-INSC-EST               PIC 9(15).                        
             15 CAD-SITUACAOX.                                                  
                20 CAD-SITUACAO               PIC 9(01).                        
             15 CAD-DATA-INCLUSAOX.                                             
                20 CAD-DATA-INCLUSAO          PIC 9(08).                        
             15 CAD-DATA-EXCLUSAOX.                                             
                20 CAD-DATA-EXCLUSAO          PIC 9(08).                        
             15 CAD-NUM-LOT-ANTERIORX.                                          
                20 CAD-NUM-LOT-ANTERIOR       PIC 9(10).                        
             15 CAD-COD-AG-MASTERX.                                             
                20 CAD-COD-AG-MASTER          PIC 9(09).                        
             15 CAD-CAT-LOTERICOX.                                              
                20 CAD-CAT-LOTERICO           PIC 9(02).                        
             15 CAD-COD-STATUSX.                                                
                20 CAD-COD-STATUS             PIC 9(02).                        
             15 CAD-BANCO-DESC-CPMFX.                                           
                20 CAD-BANCO-DESC-CPMF        PIC 9(09).                        
             15 CAD-AGEN-DESC-CPMFX.                                            
                20 CAD-AGEN-DESC-CPMF         PIC 9(05).                        
             15 CAD-CONTA-DESC-CPMFX.                                           
V.02  *         20 CAD-CONTA-DESC-CPMF        PIC 9(12).                        
V.02            20 CAD-CONTA-DESC-CPMF        PIC 9(17).                        
             15 CAD-BANCO-ISENTAX.                                              
                20 CAD-BANCO-ISENTA             PIC 9(09).                      
             15 CAD-AGEN-ISENTAX.                                               
                20 CAD-AGEN-ISENTA              PIC 9(05).                      
             15 CAD-CONTA-ISENTAX.                                              
V.02  *         20 CAD-CONTA-ISENTA             PIC 9(12).                      
V.02            20 CAD-CONTA-ISENTA             PIC 9(17).                      
             15 CAD-BANCO-CAUCAOX.                                              
                20 CAD-BANCO-CAUCAO             PIC 9(09).                      
             15 CAD-AGEN-CAUCAOX.                                               
                20 CAD-AGEN-CAUCAO              PIC 9(05).                      
             15 CAD-CONTA-CAUCAOX.                                              
V.02  *         20 CAD-CONTA-CAUCAO             PIC 9(12).                      
V.02            20 CAD-CONTA-CAUCAO             PIC 9(17).                      
             15 CAD-NIVEL-COMISSAOX.                                            
                20 CAD-NIVEL-COMISSAO           PIC 9(01).                      
             15 CAD-PV-SUBX.                                                    
                20 CAD-PV-SUB                   PIC 9(04).                      
             15 CAD-EN-SUBX.                                                    
                20 CAD-EN-SUB                   PIC 9(04).                      
             15 CAD-UNIDADE-SUBX.                                               
                20 CAD-UNIDADE-SUB              PIC 9(04).                      
             15 CAD-MATR-CONSULTORX.                                            
                20 CAD-MATR-CONSULTOR           PIC 9(07).                      
             15 CAD-NOME-CONSULTORX.                                            
                20 CAD-NOME-CONSULTOR           PIC X(40).                      
             15 CAD-EMAILX.                                                     
                20 CAD-EMAIL                    PIC X(50).                      
             15 CAD-TIPO-GARANTIAX.                                             
                20 CAD-TIPO-GARANTIA            PIC X(01).                      
             15 CAD-VALOR-GARANTIAX.                                            
                20 CAD-VALOR-GARANTIA           PIC 9(07)V99.                   
             15 CAD-NUM-SEGURADORAX.                                            
                20 CAD-NUM-SEGURADORA           PIC 9(03).                      
             15 CAD-BONUS-CKTX.                                                 
                20 CAD-BONUS-CKT                PIC 9(01).                      
             15 CAD-BONUS-ALARMEX.                                              
                20 CAD-BONUS-ALARME             PIC 9(01).                      
             15 CAD-BONUS-COFREX.                                               
                20 CAD-BONUS-COFRE              PIC 9(01).                      
             15 CAD-NUMERO-FAX.                                                 
                20 CAD-DDD-FAX                  PIC 9(04).                      
                20 CAD-FAX                      PIC 9(08).                      
             15 CAD-RENOVAR                     PIC 9(01).                      
V.02  *      15 FILLER                          PIC X(132).                     
V.02         15 FILLER                          PIC X(117).                     
             15 CAD-NSRX.                                                       
                20 CAD-NSR                      PIC 9(06).                      
                                                                                
      *                                                                         
      *--* MASCARAS DE EDICAO                                                   
      *                                                                         
           05 W-CGC.                                                            
              10 W-CGC-8               PIC X(08) VALUE SPACES.                  
              10 W-CGC-4               PIC X(04) VALUE SPACES.                  
              10 W-CGC-2               PIC X(02) VALUE SPACES.                  
           05 W-CGC-ED.                                                         
              10 W-CGC-8-ED            PIC X(08) VALUE SPACES.                  
              10 FILLER                PIC X(01) VALUE '/'.                     
              10 W-CGC-4-ED            PIC X(04) VALUE SPACES.                  
              10 FILLER                PIC X(01) VALUE '-'.                     
              10 W-CGC-2-ED            PIC X(02) VALUE SPACES.                  
      *                                                                         
           05 W-AC-CAD-LIDOS                PIC 9(07) VALUE ZEROS.              
           05 W-AC-MOV-LIDOS                PIC 9(07) VALUE ZEROS.              
           05 W-AC-LOTERICOS-GRAVADOS       PIC 9(07) VALUE ZEROS.              
           05 W-AC-LOTERICOS-REJEITADOS     PIC 9(07) VALUE ZEROS.              
           05 W-AC-LOTERICOS-BLACKLIST      PIC 9(07) VALUE ZEROS.              
           05 W-AC-MOV-LOTERICOS-BAIXADOS   PIC 9(07) VALUE ZEROS.              
           05 W-AC-MOV-LOTERICOS-REJEITADOS PIC 9(07) VALUE ZEROS.              
      *                                                                         
OL0906     05 W-AC-ATIVOS                   PIC 9(07) VALUE ZEROS.              
OL0906     05 W-AC-INATIVOS                 PIC 9(07) VALUE ZEROS.              
OL0906     05 W-AC-PRECAD                   PIC 9(07) VALUE ZEROS.              
OL0906     05 W-AC-SEGURADORA-CS            PIC 9(07) VALUE ZEROS.              
OL0906     05 W-AC-SEGURADORA-OUTRAS        PIC 9(07) VALUE ZEROS.              
OL0906     05 W-AC-GARANTIA-S               PIC 9(07) VALUE ZEROS.              
OL0906     05 W-AC-GARANTIA-C               PIC 9(07) VALUE ZEROS.              
      *                                                                         
           05 WS-FCPEND-DELETADOS           PIC 9(07) VALUE ZEROS.              
           05 WS-FCLOT-TOTAL-INCLUIDOS      PIC 9(07) VALUE ZEROS.              
           05 WS-FCLOT-TOTAL-ALTERADOS      PIC 9(07) VALUE ZEROS.              
      *                                                                         
           05 WS-MVPROP-TOTAL-SEGURADOS     PIC 9(07) VALUE ZEROS.              
           05 WS-MVPROP-TOTAL-NSEGURADOS    PIC 9(07) VALUE ZEROS.              
           05 WS-MVPROP-TOTAL-INCLUIDOS     PIC 9(07) VALUE ZEROS.              
           05 WS-MVPROP-TOTAL-ALTERADOS     PIC 9(07) VALUE ZEROS.              
           05 WS-MVPROP-TOTAL-CANCELADOS    PIC 9(07) VALUE ZEROS.              
      *                                                                         
           05 WS-ERRO-DATA                  PIC S9(07) VALUE  ZEROS.            
      *                                                                         
      *                                                                         
V.02  *  05         WS-CONTA-BANCARIA PIC 9(12).                                
V.02     05         WS-CONTA-BANCARIA PIC 9(17).                                
         05         WS-CONTA-BANCARIA-R REDEFINES WS-CONTA-BANCARIA.            
V.02  *    10       WS-OPERACAO-CONTA PIC  X(003).                              
V.02       10       WS-OPERACAO-CONTA PIC  X(004).                              
V.02  *    10       WS-NUMERO-CONTA   PIC  9(008).                              
V.02       10       WS-NUMERO-CONTA   PIC  9(012).                              
           10       WS-DV-CONTA       PIC  X(001).                              
V.01  *                                                                         
V.02  *  05         WS-CONTA-BANCO    PIC 9(012).                               
V.02     05         WS-CONTA-BANCO    PIC 9(017).                               
         05         WS-CONTA-BANCO-R  REDEFINES   WS-CONTA-BANCO.               
V.02  *    10       WS-OPERA-CONTA    PIC 9(003).                               
V.02       10       WS-OPERA-CONTA    PIC 9(004).                               
V.02  *    10       WS-FILLER         PIC 9(009).                               
V.02       10       WS-FILLER         PIC 9(013).                               
      *                                                                         
V.02  *  05         WS-CONTA-10POS    PIC 9(10).                                
V.02     05         WS-CONTA-12POS    PIC 9(12).                                
      *                                                                         
         05         WIND1             PIC 9(02).                                
         05         WIND2             PIC 9(02).                                
         05         WTAM-TEL          PIC 9(02).                                
         05         WS-NUM-TELEF-ENT  PIC X(12).                                
         05         FILLER            REDEFINES WS-NUM-TELEF-ENT.               
           10       WS-DDD-ENT        PIC  9(005).                              
           10       WS-TELEFONE-ENT   PIC  9(007).                              
         05         WS-NUM-TELEF-SAI  PIC X(12).                                
         05         FILLER            REDEFINES WS-NUM-TELEF-SAI.               
           10       WS-DDD-SAI        PIC  9(004).                              
           10       WS-TELEFONE-SAI   PIC  9(008).                              
      *                                                                         
         05         WS-NUMERO-FAX     PIC X(16).                                
         05         WS-NUMERO-FAX-R   REDEFINES WS-NUMERO-FAX.                  
           10       WS-NUMER-FAX.                                               
             20       WS-DDD-FAX        PIC  9(004).                            
             20       WS-NUM-FAX        PIC  9(008).                            
           10       FILLER            PIC  X(004).                              
      *                                                                         
      *                                                                         
         05         WS-CODIGO-BANCO   PIC 9(09).                                
         05         WS-CODIGO-BANCO-R REDEFINES WS-CODIGO-BANCO.                
           10       FILLER            PIC  X(006).                              
           10       WS-COD-BANCO      PIC  X(003).                              
                                                                                
      ****************************************************************          
                                                                                
V.02  *  05         WS1-CONTA-BANCARIA PIC 9(12).                               
V.02     05         WS1-CONTA-BANCARIA PIC 9(17).                               
         05         WS1-CONTA-BANCARIA-R REDEFINES WS1-CONTA-BANCARIA.          
V.02  *    10       WS1-OPERACAO-CONTA PIC  9(003).                             
V.02       10       WS1-OPERACAO-CONTA PIC  9(004).                             
V.02  *    10       WS1-NUMERO-CONTA   PIC  9(008).                             
V.02       10       WS1-NUMERO-CONTA   PIC  9(012).                             
           10       WS1-DV-CONTA       PIC  9(001).                             
      *                                                                         
V.01  *  05         WS-CONBAN-BANCO   PIC X(03).                                
V.02  *  05         WS-CONBAN-BANCO   PIC 9(03).                                
V.02     05         WS-CONBAN-BANCO   PIC 9(04).                                
V.01  *  05         WS-CONBAN-BANCO-N REDEFINES WS-CONBAN-BANCO                 
V.01  *                               PIC 9(03).                                
                                                                                
V.01  *  05         WS-CONBAN-AGENCIA PIC X(04).                                
V.01     05         WS-CONBAN-AGENCIA PIC 9(04).                                
V.01  *  05         WS-CONBAN-AGENCIA-N REDEFINES WS-CONBAN-AGENCIA             
V.01  *                               PIC 9(04).                                
                                                                                
V.01  *  05         WS-CONBAN-OP      PIC X(03).                                
V.02  *  05         WS-CONBAN-OP      PIC 9(03).                                
V.02     05         WS-CONBAN-OP      PIC 9(04).                                
V.01  *  05         WS-CONBAN-OP-N    REDEFINES WS-CONBAN-OP                    
V.01  *                               PIC 9(03).                                
                                                                                
V.01  *  05         WS-CONBAN-CONTA   PIC X(10).                                
V.02  *  05         WS-CONBAN-CONTA   PIC 9(10).                                
V.02     05         WS-CONBAN-CONTA   PIC 9(12).                                
V.01  *  05         WS-CONBAN-CONTA-N REDEFINES WS-CONBAN-CONTA                 
V.01  *                               PIC 9(10).                                
                                                                                
V.01     05         WS-CONBAN-DV      PIC X(01).                                
V.01  *  05         WS-CONBAN-DV      PIC 9(01).                                
V.01  *  05         WS-CONBAN-DV-N    REDEFINES WS-CONBAN-DV                    
V.01  *                               PIC 9(01).                                
                                                                                
************************************************************************        
                                                                                
                                                                                
                                                                                
         05         WS-CODIGO-AGENCIA PIC 9(05).                                
         05         WS-CODIGO-AGENCIA-R REDEFINES WS-CODIGO-AGENCIA.            
           10       FILLER            PIC  X(001).                              
           10       WS-COD-AGENCIA    PIC  X(004).                              
      *                                                                         
         05         WS-DATA-INIVIG    PIC  X(10).                               
         05         WS-DATA-TERVIG    PIC  X(10).                               
      *                                                                         
         05         WS-DATA-SEC.                                                
           10       WS-SS-DATA-SEC    PIC  9(002)    VALUE  ZEROS.              
           10       WS-AA-DATA-SEC    PIC  9(002)    VALUE  ZEROS.              
           10       WS-MM-DATA-SEC    PIC  9(002)    VALUE  ZEROS.              
           10       WS-DD-DATA-SEC    PIC  9(002)    VALUE  ZEROS.              
      *                                                                         
         05         WS-DATA.                                                    
           10       WS-DD-DATA        PIC  9(002)    VALUE  ZEROS.              
           10       WS-MM-DATA        PIC  9(002)    VALUE  ZEROS.              
           10       WS-AA-DATA        PIC  9(002)    VALUE  ZEROS.              
      *                                                                         
         05         WS-DATE.                                                    
           10       WS-AA-DATE        PIC  9(002)    VALUE  ZEROS.              
           10       WS-MM-DATE        PIC  9(002)    VALUE  ZEROS.              
           10       WS-DD-DATE        PIC  9(002)    VALUE  ZEROS.              
      *                                                                         
         05         WS-DT-ANT.                                                  
           10       WS-AA-ANT         PIC  9(002)    VALUE  ZEROS.              
           10       WS-MM-ANT         PIC  9(002)    VALUE  ZEROS.              
           10       WS-DD-ANT         PIC  9(002)    VALUE  ZEROS.              
      *                                                                         
         05         WS-DT-POS.                                                  
           10       WS-AA-POS         PIC  9(002)    VALUE  ZEROS.              
           10       WS-MM-POS         PIC  9(002)    VALUE  ZEROS.              
           10       WS-DD-POS         PIC  9(002)    VALUE  ZEROS.              
      *                                                                         
         05         WS-TIME.                                                    
           10       WS-HH-TIME        PIC  9(002)    VALUE ZEROS.               
           10       WS-MM-TIME        PIC  9(002)    VALUE ZEROS.               
           10       WS-SS-TIME        PIC  9(002)    VALUE ZEROS.               
           10       WS-CC-TIME        PIC  9(002)    VALUE ZEROS.               
      *                                                                         
                                                                                
         05         WTIME-DAY          PIC  99.99.99.99.                        
         05         FILLER             REDEFINES      WTIME-DAY.                
           10       WTIME-DAYR.                                                 
             20     WTIME-HORA         PIC  X(002).                             
             20     WTIME-2PT1         PIC  X(001).                             
             20     WTIME-MINU         PIC  X(002).                             
             20     WTIME-2PT2         PIC  X(001).                             
             20     WTIME-SEGU         PIC  X(002).                             
           10       WTIME-2PT3         PIC  X(001).                             
           10       WTIME-CCSE         PIC  X(002).                             
                                                                                
      *                                                                         
         05         WDATA-CABEC.                                                
           10       WDATA-DD-CABEC    PIC  9(002)    VALUE ZEROS.               
           10       FILLER            PIC  X(001)    VALUE '/'.                 
           10       WDATA-MM-CABEC    PIC  9(002)    VALUE ZEROS.               
           10       FILLER            PIC  X(001)    VALUE '/'.                 
           10       WDATA-AA-CABEC    PIC  9(002)    VALUE ZEROS.               
      *                                                                         
         05         WHORA-CABEC.                                                
           10       WHORA-HH-CABEC    PIC  9(002)    VALUE ZEROS.               
           10       FILLER            PIC  X(001)    VALUE ':'.                 
           10       WHORA-MM-CABEC    PIC  9(002)    VALUE ZEROS.               
           10       FILLER            PIC  X(001)    VALUE ':'.                 
           10       WHORA-SS-CABEC    PIC  9(002)    VALUE ZEROS.               
      *                                                                         
         05         WDATA-MOVABE.                                               
           10       WDATA-SC-MOVABE   PIC  9(002)    VALUE ZEROS.               
           10       WDATA-AA-MOVABE   PIC  9(002)    VALUE ZEROS.               
           10       FILLER            PIC  X(001)    VALUE '-'.                 
           10       WDATA-MM-MOVABE   PIC  9(002)    VALUE ZEROS.               
           10       FILLER            PIC  X(001)    VALUE '-'.                 
           10       WDATA-DD-MOVABE   PIC  9(002)    VALUE ZEROS.               
      *                                                                         
      *                                                                         
         05 TAB-UF.                                                             
            10 FILLER PIC X(30) VALUE 'AC ACRE'.                                
            10 FILLER PIC X(30) VALUE 'AL ALAGOAS'.                             
            10 FILLER PIC X(30) VALUE 'AM AMAZONAS'.                            
            10 FILLER PIC X(30) VALUE 'AP AMAPA   '.                            
            10 FILLER PIC X(30) VALUE 'BA BAHIA'.                               
            10 FILLER PIC X(30) VALUE 'CE CEARA'.                               
            10 FILLER PIC X(30) VALUE 'DF DISTRITO FEDERAL'.                    
            10 FILLER PIC X(30) VALUE 'ES ESPIRITO SANTO'.                      
            10 FILLER PIC X(30) VALUE 'GO GOIANIA'.                             
            10 FILLER PIC X(30) VALUE 'MA MARANHAO'.                            
            10 FILLER PIC X(30) VALUE 'MG BELO HORIZONTE'.                      
            10 FILLER PIC X(30) VALUE 'MS MATO GROSSO DO SUL'.                  
            10 FILLER PIC X(30) VALUE 'MT MATO GROSSO'.                         
            10 FILLER PIC X(30) VALUE 'PA PARA'.                                
            10 FILLER PIC X(30) VALUE 'PB PARAIBA'.                             
            10 FILLER PIC X(30) VALUE 'PE PERNAMBUCO'.                          
            10 FILLER PIC X(30) VALUE 'PI PIAUI'.                               
            10 FILLER PIC X(30) VALUE 'PR PARANA'.                              
            10 FILLER PIC X(30) VALUE 'RJ RIO DE JANEIRO'.                      
            10 FILLER PIC X(30) VALUE 'RN RIO G DO NORTE'.                      
            10 FILLER PIC X(30) VALUE 'RO RONDONIA'.                            
            10 FILLER PIC X(30) VALUE 'RR RORAIMA'.                             
            10 FILLER PIC X(30) VALUE 'RS RIO G DO SUL'.                        
            10 FILLER PIC X(30) VALUE 'SC SANTA CATARINA'.                      
            10 FILLER PIC X(30) VALUE 'SE SERGIPE'.                             
            10 FILLER PIC X(30) VALUE 'SP SAO PAULO'.                           
            10 FILLER PIC X(30) VALUE 'TO TOCANTINS'.                           
         05 TAB-UF-R REDEFINES TAB-UF OCCURS 27 TIMES.                          
            10 TB-UF   PIC X(02).                                               
            10 FILLER  PIC X(01).                                               
            10 TB-EST  PIC X(27).                                               
      *----------------------------------------------------------------*        
      * DATAS AUXILIARES E DE FORMATACAO PARA CRITICA                  *        
      *----------------------------------------------------------------*        
      *                                                                         
           05 W-CAD-DATA-INI-VIG            PIC X(10) VALUE ZERO.               
           05 W-CAD-DATA-EXCLUSAO           PIC X(10) VALUE ZERO.               
           05 W-CAD-DATA-TER-VIG            PIC X(10) VALUE ZERO.               
           05 W-CAD-DATA-INI-1P             PIC X(10) VALUE ZERO.               
           05 W-CAD-DATA-FIM-1P             PIC X(10) VALUE ZERO.               
           05 W-CAD-DATA-GERACAO            PIC X(10) VALUE SPACES.             
           05 W-CAD-DATA-DEB.                                                   
              10 FILLER                     PIC X(08) VALUE SPACES.             
              10 W-CAD-DATA-DEB-DD          PIC 9(02) VALUE ZERO.               
      *                                                                         
           05 W-DATA-DDMMAAAA.                                                  
              10 W-DATA-DDMMAAAA-DD         PIC 9(02) VALUE ZERO.               
              10 W-DATA-DDMMAAAA-MM         PIC 9(02) VALUE ZERO.               
              10 W-DATA-DDMMAAAA-AAAA       PIC 9(04) VALUE ZERO.               
      *                                                                         
           05 W-DATA-AAAAMMDD.                                                  
              10 W-DATA-AAAAMMDD-AAAA       PIC 9(04) VALUE ZERO.               
              10 W-DATA-AAAAMMDD-MM         PIC 9(02) VALUE ZERO.               
              10 W-DATA-AAAAMMDD-DD         PIC 9(02) VALUE ZERO.               
      *                                                                         
           05 W-DATA-AAAA-MM-DD.                                                
              10 W-DATA-AAAA-MM-DD-AAAA     PIC 9(04) VALUE ZERO.               
              10 FILLER                     PIC X(01) VALUE '-'.                
              10 W-DATA-AAAA-MM-DD-MM       PIC 9(02) VALUE ZERO.               
              10 FILLER                     PIC X(01) VALUE '-'.                
              10 W-DATA-AAAA-MM-DD-DD       PIC 9(02) VALUE ZERO.               
      *                                                                         
      *----------------------------------------------------------------*        
      *                    LINHAS DE CABECALHOS                        *        
      *----------------------------------------------------------------*        
      *                                                                         
           05 LC01.                                                             
              10 FILLER                     PIC X(43) VALUE 'LT2000B'.          
              10 LC01-EMPRESA               PIC X(040) VALUE  SPACES.           
              10 FILLER                     PIC X(036) VALUE  SPACES.           
              10 FILLER                     PIC X(009) VALUE 'PAGINA'.          
              10 LC01-PAGINA                PIC ZZZ9.                           
      *                                                                         
           05 LC02.                                                             
              10 FILLER                     PIC X(119) VALUE  SPACES.           
              10 FILLER                     PIC X(05) VALUE 'DATA'.             
              10 LC02-DATA                  PIC X(08) VALUE  SPACES.            
      *                                                                         
           05 LC03.                                                             
              10 FILLER                     PIC X(33) VALUE  SPACES.            
              10 FILLER                     PIC X(86) VALUE                     
                 ' RELATORIO DE INCONSISTENCIAS NO MOVIMENTO DE                 
      -          'LOTERICOS - SIGEL'.                                           
              10 FILLER                     PIC X(05) VALUE 'HORA'.             
              10 LC03-HORA                  PIC X(08) VALUE  SPACES.            
      *                                                                         
           05 LC05.                                                             
              10 FILLER                     PIC X(132) VALUE ALL '-'.           
      *                                                                         
           05 LC06.                                                             
              10 FILLER                     PIC X(132) VALUE ALL ' '.           
      *                                                                         
           05 LD01-CAD.                                                         
              10 FILLER                     PIC X(11) VALUE                     
                 'COD. CEF:'.                                                   
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD01-CAD-COD-CEF           PIC X(09) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(13) VALUE                     
                 'RAZAO SOCIAL:'.                                               
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD01-CAD-RAZAO-SOCIAL      PIC X(56) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(09) VALUE                     
                 'SITUACAO:'.                                                   
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD01-SITUACAO              PIC X(01) VALUE SPACES.             
      *                                                                         
           05 LD02-CAD.                                                         
              10 FILLER                     PIC X(08) VALUE                     
                 'C.G.C.:'.                                                     
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD02-CAD-CGC               PIC X(16) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(05) VALUE                     
                 'END.:'.                                                       
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD02-CAD-END               PIC X(50) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(07) VALUE                     
                 'BAIRRO:'.                                                     
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD02-CAD-BAIRRO            PIC X(20) VALUE SPACES.             
      *                                                                         
           05 LD02A-CAD.                                                        
              10 FILLER                     PIC X(09) VALUE                     
                 'INSC MUN:'.                                                   
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD02A-CAD-INSC-MUN         PIC X(16) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(09) VALUE                     
                 'INSC EST:'.                                                   
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD02A-CAD-INSC-EST         PIC X(16) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
      *                                                                         
           05 LD02B-CAD.                                                        
              10 FILLER                     PIC X(09) VALUE                     
                 'LOT-ANT :'.                                                   
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD02B-LOT-ANT              PIC X(09) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(12) VALUE                     
                 'AGEN MASTER:'.                                                
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD02B-AGE-MASTER           PIC X(09) VALUE SPACES.             
              10 FILLER                     PIC X(10) VALUE SPACES.             
              10 FILLER                     PIC X(10) VALUE                     
                 'CATEGORIA:'.                                                  
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD02B-CATEGORIA            PIC X(02) VALUE SPACES.             
              10 FILLER                     PIC X(03) VALUE SPACES.             
              10 FILLER                     PIC X(07) VALUE                     
                 'STATUS:'.                                                     
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD02B-STATUS               PIC X(02) VALUE SPACES.             
      *                                                                         
           05 LD03-CAD.                                                         
              10 FILLER                     PIC X(04) VALUE                     
                 'CEP:'.                                                        
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD03-CAD-CEP-1             PIC 9(08) VALUE 0.                  
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(07) VALUE                     
                 'CIDADE:'.                                                     
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD03-CAD-CIDADE            PIC X(25) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(03) VALUE                     
                 'UF:'.                                                         
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD03-CAD-UF                PIC X(02) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(05) VALUE                     
                 'FONE:'.                                                       
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD03-CAD-DDD-FONE          PIC 9(03) VALUE 0.                  
              10 FILLER                     PIC X(01) VALUE '-'.                
              10 LD03-CAD-FONE              PIC 9(08) VALUE 0.                  
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(06) VALUE                     
                 ' FAX:'.                                                       
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD03-CAD-DDD-FAX           PIC 9(04) VALUE 0.                  
              10 FILLER                     PIC X(01) VALUE '-'.                
              10 LD03-CAD-FAX               PIC 9(08) VALUE 0.                  
      *                                                                         
           05 LD04-CAD.                                                         
              10 FILLER                     PIC X(12) VALUE                     
                 'IS.INCENDIO:'.                                                
              10 LD04-CAD-INCENDIO          PIC ZZZ.ZZZ.ZZ9,99.                 
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(07) VALUE                     
                 'TX INC:'.                                                     
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD04-CAD-TAXA-IS-INCENDIO  PIC ZZ9,999999999.                  
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(12) VALUE                     
                 'DANOS ELE:'.                                                  
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD04-CAD-DANOS             PIC ZZZ.ZZZ.ZZ9,99.                 
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(07) VALUE                     
                 'TX DEL:'.                                                     
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD04-CAD-TAXA-IS-DANOS     PIC ZZ9,999999999.                  
      *                                                                         
           05 LD04-CAD-A.                                                       
              10 FILLER                     PIC X(12) VALUE                     
                 'IS ACIDENTE:'.                                                
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD04-CAD-ACIDENTE          PIC ZZZ.ZZZ.ZZ9,99.                 
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(07) VALUE                     
                 'TX ACI:'.                                                     
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD04-CAD-TAXA-IS-ACIDENTE  PIC ZZ9,999999999.                  
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(12) VALUE                     
                 'IS VALORES:'.                                                 
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD04-CAD-VALORES           PIC ZZZ.ZZZ.ZZ9,99.                 
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(07) VALUE                     
                 'TX VAL:'.                                                     
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD04-CAD-TAXA-IS-VALORES   PIC ZZ9,999999999.                  
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(09) VALUE                     
                 'TIPO GAR:'.                                                   
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD04-CAD-TIPO-GARANTIA     PIC X(01) VALUE SPACES.             
      *                                                                         
           05 LD04-CAD-B.                                                       
              10 FILLER                     PIC X(08) VALUE                     
                 'B. SIN.:'.                                                    
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD04-CAD-BONUS-SIN         PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(08) VALUE                     
                 'B. ALA.:'.                                                    
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD04-CAD-BONUS-ALA         PIC X(02) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(08) VALUE                     
                 'B. CKT.:'.                                                    
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD04-CAD-BONUS-CKT         PIC X(02) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(08) VALUE                     
                 'B. COF.:'.                                                    
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD04-CAD-BONUS-COF         PIC X(02) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
      *                                                                         
           05 LD05-CAD.                                                         
              10 FILLER                     PIC X(15) VALUE                     
                 'DATA INCLUSAO:'.                                              
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD05-CAD-DATA-INCLUSAO     PIC X(10) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(15) VALUE                     
                 'DATA EXCLUSAO :'.                                             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD05-CAD-DATA-EXCLUSAO     PIC X(10) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(15) VALUE                     
                 'DATA GERACAO  :'.                                             
              10 LD05-CAD-DATA-GERACAO      PIC X(10) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(11) VALUE                     
                 'NR.SEQ REG:'.                                                 
              10 LD05-NSR                   PIC X(06) VALUE SPACES.             
      *                                                                         
           05 LD06-CAD.                                                         
              10 FILLER                     PIC X(09) VALUE                     
                 'MAT.CONS:'.                                                   
              10 LD06-MAT-CONSULTOR         PIC X(07) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(10) VALUE                     
                 'NOME CONS:'.                                                  
              10 LD06-NOME-CONSULTOR        PIC X(40).                          
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(06) VALUE                     
                 'EMAIL:'.                                                      
              10 LD06-EMAIL                 PIC X(50).                          
              10 FILLER                     PIC X(01) VALUE SPACES.             
      *                                                                         
           05 LD06A-CAD.                                                        
              10 FILLER                     PIC X(07) VALUE                     
                 'PV-SUB:'.                                                     
              10 LD06A-PV-SUB               PIC X(04) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(07) VALUE                     
                 'EN SUB:'.                                                     
              10 LD06A-EN-SUB               PIC X(04).                          
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(09) VALUE                     
                 'UNID SUB:'.                                                   
              10 LD06A-UNIDADE-SUB          PIC X(04).                          
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(12) VALUE                     
                 'NIVEL COMIS:'.                                                
              10 LD06A-NIVEL-COMISSAO       PIC X(01).                          
      *                                                                         
      *                                                                         
           05 LD06B-CAD.                                                        
              10 FILLER                     PIC X(11) VALUE                     
                 'N.FANTASIA:'.                                                 
              10 LD06B-NOME-FANTASIA        PIC X(30) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(09) VALUE                     
                 'CONTATO1:'.                                                   
              10 LD06B-CONTATO1             PIC X(20).                          
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(09) VALUE                     
                 'CONTATO2:'.                                                   
              10 LD06B-CONTATO2             PIC X(20).                          
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(10) VALUE                     
                 'COD.MUNIC:'.                                                  
              10 LD06B-COD-MUNICIPIO        PIC X(12).                          
                                                                                
           05 LD06C-CAD.                                                        
              10 FILLER                     PIC X(17) VALUE                     
                 'NUM. SEGURADORA:'.                                            
              10 LD06C-NUM-SEGURADORA      PIC X(50) VALUE SPACES.              
                                                                                
           05 LD07-CAD.                                                         
              10 FILLER                     PIC X(17) VALUE                     
                 'C/C CPMF   BANCO:'.                                           
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD07-CAD-BANCO             PIC 9(03) VALUE 0.                  
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(08) VALUE                     
                 'AGENCIA:'.                                                    
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD07-CAD-AGENCIA           PIC 9(04) VALUE 0.                  
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(09) VALUE                     
                 'OPERACAO:'.                                                   
              10 FILLER                     PIC X(01) VALUE SPACES.             
V.02  *       10 LD07-CAD-OPERACAO          PIC X(03) VALUE SPACES.             
V.02          10 LD07-CAD-OPERACAO          PIC X(04) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(06) VALUE                     
                 'CONTA:'.                                                      
              10 FILLER                     PIC X(01) VALUE SPACES.             
V.02  *       10 LD07-CAD-CONTA             PIC X(08) VALUE SPACES.             
V.02          10 LD07-CAD-CONTA             PIC X(12) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(03) VALUE                     
                 'DV:'.                                                         
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD07-CAD-DV-CONTA          PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(02) VALUE SPACES.             
      *                                                                         
           05 LD07A-CAD.                                                        
              10 FILLER                     PIC X(17) VALUE                     
                 'C/C ISENTA BANCO:'.                                           
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD07A-CAD-BANCO            PIC 9(03) VALUE 0.                  
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(08) VALUE                     
                 'AGENCIA:'.                                                    
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD07A-CAD-AGENCIA          PIC 9(04) VALUE 0.                  
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(09) VALUE                     
                 'OPERACAO:'.                                                   
              10 FILLER                     PIC X(01) VALUE SPACES.             
V.02  *       10 LD07A-CAD-OPERACAO         PIC X(03) VALUE SPACES.             
V.02          10 LD07A-CAD-OPERACAO         PIC X(04) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(06) VALUE                     
                 'CONTA:'.                                                      
              10 FILLER                     PIC X(01) VALUE SPACES.             
V.02  *       10 LD07A-CAD-CONTA            PIC X(08) VALUE SPACES.             
V.02          10 LD07A-CAD-CONTA            PIC X(12) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(03) VALUE                     
                 'DV:'.                                                         
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD07A-CAD-DV-CONTA         PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(02) VALUE SPACES.             
      *                                                                         
           05 LD07B-CAD.                                                        
              10 FILLER                     PIC X(17) VALUE                     
                 'C/C CAUCAO BANCO:'.                                           
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD07B-CAD-BANCO            PIC 9(03) VALUE 0.                  
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(08) VALUE                     
                 'AGENCIA:'.                                                    
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD07B-CAD-AGENCIA          PIC 9(04) VALUE 0.                  
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(09) VALUE                     
                 'OPERACAO:'.                                                   
              10 FILLER                     PIC X(01) VALUE SPACES.             
V.02  *       10 LD07B-CAD-OPERACAO         PIC X(03) VALUE SPACES.             
V.02          10 LD07B-CAD-OPERACAO         PIC X(04) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(06) VALUE                     
                 'CONTA:'.                                                      
              10 FILLER                     PIC X(01) VALUE SPACES.             
V.02  *       10 LD07B-CAD-CONTA            PIC X(08) VALUE SPACES.             
V.02          10 LD07B-CAD-CONTA            PIC X(12) VALUE SPACES.             
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(03) VALUE                     
                 'DV:'.                                                         
              10 FILLER                     PIC X(01) VALUE SPACES.             
              10 LD07B-CAD-DV-CONTA         PIC X(01) VALUE SPACES.             
              10 FILLER                     PIC X(02) VALUE SPACES.             
      *                                                                         
      *----------------------------------------------------------------*        
      *                    LINHAS DE MENSAGEM DE ERRO                  *        
      *----------------------------------------------------------------*        
      *                                                                         
           05 LD00.                                                             
              10 FILLER                     PIC X(010) VALUE SPACES.            
              10 LD00-MSG1                  PIC X(122) VALUE SPACES.            
      *----------------------------------------------------------------*        
      *                    LINHAS DE TOTAIS                                     
      *----------------------------------------------------------------*        
      *                                                                         
           05 LT00.                                                             
              10 FILLER                     PIC X(10) VALUE SPACES.             
              10 LT00-TEXTO                 PIC X(50) VALUE SPACES.             
              10 FILLER                     PIC X(10) VALUE ALL '.'.            
              10 FILLER                     PIC X(03) VALUE SPACES.             
              10 LT00-TOTAIS                PIC ZZZ.ZZZ.ZZ9.                    
      *                                                                         
           05 LT01.                                                             
              10 FILLER                     PIC X(10) VALUE SPACES.             
              10 LT01-TEXTO                 PIC X(50) VALUE SPACES.             
              10 FILLER                     PIC X(10) VALUE ALL '.'.            
              10 LT01-TOTAIS                PIC ZZZ.ZZZ.ZZ9,99.                 
      *                                                                         
      *----------------------------------------------------------------*        
      *       AREA DE LINKAGEM UTILIZANDO A PROCONV (CONVERTE CARACTER)         
      *----------------------------------------------------------------*        
      *                                                                         
      *----------------------------------------------------------------*        
                                                                                
       01      LK-CONVERSAO.                                                    
         05    LK-TAM-CAMPO             PIC 9(003)  VALUE ZEROS.                
         05    LK-CAMPO-ENTRADA         PIC X(100)  VALUE SPACES.               
         05    LK-CAMPO-SAIDA           PIC X(100)  VALUE SPACES.               
         05    LK-CONVER-MAIUSCULA      PIC X(001)  VALUE SPACES.               
                                                                                
      *--* AREAS DE DISPLAY DE ERRO SQL                                         
      *                                                                         
         05        WABEND.                                                      
           10      FILLER              PIC  X(008) VALUE                        
                  'LT2000B '.                                                   
           10      FILLER              PIC  X(025) VALUE                        
                  '*** ERRO EXEC SQL NUMERO '.                                  
           10      WNR-EXEC-SQL        PIC  X(004) VALUE '0000'.                
           10      FILLER              PIC  X(013) VALUE                        
                  ' *** SQLCODE '.                                              
           10      WSQLCODE            PIC  ZZZZZ999- VALUE ZEROS.              
      *----------------------------------------------------------------*        
      *                                                                         
       01 LK-AREA-LINK2.                                                        
           05 LK-DATA-ATUAL     PIC S9(08)      VALUE +0 COMP.                  
           05 LK-NUM-APOLICE    PIC S9(13)      VALUE +0 COMP-3.                
           05 LK-COD-RETORNO    PIC S9(04)      VALUE +0 COMP.                  
      *----------------------------------------------------------------*        
                                                                                
      *                                                                         
      *                                                                         
       PROCEDURE DIVISION.                                                      
      *-------------------                                                      
      *                                                                         
           EXEC SQL WHENEVER SQLWARNING CONTINUE  END-EXEC.                     
      *                                                                         
           EXEC SQL WHENEVER SQLERROR   CONTINUE  END-EXEC.                     
      *                                                                         
           EXEC SQL WHENEVER NOT FOUND  CONTINUE  END-EXEC.                     
      *                                                                         
           PERFORM  R0100-SELECT-V1SISTEMA                                      
      *                                                                         
           PERFORM  R0110-SELECT-APOLICE                                        
      *                                                                         
           PERFORM  R9000-OPEN-ARQUIVOS.                                        
      *                                                                         
           PERFORM  R7510-MONTA-CABECALHO.                                      
      *                                                                         
           PERFORM  R0900-LE-CADASTRO.                                          
      *                                                                         
           IF WFIM-CADASTRO  NOT EQUAL  SPACES                                  
              DISPLAY 'NENHUM REGISTRO ENCONTRADO NO ARQUIVO SIGEL '            
              GO TO    R0000-90-FINALIZA.                                       
      *                                                                         
      *    PERFORM R5000-SELECT-MAX-CONTA.                                      
      *                                                                         
           PERFORM  R1000-PROCESSA-CADASTRO                                     
             UNTIL  WFIM-CADASTRO  NOT EQUAL  SPACES.                           
      *                                                                         
      *    PERFORM R5100-UPDATE-MAX-CONTA.                                      
      *                                                                         
           EXEC SQL      COMMIT            WORK         END-EXEC.               
      *                                                                         
       R0000-90-FINALIZA.                                                       
      *                                                                         
           ADD       99       TO  W-AC-LINHA.                                   
           PERFORM  R7520-CABECALHO.                                            
      *                                                                         
           MOVE 'TOTAL DE REGISTROS LIDOS            '    TO LT00-TEXTO.        
           MOVE  W-AC-CAD-LIDOS             TO  LT00-TOTAIS                     
           WRITE  REG-RLT2000B  FROM  LT00  AFTER  2.                           
      *                                                                         
           MOVE 'TOTAL DE REGISTROS GRAVADOS   '          TO LT00-TEXTO.        
           MOVE  W-AC-LOTERICOS-GRAVADOS    TO  LT00-TOTAIS                     
           WRITE  REG-RLT2000B  FROM  LT00  AFTER  2.                           
      *                                                                         
           MOVE 'TOTAL DE REGISTROS REJEITADOS'           TO LT00-TEXTO.        
           MOVE  W-AC-LOTERICOS-REJEITADOS  TO  LT00-TOTAIS                     
           WRITE  REG-RLT2000B  FROM  LT00  AFTER  2.                           
      *                                                                         
      *                                                                         
      *                                                                         
           MOVE 'TOTAL DE REGISTROS BLACKLIST '           TO LT00-TEXTO.        
           MOVE  W-AC-LOTERICOS-BLACKLIST   TO  LT00-TOTAIS                     
           WRITE  REG-RLT2000B  FROM  LT00  AFTER  2.                           
      *                                                                         
      *                                                                         
           MOVE    '  TOTAL DE REGISTROS INCLUIDOS NA FC-LOTERICO ..='          
                      TO LT00-TEXTO.                                            
           MOVE       WS-FCLOT-TOTAL-INCLUIDOS TO LT00-TOTAIS                   
           WRITE  REG-RLT2000B  FROM  LT00  AFTER  2.                           
      *                                                                         
           MOVE    '  TOTAL DE REGISTROS ALTERADOS NA FC-LOTERICO ..='          
                      TO LT00-TEXTO.                                            
           MOVE       WS-FCLOT-TOTAL-ALTERADOS TO LT00-TOTAIS.                  
           WRITE  REG-RLT2000B  FROM  LT00  AFTER  2.                           
      *                                                                         
           MOVE    '  TOTAL DE LOTERICOS JA SEGURADOS ..............='          
                      TO LT00-TEXTO.                                            
           MOVE       WS-MVPROP-TOTAL-SEGURADOS TO LT00-TOTAIS.                 
           WRITE  REG-RLT2000B  FROM  LT00  AFTER  2.                           
      *                                                                         
           MOVE    '  TOTAL DE LOTERICOS NAO SEGURADOS .............='          
                      TO LT00-TEXTO.                                            
           MOVE     WS-MVPROP-TOTAL-NSEGURADOS TO LT00-TOTAIS.                  
           WRITE  REG-RLT2000B  FROM  LT00  AFTER  2.                           
      *                                                                         
      ****************************************************************          
OL0906     MOVE    '  TOTAL DE LOTERICOS ATIVOS NO SIGEL............='          
OL0906                TO LT00-TEXTO.                                            
OL0906     MOVE     W-AC-ATIVOS                TO LT00-TOTAIS.                  
OL0906     WRITE  REG-RLT2000B  FROM  LT00  AFTER  2.                           
      ****************************************************************          
OL0906     MOVE    '  TOTAL DE LOTERICOS INATIVOS NO SIGEL..........='          
OL0906                TO LT00-TEXTO.                                            
OL0906     MOVE     W-AC-INATIVOS                TO LT00-TOTAIS.                
OL0906     WRITE  REG-RLT2000B  FROM  LT00  AFTER  2.                           
      ****************************************************************          
OL0906     MOVE    '  TOTAL DE LOTERICOS EM PRE-CADASTRO NO SIGEL...='          
OL0906                TO LT00-TEXTO.                                            
OL0906     MOVE     W-AC-PRECAD                  TO LT00-TOTAIS.                
OL0906     WRITE  REG-RLT2000B  FROM  LT00  AFTER  2.                           
      ****************************************************************          
OL0906     MOVE    '  TOTAL DE LOTERICOS DA SEGURADORA 001(CS)......='          
OL0906                TO LT00-TEXTO.                                            
OL0906     MOVE     W-AC-SEGURADORA-CS           TO LT00-TOTAIS.                
OL0906     WRITE  REG-RLT2000B  FROM  LT00  AFTER  2.                           
      ****************************************************************          
OL0906     MOVE    '  TOTAL DE LOTERICOS DE OUTRAS SEGURADOARAS.....='          
OL0906                TO LT00-TEXTO.                                            
OL0906     MOVE     W-AC-SEGURADORA-OUTRAS       TO LT00-TOTAIS.                
OL0906     WRITE  REG-RLT2000B  FROM  LT00  AFTER  2.                           
      ****************************************************************          
OL0906     MOVE    '  TOTAL DE LOTERICOS COM GARANTIA = S (SEGURO)..='          
OL0906                TO LT00-TEXTO.                                            
OL0906     MOVE     W-AC-GARANTIA-S              TO LT00-TOTAIS.                
OL0906     WRITE  REG-RLT2000B  FROM  LT00  AFTER  2.                           
      ****************************************************************          
OL0906     MOVE    '  TOTAL DE LOTERICOS COM GARANTIA = C (CAUCAO)..='          
OL0906                TO LT00-TEXTO.                                            
OL0906     MOVE     W-AC-GARANTIA-C              TO LT00-TOTAIS.                
OL0906     WRITE  REG-RLT2000B  FROM  LT00  AFTER  2.                           
      *                                                                         
      *                                                                         
           MOVE    '  TOTAL DE LOTERICOS - INCLUSOES NO MOVIMENTO   ='          
                      TO LT00-TEXTO.                                            
           MOVE     WS-MVPROP-TOTAL-INCLUIDOS TO LT00-TOTAIS.                   
           WRITE  REG-RLT2000B  FROM  LT00  AFTER  2.                           
      *                                                                         
           MOVE    '  TOTAL DE LOTERICOS - MOV. DE ALTERACOES    - 3='          
                      TO LT00-TEXTO.                                            
           MOVE   WS-MVPROP-TOTAL-ALTERADOS TO LT00-TOTAIS.                     
           WRITE  REG-RLT2000B  FROM  LT00  AFTER  2.                           
      *                                                                         
           MOVE    '  TOTAL DE LOTERICOS - MOV DE CANCELAMENTOS - 7 ='          
                      TO LT00-TEXTO.                                            
           MOVE    WS-MVPROP-TOTAL-CANCELADOS TO LT00-TOTAIS.                   
           WRITE  REG-RLT2000B  FROM  LT00  AFTER  2.                           
      *                                                                         
      *                                                                         
           PERFORM  R9100-CLOSE-ARQUIVOS.                                       
      *                                                                         
                                                                                
           DISPLAY '              PROGRAMA - LT2000B                  '         
           DISPLAY '                                                  '         
           DISPLAY '                TERMINO NORMAL                    '         
           DISPLAY '                                                  '         
           DISPLAY '  TOTAL DE REGISTROS LIDOS NO ARQ SIGEL ....... ='          
                               W-AC-CAD-LIDOS.                                  
           DISPLAY '  TOTAL DE REGISTROS REJEITADOS ............... ='          
                               W-AC-LOTERICOS-REJEITADOS.                       
           DISPLAY '  TOTAL DE REGISTROS GRAVADOS ................. ='          
                               W-AC-LOTERICOS-GRAVADOS.                         
           DISPLAY '                  '.                                        
           DISPLAY '                  '.                                        
           DISPLAY '  TOTAL DE REGISTROS INCLUIDOS NA FC-LOTERICO ..='          
                               WS-FCLOT-TOTAL-INCLUIDOS.                        
           DISPLAY '  TOTAL DE REGISTROS ALTERADOS NA FC-LOTERICO ..='          
                               WS-FCLOT-TOTAL-ALTERADOS.                        
           DISPLAY '             '.                                             
           DISPLAY '             '.                                             
           DISPLAY '  TOTAL DE LOTERICOS JA SEGURADOS ..............='          
                               WS-MVPROP-TOTAL-SEGURADOS.                       
           DISPLAY '  TOTAL DE LOTERICOS NAO SEGURADOS .............='          
                               WS-MVPROP-TOTAL-NSEGURADOS.                      
           DISPLAY '  TOTAL DE LOTERICOS - INCLUSOES NO MOVIMENTO   ='          
                               WS-MVPROP-TOTAL-INCLUIDOS.                       
           DISPLAY '  TOTAL DE LOTERICOS - MOV DE ALTERACOES    - 3 ='          
                               WS-MVPROP-TOTAL-ALTERADOS.                       
           DISPLAY '  TOTAL DE LOTERICOS - MOV DE CANCELAMENTOS - 7 ='          
                               WS-MVPROP-TOTAL-CANCELADOS.                      
           DISPLAY '  TOTAL DE DELECOES NA FC_PEND_LOTERICOS .......='          
                               WS-FCPEND-DELETADOS.                             
                                                                                
           DISPLAY '                                                  '.        
           DISPLAY '**************************************************'.        
      *                                                                         
           MOVE          ZEROS             TO           RETURN-CODE             
                                                                                
           STOP          RUN.                                                   
      *                                                                         
       R0000-SAIDA. EXIT.                                                       
      *----------------------------------------------------------------*        
       R0100-SELECT-V1SISTEMA   SECTION.                                        
      *---------------------------------                                        
      *                                                                         
           MOVE  '0001'  TO  WNR-EXEC-SQL.                                      
      *                                                                         
           EXEC SQL                                                             
             SELECT  DTMOVABE                                                   
               INTO :V0SIST-DTMOVABE                                            
               FROM  SEGUROS.V0SISTEMA                                          
              WHERE  IDSISTEM  =  'LT'                                          
           END-EXEC.                                                            
      *                                                                         
           IF (SQLCODE  NOT EQUAL  ZEROS)  AND                                  
              (SQLCODE  NOT EQUAL  100)                                         
                 DISPLAY 'PROBLEMA SELECT V1SISTEMA'                            
                 GO  TO  R9999-ROT-ERRO.                                        
      *                                                                         
           MOVE    V0SIST-DTMOVABE            TO  WDATA-MOVABE.                 
           MOVE    WDATA-AA-MOVABE            TO  WS-AA-ANT.                    
           MOVE    WDATA-MM-MOVABE            TO  WS-MM-ANT.                    
           MOVE    01                         TO  WS-DD-ANT.                    
           MOVE    WDATA-AA-MOVABE            TO  WS-AA-POS.                    
           MOVE    WDATA-MM-MOVABE            TO  WS-MM-POS.                    
           MOVE    WDATA-DD-MOVABE            TO  WS-DD-POS.                    
      *                                                                         
           DISPLAY 'LT2000B - DTMOVABE :'V0SIST-DTMOVABE.                       
      *                                                                         
       R0100-SAIDA. EXIT.                                                       
      *----------------------------------------------------------------*        
       R0110-SELECT-APOLICE SECTION.                                            
      *---------------------------                                              
      *                                                                         
           MOVE  '0110'  TO  WNR-EXEC-SQL.                                      
      *                                                                         
           MOVE 0        TO LK-DATA-ATUAL.                                      
           CALL 'LT2100S' USING LK-AREA-LINK2.                                  
                                                                                
           MOVE LK-NUM-APOLICE TO WS-NUM-APOLICE                                
                                LTSOLPAR-PARAM-DEC01.                           
                                                                                
                                                                                
      *                                                                         
       R0110-SAIDA. EXIT.                                                       
      *----------------------------------------------------------------*        
      *-----------------------------------------------------------------        
       R0900-LE-CADASTRO                                        SECTION.        
      *-----------------------------------------------------------------        
      *                                                                         
           READ  CADASTRO                                                       
                 AT END                                                         
                 MOVE  'SIM'  TO  WFIM-CADASTRO                                 
                 GO  TO  R0900-SAIDA.                                           
      *                                                                         
            ADD  1  TO  W-AC-CAD-LIDOS.                                         
      *                                                                         
           MOVE  REG-CAD TO  REG-CAD-CADASTRO.                                  
      *                                                                         
           IF CAD-TIPO   = 'H'                                                  
              MOVE  REG-CAD TO  REG-CAD-HEADER                                  
              PERFORM R0950-TRATA-HEADER                                        
              GO  TO  R0900-LE-CADASTRO.                                        
      *                                                                         
           IF CAD-TIPO   = 'T'                                                  
              MOVE  REG-CAD TO  REG-CAD-TRAILLER                                
              GO  TO  R0900-LE-CADASTRO.                                        
                                                                                
                                                                                
ALTSS *    IF W-AC-CAD-LIDOS  >   800                                           
ALTSS *       MOVE  'SIM'  TO  WFIM-CADASTRO                                    
ALTSS *       DISPLAY 'R0900 - FIM TESTE TESTE TESTE'                           
      *       GO  TO  R0900-SAIDA.                                              
                                                                                
           .                                                                    
       R0900-SAIDA. EXIT.                                                       
      *----------------------------------------------------------------*        
       R0950-TRATA-HEADER   SECTION.                                            
      *-----------------------------                                            
      *                                                                         
           MOVE  SPACES                TO  W-CAD-DATA-GERACAO                   
      *                                                                         
           IF CAD-DATA-GERACAO IS NUMERIC                                       
              MOVE  CAD-DATA-GERACAO      TO  W-DATA-AAAAMMDD                   
                                           WS-DATA-SEC                          
              MOVE  W-DATA-AAAAMMDD-DD    TO  W-DATA-AAAA-MM-DD-DD              
              MOVE  W-DATA-AAAAMMDD-MM    TO  W-DATA-AAAA-MM-DD-MM              
              MOVE  W-DATA-AAAAMMDD-AAAA  TO  W-DATA-AAAA-MM-DD-AAAA            
              MOVE  W-DATA-AAAA-MM-DD     TO  W-CAD-DATA-GERACAO                
              PERFORM  R1055-CRITICA-DATA                                       
           ELSE                                                                 
              MOVE 1 TO WS-ERRO-DATA.                                           
      *                                                                         
           IF WS-ERRO-DATA  EQUAL  1                                            
              DISPLAY 'R0950-ERRO DATA GERACAO =' CAD-DATA-GERACAO              
              GO TO R9999-ROT-ERRO.                                             
      *                                                                         
       R0950-SAIDA. EXIT.                                                       
      *----------------------------------------------------------------*        
       R1000-PROCESSA-CADASTRO                                  SECTION.        
      *----------------------------------------------------------------*        
      *                                                                         
           PERFORM  R7650-CONVERTE-CARACTER.                                    
      *                                                                         
           PERFORM  R1050-CRITICA-CADASTRO.                                     
      *                                                                         
           IF WS-OBRIGATORIO = 1                                                
              ADD  1  TO  W-AC-LOTERICOS-REJEITADOS                             
              GO TO R1000-LER-CADASTRO.                                         
                                                                                
           MOVE SPACES               TO  LD00-MSG1                              
           MOVE 'LT2000B'            TO  LTMVPROP-COD-USUARIO.                  
                                                                                
           PERFORM R6020-SELECT-FC-LOTERICO.                                    
                                                                                
           PERFORM R6030-SELECT-V0LOTERICO01.                                   
                                                                                
           IF W-CHAVE-CADASTRADO-SIGEL   EQUAL  'NAO'                           
              PERFORM R6220-GRAVAR-FC-CONTA                                     
              PERFORM R6200-MONTAR-FC-LOTERICO                                  
              PERFORM R6210-INSERT-FC-LOTERICO                                  
              GO TO   R1000-LER-CADASTRO                                        
           END-IF                                                               
                                                                                
           PERFORM R6000-VER-ALTERACAO-LOTERICO.                                
           PERFORM R6060-VER-ALTERACAO-FC-CONTA.                                
           PERFORM R6850-VER-ALTERACAO-BONUS.                                   
                                                                                
           IF W-CHAVE-HOUVE-ALTERACAO = 'SIM'                                   
              PERFORM  R6200-MONTAR-FC-LOTERICO                                 
              PERFORM  R6700-UPDATE-FC-LOTERICO                                 
           END-IF                                                               
           .                                                                    
       R1000-LER-CADASTRO.                                                      
                                                                                
           PERFORM  R0900-LE-CADASTRO.                                          
           .                                                                    
       R1000-SAIDA. EXIT.                                                       
      *-----------------------------------------------------------------        
                                                                                
      *                                                                         
       R1010-VERIFICA-UF  SECTION.                                              
      *--------------------------                                               
      *                                                                         
           IF CAD-UF = TB-UF(WS-IND)                                            
              MOVE  1  TO WS-TEM-UF.                                            
      *                                                                         
       R1010-SAIDA. EXIT.                                                       
      *                                                                         
       R1050-CRITICA-CADASTRO  SECTION.                                         
      *---------------------------------------                                  
           MOVE SPACES TO LD00-MSG1.                                            
           MOVE 0      TO WS-OBRIGATORIO WS-NECESSARIO WS-IMPRIMIU.             
      *                                                                         
      *  TIPO DE REGISTRO = C  DETALHES                                         
                                                                                
           IF CAD-TIPO           NOT EQUAL   'C'                                
              MOVE 'TIPO DE REGISTRO INVALIDO             ' TO LD00-MSG1        
              MOVE 1       TO  WS-OBRIGATORIO                                   
              MOVE SPACES  TO  CAD-TIPO                                         
              PERFORM  R7600-IMPRIME-LD00-MSG1.                                 
                                                                                
           IF CAD-COD-CEF        NOT NUMERIC  OR                                
              CAD-COD-CEF        EQUAL ZEROS                                    
              MOVE 'CODIGO LOTERICO INVALIDO              ' TO LD00-MSG1        
              MOVE 1     TO  WS-OBRIGATORIO                                     
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0     TO  CAD-COD-CEF.                                       
                                                                                
           IF CAD-DV-CEF        NOT NUMERIC                                     
              MOVE 'DIGITO DO LOTERICO INVALIDO         ' TO LD00-MSG1          
              MOVE 1     TO  WS-OBRIGATORIO                                     
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0     TO  CAD-COD-CEF.                                       
                                                                                
           IF CAD-RAZAO-SOCIAL EQUAL SPACES                                     
              MOVE 'FALTA RAZAO SOCIAL                ' TO LD00-MSG1            
              MOVE 1      TO  WS-NECESSARIO                                     
              PERFORM  R7600-IMPRIME-LD00-MSG1.                                 
                                                                                
           IF CAD-ENDERECO  EQUAL SPACES                                        
              MOVE 'FALTA ENDERECO                    ' TO LD00-MSG1            
              MOVE 1       TO  WS-NECESSARIO                                    
              PERFORM  R7600-IMPRIME-LD00-MSG1.                                 
                                                                                
           IF CAD-BAIRRO    EQUAL SPACES                                        
              MOVE 'FALTA BAIRRO                      ' TO LD00-MSG1            
              MOVE 1     TO  WS-NECESSARIO                                      
              PERFORM  R7600-IMPRIME-LD00-MSG1.                                 
                                                                                
           IF CAD-COD-MUNICIPIO   EQUAL SPACES                                  
              MOVE 'FALTA CODIGO DO MUNICIPIO         ' TO LD00-MSG1            
              PERFORM  R7600-IMPRIME-LD00-MSG1.                                 
                                                                                
           IF CAD-CIDADE    EQUAL SPACES                                        
              MOVE 'FALTA CIDADE                      ' TO LD00-MSG1            
              MOVE 1     TO  WS-NECESSARIO                                      
              PERFORM  R7600-IMPRIME-LD00-MSG1.                                 
                                                                                
           IF CAD-CEP            NOT NUMERIC   OR                               
              CAD-CEP            EQUAL  ZEROS                                   
              MOVE 'CEP INVALIDO                      ' TO LD00-MSG1            
              MOVE 1     TO  WS-NECESSARIO                                      
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0     TO  CAD-CEP.                                           
                                                                                
           IF CAD-UF        EQUAL SPACES                                        
              MOVE 'FALTA UF                    ' TO LD00-MSG1                  
              MOVE 1     TO  WS-NECESSARIO                                      
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
           ELSE                                                                 
              MOVE  0      TO WS-TEM-UF                                         
              PERFORM  R1010-VERIFICA-UF VARYING WS-IND FROM 1 BY 1             
                       UNTIL WS-IND > 27                                        
              IF WS-TEM-UF = 0                                                  
                 MOVE SPACES  TO  CAD-UF                                        
                 MOVE 1       TO  WS-NECESSARIO                                 
                 MOVE 'UF NAO CADASTRADA ' TO LD00-MSG1                         
                 PERFORM  R7600-IMPRIME-LD00-MSG1.                              
      *                                                                         
           IF CAD-TELEFONE EQUAL SPACES                                         
              MOVE ZEROS    TO   CAD-TELEFONE                                   
           ELSE                                                                 
              MOVE CAD-TELEFONE  TO  WS-NUM-TELEF-ENT                           
              PERFORM R1100-TRATAR-TELEF-FAX                                    
              MOVE WS-NUM-TELEF-SAI  TO  CAD-TELEFONE.                          
      *                                                                         
           IF CAD-DDD-FONE NOT NUMERIC                                          
              MOVE 'DDD INVALIDO       ' TO LD00-MSG1                           
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE ZEROS    TO   CAD-DDD-FONE.                                  
                                                                                
           IF CAD-FONE  NOT NUMERIC                                             
              MOVE 'TELEFONE INVALIDO       ' TO LD00-MSG1                      
              MOVE ZEROS    TO   CAD-TELEFONE                                   
              PERFORM  R7600-IMPRIME-LD00-MSG1.                                 
                                                                                
           IF CAD-CGC NOT NUMERIC  OR  CAD-CGC EQUAL ZEROS                      
              MOVE 'CGC INVALIDO                     ' TO LD00-MSG1             
              MOVE 1     TO  WS-NECESSARIO                                      
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0     TO  CAD-CGC.                                           
                                                                                
           IF CAD-INSC-MUN       NOT NUMERIC  OR                                
              CAD-INSC-MUNX       =  ZEROS                                      
              MOVE SPACES     TO  CAD-INSC-MUNX.                                
      *       MOVE 'INSCR. MUNIC. NAO NUMERICA   ' TO LD00-MSG1                 
      *       PERFORM  R7600-IMPRIME-LD00-MSG1                                  
      *       MOVE 0     TO  CAD-INSC-MUN.                                      
                                                                                
           IF CAD-INSC-EST       NOT NUMERIC  OR                                
              CAD-INSC-ESTX       =  ZEROS                                      
              MOVE SPACES     TO  CAD-INSC-ESTX.                                
      *       MOVE 'INSCR ESTADUAL NAO NUMERICA  ' TO LD00-MSG1                 
      *       PERFORM  R7600-IMPRIME-LD00-MSG1                                  
      *       MOVE 0     TO  CAD-INSC-EST.                                      
                                                                                
           IF CAD-SITUACAO       NOT EQUAL 0 AND                                
              CAD-SITUACAO       NOT EQUAL 1 AND                                
              CAD-SITUACAO       NOT EQUAL 2                                    
              MOVE 'SITUACAO INVALIDA       ' TO LD00-MSG1                      
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 2     TO  CAD-SITUACAO.                                      
                                                                                
           MOVE  SPACES                TO  W-CAD-DATA-INI-VIG                   
                                           W-CAD-DATA-TER-VIG                   
                                           W-CAD-DATA-EXCLUSAO.                 
                                                                                
           IF CAD-DATA-INCLUSAO  NOT NUMERIC OR                                 
              CAD-DATA-INCLUSAO  EQUAL ZEROS                                    
              MOVE 'DATA DE INCLUSAO INVALIDA       ' TO LD00-MSG1              
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0     TO  CAD-DATA-INCLUSAO                                  
           ELSE                                                                 
           MOVE  CAD-DATA-INCLUSAO     TO  W-DATA-AAAAMMDD                      
                                           WS-DATA-SEC                          
           MOVE  W-DATA-AAAAMMDD-DD    TO  W-DATA-AAAA-MM-DD-DD                 
           MOVE  W-DATA-AAAAMMDD-MM    TO  W-DATA-AAAA-MM-DD-MM                 
           MOVE  W-DATA-AAAAMMDD-AAAA  TO  W-DATA-AAAA-MM-DD-AAAA               
           MOVE  W-DATA-AAAA-MM-DD     TO  W-CAD-DATA-INI-VIG                   
           PERFORM  R1055-CRITICA-DATA                                          
           IF WS-ERRO-DATA  EQUAL  1                                            
              MOVE 'DATA DE INCLUSAO INVALIDA       ' TO LD00-MSG1              
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0     TO  CAD-DATA-INCLUSAO                                  
                             W-CAD-DATA-INI-VIG.                                
                                                                                
           IF CAD-DATA-EXCLUSAO  NOT NUMERIC                                    
              MOVE 'DATA DE EXCLUSAO NAO NUMERICA   ' TO LD00-MSG1              
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0     TO  CAD-DATA-EXCLUSAO                                  
           ELSE                                                                 
           IF CAD-DATA-EXCLUSAO  > ZEROS                                        
              MOVE  CAD-DATA-EXCLUSAO     TO  W-DATA-AAAAMMDD                   
                                              WS-DATA-SEC                       
              MOVE  W-DATA-AAAAMMDD-DD    TO  W-DATA-AAAA-MM-DD-DD              
              MOVE  W-DATA-AAAAMMDD-MM    TO  W-DATA-AAAA-MM-DD-MM              
              MOVE  W-DATA-AAAAMMDD-AAAA  TO  W-DATA-AAAA-MM-DD-AAAA            
              MOVE  W-DATA-AAAA-MM-DD     TO  W-CAD-DATA-TER-VIG                
              MOVE  W-DATA-AAAA-MM-DD     TO  W-CAD-DATA-EXCLUSAO               
              PERFORM  R1055-CRITICA-DATA                                       
              IF WS-ERRO-DATA  EQUAL  1                                         
                 MOVE 'DATA DE EXCLUSAO INVALIDA       ' TO LD00-MSG1           
                 PERFORM  R7600-IMPRIME-LD00-MSG1                               
                 MOVE 0     TO  CAD-DATA-EXCLUSAO                               
                                W-CAD-DATA-TER-VIG                              
                                W-CAD-DATA-EXCLUSAO.                            
                                                                                
           IF CAD-NUM-LOT-ANTERIOR NOT NUMERIC                                  
              MOVE 'COD. LOTERICO ANTERIOR INVALIDO     ' TO LD00-MSG1          
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0     TO  CAD-NUM-LOT-ANTERIOR                               
           ELSE                                                                 
              IF CAD-NUM-LOT-ANTERIOR EQUAL CAD-COD-CEF                         
                 MOVE 0     TO  CAD-NUM-LOT-ANTERIOR.                           
                                                                                
           IF CAD-COD-AG-MASTER  NOT NUMERIC                                    
              MOVE 'CODIGO AGENTE MASTER INVALIDO     ' TO LD00-MSG1            
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0     TO  CAD-COD-AG-MASTER.                                 
                                                                                
           IF CAD-CAT-LOTERICO  NOT NUMERIC OR                                  
              CAD-CAT-LOTERICO  EQUAL ZEROS                                     
              MOVE 'CATEGORIA INVALIDA                ' TO LD00-MSG1            
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0     TO  CAD-CAT-LOTERICO.                                  
                                                                                
           IF CAD-COD-STATUS    NOT NUMERIC OR                                  
              CAD-COD-STATUS    EQUAL ZEROS                                     
              MOVE 'CODIGO DO STATUS INVALIDO         ' TO LD00-MSG1            
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0     TO  CAD-COD-STATUS.                                    
                                                                                
***********  CRITICA DE CONTAS BANCARIAS ****************************           
                                                                                
           IF CAD-BANCO-DESC-CPMF  NOT NUMERIC  OR                              
              CAD-BANCO-DESC-CPMF  EQUAL ZEROS                                  
              MOVE 'BANCO COM DESC CPMF INVALIDO          ' TO LD00-MSG1        
              MOVE 1     TO  WS-NECESSARIO                                      
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0     TO  CAD-BANCO-DESC-CPMF.                               
                                                                                
           IF CAD-AGEN-DESC-CPMF  NOT NUMERIC  OR                               
              CAD-AGEN-DESC-CPMF  EQUAL ZEROS                                   
              MOVE 'AGENCIA COM DESC CPMF INVALIDA       ' TO LD00-MSG1         
              MOVE 1     TO  WS-NECESSARIO                                      
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0     TO  CAD-AGEN-DESC-CPMF.                                
                                                                                
           IF CAD-CONTA-DESC-CPMF  NOT NUMERIC  OR                              
              CAD-CONTA-DESC-CPMF  EQUAL ZEROS                                  
              MOVE 'CONTA COM DESC CPMF INVALIDA       ' TO LD00-MSG1           
              MOVE 1     TO  WS-NECESSARIO                                      
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0     TO  CAD-CONTA-DESC-CPMF.                               
                                                                                
  **********************************************************************        
                                                                                
           IF CAD-BANCO-ISENTA     NOT NUMERIC  OR                              
              CAD-BANCO-ISENTA     EQUAL ZEROS                                  
              MOVE 'BANCO ISENTA  INVALIDO          ' TO LD00-MSG1              
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0     TO  CAD-BANCO-ISENTA.                                  
                                                                                
           IF CAD-AGEN-ISENTA     NOT NUMERIC  OR                               
              CAD-AGEN-ISENTA     EQUAL ZEROS                                   
              MOVE 'AGENCIA ISENTA INVALIDA       ' TO LD00-MSG1                
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0     TO  CAD-AGEN-ISENTA.                                   
                                                                                
           IF CAD-CONTA-ISENTA     NOT NUMERIC  OR                              
              CAD-CONTA-ISENTA     EQUAL ZEROS                                  
              MOVE 'CONTA ISENTA  INVALIDA       ' TO LD00-MSG1                 
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0     TO  CAD-CONTA-ISENTA.                                  
                                                                                
  ****************************************************************              
                                                                                
           IF CAD-BANCO-CAUCAO     NOT NUMERIC  OR                              
              CAD-BANCO-CAUCAO     EQUAL ZEROS                                  
  **********  MOVE 'BANCO CAUCAO  INVALIDO          ' TO LD00-MSG1              
  **********  PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0     TO  CAD-BANCO-CAUCAO.                                  
                                                                                
           IF CAD-AGEN-CAUCAO     NOT NUMERIC  OR                               
              CAD-AGEN-CAUCAO     EQUAL ZEROS                                   
  **********  MOVE 'AGENCIA CAUCAO INVALIDA       ' TO LD00-MSG1                
  **********  PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0     TO  CAD-AGEN-CAUCAO.                                   
                                                                                
           IF CAD-CONTA-CAUCAO     NOT NUMERIC  OR                              
              CAD-CONTA-CAUCAO     EQUAL ZEROS                                  
   *********  MOVE 'CONTA CAUCAO  INVALIDA       ' TO LD00-MSG1                 
   *********  PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0     TO  CAD-CONTA-CAUCAO.                                  
                                                                                
   ****************************************************************             
                                                                                
           IF CAD-NIVEL-COMISSAO NOT NUMERIC                                    
              MOVE 'NIVEL DE COMISSAO INVALIDA       ' TO LD00-MSG1             
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0       TO CAD-NIVEL-COMISSAO.                               
                                                                                
           IF CAD-PV-SUB  NOT NUMERIC                                           
              MOVE 'CODIGO DO PV INVALIDO       ' TO LD00-MSG1                  
              MOVE 1       TO WS-NECESSARIO                                     
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0       TO CAD-PV-SUB.                                       
                                                                                
           IF CAD-EN-SUB  NOT NUMERIC                                           
              MOVE 'CODIGO DO EN INVALIDO       ' TO LD00-MSG1                  
              MOVE 1       TO WS-NECESSARIO                                     
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0       TO CAD-EN-SUB.                                       
                                                                                
           IF CAD-UNIDADE-SUB  NOT NUMERIC                                      
              MOVE 'CODIGO DA UNIDADE-SUB INVALIDO    ' TO LD00-MSG1            
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0       TO CAD-UNIDADE-SUB.                                  
                                                                                
           IF CAD-MATR-CONSULTOR NOT NUMERIC                                    
              MOVE 'MATRICULA DO CONSULTOR INVALIDA  ' TO LD00-MSG1             
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0       TO CAD-MATR-CONSULTOR.                               
                                                                                
           IF CAD-TIPO-GARANTIA NOT EQUAL ' ' AND                               
              CAD-TIPO-GARANTIA NOT EQUAL 'C' AND                               
              CAD-TIPO-GARANTIA NOT EQUAL 'S'                                   
              MOVE 'TIPO DE GARANTIA INVALIDA       ' TO LD00-MSG1              
              MOVE 1       TO   WS-NECESSARIO                                   
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE SPACES  TO CAD-TIPO-GARANTIA.                                
                                                                                
           IF CAD-TIPO-GARANTIA EQUAL ' '                                       
              MOVE 0    TO   CAD-VALOR-GARANTIA                                 
           ELSE                                                                 
             IF CAD-VALOR-GARANTIA NOT NUMERIC  OR                              
                CAD-VALOR-GARANTIA EQUAL ZEROS                                  
                MOVE 'VALOR DE GARANTIA INVALIDO       ' TO LD00-MSG1           
                MOVE 1    TO   WS-NECESSARIO                                    
                PERFORM  R7600-IMPRIME-LD00-MSG1                                
                MOVE 0    TO   CAD-VALOR-GARANTIA.                              
                                                                                
           IF CAD-NUM-SEGURADORAX =  SPACES                                     
              MOVE ZEROS TO  CAD-NUM-SEGURADORA.                                
                                                                                
           IF CAD-TIPO-GARANTIA NOT = 'S'                                       
              MOVE 0    TO   CAD-NUM-SEGURADORA                                 
           ELSE                                                                 
              IF CAD-NUM-SEGURADORA  NOT NUMERIC   OR                           
                 CAD-NUM-SEGURADORA  =   ZEROS                                  
                 MOVE 'NUM.SEGURADORA INVALIDO       ' TO LD00-MSG1             
                 MOVE 1    TO   WS-NECESSARIO                                   
                 PERFORM  R7600-IMPRIME-LD00-MSG1                               
                 MOVE 0    TO   CAD-NUM-SEGURADORA.                             
                                                                                
           IF CAD-BONUS-CKTX  NOT EQUAL  '0'   AND                              
              CAD-BONUS-CKTX  NOT EQUAL  '1'   AND                              
              CAD-BONUS-CKTX  NOT EQUAL  '2'                                    
              MOVE 'BONUS DE CKT-TV INVALIDO              ' TO LD00-MSG1        
              MOVE 1    TO   WS-NECESSARIO                                      
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0    TO   CAD-BONUS-CKT.                                     
                                                                                
           IF CAD-BONUS-ALARMEX  NOT EQUAL  '0'   AND                           
              CAD-BONUS-ALARMEX  NOT EQUAL  '1'   AND                           
              CAD-BONUS-ALARMEX  NOT EQUAL  '2'                                 
              MOVE 'BONUS DE ALARME INVALIDO              ' TO LD00-MSG1        
              MOVE 1    TO   WS-NECESSARIO                                      
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0    TO   CAD-BONUS-ALARME.                                  
                                                                                
           IF CAD-BONUS-COFREX  NOT EQUAL  '0'   AND                            
              CAD-BONUS-COFREX  NOT EQUAL  '1'   AND                            
              CAD-BONUS-COFREX  NOT EQUAL  '2'                                  
              MOVE 'BONUS DE COFRE  INVALIDO              ' TO LD00-MSG1        
              MOVE 1    TO   WS-NECESSARIO                                      
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE 0    TO   CAD-BONUS-COFRE.                                   
                                                                                
           IF CAD-NUMERO-FAX EQUAL SPACES                                       
              MOVE ZEROS    TO   CAD-NUMERO-FAX                                 
           ELSE                                                                 
              MOVE CAD-NUMERO-FAX  TO  WS-NUM-TELEF-ENT                         
              PERFORM R1100-TRATAR-TELEF-FAX                                    
              MOVE WS-NUM-TELEF-SAI  TO  CAD-NUMERO-FAX.                        
      *                                                                         
           IF CAD-FAX NOT NUMERIC                                               
              MOVE 'FAX NAO NUMERICO   ' TO LD00-MSG1                           
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE ZEROS  TO   CAD-NUMERO-FAX.                                  
                                                                                
           IF CAD-DDD-FAX NOT NUMERIC                                           
              MOVE 'DDD FAX NAO NUMERICO   ' TO LD00-MSG1                       
              PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE ZEROS  TO   CAD-DDD-FAX.                                     
      *                                                                         
                                                                                
OL0906     IF CAD-SITUACAO       EQUAL 0                                        
OL0906        ADD 1       TO W-AC-INATIVOS.                                     
                                                                                
OL0906     IF CAD-SITUACAO       EQUAL 1                                        
OL0906        ADD 1       TO W-AC-ATIVOS.                                       
                                                                                
OL0906     IF CAD-SITUACAO       EQUAL 2                                        
OL0906        ADD 1       TO W-AC-PRECAD.                                       
                                                                                
OL0906     IF CAD-TIPO-GARANTIA     EQUAL 'C'                                   
OL0906        ADD 1 TO            W-AC-GARANTIA-C.                              
                                                                                
OL0906     IF CAD-TIPO-GARANTIA     EQUAL 'S'                                   
OL0906        ADD 1 TO            W-AC-GARANTIA-S.                              
                                                                                
OL0906     IF CAD-NUM-SEGURADORA  = 1                                           
OL0906        ADD 1 TO W-AC-SEGURADORA-CS                                       
OL0906     ELSE                                                                 
OL0906        IF CAD-NUM-SEGURADORA  > 1                                        
OL0906           ADD 1 TO W-AC-SEGURADORA-OUTRAS.                               
                                                                                
       R1050-SAIDA. EXIT.                                                       
      *-----------------------------------------------------------------        
       R1055-CRITICA-DATA SECTION.                                              
      *                                                                         
      * ENTRADA=WS-DATA-SEC.                                                    
      * SAIDA  =WS-ERRO-DATA =0 (DATA OK)      =1 DATA ERRADA                   
      *                                                                         
           MOVE 0 TO WS-ERRO-DATA.                                              
      *                                                                         
           IF WS-DATA-SEC  NOT NUMERIC                                          
              MOVE  1  TO  WS-ERRO-DATA                                         
           ELSE                                                                 
              IF WS-MM-DATA-SEC     GREATER 12  OR                              
                 WS-MM-DATA-SEC     EQUAL    0  OR                              
                 WS-DD-DATA-SEC     EQUAL    0  OR                              
                 WS-SS-DATA-SEC     EQUAL    0                                  
                 MOVE  1  TO  WS-ERRO-DATA                                      
              ELSE                                                              
                IF WS-MM-DATA-SEC     EQUAL   04  OR                            
                   WS-MM-DATA-SEC     EQUAL   06  OR                            
                   WS-MM-DATA-SEC     EQUAL   09  OR                            
                   WS-MM-DATA-SEC     EQUAL   11                                
                   IF WS-DD-DATA-SEC  GREATER 30                                
                      MOVE  1  TO  WS-ERRO-DATA                                 
                ELSE                                                            
                   NEXT SENTENCE                                                
           ELSE                                                                 
           IF WS-MM-DATA-SEC  EQUAL  02                                         
              DIVIDE WS-AA-DATA-SEC BY 4  GIVING  WS-AUX                        
                                        REMAINDER WS-RESTO                      
              IF WS-RESTO                   GREATER 0                           
                 IF WS-DD-DATA-SEC             GREATER 28                       
                    MOVE  1  TO  WS-ERRO-DATA                                   
                 ELSE                                                           
                    NEXT SENTENCE                                               
              ELSE                                                              
                 IF WS-DD-DATA-SEC  GREATER  29                                 
                    MOVE  1  TO  WS-ERRO-DATA                                   
                 ELSE                                                           
                    NEXT SENTENCE                                               
           ELSE                                                                 
              IF WS-DD-DATA-SEC  GREATER  31                                    
                 MOVE  1  TO  WS-ERRO-DATA.                                     
      *                                                                         
       R1055-SAIDA. EXIT.                                                       
      *----------------------------------------------------------------*        
      *-----------------------------------------------------------------        
       R1100-TRATAR-TELEF-FAX SECTION.                                          
      *                                                                         
           IF WS-NUM-TELEF-ENT NUMERIC                                          
              MOVE WS-DDD-ENT      TO  WS-DDD-SAI                               
              MOVE WS-TELEFONE-ENT TO  WS-TELEFONE-SAI                          
              GO TO R1100-SAIDA.                                                
                                                                                
           MOVE  ZEROS  TO  WS-NUM-TELEF-SAI.                                   
                                                                                
           IF WS-DDD-ENT NUMERIC                                                
              MOVE WS-DDD-ENT      TO  WS-DDD-SAI                               
              MOVE 7               TO  WTAM-TEL                                 
              GO TO R1100-MONTA-TELEF.                                          
                                                                                
******** MOVIMENTAR  DDD DA ENTRADA PARA SAIDA  ***********                     
                                                                                
           MOVE 4 TO WIND1  WIND2.                                              
      *                                                                         
           PERFORM R1110-MOVIMENTA-TEL WIND2 TIMES.                             
                                                                                
           MOVE 8               TO  WTAM-TEL.                                   
      *                                                                         
           IF  WS-NUM-TELEF-ENT(5:8) NUMERIC                                    
               MOVE WS-NUM-TELEF-ENT(5:8) TO WS-TELEFONE-SAI                    
               GO TO R1100-SAIDA.                                               
                                                                                
******** MOVIMENTAR TELEF/FAX DA ENTRADA PARA SAIDA  ***********                
                                                                                
       R1100-MONTA-TELEF.                                                       
                                                                                
           MOVE 12 TO WIND1  WIND2.                                             
      *                                                                         
           PERFORM R1110-MOVIMENTA-TEL WTAM-TEL TIMES.                          
                                                                                
       R1100-SAIDA. EXIT.                                                       
      *-----------------------------------------------------------------        
       R1110-MOVIMENTA-TEL  SECTION.                                            
      *                                                                         
           IF WS-NUM-TELEF-ENT(WIND1:1) NUMERIC                                 
              MOVE WS-NUM-TELEF-ENT(WIND1:1) TO                                 
                                   WS-NUM-TELEF-SAI(WIND2:1)                    
              COMPUTE WIND2 = WIND2 - 1.                                        
                                                                                
           COMPUTE WIND1 = WIND1 - 1.                                           
                                                                                
       R1110-SAIDA. EXIT.                                                       
      *-----------------------------------------------------------------        
       R5000-SELECT-MAX-CONTA                                   SECTION.        
      *-----------------------------------------------------------------        
      *                                                                         
           MOVE  '0002'  TO  WNR-EXEC-SQL.                                      
                                                                                
           EXEC SQL                                                             
             SELECT  NUM_SEQ                                                    
               INTO :FCSEQUEN-NUM-SEQ                                           
               FROM  FDRCAP.FC_SEQUENCE                                         
              WHERE  COD_SEQ   =   'COB'                                        
           END-EXEC.                                                            
                                                                                
           IF SQLCODE NOT EQUAL ZEROS                                           
              DISPLAY 'ERRO SELECT FC_SEQUENCE '                                
              GO  TO  R9999-ROT-ERRO.                                           
                                                                                
            DISPLAY 'R5000-MAX IDE-CONTA-BAN='  FCSEQUEN-NUM-SEQ                
                                                                                
            MOVE FCSEQUEN-NUM-SEQ  TO MAX-IDE-CONTA-BANCARIA.                   
      *                                                                         
       R5000-EXIT.  EXIT.                                                       
      *-----------------------------------------------------------------        
       R5100-UPDATE-MAX-CONTA   SECTION.                                        
      *-----------------------------------                                      
      *                                                                         
           MOVE  '0002'  TO  WNR-EXEC-SQL.                                      
      *                                                                         
           MOVE MAX-IDE-CONTA-BANCARIA  TO  FCSEQUEN-NUM-SEQ                    
                                                                                
           EXEC SQL                                                             
             UPDATE  FDRCAP.FC_SEQUENCE                                         
                SET  NUM_SEQ  = :FCSEQUEN-NUM-SEQ                               
              WHERE  COD_SEQ   =   'COB'                                        
           END-EXEC.                                                            
      *                                                                         
           IF SQLCODE NOT EQUAL ZEROS                                           
              DISPLAY 'ERRO SELECT FC_SEQUENCE '                                
              GO  TO  R9999-ROT-ERRO.                                           
      *                                                                         
       R5100-EXIT.  EXIT.                                                       
      *-----------------------------------------------------------------        
                                                                                
       R6000-VER-ALTERACAO-LOTERICO  SECTION.                                   
      *---------------------------------------                                  
           MOVE SPACES TO LD00-MSG1.                                            
      *                                                                         
           MOVE  'NAO' TO W-CHAVE-HOUVE-ALTERACAO                               
                          W-CHAVE-ALTEROU-SEGURADO                              
                                                                                
           MOVE  'N'  TO LTMVPROP-IND-ALT-DADOS-PES                             
                         LTMVPROP-IND-ALT-ENDER                                 
                         LTMVPROP-IND-ALT-COBER                                 
                         LTMVPROP-IND-ALT-BONUS.                                
      *                                                                         
           MOVE  'N'  TO W-IND-ALT-FAXTELEME.                                   
      *                                                                         
           MOVE 3    TO LTMVPROP-COD-MOVIMENTO.                                 
      *                                                                         
           IF CAD-CGCX          NOT EQUAL FCLOTERI-COD-CGC          OR          
              CAD-NOME-FANTASIA NOT EQUAL FCLOTERI-NOM-FANTASIA     OR          
              CAD-RAZAO-SOCIAL  NOT EQUAL FCLOTERI-NOM-RAZAO-SOCIAL             
##    *       DISPLAY 'ALTER 1 = ' CAD-CODIGO-CEF                               
              MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO                             
              MOVE  'S'  TO LTMVPROP-IND-ALT-DADOS-PES.                         
                                                                                
           IF CAD-EN-SUB        NOT EQUAL FCLOTERI-NUM-ENCEF        OR          
              CAD-NUM-LOT-ANTERIOR NOT EQUAL FCLOTERI-NUM-LOTER-ANT OR          
              CAD-DV-CEF        NOT EQUAL FCLOTERI-NUM-DV-LOTERICO  OR          
              CAD-PV-SUB        NOT EQUAL FCLOTERI-NUM-PVCEF                    
##    *       DISPLAY 'ALTER 2 = ' CAD-CODIGO-CEF                               
              MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO                             
              MOVE  'S'  TO LTMVPROP-IND-ALT-DADOS-PES.                         
                                                                                
           MOVE FCLOTERI-NOM-BAIRRO    TO WS-NOME-BAIRRO.                       
           MOVE FCLOTERI-NOM-MUNICIPIO TO WS-NOME-CIDADE.                       
                                                                                
           IF CAD-UF        NOT EQUAL FCLOTERI-COD-UF          OR               
              CAD-EMAIL     NOT EQUAL FCLOTERI-DES-EMAIL       OR               
              CAD-ENDERECO  NOT EQUAL FCLOTERI-DES-ENDERECO    OR               
              CAD-BAIRRO    NOT EQUAL WS-BAIRRO                OR               
              CAD-CIDADE    NOT EQUAL WS-CIDADE                OR               
              CAD-CEP       NOT EQUAL FCLOTERI-NUM-CEP                          
##    *       DISPLAY 'ALTER 3 = ' CAD-CODIGO-CEF                               
              MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO                             
              MOVE  'S'  TO LTMVPROP-IND-ALT-ENDER.                             
                                                                                
                                                                                
OL1801*    IF CAD-EMAIL     NOT EQUAL FCLOTERI-DES-EMAIL                        
      *       DISPLAY 'ALTER 3 = ' CAD-CODIGO-CEF                               
      *       MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO                             
OL1801*****   MOVE  'S'  TO LTMVPROP-IND-ALT-ENDER                              
      *       MOVE  'S'  TO W-IND-ALT-FAXTELEME.                                
                                                                                
                                                                                
           MOVE FCLOTERI-NUM-TELEFONE TO WS-NUM-TELEF-SAI                       
           MOVE FCLOTERI-NUM-FAX      TO WS-NUMERO-FAX.                         
                                                                                
                                                                                
           IF CAD-TELEFONE   NOT EQUAL WS-NUM-TELEF-SAI                         
ALT-K1*       CAD-NUMERO-FAX NOT EQUAL WS-NUMER-FAX                             
##    *       DISPLAY 'ALTER 4 = ' CAD-CODIGO-CEF                               
##    *       DISPLAY 'CAD.TEL = ' CAD-TELEFONE                                 
##    *               ' FC_LOT = ' WS-NUM-TELEF-SAI                             
##    *       DISPLAY 'CAD.FAX = ' CAD-NUMERO-FAX                               
##    *               ' FC_FAX = ' WS-NUMER-FAX                                 
              MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO                             
              MOVE  'S'  TO LTMVPROP-IND-ALT-ENDER .                            
OL0805*       MOVE  'S'  TO W-IND-ALT-FAXTELEME.                                
                                                                                
                                                                                
OL1801     IF CAD-NUMERO-FAX NOT EQUAL WS-NUMER-FAX        AND                  
              CAD-FAX        NOT EQUAL 99999999                                 
              DISPLAY 'ALTER 4 = ' CAD-CODIGO-CEF                               
              DISPLAY 'CAD.FAX = ' CAD-NUMERO-FAX                               
                      ' FC_FAX = ' WS-NUMER-FAX                                 
              MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO                             
              MOVE  'S'  TO LTMVPROP-IND-ALT-ENDER.                             
OL1801******* MOVE  'S'  TO W-IND-ALT-FAXTELEME.                                
                                                                                
                                                                                
           IF CAD-VALOR-GARANTIA  NOT EQUAL FCLOTERI-VLR-GARANTIA               
##    *       DISPLAY 'ALTER 5 = ' CAD-CODIGO-CEF                               
              MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO                             
              MOVE  'S'  TO LTMVPROP-IND-ALT-COBER.                             
      *                                                                         
           IF CAD-COD-MUNICIPIO  NOT = FCLOTERI-COD-MUNICIPIO     OR            
              CAD-INSC-MUNX      NOT = FCLOTERI-COD-INSCR-MUNIC   OR            
              CAD-INSC-ESTX      NOT = FCLOTERI-COD-INSCR-ESTAD   OR            
              CAD-SITUACAOX      NOT = FCLOTERI-STA-LOTERICO                    
##    *       DISPLAY 'ALTER 6 = ' CAD-CODIGO-CEF                               
              MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO.                            
                                                                                
           IF CAD-COD-AG-MASTERX NOT = FCLOTERI-COD-AGENTE-MASTER OR            
              CAD-CAT-LOTERICO   NOT = FCLOTERI-IND-CAT-LOTERICO  OR            
              CAD-COD-STATUS     NOT = FCLOTERI-IND-STA-LOTERICO  OR            
              CAD-UNIDADE-SUB    NOT = FCLOTERI-IND-UNIDADE-SUB   OR            
              W-CAD-DATA-INI-VIG NOT = FCLOTERI-DTH-INCLUSAO                    
##    *       DISPLAY 'ALTER 7 = ' CAD-CODIGO-CEF                               
              MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO.                            
                                                                                
           IF CAD-TIPO-GARANTIA  NOT = FCLOTERI-COD-GARANTIA      OR            
              CAD-MATR-CONSULTOR NOT = FCLOTERI-NUM-MATR-CONSULTOR OR           
              CAD-NOME-CONSULTOR NOT = FCLOTERI-NOM-CONSULTOR     OR            
              CAD-CONTATO1       NOT = FCLOTERI-NOM-CONTATO1      OR            
              CAD-CONTATO2       NOT = FCLOTERI-NOM-CONTATO2      OR            
              CAD-NIVEL-COMISSAOX NOT = FCLOTERI-STA-NIVEL-COMIS                
##    *       DISPLAY 'ALTER 8 = ' CAD-CODIGO-CEF                               
              MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO.                            
                                                                                
           IF W-CAD-DATA-EXCLUSAO NOT = SPACES AND                              
              W-CAD-DATA-EXCLUSAO NOT = FCLOTERI-DTH-EXCLUSAO                   
##    *       DISPLAY 'ALTER 9 = ' CAD-CODIGO-CEF                               
              MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO.                            
                                                                                
           IF CAD-NUM-SEGURADORA  NOT = FCLOTERI-NUM-SEGURADORA                 
              MOVE 'SIM' TO W-CHAVE-HOUVE-ALTERACAO.                            
                                                                                
      *    VERIFICAR EXCLUSAO DO SEGURADO (GERA CANCELAMENTO DO SEGURO)         
      *                                                                         
           IF W-CHAVE-CADASTRADO-SASSE  NOT =  'SIM'                            
              GO TO R6000-PULA.                                                 
                                                                                
           IF CAD-SITUACAO EQUAL '0'                                            
              MOVE  7    TO LTMVPROP-COD-MOVIMENTO                              
              MOVE  'SIM' TO W-CHAVE-ALTEROU-SEGURADO                           
              GO TO R6000-SAIDA.                                                
                                                                                
           IF CAD-TIPO-GARANTIA  NOT = 'S'                                      
              MOVE  7    TO LTMVPROP-COD-MOVIMENTO                              
              MOVE  'SIM' TO W-CHAVE-ALTEROU-SEGURADO                           
              GO TO R6000-SAIDA.                                                
                                                                                
           IF CAD-NUM-SEGURADORA   NOT EQUAL  001                               
              MOVE  7    TO LTMVPROP-COD-MOVIMENTO                              
              MOVE  'SIM' TO W-CHAVE-ALTEROU-SEGURADO                           
              GO TO R6000-SAIDA.                                                
                                                                                
           IF LTMVPROP-IND-ALT-DADOS-PES = 'S'   OR                             
              LTMVPROP-IND-ALT-ENDER     = 'S'   OR                             
              W-IND-ALT-FAXTELEME        = 'S'   OR                             
              LTMVPROP-IND-ALT-COBER     = 'S'   OR                             
              LTMVPROP-IND-ALT-BONUS     = 'S'                                  
              MOVE  'SIM' TO W-CHAVE-ALTEROU-SEGURADO.                          
      *                                                                         
       R6000-PULA.                                                              
           IF WS-NECESSARIO = 1                                                 
              MOVE  'NAO' TO W-CHAVE-ALTEROU-SEGURADO.                          
                                                                                
       R6000-SAIDA. EXIT.                                                       
      *                                                                         
       R6020-SELECT-FC-LOTERICO SECTION.                                        
      *------------------------------------                                     
      *                                                                         
           MOVE  '6020'  TO  WNR-EXEC-SQL.                                      
      *                                                                         
           MOVE  CAD-COD-CEF    TO  FCLOTERI-NUM-LOTERICO.                      
           MOVE  'NAO'          TO  W-CHAVE-CADASTRADO-SIGEL.                   
      *                                                                         
           EXEC SQL                                                             
             SELECT  VALUE(COD_AGENTE_MASTER,' '),                              
                     VALUE(COD_CGC,'000000000000000'),                          
                     VALUE(COD_INSCR_ESTAD,' '),                                
                     VALUE(COD_INSCR_MUNIC,' '),                                
                     VALUE(COD_MUNICIPIO,' '),                                  
                     VALUE(COD_UF,' '),                                         
                     VALUE(DES_EMAIL,' '),                                      
                     VALUE(DES_ENDERECO,' '),                                   
                     VALUE(DTH_EXCLUSAO,DATE('9999-12-31')),                    
                     VALUE(DTH_INCLUSAO,DATE('9999-12-31')),                    
                     VALUE(IDE_CONTA_CAUCAO,0),                                 
                     VALUE(IDE_CONTA_CPMF,0),                                   
                     VALUE(IDE_CONTA_ISENTA,0),                                 
                     VALUE(IND_CAT_LOTERICO,0),                                 
                     VALUE(IND_STA_LOTERICO,0),                                 
                     VALUE(IND_UNIDADE_SUB,0),                                  
                     VALUE(NOM_BAIRRO,' '),                                     
                     VALUE(NOM_CONSULTOR,' '),                                  
                     VALUE(NOM_CONTATO1,' '),                                   
                     VALUE(NOM_CONTATO2,' '),                                   
                     VALUE(NOM_FANTASIA,' '),                                   
                     VALUE(NOM_MUNICIPIO,' '),                                  
                     VALUE(NOM_RAZAO_SOCIAL,' '),                               
                     VALUE(NUM_CEP,0),                                          
                     VALUE(NUM_ENCEF,0),                                        
                     VALUE(NUM_LOTER_ANT,0),                                    
                     VALUE(NUM_MATR_CONSULTOR,0),                               
                     VALUE(NUM_PVCEF,0),                                        
                     VALUE(NUM_TELEFONE,'000000000000'),                        
                     VALUE(STA_DADOS_N_CADAST,' '),                             
                     VALUE(STA_LOTERICO,' '),                                   
                     VALUE(STA_NIVEL_COMIS,' '),                                
                     VALUE(STA_ULT_ALT_ONLINE,' '),                             
                     VALUE(COD_GARANTIA,' '),                                   
                     VALUE(VLR_GARANTIA,0),                                     
                     VALUE(DTH_GERACAO,DATE('9999-12-31')),                     
                     VALUE(NUM_FAX,'000000000000'),                             
                     VALUE(NUM_DV_LOTERICO,0),                                  
                     VALUE(NUM_SEGURADORA,0)                                    
               INTO :FCLOTERI-COD-AGENTE-MASTER ,                               
                    :FCLOTERI-COD-CGC           ,                               
                    :FCLOTERI-COD-INSCR-ESTAD   ,                               
                    :FCLOTERI-COD-INSCR-MUNIC   ,                               
                    :FCLOTERI-COD-MUNICIPIO     ,                               
                    :FCLOTERI-COD-UF            ,                               
                    :FCLOTERI-DES-EMAIL         ,                               
                    :FCLOTERI-DES-ENDERECO      ,                               
                    :FCLOTERI-DTH-EXCLUSAO      ,                               
                    :FCLOTERI-DTH-INCLUSAO      ,                               
                    :FCLOTERI-IDE-CONTA-CAUCAO  ,                               
                    :FCLOTERI-IDE-CONTA-CPMF    ,                               
                    :FCLOTERI-IDE-CONTA-ISENTA  ,                               
                    :FCLOTERI-IND-CAT-LOTERICO  ,                               
                    :FCLOTERI-IND-STA-LOTERICO  ,                               
                    :FCLOTERI-IND-UNIDADE-SUB   ,                               
                    :FCLOTERI-NOM-BAIRRO        ,                               
                    :FCLOTERI-NOM-CONSULTOR     ,                               
                    :FCLOTERI-NOM-CONTATO1      ,                               
                    :FCLOTERI-NOM-CONTATO2      ,                               
                    :FCLOTERI-NOM-FANTASIA      ,                               
                    :FCLOTERI-NOM-MUNICIPIO     ,                               
                    :FCLOTERI-NOM-RAZAO-SOCIAL  ,                               
                    :FCLOTERI-NUM-CEP           ,                               
                    :FCLOTERI-NUM-ENCEF         ,                               
                    :FCLOTERI-NUM-LOTER-ANT     ,                               
                    :FCLOTERI-NUM-MATR-CONSULTOR,                               
                    :FCLOTERI-NUM-PVCEF         ,                               
                    :FCLOTERI-NUM-TELEFONE      ,                               
                    :FCLOTERI-STA-DADOS-N-CADAST,                               
                    :FCLOTERI-STA-LOTERICO      ,                               
                    :FCLOTERI-STA-NIVEL-COMIS   ,                               
                    :FCLOTERI-STA-ULT-ALT-ONLINE,                               
                    :FCLOTERI-COD-GARANTIA      ,                               
                    :FCLOTERI-VLR-GARANTIA      ,                               
                    :FCLOTERI-DTH-GERACAO       ,                               
                    :FCLOTERI-NUM-FAX           ,                               
                    :FCLOTERI-NUM-DV-LOTERICO   ,                               
                    :FCLOTERI-NUM-SEGURADORA                                    
               FROM  FDRCAP.FC_LOTERICO                                         
             WHERE  NUM_LOTERICO   =  :FCLOTERI-NUM-LOTERICO                    
           END-EXEC.                                                            
      *                                                                         
      *                                                                         
           IF SQLCODE  EQUAL  ZEROS                                             
              MOVE  'SIM'  TO  W-CHAVE-CADASTRADO-SIGEL                         
           ELSE                                                                 
              IF SQLCODE  NOT EQUAL  100                                        
                 DISPLAY ' 6020-ERRO SELECT FC_LOTERICO SIGEL ='                
                 DISPLAY ' NUM-LOTERICO=' FCLOTERI-NUM-LOTERICO                 
                 GO  TO  R9999-ROT-ERRO.                                        
      *                                                                         
       R6020-SAIDA.  EXIT.                                                      
      *-----------------------------------------------------------------        
       R6030-SELECT-V0LOTERICO01                                SECTION.        
      *-----------------------------------------------------------------        
      *                                                                         
           MOVE  '6030'  TO  WNR-EXEC-SQL.                                      
                                                                                
           MOVE  CAD-CODIGO-CEF  TO  V0LOT-COD-LOT-CEF.                         
                                                                                
           MOVE  'NAO'          TO  W-CHAVE-CADASTRADO-SASSE                    
                                                                                
           EXEC SQL                                                             
             SELECT  COD_LOT_FENAL                                              
               INTO :V0LOT-COD-LOT-FENAL                                        
               FROM  SEGUROS.V0LOTERICO01                                       
             WHERE  NUM_APOLICE    =  :WS-NUM-APOLICE                           
             AND    COD_LOT_FENAL  =  :V0LOT-COD-LOT-CEF                        
             AND    SITUACAO       <  '1'                                       
           END-EXEC.                                                            
                                                                                
           IF SQLCODE  EQUAL  ZEROS                                             
      *****   MOVE 'LOTERICO JA E SEGURADO' TO LD00-MSG1                        
      *****   PERFORM  R7600-IMPRIME-LD00-MSG1                                  
              MOVE  'SIM'  TO  W-CHAVE-CADASTRADO-SASSE                         
              ADD 1 TO WS-MVPROP-TOTAL-SEGURADOS                                
           ELSE                                                                 
              IF SQLCODE  EQUAL  100                                            
                 ADD 1 TO WS-MVPROP-TOTAL-NSEGURADOS                            
              ELSE                                                              
                 DISPLAY 'R6030-ERRO DE SELECT V0LOTERICO01 '                   
                 DISPLAY 'COD-CEF    = ' V0LOT-COD-LOT-CEF                      
                 GO  TO  R9999-ROT-ERRO.                                        
      *                                                                         
           IF W-CHAVE-CADASTRADO-SIGEL = 'NAO'                                  
              IF W-CHAVE-CADASTRADO-SASSE = 'SIM'                               
              DISPLAY ' LOTERICO SEGURADO SEM CADASTRO NO SIGEL = '             
              DISPLAY ' NUM-LOTERICO=' FCLOTERI-NUM-LOTERICO                    
              GO  TO  R9999-ROT-ERRO.                                           
      *                                                                         
       R6030-SAIDA.  EXIT.                                                      
      *-----------------------------------------------------------------        
       R6060-VER-ALTERACAO-FC-CONTA                             SECTION.        
      *-----------------------------------------------------------------        
           MOVE  '0008'  TO  WNR-EXEC-SQL.                                      
                                                                                
           MOVE ZEROS    TO WS-IDE-CONTA-CPMF                                   
                            WS-IDE-CONTA-ISENTA                                 
                            WS-IDE-CONTA-CAUCAO.                                
                                                                                
           IF CAD-BANCO-DESC-CPMF  EQUAL ZEROS OR                               
              CAD-AGEN-DESC-CPMF   EQUAL ZEROS OR                               
              CAD-CONTA-DESC-CPMF  EQUAL ZEROS                                  
              GO   TO     R6060-CONTA-ISENTA                                    
           END-IF                                                               
                                                                                
           MOVE FCLOTERI-IDE-CONTA-CPMF  TO FCCONBAN-IDE-CONTA-BANCARIA         
                                            WS-IDE-CONTA-CPMF.                  
           PERFORM R6070-LER-CONTA-BANCARIA.                                    
                                                                                
           MOVE  CAD-CONTA-DESC-CPMF  TO WS1-CONTA-BANCARIA                     
V.01  *    IF CAD-BANCO-DESC-CPMF   EQUAL WS-CONBAN-BANCO-N    AND              
V.01  *       CAD-AGEN-DESC-CPMF    EQUAL WS-CONBAN-AGENCIA-N  AND              
V.01  *       WS1-OPERACAO-CONTA    EQUAL WS-CONBAN-OP-N       AND              
V.01  *       WS1-NUMERO-CONTA      EQUAL WS-CONBAN-CONTA-N    AND              
V.01  *       WS1-DV-CONTA          EQUAL WS-CONBAN-DV-N                        
V.01       IF CAD-BANCO-DESC-CPMF   EQUAL WS-CONBAN-BANCO      AND              
V.01          CAD-AGEN-DESC-CPMF    EQUAL WS-CONBAN-AGENCIA    AND              
V.01          WS1-OPERACAO-CONTA    EQUAL WS-CONBAN-OP         AND              
V.01          WS1-NUMERO-CONTA      EQUAL WS-CONBAN-CONTA      AND              
V.01          WS1-DV-CONTA          EQUAL WS-CONBAN-DV                          
              GO TO R6060-CONTA-ISENTA                                          
           END-IF                                                               
                                                                                
           IF W-CHAVE-CADASTRADO-SASSE EQUAL 'SIM' AND                          
              WS-NECESSARIO = 0                                                 
              MOVE 'SIM'             TO  W-CHAVE-ALTEROU-SEGURADO               
              MOVE 'S'               TO  LTMVPROP-IND-ALT-DADOS-PES             
           END-IF                                                               
                                                                                
           MOVE 'SIM'             TO  W-CHAVE-HOUVE-ALTERACAO                   
           MOVE CAD-BANCO-DESC-CPMF   TO WS-CODIGO-BANCO                        
           MOVE CAD-AGEN-DESC-CPMF    TO WS-CODIGO-AGENCIA                      
           MOVE CAD-CONTA-DESC-CPMF   TO WS-CONTA-BANCARIA                      
V.01                                     WS-CONTA-BANCO                         
V.02  *    MOVE WS-NUMERO-CONTA       TO WS-CONTA-10POS.                        
V.02       MOVE WS-NUMERO-CONTA       TO WS-CONTA-12POS.                        
                                                                                
V.01  *    MOVE WS-COD-BANCO      TO  FCCONBAN-COD-BANCO                        
V.01       MOVE WS-CODIGO-BANCO   TO  FCCONBAN-COD-BANCO                        
V.01  *    MOVE WS-COD-AGENCIA    TO  FCCONBAN-COD-AGENCIA                      
V.01       MOVE WS-CODIGO-AGENCIA TO  FCCONBAN-COD-AGENCIA                      
V.02  *    MOVE WS-CONTA-10POS    TO  FCCONBAN-COD-CONTA                        
V.02       MOVE WS-CONTA-12POS    TO  FCCONBAN-COD-CONTA                        
V.01  *    MOVE WS-OPERACAO-CONTA TO  FCCONBAN-COD-OP-CONTA                     
V.01       MOVE WS-OPERA-CONTA    TO  FCCONBAN-COD-OP-CONTA                     
           MOVE WS-DV-CONTA       TO  FCCONBAN-COD-DV-CONTA                     
           PERFORM R6080-VER-CONTA-EXISTENTE.                                   
                                                                                
           IF   FCCONBAN-IDE-CONTA-BANCARIA > ZEROS                             
                MOVE FCCONBAN-IDE-CONTA-BANCARIA TO WS-IDE-CONTA-CPMF           
           ELSE                                                                 
                PERFORM R6230-INSERT-FC-CONTA                                   
                MOVE  MAX-IDE-CONTA-BANCARIA     TO WS-IDE-CONTA-CPMF           
           END-IF                                                               
           .                                                                    
       R6060-CONTA-ISENTA.                                                      
      *                                                                         
           IF CAD-BANCO-ISENTA     EQUAL ZEROS OR                               
              CAD-AGEN-ISENTA      EQUAL ZEROS OR                               
              CAD-CONTA-ISENTA     EQUAL ZEROS                                  
              GO TO R6060-CONTA-CAUCAO                                          
           END-IF                                                               
                                                                                
           MOVE FCLOTERI-IDE-CONTA-ISENTA TO FCCONBAN-IDE-CONTA-BANCARIA        
                                             WS-IDE-CONTA-ISENTA.               
           PERFORM R6070-LER-CONTA-BANCARIA.                                    
                                                                                
           MOVE  CAD-CONTA-ISENTA     TO WS1-CONTA-BANCARIA                     
V.01  *    IF CAD-BANCO-ISENTA      EQUAL WS-CONBAN-BANCO-N    AND              
V.01  *       CAD-AGEN-ISENTA       EQUAL WS-CONBAN-AGENCIA-N  AND              
V.01  *       WS1-OPERACAO-CONTA    EQUAL WS-CONBAN-OP-N       AND              
V.01  *       WS1-NUMERO-CONTA      EQUAL WS-CONBAN-CONTA-N    AND              
V.01  *       WS1-DV-CONTA          EQUAL WS-CONBAN-DV-N                        
V.01       IF CAD-BANCO-ISENTA      EQUAL WS-CONBAN-BANCO      AND              
V.01          CAD-AGEN-ISENTA       EQUAL WS-CONBAN-AGENCIA    AND              
V.01          WS1-OPERACAO-CONTA    EQUAL WS-CONBAN-OP         AND              
V.01          WS1-NUMERO-CONTA      EQUAL WS-CONBAN-CONTA      AND              
V.01          WS1-DV-CONTA          EQUAL WS-CONBAN-DV                          
              GO TO  R6060-CONTA-CAUCAO                                         
           END-IF                                                               
                                                                                
           MOVE 'SIM'                 TO  W-CHAVE-HOUVE-ALTERACAO               
           MOVE CAD-BANCO-ISENTA      TO WS-CODIGO-BANCO                        
           MOVE CAD-AGEN-ISENTA       TO WS-CODIGO-AGENCIA                      
           MOVE CAD-CONTA-ISENTA      TO WS-CONTA-BANCARIA                      
V.01                                     WS-CONTA-BANCO                         
V.02  *    MOVE WS-NUMERO-CONTA       TO WS-CONTA-10POS.                        
V.02       MOVE WS-NUMERO-CONTA       TO WS-CONTA-12POS.                        
                                                                                
V.01  *    MOVE WS-COD-BANCO      TO  FCCONBAN-COD-BANCO                        
V.01       MOVE WS-CODIGO-BANCO   TO  FCCONBAN-COD-BANCO                        
V.01  *    MOVE WS-COD-AGENCIA    TO  FCCONBAN-COD-AGENCIA                      
V.01       MOVE WS-CODIGO-AGENCIA TO  FCCONBAN-COD-AGENCIA                      
V.02  *    MOVE WS-CONTA-10POS    TO  FCCONBAN-COD-CONTA                        
V.02       MOVE WS-CONTA-12POS    TO  FCCONBAN-COD-CONTA                        
V.01  *    MOVE WS-OPERACAO-CONTA TO  FCCONBAN-COD-OP-CONTA                     
V.01       MOVE WS-OPERA-CONTA    TO  FCCONBAN-COD-OP-CONTA                     
           MOVE WS-DV-CONTA       TO  FCCONBAN-COD-DV-CONTA                     
           PERFORM R6080-VER-CONTA-EXISTENTE.                                   
                                                                                
           IF   FCCONBAN-IDE-CONTA-BANCARIA > ZEROS                             
                MOVE FCCONBAN-IDE-CONTA-BANCARIA TO WS-IDE-CONTA-ISENTA         
           ELSE                                                                 
                PERFORM R6230-INSERT-FC-CONTA                                   
                MOVE  MAX-IDE-CONTA-BANCARIA     TO WS-IDE-CONTA-ISENTA         
           END-IF                                                               
           .                                                                    
       R6060-CONTA-CAUCAO.                                                      
      *                                                                         
           IF CAD-BANCO-CAUCAO     EQUAL ZEROS OR                               
              CAD-AGEN-CAUCAO      EQUAL ZEROS OR                               
              CAD-CONTA-CAUCAO     EQUAL ZEROS                                  
              GO TO R6060-SAIDA.                                                
                                                                                
           MOVE FCLOTERI-IDE-CONTA-CAUCAO TO FCCONBAN-IDE-CONTA-BANCARIA        
                                            WS-IDE-CONTA-CAUCAO.                
           PERFORM R6070-LER-CONTA-BANCARIA.                                    
                                                                                
           MOVE CAD-CONTA-CAUCAO     TO WS1-CONTA-BANCARIA                      
V.01  *    IF CAD-BANCO-CAUCAO      EQUAL WS-CONBAN-BANCO-N    AND              
V.01  *       CAD-AGEN-CAUCAO       EQUAL WS-CONBAN-AGENCIA-N  AND              
V.01  *       WS1-OPERACAO-CONTA    EQUAL WS-CONBAN-OP-N       AND              
V.01  *       WS1-NUMERO-CONTA      EQUAL WS-CONBAN-CONTA-N    AND              
V.01  *       WS1-DV-CONTA          EQUAL WS-CONBAN-DV-N                        
V.01       IF CAD-BANCO-CAUCAO      EQUAL WS-CONBAN-BANCO      AND              
V.01          CAD-AGEN-CAUCAO       EQUAL WS-CONBAN-AGENCIA    AND              
V.01          WS1-OPERACAO-CONTA    EQUAL WS-CONBAN-OP         AND              
V.01          WS1-NUMERO-CONTA      EQUAL WS-CONBAN-CONTA      AND              
V.01          WS1-DV-CONTA          EQUAL WS-CONBAN-DV                          
              GO         TO       R6060-SAIDA                                   
           END-IF                                                               
                                                                                
           MOVE CAD-BANCO-CAUCAO      TO WS-CODIGO-BANCO                        
           MOVE CAD-AGEN-CAUCAO       TO WS-CODIGO-AGENCIA                      
           MOVE CAD-CONTA-CAUCAO      TO WS-CONTA-BANCARIA                      
V.01                                     WS-CONTA-BANCO                         
V.02  *    MOVE WS-NUMERO-CONTA       TO WS-CONTA-10POS.                        
V.02       MOVE WS-NUMERO-CONTA       TO WS-CONTA-12POS.                        
                                                                                
           MOVE 'SIM'             TO  W-CHAVE-HOUVE-ALTERACAO                   
V.01  *    MOVE WS-COD-BANCO      TO  FCCONBAN-COD-BANCO                        
V.01       MOVE WS-CODIGO-BANCO   TO  FCCONBAN-COD-BANCO                        
V.01  *    MOVE WS-COD-AGENCIA    TO  FCCONBAN-COD-AGENCIA                      
V.01       MOVE WS-CODIGO-AGENCIA TO  FCCONBAN-COD-AGENCIA                      
V.02  *    MOVE WS-CONTA-10POS    TO  FCCONBAN-COD-CONTA                        
V.02       MOVE WS-CONTA-12POS    TO  FCCONBAN-COD-CONTA                        
V.01  *    MOVE WS-OPERACAO-CONTA TO  FCCONBAN-COD-OP-CONTA                     
V.01       MOVE WS-OPERA-CONTA    TO  FCCONBAN-COD-OP-CONTA                     
           MOVE WS-DV-CONTA       TO  FCCONBAN-COD-DV-CONTA                     
           PERFORM R6080-VER-CONTA-EXISTENTE.                                   
                                                                                
           IF   FCCONBAN-IDE-CONTA-BANCARIA > ZEROS                             
                MOVE FCCONBAN-IDE-CONTA-BANCARIA TO WS-IDE-CONTA-CAUCAO         
           ELSE                                                                 
                PERFORM R6230-INSERT-FC-CONTA                                   
                MOVE  MAX-IDE-CONTA-BANCARIA     TO WS-IDE-CONTA-CAUCAO         
           END-IF                                                               
           .                                                                    
       R6060-SAIDA. EXIT.                                                       
      *                                                                         
      *-----------------------------------------------------------------        
       R6070-LER-CONTA-BANCARIA                                 SECTION.        
      *-----------------------------------------------------------------        
      *                                                                         
V.01  *    MOVE SPACES                 TO     WS-CONBAN-BANCO                   
V.01       MOVE ZEROS                  TO     WS-CONBAN-BANCO                   
                                              WS-CONBAN-AGENCIA                 
                                              WS-CONBAN-OP                      
                                              WS-CONBAN-CONTA.                  
V.01  *                                       WS-CONBAN-DV .                    
V.01       MOVE SPACES                 TO     WS-CONBAN-DV .                    
                                                                                
           IF FCCONBAN-IDE-CONTA-BANCARIA = ZEROS                               
              MOVE ZEROS TO FCCONBAN-COD-AGENCIA                                
                            FCCONBAN-COD-BANCO                                  
                            FCCONBAN-COD-CONTA                                  
                            FCCONBAN-COD-DV-CONTA                               
                            FCCONBAN-COD-OP-CONTA                               
              GO TO R6070-SAIDA.                                                
                                                                                
           EXEC SQL                                                             
             SELECT                                                             
                    IDE_CONTA_BANCARIA,                                         
                    COD_AGENCIA,                                                
                    COD_BANCO,                                                  
                    COD_CONTA,                                                  
                    COD_DV_CONTA,                                               
                    COD_OP_CONTA,                                               
                    COD_TIPO_CONTA                                              
             INTO   :FCCONBAN-IDE-CONTA-BANCARIA,                               
                    :FCCONBAN-COD-AGENCIA,                                      
                    :FCCONBAN-COD-BANCO,                                        
                    :FCCONBAN-COD-CONTA,                                        
                    :FCCONBAN-COD-DV-CONTA,                                     
                    :FCCONBAN-COD-OP-CONTA,                                     
                    :FCCONBAN-COD-TIPO-CONTA                                    
               FROM FDRCAP.FC_CONTA_BANCARIA                                    
             WHERE  IDE_CONTA_BANCARIA =:FCCONBAN-IDE-CONTA-BANCARIA            
           END-EXEC.                                                            
      *                                                                         
           IF SQLCODE  NOT EQUAL ZEROS                                          
                 DISPLAY ' 6070-ERRO LER CONTA BAN LOT=' CAD-COD-CEF            
                 DISPLAY ' AGEN=' FCCONBAN-COD-AGENCIA                          
                         ' BANCO=' FCCONBAN-COD-BANCO                           
                         ' CONTA=' FCCONBAN-COD-CONTA                           
                         ' DV=' FCCONBAN-COD-DV-CONTA                           
                         ' OP=' FCCONBAN-COD-OP-CONTA                           
                         ' IDE= ' FCCONBAN-IDE-CONTA-BANCARIA                   
                         GO  TO  R9999-ROT-ERRO.                                
      *                                                                         
           IF FCCONBAN-COD-TIPO-CONTA NOT = 'LOT'                               
              DISPLAY ' 6070-ENCONTRADO TIPO CTA BANCARIA DIF. LOT '            
                               FCCONBAN-IDE-CONTA-BANCARIA                      
                      '  '  CAD-COD-CEF                                         
              MOVE ZEROS TO FCCONBAN-COD-AGENCIA                                
                            FCCONBAN-COD-BANCO                                  
                            FCCONBAN-COD-CONTA                                  
                            FCCONBAN-COD-DV-CONTA                               
                            FCCONBAN-COD-OP-CONTA.                              
   ********** GO  TO  R9999-ROT-ERRO.                                           
                                                                                
           MOVE FCCONBAN-COD-BANCO     TO     WS-CONBAN-BANCO                   
           MOVE FCCONBAN-COD-AGENCIA   TO     WS-CONBAN-AGENCIA                 
           MOVE FCCONBAN-COD-OP-CONTA  TO     WS-CONBAN-OP                      
           MOVE FCCONBAN-COD-CONTA     TO     WS-CONBAN-CONTA                   
           MOVE FCCONBAN-COD-DV-CONTA  TO     WS-CONBAN-DV                      
           .                                                                    
       R6070-SAIDA. EXIT.                                                       
      *                                                                         
      *-----------------------------------------------------------------        
       R6080-VER-CONTA-EXISTENTE                                SECTION.        
      *-----------------------------------------------------------------        
      *                                                                         
           EXEC SQL                                                             
             SELECT                                                             
                    IDE_CONTA_BANCARIA                                          
             INTO   :FCCONBAN-IDE-CONTA-BANCARIA                                
               FROM FDRCAP.FC_CONTA_BANCARIA                                    
             WHERE                                                              
                    COD_BANCO      = :FCCONBAN-COD-BANCO                        
               AND  COD_AGENCIA    = :FCCONBAN-COD-AGENCIA                      
               AND  COD_CONTA      = :FCCONBAN-COD-CONTA                        
               AND  COD_OP_CONTA   = :FCCONBAN-COD-OP-CONTA                     
               AND  COD_DV_CONTA   = :FCCONBAN-COD-DV-CONTA                     
               AND  COD_TIPO_CONTA = 'LOT'                                      
           END-EXEC.                                                            
      *                                                                         
           IF SQLCODE  NOT EQUAL  ZEROS                                         
              IF SQLCODE  EQUAL  100                                            
                 MOVE ZEROS  TO  FCCONBAN-IDE-CONTA-BANCARIA                    
              ELSE                                                              
               DISPLAY ' 6070-ERRO LER CONTA BAN LOT=' CAD-COD-CEF              
               DISPLAY ' AGEN=' FCCONBAN-COD-AGENCIA                            
                       ' BANCO=' FCCONBAN-COD-BANCO                             
                       ' CONTA=' FCCONBAN-COD-CONTA                             
                       ' DV=' FCCONBAN-COD-DV-CONTA                             
                       ' OP=' FCCONBAN-COD-OP-CONTA                             
                       ' IDE= ' FCCONBAN-IDE-CONTA-BANCARIA                     
                       GO  TO  R9999-ROT-ERRO.                                  
      *                                                                         
       R6080-SAIDA. EXIT.                                                       
      *                                                                         
      *-----------------------------------------------------------------        
      *                                                                         
       R6200-MONTAR-FC-LOTERICO SECTION.                                        
      *---------------------------------                                        
      *                                                                         
           MOVE  0  TO   VIND-AGENTE-MASTER                                     
                         VIND-COD-CGC                                           
                         VIND-COD-INSCR-ESTAD                                   
                         VIND-COD-INSCR-MUNIC                                   
                         VIND-COD-MUNICIPIO                                     
                         VIND-COD-UF                                            
                         VIND-DES-EMAIL                                         
                         VIND-DES-ENDERECO                                      
                         VIND-DTH-EXCLUSAO                                      
                         VIND-DTH-INCLUSAO                                      
                         VIND-IDE-CONTA-CAUCAO                                  
                         VIND-IDE-CONTA-CPMF                                    
                         VIND-IDE-CONTA-ISENTA                                  
                         VIND-IND-CAT-LOTERICO                                  
                         VIND-IND-STA-LOTERICO                                  
                         VIND-IND-UNIDADE-SUB                                   
                         VIND-NOM-BAIRRO                                        
                         VIND-NOM-CONSULTOR                                     
                         VIND-NOM-CONTATO1                                      
                         VIND-NOM-CONTATO2                                      
                         VIND-NOM-FANTASIA                                      
                         VIND-NOM-MUNICIPIO                                     
                         VIND-NOM-RAZAO-SOCIAL                                  
                         VIND-NUM-CEP                                           
                         VIND-NUM-ENCEF                                         
                         VIND-NUM-LOTER-ANT                                     
                         VIND-NUM-MATR-CON                                      
                         VIND-NUM-PVCEF                                         
                         VIND-NUM-TELEFONE                                      
                         VIND-STA-DADOS-N                                       
                         VIND-STA-LOTERICO                                      
                         VIND-STA-NIVEL-COMIS                                   
                         VIND-STA-ULT-ALT                                       
                         VIND-COD-GARANTIA                                      
                         VIND-VLR-GARANTIA                                      
                         VIND-NUM-FAX.                                          
                                                                                
           MOVE  CAD-COD-CEF             TO  FCLOTERI-NUM-LOTERICO.             
           MOVE  CAD-DV-CEF              TO  FCLOTERI-NUM-DV-LOTERICO.          
                                                                                
           IF CAD-COD-AG-MASTER IS NUMERIC AND  CAD-COD-AG-MASTER > 0           
              MOVE  CAD-COD-AG-MASTERX  TO  FCLOTERI-COD-AGENTE-MASTER          
           ELSE                                                                 
              MOVE  -1 TO   VIND-AGENTE-MASTER.                                 
                                                                                
           IF    CAD-CGC IS NUMERIC AND  CAD-CGC > 0                            
               MOVE  CAD-CGCX            TO  FCLOTERI-COD-CGC                   
           ELSE                                                                 
              MOVE  -1 TO   VIND-COD-CGC.                                       
                                                                                
           IF   CAD-INSC-ESTX NOT = SPACES                                      
                MOVE  CAD-INSC-ESTX    TO  FCLOTERI-COD-INSCR-ESTAD             
           ELSE                                                                 
              MOVE  -1 TO   VIND-COD-INSCR-ESTAD.                               
                                                                                
           IF    CAD-INSC-MUNX NOT = SPACES                                     
                MOVE  CAD-INSC-MUNX    TO  FCLOTERI-COD-INSCR-MUNIC             
           ELSE                                                                 
              MOVE  -1 TO   VIND-COD-INSCR-MUNIC.                               
                                                                                
           IF  CAD-COD-MUNICIPIO NOT EQUAL SPACES                               
               MOVE  CAD-COD-MUNICIPIO  TO  FCLOTERI-COD-MUNICIPIO              
           ELSE                                                                 
              MOVE  -1 TO   VIND-COD-MUNICIPIO.                                 
                                                                                
           IF    CAD-UF NOT EQUAL SPACES                                        
               MOVE  CAD-UF                  TO  FCLOTERI-COD-UF                
           ELSE                                                                 
              MOVE  -1 TO   VIND-COD-UF.                                        
                                                                                
           IF    CAD-EMAIL NOT EQUAL SPACES                                     
               MOVE  CAD-EMAIL               TO  FCLOTERI-DES-EMAIL             
           ELSE                                                                 
              MOVE  -1 TO   VIND-DES-EMAIL.                                     
                                                                                
           IF    CAD-ENDERECO  NOT EQUAL SPACES                                 
              MOVE  CAD-ENDERECO          TO  FCLOTERI-DES-ENDERECO             
           ELSE                                                                 
              MOVE  -1 TO   VIND-DES-ENDERECO.                                  
                                                                                
           IF    CAD-DATA-INCLUSAO > 0                                          
               MOVE  W-CAD-DATA-INI-VIG      TO  FCLOTERI-DTH-INCLUSAO          
           ELSE                                                                 
               MOVE  SPACES                  TO  FCLOTERI-DTH-INCLUSAO          
               MOVE  -1 TO   VIND-DTH-INCLUSAO.                                 
                                                                                
           IF  CAD-DATA-EXCLUSAO > 0                                            
               MOVE  W-CAD-DATA-TER-VIG      TO  FCLOTERI-DTH-EXCLUSAO          
           ELSE                                                                 
              MOVE  SPACES                  TO  FCLOTERI-DTH-EXCLUSAO           
              MOVE  -1 TO   VIND-DTH-EXCLUSAO.                                  
                                                                                
           IF  WS-IDE-CONTA-CAUCAO > 0                                          
               MOVE  WS-IDE-CONTA-CAUCAO  TO  FCLOTERI-IDE-CONTA-CAUCAO         
           ELSE                                                                 
              MOVE  -1 TO   VIND-IDE-CONTA-CAUCAO.                              
                                                                                
           IF    WS-IDE-CONTA-CPMF > 0                                          
              MOVE  WS-IDE-CONTA-CPMF       TO  FCLOTERI-IDE-CONTA-CPMF         
           ELSE                                                                 
              MOVE  -1 TO   VIND-IDE-CONTA-CPMF.                                
                                                                                
           IF WS-IDE-CONTA-ISENTA > 0                                           
              MOVE  WS-IDE-CONTA-ISENTA  TO  FCLOTERI-IDE-CONTA-ISENTA          
           ELSE                                                                 
              MOVE  -1 TO   VIND-IDE-CONTA-ISENTA.                              
                                                                                
           IF CAD-CAT-LOTERICO IS NUMERIC AND                                   
              CAD-CAT-LOTERICO > 0                                              
              MOVE  CAD-CAT-LOTERICO  TO  FCLOTERI-IND-CAT-LOTERICO             
           ELSE                                                                 
              MOVE  -1 TO   VIND-IND-CAT-LOTERICO.                              
                                                                                
           IF CAD-COD-STATUS   IS NUMERIC AND                                   
              CAD-COD-STATUS   > 0                                              
              MOVE  CAD-COD-STATUS    TO  FCLOTERI-IND-STA-LOTERICO             
           ELSE                                                                 
              MOVE  -1 TO   VIND-IND-STA-LOTERICO.                              
                                                                                
           IF CAD-UNIDADE-SUB IS NUMERIC AND                                    
              CAD-UNIDADE-SUB > 0                                               
              MOVE  CAD-UNIDADE-SUB  TO  FCLOTERI-IND-UNIDADE-SUB               
           ELSE                                                                 
              MOVE  -1 TO   VIND-IND-UNIDADE-SUB.                               
                                                                                
           IF    CAD-BAIRRO NOT EQUAL SPACES                                    
              MOVE  CAD-BAIRRO           TO  FCLOTERI-NOM-BAIRRO                
           ELSE                                                                 
              MOVE  -1 TO   VIND-NOM-BAIRRO.                                    
                                                                                
           IF    CAD-NOME-CONSULTOR NOT EQUAL SPACES                            
              MOVE  CAD-NOME-CONSULTOR      TO  FCLOTERI-NOM-CONSULTOR          
           ELSE                                                                 
              MOVE  -1 TO   VIND-NOM-CONSULTOR.                                 
                                                                                
           IF    CAD-CONTATO1 NOT EQUAL SPACES                                  
               MOVE  CAD-CONTATO1            TO  FCLOTERI-NOM-CONTATO1          
           ELSE                                                                 
              MOVE  -1 TO   VIND-NOM-CONTATO1.                                  
                                                                                
           IF    CAD-CONTATO2 NOT EQUAL SPACES                                  
               MOVE  CAD-CONTATO2            TO  FCLOTERI-NOM-CONTATO2          
           ELSE                                                                 
              MOVE  -1 TO   VIND-NOM-CONTATO2.                                  
                                                                                
           IF    CAD-NOME-FANTASIA NOT EQUAL SPACES                             
              MOVE  CAD-NOME-FANTASIA       TO  FCLOTERI-NOM-FANTASIA           
           ELSE                                                                 
              MOVE  -1 TO   VIND-NOM-FANTASIA.                                  
                                                                                
           IF    CAD-CIDADE NOT EQUAL SPACES                                    
              MOVE  CAD-CIDADE              TO  FCLOTERI-NOM-MUNICIPIO          
           ELSE                                                                 
              MOVE  -1 TO   VIND-NOM-MUNICIPIO.                                 
                                                                                
           IF    CAD-RAZAO-SOCIAL NOT EQUAL SPACES                              
               MOVE  CAD-RAZAO-SOCIAL   TO  FCLOTERI-NOM-RAZAO-SOCIAL           
           ELSE                                                                 
              MOVE  -1 TO   VIND-NOM-RAZAO-SOCIAL.                              
                                                                                
           IF    CAD-CEP  IS NUMERIC AND                                        
                 CAD-CEP  > 0                                                   
              MOVE  CAD-CEP                 TO  FCLOTERI-NUM-CEP                
           ELSE                                                                 
              MOVE  -1 TO   VIND-NUM-CEP.                                       
                                                                                
           IF    CAD-EN-SUB IS NUMERIC AND                                      
                 CAD-EN-SUB > 0                                                 
              MOVE  CAD-EN-SUB              TO  FCLOTERI-NUM-ENCEF              
           ELSE                                                                 
              MOVE  -1 TO   VIND-NUM-ENCEF.                                     
                                                                                
           IF    CAD-NUM-LOT-ANTERIOR IS NUMERIC AND                            
                 CAD-NUM-LOT-ANTERIOR > 0                                       
             MOVE  CAD-NUM-LOT-ANTERIOR    TO  FCLOTERI-NUM-LOTER-ANT           
           ELSE                                                                 
              MOVE  -1 TO   VIND-NUM-LOTER-ANT.                                 
                                                                                
           IF    CAD-MATR-CONSULTOR IS NUMERIC AND                              
                 CAD-MATR-CONSULTOR > 0                                         
                MOVE  CAD-MATR-CONSULTOR TO                                     
                                   FCLOTERI-NUM-MATR-CONSULTOR                  
           ELSE                                                                 
              MOVE  -1 TO   VIND-NUM-MATR-CON.                                  
                                                                                
           IF  CAD-PV-SUB IS NUMERIC AND                                        
               CAD-PV-SUB > 0                                                   
               MOVE  CAD-PV-SUB              TO  FCLOTERI-NUM-PVCEF             
           ELSE                                                                 
              MOVE  -1 TO   VIND-NUM-PVCEF.                                     
                                                                                
           IF CAD-TELEFONE  NOT  =  SPACES   AND   ZEROS                        
              MOVE  CAD-TELEFONE         TO  FCLOTERI-NUM-TELEFONE              
           ELSE                                                                 
              MOVE  -1   TO VIND-NUM-TELEFONE.                                  
                                                                                
           IF    CAD-NIVEL-COMISSAO  > 0                                        
             MOVE  CAD-NIVEL-COMISSAOX TO  FCLOTERI-STA-NIVEL-COMIS             
           ELSE                                                                 
              MOVE  -1 TO   VIND-STA-NIVEL-COMIS.                               
                                                                                
           MOVE  CAD-SITUACAO     TO  FCLOTERI-STA-LOTERICO.                    
                                                                                
           MOVE  ' '                TO  FCLOTERI-STA-ULT-ALT-ONLINE             
           MOVE  -1                 TO  VIND-STA-ULT-ALT.                       
                                                                                
           MOVE  CAD-TIPO-GARANTIA    TO  FCLOTERI-COD-GARANTIA.                
                                                                                
           IF CAD-VALOR-GARANTIA  IS NUMERIC AND                                
              CAD-VALOR-GARANTIA  > 0                                           
              MOVE  CAD-VALOR-GARANTIA      TO  FCLOTERI-VLR-GARANTIA           
           ELSE                                                                 
              MOVE  -1 TO   VIND-VLR-GARANTIA.                                  
                                                                                
           MOVE  W-CAD-DATA-GERACAO     TO  FCLOTERI-DTH-GERACAO.               
           MOVE  CAD-NUM-SEGURADORA     TO  FCLOTERI-NUM-SEGURADORA.            
                                                                                
           IF  CAD-NUMERO-FAX  NOT  =  SPACES  AND ZEROS                        
               MOVE  CAD-NUMERO-FAX      TO  FCLOTERI-NUM-FAX                   
           ELSE                                                                 
              MOVE  -1   TO VIND-NUM-FAX.                                       
                                                                                
           IF  WS-NECESSARIO = 1                                                
               MOVE  'S'           TO  FCLOTERI-STA-DADOS-N-CADAST              
           ELSE                                                                 
               MOVE  'N'           TO  FCLOTERI-STA-DADOS-N-CADAST.             
                                                                                
       R6200-SAIDA.   EXIT.                                                     
                                                                                
                                                                                
       R6210-INSERT-FC-LOTERICO SECTION.                                        
      *----------------------------------                                       
                                                                                
           MOVE  '0008'  TO  WNR-EXEC-SQL.                                      
      *                                                                         
           EXEC SQL INSERT INTO  FDRCAP.FC_LOTERICO                             
                        (NUM_LOTERICO,                                          
                         COD_AGENTE_MASTER,                                     
                         COD_CGC,                                               
                         COD_INSCR_ESTAD,                                       
                         COD_INSCR_MUNIC,                                       
                         COD_MUNICIPIO,                                         
                         COD_UF,                                                
                         DES_EMAIL,                                             
                         DES_ENDERECO,                                          
                         DTH_EXCLUSAO,                                          
                         DTH_INCLUSAO,                                          
                         IDE_CONTA_CAUCAO,                                      
                         IDE_CONTA_CPMF,                                        
                         IDE_CONTA_ISENTA,                                      
                         IND_CAT_LOTERICO,                                      
                         IND_STA_LOTERICO,                                      
                         IND_UNIDADE_SUB,                                       
                         NOM_BAIRRO,                                            
                         NOM_CONSULTOR,                                         
                         NOM_CONTATO1,                                          
                         NOM_CONTATO2,                                          
                         NOM_FANTASIA,                                          
                         NOM_MUNICIPIO,                                         
                         NOM_RAZAO_SOCIAL,                                      
                         NUM_CEP,                                               
                         NUM_ENCEF,                                             
                         NUM_LOTER_ANT,                                         
                         NUM_MATR_CONSULTOR,                                    
                         NUM_PVCEF,                                             
                         NUM_TELEFONE,                                          
                         STA_DADOS_N_CADAST,                                    
                         STA_LOTERICO,                                          
                         STA_NIVEL_COMIS,                                       
                         STA_ULT_ALT_ONLINE,                                    
                         COD_GARANTIA,                                          
                         VLR_GARANTIA,                                          
                         DTH_GERACAO,                                           
                         NUM_FAX,                                               
                         NUM_DV_LOTERICO,                                       
                         NUM_SEGURADORA)                                        
             VALUES (:FCLOTERI-NUM-LOTERICO,                                    
                     :FCLOTERI-COD-AGENTE-MASTER :VIND-AGENTE-MASTER,           
                     :FCLOTERI-COD-CGC :VIND-COD-CGC,                           
                     :FCLOTERI-COD-INSCR-ESTAD :VIND-COD-INSCR-ESTAD,           
                     :FCLOTERI-COD-INSCR-MUNIC :VIND-COD-INSCR-MUNIC,           
                     :FCLOTERI-COD-MUNICIPIO :VIND-COD-MUNICIPIO,               
                     :FCLOTERI-COD-UF :VIND-COD-UF,                             
                     :FCLOTERI-DES-EMAIL :VIND-DES-EMAIL,                       
                     :FCLOTERI-DES-ENDERECO :VIND-DES-ENDERECO,                 
                     :FCLOTERI-DTH-EXCLUSAO :VIND-DTH-EXCLUSAO,                 
                     :FCLOTERI-DTH-INCLUSAO :VIND-DTH-INCLUSAO,                 
                     :FCLOTERI-IDE-CONTA-CAUCAO :VIND-IDE-CONTA-CAUCAO,         
                     :FCLOTERI-IDE-CONTA-CPMF   :VIND-IDE-CONTA-CPMF,           
                     :FCLOTERI-IDE-CONTA-ISENTA :VIND-IDE-CONTA-ISENTA,         
                     :FCLOTERI-IND-CAT-LOTERICO :VIND-IND-CAT-LOTERICO,         
                     :FCLOTERI-IND-STA-LOTERICO :VIND-IND-STA-LOTERICO,         
ALTS***************  :FCLOTERI-IND-UNIDADE-SUB  :VIND-IND-UNIDADE-SUB,          
ALTS                  NULL ,                                                    
                     :FCLOTERI-NOM-BAIRRO :VIND-NOM-BAIRRO,                     
                     :FCLOTERI-NOM-CONSULTOR :VIND-NOM-CONSULTOR,               
                     :FCLOTERI-NOM-CONTATO1  :VIND-NOM-CONTATO1,                
                     :FCLOTERI-NOM-CONTATO2  :VIND-NOM-CONTATO2,                
                     :FCLOTERI-NOM-FANTASIA  :VIND-NOM-FANTASIA,                
                     :FCLOTERI-NOM-MUNICIPIO :VIND-NOM-MUNICIPIO,               
                     :FCLOTERI-NOM-RAZAO-SOCIAL :VIND-NOM-RAZAO-SOCIAL,         
                     :FCLOTERI-NUM-CEP :VIND-NUM-CEP,                           
                     :FCLOTERI-NUM-ENCEF :VIND-NUM-ENCEF,                       
                     :FCLOTERI-NUM-LOTER-ANT :VIND-NUM-LOTER-ANT,               
                     :FCLOTERI-NUM-MATR-CONSULTOR :VIND-NUM-MATR-CON,           
                     :FCLOTERI-NUM-PVCEF :VIND-NUM-PVCEF,                       
                     :FCLOTERI-NUM-TELEFONE :VIND-NUM-TELEFONE,                 
                     :FCLOTERI-STA-DADOS-N-CADAST :VIND-STA-DADOS-N,            
                     :FCLOTERI-STA-LOTERICO :VIND-STA-LOTERICO,                 
                     :FCLOTERI-STA-NIVEL-COMIS :VIND-STA-NIVEL-COMIS,           
                     :FCLOTERI-STA-ULT-ALT-ONLINE :VIND-STA-ULT-ALT,            
                     :FCLOTERI-COD-GARANTIA :VIND-COD-GARANTIA,                 
                     :FCLOTERI-VLR-GARANTIA :VIND-VLR-GARANTIA,                 
                     :FCLOTERI-DTH-GERACAO :VIND-DTH-GERACAO,                   
                     :FCLOTERI-NUM-FAX :VIND-NUM-FAX,                           
                     :FCLOTERI-NUM-DV-LOTERICO,                                 
                     :FCLOTERI-NUM-SEGURADORA)                                  
           END-EXEC.                                                            
      *                                                                         
           IF SQLCODE  EQUAL  ZEROS                                             
              ADD 1  TO  WS-FCLOT-TOTAL-INCLUIDOS                               
              ADD 1  TO  W-AC-LOTERICOS-GRAVADOS                                
           ELSE                                                                 
              DISPLAY 'ERRO INSERT FCLOTERICO................... '              
              DISPLAY 'COD. LOTERICO   = ' FCLOTERI-NUM-LOTERICO                
              DISPLAY ' DV =' FCLOTERI-NUM-DV-LOTERICO                          
              DISPLAY  'DTH-EXC=' FCLOTERI-DTH-EXCLUSAO                         
              DISPLAY  'DTH-INC=' FCLOTERI-DTH-INCLUSAO                         
              DISPLAY  'IDE-CONTA-CAUCAO=' FCLOTERI-IDE-CONTA-CAUCAO            
              DISPLAY  'IDE-CONTA-CPMF=' FCLOTERI-IDE-CONTA-CPMF                
              DISPLAY  'IDE-CONTA-ISENTA=' FCLOTERI-IDE-CONTA-ISENTA            
              DISPLAY  'VIND-DTH-EXC=' VIND-DTH-EXCLUSAO                        
              DISPLAY  'VIND-DTH-INC=' VIND-DTH-INCLUSAO                        
              DISPLAY  'VIND-DTH-CONTA-CAUCAO=' VIND-IDE-CONTA-CAUCAO           
              DISPLAY  'VIND-DTH-CONTA-CPMF=' VIND-IDE-CONTA-CPMF               
              DISPLAY  'VIND-IDE-CONTA-ISENTA=' VIND-IDE-CONTA-ISENTA           
              GO  TO  R9999-ROT-ERRO.                                           
      *                                                                         
       R6210-SAIDA. EXIT.                                                       
      *-----------------------------------------------------------------        
       R6220-GRAVAR-FC-CONTA                                    SECTION.        
      *-----------------------------------------------------------------        
      *                                                                         
           MOVE ZEROS    TO WS-IDE-CONTA-CPMF                                   
                            WS-IDE-CONTA-ISENTA                                 
                            WS-IDE-CONTA-CAUCAO.                                
                                                                                
           IF CAD-BANCO-DESC-CPMF  EQUAL ZEROS OR                               
              CAD-AGEN-DESC-CPMF   EQUAL ZEROS OR                               
              CAD-CONTA-DESC-CPMF  EQUAL ZEROS                                  
              GO TO R6220-CONTA-ISENTA                                          
           END-IF                                                               
                                                                                
           MOVE CAD-BANCO-DESC-CPMF   TO WS-CODIGO-BANCO                        
           MOVE CAD-AGEN-DESC-CPMF    TO WS-CODIGO-AGENCIA                      
           MOVE CAD-CONTA-DESC-CPMF   TO WS-CONTA-BANCARIA                      
V.01                                     WS-CONTA-BANCO                         
V.02  *    MOVE WS-NUMERO-CONTA       TO WS-CONTA-10POS.                        
V.02       MOVE WS-NUMERO-CONTA       TO WS-CONTA-12POS.                        
                                                                                
V.01  *    MOVE WS-COD-BANCO      TO  FCCONBAN-COD-BANCO                        
V.01       MOVE WS-CODIGO-BANCO   TO  FCCONBAN-COD-BANCO                        
V.01  *    MOVE WS-COD-AGENCIA    TO  FCCONBAN-COD-AGENCIA                      
V.01       MOVE WS-CODIGO-AGENCIA TO  FCCONBAN-COD-AGENCIA                      
V.02  *    MOVE WS-CONTA-10POS    TO  FCCONBAN-COD-CONTA                        
V.02       MOVE WS-CONTA-12POS    TO  FCCONBAN-COD-CONTA                        
V.01  *    MOVE WS-OPERACAO-CONTA TO  FCCONBAN-COD-OP-CONTA                     
V.01       MOVE WS-OPERA-CONTA    TO  FCCONBAN-COD-OP-CONTA                     
           MOVE WS-DV-CONTA       TO  FCCONBAN-COD-DV-CONTA                     
                                                                                
           PERFORM R6080-VER-CONTA-EXISTENTE.                                   
                                                                                
           IF   FCCONBAN-IDE-CONTA-BANCARIA > ZEROS                             
                MOVE FCCONBAN-IDE-CONTA-BANCARIA TO WS-IDE-CONTA-CPMF           
           ELSE                                                                 
                PERFORM R6230-INSERT-FC-CONTA                                   
                MOVE MAX-IDE-CONTA-BANCARIA   TO WS-IDE-CONTA-CPMF              
           END-IF                                                               
           .                                                                    
       R6220-CONTA-ISENTA.                                                      
      *                                                                         
           IF CAD-BANCO-ISENTA     EQUAL ZEROS OR                               
              CAD-AGEN-ISENTA      EQUAL ZEROS OR                               
              CAD-CONTA-ISENTA     EQUAL ZEROS                                  
              GO TO R6220-CONTA-CAUCAO.                                         
                                                                                
           MOVE CAD-BANCO-ISENTA      TO WS-CODIGO-BANCO                        
           MOVE CAD-AGEN-ISENTA       TO WS-CODIGO-AGENCIA                      
           MOVE  CAD-CONTA-ISENTA     TO WS-CONTA-BANCARIA                      
V.01                                     WS-CONTA-BANCO                         
V.02  *    MOVE  WS-NUMERO-CONTA      TO WS-CONTA-10POS.                        
V.02       MOVE  WS-NUMERO-CONTA      TO WS-CONTA-12POS.                        
                                                                                
V.01  *    MOVE WS-COD-BANCO      TO  FCCONBAN-COD-BANCO                        
V.01       MOVE WS-CODIGO-BANCO   TO  FCCONBAN-COD-BANCO                        
V.01  *    MOVE WS-COD-AGENCIA    TO  FCCONBAN-COD-AGENCIA                      
V.01       MOVE WS-CODIGO-AGENCIA TO  FCCONBAN-COD-AGENCIA                      
V.02  *    MOVE WS-CONTA-10POS    TO  FCCONBAN-COD-CONTA                        
V.02       MOVE WS-CONTA-12POS    TO  FCCONBAN-COD-CONTA                        
V.01  *    MOVE WS-OPERACAO-CONTA TO  FCCONBAN-COD-OP-CONTA                     
V.01       MOVE WS-OPERA-CONTA    TO  FCCONBAN-COD-OP-CONTA                     
           MOVE WS-DV-CONTA       TO  FCCONBAN-COD-DV-CONTA                     
                                                                                
           PERFORM R6080-VER-CONTA-EXISTENTE.                                   
                                                                                
           IF   FCCONBAN-IDE-CONTA-BANCARIA > ZEROS                             
                MOVE FCCONBAN-IDE-CONTA-BANCARIA TO WS-IDE-CONTA-ISENTA         
           ELSE                                                                 
                PERFORM R6230-INSERT-FC-CONTA                                   
                MOVE  MAX-IDE-CONTA-BANCARIA     TO WS-IDE-CONTA-ISENTA         
           END-IF                                                               
           .                                                                    
       R6220-CONTA-CAUCAO.                                                      
      *                                                                         
           IF CAD-BANCO-CAUCAO     EQUAL ZEROS OR                               
              CAD-AGEN-CAUCAO      EQUAL ZEROS OR                               
              CAD-CONTA-CAUCAO     EQUAL ZEROS                                  
              GO TO R6220-SAIDA.                                                
                                                                                
           MOVE CAD-BANCO-CAUCAO      TO WS-CODIGO-BANCO                        
           MOVE CAD-AGEN-CAUCAO       TO WS-CODIGO-AGENCIA                      
           MOVE  CAD-CONTA-CAUCAO     TO WS-CONTA-BANCARIA                      
V.01                                     WS-CONTA-BANCO                         
V.02  *    MOVE  WS-NUMERO-CONTA      TO WS-CONTA-10POS.                        
V.02       MOVE  WS-NUMERO-CONTA      TO WS-CONTA-12POS.                        
                                                                                
V.01  *    MOVE WS-COD-BANCO      TO  FCCONBAN-COD-BANCO                        
V.01       MOVE WS-CODIGO-BANCO   TO  FCCONBAN-COD-BANCO                        
V.01  *    MOVE WS-COD-AGENCIA    TO  FCCONBAN-COD-AGENCIA                      
V.01       MOVE WS-CODIGO-AGENCIA TO  FCCONBAN-COD-AGENCIA                      
V.02  *    MOVE WS-CONTA-10POS    TO  FCCONBAN-COD-CONTA                        
V.02       MOVE WS-CONTA-12POS    TO  FCCONBAN-COD-CONTA                        
V.01  *    MOVE WS-OPERACAO-CONTA TO  FCCONBAN-COD-OP-CONTA                     
V.01       MOVE WS-OPERA-CONTA    TO  FCCONBAN-COD-OP-CONTA                     
           MOVE WS-DV-CONTA       TO  FCCONBAN-COD-DV-CONTA                     
                                                                                
           PERFORM R6080-VER-CONTA-EXISTENTE.                                   
                                                                                
           IF   FCCONBAN-IDE-CONTA-BANCARIA > ZEROS                             
                MOVE FCCONBAN-IDE-CONTA-BANCARIA TO WS-IDE-CONTA-CAUCAO         
           ELSE                                                                 
                PERFORM R6230-INSERT-FC-CONTA                                   
                MOVE  MAX-IDE-CONTA-BANCARIA     TO WS-IDE-CONTA-CAUCAO         
           END-IF                                                               
           .                                                                    
       R6220-SAIDA.                                                             
           EXIT.                                                                
      *                                                                         
      *-----------------------------------------------------------------        
       R6230-INSERT-FC-CONTA                                    SECTION.        
      *-----------------------------------------------------------------        
           MOVE  '0008'  TO  WNR-EXEC-SQL.                                      
                                                                                
           PERFORM R5000-SELECT-MAX-CONTA                                       
           COMPUTE MAX-IDE-CONTA-BANCARIA = MAX-IDE-CONTA-BANCARIA + 1          
           MOVE MAX-IDE-CONTA-BANCARIA TO FCCONBAN-IDE-CONTA-BANCARIA           
V.03       MOVE 20                     TO FCCONBAN-COD-EMPRESA                  
                                                                                
           EXEC SQL INSERT INTO  FDRCAP.FC_CONTA_BANCARIA                       
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
                     'LOT',                                                     
V.03                 :FCCONBAN-COD-EMPRESA)                                     
           END-EXEC.                                                            
      *                                                                         
           IF SQLCODE  NOT EQUAL  ZEROS                                         
              IF SQLCODE  EQUAL  -803                                           
V.03             DISPLAY ' R6230-ERRO -803 INSERT FC-CONTA BANCARIA'            
                 DISPLAY ' AGEN='    FCCONBAN-COD-AGENCIA                       
                         ' BANCO='   FCCONBAN-COD-BANCO                         
                         ' CONTA='   FCCONBAN-COD-CONTA                         
                         ' DV='      FCCONBAN-COD-DV-CONTA                      
                         ' OP='      FCCONBAN-COD-OP-CONTA                      
                         ' IDE= '    FCCONBAN-IDE-CONTA-BANCARIA                
                         ' CODLOT= ' CAD-COD-CEF                                
V.03                     ' EMP='     FCCONBAN-COD-EMPRESA                       
                         '  CONTA JA CADASTRADA '                               
                        GO  TO R6230-SAIDA                                      
              ELSE                                                              
                 DISPLAY ' R6230-ERRO INSERT FC-CONTA BANCARIA'                 
                 DISPLAY ' AGEN='  FCCONBAN-COD-AGENCIA                         
                         ' BANCO=' FCCONBAN-COD-BANCO                           
                         ' CONTA=' FCCONBAN-COD-CONTA                           
                         ' DV='    FCCONBAN-COD-DV-CONTA                        
                         ' OP='    FCCONBAN-COD-OP-CONTA                        
                         ' IDE= '  FCCONBAN-IDE-CONTA-BANCARIA                  
                         ' CODLOT= ' CAD-COD-CEF                                
V.03                     ' EMP='   FCCONBAN-COD-EMPRESA                         
V.03                    GO  TO  R9999-ROT-ERRO                                  
V.03          END-IF                                                            
V.03       END-IF.                                                              
                                                                                
           PERFORM R5100-UPDATE-MAX-CONTA.                                      
           .                                                                    
       R6230-SAIDA. EXIT.                                                       
      *                                                                         
      *----------------------------------------------------------------*        
      *                                                                         
      *                                                                         
       R6600-GRAVAR-MOVIMENTO   SECTION.                                        
      *---------------------------------                                        
      *  COD-MOVIMENTO :  1-INCLUSAO                                            
      *                   3-ALTERACAO VIA SIGEL                                 
      *                   5-ALTERACAO DE I.S.  ON-LINE                          
      *                   7-CANCELAMENTO                                        
      *                                                                         
      *    OBS: A INCLUSAO   EFETUADA VIA APLICACAO WEB                         
      *                                                                         
           MOVE 7105                  TO  LTMVPROP-COD-PRODUTO                  
           MOVE 6204                  TO  LTMVPROP-COD-EXT-ESTIP                
           MOVE CAD-CODIGO-CEF        TO  LTMVPROP-COD-EXT-SEGURADO             
           MOVE W-CAD-DATA-GERACAO    TO  LTMVPROP-DATA-MOVIMENTO.              
                                                                                
           MOVE         WS-HH-TIME         TO          WTIME-HORA               
           MOVE        '.'                 TO          WTIME-2PT1               
           MOVE         WS-MM-TIME         TO          WTIME-MINU               
           MOVE        '.'                 TO          WTIME-2PT2               
           MOVE         WS-SS-TIME         TO          WTIME-SEGU               
           MOVE         WTIME-DAYR         TO  LTMVPROP-HORA-MOVIMENTO.         
                                                                                
           MOVE  WS-NUM-APOLICE         TO  LTMVPROP-NUM-APOLICE.               
           MOVE  SPACES                 TO  LTMVPROP-COD-USUARIO-CANCEL.        
      *                                                                         
           MOVE  CAD-RAZAO-SOCIAL  TO  LTMVPROP-NOM-RAZAO-SOCIAL                
           MOVE  CAD-NOME-FANTASIA TO  LTMVPROP-NOME-FANTASIA                   
           MOVE  CAD-CGC           TO  LTMVPROP-CGCCPF                          
           MOVE  CAD-END           TO  LTMVPROP-ENDERECO                        
           MOVE  CAD-COMPL-END     TO  LTMVPROP-COMPL-ENDER                     
           MOVE  CAD-BAIRRO        TO  LTMVPROP-BAIRRO                          
           MOVE  CAD-CEP           TO  LTMVPROP-CEP                             
           MOVE  CAD-CIDADE        TO  LTMVPROP-CIDADE                          
           MOVE  CAD-UF            TO  LTMVPROP-SIGLA-UF                        
           MOVE  CAD-DDD-FONE      TO  LTMVPROP-DDD                             
           MOVE  CAD-FONE          TO  LTMVPROP-NUM-FONE                        
           MOVE  CAD-FAX           TO  LTMVPROP-NUM-FAX                         
           MOVE  CAD-EMAIL         TO  LTMVPROP-EMAIL                           
                                                                                
           MOVE CAD-EN-SUB             TO LTMVPROP-COD-DIVISAO                  
           MOVE CAD-PV-SUB             TO LTMVPROP-COD-SUBDIVISAO               
                                                                                
           MOVE CAD-BANCO-DESC-CPMF    TO WS-CODIGO-BANCO                       
           MOVE WS-COD-BANCO           TO LTMVPROP-COD-BANCO                    
                                                                                
           MOVE CAD-AGEN-DESC-CPMF     TO WS-CODIGO-AGENCIA                     
           MOVE WS-COD-AGENCIA         TO LTMVPROP-COD-AGENCIA                  
                                                                                
           MOVE CAD-CONTA-DESC-CPMF    TO WS-CONTA-BANCARIA                     
V.02  *    MOVE WS-NUMERO-CONTA        TO WS-CONTA-10POS                        
V.02       MOVE WS-NUMERO-CONTA        TO WS-CONTA-12POS                        
V.02  *    MOVE WS-CONTA-10POS         TO LTMVPROP-COD-CONTA                    
V.02       MOVE WS-CONTA-12POS         TO LTMVPROP-COD-CONTA                    
           MOVE WS-DV-CONTA            TO LTMVPROP-COD-DV-CONTA                 
           MOVE WS-OPERACAO-CONTA      TO LTMVPROP-COD-OP-CONTA.                
      *                                                                         
           MOVE CAD-NUM-LOT-ANTERIOR TO LTMVPROP-COD-EXT-SEG-ANT                
                                                                                
OL1801******** ALTERACAO DO DDD PARA -1  SE HOUVER SOMENTE ALT. DE              
OL1801******** TELEFONE , EMAIL E FAX ( LT2002B NAO GERA CERTIFICADO )          
                                                                                
OL1801     IF LTMVPROP-IND-ALT-DADOS-PES = 'N'   AND                            
OL1801        LTMVPROP-IND-ALT-ENDER     = 'N'   AND                            
OL1801        W-IND-ALT-FAXTELEME        = 'S'   AND                            
OL1801        LTMVPROP-IND-ALT-COBER     = 'N'   AND                            
OL1801        LTMVPROP-IND-ALT-BONUS     = 'N'                                  
OL1801        MOVE  'S'  TO LTMVPROP-IND-ALT-ENDER                              
OL1801        IF LTMVPROP-DDD > 0                                               
OL1801           COMPUTE LTMVPROP-DDD = LTMVPROP-DDD * - 1                      
OL1801        ELSE                                                              
OL1801           COMPUTE LTMVPROP-DDD = - 1 .                                   
                                                                                
*******************************************************************             
                                                                                
                                                                                
           MOVE '3'                     TO LTMVPROP-SIT-MOVIMENTO.              
      *                                                                         
           MOVE  W-CAD-DATA-GERACAO  TO LTMVPROP-DT-INIVIG-PROPOSTA.            
                                                                                
           MOVE 0                    TO  LTMVPROP-VAL-PREMIO.                   
                                                                                
           MOVE  '6600'  TO  WNR-EXEC-SQL.                                      
                                                                                
           EXEC SQL INSERT INTO  SEGUROS.LT_MOV_PROPOSTA                        
                       (COD_PRODUTO,                                            
                        COD_EXT_ESTIP,                                          
                        COD_EXT_SEGURADO,                                       
                        DATA_MOVIMENTO,                                         
                        HORA_MOVIMENTO,                                         
                        COD_MOVIMENTO,                                          
                        NOM_RAZAO_SOCIAL,                                       
                        NOME_FANTASIA,                                          
                        CGCCPF,                                                 
                        ENDERECO,                                               
                        COMPL_ENDER,                                            
                        BAIRRO,                                                 
                        CEP,                                                    
                        CIDADE,                                                 
                        SIGLA_UF,                                               
                        DDD,                                                    
                        NUM_FONE,                                               
                        NUM_FAX,                                                
                        EMAIL,                                                  
                        COD_DIVISAO,                                            
                        COD_SUBDIVISAO,                                         
                        COD_BANCO,                                              
                        COD_AGENCIA,                                            
                        COD_CONTA,                                              
                        COD_DV_CONTA,                                           
                        COD_OP_CONTA,                                           
                        COD_EXT_SEG_ANT,                                        
                        SIT_MOVIMENTO,                                          
                        DT_INIVIG_PROPOSTA,                                     
                        IND_ALT_DADOS_PES,                                      
                        IND_ALT_ENDER,                                          
                        IND_ALT_COBER,                                          
                        IND_ALT_BONUS,                                          
                        COD_USUARIO,                                            
                        TIMESTAMP,                                              
                        VAL_PREMIO,                                             
                        NUM_APOLICE,                                            
                        COD_USUARIO_CANCEL)                                     
             VALUES  (:LTMVPROP-COD-PRODUTO,                                    
                      :LTMVPROP-COD-EXT-ESTIP,                                  
                      :LTMVPROP-COD-EXT-SEGURADO,                               
                      :LTMVPROP-DATA-MOVIMENTO,                                 
                      :LTMVPROP-HORA-MOVIMENTO,                                 
                      :LTMVPROP-COD-MOVIMENTO,                                  
                      :LTMVPROP-NOM-RAZAO-SOCIAL,                               
                      :LTMVPROP-NOME-FANTASIA,                                  
                      :LTMVPROP-CGCCPF,                                         
                      :LTMVPROP-ENDERECO,                                       
                      :LTMVPROP-COMPL-ENDER,                                    
                      :LTMVPROP-BAIRRO,                                         
                      :LTMVPROP-CEP,                                            
                      :LTMVPROP-CIDADE,                                         
                      :LTMVPROP-SIGLA-UF,                                       
                      :LTMVPROP-DDD,                                            
                      :LTMVPROP-NUM-FONE,                                       
                      :LTMVPROP-NUM-FAX,                                        
                      :LTMVPROP-EMAIL,                                          
                      :LTMVPROP-COD-DIVISAO,                                    
                      :LTMVPROP-COD-SUBDIVISAO,                                 
                      :LTMVPROP-COD-BANCO,                                      
                      :LTMVPROP-COD-AGENCIA,                                    
                      :LTMVPROP-COD-CONTA,                                      
                      :LTMVPROP-COD-DV-CONTA,                                   
                      :LTMVPROP-COD-OP-CONTA,                                   
                      :LTMVPROP-COD-EXT-SEG-ANT,                                
                      :LTMVPROP-SIT-MOVIMENTO,                                  
                      :LTMVPROP-DT-INIVIG-PROPOSTA,                             
                      :LTMVPROP-IND-ALT-DADOS-PES,                              
                      :LTMVPROP-IND-ALT-ENDER,                                  
                      :LTMVPROP-IND-ALT-COBER,                                  
                      :LTMVPROP-IND-ALT-BONUS,                                  
                      :LTMVPROP-COD-USUARIO,                                    
                       CURRENT TIMESTAMP,                                       
                      :LTMVPROP-VAL-PREMIO,                                     
                      :LTMVPROP-NUM-APOLICE,                                    
                      :LTMVPROP-COD-USUARIO-CANCEL)                             
           END-EXEC.                                                            
                                                                                
           IF SQLCODE  NOT EQUAL  ZEROS                                         
              IF SQLCODE      EQUAL  - 803                                      
                DISPLAY 'R6600-ERRO INSERT MOVIMENTO SQLCODE -803....'          
                DISPLAY ' COD. LOT = ' LTMVPROP-COD-EXT-SEGURADO                
                DISPLAY ' RAZAO =' LTMVPROP-NOM-RAZAO-SOCIAL                    
                DISPLAY ' CGC =' LTMVPROP-CGCCPF                                
                DISPLAY ' COD.MOV =' LTMVPROP-COD-MOVIMENTO                     
                GO  TO  R9999-ROT-ERRO                                          
             ELSE                                                               
                DISPLAY 'R6600-ERRO INSERT MOVIMENTO.............. '            
                DISPLAY 'COD. LOT= ' LTMVPROP-COD-EXT-SEGURADO                  
                DISPLAY ' RAZAO  =' LTMVPROP-NOM-RAZAO-SOCIAL                   
                DISPLAY ' CGC    =' LTMVPROP-CGCCPF                             
                DISPLAY ' COD.MOV=' LTMVPROP-COD-MOVIMENTO                      
                        ' PRODUTO=' LTMVPROP-COD-PRODUTO                        
                        ' ESTIP='   LTMVPROP-COD-EXT-ESTIP                      
                        ' DATA=' LTMVPROP-DATA-MOVIMENTO                        
                        ' HORA=' LTMVPROP-HORA-MOVIMENTO                        
                 GO  TO  R9999-ROT-ERRO.                                        
                                                                                
           ADD 1 TO  WS-MVPROP-TOTAL-INCLUIDOS.                                 
                                                                                
           IF LTMVPROP-COD-MOVIMENTO = 3                                        
              ADD 1 TO  WS-MVPROP-TOTAL-ALTERADOS                               
           ELSE                                                                 
           IF LTMVPROP-COD-MOVIMENTO = 7                                        
              ADD 1 TO  WS-MVPROP-TOTAL-CANCELADOS.                             
                                                                                
       R6600-SAIDA. EXIT.                                                       
      *----------------------------------------------------------------*        
      *                                                                         
       R6610-GRAVAR-MOV-COBER   SECTION.                                        
      *---------------------------------                                        
      *  GRAVAR TABELA LT-MOV-PROP-COBER                                        
      *                                                                         
           IF LTMVPROP-COD-MOVIMENTO = 7   OR                                   
              (LTMVPROP-IND-ALT-COBER = 'N'  AND                                
               LTMVPROP-IND-ALT-BONUS = 'N')                                    
              GO TO R6610-SAIDA.                                                
                                                                                
           MOVE LTMVPROP-COD-PRODUTO     TO  LTMVPRCO-COD-PRODUTO               
           MOVE LTMVPROP-COD-EXT-ESTIP   TO  LTMVPRCO-COD-EXT-ESTIP             
           MOVE LTMVPROP-COD-EXT-SEGURADO TO  LTMVPRCO-COD-EXT-SEGURADO         
           MOVE LTMVPROP-DATA-MOVIMENTO  TO  LTMVPRCO-DATA-MOVIMENTO            
           MOVE LTMVPROP-COD-MOVIMENTO   TO  LTMVPRCO-COD-MOVIMENTO             
           MOVE LTMVPROP-HORA-MOVIMENTO  TO  LTMVPRCO-HORA-MOVIMENTO            
           MOVE  4                       TO  LTMVPRCO-COD-COBERTURA             
                                                                                
           MOVE  CAD-VALOR-GARANTIA TO  LTMVPRCO-VAL-IMP-SEGURADA               
           MOVE  0                  TO  LTMVPRCO-VAL-TAXA-PREMIO.               
                                                                                
      *                                                                         
           MOVE  '6610'  TO  WNR-EXEC-SQL.                                      
                                                                                
           EXEC SQL INSERT INTO SEGUROS.LT_MOV_PROP_COBER                       
                       (COD_PRODUTO,                                            
                        COD_EXT_ESTIP,                                          
                        COD_EXT_SEGURADO,                                       
                        DATA_MOVIMENTO,                                         
                        HORA_MOVIMENTO,                                         
                        COD_MOVIMENTO,                                          
                        COD_COBERTURA,                                          
                        VAL_IMP_SEGURADA,                                       
                        VAL_TAXA_PREMIO)                                        
             VALUES  (:LTMVPRCO-COD-PRODUTO,                                    
                      :LTMVPRCO-COD-EXT-ESTIP,                                  
                      :LTMVPRCO-COD-EXT-SEGURADO,                               
                      :LTMVPRCO-DATA-MOVIMENTO,                                 
                      :LTMVPRCO-HORA-MOVIMENTO,                                 
                      :LTMVPRCO-COD-MOVIMENTO,                                  
                      :LTMVPRCO-COD-COBERTURA,                                  
                      :LTMVPRCO-VAL-IMP-SEGURADA,                               
                      :LTMVPRCO-VAL-TAXA-PREMIO)                                
           END-EXEC.                                                            
                                                                                
           IF SQLCODE  NOT EQUAL  ZEROS                                         
              DISPLAY 'R6610-ERRO INSERT MOVIMENTO COBERTURA '                  
              DISPLAY 'COD. LOTERICO   = ' LTMVPRCO-COD-EXT-SEGURADO            
              GO  TO  R9999-ROT-ERRO.                                           
      *                                                                         
       R6610-SAIDA. EXIT.                                                       
      *----------------------------------------------------------------*        
      *                                                                         
      *                                                                         
       R6620-GRAVAR-MOV-BONUS   SECTION.                                        
      *---------------------------------                                        
      *  GRAVAR TABELA LT-MOV-PROP-BONUS                                        
      *                                                                         
           IF LTMVPROP-COD-MOVIMENTO = 7   OR                                   
              LTMVPROP-IND-ALT-BONUS = 'N'                                      
              GO TO R6620-SAIDA.                                                
                                                                                
           MOVE LTMVPRCO-COD-PRODUTO      TO  LTMVPRBO-COD-PRODUTO              
           MOVE LTMVPRCO-COD-EXT-ESTIP    TO  LTMVPRBO-COD-EXT-ESTIP            
           MOVE LTMVPRCO-COD-EXT-SEGURADO TO  LTMVPRBO-COD-EXT-SEGURADO         
           MOVE LTMVPRCO-DATA-MOVIMENTO   TO  LTMVPRBO-DATA-MOVIMENTO.          
           MOVE LTMVPRCO-HORA-MOVIMENTO   TO  LTMVPRBO-HORA-MOVIMENTO.          
           MOVE LTMVPRCO-COD-MOVIMENTO    TO  LTMVPRBO-COD-MOVIMENTO.           
           MOVE LTMVPRCO-COD-COBERTURA    TO  LTMVPRBO-COD-COBERTURA.           
                                                                                
      *                                                                         
      *=> BONUS DE ALARME - SE TEM O BONUS E DE 1%                              
      *                                                                         
           IF CAD-BONUS-ALARME EQUAL  1                                         
              MOVE      2                   TO  LTMVPRBO-COD-BONUS              
              PERFORM  R6630-GRAVAR-MOV-BONUS.                                  
      *                                                                         
      *=> BONUS DE VIDEO CASSETE - SE TEM O BONUS E DE 2%                       
      *                                                                         
           IF CAD-BONUS-CKT    EQUAL  1                                         
              MOVE      3                   TO  LTMVPRBO-COD-BONUS              
              PERFORM  R6630-GRAVAR-MOV-BONUS.                                  
      *                                                                         
      *=> BONUS DE COFRE - SE TEM O BONUS E DE 7%                               
      *                                                                         
           IF CAD-BONUS-COFRE EQUAL  1                                          
              MOVE      4                   TO  LTMVPRBO-COD-BONUS              
              PERFORM  R6630-GRAVAR-MOV-BONUS.                                  
      *                                                                         
       R6620-SAIDA. EXIT.                                                       
      *                                                                         
       R6630-GRAVAR-MOV-BONUS   SECTION.                                        
      *                                                                         
           MOVE  '6630'  TO  WNR-EXEC-SQL.                                      
                                                                                
           EXEC SQL INSERT INTO  SEGUROS.LT_MOV_PROP_BONUS                      
                       (COD_PRODUTO,                                            
                        COD_EXT_ESTIP,                                          
                        COD_EXT_SEGURADO,                                       
                        DATA_MOVIMENTO,                                         
                        HORA_MOVIMENTO,                                         
                        COD_MOVIMENTO,                                          
                        COD_COBERTURA,                                          
                        COD_BONUS)                                              
             VALUES  (:LTMVPRBO-COD-PRODUTO,                                    
                      :LTMVPRBO-COD-EXT-ESTIP,                                  
                      :LTMVPRBO-COD-EXT-SEGURADO,                               
                      :LTMVPRBO-DATA-MOVIMENTO,                                 
                      :LTMVPRBO-HORA-MOVIMENTO,                                 
                      :LTMVPRBO-COD-MOVIMENTO,                                  
                      :LTMVPRBO-COD-COBERTURA,                                  
                      :LTMVPRBO-COD-BONUS)                                      
           END-EXEC.                                                            
                                                                                
           IF SQLCODE  NOT EQUAL  ZEROS                                         
              DISPLAY 'R6630-ERRO INSERT MOVIMENTO BONUS     '                  
              DISPLAY 'COD. LOTERICO   = ' LTMVPRBO-COD-EXT-SEGURADO            
              DISPLAY 'DATA MOVIMENTO  = ' LTMVPRBO-DATA-MOVIMENTO              
              DISPLAY 'HORA MOVIMENTO  = ' LTMVPRBO-HORA-MOVIMENTO              
              DISPLAY 'COD. COBERTURA  = ' LTMVPRBO-COD-COBERTURA               
              DISPLAY 'COD. BONUS      = ' LTMVPRBO-COD-BONUS.                  
      *       GO  TO  R9999-ROT-ERRO.                                           
                                                                                
       R6630-SAIDA. EXIT.                                                       
      *----------------------------------------------------------------*        
      *                                                                         
       R6700-UPDATE-FC-LOTERICO SECTION.                                        
      *---------------------------------                                        
      *                                                                         
           MOVE  '0008'  TO  WNR-EXEC-SQL.                                      
      *                                                                         
           EXEC SQL                                                             
            UPDATE FDRCAP.FC_LOTERICO                                           
             SET COD_AGENTE_MASTER =:FCLOTERI-COD-AGENTE-MASTER                 
                                    :VIND-COD-AGENTE-MASTER,                    
                 COD_CGC           =:FCLOTERI-COD-CGC                           
                                    :VIND-COD-CGC,                              
                 COD_INSCR_ESTAD   =:FCLOTERI-COD-INSCR-ESTAD                   
                                    :VIND-COD-INSCR-ESTAD    ,                  
                 COD_INSCR_MUNIC   =:FCLOTERI-COD-INSCR-MUNIC                   
                                    :VIND-COD-INSCR-MUNIC,                      
                 COD_MUNICIPIO     =:FCLOTERI-COD-MUNICIPIO                     
                                    :VIND-COD-MUNICIPIO,                        
                 COD_UF            =:FCLOTERI-COD-UF                            
                                    :VIND-COD-UF,                               
                 DES_EMAIL         =:FCLOTERI-DES-EMAIL                         
                                    :VIND-DES-EMAIL,                            
                 DES_ENDERECO      =:FCLOTERI-DES-ENDERECO                      
                                    :VIND-DES-ENDERECO,                         
                 DTH_EXCLUSAO      =:FCLOTERI-DTH-EXCLUSAO                      
                                    :VIND-DTH-EXCLUSAO,                         
                 DTH_INCLUSAO      =:FCLOTERI-DTH-INCLUSAO                      
                                    :VIND-DTH-INCLUSAO,                         
                 IDE_CONTA_CAUCAO  =:FCLOTERI-IDE-CONTA-CAUCAO                  
                                    :VIND-IDE-CONTA-CAUCAO,                     
                 IDE_CONTA_CPMF    =:FCLOTERI-IDE-CONTA-CPMF                    
                                    :VIND-IDE-CONTA-CPMF,                       
                 IDE_CONTA_ISENTA  =:FCLOTERI-IDE-CONTA-ISENTA                  
                                    :VIND-IDE-CONTA-ISENTA,                     
                 IND_CAT_LOTERICO  =:FCLOTERI-IND-CAT-LOTERICO                  
                                    :VIND-IND-CAT-LOTERICO,                     
                 IND_STA_LOTERICO  =:FCLOTERI-IND-STA-LOTERICO                  
                                    :VIND-IND-STA-LOTERICO,                     
ALTS************ IND_UNIDADE_SUB   =:FCLOTERI-IND-UNIDADE-SUB                   
ALTS************                    :VIND-IND-UNIDADE-SUB,                      
                 NOM_BAIRRO        =:FCLOTERI-NOM-BAIRRO                        
                                    :VIND-NOM-BAIRRO,                           
                 NOM_CONSULTOR     =:FCLOTERI-NOM-CONSULTOR                     
                                    :VIND-NOM-CONSULTOR,                        
                 NOM_CONTATO1       =:FCLOTERI-NOM-CONTATO1                     
                                     :VIND-NOM-CONTATO1,                        
                 NOM_CONTATO2       =:FCLOTERI-NOM-CONTATO2                     
                                     :VIND-NOM-CONTATO2,                        
                 NOM_FANTASIA       =:FCLOTERI-NOM-FANTASIA                     
                                     :VIND-NOM-FANTASIA,                        
                 NOM_MUNICIPIO      =:FCLOTERI-NOM-MUNICIPIO                    
                                     :VIND-NOM-MUNICIPIO,                       
                 NOM_RAZAO_SOCIAL   =:FCLOTERI-NOM-RAZAO-SOCIAL                 
                                     :VIND-NOM-RAZAO-SOCIAL,                    
                 NUM_CEP            =:FCLOTERI-NUM-CEP                          
                                     :VIND-NUM-CEP,                             
                 NUM_ENCEF          =:FCLOTERI-NUM-ENCEF                        
                                     :VIND-NUM-ENCEF,                           
                 NUM_LOTER_ANT      =:FCLOTERI-NUM-LOTER-ANT                    
                                     :VIND-NUM-LOTER-ANT,                       
                 NUM_MATR_CONSULTOR =:FCLOTERI-NUM-MATR-CONSULTOR               
                                     :VIND-NUM-MATR-CON,                        
                 NUM_PVCEF          =:FCLOTERI-NUM-PVCEF                        
                                     :VIND-NUM-PVCEF,                           
                 NUM_TELEFONE       =:FCLOTERI-NUM-TELEFONE                     
                                     :VIND-NUM-TELEFONE,                        
                 STA_DADOS_N_CADAST =:FCLOTERI-STA-DADOS-N-CADAST               
                                     :VIND-STA-DADOS-N,                         
                 STA_LOTERICO       =:FCLOTERI-STA-LOTERICO                     
                                     :VIND-STA-LOTERICO,                        
                 STA_NIVEL_COMIS    =:FCLOTERI-STA-NIVEL-COMIS                  
                                     :VIND-STA-NIVEL-COMIS,                     
                 STA_ULT_ALT_ONLINE =:FCLOTERI-STA-ULT-ALT-ONLINE               
                                     :VIND-STA-ULT-ALT,                         
                 COD_GARANTIA       =:FCLOTERI-COD-GARANTIA                     
                                     :VIND-COD-GARANTIA,                        
                 VLR_GARANTIA       =:FCLOTERI-VLR-GARANTIA                     
                                     :VIND-VLR-GARANTIA,                        
                 DTH_GERACAO        =:FCLOTERI-DTH-GERACAO                      
                                     :VIND-DTH-GERACAO,                         
                 NUM_DV_LOTERICO    =:FCLOTERI-NUM-DV-LOTERICO,                 
                 NUM_FAX            =:FCLOTERI-NUM-FAX                          
                                     :VIND-NUM-FAX,                             
                 NUM_SEGURADORA     =:FCLOTERI-NUM-SEGURADORA                   
            WHERE                                                               
             NUM_LOTERICO =:FCLOTERI-NUM-LOTERICO                               
           END-EXEC.                                                            
                                                                                
                                                                                
           IF SQLCODE  EQUAL  ZEROS                                             
              ADD 1  TO WS-FCLOT-TOTAL-ALTERADOS                                
              ADD 1  TO W-AC-LOTERICOS-GRAVADOS                                 
           ELSE                                                                 
              DISPLAY '6700-ERRO UPDATE FCLOTERICO................... '         
              DISPLAY 'COD. LOTERICO   = ' FCLOTERI-NUM-LOTERICO                
              DISPLAY  'DTH-EXC=' FCLOTERI-DTH-EXCLUSAO                         
              DISPLAY  'DTH-INC=' FCLOTERI-DTH-INCLUSAO                         
              DISPLAY  'IDE-CONTA-CAUCAO=' FCLOTERI-IDE-CONTA-CAUCAO            
              DISPLAY  'IDE-CONTA-CPMF=' FCLOTERI-IDE-CONTA-CPMF                
              DISPLAY  'IDE-CONTA-ISENTA=' FCLOTERI-IDE-CONTA-ISENTA            
              DISPLAY  'VIND-DTH-EXC=' VIND-DTH-EXCLUSAO                        
              DISPLAY  'VIND-DTH-INC=' VIND-DTH-INCLUSAO                        
              DISPLAY  'VIND-DTH-CONTA-CAUCAO=' VIND-IDE-CONTA-CAUCAO           
              DISPLAY  'VIND-DTH-CONTA-CPMF=' VIND-IDE-CONTA-CPMF               
              DISPLAY  'VIND-IDE-CONTA-ISENTA=' VIND-IDE-CONTA-ISENTA           
              GO  TO  R9999-ROT-ERRO.                                           
      *                                                                         
       R6700-SAIDA. EXIT.                                                       
      *----------------------------------------------------------------*        
      *                                                                         
       R6750-DELETE-FC-PEND-LOTERICO SECTION.                                   
      *---------------------------------                                        
      *                                                                         
           MOVE  '0008'  TO  WNR-EXEC-SQL.                                      
      *                                                                         
      *    EXEC SQL                                                             
      *     DELETE FROM FDRCAP.FC_TIPO_PEND_LOTER                               
      *     WHERE                                                               
      *      NUM_LOTERICO =:FCLOTERI-NUM-LOTERICO                               
      *    END-EXEC.                                                            
      *                                                                         
      *    IF SQLCODE  EQUAL  ZEROS                                             
      *       ADD  1  TO  WS-FCPEND-DELETADOS                                   
      *    ELSE                                                                 
      *    IF SQLCODE   = 100                                                   
      *       GO TO R6750-SAIDA                                                 
      *    ELSE                                                                 
      *       DISPLAY '6750-ERRO DELETE FC_TIPO_PEND_LOTER......... '           
      *       DISPLAY 'COD. LOTERICO   = ' FCLOTERI-NUM-LOTERICO                
      *       GO  TO  R9999-ROT-ERRO.                                           
      *                                                                         
      *    EXEC SQL                                                             
      *     DELETE FROM FDRCAP.FC_PEND_LOTERICO                                 
      *     WHERE                                                               
      *      NUM_LOTERICO =:FCLOTERI-NUM-LOTERICO                               
      *    END-EXEC.                                                            
      *                                                                         
      *    IF SQLCODE  NOT EQUAL  ZEROS                                         
      *          DISPLAY '6750-ERRO DELETE FC_PEND_LOTERICO.......... '         
      *          DISPLAY 'COD. LOTERICO   = ' FCLOTERI-NUM-LOTERICO             
      *          GO  TO  R9999-ROT-ERRO.                                        
      *                                                                         
       R6750-SAIDA. EXIT.                                                       
      *----------------------------------------------------------------*        
       R6800-SELECT-BONUS SECTION.                                              
      *--------------------------                                               
      *                                                                         
           MOVE  '6800'  TO  WNR-EXEC-SQL.                                      
      *                                                                         
           EXEC SQL                                                             
             SELECT  NUM_LOTERICO       ,                                       
                     COD_BONUS                                                  
               INTO  :LTLOTBON-NUM-LOTERICO,                                    
                     :LTLOTBON-COD-BONUS                                        
               FROM  SEGUROS.LT_LOTERICO_BONUS                                  
              WHERE  NUM_LOTERICO   =  :LTLOTBON-NUM-LOTERICO                   
  *           AND    COD_BONUS      =  :LTLOTBON-COD-BONUS                      
           END-EXEC.                                                            
      *                                                                         
           IF SQLCODE  NOT EQUAL  ZEROS                                         
              IF SQLCODE NOT EQUAL  100                                         
                 DISPLAY 'ERRO SELECT LT_LOTERICO_BONUS....'                    
                 DISPLAY 'COD. LOTERICO   = ' CAD-COD-CEF                       
                 DISPLAY 'BONUS COM ERRO  = ' LTLOTBON-COD-BONUS                
                 GO  TO  R9999-ROT-ERRO.                                        
      *                                                                         
       R6800-SAIDA. EXIT.                                                       
      *                                                                         
      *----------------------------------------------------------------*        
       R6810-GRAVAR-LOTERICO-BONUS  SECTION.                                    
      *------------------------------------                                     
      *                                                                         
           MOVE  CAD-COD-CEF                TO  LTLOTBON-NUM-LOTERICO.          
      *                                                                         
      *=> BONUS DE ALARME - SE TEM O BONUS E DE 1%                              
      *                                                                         
           IF CAD-BONUS-ALARME EQUAL  1                                         
              MOVE      2                   TO  LTLOTBON-COD-BONUS              
              PERFORM  R6830-INSERT-BONUS.                                      
      *                                                                         
      *=> BONUS DE VIDEO CASSETE - SE TEM O BONUS E DE 2%                       
      *                                                                         
           IF CAD-BONUS-CKT    EQUAL  1                                         
              MOVE      3                   TO  LTLOTBON-COD-BONUS              
              PERFORM  R6830-INSERT-BONUS.                                      
      *                                                                         
      *=> BONUS DE COFRE - SE TEM O BONUS E DE 7%                               
      *                                                                         
           IF CAD-BONUS-COFRE EQUAL  1                                          
              MOVE      4                   TO  LTLOTBON-COD-BONUS              
              PERFORM  R6830-INSERT-BONUS.                                      
      *                                                                         
       R6810-SAIDA. EXIT.                                                       
      *-----------------------------------------------------------------        
       R6820-DELETE-BONUS SECTION.                                              
      *--------------------------                                               
           MOVE  '6820'  TO  WNR-EXEC-SQL.                                      
      *                                                                         
           EXEC SQL                                                             
              DELETE FROM  SEGUROS.LT_LOTERICO_BONUS                            
           WHERE                                                                
              NUM_LOTERICO =:LTLOTBON-NUM-LOTERICO                              
           END-EXEC.                                                            
      *                                                                         
           IF SQLCODE  NOT EQUAL  ZEROS                                         
              AND SQLCODE  NOT EQUAL  100                                       
              DISPLAY ' R6820-ERRO DELETE LT_LOTERICO_BONUS COD-CEF:'           
                      CAD-COD-CEF                                               
              GO  TO  R9999-ROT-ERRO.                                           
      *                                                                         
       R6820-SAIDA. EXIT.                                                       
      *-----------------------------------------------------------------        
       R6830-INSERT-BONUS SECTION.                                              
      *--------------------------                                               
           MOVE  '0010'  TO  WNR-EXEC-SQL.                                      
      *                                                                         
           EXEC SQL                                                             
            INSERT INTO  SEGUROS.LT_LOTERICO_BONUS                              
                      (NUM_LOTERICO       ,                                     
                        COD_BONUS)                                              
             VALUES   (:LTLOTBON-NUM-LOTERICO ,                                 
                       :LTLOTBON-COD-BONUS)                                     
           END-EXEC.                                                            
      *                                                                         
           IF SQLCODE  NOT EQUAL  ZEROS                                         
              DISPLAY ' R6830-ERRO INSERT LT_LOTERICO_BONUS'                    
                      CAD-COD-CEF                                               
              GO  TO  R9999-ROT-ERRO.                                           
      *                                                                         
       R6830-SAIDA. EXIT.                                                       
      *-----------------------------------------------------------------        
      *                                                                         
      *----------------------------------------------------------------*        
       R6850-VER-ALTERACAO-BONUS  SECTION.                                      
      *------------------------------------                                     
      *                                                                         
           MOVE 0 TO WS-ALARME WS-CKT WS-COFRE.                                 
      *                                                                         
           MOVE  CAD-COD-CEF                TO  LTLOTBON-NUM-LOTERICO.          
           MOVE  2                          TO  LTLOTBON-COD-BONUS.             
           PERFORM  R6800-SELECT-BONUS.                                         
           IF SQLCODE = ZEROS                                                   
              MOVE 1 TO WS-ALARME.                                              
      *                                                                         
           MOVE  3                          TO  LTLOTBON-COD-BONUS.             
           PERFORM  R6800-SELECT-BONUS.                                         
           IF SQLCODE = ZEROS                                                   
              MOVE 1 TO WS-CKT.                                                 
      *                                                                         
           MOVE  4                          TO  LTLOTBON-COD-BONUS.             
           PERFORM  R6800-SELECT-BONUS.                                         
           IF SQLCODE = ZEROS                                                   
              MOVE 1 TO WS-COFRE.                                               
      *                                                                         
OL2208     IF CAD-BONUS-ALARME NOT EQUAL  1                                     
              MOVE ZEROS TO CAD-BONUS-ALARME.                                   
OL2208     IF CAD-BONUS-CKT NOT EQUAL  1                                        
              MOVE ZEROS TO CAD-BONUS-CKT.                                      
OL2208     IF CAD-BONUS-COFRE  NOT EQUAL  1                                     
              MOVE ZEROS TO CAD-BONUS-COFRE.                                    
                                                                                
           IF CAD-BONUS-ALARME EQUAL  WS-ALARME AND                             
              CAD-BONUS-CKT    EQUAL  WS-CKT    AND                             
              CAD-BONUS-COFRE  EQUAL  WS-COFRE                                  
              GO TO R6850-SAIDA.                                                
      *                                                                         
           DISPLAY ' ALT BONUS =' CAD-CODIGO-CEF.                               
      *                                                                         
           MOVE  'SIM' TO W-CHAVE-HOUVE-ALTERACAO                               
                          W-CHAVE-ALTEROU-SEGURADO                              
           MOVE 'S'      TO LTMVPROP-IND-ALT-BONUS.                             
      *                                                                         
      *                                                                         
       R6850-SAIDA. EXIT.                                                       
      *-----------------------------------------------------------------        
      *                                                                         
      *----------------------------------------------------------------*        
       R6900-ALTERAR-BONUS  SECTION.                                            
      *------------------------------------                                     
      *                                                                         
      *                                                                         
           IF  LTMVPROP-IND-ALT-BONUS  =  'N'                                   
               GO TO R6900-SAIDA.                                               
                                                                                
           PERFORM  R6820-DELETE-BONUS.                                         
                                                                                
      *                                                                         
      *=> BONUS DE ALARME - SE TEM O BONUS E DE 1%                              
      *                                                                         
           IF CAD-BONUS-ALARME EQUAL  1                                         
              MOVE      2                   TO  LTLOTBON-COD-BONUS              
              PERFORM  R6830-INSERT-BONUS.                                      
      *                                                                         
      *=> BONUS DE VIDEO CASSETE - SE TEM O BONUS E DE 2%                       
      *                                                                         
           IF CAD-BONUS-CKT    EQUAL  1                                         
              MOVE      3                   TO  LTLOTBON-COD-BONUS              
              PERFORM  R6830-INSERT-BONUS.                                      
      *                                                                         
      *=> BONUS DE COFRE - SE TEM O BONUS E DE 7%                               
      *                                                                         
           IF CAD-BONUS-COFRE EQUAL  1                                          
              MOVE      4                   TO  LTLOTBON-COD-BONUS              
              PERFORM  R6830-INSERT-BONUS.                                      
      *                                                                         
       R6900-SAIDA. EXIT.                                                       
      *-----------------------------------------------------------------        
      *                                                                         
      *----------------------------------------------------------------*        
       R7500-IMPRIME-CADASTRO  SECTION.                                         
      *---------------------------------                                        
      *                                                                         
      *                                                                         
           MOVE  CAD-COD-CEFX          TO  LD01-CAD-COD-CEF                     
           MOVE  CAD-RAZAO-SOCIAL      TO  LD01-CAD-RAZAO-SOCIAL                
           MOVE  CAD-SITUACAOX         TO  LD01-SITUACAO                        
                                                                                
           MOVE  CAD-CGC               TO  W-CGC.                               
           MOVE  W-CGC-8               TO  W-CGC-8-ED                           
           MOVE  W-CGC-4               TO  W-CGC-4-ED                           
           MOVE  W-CGC-2               TO  W-CGC-2-ED                           
           MOVE  W-CGC-ED              TO  LD02-CAD-CGC                         
           MOVE  CAD-ENDERECO          TO  LD02-CAD-END                         
           MOVE  CAD-BAIRRO            TO  LD02-CAD-BAIRRO                      
                                                                                
           MOVE  CAD-INSC-MUNX         TO  LD02A-CAD-INSC-MUN                   
           MOVE  CAD-INSC-ESTX         TO  LD02A-CAD-INSC-EST                   
           MOVE  CAD-NUM-LOT-ANTERIORX TO  LD02B-LOT-ANT                        
           MOVE  CAD-COD-AG-MASTERX    TO  LD02B-AGE-MASTER                     
           MOVE  CAD-CAT-LOTERICOX     TO  LD02B-CATEGORIA                      
           MOVE  CAD-COD-STATUSX       TO  LD02B-STATUS                         
                                                                                
           MOVE  CAD-CEP               TO  LD03-CAD-CEP-1.                      
           MOVE  CAD-CIDADE            TO  LD03-CAD-CIDADE                      
           MOVE  CAD-UF                TO  LD03-CAD-UF                          
           MOVE  CAD-DDD-FONE          TO  LD03-CAD-DDD-FONE                    
           MOVE  CAD-FONE              TO  LD03-CAD-FONE                        
           MOVE  CAD-DDD-FAX           TO  LD03-CAD-DDD-FAX                     
           MOVE  CAD-FAX               TO  LD03-CAD-FAX                         
                                                                                
           MOVE  CAD-TIPO-GARANTIAX    TO  LD04-CAD-TIPO-GARANTIA               
           MOVE  CAD-VALOR-GARANTIA    TO  LD04-CAD-VALORES                     
           MOVE  CAD-BONUS-ALARMEX     TO  LD04-CAD-BONUS-ALA                   
           MOVE  CAD-BONUS-CKTX        TO  LD04-CAD-BONUS-CKT                   
           MOVE  CAD-BONUS-COFREX      TO  LD04-CAD-BONUS-COF                   
                                                                                
           MOVE  CAD-DATA-INCLUSAO     TO  W-DATA-AAAAMMDD                      
           MOVE  W-DATA-AAAAMMDD-DD    TO  W-DATA-AAAA-MM-DD-DD                 
           MOVE  W-DATA-AAAAMMDD-MM    TO  W-DATA-AAAA-MM-DD-MM                 
           MOVE  W-DATA-AAAAMMDD-AAAA  TO  W-DATA-AAAA-MM-DD-AAAA               
           MOVE  W-DATA-AAAA-MM-DD     TO  LD05-CAD-DATA-INCLUSAO.              
                                                                                
           MOVE  CAD-DATA-EXCLUSAO     TO  W-DATA-AAAAMMDD                      
           MOVE  W-DATA-AAAAMMDD-DD    TO  W-DATA-AAAA-MM-DD-DD                 
           MOVE  W-DATA-AAAAMMDD-MM    TO  W-DATA-AAAA-MM-DD-MM                 
           MOVE  W-DATA-AAAAMMDD-AAAA  TO  W-DATA-AAAA-MM-DD-AAAA               
           MOVE  W-DATA-AAAA-MM-DD     TO  LD05-CAD-DATA-EXCLUSAO.              
                                                                                
           MOVE  W-CAD-DATA-GERACAO    TO  LD05-CAD-DATA-GERACAO.               
                                                                                
      *    MOVE  CAD-DATA-INCLUSAOX    TO  LD05-CAD-DATA-INCLUSAO               
      *    MOVE  CAD-DATA-EXCLUSAOX    TO  LD05-CAD-DATA-EXCLUSAO               
      *    MOVE  CAD-DATA-GERACAOX     TO  LD05-CAD-DATA-GERACAO.               
           MOVE  CAD-NSRX              TO  LD05-NSR.                            
      *                                                                         
           MOVE  CAD-MATR-CONSULTORX        TO LD06-MAT-CONSULTOR.              
           MOVE  CAD-NOME-CONSULTOR         TO LD06-NOME-CONSULTOR              
           MOVE  CAD-EMAIL                  TO LD06-EMAIL.                      
      *                                                                         
           MOVE  CAD-PV-SUBX                TO LD06A-PV-SUB.                    
           MOVE  CAD-EN-SUBX                TO LD06A-EN-SUB.                    
           MOVE  CAD-UNIDADE-SUBX           TO LD06A-UNIDADE-SUB                
           MOVE  CAD-NIVEL-COMISSAOX        TO LD06A-NIVEL-COMISSAO.            
      *                                                                         
           MOVE  CAD-NOME-FANTASIA          TO LD06B-NOME-FANTASIA.             
           MOVE  CAD-CONTATO1               TO LD06B-CONTATO1.                  
           MOVE  CAD-CONTATO2               TO LD06B-CONTATO2.                  
           MOVE  CAD-COD-MUNICIPIO          TO LD06B-COD-MUNICIPIO.             
      *                                                                         
           MOVE  CAD-NUM-SEGURADORA    TO LD06C-NUM-SEGURADORA.                 
      *                                                                         
           MOVE  CAD-BANCO-DESC-CPMF   TO  LD07-CAD-BANCO.                      
           MOVE  CAD-AGEN-DESC-CPMF    TO  LD07-CAD-AGENCIA.                    
           MOVE  CAD-CONTA-DESC-CPMF   TO  WS-CONTA-BANCARIA.                   
           MOVE  WS-OPERACAO-CONTA     TO  LD07-CAD-OPERACAO.                   
           MOVE  WS-NUMERO-CONTA       TO  LD07-CAD-CONTA.                      
           MOVE  WS-DV-CONTA           TO  LD07-CAD-DV-CONTA.                   
      *                                                                         
           MOVE  CAD-BANCO-ISENTA      TO  LD07A-CAD-BANCO.                     
           MOVE  CAD-AGEN-ISENTA       TO  LD07A-CAD-AGENCIA.                   
           MOVE  CAD-CONTA-ISENTA      TO  WS-CONTA-BANCARIA.                   
           MOVE  WS-OPERACAO-CONTA     TO  LD07A-CAD-OPERACAO.                  
           MOVE  WS-NUMERO-CONTA       TO  LD07A-CAD-CONTA.                     
           MOVE  WS-DV-CONTA           TO  LD07A-CAD-DV-CONTA.                  
      *                                                                         
           MOVE  CAD-BANCO-CAUCAO      TO  LD07B-CAD-BANCO.                     
           MOVE  CAD-AGEN-CAUCAO       TO  LD07B-CAD-AGENCIA.                   
           MOVE  CAD-CONTA-CAUCAO      TO  WS-CONTA-BANCARIA.                   
           MOVE  WS-OPERACAO-CONTA     TO  LD07B-CAD-OPERACAO.                  
           MOVE  WS-NUMERO-CONTA       TO  LD07B-CAD-CONTA.                     
           MOVE  WS-DV-CONTA           TO  LD07B-CAD-DV-CONTA.                  
      *                                                                         
           ADD        1       TO  W-AC-LINHA.                                   
           WRITE  REG-RLT2000B  FROM  LC05 AFTER 1.                             
                                                                                
           ADD        2       TO  W-AC-LINHA.                                   
           PERFORM  R7520-CABECALHO.                                            
           WRITE  REG-RLT2000B  FROM  LD01-CAD  AFTER  2.                       
      *                                                                         
           ADD        1       TO  W-AC-LINHA.                                   
           PERFORM  R7520-CABECALHO.                                            
           WRITE  REG-RLT2000B  FROM  LD02-CAD  AFTER  1.                       
      *                                                                         
           ADD        1       TO  W-AC-LINHA.                                   
           PERFORM  R7520-CABECALHO.                                            
           WRITE  REG-RLT2000B  FROM  LD02A-CAD  AFTER  1.                      
      *                                                                         
           ADD        1       TO  W-AC-LINHA.                                   
           PERFORM  R7520-CABECALHO.                                            
           WRITE  REG-RLT2000B  FROM  LD02B-CAD  AFTER  1.                      
      *                                                                         
           ADD        1       TO  W-AC-LINHA.                                   
           PERFORM  R7520-CABECALHO.                                            
           WRITE  REG-RLT2000B  FROM  LD03-CAD  AFTER  1.                       
      *                                                                         
           ADD        3       TO  W-AC-LINHA.                                   
           PERFORM  R7520-CABECALHO.                                            
           WRITE  REG-RLT2000B  FROM  LD04-CAD   AFTER  1.                      
           WRITE  REG-RLT2000B  FROM  LD04-CAD-A AFTER  1.                      
           WRITE  REG-RLT2000B  FROM  LD04-CAD-B AFTER  1.                      
      *                                                                         
           ADD        1       TO  W-AC-LINHA.                                   
           PERFORM  R7520-CABECALHO.                                            
           WRITE  REG-RLT2000B  FROM  LD05-CAD  AFTER  1.                       
      *                                                                         
           ADD        1       TO  W-AC-LINHA.                                   
           PERFORM  R7520-CABECALHO.                                            
           WRITE  REG-RLT2000B  FROM  LD06-CAD  AFTER  1.                       
      *                                                                         
           ADD        1       TO  W-AC-LINHA.                                   
           PERFORM  R7520-CABECALHO.                                            
           WRITE  REG-RLT2000B  FROM  LD06A-CAD  AFTER  1.                      
      *                                                                         
           ADD        1       TO  W-AC-LINHA.                                   
           PERFORM  R7520-CABECALHO.                                            
           WRITE  REG-RLT2000B  FROM  LD06B-CAD  AFTER  1.                      
      *                                                                         
           ADD        1       TO  W-AC-LINHA.                                   
           PERFORM  R7520-CABECALHO.                                            
           WRITE  REG-RLT2000B  FROM  LD06C-CAD  AFTER  1.                      
      *                                                                         
           ADD        1       TO  W-AC-LINHA.                                   
           PERFORM  R7520-CABECALHO.                                            
           WRITE  REG-RLT2000B  FROM  LD07-CAD  AFTER  1.                       
      *                                                                         
           ADD        1       TO  W-AC-LINHA.                                   
           PERFORM  R7520-CABECALHO.                                            
           WRITE  REG-RLT2000B  FROM  LD07A-CAD  AFTER  1.                      
      *                                                                         
           ADD        1       TO  W-AC-LINHA.                                   
           PERFORM  R7520-CABECALHO.                                            
           WRITE  REG-RLT2000B  FROM  LD07B-CAD  AFTER  1.                      
      *                                                                         
           ADD        1       TO  W-AC-LINHA.                                   
           PERFORM  R7520-CABECALHO.                                            
           WRITE  REG-RLT2000B  FROM  LC06       AFTER  1.                      
      *                                                                         
       R7500-SAIDA.  EXIT.                                                      
      *----------------------------------------------------------------*        
       R7510-MONTA-CABECALHO     SECTION.                                       
      *------------------------------------                                     
      *                                                                         
           ACCEPT      WS-DATE             FROM    DATE                         
           MOVE        WS-DD-DATE          TO      WDATA-DD-CABEC               
           MOVE        WS-MM-DATE          TO      WDATA-MM-CABEC               
           MOVE        WS-AA-DATE          TO      WDATA-AA-CABEC               
           MOVE        WDATA-CABEC         TO      LC02-DATA                    
      *                                                                         
           ACCEPT      WS-TIME             FROM    TIME                         
           MOVE        WS-HH-TIME          TO      WHORA-HH-CABEC               
           MOVE        WS-MM-TIME          TO      WHORA-MM-CABEC               
           MOVE        WS-SS-TIME          TO      WHORA-SS-CABEC               
           MOVE        WHORA-CABEC         TO      LC03-HORA                    
      *                                                                         
           EXEC SQL    SELECT     NOME_EMPRESA                                  
                       INTO      :V1EMPR-NOM-EMP                                
                       FROM       SEGUROS.V1EMPRESA                             
                       WHERE      COD_EMPRESA  =   0                            
           END-EXEC.                                                            
      *                                                                         
           MOVE        V1EMPR-NOM-EMP      TO      LK-TITULO                    
           CALL       'PROALN01'           USING   LK-LINK                      
      *                                                                         
           IF          LK-RTCODE           EQUAL   ZEROS                        
             MOVE      LK-TITULO           TO      LC01-EMPRESA                 
           ELSE                                                                 
             DISPLAY  'PROBLEMA CALL V1EMPRESA'                                 
             STOP      RUN.                                                     
      *                                                                         
           MOVE    99    TO W-AC-LINHA.                                         
      *                                                                         
       R7510-SAIDA. EXIT.                                                       
      *----------------------------------------------------------------*        
       R7520-CABECALHO  SECTION.                                                
      *-------------------------                                                
      *                                                                         
           IF W-AC-LINHA  GREATER  60                                           
              ADD        1       TO  W-AC-PAGINA                                
              MOVE  W-AC-PAGINA  TO  LC01-PAGINA                                
              WRITE  REG-RLT2000B  FROM  LC01 AFTER PAGE                        
              WRITE  REG-RLT2000B  FROM  LC02 AFTER 1                           
              WRITE  REG-RLT2000B  FROM  LC03 AFTER 1                           
              WRITE  REG-RLT2000B  FROM  LC06 AFTER 1                           
              MOVE       4       TO  W-AC-LINHA.                                
      *                                                                         
       R7520-SAIDA. EXIT.                                                       
      *-----------------------------------------------------------------        
       R7600-IMPRIME-LD00-MSG1  SECTION.                                        
      *---------------------------------                                        
      *                                                                         
###        IF CAD-SITUACAOX  =  ZEROS OR 2                                      
###           GO TO R7600-SAIDA.                                                
                                                                                
###        IF WS-IMPRIMIU EQUAL ZEROS                                           
###           MOVE       1       TO  WS-IMPRIMIU                                
###           PERFORM  R7500-IMPRIME-CADASTRO.                                  
      *                                                                         
           ADD        1       TO  W-AC-LINHA                                    
           PERFORM  R7520-CABECALHO                                             
           WRITE  REG-RLT2000B  FROM  LD00       AFTER  1.                      
      *                                                                         
       R7600-SAIDA. EXIT.                                                       
      *----------------------------------------------------------------*        
       R7650-CONVERTE-CARACTER   SECTION.                                       
      *------------------------------------                                     
      *                                                                         
                MOVE 'S' TO LK-CONVER-MAIUSCULA                                 
                MOVE 40  TO LK-TAM-CAMPO                                        
                MOVE CAD-RAZAO-SOCIAL  TO LK-CAMPO-ENTRADA                      
                CALL 'PROCONV'         USING LK-CONVERSAO                       
                MOVE LK-CAMPO-SAIDA    TO CAD-RAZAO-SOCIAL.                     
                                                                                
                MOVE 'S' TO LK-CONVER-MAIUSCULA                                 
                MOVE 30  TO LK-TAM-CAMPO                                        
                MOVE CAD-NOME-FANTASIA TO LK-CAMPO-ENTRADA                      
                CALL 'PROCONV'         USING LK-CONVERSAO                       
                MOVE LK-CAMPO-SAIDA    TO CAD-NOME-FANTASIA.                    
                                                                                
                MOVE 'S' TO LK-CONVER-MAIUSCULA                                 
                MOVE 40  TO LK-TAM-CAMPO                                        
                MOVE CAD-END           TO LK-CAMPO-ENTRADA                      
                CALL 'PROCONV'         USING LK-CONVERSAO                       
                MOVE LK-CAMPO-SAIDA    TO CAD-END.                              
                                                                                
                MOVE 'S' TO LK-CONVER-MAIUSCULA                                 
                MOVE 16  TO LK-TAM-CAMPO                                        
                MOVE CAD-COMPL-END     TO LK-CAMPO-ENTRADA                      
                CALL 'PROCONV'         USING LK-CONVERSAO                       
                MOVE LK-CAMPO-SAIDA    TO CAD-COMPL-END.                        
                                                                                
                MOVE 'S' TO LK-CONVER-MAIUSCULA                                 
                MOVE 20  TO LK-TAM-CAMPO                                        
                MOVE CAD-BAIRRO        TO LK-CAMPO-ENTRADA                      
                CALL 'PROCONV'         USING LK-CONVERSAO                       
                MOVE LK-CAMPO-SAIDA    TO CAD-BAIRRO.                           
                                                                                
                MOVE 'S' TO LK-CONVER-MAIUSCULA                                 
                MOVE 12  TO LK-TAM-CAMPO                                        
                MOVE CAD-COD-MUNICIPIO TO LK-CAMPO-ENTRADA                      
                CALL 'PROCONV'         USING LK-CONVERSAO                       
                MOVE LK-CAMPO-SAIDA    TO CAD-COD-MUNICIPIO.                    
                                                                                
                MOVE 'S' TO LK-CONVER-MAIUSCULA                                 
                MOVE 25  TO LK-TAM-CAMPO                                        
                MOVE CAD-CIDADE        TO LK-CAMPO-ENTRADA                      
                CALL 'PROCONV'         USING LK-CONVERSAO                       
                MOVE LK-CAMPO-SAIDA    TO CAD-CIDADE.                           
                                                                                
                MOVE 'S' TO LK-CONVER-MAIUSCULA                                 
                MOVE 02  TO LK-TAM-CAMPO                                        
                MOVE CAD-UF            TO LK-CAMPO-ENTRADA                      
                CALL 'PROCONV'         USING LK-CONVERSAO                       
                MOVE LK-CAMPO-SAIDA    TO CAD-UF.                               
                                                                                
                MOVE 'S' TO LK-CONVER-MAIUSCULA                                 
                MOVE 40  TO LK-TAM-CAMPO                                        
                MOVE CAD-NOME-CONSULTOR TO LK-CAMPO-ENTRADA                     
                CALL 'PROCONV'         USING LK-CONVERSAO                       
                MOVE LK-CAMPO-SAIDA    TO CAD-NOME-CONSULTOR.                   
                                                                                
      *                                                                         
       R7650-SAIDA. EXIT.                                                       
                                                                                
       R6990-GRAVAR-PARAM-RENOVAR  SECTION.                                     
                                                                                
      *---------------------------------                                        
      *                                                                         
                                                                                
           MOVE 7105              TO       V0SOL-COD-PRODUTO                    
           MOVE CAD-CODIGO-CEF    TO       V0SOL-COD-CLIENTE                    
           MOVE V0SIST-DTMOVABE   TO       V0SOL-DATA-SOLICITACAO               
           MOVE 'LT2000B'         TO       V0SOL-COD-USUARIO                    
           MOVE 'LT2018B'         TO       V0SOL-COD-PROGRAMA                   
           MOVE V0SIST-DTMOVABE   TO       V0SOL-DATA-PREV-PROC                 
           MOVE  0                TO       V0SOL-COD-MOVIMENTO                  
           MOVE  '0'              TO       V0SOL-SIT-SOLICITACAO                
           MOVE CAD-RENOVAR       TO       V0SOL-TIPO-SOLICITACAO               
           MOVE V0SIST-DTMOVABE   TO       V0SOL-PARAM-DATE01                   
           MOVE V0SIST-DTMOVABE   TO       V0SOL-PARAM-DATE02                   
           MOVE V0SIST-DTMOVABE   TO       V0SOL-PARAM-DATE03                   
           MOVE 0                 TO       V0SOL-PARAM-SMINT01                  
           MOVE 0                 TO       V0SOL-PARAM-SMINT02                  
           MOVE 0                 TO       V0SOL-PARAM-SMINT03                  
           MOVE 0                 TO       V0SOL-PARAM-INTG01.                  
           MOVE 0                 TO       V0SOL-PARAM-INTG02                   
           MOVE 0                 TO       V0SOL-PARAM-INTG03                   
           MOVE 0107100070673     TO       V0SOL-PARAM-DEC01                    
           MOVE 0                 TO       V0SOL-PARAM-DEC02                    
           MOVE 0                 TO       V0SOL-PARAM-DEC03                    
           MOVE 0                 TO       V0SOL-PARAM-FLOAT01                  
           MOVE 0                 TO       V0SOL-PARAM-FLOAT02                  
           MOVE ' '               TO       V0SOL-PARAM-CHAR01                   
           MOVE ' '               TO       V0SOL-PARAM-CHAR02                   
           MOVE ' '               TO       V0SOL-PARAM-CHAR03                   
           MOVE ' '               TO       V0SOL-PARAM-CHAR04                   
                                                                                
           PERFORM  R6995-INSERT-PARAMETRO.                                     
                                                                                
           ADD 1 TO  W-AC-LOTERICOS-GRAVADOS.                                   
           DISPLAY 'LOTERICO NAO DESEJA RENOVAR='CAD-CODIGO-CEF.                
      *                                                                         
       R6990-SAIDA. EXIT.                                                       
      *----------------------------------------------------------------*        
       R6995-INSERT-PARAMETRO   SECTION.                                        
      *---------------------------------                                        
      *                                                                         
           MOVE  '0008'  TO  WNR-EXEC-SQL.                                      
      *                                                                         
           EXEC SQL INSERT INTO  SEGUROS.LT_SOLICITA_PARAM                      
                   (COD_PRODUTO     ,                                           
                    COD_CLIENTE     ,                                           
                    COD_PROGRAMA    ,                                           
                    TIPO_SOLICITACAO,                                           
                    DATA_SOLICITACAO,                                           
                    COD_USUARIO     ,                                           
                    DATA_PREV_PROC  ,                                           
                    SIT_SOLICITACAO ,                                           
                    TSTMP_SITUACAO  ,                                           
                    PARAM_DATE01    ,                                           
                    PARAM_DATE02    ,                                           
                    PARAM_DATE03    ,                                           
                    PARAM_SMINT01   ,                                           
                    PARAM_SMINT02   ,                                           
                    PARAM_SMINT03   ,                                           
                    PARAM_INTG01    ,                                           
                    PARAM_INTG02    ,                                           
                    PARAM_INTG03    ,                                           
                    PARAM_DEC01     ,                                           
                    PARAM_DEC02     ,                                           
                    PARAM_DEC03     ,                                           
                    PARAM_FLOAT01   ,                                           
                    PARAM_FLOAT02   ,                                           
                    PARAM_CHAR01    ,                                           
                    PARAM_CHAR02    ,                                           
                    PARAM_CHAR03    ,                                           
                    PARAM_CHAR04)                                               
             VALUES (:V0SOL-COD-PRODUTO        ,                                
                     :V0SOL-COD-CLIENTE        ,                                
                     :V0SOL-COD-PROGRAMA       ,                                
                     :V0SOL-TIPO-SOLICITACAO   ,                                
                     :V0SOL-DATA-SOLICITACAO   ,                                
                     :V0SOL-COD-USUARIO        ,                                
                     :V0SOL-DATA-PREV-PROC     ,                                
                     :V0SOL-SIT-SOLICITACAO    ,                                
                      CURRENT  TIMESTAMP       ,                                
                     :V0SOL-PARAM-DATE01       ,                                
                     :V0SOL-PARAM-DATE02       ,                                
                     :V0SOL-PARAM-DATE03       ,                                
                     :V0SOL-PARAM-SMINT01      ,                                
                     :V0SOL-PARAM-SMINT02      ,                                
                     :V0SOL-PARAM-SMINT03      ,                                
                     :V0SOL-PARAM-INTG01       ,                                
                     :V0SOL-PARAM-INTG02       ,                                
                     :V0SOL-PARAM-INTG03       ,                                
                     :V0SOL-PARAM-DEC01        ,                                
                     :V0SOL-PARAM-DEC02        ,                                
                     :V0SOL-PARAM-DEC03        ,                                
                     :V0SOL-PARAM-FLOAT01      ,                                
                     :V0SOL-PARAM-FLOAT02      ,                                
                     :V0SOL-PARAM-CHAR01       ,                                
                     :V0SOL-PARAM-CHAR02       ,                                
                     :V0SOL-PARAM-CHAR03       ,                                
                     :V0SOL-PARAM-CHAR03)                                       
           END-EXEC.                                                            
      *                                                                         
           IF SQLCODE  NOT EQUAL  ZEROS                                         
              DISPLAY ' R6990-ERRO INSERT LT-SOLICITA_PARAM '                   
              DISPLAY ' COD. PROGRAMA   = ' V0SOL-COD-PROGRAMA                  
              DISPLAY ' DATA-PREV-PROC  =' V0SOL-DATA-SOLICITACAO               
              DISPLAY ' SEGURADO        =' V0SOL-COD-CLIENTE                    
              GO  TO  R9999-ROT-ERRO.                                           
      *                                                                         
       R6995-SAIDA. EXIT.                                                       
      *----------------------------------------------------------------*        
      *                                                                         
      *                                                                         
       R9000-OPEN-ARQUIVOS        SECTION.                                      
      *-------------------------------------                                    
      *                                                                         
           OPEN INPUT  CADASTRO.                                                
           OPEN OUTPUT RLT2000B.                                                
      *                                                                         
       R9000-SAIDA. EXIT.                                                       
      *----------------------------------------------------------------*        
      *                   ROTINA CLOSE ARQUIVOS                        *        
      *----------------------------------------------------------------*        
       R9100-CLOSE-ARQUIVOS       SECTION.                                      
      *-------------------------------------                                    
      *                                                                         
           CLOSE  CADASTRO  RLT2000B.                                           
      *                                                                         
       R9100-SAIDA. EXIT.                                                       
      *----------------------------------------------------------------*        
      *                   ROTINA DE ERRO E ABEND                       *        
      *----------------------------------------------------------------*        
       R9999-ROT-ERRO             SECTION.                                      
      *-------------------------------------                                    
      *                                                                         
           MOVE        SQLCODE           TO           WSQLCODE.                 
      *                                                                         
           DISPLAY     WABEND                                                   
      *                                                                         
           CLOSE  CADASTRO  RLT2000B.                                           
      *                                                                         
           EXEC  SQL   WHENEVER        SQLWARNING     CONTINUE END-EXEC.        
      *                                                                         
           EXEC  SQL   ROLLBACK WORK                  END-EXEC.                 
      *                                                                         
           MOVE        99              TO             RETURN-CODE.              
      *                                                                         
           STOP        RUN.                                                     
      *                                                                         
       R9999-SAIDA. EXIT.                                                       
