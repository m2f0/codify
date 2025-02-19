"""
Módulo de validação para inputs do sistema.

Este módulo fornece classes e funções para validação de inputs,
garantindo a integridade e segurança dos dados processados pelo sistema.

Typical usage example:

    validator = InputValidator()
    result = validator.validate_path("/path/to/file.cs")
    if result.is_valid:
        process_file(result.value)
    else:
        logger.error(result.message)
"""

from pathlib import Path
from typing import Optional, Union, Any, Dict
import os
import re
from dataclasses import dataclass
from enum import Enum

class ValidationError(Exception):
    """Exceção lançada quando ocorrem erros de validação.
    
    Attributes:
        message: Mensagem descritiva do erro
        code: Código opcional do erro
    """
    
    def __init__(self, message: str, code: Optional[int] = None):
        """Inicializa ValidationError.

        Args:
            message: Descrição detalhada do erro
            code: Código numérico opcional do erro
        """
        super().__init__(message)
        self.code = code

class FileType(Enum):
    """Enumeração dos tipos de arquivo suportados pelo sistema.
    
    Attributes:
        CSHARP: Arquivos C# (.cs)
        COBOL: Arquivos COBOL (.cbl)
        COBOL_ALT: Arquivos COBOL alternativos (.cob)
        PROJECT: Arquivos de projeto C# (.csproj)
        CONFIG: Arquivos de configuração (.config)
        JSON: Arquivos JSON (.json)
    """
    
    CSHARP = ".cs"
    COBOL = ".cbl"
    COBOL_ALT = ".cob"
    PROJECT = ".csproj"
    CONFIG = ".config"
    JSON = ".json"

@dataclass
class ValidationResult:
    """Resultado de uma operação de validação.
    
    Attributes:
        is_valid: Indica se a validação foi bem-sucedida
        message: Mensagem descritiva do resultado
        value: Valor validado e possivelmente transformado
    """
    
    is_valid: bool
    message: str
    value: Any = None

class InputValidator:
    """Classe principal para validação de inputs do sistema.
    
    Esta classe fornece métodos estáticos para validar diferentes tipos
    de inputs, incluindo caminhos de arquivo, conteúdo de código e
    parâmetros de build.
    
    Attributes:
        MAX_FILE_SIZE: Tamanho máximo permitido para arquivos em bytes
        ALLOWED_EXTENSIONS: Lista de extensões de arquivo permitidas
    """
    
    MAX_FILE_SIZE = 10_000_000  # 10MB
    ALLOWED_EXTENSIONS = {ext.value for ext in FileType}

    @staticmethod
    def validate_path(path: Union[str, Path], 
                     must_exist: bool = True,
                     file_type: Optional[FileType] = None) -> ValidationResult:
        """Valida um caminho de arquivo ou diretório.
        
        Realiza validações de segurança e integridade em caminhos de arquivo,
        incluindo verificação de existência e tipo de arquivo.
        
        Args:
            path: Caminho a ser validado (string ou objeto Path)
            must_exist: Se True, verifica se o caminho existe
            file_type: Tipo esperado do arquivo (opcional)
            
        Returns:
            ValidationResult contendo o resultado da validação
            
        Examples:
            >>> result = InputValidator.validate_path("/path/file.cs", 
                                                    file_type=FileType.CSHARP)
            >>> if result.is_valid:
            ...     process_file(result.value)
        """
        if not path:
            return ValidationResult(False, "Caminho não pode ser vazio")
            
        try:
            path_obj = Path(path)
            
            # Validação básica do caminho
            if not path_obj.is_absolute():
                path_obj = Path.cwd() / path_obj
                
            # Verifica existência
            if must_exist and not path_obj.exists():
                return ValidationResult(False, f"Caminho não existe: {path_obj}")
                
            # Verifica tipo de arquivo
            if file_type and path_obj.is_file():
                if not str(path_obj).lower().endswith(file_type.value):
                    return ValidationResult(
                        False, 
                        f"Arquivo deve ter extensão {file_type.value}"
                    )
            
            return ValidationResult(True, "Caminho válido", path_obj)
            
        except Exception as e:
            return ValidationResult(False, f"Erro ao validar caminho: {str(e)}")

    @staticmethod
    def validate_build_input(project_path: str, 
                           app_name: str) -> ValidationResult:
        """Valida inputs para build de projeto C#.
        
        Args:
            project_path: Caminho do projeto C#
            app_name: Nome do aplicativo
            
        Returns:
            ValidationResult com o resultado da validação
            
        Raises:
            ValidationError: Se os inputs forem inválidos
        """
        # Valida project_path
        path_result = InputValidator.validate_path(
            project_path, 
            must_exist=True
        )
        if not path_result.is_valid:
            return path_result
            
        # Valida app_name
        if not app_name or not isinstance(app_name, str):
            return ValidationResult(False, "Nome do aplicativo inválido")
            
        # Verifica se existe arquivo .csproj
        csproj_path = Path(project_path) / f"{app_name}.csproj"
        if not csproj_path.exists():
            return ValidationResult(
                False, 
                f"Arquivo de projeto não encontrado: {csproj_path}"
            )
            
        return ValidationResult(True, "Inputs válidos", {
            "project_path": project_path,
            "app_name": app_name
        })

    @staticmethod
    def validate_code_content(content: str, 
                            max_size: int = MAX_FILE_SIZE) -> ValidationResult:
        """Valida conteúdo de código fonte.
        
        Args:
            content: Conteúdo do código a ser validado
            max_size: Tamanho máximo permitido em bytes
            
        Returns:
            ValidationResult com o resultado da validação
        """
        if not content:
            return ValidationResult(False, "Conteúdo não pode ser vazio")
            
        if not isinstance(content, str):
            return ValidationResult(False, "Conteúdo deve ser string")
            
        if len(content) > max_size:
            return ValidationResult(
                False, 
                f"Conteúdo excede tamanho máximo de {max_size} bytes"
            )
            
        return ValidationResult(True, "Conteúdo válido", content)

    @staticmethod
    def validate_build_output(output: str) -> ValidationResult:
        """
        Valida output de build.
        """
        if not output:
            return ValidationResult(False, "Output não pode ser vazio")
            
        # Verifica se contém informações mínimas esperadas
        if not any(marker in output for marker in ['error', 'warning', 'Build succeeded', 'Build failed']):
            return ValidationResult(False, "Output de build inválido ou incompleto")
            
        return ValidationResult(True, "Output válido", output)

    @staticmethod
    def sanitize_filename(filename: str) -> str:
        """Sanitiza nome de arquivo removendo caracteres inválidos.
        
        Args:
            filename: Nome do arquivo a ser sanitizado
            
        Returns:
            String contendo o nome do arquivo sanitizado
            
        Examples:
            >>> InputValidator.sanitize_filename("my:file*.cs")
            'myfile.cs'
        """
        # Remove caracteres inválidos
        filename = re.sub(r'[<>:"/\\|?*]', '', filename)
        # Remove espaços extras
        filename = ' '.join(filename.split())
        return filename.strip()
