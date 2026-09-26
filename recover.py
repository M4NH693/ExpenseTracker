import json
import os

transcript_path = r'C:\Users\vmanh\.gemini\antigravity\brain\84bbbaa5-f79e-430e-9736-bfec614fae14\.system_generated\logs\transcript_full.jsonl'

files_to_recover = {
    'TrangChuView.cs': '',
    'CategoriesView.cs': '',
    'LichView.cs': '',
    'NhapVaoView.cs': '',
    'ThongKeView.cs': ''
}

with open(transcript_path, 'r', encoding='utf-8') as f:
    for line in f:
        try:
            data = json.loads(line)
            # Check tool calls
            if 'tool_calls' in data:
                for tc in data['tool_calls']:
                    if tc['function']['name'] == 'default_api:write_to_file':
                        args = json.loads(tc['function']['arguments'])
                        path = args.get('TargetFile', '')
                        filename = os.path.basename(path)
                        if filename in files_to_recover:
                            files_to_recover[filename] = args.get('CodeContent', '')
        except Exception as e:
            pass

for fname, content in files_to_recover.items():
    if content:
        with open(f'd:/APPLICATIONS/HTML/quanlycitieu/Views/{fname}', 'w', encoding='utf-8') as out_f:
            # We want to prepend using QuanLyChiTieu;
            out_f.write("using QuanLyChiTieu;\n" + content)
            print(f"Recovered {fname}")
