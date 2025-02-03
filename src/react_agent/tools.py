"""This module provides example tools for web scraping and search functionality.

It includes a basic Tavily search function (as an example)

These tools are intended as free examples to get started. For production use,
consider implementing more robust and specialized tools tailored to your needs.
"""

from typing import Any, Callable, Dict, List, Optional, cast
from langchain_community.document_loaders import DirectoryLoader, TextLoader
from langchain_community.vectorstores import FAISS
from langchain_openai import OpenAIEmbeddings  # Atualizado para nova importação
from langchain.text_splitter import Language, RecursiveCharacterTextSplitter
from langchain_core.runnables import RunnableConfig
from langchain_core.tools import InjectedToolArg
from typing_extensions import Annotated
from langsmith.run_helpers import traceable
from langchain_community.tools import TavilySearchResults
import os
import subprocess
import time
from pathlib import Path

from react_agent.configuration import Configuration


async def search(
    query: str, *, config: Annotated[RunnableConfig, InjectedToolArg]
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
        print(f"Erro na busca Tavily: {str(e)}")
        return None


_vector_store = None

@traceable(name="validate_csharp_directory")
def validate_csharp_directory() -> bool:
    """Validate if the C# project directory exists and contains expected files."""
    directory_path = os.getenv("CSHARP_PROJECT_DIR", "./csharp_project")
    
    if not os.path.exists(directory_path):
        print(f"Error: Directory {directory_path} does not exist")
        return False
        
    # Verificar arquivo .sln
    sln_files = [f for f in os.listdir(directory_path) if f.endswith('.sln')]
    if not sln_files:
        print(f"Error: No .sln file found in {directory_path}")
        return False
        
    # Verificar pasta ConsoleApp
    console_app_path = os.path.join(directory_path, "ConsoleApp")
    if not os.path.exists(console_app_path):
        print(f"Error: ConsoleApp directory not found in {directory_path}")
        return False
        
    # Verificar arquivo .csproj
    csproj_path = os.path.join(console_app_path, "ConsoleApp.csproj")
    if not os.path.exists(csproj_path):
        print(f"Error: ConsoleApp.csproj not found in {console_app_path}")
        return False
        
    print(f"C# project directory validated successfully: {directory_path}")
    return True

@traceable(name="initialize_vector_store")
def initialize_vector_store(directory_path: str) -> Optional[FAISS]:
    """Initialize the vector store with C# codebase."""
    try:
        print(f"Initializing vector store from directory: {directory_path}")
        
        if not validate_csharp_directory():
            print(f"Directory validation failed for: {directory_path}")
            raise ValueError("Invalid C# project directory configuration")
        
        print("Loading documents...")
        loader = DirectoryLoader(
            path=directory_path,
            glob="**/*.*",
            show_progress=True,
            use_multithreading=True,
            loader_cls=TextLoader,
            loader_kwargs={'autodetect_encoding': True},
            load_hidden=False,
            silent_errors=True
        )
        documents = loader.load()
        print(f"Loaded {len(documents)} documents")
        
        documents = [doc for doc in documents if _is_code_file(doc.metadata["source"])]
        print(f"Filtered {len(documents)} C# related documents")
        
        if not documents:
            print("Warning: No C# related documents found after filtering")
            return None
        
        # Corrigido o text_splitter removendo is_separator_regex
        text_splitter = RecursiveCharacterTextSplitter.from_language(
            language=Language.C,  # Usando C como mais próximo do C#
            chunk_size=1000,
            chunk_overlap=200,
            length_function=len
        )
        
        texts = text_splitter.split_documents(documents)
        print(f"Split into {len(texts)} chunks")
        
        if not texts:
            print("Warning: No text chunks were created")
            return None
            
        try:
            print("Creating vector store with OpenAI embeddings...")
            embeddings = OpenAIEmbeddings()
            vector_store = FAISS.from_documents(texts, embeddings)
            
            vector_store_path = os.path.join(directory_path, ".vector_store")
            os.makedirs(vector_store_path, exist_ok=True)
            vector_store.save_local(vector_store_path)
            print(f"Vector store saved to {vector_store_path}")
            
            return vector_store
            
        except Exception as e:
            print(f"Error creating vector store: {str(e)}")
            return None
            
    except Exception as e:
        print(f"Error in initialize_vector_store: {str(e)}")
        return None

@traceable(name="get_vector_store")
def get_vector_store() -> Optional[FAISS]:
    """Get or create the vector store singleton."""
    global _vector_store
    try:
        if _vector_store is None:
            directory_path = os.getenv("CSHARP_PROJECT_DIR", "./csharp_project")
            if not directory_path:
                print("Error: CSHARP_PROJECT_DIR environment variable not set")
                return None
                
            vector_store_path = os.path.join(directory_path, ".vector_store")
            if os.path.exists(vector_store_path):
                print("Loading existing vector store...")
                try:
                    _vector_store = FAISS.load_local(
                        vector_store_path,
                        OpenAIEmbeddings()
                    )
                except Exception as e:
                    print(f"Error loading existing vector store: {str(e)}")
                    print("Attempting to create new vector store...")
                    _vector_store = initialize_vector_store(directory_path)
            else:
                print("Creating new vector store...")
                _vector_store = initialize_vector_store(directory_path)
        
        return _vector_store
        
    except Exception as e:
        print(f"Error in get_vector_store: {str(e)}")
        return None

@traceable(name="search_csharp_code")
async def search_csharp_code(
    query: str,
    *,
    config: Annotated[RunnableConfig, InjectedToolArg]
) -> Optional[List[dict[str, Any]]]:
    """Search for information in C# codebase."""
    try:
        vector_store = get_vector_store()
        if vector_store is None:
            print("Error: Vector store not available")
            return None
            
        results = vector_store.similarity_search_with_score(query, k=5)
        
        return [
            {
                "content": doc.page_content,
                "source": doc.metadata.get("source", "unknown"),
                "relevance_score": float(score)
            }
            for doc, score in results
        ]
    except Exception as e:
        print(f"Error in search_csharp_code: {str(e)}")
        return None

@traceable(name="is_code_file")
def _is_code_file(file_path: str) -> bool:
    """Check if the file is a C# related file."""
    code_extensions = {
        '.cs',      # C# source files
        '.csproj',  # C# project files
        '.sln',     # Solution files
        '.config',  # Configuration files
        '.json',    # JSON files (settings, configs)
        '.xml',     # XML files
        '.cshtml',  # Razor views
        '.razor',   # Razor components
        '.xaml'     # XAML files
    }
    return os.path.splitext(file_path)[1].lower() in code_extensions

@traceable(name="build_csharp_project")
async def build_csharp_project(
    project_path: str = "",
    *,
    config: Annotated[RunnableConfig, InjectedToolArg]
) -> Dict[str, Any]:
    """Build C# project and monitor for build errors.
    
    Args:
        project_path: Optional path to specific .csproj file. If empty, builds entire solution.
        config: Configuration injected by the runtime.
    
    Returns:
        Dict containing build results including:
        - success: Boolean indicating if build succeeded
        - message: Summary of build result
        - errors: List of build errors if any
        - warnings: List of build warnings if any
        - full_output: Complete build output
    """
    try:
        # Validate C# environment first
        if not validate_csharp_directory():
            return {
                "success": False,
                "error": "Invalid C# project directory configuration"
            }

        directory_path = os.getenv("CSHARP_PROJECT_DIR", "./csharp_project")
        
        # Determine what to build (specific project or entire solution)
        if project_path:
            full_project_path = Path(directory_path) / project_path
            if not full_project_path.exists():
                return {
                    "success": False,
                    "error": f"Project file not found: {full_project_path}"
                }
            build_target = str(full_project_path)
        else:
            # Find .sln file
            sln_files = list(Path(directory_path).glob("*.sln"))
            if not sln_files:
                return {
                    "success": False,
                    "error": "No .sln file found in project directory"
                }
            build_target = str(sln_files[0])

        # Start build process
        process = subprocess.Popen(
            ["dotnet", "build", build_target, "--verbosity", "detailed"],
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            text=True,
            bufsize=1,
            universal_newlines=True
        )

        build_output = []
        errors = []
        warnings = []

        # Monitor build output in real-time
        while True:
            output = process.stdout.readline()
            if output == '' and process.poll() is not None:
                break
            if output:
                line = output.strip()
                build_output.append(line)
                
                # Parse for errors and warnings
                if ": error" in line:
                    errors.append(line)
                elif ": warning" in line:
                    warnings.append(line)

        # Get final return code and any remaining output
        return_code = process.poll()
        
        result = {
            "success": return_code == 0,
            "build_target": build_target,
            "return_code": return_code,
            "errors": errors,
            "warnings": warnings,
            "full_output": build_output
        }

        # Add summary message
        if result["success"]:
            result["message"] = "Build completed successfully"
            if warnings:
                result["message"] += f" with {len(warnings)} warnings"
        else:
            result["message"] = f"Build failed with {len(errors)} errors"
            if warnings:
                result["message"] += f" and {len(warnings)} warnings"

        return result

    except Exception as e:
        return {
            "success": False,
            "error": str(e),
            "message": "Build process failed due to an unexpected error"
        }

@traceable(name="list_vectorstore_files")
def list_vectorstore_files(*args, **kwargs) -> List[str]:
    """Lista todos os arquivos indexados no vector store."""
    try:
        vector_store = get_vector_store()
        if vector_store is None:
            return ["Erro: Vector store não disponível"]
            
        # Busca todos os documentos
        results = vector_store.similarity_search("", k=1000)
        
        # Extrai caminhos únicos
        files = set()
        for doc in results:
            source = doc.metadata.get("source", "unknown")
            if source != "unknown":
                files.add(source)
                
        return sorted(list(files))
        
    except Exception as e:
        logger.error(f"Erro ao listar arquivos: {str(e)}")
        return [f"Erro ao listar arquivos: {str(e)}"]

@traceable(name="read_file_content")
def read_file_content(file_path: str) -> Dict[str, Any]:
    """Lê e retorna o conteúdo de um arquivo específico."""
    try:
        directory_path = os.getenv("CSHARP_PROJECT_DIR", "./csharp_project")
        
        # Se o caminho começa com "csharp_project", remove essa parte
        if file_path.startswith("csharp_project"):
            file_path = file_path[len("csharp_project"):].lstrip('/\\')
            
        full_path = os.path.join(directory_path, file_path)
        full_path = os.path.normpath(full_path)
        
        if not os.path.exists(full_path):
            return {
                "success": False,
                "error": f"Arquivo não encontrado: {full_path}"
            }
            
        if not _is_code_file(full_path):
            return {
                "success": False,
                "error": "Não é um arquivo de código válido"
            }
            
        with open(full_path, 'r', encoding='utf-8') as f:
            content = f.read()
            
        return {
            "success": True,
            "content": content,
            "file_path": file_path
        }
        
    except Exception as e:
        return {
            "success": False,
            "error": str(e)
        }

TOOLS: List[Callable[..., Any]] = [
    search,
    search_csharp_code,
    build_csharp_project,
    list_vectorstore_files,
    read_file_content
]
