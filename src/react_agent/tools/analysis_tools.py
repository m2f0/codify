"""Code analysis tools."""
import re
import logging
from typing import Dict
from .file_tools import read_file_content

logger = logging.getLogger(__name__)

def analyze_build_errors_and_suggest(build_output: str) -> dict:
    """Analisa os erros do build e retorna uma análise formatada."""
    error_analysis = {
        "files": {},
        "errors": []
    }
    
    for line in build_output.split('\n'):
        if '.cs(' in line and 'error' in line.lower():
            match = re.search(r'([\w\.]+\.cs)\((\d+),.*?\): error (CS\d+): (.*)', line)
            if match:
                file_name = match.group(1)
                line_number = int(match.group(2))
                error_code = match.group(3)
                error_message = match.group(4)
                
                if file_name not in error_analysis["files"]:
                    file_content = read_file_content(file_name)
                    error_analysis["files"][file_name] = {
                        "content": file_content,
                        "errors": []
                    }
                
                if file_content := error_analysis["files"][file_name]["content"]:
                    lines = file_content.split('\n')
                    original_line = lines[line_number - 1] if line_number <= len(lines) else None
                else:
                    original_line = None
                
                error_info = {
                    "line": line_number,
                    "code": error_code,
                    "message": error_message,
                    "original_line": original_line
                }
                
                error_analysis["files"][file_name]["errors"].append(error_info)
                error_analysis["errors"].append({
                    "file": file_name,
                    **error_info
                })
    
    return error_analysis

def show_corrected_code(file_name: str, error_analysis: Dict = None) -> str:
    """Mostra o código corrigido para um arquivo específico."""
    if not error_analysis:
        return f"Não há análise de erros disponível para o arquivo {file_name}"
        
    if file_name not in error_analysis["files"]:
        return f"Não foram encontrados erros para o arquivo {file_name}"
        
    file_info = error_analysis["files"][file_name]
    if not file_info["content"]:
        base_dir = os.getenv("CSHARP_PROJECT_DIR", "./csharp_project")
        return (f"Não foi possível ler o conteúdo do arquivo {file_name}. "
                f"Verifique se o arquivo existe em {base_dir} ou suas subpastas.")
        
    result = []
    lines = file_info["content"].split('\n')
    
    for error in file_info["errors"]:
        line_num = error["line"]
        if line_num <= len(lines):
            original_line = lines[line_num - 1]
            corrected_line = original_line
            
            if error["code"] == "CS0019" and "!=" in original_line:
                type_match = re.search(r"type '([^']+)' and 'string'", error["message"])
                if type_match:
                    corrected_line = re.sub(
                        r'(\w+)\s*!=\s*"([^"]*)"',
                        r'\1.ToString() != "\2"',
                        original_line
                    )
            
            result.extend([
                f"Linha {line_num}:",
                f"Original: {original_line}",
                f"Corrigido: {corrected_line}",
                f"Erro: {error['code']} - {error['message']}\n"
            ])
    
    return "\n".join(result) if result else f"Não foram encontradas correções automáticas para os erros em {file_name}"