import json

with open('cobol_source/cobol_csharp_mapping.json', 'r', encoding='utf-8') as f:
    try:
        json.load(f)
        print("JSON válido!")
    except json.JSONDecodeError as e:
        print(f"Erro no JSON: {str(e)}")