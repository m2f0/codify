"""Build-related tools."""
import asyncio
import os
import logging
import traceback
from typing import Dict, Any

logger = logging.getLogger(__name__)

async def build_csharp_project(
    project_path: str,
    app_name: str,
    config: Dict[str, Any],
    session_id: str = None
) -> Dict[str, Any]:
    """
    Executa o build de um projeto C#.
    """
    try:
        full_path = os.path.join(project_path, app_name)
        if not os.path.exists(full_path):
            return {
                "success": False,
                "output": f"Arquivo de projeto não encontrado: {full_path}",
                "session_id": session_id
            }
        
        env = os.environ.copy()
        env["PYTHONIOENCODING"] = "utf-8"
        env["DOTNET_CLI_UI_LANGUAGE"] = "en"
        
        process = await asyncio.create_subprocess_exec(
            "dotnet", "build", full_path,
            "-v", "detailed",
            "/p:WarningLevel=0",
            stdout=asyncio.subprocess.PIPE,
            stderr=asyncio.subprocess.PIPE,
            env=env
        )
        
        stdout, stderr = await process.communicate()
        
        output = stdout.decode('utf-8', errors='replace')
        error_output = stderr.decode('utf-8', errors='replace')
        
        output = output.replace('\r\n', '\n').replace('\r', '\n')
        error_output = error_output.replace('\r\n', '\n').replace('\r', '\n')
        
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