# Thermal Math Verification

1. Normalize the equation or conversion with explicit units.
2. Run `python .agent/skills/wolfram-thermal-verifier/scripts/wolfram_verify.py "<query>" --endpoint website` // turbo
3. If `WOLFRAM_APP_ID` is available, rerun with `--endpoint short --fetch` or `--endpoint full --fetch` depending on the level of detail required.
4. Compare the Wolfram result against the equation or constant planned for code or docs.
5. Record the verified query and result before freezing the value into the thermal system.
