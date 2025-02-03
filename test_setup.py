from dotenv import load_dotenv
from react_agent.tools import validate_csharp_directory, initialize_vector_store
import os

def main():
    load_dotenv()
    
    print("Validating C# project directory...")
    if validate_csharp_directory():
        directory_path = os.getenv("CSHARP_PROJECT_DIR", "./csharp_project")
        print("Initializing vector store...")
        vector_store = initialize_vector_store(directory_path)
        if vector_store:
            print("Setup completed successfully!")
        else:
            print("Setup failed: Could not initialize vector store")
    else:
        print("Setup failed: Invalid C# project directory")

if __name__ == "__main__":
    main()
