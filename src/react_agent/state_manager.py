"""State management for the application."""
from dataclasses import dataclass, field
from typing import Dict, Any, Optional
from datetime import datetime
import threading
from contextlib import contextmanager

@dataclass
class BuildResult:
    """Representa o resultado de um build."""
    success: bool
    output: str
    session_id: str
    timestamp: datetime = field(default_factory=datetime.now)
    error: Optional[str] = None

@dataclass
class BuildAnalysis:
    """Representa a análise de um build."""
    files: Dict[str, Any]
    errors: list
    timestamp: datetime = field(default_factory=datetime.now)
    build_session_id: Optional[str] = None

class StateManager:
    """Gerenciador centralizado de estado da aplicação."""
    _instance = None
    _lock = threading.Lock()

    def __new__(cls):
        if cls._instance is None:
            with cls._lock:
                if cls._instance is None:
                    cls._instance = super().__new__(cls)
                    cls._instance._initialize()
        return cls._instance

    def _initialize(self):
        """Inicializa o estado interno."""
        self._state_lock = threading.Lock()
        self._build_result: Optional[BuildResult] = None
        self._build_analysis: Optional[BuildAnalysis] = None
        self._subscribers = []

    @contextmanager
    def state_transaction(self):
        """Context manager para operações atômicas no estado."""
        with self._state_lock:
            yield

    def set_build_result(self, result: Dict[str, Any]) -> BuildResult:
        """
        Atualiza o resultado do build de forma thread-safe.
        """
        with self._state_lock:
            self._build_result = BuildResult(
                success=result.get('success', False),
                output=result.get('output', ''),
                session_id=result.get('session_id', ''),
                error=result.get('error')
            )
            self._notify_subscribers('build_result', self._build_result)
            return self._build_result

    def set_build_analysis(self, analysis: Dict[str, Any], build_session_id: Optional[str] = None) -> BuildAnalysis:
        """
        Atualiza a análise do build de forma thread-safe.
        """
        with self._state_lock:
            self._build_analysis = BuildAnalysis(
                files=analysis.get('files', {}),
                errors=analysis.get('errors', []),
                build_session_id=build_session_id
            )
            self._notify_subscribers('build_analysis', self._build_analysis)
            return self._build_analysis

    def get_build_result(self) -> Optional[BuildResult]:
        """Retorna o resultado do último build."""
        with self._state_lock:
            return self._build_result

    def get_build_analysis(self) -> Optional[BuildAnalysis]:
        """Retorna a análise do último build."""
        with self._state_lock:
            return self._build_analysis

    def get_latest_state(self) -> Dict[str, Any]:
        """Retorna o estado completo atual."""
        with self._state_lock:
            return {
                'build_result': self._build_result,
                'build_analysis': self._build_analysis
            }

    def subscribe(self, callback):
        """Adiciona um subscriber para mudanças de estado."""
        with self._state_lock:
            self._subscribers.append(callback)

    def unsubscribe(self, callback):
        """Remove um subscriber."""
        with self._state_lock:
            self._subscribers.remove(callback)

    def _notify_subscribers(self, event_type: str, data: Any):
        """Notifica todos os subscribers sobre mudanças no estado."""
        for subscriber in self._subscribers:
            try:
                subscriber(event_type, data)
            except Exception as e:
                print(f"Error notifying subscriber: {e}")

    def clear_state(self):
        """Limpa todo o estado."""
        with self._state_lock:
            self._build_result = None
            self._build_analysis = None
            self._notify_subscribers('state_cleared', None)