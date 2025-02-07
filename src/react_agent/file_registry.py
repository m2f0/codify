from typing import Dict, Set
from pathlib import Path

class FileRegistry:
    _instance = None
    
    def __new__(cls):
        if cls._instance is None:
            cls._instance = super(FileRegistry, cls).__new__(cls)
            cls._instance.file_locations: Dict[str, str] = {}
            cls._instance.indexed_files: Set[str] = set()
        return cls._instance
    
    def register_file(self, filename: str, full_path: str):
        """Registra a localização completa de um arquivo."""
        self.file_locations[Path(filename).name] = full_path
        self.indexed_files.add(full_path)
    
    def get_file_path(self, filename: str) -> str:
        """Retorna o caminho completo de um arquivo pelo nome."""
        return self.file_locations.get(Path(filename).name)
    
    def clear(self):
        """Limpa o registro."""
        self.file_locations.clear()
        self.indexed_files.clear()