# Plan de Activos Visuales para Portafolio

Documento operativo para convertir los breakdowns ya redactados en material visual concreto para ArtStation, LinkedIn, reel y CV.

## Objetivo

Definir exactamente qué screenshots, clips, GIFs y diagramas hacen falta para que las piezas de portafolio no dependan sólo de texto.

## Prioridad 1. Hero del proyecto

- **Hero render del dron completo**
  - Modo sugerido: Blueprint o Thermal.
  - Uso: portada de ArtStation, banner de LinkedIn, thumbnail del reel.
- **Hero render alternativo en vista realista**
  - Uso: contraste con la vista técnica y apoyo para CV o deck.

## Prioridad 2. Breakdown del pipeline CAD a WebGL

- **Screenshot del estado fuente CAD o ensamblaje técnico**
  - Uso: introducir el problema original.
- **Lámina del problema de pivotes, duplicados o transformaciones horneadas**
  - Uso: explicar por qué hubo que desarrollar tooling propio.
- **Comparativa antes/después de optimización geométrica**
  - Uso: evidenciar reducción de complejidad.
- **Diagrama del pipeline**
  - Flujo mínimo: CAD → limpieza → simetría/instancing → optimización → shading → Unity → WebGL.
  - Uso: slide central del breakdown y apoyo para entrevistas.
- **Clip corto del resultado final en navegador**
  - Uso: cierre de la pieza y prueba de entrega real.

## Prioridad 3. Breakdown del sistema interactivo

- **Clip de selección de pieza + apertura de info panel**
  - Uso: demostrar lectura contextual y navegación del producto.
- **Grid de modos de visualización**
  - Incluir como mínimo: Realistic, Blueprint, Thermal, X-Ray y Wireframe.
  - Uso: slide o imagen comparativa de features.
- **Clip de exploded view**
  - Uso: mostrar lectura del ensamblaje y separación de componentes.
- **Clip de corte transversal**
  - Uso: reforzar el valor analítico del visor.
- **Lámina simple de arquitectura o flujo interactivo**
  - Flujo sugerido: input → selección → estado → UI/render.

## Prioridad 4. Pruebas de valor técnico

- **Close-up del shader Thermal con anotaciones**
  - Uso: explicar lectura funcional, no sólo estética.
- **Close-up de Blueprint o WireframeWebGL**
  - Uso: hablar de compatibilidad WebGL y decisiones de rendering.
- **Captura del manifiesto técnico o del código staged**
  - Uso: apoyar claims de tooling, HLSL y C# sin saturar la pieza.

## Entregables mínimos por canal

### ArtStation

- 1 portada hero.
- 1 diagrama de pipeline.
- 1 comparativa antes/después.
- 1 grid de view modes.
- 2 clips o GIFs cortos.

### LinkedIn

- 1 carrusel resumido de 4 a 6 slides.
- 1 clip vertical o cuadrado.
- 1 texto corto apoyado en claims reutilizables.

### Reel

- 1 apertura hero.
- 1 bloque de interacción del visor.
- 1 bloque de shaders / modos.
- 1 bloque de pipeline.
- 1 cierre con métricas y stack.

## Orden recomendado de captura

1. Hero renders.
2. Grid de view modes.
3. Clip de selección + info panel.
4. Clip de exploded view.
5. Clip de corte transversal.
6. Comparativa antes/después.
7. Diagrama de pipeline.

## Dependencias abiertas

- URL pública final del build WebGL para capturas limpias.
- Decidir si las cifras públicas serán conservadoras o exhaustivas.
- Confirmar si el hero principal será Blueprint o Thermal.
- Preparar una carpeta en `assets_fuente/` para almacenar estas capturas cuando empiece la producción visual.
