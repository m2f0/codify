from flask import Flask, render_template, request, jsonify, Response
from langchain_openai import ChatOpenAI
from langchain.agents import AgentExecutor, create_openai_functions_agent
from langchain.prompts import ChatPromptTemplate, MessagesPlaceholder
from langchain.tools import Tool
from langchain.memory import ConversationBufferMemory
from langchain.schema import AIMessage, HumanMessage
from react_agent.tools import (
    list_vectorstore_files, 
    read_file_content, 
    build_csharp_project,
    validate_csharp_directory,
    initialize_vector_store
)
import logging
import traceback
from dotenv import load_dotenv
import os
import json
import asyncio
from functools import partial
import warnings
import sys
from typing import Any, Dict
import uuid
from datetime import datetime

app = Flask(__name__)

# Adicione esta variável global para armazenar o último resultado do build
last_build_result = None

# Suppress FAISS GPU warning
warnings.filterwarnings('ignore', category=UserWarning, message='Failed to load GPU Faiss')

# Configure logging more specifically
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)

# Set urllib3 logging to WARNING to reduce connection pool noise
logging.getLogger('urllib3').setLevel(logging.WARNING)

load_dotenv()

# Inicializa o vector store antes de configurar o resto da aplicação
print("Validando diretório C# e inicializando vector store...")
if validate_csharp_directory():
    directory_path = os.getenv("CSHARP_PROJECT_DIR", "./csharp_project")
    vector_store = initialize_vector_store(directory_path)
    if vector_store:
        print("Vector store inicializado com sucesso!")
    else:
        print("ERRO: Falha ao inicializar vector store")
else:
    print("ERRO: Diretório C# inválido")

llm = ChatOpenAI(model="gpt-4-turbo-preview", temperature=0)
memory = ConversationBufferMemory(
    return_messages=True,
    memory_key="chat_history"
)

tools = [
    Tool(
        name="list_files",
        description="Lista todos os arquivos indexados",
        func=list_vectorstore_files,
    ),
    Tool(
        name="read_file",
        description="Lê o conteúdo de um arquivo específico",
        func=read_file_content,
    )
]

async def console_test(message: str) -> dict:
    """Executa um comando de console e retorna o resultado."""
    output = f"\n=== Console Test ===\n{message}\n==================\n"
    return {
        "success": True,
        "output": output,
        "type": "console"
    }

class BuildSession:
    def __init__(self):
        self.session_id = str(uuid.uuid4())
        self.timestamp = datetime.utcnow()
        self.logs = []

    def add_log(self, message: str, level: str = "INFO"):
        timestamp = datetime.utcnow()
        log_entry = {
            "timestamp": timestamp.isoformat(),
            "session_id": self.session_id,
            "level": level,
            "message": message
        }
        self.logs.append(log_entry)
        return log_entry

async def handle_build_command(file_path: str) -> dict:
    """Executa o comando de build e retorna o resultado formatado."""
    build_session = BuildSession()
    
    if not file_path:
        file_path = "Sias.Loterico.csproj"
    
    build_session.add_log(f"Iniciando build para arquivo: {file_path}")
    
    if file_path.startswith("csharp_project"):
        normalized_path = file_path
    else:
        normalized_path = f"csharp_project/LT2000B/Sias.Loterico/{file_path}"
    
    project_path = os.path.dirname(normalized_path)
    app_name = os.path.basename(normalized_path)
    
    build_session.add_log(f"Caminho normalizado: {normalized_path}")
    build_session.add_log(f"Diretório do projeto: {project_path}")
    
    result = await build_csharp_project(
        project_path=project_path,
        app_name=app_name,
        config={},
        session_id=build_session.session_id
    )
    
    # Formata a saída mantendo todo o log
    output_parts = []
    output_parts.append(f"=== Build Result (Session: {build_session.session_id}) ===")
    output_parts.append(f"Started: {build_session.timestamp.isoformat()}")
    output_parts.append(f"Project: {app_name}")
    output_parts.append(f"Status: {'Success' if result['success'] else 'Failed'}")
    
    if result.get('output'):
        output_parts.append("\n=== Build Output ===")
        output_parts.append(result['output'])
    
    if result.get('error'):
        output_parts.append("\n=== Build Errors ===")
        output_parts.append(result['error'])
        build_session.add_log(result['error'], "ERROR")
    
    output_parts.append("==================")
    
    final_output = "\n".join(output_parts)
    
    return {
        "success": result['success'],
        "output": final_output,
        "type": "build",
        "session_id": build_session.session_id,
        "logs": build_session.logs
    }

async def process_command(message: str):
    """Processa comandos específicos."""
    message = message.lower().strip()
    
    if message.startswith("console "):
        console_message = message[8:].strip()  # Remove "console " e espaços
        return await console_test(console_message)
    
    # Processa comandos de build
    if message.startswith("build ") or "faça o build" in message or "fazer build" in message:
        # Extrai o nome do arquivo/caminho do comando
        file_path = ""
        if message.startswith("build "):
            file_path = message.replace("build", "", 1).strip()
        else:
            # Tenta extrair o nome do arquivo do comando em português
            words = message.split()
            for i, word in enumerate(words):
                if word in ["de", "do", "da"] and i + 1 < len(words):
                    file_path = words[i + 1]
                    break
        
        return await handle_build_command(file_path)
    
    return None

