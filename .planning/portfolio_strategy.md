# PORTFOLIO STRATEGY: JUNIOR TECHNICAL ARTIST (2026)

## CORE INSIGHTS FROM RESEARCH (2025)
1. **Profiler Screenshots:** NOT mandatory for Junior roles. Recruiters want to see **concept understanding** (Before/After) rather than raw metric dumps (Senior skill). focus on visual proof of optimization.
2. **Tools Standard:** Mastery of **Blender Native + UVPackmaster/ZenUV** is sufficient. RizomUV is a niche specialist tool not expected for Juniors.
3. **Specialization:** Don't try to be everything. Pick a lane (Tools vs. Shaders vs. Optimization). Your "Killer Combo" mixes these but emphasizes your engineering background.

---

## THE "KILLER COMBO" (TARGET STRATEGY)

### 1. The "Optimization Doctor" (Thesis Drone)
- **Role:** Shows you understand *Hardware Constraints*.
- **Content:** The WebGL Drone Thesis.
- **The Tech Flex:** "Reduced draw calls by 60% and VRAM by 50% for WebGL/Mobile target."
- **Evidence:** Split-screen video (15 FPS vs 60 FPS) + Simple "Before/After" table. **No complex profiler dumps needed.**

### 2. The "Toolsmith" (Thesis Extraction)
- **Role:** Shows you *Save Time* for artists.
- **Content:** Unity Editor Window (e.g., "Automated Part Setup" or "LOD Manager").
- **The Tech Flex:** "Automated manual setup of 50 drone parts, saving ~4 hours of manual work."
- **Evidence:** GIF of tool usage + GitHub repo link.

### 3. The "Math Wizard" (Shader Studies)
- **Role:** Shows you understand *rendering math*.
- **Content:** 3 Spheres (Dissolve, Hologram, Custom Lighting).
- **The Tech Flex:** "Procedural effects using noise math, vertex displacement, and stencil buffers without heavy textures."
- **Evidence:** Interactive GIFs + Screenshot of the Graph/Code logic.

### 4. The "Art-to-Tech Bridge" (NEW: The Human Portrait)
- **Role:** Shows you *Respect Art* but prioritize *Performance*.
- **Content:** The CG Cookie Human Portrait → **Optimized for Mobile**.
- **The Strategy:** DO NOT just upload the high-res render. That makes you a Character Artist.
- **The Tech Flex:** "High Fidelity to Mobile Pipeline."
  - **High Poly:** The original sculpt (Reference).
  - **Low Poly:** Aggressive retopology (<20k tris).
  - **Tech:** Hair Cards vs Particle Hair (Performance comparison).
  - **Shader:** Fake SSS (Subsurface Scattering) using lightweight math/textures instead of expensive rendering.
- **Evidence:** Sketchfab 3D Viewer + Wireframe overlay.

---

## DEEP DIVE: "HUMAN" PORTRAIT STRATEGY

**Objective:** Prove you can take "Cinematic Quality" and make it run on an iPhone 11.

**Execution Steps (Tech Art Focus):**
1.  **Baking:** Show the Normal Map bake quality from Blender (using ZenUV/UVPackmaster).
2.  **Hair Cards:** Don't use particle hair. Create optimized polygon hair cards. Show the "Overdraw" view in Unity Profiler to discuss fill-rate optimization.
3.  **LOD Generation:** Create 3 Levels of Detail (LOD0, LOD1, LOD2). Show a video switching between them.
4.  **Shader:** Create a custom URP shader for the skin that looks good *without* post-processing.

**Title for Portfolio:** *"Mobile Hero Character Pipeline: Optimization Study"*
**Subtitle:** *"Translating High-Fidelity Sculpt to Performant Mobile Asset"*

---

## FUTURE PROJECTS (After Hired)

Once you secure the Junior role, broaden scope:
1.  **Procedural Content Generation (PCG):** Houdini or Unity script that generates geometry (e.g., bridges, pipes) automatically.
2.  **Compute Shaders:** Boids simulation or GPU physics (Massive particle counts).
3.  **Render Pipeline Feature:** Custom URP Render Feature (e.g., replacement outline renderer).

---
