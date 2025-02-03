from dotenv import load_dotenv
from react_agent.graph import graph
import asyncio

async def main():
    load_dotenv()
    
    response = await graph.ainvoke(
        {"messages": [("user", "List all files in the vector store")]},
        {"configurable": {"system_prompt": "You are a helpful AI assistant. Use the list_vectorstore_files tool to show all indexed C# files."}}
    )
    
    print(response["messages"][-1].content)

if __name__ == "__main__":
    asyncio.run(main())