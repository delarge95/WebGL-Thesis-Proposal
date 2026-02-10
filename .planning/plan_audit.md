# PLAN AUDIT v2: Alexander Woodcock
> **Date:** 2025-12-19 | **Status:** UPDATED WITH USER INPUT

---

## EXECUTIVE SUMMARY (Revised)

| Metric | Assessment | Risk |
|--------|------------|------|
| **Thesis Feasibility** | Strong methodology, execution pending | 🟡 MEDIUM |
| **Portfolio Competitiveness** | Gaps remain but addressable | 🟡 MEDIUM |
| **Timeline Realism** | Tight, especially if courses continue | 🟡 MEDIUM |
| **Skill-Market Fit** | Will improve with proper documentation | 🟢 LOW |

**Revised Score: 5.8 / 10** (Previously 4.3)

---

## USER DECISIONS CONFIRMED

| Decision | User Answer |
|----------|-------------|
| Retopo CAD | ✅ YES |
| Course Freeze | ❌ NO — User believes time exists for both |
| Optimization Documentation | ✅ YES — Needs HOW-TO |
| Methodology Status | ✅ COMPLETE (DSR + SUS + NASA-TLX) |

---

## METHODOLOGY VALIDATION ✅

**Status:** COMPLETE AND ACADEMICALLY SOUND

### Framework
- Design Science Research (DSR) — Hevner et al., 2004
- 6 stages properly defined
- Agile/Scrum with 4-week sprints

