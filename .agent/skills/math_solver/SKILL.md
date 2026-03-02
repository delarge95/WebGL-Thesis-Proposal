---
description: Native Math & Logic Solver for Physics and Engineering Calculations
---

# `math_solver` Skill

## Goal
Validar ecuaciones físicas del dron (carga alar, empuje, torque de motores) y generar las tablas/fórmulas LaTeX para la documentación oficial, prescindiendo de APIs externas (Wolfram).

## Capabilities
- Escritura y ejecución de scripts nativos en Python en el entorno temporal (`/tmp/`).
- Resolución analítica y numérica usando dependencias base (`math`) o `sympy`/`numpy` si el entorno local los posee.
- Formateo de las salidas directamente en sintaxis matemática de LaTeX (`amsmath`).

## Execution Protocol
1. Cuando se necesite verificar una métrica física: Crea un script en `/tmp/solve_metric.py`.
2. Establece las constantes del drone según `DronePartData.cs` o la Tabla R-01 del documento técnico.
3. Ejecuta el archivo vía el comando de terminal local (`python /tmp/solve_metric.py`).
4. Lee el output y formatea el resultado en un bloque LaTeX preciso.
5. Embebe el bloque dentro de la revisión de documentación (`tech_writer`).
