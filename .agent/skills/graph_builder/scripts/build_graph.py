import os
import re
import json
import argparse
from pathlib import Path

def parse_csharp(file_path):
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Simple regex parsing (robust enough for MVP)
    class_match = re.search(r'class\s+(\w+)\s*(?::\s*([^{]+))?', content)
    methods = re.findall(r'public\s+[\w<>]+\s+(\w+)\s*\(', content)
    
    if class_match:
        class_name = class_match.group(1)
        parent_class = class_match.group(2).strip() if class_match.group(2) else None
        return {
            "type": "script",
            "name": class_name,
            "parent": parent_class,
            "methods": methods,
            "path": str(file_path)
        }
    return None

def parse_uxml(file_path):
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Extract element names and potential script references
    names = re.findall(r'name="([^"]+)"', content)
    classes = re.findall(r'class="([^"]+)"', content)
    
    return {
        "type": "uxml",
        "name": Path(file_path).name,
        "elements": names,
        "referenced_classes": classes,
        "path": str(file_path)
    }

def build_graph(project_path):
    assets_path = Path(project_path) / "Assets"
    nodes = []
    edges = []
    
    print(f"Scanning {assets_path}...")
    
    # 1. Scan Scripts
    script_map = {} # name -> node_id
    for root, _, files in os.walk(assets_path):
        for file in files:
            if file.endswith(".cs"):
                data = parse_csharp(os.path.join(root, file))
                if data:
                    node_id = data["name"]
                    nodes.append({"id": node_id, "data": data})
                    script_map[node_id] = node_id

    # 2. Scan UI
    for root, _, files in os.walk(assets_path):
        for file in files:
            if file.endswith(".uxml"):
                data = parse_uxml(os.path.join(root, file))
                nodes.append({"id": data["name"], "data": data})
                
                # Check for implicit links (class names in UXML matching script names)
                for ref_class in data["referenced_classes"]:
                    # Clean Up class names that might be multiple
                    for single_class in ref_class.split(' '):
                        if single_class in script_map:
                            edges.append({
                                "source": data["name"],
                                "target": single_class,
                                "relation": "references_script"
                            })

    output = {
        "nodes": nodes,
        "edges": edges,
        "metadata": {
            "node_count": len(nodes),
            "edge_count": len(edges)
        }
    }
    
    return output

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("--project-path", required=True)
    args = parser.parse_args()
    
    graph = build_graph(args.project_path)
    
    out_path = Path(args.project_path).parent / "logs" / "project_graph.json"
    out_path.parent.mkdir(parents=True, exist_ok=True)
    
    with open(out_path, 'w') as f:
        json.dump(graph, f, indent=2)
        
    print(f"Graph built: {len(graph['nodes'])} nodes, {len(graph['edges'])} edges")
    print(f"Saved to: {out_path}")
