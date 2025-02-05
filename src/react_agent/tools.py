"""Define tools for the agent."""
from typing import Optional, Dict, Any, List, Annotated
from langchain_core.callbacks import CallbackManagerForToolRun
from langchain_core.runnables import RunnableConfig
from langsmith.run_helpers import traceable
from langchain.tools import Tool
import os
import subprocess
import logging
from langchain_community.vectorstores import FAISS
from langchain_openai import OpenAIEmbeddings
from langchain.schema import Document
import codecs
import locale
import re

logger = logging.getLogger(__name__)

class SimpleFileLoader:
    def __init__(self, file_path: str):
        self.file_path = file_path

    def load(self) -> List[Document]:
        try:
            with open(self.file_path, 'r', encoding='utf-8') as file:
                content = file.read()
                metadata = {"source": self.file_path}
                return [Document(page_content=content, metadata=metadata)]
        except Exception as e:
            logger.error(f"Erro ao carregar {self.file_path}: {str(e)}")
            return []

class VectorStoreInitError(Exception):
    """Erro ao inicializar o vector store."""
    pass

def validate_csharp_directory() -> bool:
    """
    Valida se o diretório do projeto C# existe e contém arquivos necessários.
    """
    base_directory = os.getenv("CSHARP_PROJECT_DIR", "./csharp_project")
    project_path = os.path.join(base_directory, "LT2000B", "Sias.Loterico")
    
    if not os.path.exists(project_path):
        logger.error(f"Diretório {project_path} não encontrado")
        return False
        
    # Verifica se existe o arquivo .csproj específico
    csproj_path = os.path.join(project_path, "Sias.Loterico.csproj")
    if not os.path.exists(csproj_path):
        logger.error(f"Arquivo Sias.Loterico.csproj não encontrado em {project_path}")
        return False
        
    return True

def initialize_vector_store(directory_path: str) -> Optional[FAISS]:
    """
    Inicializa o vector store com os arquivos do diretório.
    """
    try:
        documents = []
        for root, _, files in os.walk(directory_path):
            for file in files:
                if file.endswith('.cs'):
                    file_path = os.path.join(root, file)
                    try:
                        loader = SimpleFileLoader(file_path)
                        documents.extend(loader.load())
                    except Exception as e:
                        logger.error(f"Erro ao carregar {file_path}: {str(e)}")
        
        if not documents:
            logger.error("Nenhum documento carregado")
            return None
            
        embeddings = OpenAIEmbeddings()
        vector_store = FAISS.from_documents(documents, embeddings)
        
        return vector_store
        
    except Exception as e:
        logger.error(f"Erro ao inicializar vector store: {str(e)}")
        raise VectorStoreInitError(str(e))

def list_vectorstore_files() -> List[str]:
    """
    Lista todos os arquivos indexados no vector store.
    """
    directory_path = os.getenv("CSHARP_PROJECT_DIR", "./csharp_project")
    files = []
    
    for root, _, filenames in os.walk(directory_path):
        for filename in filenames:
            if filename.endswith('.cs'):
                relative_path = os.path.relpath(
                    os.path.join(root, filename),
                    directory_path
                )
                files.append(relative_path)
                
    return files

def read_file_content(file_path: str) -> str:
    """
    Lê o conteúdo de um arquivo específico.
    """
    try:
        directory_path = os.getenv("CSHARP_PROJECT_DIR", "./csharp_project")
        full_path = os.path.join(directory_path, file_path)
        
        if not os.path.exists(full_path):
            return f"Arquivo não encontrado: {file_path}"
            
        with open(full_path, 'r', encoding='utf-8') as file:
            return file.read()
            
    except Exception as e:
        logger.error(f"Erro ao ler arquivo {file_path}: {str(e)}")
        return f"Erro ao ler arquivo: {str(e)}"

