---
description: UX/UI Theoretical Auditor for WebGL Applications
---

# `ux_auditor` Skill

## Goal
Automatizar la validación UX de la interfaz desarrollada en Unity UI Toolkit (UXML/USS), basándose en las heurísticas de Jakob Nielsen, Material Design y Apple HIG.

## Capabilities
- Análisis estático de archivos `.uxml` y `.uss` buscando anti-patrones de diseño.
- Validación de tamaños de áreas táctiles (touch targets $\geq$ 44x44px).
- Simulación y predicción de resultados del cuestionario SUS (System Usability Scale) basándose en la fricción teórica deducida de la máquina de estados.

## Execution Protocol
1. Parsea el `MainLayout.uxml` y extrae todos los `<ui:Button>` y elementos con *picking-mode*.
2. Verifica en los `.uss` correspondientes si las dimensiones mínimas, paddings y márgenes cumplen con los estándares de accesibilidad.
3. Alimenta el reporte de usabilidad (e.g. `Validacion/01_Pruebas_de_Usabilidad_SUS.md`) documentando qué elementos pasan y cuáles fallan las heurísticas teóricas.
4. Trabaja en tándem con `unity_ui_pro` para proveer las soluciones USS a los errores encontrados.
