import chardet
import codecs
import logging
from typing import Optional, Tuple
from pathlib import Path

logger = logging.getLogger(__name__)

class EncodingHandler:
    """Gerenciador de encoding para leitura de arquivos."""
    
    # Encodings comuns em sistemas legados
    COMMON_ENCODINGS = [
        'utf-8', 'iso-8859-1', 'cp1252', 'cp437', 
        'ibm037', 'ibm500', 'cp875'  # Encodings comuns em COBOL
    ]
    
    @staticmethod
    def detect_encoding(file_path: str) -> Tuple[str, float]:
        """
        Detecta o encoding de um arquivo.
        Retorna: (encoding, confidence)
        """
        try:
            with open(file_path, 'rb') as file:
                raw_data = file.read()
                result = chardet.detect(raw_data)
                return result['encoding'] or 'utf-8', result['confidence']
        except Exception as e:
            logger.error(f"Erro ao detectar encoding de {file_path}: {e}")
            return 'utf-8', 0.0

    @classmethod
    def read_file(cls, file_path: str) -> Tuple[str, str]:
        """
        Lê um arquivo tentando diferentes encodings.
        Retorna: (conteúdo, encoding_usado)
        """
        file_path = Path(file_path)
        
        # Primeiro tenta detectar automaticamente
        detected_encoding, confidence = cls.detect_encoding(str(file_path))
        
        # Se a confiança for alta, tenta primeiro o encoding detectado
        if confidence > 0.8:
            try:
                with codecs.open(str(file_path), 'r', encoding=detected_encoding) as f:
                    return f.read(), detected_encoding
            except UnicodeDecodeError:
                pass

        # Tenta cada encoding conhecido
        for encoding in cls.COMMON_ENCODINGS:
            try:
                with codecs.open(str(file_path), 'r', encoding=encoding) as f:
                    content = f.read()
                    logger.info(f"Arquivo {file_path} lido com sucesso usando {encoding}")
                    return content, encoding
            except UnicodeDecodeError:
                continue
            except Exception as e:
                logger.error(f"Erro ao ler {file_path} com {encoding}: {e}")
                continue

        raise ValueError(f"Não foi possível ler o arquivo {file_path} com nenhum encoding conhecido")

    @staticmethod
    def write_file(file_path: str, content: str, encoding: str = 'utf-8') -> None:
        """
        Escreve conteúdo em um arquivo com encoding específico.
        """
        try:
            with codecs.open(file_path, 'w', encoding=encoding) as f:
                f.write(content)
            logger.info(f"Arquivo {file_path} escrito com sucesso usando {encoding}")
        except Exception as e:
            logger.error(f"Erro ao escrever {file_path} com {encoding}: {e}")
            raise

    @staticmethod
    def convert_encoding(content: str, from_encoding: str, to_encoding: str) -> str:
        """
        Converte conteúdo de um encoding para outro.
        """
        try:
            # Primeiro decodifica para bytes usando o encoding original
            byte_content = content.encode(from_encoding)
            # Depois decodifica para string usando o novo encoding
            return byte_content.decode(to_encoding)
        except Exception as e:
            logger.error(f"Erro ao converter de {from_encoding} para {to_encoding}: {e}")
            raise