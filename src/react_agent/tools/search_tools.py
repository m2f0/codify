"""Search related tools."""
import logging
from typing import Dict, Any
from langchain_community.tools import TavilySearchResults

logger = logging.getLogger(__name__)

async def search(query: str) -> Dict[str, Any]:
    """
    Realiza uma busca web usando Tavily.
    """
    try:
        search_tool = TavilySearchResults(max_results=3)
        results = await search_tool.ainvoke({"query": query})
        return results
    
    except Exception as e:
        logger.error(f"Erro na busca: {str(e)}")
        return {"error": f"Falha na busca: {str(e)}"}