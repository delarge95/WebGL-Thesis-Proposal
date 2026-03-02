# Skill Architecture & Document Generation Plan

This document outlines the strategy for generating the final formal thesis documentation and the architecture for the required native Agent Skills. Since external API keys (like Wolfram) are not viable, the agent will rely on native Python environments and strict file-system tools.

## 1. Native Agent Skills to be Created

El proyecto ya cuenta con múltiples *skills* para Unity (`arch_guard`, `webgl_optimizer`, etc.). Se desarrollarán los siguientes nuevos *skills* para satisfacer las demandas académicas de la tesis:

### 1.1 `math_solver` (Lógica y Matemáticas)
- **Propósito:** Validar ecuaciones físicas del dron (carga alar, empuje, torque de los motores) y generar las tablas/fórmulas LaTeX para la Propuesta y el Informe.
- **Implementación:** 
  - Scripts en Python estructurados (usando `sympy`, `numpy` si estuvieran instalados, o simplemente la librería estándar `math`).
  - El Agente escribirá el script en `/tmp/`, ejecutará un comando de terminal nativo y extraerá el resultado para inyectarlo en la documentación.
- **Output:** Bloques LaTeX precisos.

### 1.2 `ux_auditor` (Auditoría UX/UI y SUS)
- **Propósito:** Automatizar la validación teórica de la interfaz WebGL basándose en las heurísticas de Nielsen y las recomendaciones de Material Design / Apple HIG (complementando el skill existente `unity_ui_pro`).
- **Implementación:**
  - Evaluará los layouts `.uxml` y `.uss` para asegurar tamaños de áreas táctiles ($>$ 44x44px), contrastes de color, y tiempos de respuesta teóricos.
  - Generará reportes formateados y simulará la tabulación de los resultados SUS una vez que se obtengan datos de usuarios.

### 1.3 `tech_writer` (Generación de Documentos)
- **Propósito:** Escribir y formatear documentos `.tex` o `.md` garantizando coherencia terminológica (e.g. siempre decir "Unity 6 (6000.0.x) LTS").
- **Implementación:**
  - Uso intensivo del `EventBus` y `GameManager` analizados en el `CODEBASE_TRUTH.md` para evitar inventar clases.
  - Reglas estrictas para no borrar comandos LaTeX cruciales (como delimitadores de sección) al actualizar documentos.

### 1.4 `workflow_generator`
- **Propósito:** Generar Workflows automatizados (archivos markdown con instrucciones paso a paso + tags `// turbo`) en `.agent/workflows/`.
- **Implementación:**
  - Creará pipelines de comandos unificados (ej. "Compilar Proyecto", "Correr Tests", "Actualizar Documentos").

## 2. Generación de Documentación Formal

### 2.1 Estrategia de Sincronización
Toda la documentación (`Propuesta`, `Informe_final`, y Manuales) debe sincronizarse con la ÚNICA FUENTE DE VERDAD (`CODEBASE_TRUTH.md`). 

1. **Reescribir Métricas:** Cambiar todas las menciones difusas ("65+ scripts", "9000 líneas") por las reales.
2. **Actualizar Diagramas:** Reflejar que el UI Toolkit usa un controlador monolítico `UIModeController` y que la lógica de encendido está en `DroneStateController`.
3. **Consolidar Versiones:** Garantizar que en todo documento diga "Unity 6".

### 2.2 Orden de Generación y Refactor
1. Modificar `Propuesta/sections/resultados.tex` y `objetivos.tex` para alinear.
2. Actualizar el `01_Arquitectura_del_Sistema.md` con las métricas correctas de Singletons y dependencias.
3. Actualizar `Manual_tecnico/` y `Manual_usuario/` para que coincidan.
4. Crear la estructura de `workflows` y `skills` nuevos prometidos.

---
**Status actual:** Plan diseñado. Listo para iniciar Fase 5 (Implementación).