### Validation Methods
- **SUS (System Usability Scale)** — Industry standard
- **NASA-TLX** — Cognitive load measurement ✅
- **Sample Size:** 8-12 users (exceeds Nielsen's 5-user threshold)
- **Protocol:** Think-Aloud + Questionnaires

### KPIs (Quantitative)
| KPI | Target | Measurement Tool |
|-----|--------|------------------|
| Polygons | <100,000 tris | Unity Stats |
| Texel Density | 10.24 ± 2 px/cm | Blender/Substance |
| Draw Calls | <50 | Unity Profiler |
| Frame Time | <33.33ms (30 FPS) | Unity Profiler |
| VRAM Textures | <64MB | Memory Profiler |
| TTI Shell | <3s | Chrome DevTools |
| TTI Full Model | <10s (4G) | Chrome DevTools |

**Assessment:** This methodology is academically rigorous. Previous concern about validation was based on incomplete data.

---

## REMAINING GAPS (Updated)

### Gap 1: Portfolio Optimization Evidence
**Status:** Still missing, but solvable.

**HOW TO DOCUMENT OPTIMIZATION:**

1. **Before Metrics (Capture NOW before any optimization)**
   - Unity Profiler screenshot: Frame time, Draw calls, SetPass calls
   - Memory Profiler: Texture memory, Mesh memory
   - WebGL build size (MB)
   - Chrome DevTools: Network waterfall, load time

2. **During Development (Document each optimization)**
   - Create simple markdown log:
     ```
     ## Optimization Log
     
     ### 2025-12-XX: Texture Atlasing
     - Before: 47 materials, 89 draw calls
     - After: 12 materials, 34 draw calls
     - Action: Combined diffuse textures into 2K atlas
     
     ### 2025-12-XX: LOD Implementation
     - Before: 95,000 tris constant
     - After: 45,000 tris at distance >5m
     ```

3. **After Metrics (Final build)**
   - Same measurement points as "Before"
   - Side-by-side comparison table

4. **Portfolio Presentation**
   - ArtStation post with:
     - Split-screen video (Profiler before/after)
     - Quantitative table of improvements
     - Brief explanation of each technique

### Gap 2: Editor Tools
**Status:** Not addressed yet.

**Recommendation:** After thesis completion, extract useful scripts and publish.

### Gap 3: Blender Pipeline Evidence
**Status:** Will be solved IF retopo/bake process is documented.

**Action:** Screenshot each step during drone retopo.

---

## TIMELINE RISK (Updated)

### User Position
User believes courses and thesis can run in parallel.

### Objective Risk Assessment
- **32 effective days** until deadline
- **Thesis work remaining:** Retopo, UVs, Unity, UI, Testing, User Tests, Documentation
- **Courses:** Snow section (4 videos) + Water section (8 videos) pending

### Mathematical Reality
If using original schedule's time blocks:
- Thesis: ~4 hours/day
- Courses: ~2 hours/day

This is feasible **IF** no unexpected blockers occur.

### ⚠️ WARNING (Objective, Not Opinion)
The plan has **zero buffer**. Any issue (hardware, scope creep, tutor feedback, user testing delays) will require sacrificing course time.

**Data point:** The user's self-identified traits include "Analysis Paralysis" and "Shiny Object Syndrome". Parallel activities increase trigger risk.

---

## HOW TO DOCUMENT OPTIMIZATION (Detailed)

### Step 1: Baseline Capture (Before ANY optimization)
**Tools needed:** Unity Profiler, Memory Profiler, Chrome DevTools

```
// Create folder: thesis_project/optimization_evidence/baseline/

Screenshot 1: Unity Profiler → CPU → Frame time breakdown
Screenshot 2: Unity Profiler → Rendering → Batches, SetPass, Tris
Screenshot 3: Memory Profiler → Snapshot → Texture breakdown
Screenshot 4: Build Report → File sizes
Screenshot 5: Chrome DevTools → Network → Load waterfall
```

### Step 2: Log Each Optimization
Create file: `OPTIMIZATION_LOG.md`

```markdown
# Optimization Log — Drone Viewer

## Baseline (DATE)
- Draw Calls: XX
- Tris: XX
- VRAM: XX MB
- Load Time: XX s

## Optimization 1: [NAME] (DATE)
- **Problem:** [What was wrong]
- **Solution:** [What you did]
- **Result:** Draw Calls XX → XX (XX% reduction)
- **Evidence:** [Screenshot filename]

## Optimization 2: [NAME]...
```

### Step 3: Final Comparison Table
Include in thesis Chapter 4 (Results):

| Metric | Before | After | Improvement | Target | Status |
|--------|--------|-------|-------------|--------|--------|
| Draw Calls | 89 | 34 | -62% | <50 | ✅ |
| Polygons | 150K | 85K | -43% | <100K | ✅ |
| VRAM | 128MB | 58MB | -55% | <64MB | ✅ |
| Load Time | 15s | 8s | -47% | <10s | ✅ |

### Step 4: Portfolio Asset
Use same data for ArtStation post:
- Video: Screen record Profiler before/after
- Table: Copy from thesis
- Text: 2-3 paragraphs explaining approach

---

## ACTION ITEMS

### Immediate
1. ✅ Start retopo (confirmed)
2. 📸 Capture baseline metrics BEFORE any optimization
3. 📝 Create `OPTIMIZATION_LOG.md` file

### During Development
4. Document each optimization with screenshots
5. Screenshot Blender pipeline steps

### Pre-Submission
6. Capture final metrics
7. Create comparison table
8. Conduct user testing (8-12 users, SUS + NASA-TLX)

### Post-Thesis (May deadline for portfolio)
9. Create ArtStation optimization post
10. Extract and publish Editor scripts
11. Complete remaining course sections

---

## REVISED SCORING

| Category | Previous | Now | Change |
|----------|----------|-----|--------|
| Timeline Realism | 4/10 | 5/10 | +1 (user committed) |
| Portfolio Readiness | 3/10 | 5/10 | +2 (path defined) |
| Thesis Technical Scope | 7/10 | 8/10 | +1 (methodology solid) |
| Market Fit | 5/10 | 6/10 | +1 (will improve) |
| Financial Safety | 2/10 | 2/10 | — |
| Psychological Risk | 5/10 | 5/10 | — |
| **OVERALL** | **4.3** | **5.8** | **+1.5** |

---

**Audit v2 completed: 2025-12-19**
