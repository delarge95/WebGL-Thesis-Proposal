---
name: wolfram-thermal-verifier
description: Verify thermal, physics, and engineering calculations for the drone thermal simulation with WolframAlpha before numbers, equations, unit conversions, or approximations are committed to code or documentation. Use when deriving or checking heat-transfer equations, conduction estimates, convection terms, thermal time constants, load mappings, Celsius/Kelvin conversions, or any supporting engineering math.
---

# Wolfram Thermal Verifier

## Quick Start

1. Normalize the equation or engineering question in plain language with explicit units.
2. Run `scripts/wolfram_verify.py` to build the WolframAlpha query URL and optional API request.
3. Compare the Wolfram result with the planned equation, units, and assumptions.
4. Record the verified form in the thermal docs or in the code change that depends on it.

## Workflow

### 1. Frame the query

Always include:

- the physical law or unknown,
- the known constants and units,
- and the exact scenario being validated.

Good examples:

- `solve dT/dt = (80 C - T)/10 s`
- `thermal diffusivity of aluminum in m^2/s`
- `convert 0.12 W/(m K) to W/(mm K)`
- `time constant for 0.52 kg battery with cp 1000 J/(kg K) and heat flow 15 W`

### 2. Choose the endpoint

- Use the website URL for quick manual confirmation.
- Use the Short Answers API for compact numeric checks.
- Use the Full Results API when pods, assumptions, or structured output are needed.
- Use the LLM API only when a compact machine-readable explanation is more useful than raw pods.

Read `references/wolfram-workflow.md` when choosing an endpoint or query pattern is not obvious.

### 3. Apply the verification rule

Do not freeze a constant, formula, or conversion into code or docs until:

- the expression has been checked,
- the units are consistent,
- and the result is traceable to the Wolfram query used.

### 4. Keep the project honest

If WolframAlpha does not support a query cleanly, say so and keep the approximation labeled as a heuristic in both docs and code.

## Resources

### scripts/

- `scripts/wolfram_verify.py`: build a WolframAlpha query URL and optionally call the official API when an AppID is available through `WOLFRAM_APP_ID`.

### references/

- `references/wolfram-workflow.md`: endpoint selection, query templates, and verification checklist for the drone thermal system.