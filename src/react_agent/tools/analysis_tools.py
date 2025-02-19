"""
Módulo de ferramentas para análise de código e erros.

Este módulo fornece funcionalidades para análise de erros de compilação
e sugestão de correções, comparando código C# com COBOL original.

Typical usage example:

    analysis = analyze_build_errors_and_suggest(build_output)
    for file, errors in analysis["files"].items():
        print(f"Errors in {file}: {errors}")
"""

from ..utils.validators import InputValidator, ValidationError
from typing import Dict, List, Optional, Any
import logging
import re

logger = logging.getLogger(__name__)

def analyze_build_errors_and_suggest(build_output: str) -> Dict[str, Any]:
    """Analisa erros do build e sugere correções.
    
    Processa o output do build para identificar erros de compilação
    e sugere correções baseadas no código COBOL original.
    
    Args:
        build_output: String contendo o output completo do build
    
    Returns:
        Dict contendo:
            - files: Dict[str, List[Dict]] com erros por arquivo
            - errors: List[str] com erros gerais
    
    Examples:
        >>> output = get_build_output()
        >>> analysis = analyze_build_errors_and_suggest(output)
        >>> for file, errors in analysis["files"].items():
        ...     print(f"File {file} has {len(errors)} errors")
    
    Note:
        O formato dos erros segue o padrão do compilador C#:
        arquivo(linha,coluna): error CSxxxx: mensagem
    """
    # Valida input
    validation_result = InputValidator.validate_build_output(build_output)
    if not validation_result.is_valid:
        logger.error(f"Output inválido: {validation_result.message}")
        return {
            "files": {},
            "errors": [validation_result.message]
        }

    try:
        error_analysis = {
            "files": {},
            "errors": []
        }
        
        # Processa cada linha do output
        for line in build_output.split('\n'):
            if '.cs(' in line and 'error' in line.lower():
                match = re.search(r'([\w\.]+\.cs)\((\d+),.*?\): error (CS\d+): (.*)', line)
                if match:
                    file_name = InputValidator.sanitize_filename(match.group(1))
                    
                    # Valida arquivo
                    file_validation = InputValidator.validate_path(
                        file_name,
                        must_exist=False,
                        file_type=FileType.CSHARP
                    )
                    
                    if not file_validation.is_valid:
                        logger.warning(f"Arquivo suspeito: {file_validation.message}")
                        continue
                        
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
        
    except Exception as e:
        logger.error(f"Erro na análise: {str(e)}")
        return {
            "files": {},
            "errors": [f"Erro na análise: {str(e)}"]
        }

def show_corrected_code(
    file_name: str,
    build_analysis: Dict[str, Any]
) -> Dict[str, Any]:
    """Mostra código corrigido para um arquivo específico.
    
    Compara o código C# com o COBOL original e sugere correções
    baseadas nos erros de build encontrados.
    
    Args:
        file_name: Nome do arquivo a ser analisado
        build_analysis: Resultado da análise prévia do build
    
    Returns:
        Dict contendo:
            - original: Código original
            - suggested: Código com sugestões de correção
            - changes: Lista de mudanças sugeridas
    
    Raises:
        FileNotFoundError: Se o arquivo não for encontrado
        ValidationError: Se os inputs forem inválidos
    """
    if not build_analysis:
        return {
            "error": f"Não há análise de erros disponível para o arquivo {file_name}"
        }
    
    if file_name not in build_analysis["files"]:
        return {
            "error": f"Não foram encontrados erros para o arquivo {file_name}"
        }
    
    file_info = build_analysis["files"][file_name]
    if not file_info["content"]:
        return {
            "error": f"Não foi possível ler o conteúdo do arquivo {file_name}"
        }
    
    result = []
    lines = file_info["content"].split('\n')
    
    for error in file_info["errors"]:
        line_num = error["line"]
        if line_num <= len(lines):
            original_line = lines[line_num - 1]
            corrected_line = original_line
            
            # Adicione mais lógicas de correção aqui
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
    
    return {
        "original": file_info["content"],
        "suggested": "\n".join(result) if result else f"Não foram encontradas correções automáticas para os erros em {file_name}",
        "changes": result
    }
