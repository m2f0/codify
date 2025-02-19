"""Vector store related tools."""
import os
import logging
from typing import Optional
from langchain_community.vectorstores import FAISS
from langchain_openai import OpenAIEmbeddings
from .file_tools import SimpleFileLoader

logger = logging.getLogger(__name__)

def initialize_vector_store(directory_path: str) -> Optional[FAISS]:
    """
    Inicializa o vectorstore com os arquivos do diretório.
    """
    try:
        documents = []
        
        # Carrega todos os arquivos .cs e COBOL
        for root, _, files in os.walk(directory_path):
            for file in files:
                if file.endswith(('.cs', '.cbl', '.cob', '.cobol')):
                    file_path = os.path.join(root, file)
                    loader = SimpleFileLoader(file_path)
                    documents.extend(loader.load())
        
        if not documents:
            logger.warning(f"Nenhum arquivo encontrado em {directory_path}")
            return None
        
        # Inicializa o vectorstore
        embeddings = OpenAIEmbeddings()
        vectorstore = FAISS.from_documents(documents, embeddings)
        
        logger.info(f"Vectorstore inicializado com {len(documents)} documentos")
        return vectorstore
    
    except Exception as e:
        logger.error(f"Erro ao inicializar vectorstore: {str(e)}")
        return None

def validate_csharp_directory(directory_path: str) -> bool:
    """
    Valida se o diretório contém arquivos C#.
    """
    try:
        if not os.path.exists(directory_path):
            logger.error(f"Diretório não encontrado: {directory_path}")
            return False
        
        has_csharp_files = False
        for root, _, files in os.walk(directory_path):
            if any(file.endswith('.cs') for file in files):
                has_csharp_files = True
                break
        
        if not has_csharp_files:
            logger.error(f"Nenhum arquivo C# encontrado em {directory_path}")
            return False
        
        return True
    
    except Exception as e:
        logger.error(f"Erro ao validar diretório: {str(e)}")
        return False