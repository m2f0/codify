from dotenv import load_dotenv
from react_agent.tools import list_vectorstore_files
import asyncio

def main():
    load_dotenv()
    
    print("Listing files in vector store:")
    files = list_vectorstore_files()
    for file in files:
        print(f"- {file}")

if __name__ == "__main__":
    main()
