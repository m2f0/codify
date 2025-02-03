from dotenv import load_dotenv
import os
import subprocess

def build_csharp_project():
    try:
        project_path = os.path.join("csharp_project", "LT2000B", "Sias.Loterico", "Sias.Loterico.csproj")
        if not os.path.exists(project_path):
            print(f"Project file not found: {project_path}")
            return False
            
        print(f"Building project: {project_path}")
        # Adicionando -v detailed para obter logs mais detalhados
        result = subprocess.run(["dotnet", "build", project_path, "-v", "detailed"], 
                              capture_output=True, 
                              text=True,
                              encoding='utf-8')
        
        # Imprime a saída completa do build
        print("\n=== Build Output ===")
        if result.stdout:
            print(result.stdout)
        
        if result.returncode != 0:
            print("\n=== Build Errors ===")
            print(result.stderr)
            return False
            
        return True
        
    except Exception as e:
        print(f"Error during build: {str(e)}")
        return False

def main():
    load_dotenv()
    success = build_csharp_project()
    print(f"\nBuild {'succeeded' if success else 'failed'}")

if __name__ == "__main__":
    main()
