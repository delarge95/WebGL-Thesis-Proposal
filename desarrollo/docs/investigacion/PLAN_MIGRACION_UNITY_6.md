# Plan de Migración a Unity 6.3 (Enfoque Mesh LOD y WebGL)

Este documento detalla la investigación y el plan de acción para migrar el Gemelo Digital desde la versión actual hacia **Unity 6.3**, con el objetivo principal de aprovechar el nuevo sistema de **Mesh LOD**. 

Esta migración se ejecutará **exclusivamente** cuando la Etapa 4 y 5 del Sistema Térmico actual estén finalizadas y validadas.

---

## 1. Investigación: ¿Qué es el Mesh LOD de Unity 6.3?

El sistema de Mesh LOD (introducido en Unity 6.2 y refinado en 6.3) cambia completamente el paradigma de optimización respecto a los antiguos `LOD Groups`.

### Ventajas Clave para nuestro WebGL
* **Cero GameObjects adicionales:** No requiere crear una jerarquía de modelo_LOD0, modelo_LOD1, etc.
* **Huella de memoria mínima:** Todos los LODs se guardan dentro del mismo *Index Buffer* de la malla original, reutilizando el mismo *Vertex Buffer*. Esto es el "Santo Grial" para reducir el peso de descarga del archivo `.data` en WebGL.
* **Generación Automática:** Se activa directamente en las opciones de importación (Import Settings) del modelo 3D.
* **Visualización en 6.3:** Unity 6.3 añade barras deslizables en el Inspector del `Mesh Renderer` para depurar qué LOD se está renderizando y visualizadores en tiempo real en la vista de escena.

### Limitaciones Críticas (Lo que hay que tener en cuenta)
* **Destruye el Static Batching:** Las mallas que usan Mesh LOD no pueden usar *Static Batching*. En nuestro caso, como el shader térmico cambia colores por vértice en tiempo real, ya teníamos el Static Batching comprometido, por lo que no es una gran pérdida, pero afectará el conteo de *Draw Calls*.
* **Poligonaje mínimo:** Solo funciona en mallas que tengan **al menos 256 triángulos**. Piezas de *Tier Mínimo* (como los tornillos o la placa del PDB si es muy simple) serán ignoradas por el sistema.
* **Compatibilidad de Shaders:** Dado que reutiliza el Vertex Buffer, necesitaremos verificar que nuestro `Thermal.shader` (que confía fuertemente en coordenadas espaciales y vértices) lea correctamente las reducciones indexadas sin deformar los gradientes de calor.

---

## 2. Cambios Estructurales (Breaking Changes) en Unity 6 para WebGL

Actualizar a Unity 6 trae cambios de arquitectura fundamentales para compilar en web:

1. **Muerte de WebGL 1.0:** Unity 6 elimina/deprecia por completo el soporte para WebGL 1.0. Esto nos obliga a estar 100% seguros de que el proyecto solo compila para **WebGL 2.0**. Además, el *Linear Color Space* es ahora obligatorio (lo cual mejora la estética visual, pero los materiales podrían verse ligeramente más oscuros o brillantes inicialmente).
2. **Soporte Oficial Móvil:** Esta es la mejor noticia para la tesis. Unity 6 soporta *oficialmente* navegadores móviles (iOS Safari, Android Chrome). Ya no saldrá el cartel de advertencia de "WebGL is not supported on mobile devices".
3. **WebGPU (Preview):** Introduce un renderizador experimental WebGPU, mucho más rápido que WebGL 2.0, aunque se recomienda mantenerlo apagado para producción hasta que sea estable.

---

## 3. Plan de Acción (Hoja de Ruta de Migración)

Este es el paso a paso a ejecutar **después** de cerrar el solver térmico:

### Fase 1: Preparación (Pre-Unity 6)
- [ ] Finalizar y hacer `git commit` / `Push` del Sistema Térmico funcionando perfecto en la versión actual (Unity 2022/2023).
- [ ] Crear nueva rama en el repositorio: `feature/unity6-migration`.
- [ ] En *Player Settings*, forzar explícitamente **WebGL 2.0** y **Linear Color Space**.

### Fase 2: El Salto de Motor (Upgrade)
- [ ] Instalar Unity 6.3.x (LTS) vía Unity Hub.
- [ ] Abrir el proyecto en la nueva versión.
- [ ] Resolver errores de consola inmediatos (generalmente alertas de APIs obsoletas relacionadas a UI Toolkit o input systems).

### Fase 3: Integración de Mesh LOD
- [ ] Seleccionar todas las mallas del Dron en la carpeta de *Project* (Especialmente el Tier Crítico: Motores, Brazos, Batería, Landing Gear).
- [ ] En el Inspector de importación, habilitar la casilla **"Generate Mesh LOD"**.
- [ ] Configurar el ratio de reducción (p. ej., preservar 50% de polígonos a media distancia, 20% a larga distancia).
- [ ] Añadir a la cámara principal o escena la configuración necesaria para que Unity evalúe la distancia.
- [ ] Usar la nueva herramienta de depuración en la Escena (Unity 6.3 toggle labels) para verificar numéricamente el ahorro poligonal.

### Fase 4: QA y Validación Térmica
- [ ] Probar el encendido térmico (Solver). 
- [ ] Alejar la cámara hasta que haga el salto de LOD 0 a LOD 1.
- [ ] Verificar que el *Visual Effect* de calor (`Thermal.shader`) no se corrompa al reducirse la de los índices del vertex buffer.
- [ ] Compilar (Build) para WebGL y desplegar en GitHub Pages.
- [ ] Cargar el link en un teléfono móvil para validar la nueva compatibilidad oficial móvil de Unity 6.
