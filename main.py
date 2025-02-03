from dotenv import load_dotenv
from langchain_openai import ChatOpenAI

load_dotenv()  # Load environment variables from .env file
llm = ChatOpenAI()
llm.invoke("Hello, world!")
