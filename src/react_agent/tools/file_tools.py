"""File handling tools."""
import os
import logging
from typing import List
from langchain.schema import Document

logger = logging.getLogger(__name__)

class SimpleFileLoader:
    """Carregador simples de arquivos."""
    def __init__(self, file_path: str):
        self.file_path = file_path

    def load(self) -> List[Document]:
        """Carrega um arquivo e retorna como Document."""
        try:
            with open(self.file_path, 'r', encoding='utf-8') as file:
                content = file.read()
                metadata = {"source": self.file_path}
                return [Document(page_content=content, metadata=metadata)]
        except Exception as e:
            logger.error(f"Erro ao carregar arquivo {self.file_path}: {str(e)}")
            return []

def read_file_content(file_name: str) -> str:
    """
    Lê o conteúdo de um arquivo específico.
    """
    try:
        base_dir = os.getenv("CSHARP_PROJECT_DIR", "./csharp_project")
        
        # Procura o arquivo recursivamente
        for root, _, files in os.walk(base_dir):
            if file_name in files:
                file_path = os.path.join(root, file_name)
                with open(file_path, 'r', encoding='utf-8') as file:
                    return file.read()
        
        return f"Arquivo {file_name} não encontrado em {base_dir}"
    
    except Exception as e:
        logger.error(f"Erro ao ler arquivo {file_name}: {str(e)}")
        return f"Erro ao ler arquivo {file_name}: {str(e)}"

def list_vectorstore_files() -> List[str]:
    """
    Lista todos os arquivos disponíveis no vectorstore.
    """
    try:
        base_dir = os.getenv("CSHARP_PROJECT_DIR", "./csharp_project")
        files = []
        
        for root, _, filenames in os.walk(base_dir):
            for filename in filenames:
                if filename.endswith(('.cs', '.cbl', '.cob', '.cobol')):
                    relative_path = os.path.relpath(os.path.join(root, filename), base_dir)
                    files.append(relative_path)
        
        return files
    
    except Exception as e:
        logger.error(f"Erro ao listar arquivos: {str(e)}")
        return []