async def search(
    query: str, 
    *, 
    config: Annotated[RunnableConfig, CallbackManagerForToolRun]
) -> Optional[list[dict[str, Any]]]:
    """Search for general web results."""
    configuration = Configuration.from_runnable_config(config)
    
    tavily_key = os.getenv("TAVILY_API_KEY")
    if not tavily_key:
        raise ValueError("TAVILY_API_KEY não está definida")
    
    try:
        wrapped = TavilySearchResults(
            api_key=tavily_key,
            max_results=configuration.max_search_results,
            k=configuration.max_search_results
        )
        result = await wrapped.ainvoke({"query": query})
        return cast(list[dict[str, Any]], result)
    except Exception as e:
        logger.error(f"Erro na busca Tavily: {str(e)}")
        return None

@traceable(name="analyze_build_errors_and_suggest")
def analyze_build_errors_and_suggest(
    build_output: str,
    run_manager: Optional[CallbackManagerForToolRun] = None,
) -> Dict[str, Any]:
    """
    Analisa os erros de build e propõe soluções para cada tipo de erro encontrado.
    
    Args:
        build_output: Output do processo de build
        run_manager: Gerenciador de callbacks opcional
    
    Returns:
        Dict contendo análise dos erros e sugestões de correção
    """
    try:
        # Estrutura para armazenar erros categorizados
        error_analysis = {
            "compilation_errors": [],
            "reference_errors": [],
            "syntax_errors": [],
            "other_errors": [],
            "suggestions": []
        }
        
        current_error = []
        
        for line in build_output.split('\n'):
            line = line.strip()
            
            # Identifica início de um novo erro
            if 'error' in line.lower():
                if current_error:
                    error_info = analyze_single_error('\n'.join(current_error))
                    categorize_error(error_info, error_analysis)
                current_error = [line]
            elif current_error and line:
                current_error.append(line)
                
        # Processa o último erro
        if current_error:
            error_info = analyze_single_error('\n'.join(current_error))
            categorize_error(error_info, error_analysis)
            
        # Gera sugestões baseadas nos erros encontrados
        generate_suggestions(error_analysis)
        
        return {
            "success": True,
            "analysis": error_analysis,
            "summary": {
                "total_errors": sum(len(errors) for errors in error_analysis.values()),
                "compilation_errors": len(error_analysis["compilation_errors"]),
                "reference_errors": len(error_analysis["reference_errors"]),
                "syntax_errors": len(error_analysis["syntax_errors"]),
                "other_errors": len(error_analysis["other_errors"])
            },
            "suggestions": error_analysis["suggestions"]
        }
        
    except Exception as e:
        logger.error(f"Erro ao analisar build e gerar sugestões: {str(e)}")
        return {
            "success": False,
            "error": str(e),
            "message": "Falha ao analisar erros do build e gerar sugestões"
        }

def analyze_single_error(error_text: str) -> Dict[str, str]:
    """
    Analisa um único erro e extrai informações relevantes.
    """
    error_info = {
        "type": "unknown",
        "message": error_text,
        "file": "",
        "line": "",
        "code": ""
    }
    
    # Extrai informações do erro usando regex
    if "CS" in error_text:  # Erro de compilação C#
        match = re.search(r'(CS\d+)', error_text)
        if match:
            error_info["code"] = match.group(1)
            
        # Extrai nome do arquivo e linha
        file_match = re.search(r'([\w\.]+\.cs)\((\d+),', error_text)
        if file_match:
            error_info["file"] = file_match.group(1)
            error_info["line"] = file_match.group(2)
            
    return error_info

def categorize_error(error_info: Dict[str, str], analysis: Dict[str, List]) -> None:
    """
    Categoriza o erro com base nas informações extraídas.
    """
    if error_info["code"].startswith("CS"):
        code = error_info["code"]
        if code in ["CS0246", "CS0234"]:  # Erros de referência
            analysis["reference_errors"].append(error_info)
        elif code in ["CS1002", "CS1513"]:  # Erros de sintaxe
            analysis["syntax_errors"].append(error_info)
        else:
            analysis["compilation_errors"].append(error_info)
    else:
        analysis["other_errors"].append(error_info)

