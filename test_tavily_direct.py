from dotenv import load_dotenv
import os
from langchain_community.tools import TavilySearchResults
import asyncio

async def test_search():
    load_dotenv()
    tavily_key = os.getenv("TAVILY_API_KEY")
    
    search = TavilySearchResults(
        api_key=tavily_key,
        max_results=3
    )
    
    try:
        result = await search.ainvoke({"query": "What is Python?"})
        print("Busca bem sucedida!")
        print(result)
    except Exception as e:
        print(f"Erro ao fazer busca: {str(e)}")

if __name__ == "__main__":
    asyncio.run(test_search())