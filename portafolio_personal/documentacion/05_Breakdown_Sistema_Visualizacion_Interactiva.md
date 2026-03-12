# Breakdown: Sistema de Visualización Interactiva

Resumen corto para presentar el visor como pieza de producto técnico, no sólo como demo visual.

## Pieza

- **Título recomendado:** Interactive WebGL Technical Viewer for a Drone Digital Twin
- **Formato recomendado:** breakdown de producto con video corto, capturas anotadas y lista de features clave.
- **Ángulo de venta:** experiencia interactiva orientada a exploración técnica de un objeto real, combinando UI, datos, shaders y navegación 3D.

## Qué hace valiosa esta pieza

No se trata únicamente de mostrar un dron en 3D. El visor convierte el modelo en una herramienta de lectura técnica: permite seleccionar piezas, ver información contextual, cambiar modos de visualización, explorar el ensamblaje y aplicar overlays útiles para análisis.

## Capacidades que conviene destacar

### 1. Exploración de piezas

- Selección de componentes dentro del modelo 3D.
- Panel de detalle con información contextual.
- Navegación orientada a entender el ensamblaje y sus partes.

### 2. Visualización técnica

- Múltiples modos de visualización para lectura del modelo.
- Modos como Blueprint, Thermal, X-Ray o Wireframe funcionan como herramientas analíticas, no sólo estéticas.
- El sistema de clipping y corte transversal amplía el valor del visor para explicación técnica.

### 3. Interacción y cámara

- Navegación 3D con orbit, pan y zoom.
- Ajustes recientes de UX reforzaron la consistencia entre interacción, onboarding y comportamiento real de cámara.
- El info panel y la UI inferior están pensados como overlays de consulta y no como ventanas desconectadas del modelo.

### 4. Arquitectura de producto

- El visor se apoya en managers especializados para selección, modos de vista, corte y despiece.
- La arquitectura desacopla eventos, estado y comportamiento visual de forma suficiente para seguir iterando la experiencia.
- La pieza permite hablar de diseño de sistemas, no sólo de gráficos.

## Problemas interesantes que esta pieza resuelve

- Cómo mantener legibilidad técnica en una interfaz WebGL sin saturar la pantalla.
- Cómo hacer persistentes ciertas herramientas de análisis sin pelear contra el flujo de selección del modelo.
- Cómo sincronizar shading, clipping, cámara y paneles para que se perciban como un solo sistema.

## Qué conviene mostrar

- Un clip de selección de piezas y apertura del info panel.
- Un grid de modos de visualización.
- Un clip corto de exploded view y de corte transversal.
- Una lámina simple con la arquitectura de managers o el flujo input → selección → estado → UI/render.
- Un antes/después breve de los ajustes UX que reforzaron la interacción.

## Mensaje central para portafolio

La pieza demuestra capacidad para diseñar una experiencia técnica interactiva sobre un modelo 3D complejo, conectando runtime, UI, arquitectura y visualización especializada dentro de un mismo producto.

## Claims reutilizables

- Diseñé un visor WebGL interactivo para exploración técnica de un drone real.
- Integré selección, overlays informativos, modos de visualización y navegación 3D dentro de una sola experiencia.
- Trabajé sobre la arquitectura y la UX del sistema para evitar fricción entre cámara, UI y análisis visual.
- Convertí un prototipo académico en una pieza defendible también como producto técnico y portafolio.

## Evidencia técnica asociada

- `desarrollo/docs/ARCHITECTURE.md`
- `desarrollo/docs/investigacion/14_analisis_problemas_app_2026-03-10.md`
- `portafolio_personal/herramientas_fuente/shaders/`
- `portafolio_personal/herramientas_fuente/ui_tecnica/`

## Activos pendientes para cerrar esta pieza

- Video corto con navegación real del visor.
- Grid final de screenshots por modo.
- Lámina simple de arquitectura o flujo de interacción.
- Selección final de 2 o 3 features a enfatizar según si el destino es ArtStation, LinkedIn o CV.
