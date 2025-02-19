from pathlib import Path
from typing import Dict, List
from concurrent.futures import ThreadPoolExecutor
from ..utils.encoding_handler import EncodingHandler
import logging

logger = logging.getLogger(__name__)

class BatchEncodingAnalyzer:
    """Analisador de encoding em lote para projetos."""
    
    def __init__(self, base_dir: str):
        self.base_dir = Path(base_dir)
        self.encoding_handler = EncodingHandler()

    def analyze_project_files(self) -> Dict[str, Dict]:
        """
        Analisa todos os arquivos do projeto e retorna informações de encoding.
        """
        results = {}
        
        def analyze_file(file_path: Path) -> Dict:
            try:
                encoding, confidence = self.encoding_handler.detect_encoding(str(file_path))
                return {
                    'path': str(file_path),
                    'encoding': encoding,
                    'confidence': confidence,
                    'status': 'success'
                }
            except Exception as e:
                return {
                    'path': str(file_path),
                    'error': str(e),
                    'status': 'error'
                }

        # Encontra todos os arquivos relevantes
        files_to_analyze = []
        for ext in ['.cs', '.cbl', '.cob', '.cobol']:
            files_to_analyze.extend(self.base_dir.rglob(f'*{ext}'))

        # Processa arquivos em paralelo
        with ThreadPoolExecutor() as executor:
            for result in executor.map(analyze_file, files_to_analyze):
                results[result['path']] = result

        return results

    def generate_encoding_report(self) -> str:
        """
        Gera um relatório detalhado sobre encodings no projeto.
        """
        analysis = self.analyze_project_files()
        
        report = ["=== Relatório de Encoding do Projeto ===\n"]
        
        # Estatísticas gerais
        total_files = len(analysis)
        successful = sum(1 for r in analysis.values() if r['status'] == 'success')
        failed = total_files - successful
        
        report.append(f"Total de arquivos analisados: {total_files}")
        report.append(f"Análises bem-sucedidas: {successful}")
        report.append(f"Falhas: {failed}\n")
        
        # Detalhes por encoding
        encoding_stats = {}
        for result in analysis.values():
            if result['status'] == 'success':
                enc = result['encoding']
                encoding_stats[enc] = encoding_stats.get(enc, 0) + 1
        
        report.append("Distribuição de encodings:")
        for enc, count in encoding_stats.items():
            report.append(f"- {enc}: {count} arquivos")
        
        # Lista de problemas
        if failed > 0:
            report.append("\nArquivos com problemas:")
            for path, result in analysis.items():
                if result['status'] == 'error':
                    report.append(f"- {path}: {result['error']}")
        
        return "\n".join(report)