def generate_suggestions(analysis: Dict[str, List]) -> None:
    """
    Gera sugestões de correção baseadas nos erros encontrados.
    """
    suggestions = []
    
    # Sugestões para erros de referência
    if analysis["reference_errors"]:
        suggestions.append({
            "type": "reference",
            "message": "Verificar se todas as dependências estão corretamente referenciadas no projeto",
            "actions": [
                "Verificar o arquivo .csproj para garantir que todas as referências estão presentes",
                "Executar 'dotnet restore' para restaurar os pacotes NuGet",
                "Verificar se os namespaces estão corretamente importados nos arquivos"
            ]
        })
    
    # Sugestões para erros de sintaxe
    if analysis["syntax_errors"]:
        suggestions.append({
            "type": "syntax",
            "message": "Corrigir problemas de sintaxe no código",
            "actions": [
                "Verificar chaves e parênteses não fechados",
                "Verificar pontuação e ponto-e-vírgula",
                "Utilizar um editor com realce de sintaxe para identificar erros"
            ]
        })
    
    analysis["suggestions"] = suggestions

@traceable(name="build_csharp_project")
async def build_csharp_project(
    project_path: str,
    app_name: str,
    config: Annotated[RunnableConfig, CallbackManagerForToolRun],
    session_id: str
) -> Dict[str, Any]:
    """
    Executa o build do projeto C#.
    """
    try:
        logger.info(f"[Session: {session_id}] Starting build in {project_path}")
        
        # Configurar o encoding correto para o subprocess
        startup_info = None
        if os.name == 'nt':  # Windows
            startup_info = subprocess.STARTUPINFO()
            startup_info.dwFlags |= subprocess.STARTF_USESHOWWINDOW
        
        process = subprocess.Popen(
            ["dotnet", "build"],
            cwd=project_path,
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            startupinfo=startup_info,
            encoding='utf-8',  # Forçar UTF-8
            errors='replace'   # Substituir caracteres inválidos
        )
        
        stdout, stderr = process.communicate()
        
        # Normalizar a saída para UTF-8
        def normalize_output(text: str) -> str:
            if text:
                # Converter para unicode e normalizar
                return text.encode('utf-8', errors='replace').decode('utf-8')
            return ""
        
        stdout = normalize_output(stdout)
        stderr = normalize_output(stderr)
        output = stdout + stderr
        
        success = process.returncode == 0
        
        logger.info(f"[Session: {session_id}] Build completed with status: {'success' if success else 'failed'}")
        
        if not success:
            # Alterado para usar analyze_build_errors_and_suggest ao invés de analyze_build_errors
            analysis = analyze_build_errors_and_suggest(output)
            logger.error(f"[Session: {session_id}] Build failed with errors: {analysis}")
            return {
                "success": False,
                "output": output,
                "analysis": analysis,
                "session_id": session_id
            }
        
        return {
            "success": True,
            "output": output,
            "message": "Build concluído com sucesso",
            "session_id": session_id
        }
        
    except Exception as e:
        logger.error(f"[Session: {session_id}] Build error: {str(e)}")
        return {
            "success": False,
            "error": str(e),
            "message": "Falha ao executar o build",
            "session_id": session_id
        }

# Define as ferramentas disponíveis
TOOLS = [
    Tool(
        name="search",
        description="Search for general web results",
        func=search,
    ),
    Tool(
        name="analyze_build_errors_and_suggest",
        description="Analisa os erros encontrados durante o processo de build do projeto C# e propõe soluções",
        func=analyze_build_errors_and_suggest,
    ),
    Tool(
        name="build_csharp_project",
        description="Executa o build do projeto C#",
        func=build_csharp_project,
    ),
    Tool(
        name="list_files",
        description="Lista todos os arquivos indexados",
        func=list_vectorstore_files,
    ),
    Tool(
        name="read_file",
        description="Lê o conteúdo de um arquivo específico",
        func=read_file_content,
    )
]
