"""Search related tools."""
import logging
import time
from typing import Dict, Any, Optional, List
from dataclasses import dataclass
from datetime import datetime, timedelta
import aiohttp
import asyncio
from functools import lru_cache
from tenacity import retry, stop_after_attempt, wait_exponential
from langchain_community.tools import TavilySearchResults
from .common.config import Config

logger = logging.getLogger(__name__)

@dataclass
class SearchConfig:
    """Configuração para buscas."""
    max_results: int = 5
    search_depth: str = "advanced"  # basic, advanced, or comprehensive
    include_domains: Optional[List[str]] = None
    exclude_domains: Optional[List[str]] = None
    include_raw_content: bool = False
    max_tokens: int = 8000

class RateLimiter:
    """Implementa rate limiting para chamadas de API."""
    def __init__(self, calls_per_minute: int = 60):
        self.calls_per_minute = calls_per_minute
        self.calls = []
        self.lock = asyncio.Lock()

    async def acquire(self):
        """Adquire permissão para fazer uma chamada."""
        async with self.lock:
            now = datetime.now()
            # Remove chamadas antigas (mais de 1 minuto)
            self.calls = [call_time for call_time in self.calls 
                         if now - call_time < timedelta(minutes=1)]
            
            if len(self.calls) >= self.calls_per_minute:
                # Calcula tempo de espera
                oldest_call = self.calls[0]
                wait_time = 60 - (now - oldest_call).total_seconds()
                if wait_time > 0:
                    logger.warning(f"Rate limit atingido. Aguardando {wait_time:.2f} segundos")
                    await asyncio.sleep(wait_time)
            
            self.calls.append(now)

class SearchCache:
    """Cache para resultados de busca."""
    def __init__(self, max_size: int = 1000, ttl_minutes: int = 60):
        self._cache: Dict[str, Dict[str, Any]] = {}
        self._max_size = max_size
        self._ttl = timedelta(minutes=ttl_minutes)

    def get(self, key: str) -> Optional[Dict[str, Any]]:
        """Recupera resultado do cache."""
        if key in self._cache:
            result = self._cache[key]
            if datetime.now() - result["timestamp"] < self._ttl:
                logger.debug(f"Cache hit para query: {key}")
                return result["data"]
            else:
                logger.debug(f"Cache expirado para query: {key}")
                del self._cache[key]
        return None

    def set(self, key: str, value: Dict[str, Any]):
        """Armazena resultado no cache."""
        if len(self._cache) >= self._max_size:
            # Remove o item mais antigo
            oldest_key = min(self._cache.keys(), 
                           key=lambda k: self._cache[k]["timestamp"])
            del self._cache[oldest_key]
        
        self._cache[key] = {
            "data": value,
            "timestamp": datetime.now()
        }
        logger.debug(f"Resultado cacheado para query: {key}")

# Instâncias globais
rate_limiter = RateLimiter()
search_cache = SearchCache()

class EnhancedTavilySearch:
    """Implementação melhorada da busca Tavily."""
    def __init__(self, config: SearchConfig = SearchConfig()):
        self.config = config
        self._session: Optional[aiohttp.ClientSession] = None

    async def _ensure_session(self):
        """Garante que existe uma sessão HTTP ativa."""
        if self._session is None or self._session.closed:
            self._session = aiohttp.ClientSession()

    @retry(
        stop=stop_after_attempt(3),
        wait=wait_exponential(multiplier=1, min=4, max=10)
    )
    async def search(self, query: str) -> Dict[str, Any]:
        """
        Realiza busca com rate limiting, cache e retry mechanism.
        """
        # Verifica cache primeiro
        cache_key = f"{query}_{self.config.search_depth}_{self.config.max_results}"
        cached_result = search_cache.get(cache_key)
        if cached_result:
            return cached_result

        # Aplica rate limiting
        await rate_limiter.acquire()

        try:
            await self._ensure_session()
            
            search_tool = TavilySearchResults(
                max_results=self.config.max_results,
                search_depth=self.config.search_depth,
                include_domains=self.config.include_domains,
                exclude_domains=self.config.exclude_domains,
                include_raw_content=self.config.include_raw_content,
                max_tokens=self.config.max_tokens
            )

            start_time = time.time()
            results = await search_tool.ainvoke({"query": query})
            end_time = time.time()

            logger.info(f"Busca concluída em {end_time - start_time:.2f} segundos")
            logger.debug(f"Resultados encontrados: {len(results)}")

            # Armazena no cache
            search_cache.set(cache_key, results)
            
            return results

        except Exception as e:
            logger.error(f"Erro na busca: {str(e)}")
            raise
        
        finally:
            if self._session and not self._session.closed:
                await self._session.close()

@lru_cache(maxsize=100)
def create_search_config(
    max_results: int = 5,
    search_depth: str = "advanced",
    include_domains: Optional[List[str]] = None,
    exclude_domains: Optional[List[str]] = None
) -> SearchConfig:
    """
    Cria e cacheia configurações de busca.
    """
    return SearchConfig(
        max_results=max_results,
        search_depth=search_depth,
        include_domains=include_domains,
        exclude_domains=exclude_domains
    )

async def search(
    query: str,
    config: Optional[SearchConfig] = None
) -> Dict[str, Any]:
    """
    Interface principal para realizar buscas.
    """
    search_config = config or create_search_config()
    searcher = EnhancedTavilySearch(search_config)
    return await searcher.search(query)

async def batch_search(
    queries: List[str],
    config: Optional[SearchConfig] = None
) -> List[Dict[str, Any]]:
    """
    Realiza múltiplas buscas em paralelo.
    """
    search_config = config or create_search_config()
    searcher = EnhancedTavilySearch(search_config)
    
    tasks = [searcher.search(query) for query in queries]
    return await asyncio.gather(*tasks)
