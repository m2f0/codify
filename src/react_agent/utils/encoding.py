"""Utilities for handling file encodings."""
import chardet
import codecs
from typing import Optional
import logging

logger = logging.getLogger(__name__)

class EncodingHandler:
    """Handles file encoding detection and reading."""
    
    @staticmethod
    def detect_encoding(file_path: str) -> str:
        """
        Detect the encoding of a file.
        
        Args:
            file_path: Path to the file
            
        Returns:
            str: Detected encoding
        """
        try:
            with open(file_path, 'rb') as file:
                raw_data = file.read()
                result = chardet.detect(raw_data)
                return result['encoding'] or 'utf-8'
        except Exception as e:
            logger.error(f"Error detecting encoding for {file_path}: {str(e)}")
            return 'utf-8'

    @staticmethod
    def read_file(file_path: str) -> Optional[str]:
        """
        Read a file with proper encoding detection.
        
        Args:
            file_path: Path to the file
            
        Returns:
            Optional[str]: File content or None if reading fails
        """
        try:
            encoding = EncodingHandler.detect_encoding(file_path)
            with codecs.open(file_path, 'r', encoding=encoding, errors='replace') as file:
                return file.read()
        except Exception as e:
            logger.error(f"Error reading file {file_path}: {str(e)}")
            return None