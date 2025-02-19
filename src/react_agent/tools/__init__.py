"""Tools package initialization."""
from .build_tools import build_csharp_project
from .analysis_tools import analyze_build_errors_and_suggest, show_corrected_code
from .file_tools import SimpleFileLoader, read_file_content, list_vectorstore_files
from .vector_tools import initialize_vector_store, validate_csharp_directory
from .search_tools import search

# Expose all tools
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
        func=lambda file_name: show_corrected_code(file_name, last_build_analysis)
    ),
    Tool(
        name="build_csharp_project",
        description="Executa o build de um projeto C#",
        func=lambda project_path, app_name, config, session_id: build_csharp_project(project_path, app_name, config, session_id)
    )
]

__all__ = [
    "TOOLS",
    "build_csharp_project",
    "analyze_build_errors_and_suggest",
    "show_corrected_code",
    "SimpleFileLoader",
    "read_file_content",
    "list_vectorstore_files",
    "initialize_vector_store",
    "validate_csharp_directory",
    "search"
]