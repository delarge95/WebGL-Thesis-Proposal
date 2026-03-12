# docs

This folder is the published documentation layer of the repository.

## Purpose

- Host the web-facing documentation used by the repository site and public consultation flows.
- Concentrate navigable manuals and assets that can be exposed through GitHub Pages or an equivalent static site workflow.
- Keep together the documentation that is meant to be consumed from outside the development workspace.

## What belongs here

- Site assets such as `index.html`, `src/`, `dist/`, `Build/`, `main.js`, and styles.
- Public manuals and operational documents under `docs/manuals/`.
- Documentation mirrors that exist here specifically because they must be browsable or publishable from the repository web layer.

## What does not belong here

- Working research notes or internal technical exploration from the development process.
- Third-party references or academic templates.
- Formal thesis-source organization and audit material.

## Relationship with other documentation folders

- `desarrollo/docs/` is the technical working source layer.
- `docs/` is the public or publishable documentation layer.
- `External_docs/` stores only external references.
- `Informe_final/` stores the formal academic deliverables.

If a document exists both here and in `desarrollo/docs/`, the version in `desarrollo/docs/` should be understood as the working technical source, while the version here exists for publication or web consultation.
