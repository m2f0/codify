from flask import Flask, render_template, request, jsonify
from langchain_openai import ChatOpenAI
from langchain.agents import AgentExecutor, create_openai_functions_agent
from langchain.prompts import ChatPromptTemplate, MessagesPlaceholder
from langchain.tools import Tool
from langchain.memory import ConversationBufferMemory
from langchain.schema import AIMessage, HumanMessage
from react_agent.tools import list_vectorstore_files, read_file_content
import logging
import traceback

# Configuração do logging
logging.basicConfig(level=logging.DEBUG)
logger = logging.getLogger(__name__)

app = Flask(__name__)

# Inicialização do modelo e memória
llm = ChatOpenAI(model="gpt-4-turbo-preview", temperature=0)
memory = ConversationBufferMemory(
    memory_key="chat_history",
    return_messages=True
)

# Definição das ferramentas
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

# Template do prompt
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

# Criação do agente
agent = create_openai_functions_agent(llm, tools, prompt)
agent_executor = AgentExecutor(
    agent=agent,
    tools=tools,
    memory=memory,
    verbose=True
)

@app.route('/')
def home():
    return render_template('index.html')

@app.route('/chat', methods=['POST'])
async def chat():
    try:
        user_message = request.json.get('message')
        if not user_message:
            return jsonify({'error': 'Mensagem não fornecida'}), 400

        # Execute o agente e capture a resposta
        response = await agent_executor.ainvoke(
            {"input": user_message},
            {"configurable": {
                "model": "gpt-4-turbo-preview",  # Explicitly set the model
                "temperature": 0
            }}
        )
        
        # Log da resposta para debug
        logger.debug(f"Agent response: {response}")
        
        # Extraia a resposta final
        final_response = response.get('output', '')
        
        # Se a resposta for uma lista, converta para string formatada
        if isinstance(final_response, list):
            final_response = "\n".join(str(item) for item in final_response)
            
        return jsonify({'response': final_response})
        
    except Exception as e:
        logger.error(f"Error in chat endpoint: {str(e)}")
        logger.error(f"Traceback: {traceback.format_exc()}")
        return jsonify({'error': str(e)}), 500

if __name__ == '__main__':
    app.run(debug=True)
