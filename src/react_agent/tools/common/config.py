"""Configuration settings."""
import os
from typing import Dict, Any

class Config:
    CSHARP_PROJECT_DIR = os.getenv("CSHARP_PROJECT_DIR", "./csharp_project")
    VECTOR_STORE_PATH = os.getenv("VECTOR_STORE_PATH", "./vector_store")
    LOG_LEVEL = os.getenv("LOG_LEVEL", "INFO")
    
    @classmethod
    def get_build_settings(cls) -> Dict[str, Any]:
        return {
            "warning_level": 0,
            "detailed_output": True,
            "suppress_warnings": True
        }