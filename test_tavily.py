from dotenv import load_dotenv
import os

def test_tavily_key():
    load_dotenv()
    key = os.getenv("TAVILY_API_KEY")
    print(f"Tavily API Key está {'definida' if key else 'NÃO definida'}")
    if key:
        print(f"Primeiros 5 caracteres da chave: {key[:5]}...")

if __name__ == "__main__":
    test_tavily_key()