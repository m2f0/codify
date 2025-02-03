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

@traceable(name="analyze_build_errors")
def analyze_build_errors(
    build_output: str,
    run_manager: Optional[CallbackManagerForToolRun] = None,
) -> Dict[str, Any]:
    """
    Analisa os erros de build do output fornecido.
    """
    try:
        errors = []
        current_error = []
        
        for line in build_output.split('\n'):
            line = line.strip()
            if 'error' in line.lower():
                if current_error:
                    errors.append('\n'.join(current_error))
                    current_error = []
                current_error.append(line)
            elif current_error and line:
                current_error.append(line)
                
        if current_error:
            errors.append('\n'.join(current_error))
            
        return {
            "success": True,
            "errors": errors,
            "count": len(errors),
            "summary": f"Encontrados {len(errors)} erros no build",
            "details": "\n\n".join(errors) if errors else "Nenhum erro encontrado"
        }
        
    except Exception as e:
        logger.error(f"Erro ao analisar build: {str(e)}")
        return {
            "success": False,
            "error": str(e),
            "message": "Falha ao analisar erros do build"
        }

@traceable(name="build_csharp_project")
async def build_csharp_project(
    project_path: str,
    app_name: str,
    config: Annotated[RunnableConfig, CallbackManagerForToolRun]
) -> Dict[str, Any]:
    """
    Executa o build do projeto C#.
    """
    try:
        # Implementação do build
        process = subprocess.Popen(
            ["dotnet", "build"],
            cwd=project_path,
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            text=True
        )
        
        stdout, stderr = process.communicate()
        output = stdout + stderr
        
        success = process.returncode == 0
        
        # Analisa os erros se o build falhou
        if not success:
            analysis = analyze_build_errors(output)
            return {
                "success": False,
                "output": output,
                "analysis": analysis
            }
        
        return {
            "success": True,
            "output": output,
            "message": "Build concluído com sucesso"
        }
        
    except Exception as e:
        logger.error(f"Erro no build: {str(e)}")
        return {
            "success": False,
            "error": str(e),
            "message": "Falha ao executar o build"
        }

# Define as ferramentas disponíveis
TOOLS = [
    Tool(
        name="search",
        description="Search for general web results",
        func=search,
    ),
    Tool(
        name="analyze_build_errors",
        description="Analisa os erros encontrados durante o processo de build do projeto C#",
        func=analyze_build_errors,
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
