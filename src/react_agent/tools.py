"""Define tools for the agent."""
import asyncio
import traceback
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
from .state_manager import StateManager

logger = logging.getLogger(__name__)
state_manager = StateManager()

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
    project_path = os.path.join(base_directory, "LT2000B_20250205", "Sias.Loterico")
    
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

def read_file_content(file_name: str) -> str:
    """Lê o conteúdo de um arquivo."""
    base_dir = os.getenv("CSHARP_PROJECT_DIR", "./csharp_project")
    project_subdir = "LT2000B_20250205/Sias.Loterico"
    
    possible_paths = [
        file_name,  # caminho direto
        os.path.join(base_dir, file_name),  # na raiz do projeto
        os.path.join(base_dir, project_subdir, file_name),  # no diretório do projeto
        os.path.join(base_dir, project_subdir, "Classes", file_name),  # pasta Classes
        os.path.join(base_dir, project_subdir, "Models", file_name),   # pasta Models
    ]
    
    tried_paths = []
    for path in possible_paths:
        tried_paths.append(path)
        try:
            with codecs.open(path, 'r', encoding='utf-8') as file:
                return file.read()
        except (IOError, FileNotFoundError):
            continue
            
    logger.error(f"Arquivo não encontrado: {file_name}. Caminhos tentados: {tried_paths}")
    return None

def analyze_code_in_chunks(file_content: str, chunk_size: int = 50000):
    """
    Analisa o código em chunks menores para evitar exceder o limite de tokens.
    """
    chunks = [file_content[i:i + chunk_size] for i in range(0, len(file_content), chunk_size)]
    analysis = []
    
    for chunk in chunks:
        # Analisa cada chunk individualmente
        # Adicione aqui sua lógica de análise
        pass
    
    return "\n".join(analysis)

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

@traceable(name="analyze_build_errors")
def analyze_build_errors_and_suggest(build_output: str) -> dict:
    """Analisa os erros do build e retorna uma análise formatada."""
    analysis = {
        "files": {},
        "errors": []
    }
    
    # Processa o log do build para encontrar erros
    for line in build_output.split('\n'):
        if '.cs(' in line and 'error' in line.lower():
            match = re.search(r'([\w\.]+\.cs)\((\d+),.*?\): error (CS\d+): (.*)', line)
            if match:
                file_name = match.group(1)
                line_number = int(match.group(2))
                error_code = match.group(3)
                error_message = match.group(4)
                
                if file_name not in analysis["files"]:
                    file_content = read_file_content(file_name)
                    analysis["files"][file_name] = {
                        "content": file_content,
                        "errors": []
                    }
                
                # Extrai a linha original do código
                if file_content := analysis["files"][file_name]["content"]:
                    lines = file_content.split('\n')
                    original_line = lines[line_number - 1] if line_number <= len(lines) else None
                else:
                    original_line = None
                
                error_info = {
                    "line": line_number,
                    "code": error_code,
                    "message": error_message,
                    "original_line": original_line
                }
                
                analysis["files"][file_name]["errors"].append(error_info)
                analysis["errors"].append({
                    "file": file_name,
                    **error_info
                })
    
    # Atualiza o estado global através do StateManager
    state_manager.set_build_analysis(analysis)
    
    return analysis

