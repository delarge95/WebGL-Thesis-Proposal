# ArtStation Post 3: Pipeline Automation & Editor Tooling

*(Este proyecto mostrará tu capacidad de automatización, muy cotizado para el rol "Troyano" de QA Automation y Tools Programmer).*

### THE HIDDEN ART
The true art of Technical Art often lies in what the end-user never sees: the pipeline. While building the Holybro X500 V2 WebGL viewer, I quickly realized that manually configuring over 257 imported geometric nodes per iteration was a recipe for burnout. 

Instead of manual labor, I built a suite of Custom Unity Editor Windows in C# to automate the pain away.

### TOOL 1: PROJECT SETUP WIZARD
A custom Editor Window that acts as the gateway for raw imports. It strips unnecessary CAD bloat, assigns canonical naming conventions internally, and applies base materials in one click, saving hours of manual setup per import cycle.

*[Asset: Capture of the ProjectSetupWizard interface]*

### TOOL 2: IMPORTED DRONE COVERAGE AUDIT
Game development is chaotic. To ensure the final WebGL build wouldn't break on missing references, I built a specialized Inspector Audit tool. Before hitting 'Play', this script scans the scene and reports exactly which structural constraints or metadata links are broken against a canonical CSV.

**QA Automation Potential:** This goes beyond a manual button press. The architecture allows this audit to be executed headless. It serves as a precursor to CI/CD pipeline validation pre-commit hooks, ensuring artists never merge a broken asset hierarchy into the main branch, ultimately saving hours of manual QA debugging per iteration.

*[Asset: Inspector view showing green checks / red errors of the audit]*

### TOOL 3: THERMAL CONTACT GRAPH BUILDER
Setting up nodal heat transfer for the Hybrid Thermal Shader by hand is error-prone. This custom editor utility automatically maps proximity and hierarchy to generate a thermal logic graph upon which the runtime simulation runs.

---
**SOFTWARE:** Unity (Editor Scripting, UI Toolkit), C#, Python.
**TAGS:** #ToolsProgrammer #TechnicalArt #CSharp #UnityEditor #Automation #Programming
