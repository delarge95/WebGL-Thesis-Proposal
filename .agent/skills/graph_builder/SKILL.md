---
name: graph_builder
description: Builds a structural Knowledge Graph (nodes/edges) of the Unity project to prevent hallucinations.
---

# Graph Builder (GraphRAG MVP)

This skill scans the project to build a lightweight Knowledge Graph JSON. It maps:
- **Scripts** (Classes, Methods)
- **UI Assets** (UXML, USS)
- **Relationships** (Inheritance, Usage, UI Bindings)

## Usage

Run `build_graph.py` to generate `project_graph.json`. The agent can then grep this file to answer structural questions with 100% precision.

## Capabilities

1.  **C# Parser**: Extracts class names, inheritance, and method signatures.
2.  **UXML Parser**: Extracts `x:Name` and `binding-path` to link UI to Code.
3.  **Graph Export**: Outputs a NetworkX-compatible JSON node-link format.

## How to Run

```bash
python scripts/build_graph.py --project-path "e:/WebGL_tesis/desarrollo/unity_project"
```