def show_corrected_code(file_name: str, build_analysis: Optional[dict] = None) -> str:
    """Mostra o código corrigido para um arquivo específico."""
    if build_analysis is None:
        build_analysis = state_manager.get_build_analysis()
    
    if not build_analysis:
        return "Nenhuma análise de build disponível"
        
    if file_name not in build_analysis["files"]:
        return f"Não foram encontrados erros para o arquivo {file_name}"
        
    file_info = build_analysis["files"][file_name]
    if not file_info["content"]:
        base_dir = os.getenv("CSHARP_PROJECT_DIR", "./csharp_project")
        return (f"Não foi possível ler o conteúdo do arquivo {file_name}. "
                f"Verifique se o arquivo existe em {base_dir} ou suas subpastas.")
        
    result = []
    lines = file_info["content"].split('\n')
    
    for error in file_info["errors"]:
        line_num = error["line"]
        if line_num <= len(lines):
            original_line = lines[line_num - 1]
            corrected_line = original_line
            
            # Corrige comparações de tipo incorretas
            if error["code"] == "CS0019" and "!=" in original_line:
                # Extrai o tipo do objeto da mensagem de erro
                type_match = re.search(r"type '([^']+)' and 'string'", error["message"])
                if type_match:
                    object_type = type_match.group(1)
                    # Adiciona .ToString() antes da comparação com string
                    corrected_line = re.sub(
                        r'(\w+)\s*!=\s*"([^"]*)"',
                        r'\1.ToString() != "\2"',
                        original_line
                    )
            
            result.extend([
                f"Linha {line_num}:",
                f"Original: {original_line}",
                f"Corrigido: {corrected_line}",
                f"Erro: {error['code']} - {error['message']}\n"
            ])
    
    return "\n".join(result) if result else f"Não foram encontradas correções automáticas para os erros em {file_name}"

async def build_csharp_project(
    project_path: str,
    app_name: str,
    config: Dict[str, Any],
    session_id: str = None
) -> Dict[str, Any]:
    """
    Executa o build de um projeto C#.
    
    Args:
        project_path: Caminho para o diretório do projeto
        app_name: Nome do arquivo do projeto (.csproj)
        config: Configurações adicionais
        session_id: ID opcional da sessão de build
    
    Returns:
        Dict contendo o resultado do build
    """
    try:
        full_path = os.path.join(project_path, app_name)
        if not os.path.exists(full_path):
            return {
                "success": False,
                "output": f"Arquivo de projeto não encontrado: {full_path}",
                "session_id": session_id
            }
        
        # Configura o ambiente para usar UTF-8
        env = os.environ.copy()
        env["PYTHONIOENCODING"] = "utf-8"
        env["DOTNET_CLI_UI_LANGUAGE"] = "en"  # Força o idioma inglês para mensagens do dotnet
        
        # Executa o comando dotnet build com WarningLevel=0 para suprimir warnings
        process = await asyncio.create_subprocess_exec(
            "dotnet", "build", full_path,
            "-v", "detailed",
            "/p:WarningLevel=0",  # Suprime os warnings
            stdout=asyncio.subprocess.PIPE,
            stderr=asyncio.subprocess.PIPE,
            env=env  # Passa as variáveis de ambiente configuradas
        )
        
        stdout, stderr = await process.communicate()
        
        # Sempre usa UTF-8 para decodificação
        output = stdout.decode('utf-8', errors='replace')
        error_output = stderr.decode('utf-8', errors='replace')
        
        # Remove caracteres inválidos e normaliza quebras de linha
        output = output.replace('\r\n', '\n').replace('\r', '\n')
        error_output = error_output.replace('\r\n', '\n').replace('\r', '\n')
        
        combined_output = output + "\n" + error_output if error_output else output
        
        return {
            "success": process.returncode == 0,
            "output": combined_output,
            "session_id": session_id
        }
        
    except Exception as e:
        error_msg = f"Erro ao executar build: {str(e)}\n{traceback.format_exc()}"
        logger.error(error_msg)
        return {
            "success": False,
            "output": error_msg,
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
        name="analyze_build",
        description="Analisa os erros do último build e sugere correções",
        func=lambda: analyze_build_errors_and_suggest(
            last_build_result.get('output', '') if last_build_result else 'No build output available'
        ),
    ),
    Tool(
        name="show_corrected_code",
        description="Mostra o código corrigido para um arquivo específico",
        func=lambda file_name: show_corrected_code(file_name, last_build_analysis if 'last_build_analysis' in globals() else None)
    ),
    Tool(
        name="build_csharp_project",
        description="Executa o build de um projeto C#",
        func=lambda project_path, app_name, config, session_id: build_csharp_project(project_path, app_name, config, session_id)
    )
]
