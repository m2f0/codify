"""Vector store related tools."""
import os
import logging
import time
from typing import Optional, List, Dict, Any
from functools import lru_cache
from tenacity import retry, stop_after_attempt, wait_exponential
from langchain_community.vectorstores import FAISS
from langchain_openai import OpenAIEmbeddings
from .file_tools import SimpleFileLoader
from .common.config import Config

# Configuração detalhada do logger
logger = logging.getLogger(__name__)
handler = logging.StreamHandler()
formatter = logging.Formatter(
    '%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
handler.setFormatter(formatter)
logger.addHandler(handler)
logger.setLevel(logging.DEBUG)

class VectorStoreCache:
    """Cache para armazenar embeddings e resultados de busca."""
    def __init__(self, max_size: int = 1000):
        self._embeddings_cache: Dict[str, List[float]] = {}
        self._search_cache: Dict[str, List[Dict[str, Any]]] = {}
        self._max_size = max_size

    def get_embedding(self, text: str) -> Optional[List[float]]:
        return self._embeddings_cache.get(text)

    def set_embedding(self, text: str, embedding: List[float]) -> None:
        if len(self._embeddings_cache) >= self._max_size:
            # Remove o item mais antigo
            self._embeddings_cache.pop(next(iter(self._embeddings_cache)))
        self._embeddings_cache[text] = embedding

    def get_search_result(self, query: str) -> Optional[List[Dict[str, Any]]]:
        return self._search_cache.get(query)

    def set_search_result(self, query: str, results: List[Dict[str, Any]]) -> None:
        if len(self._search_cache) >= self._max_size:
            self._search_cache.pop(next(iter(self._search_cache)))
        self._search_cache[query] = results

# Instância global do cache
vector_cache = VectorStoreCache()

class CachedOpenAIEmbeddings(OpenAIEmbeddings):
    """Versão cacheada do OpenAIEmbeddings."""
    def __init__(self, **kwargs):
        super().__init__(**kwargs)
        self._cache = vector_cache

    @retry(
        stop=stop_after_attempt(3),
        wait=wait_exponential(multiplier=1, min=4, max=10),
        before_sleep=lambda retry_state: logger.warning(
            f"Tentativa {retry_state.attempt_number} falhou. Tentando novamente..."
        )
    )
    async def aembed_documents(self, texts: List[str]) -> List[List[float]]:
        """Versão assíncrona com cache para embed_documents."""
        results = []
        texts_to_embed = []
        indices = []

        # Verifica cache primeiro
        for i, text in enumerate(texts):
            cached = self._cache.get_embedding(text)
            if cached is not None:
                results.append(cached)
            else:
                texts_to_embed.append(text)
                indices.append(i)

        if texts_to_embed:
            try:
                new_embeddings = await super().aembed_documents(texts_to_embed)
                # Atualiza cache com novos embeddings
                for text, embedding in zip(texts_to_embed, new_embeddings):
                    self._cache.set_embedding(text, embedding)
                
                # Insere novos embeddings nos resultados
                for idx, embedding in zip(indices, new_embeddings):
                    results.insert(idx, embedding)
            except Exception as e:
                logger.error(f"Erro ao gerar embeddings: {str(e)}")
                raise

        return results

@retry(
    stop=stop_after_attempt(3),
    wait=wait_exponential(multiplier=1, min=4, max=10)
)
def initialize_vector_store(directory_path: str) -> Optional[FAISS]:
    """
    Inicializa o vectorstore com os arquivos do diretório.
    Inclui retry mechanism e logging detalhado.
    """
    start_time = time.time()
    logger.info(f"Iniciando inicialização do vector store para {directory_path}")
    
    try:
        documents = []
        file_count = 0
        error_count = 0
        
        # Carrega todos os arquivos .cs e COBOL
        for root, _, files in os.walk(directory_path):
            for file in files:
                if file.endswith(('.cs', '.cbl', '.cob', '.cobol')):
                    file_path = os.path.join(root, file)
                    try:
                        logger.debug(f"Processando arquivo: {file_path}")
                        loader = SimpleFileLoader(file_path)
                        file_documents = loader.load()
                        documents.extend(file_documents)
                        file_count += 1
                        logger.debug(f"Arquivo processado com sucesso: {file_path}")
                    except Exception as e:
                        error_count += 1
                        logger.error(f"Erro ao processar arquivo {file_path}: {str(e)}")
        
        if not documents:
            logger.warning(f"Nenhum documento encontrado em {directory_path}")
            return None
        
        logger.info(f"Total de arquivos processados: {file_count}")
        logger.info(f"Total de erros encontrados: {error_count}")
        
        # Inicializa o vectorstore com embeddings cacheados
        embeddings = CachedOpenAIEmbeddings()
        vectorstore = FAISS.from_documents(documents, embeddings)
        
        end_time = time.time()
        logger.info(f"Vector store inicializado com sucesso em {end_time - start_time:.2f} segundos")
        logger.info(f"Total de documentos indexados: {len(documents)}")
        
        return vectorstore
    
    except Exception as e:
        logger.error(f"Erro crítico ao inicializar vector store: {str(e)}")
        raise

def validate_csharp_directory() -> bool:
    """Valida se o diretório do projeto C# existe e contém arquivos necessários."""
    try:
        directory_path = os.getenv("CSHARP_PROJECT_DIR", "./csharp_project")
        if not os.path.exists(directory_path):
            logger.error(f"Diretório {directory_path} não encontrado")
            return False
        
        # Procura especificamente pelo arquivo Sias.Loterico.csproj
        csproj_files = []
        for root, _, files in os.walk(directory_path):
            for file in files:
                if file.lower() == "sias.loterico.csproj":
                    full_path = os.path.join(root, file)
                    csproj_files.append(full_path)
                    logger.info(f"Arquivo .csproj encontrado em: {full_path}")
        
        if not csproj_files:
            logger.error(f"Arquivo Sias.Loterico.csproj não encontrado em {directory_path}")
            return False
        
        # Lista todos os arquivos .cs
        cs_files = []
        for root, _, files in os.walk(directory_path):
            cs_files.extend([
                os.path.join(root, f) 
                for f in files 
                if f.endswith('.cs')
            ])
        
        if not cs_files:
            logger.error(f"Nenhum arquivo C# encontrado em {directory_path}")
            return False
        
        logger.info(f"Validação concluída. Encontrados {len(cs_files)} arquivos C#")
        logger.info(f"Arquivos .csproj encontrados: {csproj_files}")
        
        return True
    
    except Exception as e:
        logger.error(f"Erro ao validar diretório: {str(e)}")
        return False

@lru_cache(maxsize=1000)
def get_cached_file_content(file_path: str) -> Optional[str]:
    """
    Recupera o conteúdo do arquivo com cache.
    """
    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            return f.read()
    except Exception as e:
        logger.error(f"Erro ao ler arquivo {file_path}: {str(e)}")
        return None
