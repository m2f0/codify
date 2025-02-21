"""
Módulo de ferramentas para build de projetos C#.

Este módulo fornece funcionalidades para compilação e análise
de projetos C#, incluindo gerenciamento de erros e logs.

Typical usage example:

    result = await build_csharp_project("/path/project", "MyApp", config)
    if result["success"]:
        print(f"Build successful: {result['output']}")
"""

import asyncio
import os
import logging
import traceback
from typing import Dict, Any
from pathlib import Path
from ..utils.validators import InputValidator, ValidationError
from langchain_core.messages import AIMessage
from langchain_core.runnables import RunnableConfig
from langgraph.graph import StateGraph
from langgraph.prebuilt import ToolNode
from langsmith.run_helpers import traceable
logger = logging.getLogger(__name__)

@traceable(name="build_csharp_project")
async def build_csharp_project(
    project_path: str,
    app_name: str,
    config: Dict[str, Any] = None,
    session_id: str = None
) -> Dict[str, Any]:
    """
    Executa o build de um arquivo C# (.cs) ou projeto (.csproj)
    
    Args:
        project_path: Caminho para o diretório do projeto
        app_name: Nome do arquivo (.cs ou .csproj)
        config: Configurações adicionais (opcional)
        session_id: ID da sessão de build (opcional)
    """
    try:
        if config is None:
            config = {}
            
        base_dir = os.getenv("CSHARP_PROJECT_DIR", "./csharp_project")
        
        # Lista de possíveis caminhos onde o arquivo pode estar
        possible_paths = [app_name]  # caminho direto
        
        # Busca recursiva em todas as subpastas do diretório base
        for root, _, files in os.walk(base_dir):
            if app_name in files:
                possible_paths.append(os.path.join(root, app_name))
                
        file_path = None
        tried_paths = []
        
        # Tenta encontrar o arquivo em cada possível localização
        for path in possible_paths:
            tried_paths.append(path)
            if os.path.exists(path):
                file_path = path
                break
                
        if not file_path:
            return {
                "success": False,
                "output": f"Arquivo {app_name} não encontrado. Caminhos tentados: {', '.join(tried_paths)}",
                "session_id": session_id
            }

        logger.info(f"Arquivo encontrado: {file_path}")

        # Configura o ambiente para usar UTF-8
        env = os.environ.copy()
        env["PYTHONIOENCODING"] = "utf-8"
        env["DOTNET_CLI_UI_LANGUAGE"] = "en"
        
        # Se for arquivo .cs, usa dotnet build diretamente no arquivo
        # Se for .csproj, usa o arquivo de projeto
        cmd = ["dotnet", "build"]
        if file_path.endswith('.cs'):
            cmd.extend([file_path, "-v", "detailed"])
        else:
            cmd.extend([file_path, "-v", "detailed", "/p:WarningLevel=0"])
        
        process = await asyncio.create_subprocess_exec(
            *cmd,
            stdout=asyncio.subprocess.PIPE,
            stderr=asyncio.subprocess.PIPE,
            env=env
        )
        
        stdout, stderr = await process.communicate()
        
        output = stdout.decode('utf-8', errors='replace')
        error_output = stderr.decode('utf-8', errors='replace')
        
        combined_output = output + "\n" + error_output if error_output else output
        
        return {
            "success": process.returncode == 0,
            "output": combined_output,
            "session_id": session_id
        }
        
    except Exception as e:
        error_msg = f"Erro ao executar build: {str(e)}\n{traceback.format_exc()}"
        logger.error(error_msg)
        return {
            "success": False,
            "output": error_msg,
            "session_id": session_id
        }
