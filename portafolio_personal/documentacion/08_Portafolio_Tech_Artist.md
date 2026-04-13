# Portafolio Tech Artist - Enfoque de Cierre 2026

## Objetivo

Convertir la tesis en un portafolio coherente, verificable y competitivo para un perfil `junior/mid Technical Artist` con foco en:

- tools
- shaders
- optimization
- technical visualization

## Propuesta de valor

> Technical Artist orientado a visualización técnica interactiva en tiempo real. Capaz de conectar normalización de activos, runtime tools, shaders, documentación y arquitectura de producto para entregar experiencias WebGL legibles y mantenibles.

## A quién debe convencer el portafolio

### Recruiter

Quiere entender rápido:

- qué construiste;
- si corre de verdad;
- qué parte fue tuya;
- si el proyecto se ve profesional y verificable.

### TA Lead / Hiring Manager

Quiere verificar:

- criterio técnico;
- capacidad para resolver restricciones reales;
- valor de las herramientas creadas;
- honestidad sobre lo que está publicado, oculto o pendiente.

### Equipo de arte, producto o visualización

Quiere ver:

- sensibilidad visual;
- claridad de UX;
- capacidad para traducir un sistema técnico a una experiencia entendible;
- orden en pipeline y comunicación.

## Fuente de verdad del discurso

El portafolio debe apoyarse en:

- la app real;
- la documentación técnica activa;
- las auditorías de coherencia;
- las capturas y clips reales;
- la investigación aplicada que sí tenga relación directa con el producto.

No debe apoyarse en:

- slides viejos;
- claims históricos;
- features ocultas tratadas como shipping;
- research exploratorio como implementación real.

## Paquete principal del portafolio

### 1. Visor WebGL final del Holybro X500 V2

Debe ser la pieza principal porque ya existe una experiencia interactiva verificable.

#### Qué mostrar

- Hero final;
- selección + `bottom sheet`;
- `Inspect / Analyze / Studio`;
- leyenda térmica visible.

#### Qué vender

- experiencia web técnica y navegable;
- lectura contextual del ensamblaje;
- producto real, no solo render.

#### Qué no vender

- catálogo visible como feature final;
- settings visibles;
- measurement visible;
- ensamblaje completo expuesto.

### 2. Sistema de shaders y visualización

Debe presentarse como una solución de lectura visual aplicada.

#### Qué mostrar

- `X-Ray`
- `Solid Color`
- `Thermal`
- `Realistic`

Opcional, como profundidad técnica:

- `Blueprint`
- `Wireframe`
- `Ghosted`

#### Qué vender

- lenguaje visual técnico;
- compatibilidad WebGL;
- combinación de URP + shaders propios + clipping.

#### Qué no vender

- todos los modos como si estuvieran expuestos en la UI final;
- una BRDF completamente escrita desde cero.

### 3. Sistema térmico híbrido

Debe presentarse como `visualización aplicada`, no como solver físico de alta fidelidad.

#### Qué mostrar

- arquitectura del subsistema;
- modo térmico con leyenda;
- explicación corta de la lógica heurística;
- tooling de grafo térmico.

#### Qué vender

- integración de estado, solver reducido, shader y UI;
- criterio para traducir datos técnicos a lectura visual.

#### Qué no vender

- FEA;
- validación experimental cerrada;
- termografía real calibrada.

### 4. Caso CAD -> Unity -> WebGL

Debe presentarse como caso de pipeline y saneamiento técnico.

#### Qué mostrar

- problema de entrada;
- taxonomía `28 / 30 / 257`;
- `ImportedDroneRuntimeBinder`;
- resultado final en navegador.

#### Qué vender

- normalización semántica;
- reparación de import;
- pensamiento sistémico sobre restricciones WebGL.

#### Qué no vender

- números finales de optimización si el reimport/freeze sigue pendiente;
- pipelines exploratorios como si fueran oficiales.

### 5. Tooling de editor y verificación

Esta es una fortaleza diferencial real del proyecto.

#### Qué mostrar

- `ProjectSetupWizard`
- `ImportedDroneCoverageAudit`
- `ThermalContactGraphBuilderWindow`
- `ImportedDroneRuntimeBinder`

#### Qué vender

- no solo se construyó una escena;
- también se construyeron herramientas para configurarla, verificarla y mantenerla consistente.

### 6. Comunicación técnica y arquitectura

Esta pieza no reemplaza el visor, pero suma mucho valor en entrevistas.

#### Qué mostrar

- diagrama corto de arquitectura;
- flujo de selección;
- pipeline de shaders;
- diagrama del sistema térmico;
- matriz de desconexiones o nota breve de alcance real.

#### Qué vende

- claridad;
- criterio editorial;
- capacidad para documentar sistemas complejos sin sobreprometer.

## Claims que sí se pueden usar

- construcción de un visor WebGL técnico para un dron real;
- integración de UI, selección, modos visuales y lectura contextual;
- saneamiento runtime del modelo importado;
- tooling de editor para setup, auditoría y grafo térmico;
- uso de shaders para visualización técnica aplicada;
- integración de documentación, arquitectura y límites del sistema.

## Claims que deben salir del discurso principal

- audio implementado;
- ensamblaje completo expuesto al usuario final;
- catálogo visible como feature final;
- settings visibles como feature final;
- módulos inexistentes en el repo activo;
- Houdini como pipeline central de la build final;
- cifras históricas como `16 piezas` o `91 scripts`;
- modos ocultos presentados como experiencia final.

## Regla sobre research y blueprint

El research del proyecto sí suma valor al portafolio, pero debe aparecer bien clasificado:

- como fundamento técnico;
- como diseño de pipeline;
- como evidencia de criterio;
- no como sinónimo automático de implementación cerrada.

Esto aplica especialmente a:

- `CAD-to-Unity WebGL Optimization  Complete Technical Blueprint for Drone Visualization.md`;
- `CAD_Fastener_Optimization_Plan.md`;
- documentos sobre PiXYZ, Simplygon, Quad Remesher o estrategias equivalentes.

## Estructura recomendada para ArtStation / sitio / README

1. Problema.
2. Solución.
3. Flujo visible del producto.
4. Arquitectura resumida.
5. Tooling de soporte.
6. Sistema visual y modos.
7. Sistema térmico.
8. Pipeline CAD -> WebGL.
9. Límites honestos del estado final.

## Lista mínima de activos

- screenshot del Hero final;
- screenshot del `bottom sheet`;
- screenshot de `Inspect`;
- screenshot de `Analyze`;
- screenshot de `Studio`;
- screenshot de `Thermal` con leyenda;
- clip corto del flujo completo;
- screenshot de tooling de editor;
- diagrama corto de arquitectura;
- diagrama corto del subsistema térmico.

## Orden recomendado de publicación

### Fase 1

- visor final;
- breakdown corto del sistema interactivo;
- breakdown corto de view modes;

### Fase 2

- tooling de editor;
- sistema térmico híbrido;

### Fase 3

- pieza CAD -> Unity -> WebGL con métricas finales, solo cuando exista freeze;
- breakdown más profundo de pipeline y optimización.

## Principio editorial final

El portafolio no debe intentar venderte como todo a la vez. Su mejor lectura es esta:

> Technical Artist que sabe convertir un problema técnico real en una experiencia interactiva clara, optimizada, bien instrumentada y honestamente documentada.
