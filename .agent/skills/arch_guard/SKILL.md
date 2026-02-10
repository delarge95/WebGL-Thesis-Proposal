---
name: arch_guard
description: Roslyn Analyzer to enforce architecture rules (God Classes, Coupling) in Rider and Unity.
---

# Roslyn Architect Guard

This skill installs a custom Roslyn Analyzer into the project to prevent architectural decay.

## Usage

1.  Capabilities are active automatically in Rider/VS once installed.
2.  Run `install_analyzer.ps1` to link the analyzer to the Unity project.

## Rules

1.  **AG001 (God Class)**: Detects classes with > 50 efferent couplings.
2.  **AG002 (Unity Logic Leak)**: Detects excessive logic in `MonoBehaviour` meant for UI.

## How to Install

Execute `install_analyzer.ps1`.
