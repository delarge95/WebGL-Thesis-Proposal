# Guía de Demo - WebGL Drone Viewer

Esta guía presenta el recorrido recomendado para realizar una demostración completa y efectiva de la aplicación final documentada en el informe.

## Regla Principal
La demo debe mostrar exclusivamente el flujo visible y verificado para la entrega:
`Hero -> Explore -> selección -> bottom sheet -> Inspect / Analyze / Studio`

**No se deben mostrar ni mencionar en la demo final:**
- Herramientas de medición (ocultas).
- Catálogos de piezas o settings panels (legados).
- Modos Wireframe, Ghosted o Blueprint explícito si no están en las tarjetas visibles.
- Efectos o interacciones en progreso.

## Preparación de la Demo
1. Abrir la URL de GitHub Pages o la build local `MainScene_Final`.
2. Mostrar la pantalla **Hero**, mencionando que es la puerta de entrada y contexto del proyecto.
3. Hacer clic en **HELP** para mostrar las tarjetas animadas de **Onboarding procedural**, explicando que reemplazan videos externos por animaciones en la propia interfaz.
4. Hacer clic en **EXPLORE** para entrar al modelo 3D.

## Recorrido Paso a Paso

### 1. Interacción Base y Onboarding Procedural
- Hacer clic en **HELP** para mostrar las tarjetas animadas de **Onboarding procedural**, explicando que no son videos, sino animaciones en la propia interfaz dibujadas con `Painter2D`.
- Demostrar la órbita, pan y zoom. Explicar cómo la sensibilidad de la cámara y el paso del zoom se ajustan dinámicamente si se enfoca el dron completo frente a una pieza muy pequeña.

### 2. Inspección de Fasteners y Jerarquía de Selección
- Seleccionar una pieza madre y demostrar cómo el sistema reconoce el conjunto principal.
- Hacer un segundo clic sobre una geometría interna para descender a una **subpieza**.
- Seleccionar un fastener (tornillo). Demostrar cómo el sistema sustituye dinámicamente su representación proxy temporal por **detalle procedural** completo gracias al `FastenerInspectionManager`, manteniendo la escena ligera.
- Hacer doble clic para **aislar** la pieza o fastener. Explicar que el fastener se mantiene visible como una unidad funcional completa.

### 3. Ficha Técnica (Bottom Sheet)
- Abrir la ficha técnica inferior desde la barra de previsualización o arrastrando hacia arriba.
- Mostrar cómo se dividen los datos técnicos: *Identification*, *Specifications*, y *Assembly*.
- Aclarar que la UI muestra valores como "N/A" intencionalmente cuando la pieza no requiere esos datos, demostrando un sistema robusto, no un *mockup* estático.

### 4. Modo Inspect
- Activar los **Pins (Hotspots)**. Hacer clic en uno para demostrar cómo agrupan la lectura funcional de sistemas enteros.
- Encender el dron con el botón **Power**. Cambiar el **Load** con el deslizador. Mostrar cómo las hélices giran y la etiqueta de estado de la aplicación cambia (ej. `OFF` -> `STARTING` -> `IDLE` -> `FLYING`).

### 5. Modo Analyze
- Activar el slider de **Explode**. Separar el ensamblaje en tiempo real.
- Activar el panel **Cut** (Corte transversal). Habilitar los ejes Y y X de manera simultánea para mostrar el clipping global dual que afecta a todos los shaders por igual.
- Abrir **Filter** y hacer doble clic sobre una familia funcional (ej. *Avionics*) para aislarla visualmente.

### 6. Modo Studio y Simulación Térmica
- Mostrar los tres modos visuales: **X-Ray** (para relaciones internas), **Solid** (para siluetas sólidas) y **Thermal**.
- Cambiar los ciclos de presets en la tarjeta de entorno: `Studio` (que incluye modo Blueprint), `Time` y `Color`.
- Modificar los deslizadores de iluminación (`Light Rotation`, `Light Intensity`, `Background Tone`).
- Activar **Thermal**. Explicar claramente: *"Esta es una visualización térmica híbrida y heurística por componentes. No es un solver termodinámico exacto, sino que responde directamente a la carga (Load) actual"*. Mostrar la leyenda térmica (20°C a 100°C) y cómo eleva sus colores cálidos al exigir carga al sistema.

### 6. Cierre de la Demostración
Regresar a la vista general con el botón **RESET VIEW** y apagar el dron.
Concluir la presentación explicando que el valor de la aplicación no radica en mostrar un modelo 3D bonito, sino en **orquestar datos, visualización y estructura para hacer legible un objeto de ingeniería compleja**.

---

## Preguntas y Respuestas Frecuentes
- **¿Es una simulación física real?** No, es visualización técnica apoyada por modelos heurísticos.
- **¿Qué pasó con los shaders Blueprint o Wireframe?** El sistema los soporta internamente, pero se priorizaron `X-Ray`, `Solid` y `Thermal` en la interfaz final para evitar fatiga cognitiva, dejando el resto como funciones de motor (ej. Blueprint disponible vía entorno).
- **¿Por qué a veces la cámara se frena al acercarse?** El zoom es adaptativo; calcula los límites basados en el tamaño de la pieza seleccionada para no perder el contexto.
