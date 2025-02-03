import logging
from dotenv import load_dotenv
from react_agent.tools import initialize_vector_store, validate_csharp_directory, VectorStoreInitError
import os

logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)

def main():
    load_dotenv()
    
    try:
        logger.info("Validating C# project directory...")
        if not validate_csharp_directory():
            logger.error("Invalid C# project directory")
            return
            
        directory_path = os.getenv("CSHARP_PROJECT_DIR", "./csharp_project")
        logger.info("Initializing vector store...")
        
        vector_store = initialize_vector_store(directory_path)
        if vector_store:
            logger.info("Setup completed successfully!")
        else:
            logger.error("Setup failed: Could not initialize vector store")
            
    except VectorStoreInitError as e:
        logger.error(f"Vector store initialization failed: {e}")
    except Exception as e:
        logger.error(f"Unexpected error during setup: {e}")
        logger.exception("Full traceback:")

if __name__ == "__main__":
    main()
