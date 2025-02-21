from flask import Flask, render_template, request, jsonify, Response
from langchain_openai import ChatOpenAI
from langchain.agents import AgentExecutor, create_openai_functions_agent
from langchain.prompts import ChatPromptTemplate, MessagesPlaceholder
from langchain.tools import Tool
from langchain.memory import ConversationBufferMemory
from langchain.schema import AIMessage, HumanMessage
from typing import Optional, Dict, Any, List
import logging
import warnings
import os
import sys
import json
import traceback
import re  # Adicionando import do módulo re
from datetime import datetime, UTC
import uuid
from dotenv import load_dotenv
from react_agent.tools import (
    list_vectorstore_files, 
    read_file_content, 
    build_csharp_project,
    validate_csharp_directory,
    initialize_vector_store,
    analyze_build_errors_and_suggest,
    show_corrected_code,
    search
)
from react_agent.state_manager import StateManager

# Configurar o logger
logger = logging.getLogger(__name__)
logging.basicConfig(level=logging.INFO)

app = Flask(__name__)
state_manager = StateManager()

# Adicionar variáveis globais
last_build_result = None
last_build_analysis = None

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
        name="analyze_build",
        description="Analisa os erros do último build e sugere correções",
        func=lambda *args: analyze_build_errors_and_suggest(
            state_manager.get_build_result().output if state_manager.get_build_result() else 'No build output available'
        ),
    ),
    Tool(
        name="show_corrected_code",
        description="Mostra o código corrigido para um arquivo específico",
        func=lambda file_name: show_corrected_code(
            file_name, 
            state_manager.get_build_analysis()
        ),
    ),
    Tool(
        name="read_file",
        description="Lê o conteúdo de um arquivo específico",
        func=read_file_content,
    ),
    Tool(
        name="list_files",
        description="Lista todos os arquivos disponíveis",
        func=list_vectorstore_files,
    ),
    Tool(
        name="search",
        description="Search for general web results",
        func=search,
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
        self.timestamp = datetime.now(UTC)  # Use timezone-aware datetime
        self.logs = []

    def add_log(self, message: str, level: str = "INFO") -> dict:
        timestamp = datetime.now(UTC)  # Use timezone-aware datetime
        log_entry = {
            "timestamp": timestamp.isoformat(),
            "session_id": self.session_id,
            "level": level,
            "message": message
        }
        self.logs.append(log_entry)
        return log_entry

def format_build_output(output: str) -> list:
    """Formata o output do build para exibição no chat."""
    formatted_output = []  # Inicializa como lista vazia
    
    if not output:
        return formatted_output
        
    for line in output.split('\r\n'):
        if 'error CS' in line:
            # Extrai apenas as informações relevantes do erro
            match = re.search(r'.*\\([^\\]+\.cs)\((\d+),\d+\): (error CS\d+: .+)\[', line)
            if match:
                file, line_num, error = match.groups()
                formatted_output.append(f"📁 {file}:{line_num} ❌ {error}")
        elif 'Warning(s)' in line or 'Error(s)' in line or 'Time Elapsed' in line:
            formatted_output.append(line.strip())
    
    return formatted_output

async def handle_build_command(file_path: str) -> dict:
    """Executa o comando de build e retorna o resultado formatado."""
    build_session = BuildSession()
    app_name = os.path.basename(file_path)
    
    build_session.add_log(f"Iniciando build para: {app_name}")
    
    result = await build_csharp_project(
        project_path="",
        app_name=app_name,
        config={},
        session_id=build_session.session_id
    )
    
    # Formata o resultado do build
    timestamp = datetime.now(UTC).isoformat()
    
    # Formata o output para melhor legibilidade
    formatted_lines = format_build_output(result['output'])
    
    formatted_result = {
        "type": "build_result",
        "content": f"""🔨 Build Result (Session: {build_session.session_id})
⏰ Started: {timestamp}
📦 Project: {app_name}
📊 Status: {'✅ Success' if result['success'] else '❌ Failed'}

📋 Build Output:
{'\n'.join(formatted_lines)}
""",
        "success": result['success']
    }
    
    return formatted_result

async def process_command(message: str) -> Optional[Dict[str, Any]]:
    """Processa comandos especiais."""
    if message.startswith("console "):
        console_message = message[8:].strip()  # Remove "console " e espaços
        return await console_test(console_message)
    
    # Processa comandos de build
    if message.startswith("build ") or "faça o build" in message.lower() or "fazer build" in message.lower():
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
        
        if not file_path:
            return {
                "type": "error",
                "content": "Nome do arquivo não especificado no comando de build"
            }
            
        build_result = await handle_build_command(file_path)
        logger.info(f"Resultado do build: {build_result}")  # Log para debug
        return build_result
    
    return None

prompt = ChatPromptTemplate.from_messages([
    ("system", """Você é um assistente AI especializado em análise de código.

Para comandos específicos:
- "analisar build" → Primeiro verifique se existe um resultado de build recente antes de usar analyze_build
- "listar arquivos" → use list_files
- "ler arquivo" → use read_file

Antes de analisar o build, verifique se há um resultado de build disponível. 
Se não houver resultado de build, informe ao usuário que ele precisa executar o build primeiro.

Responda sempre em português.
"""),
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
            # Formata a saída do build de maneira mais clara
            if isinstance(command_result, dict) and command_result.get('type') == 'build_result':
                output = {
                    'chunk': command_result['content'],
                    'type': 'build_output',
                    'success': command_result.get('success', False)
                }
                return f"data: {json.dumps(output)}\n\n"
            return f"data: {json.dumps({'chunk': command_result, 'type': 'command_output'})}\n\n"
            
        # Se a mensagem contém pedido para ver código corrigido
        if "mostrar código corrigido" in user_message.lower() or "ver código corrigido" in user_message.lower():
            if not state_manager.get_build_result():
                return f"data: {json.dumps({'chunk': 'É necessário executar o build primeiro antes de ver o código corrigido.', 'type': 'error'})}\n\n"
        
        # Processa normalmente com o agente
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
        user_message = request.json.get('message', '')
        if not user_message:
            return jsonify({"error": "Empty message"})

        response = await process_stream(user_message)
        
        # Garante que a resposta seja enviada como texto formatado
        if isinstance(response, dict) and 'content' in response:
            return jsonify({
                "response": response['content'],
                "type": response.get('type', 'text'),
                "success": response.get('success', True)
            })
        else:
            return jsonify({"response": str(response)})
            
    except Exception as e:
        logger.error(f"Error in chat endpoint: {str(e)}")
        logger.error(f"Traceback: {traceback.format_exc()}")
        return jsonify({"error": str(e)}), 500

@app.route('/')
def home():
    return render_template('index.html')

@app.route('/api/build', methods=['POST'])
async def build():
    try:
        data = request.get_json()
        file_path = data.get('file_path', '')
        
        # Normalize path
        if file_path.startswith("csharp_project"):
            normalized_path = file_path
        else:
            normalized_path = f"csharp_project/LT2000B_20250205/Sias.Loterico/{file_path}"
            
        project_path = os.path.dirname(normalized_path)
        app_name = os.path.basename(normalized_path)
        
        result = await build_csharp_project(
            project_path=project_path,
            app_name=app_name,
            config={}
        )
        
        # Usa o StateManager para atualizar o estado
        build_result = state_manager.set_build_result(result)
        
        # Analisa os erros do build e atualiza o estado
        if result['output']:
            analysis = analyze_build_errors_and_suggest(result['output'])
            state_manager.set_build_analysis(analysis, build_result.session_id)
        
        return jsonify(result)
        
    except Exception as e:
        return jsonify({"error": str(e), "traceback": traceback.format_exc()})

@app.route('/api/analyze-build', methods=['POST'])
async def analyze_build():
    build_result = state_manager.get_build_result()
    
    if not build_result:
        return jsonify({
            "error": "Nenhum resultado de build disponível. Execute o build primeiro."
        })
    
    try:
        analysis = analyze_build_errors_and_suggest(build_result.output)
        state_manager.set_build_analysis(analysis, build_result.session_id)
        return jsonify(analysis)
        
    except Exception as e:
        return jsonify({"error": str(e), "traceback": traceback.format_exc()})

# Adicione a nova ferramenta ao COMMAND_HANDLERS
COMMAND_HANDLERS = {
    "build": handle_build_command
}

def process_large_file(file_path: str, chunk_size: int = 50000):
    """
    Processa arquivos grandes em chunks.
    """
    with open(file_path, 'r', encoding='utf-8') as file:
        while True:
            chunk = file.read(chunk_size)
            if not chunk:
                break
            # Processa o chunk
            yield chunk

@app.route('/analyze', methods=['POST'])
def analyze():
    try:
        file_path = request.json.get('file_path')
        for chunk in process_large_file(file_path):
            # Processa cada chunk
            result = analyze_code_in_chunks(chunk)
            # Envia resultado parcial
            yield json.dumps({'partial_result': result})
    except Exception as e:
        return jsonify({'error': str(e)}), 500

if __name__ == '__main__':
    cli = sys.modules['flask.cli']
    cli.show_server_banner = lambda *args: None  # Suppress Flask server warnings
    app.run(host='127.0.0.1', port=8080, debug=True)
