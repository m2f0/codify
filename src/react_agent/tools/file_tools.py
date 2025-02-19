"""
Módulo de ferramentas para manipulação de arquivos.

Este módulo fornece classes e funções para carregamento e manipulação
de arquivos de código fonte, com suporte a diferentes encodings.

Typical usage example:

    loader = SimpleFileLoader("/path/to/file.cs")
    documents = loader.load()
    for doc in documents:
        process_content(doc.page_content)
"""

from ..utils.validators import InputValidator, ValidationError, FileType
from ..utils import EncodingHandler
from typing import List, Optional
from langchain.schema import Document
import logging
import os

logger = logging.getLogger(__name__)

class SimpleFileLoader:
    """Carregador de arquivos com suporte a múltiplos encodings.
    
    Esta classe gerencia o carregamento de arquivos de código fonte,
    lidando com diferentes encodings e validando o conteúdo.
    
    Attributes:
        file_path: Caminho completo do arquivo
        encoding_handler: Handler para detecção de encoding
    
    Examples:
        >>> loader = SimpleFileLoader("/path/to/file.cs")
        >>> docs = loader.load()
        >>> for doc in docs:
        ...     print(doc.page_content)
    """
    
    def __init__(self, file_path: str):
        """Inicializa o loader de arquivo.
        
        Args:
            file_path: Caminho do arquivo a ser carregado
            
        Raises:
            ValidationError: Se o caminho for inválido
        """
        validation_result = InputValidator.validate_path(file_path, must_exist=True)
        if not validation_result.is_valid:
            raise ValidationError(validation_result.message)
        self.file_path = str(validation_result.value)
        self.encoding_handler = EncodingHandler()

    def load(self) -> List[Document]:
        """Load file content with proper encoding handling."""
        try:
            content = EncodingHandler.read_file(self.file_path)
            if content is None:
                return []
                
            metadata = {"source": self.file_path}
            return [Document(page_content=content, metadata=metadata)]
        except Exception as e:
            logger.error(f"Error loading {self.file_path}: {str(e)}")
            return []

def read_file_content(file_name: str) -> str:
    """Lê conteúdo de arquivo com validações."""
    # Valida nome do arquivo
    file_name = InputValidator.sanitize_filename(file_name)
    
    try:
        base_dir = os.getenv("CSHARP_PROJECT_DIR", "./csharp_project")
        
        # Valida diretório base
        dir_validation = InputValidator.validate_path(base_dir, must_exist=True)
        if not dir_validation.is_valid:
            return f"Erro: {dir_validation.message}"
        
        # Procura arquivo
        for root, _, files in os.walk(base_dir):
            if file_name in files:
                file_path = os.path.join(root, file_name)
                
                # Valida arquivo encontrado
                file_validation = InputValidator.validate_path(
                    file_path,
                    must_exist=True
                )
                if not file_validation.is_valid:
                    return f"Erro: {file_validation.message}"
                
                # Lê e valida conteúdo
                with open(file_path, 'r', encoding='utf-8') as f:
                    content = f.read()
                    
                content_validation = InputValidator.validate_code_content(content)
                if not content_validation.is_valid:
                    return f"Erro: {content_validation.message}"
                    
                return content
                
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
