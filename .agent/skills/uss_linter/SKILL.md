---
name: uss_linter
description: Scans USS and UXML files for Unity 6 compatibility issues and syntax errors.
---

# USS/UXML Linter

This skill validates the project's UI stylesheets and layouts against Unity 6 constraints. It specifically targets issues that cause "Color while reading Function" errors.

## Usage

Run this skill before committing UI changes or when encountering UI parsing errors.

## Capabilities

1.  **Transition Syntax Check**: Detects shorthand `transition` properties using `ease` functions (incompatible with Unity 6 USS).
2.  **Inline Style Validation**: Scans UXML files for inline `style="..."` attributes using `var()` (incompatible).
3.  **Path Validation**: Checks `src` attributes in UXML for valid project paths.

## How to Run

Execute the `lint_uss.ps1` script located in the `scripts` directory of this skill.

```powershell
./scripts/lint_uss.ps1
```
