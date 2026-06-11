# Legacy nested WebGL wrapper

This folder contains the Unity WebGL payload used by `../index.html`.

Only the nested `index.html` in this folder is a legacy Unity export wrapper. It is kept for traceability and must not be used as the final demo or QA entrypoint.

Active entrypoint:

`docs/Build/index.html`

Do not delete the `.unityweb`, loader, framework, wasm or `TemplateData` assets in this folder while the active wrapper references them.
