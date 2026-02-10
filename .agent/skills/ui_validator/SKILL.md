---
name: ui_validator
description: Validates UI Toolkit assets and generates safe C# bindings.
---

# UI Validator

This skill ensures your UI is robust by validating UXML/USS and optionally generating C# constants for your VisualElement names to avoid "magic strings".

## Usage

Run this skill when making changes to UI structure.

## Capabilities

1.  **Deep Nesting Check**: Warns if USS selectors are too complex (>3 levels).
2.  **Binding Generator**: Scans `.uxml` files and outputs a C# file with `const string` for every `x:Name` found. (Beta)
3.  **Broken Ref Check**: Verifies that scripts referenced in UXML actually exist.

## How to Run

Execute `validate_ui.ps1`.