prompt = ChatPromptTemplate.from_messages([
    ("system", """Você é um assistente AI especializado em análise de código.
    
Quando receber comandos para listar ou mostrar código:
1. Use list_files para obter a lista de arquivos
2. Para CADA arquivo da lista, use read_file para obter seu conteúdo
3. Apresente o conteúdo de cada arquivo precedido por seu nome

Para outros comandos:
- "listar arquivos", "liste os arquivos", "mostrar arquivos" → use apenas list_files
- "analisar código", "buscar erros" → use read_file seguido de análise

Responda sempre em português.

IMPORTANTE: Quando o usuário pedir para listar ou mostrar código, você DEVE usar read_file para cada arquivo listado."""),
    MessagesPlaceholder(variable_name="chat_history"),
    ("human", "{input}"),
    MessagesPlaceholder(variable_name="agent_scratchpad"),
])

agent = create_openai_functions_agent(llm, tools, prompt)
agent_executor = AgentExecutor(
    agent=agent,
    tools=tools,
    memory=memory,
    verbose=True
)

async def process_stream(user_message):
    try:
        # Primeiro, verifica se é um comando especial
        command_result = await process_command(user_message)
        if command_result:
            return f"data: {json.dumps(command_result)}\n\n"
            
        # Se não for um comando especial, processa normalmente com o agente
        response = await agent_executor.ainvoke(
            {"input": user_message},
            {"configurable": {
                "model": "gpt-4-turbo-preview",
                "temperature": 0
            }}
        )
        
        if response.get('output'):
            return f"data: {json.dumps({'chunk': response.get('output'), 'type': 'content'})}\n\n"
        return f"data: {json.dumps({'chunk': 'Sem resposta do agente', 'type': 'error'})}\n\n"
    
    except Exception as e:
        logger.error(f"Error in process_stream: {str(e)}")
        logger.error(f"Traceback: {traceback.format_exc()}")
        return f"data: {json.dumps({'chunk': f'Erro: {str(e)}', 'type': 'error'})}\n\n"

@app.route('/chat', methods=['POST'])
async def chat():
    try:
        user_message = request.json.get('message')
        if not user_message:
            return jsonify({'error': 'Mensagem não fornecida'}), 400

        response = await process_stream(user_message)
        return Response(
            [response],
            mimetype='text/event-stream',
            headers={
                'Cache-Control': 'no-cache',
                'X-Accel-Buffering': 'no'
            }
        )
        
    except Exception as e:
        logger.error(f"Error in chat endpoint: {str(e)}")
        logger.error(f"Traceback: {traceback.format_exc()}")
        return jsonify({'error': str(e)}), 500

@app.route('/')
def home():
    return render_template('index.html')

@app.route('/api/build', methods=['POST'])
async def build():
    global last_build_result
    try:
        data = request.get_json()
        file_path = data.get('file_path', '')
        
        # Normalize path
        if file_path.startswith("csharp_project"):
            normalized_path = file_path
        else:
            normalized_path = f"csharp_project/LT2000B/Sias.Loterico/{file_path}"
            
        project_path = os.path.dirname(normalized_path)
        app_name = os.path.basename(normalized_path)
        
        result = await build_csharp_project(
            project_path=project_path,
            app_name=app_name,
            config={}
        )
        
        # Armazena o resultado do build
        last_build_result = result
        
        return jsonify(result)
        
    except Exception as e:
        return jsonify({"error": str(e), "traceback": traceback.format_exc()})

@app.route('/api/analyze-build', methods=['POST'])
async def analyze_build():
    global last_build_result
    
    if not last_build_result:
        return jsonify({
            "error": "Nenhum resultado de build disponível. Execute o build primeiro."
        })
    
    try:
        # Formata os erros e warnings para análise
        build_analysis = {
            "status": "Success" if last_build_result['success'] else "Failed",
            "errors": [],
            "warnings": []
        }
        
        if last_build_result.get('output'):
            for line in last_build_result['output'].splitlines():
                if ": error " in line:
                    build_analysis["errors"].append(line)
                elif ": warning " in line:
                    build_analysis["warnings"].append(line)
        
        # Se houver erros no stderr, adicione-os também
        if last_build_result.get('error'):
            build_analysis["errors"].extend(
                last_build_result['error'].splitlines()
            )
            
        return jsonify(build_analysis)
        
    except Exception as e:
        return jsonify({"error": str(e), "traceback": traceback.format_exc()})

# Adicione a nova ferramenta ao COMMAND_HANDLERS
COMMAND_HANDLERS = {
    "build": handle_build_command
}

if __name__ == '__main__':
    cli = sys.modules['flask.cli']
    cli.show_server_banner = lambda *args: None  # Suppress Flask server warnings
    app.run(host='127.0.0.1', port=8080, debug=True)
