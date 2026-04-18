# Migration Metrics Workflow

This folder tracks the Unity 6.0 to 6.4 migration, the Mesh LOD rollout, and the decision gate after each wave.

## Expected workflow

1. Capture `baseline_6_0.md` before opening the project in Unity 6.4.3f1.
2. Upgrade the project, resolve compatibility issues, and capture `baseline_6_4_sin_lod.md`.
3. Run the pilot and each LOD wave with `MigrationBenchmarkRunner`.
4. Before each build handoff, run `WebGL/Migration/Write Build and Config Snapshots`.
5. Record Unity Stats, browser smoke tests, and acceptance decision in the corresponding phase file.

## Tooling added in this migration

- `Assets/Scripts/Core/Utils/WebGLProfiler.cs`
- `Assets/Scripts/Core/Utils/MigrationBenchmarkRunner.cs`
- `Assets/Editor/Antigravity/Fixes/MigrationMetricsReporter.cs`
- `Assets/Scripts/Tests/Editor/MigrationSmokeTests.cs`

## Report status values

- `avanza`: metrics and functional validation allow the next wave.
- `ajustar`: regressions or unclear gains require tuning before continuing.
- `rollback parcial`: revert the current wave or move the affected family to external LOD fallback.
