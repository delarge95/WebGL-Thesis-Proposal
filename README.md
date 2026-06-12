---
tipo: sistema
estado: mantenido
canon: github_public
actualizado: 2026-06-11
---

# TwinSight X500

Interactive Unity WebGL prototype for technical visualization, inspection and study of the Holybro X500 V2 drone assembly.

[![Unity](https://img.shields.io/badge/Unity-6000.4.3f1-black?logo=unity)](https://unity.com/)
[![WebGL](https://img.shields.io/badge/WebGL-2.0-blue)](https://www.khronos.org/webgl/)
[![CSharp](https://img.shields.io/badge/C%23-Unity%20runtime-purple)](https://learn.microsoft.com/dotnet/csharp/)
[![Academic](https://img.shields.io/badge/Scope-Academic%20thesis-green)](#academic-scope)

## Public Demo

- Demo and public documentation: <https://delarge95.github.io/WebGL-Thesis-Proposal/>
- Canonical repository: <https://github.com/delarge95/WebGL-Thesis-Proposal>
- Final report: [`Informe_final/informe_final.pdf`](Informe_final/informe_final.pdf)
- Published report copy: [`docs/manuals/informe_final.pdf`](docs/manuals/informe_final.pdf)
- User manual: [`Informe_final/Manual_de_usuario/manual_usuario.pdf`](Informe_final/Manual_de_usuario/manual_usuario.pdf)
- Technical manual: [`Informe_final/Manual_tecnico/manual_tecnico.pdf`](Informe_final/Manual_tecnico/manual_tecnico.pdf)

## Project Summary

TwinSight X500 is the final prototype for an academic thesis in Multimedia Engineering. The project turns a complex drone assembly into a browser-based 3D study and inspection environment. Its current academic scope is a **visual product twin**: it supports spatial understanding, technical reading and heuristic analysis, but it is not an operational digital twin connected to live telemetry.

The canonical final report source is the version in [`Informe_final/`](Informe_final/). Non-canonical drafts, local study wikis, local working copies, raw review notes and generated temporary files are intentionally kept out of the public GitHub surface.

The thesis report is the authoritative description of the final visible scope. Some engineering-support modules exist in code, internal tools or historical prototypes, but they are not all exposed in the final public UI evaluated in the thesis.

## Current Visible Flow

The public build and manuals describe this verified user flow:

```text
Hero -> Explore -> part selection -> bottom sheet -> Inspect / Analyze / Studio
```

Visible and defendable in the final UI:

- Orbital navigation, zoom and pan for the full drone and small components.
- Hierarchical selection for parent parts, subparts, hotspots and fasteners.
- Bottom-sheet technical information with identification, specifications and assembly data.
- Inspect mode with hotspots, isolation and contextual fastener inspection.
- Analyze mode with exploded view, cross-section and category filters.
- Studio mode with visible visual readings such as Realistic, X-Ray, Solid Color, Thermal and environment presets.
- Thermal reading as a heuristic component visualization, not calibrated FEA or physical measurement.
- WebGL deployment through GitHub Pages and Unity WebAssembly output.

## Capability Status

| Status | Meaning | Examples |
| --- | --- | --- |
| `visible_ui` | Available in the final public flow and safe to show in the defense demo. | Selection, bottom sheet, Inspect, Analyze, Studio, explode, cut, filters, Thermal heuristic view. |
| `implementado_oculto` | Exists in code or tooling but is not part of the public UI promise. | Additional view modes, internal managers, editor utilities and support systems. |
| `legacy` | Historical or non-integrated code/documentation kept for traceability. | Old panel flows, older report variants and pre-APA drafts. |
| `futuro` | Possible continuation after the thesis delivery. | Calibrated thermal model, broader device profiling and additional inspection workflows only if they are re-integrated, exposed and validated in a later public build. |
| `requiere_evidencia` | Must not be claimed until backed by the frozen build, profiling or validation files. | FPS, load time, polygon reduction, SUS/NASA-TLX values and final import QA claims. |

## Canonical Structure

```text
WebGL_tesis/
|-- desarrollo/
|   `-- unity_project/                  # Unity 6000.4.3f1 project
|       |-- Assets/Scripts/             # Runtime, UI, editor and validation scripts
|       |-- Assets/Shaders/             # URP/WebGL shaders
|       |-- Assets/Resources/           # Runtime data and fastener modules
|       `-- ProjectSettings/
|-- docs/                               # GitHub Pages site and public manuals
|   |-- Build/                          # Published WebGL build files
|   |-- manuals/                        # Public report/manual copies
|   `-- index.html                      # Public landing page
|-- Informe_final/                      # Canonical academic report package
|   |-- chapters/                       # LaTeX chapters
|   |-- Manual_de_usuario/              # User manual source/PDF
|   |-- Manual_tecnico/                 # Technical manual source/PDF
|   |-- presentation/                   # Defense outline and demo script
|   |-- validacion/                     # User and technical validation package
|   `-- informe_final.pdf               # Final compiled report
`-- README.md
```

## Unity Setup

Requirements:

- Unity `6000.4.3f1`.
- Universal Render Pipeline.
- Modern browser with WebGL 2.0 support.

Open the Unity project from:

```text
desarrollo/unity_project
```

The official scene and visible user flow are documented in the technical manual. The build served through GitHub Pages lives under [`docs/Build/`](docs/Build/).

## Main Runtime Systems

| System | Responsibility |
| --- | --- |
| `AppStateMachine` | Coordinates app states such as Loading, Intro, Exploration, Analyze and Studio. |
| `UIManager` / `UIModeController` | Orchestrates Hero, bottom sheet and Inspect/Analyze/Studio panels. |
| `SelectionManager` | Handles part, subpart, hotspot and fastener selection. |
| `PartVisibilityManager` | Isolates, restores and filters components. |
| `ExplodedViewManager` | Controls exploded assembly reading and category visibility. |
| `CrossSectionManager` | Applies shader-based clipping planes. |
| `ViewModeManager` | Manages visual modes and materials, including hidden/internal modes. |
| `ThermalSimulationManager` / `ThermalViewController` | Provides reduced heuristic thermal visualization by component. |
| `ImportedDroneRuntimeBinder` | Repairs and normalizes imported drone hierarchy for runtime use. |

## Validation Package

The final report and annexes use these evidence families:

- Technical profiling and WebGL measurement guide in [`Informe_final/validacion/06_GUIA_MEDICIONES_TECNICAS_WEBGL.md`](Informe_final/validacion/06_GUIA_MEDICIONES_TECNICAS_WEBGL.md).
- SUS questionnaire and aggregated/anonymized validation results in [`Informe_final/validacion/`](Informe_final/validacion/).
- NASA-TLX Raw instruments for perceived workload comparison.
- Think-Aloud protocol and aggregated qualitative findings in the final report.
- Public copies of manuals and instruments in [`docs/manuals/`](docs/manuals/).

Metrics must only be reported when they match the frozen build and the validation files referenced by chapter 5 of the final report.

## Defense Material

The defense package is maintained in [`Informe_final/presentation/`](Informe_final/presentation/):

- [`PRESENTATION_OUTLINE.md`](Informe_final/presentation/PRESENTATION_OUTLINE.md)
- [`PRESENTATION_SCRIPT.md`](Informe_final/presentation/PRESENTATION_SCRIPT.md)
- [`DEFENSE_EVIDENCE_MAP.md`](Informe_final/presentation/DEFENSE_EVIDENCE_MAP.md)
- [`JURY_QA_BANK.md`](Informe_final/presentation/JURY_QA_BANK.md)
- [`DEMO_SCRIPT.md`](Informe_final/presentation/DEMO_SCRIPT.md)
- [`ASSETS_REQUIREMENTS.md`](Informe_final/presentation/ASSETS_REQUIREMENTS.md)
- [`DESIGN_SYSTEM.md`](Informe_final/presentation/DESIGN_SYSTEM.md)
- [`KIMI_PROMPT.md`](Informe_final/presentation/KIMI_PROMPT.md)

The demo must show only the verified public flow. Do not present hidden managers, legacy panels, experimental tooling or unsupported metrics as final visible features.

## Public Repository Boundary

The public repository exposes the demo, final report package, manuals, validation instruments, aggregated/anonymized validation evidence and source files needed to understand or reproduce the thesis scope. Local agent state, local study wikis, bibliographic PDFs, raw participant records, raw review/audit notes, planning notes, logs, temporary folders, trash, archived drafts and personal portfolio material are intentionally excluded from the public surface. The only `.github` content kept public is the GitHub Pages workflow required to publish the demo and documentation.

## Academic Scope

Author: Alexander Woodcock Salomon
Program: Ingenieria Multimedia
Institution: Universidad Nacional Abierta y a Distancia - UNAD
Advisor in final report: Gustavo Enrique Vejarano Matiz
Year: 2026

This repository is part of an academic thesis. All rights are reserved unless a dedicated license file is added later.
