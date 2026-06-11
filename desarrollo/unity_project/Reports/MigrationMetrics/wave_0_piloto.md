# wave_0_piloto

Status: pending
Compared against: `baseline_6_4_sin_lod.md`
Conclusion: pending

## Scope

- Families included: motors, arms, landing gear
- Fallback rule: move any failing family to external `_LOD0/_LOD1/_LOD2`

## Build artifacts

| Metric | Previous | Current | Delta | Gate |
| --- | --- | --- | --- | --- |
| `.data.br` | pending | pending | pending | `<= 3%` |
| `.wasm.br` | pending | pending | pending | monitor |
| `.framework.js.br` | pending | pending | pending | monitor |
| Total downloadable size | pending | pending | pending | `<= 3%` |

## Runtime benchmark summary

| Preset | Scenario | Avg FPS | Worst frame ms | Avg heap MB | Delta vs previous | Gate |
| --- | --- | --- | --- | --- | --- | --- |
| Preset A | Idle | pending | pending | pending | pending | `>= -5%` |
| Preset A | Interactive | pending | pending | pending | pending | `>= -5%` |
| Preset B | Interactive | pending | pending | pending | pending | `+10% FPS or +10% frame time or -15% tris` |
| Preset C | Interactive | pending | pending | pending | pending | `+10% FPS or +10% frame time or -15% tris` |

## Unity Stats snapshot

| Preset | Tris | Verts | Batches | SetPass | Delta vs previous | Notes |
| --- | --- | --- | --- | --- | --- | --- |
| Preset A | pending | pending | pending | pending | pending | |
| Preset B | pending | pending | pending | pending | pending | |
| Preset C | pending | pending | pending | pending | pending | |

## Visual and functional validation

- [ ] No severe LOD popping
- [ ] No missing meshes
- [ ] Selection still targets correct parts
- [ ] Hotspots remain usable
- [ ] Explode still behaves correctly
- [ ] Cut still behaves correctly
- [ ] Thermal view and clipping remain correct

## Decision

Decision: pending
Next action: `avanza`, `ajustar`, or `rollback parcial`
