# Plan técnico: onboarding con iconos e ilustraciones por tarjeta

Fecha: 2026-04-15

## Objetivo

Ampliar el onboarding actual para que cada tarjeta incluya una representación visual breve y útil de la acción que explica. La solución prioriza estabilidad, bajo peso y mantenimiento simple en WebGL/Unity.

La recomendación base es:

- usar un icono o una microilustración estática por tarjeta;
- reservar animación solo para 1 o 2 pasos críticos;
- evitar GIF en runtime;
- mantener el onboarding ligero y legible.

## Alcance funcional

El onboarding debe cubrir las funciones visibles y reales de la app:

- navegación 3D: orbit, pan, zoom;
- selección jerárquica: pieza madre, subpieza y doble clic;
- hotspots / pins;
- inspect e isolate;
- control de power / load;
- analyze: cut, explode, filter, cross-section;
- thermal mode;
- studio / environment / blueprint.

## Propuesta visual por tarjeta

### Tarjetas estáticas

1. Orbit & Zoom
   - icono de ratón o gesto de arrastre;
   - opcional: mini silueta del dron con flechas de rotación.

2. Select Parts
   - ilustración de un dron con una pieza resaltada;
   - puede mostrar dos niveles: mother part y subpiece.

3. Hotspots
   - pins o marcadores sobre una parte resaltada;
   - útil para explicar que no todas las piezas llevan hotspot.

4. Inspect
   - mini panel lateral con una selección activa;
   - icono de lupa o ficha de información.

5. Power
   - icono de encendido + barra de carga;
   - visual claro para el estado OFF/ON y el slider.

6. Analyze
   - ilustración de corte o explosión con eje o separación;
   - puede combinarse con una vista simplificada de filtros.

7. Thermal
   - degradado térmico con leyenda min/max;
   - enfatiza que el modo cambia la representación de temperatura.

8. Studio
   - tarjeta con ambiente + blueprint como variante;
   - debe sugerir que el modo cicla entre presets de render y entorno.

### Estados animados especiales

1. Orbit & Zoom
   - animación breve de 2 a 4 frames o loop suave;
   - objetivo: enseñar rotación y zoom sin texto extra.

2. Select Parts
   - animación de resaltado de una pieza madre y una subpieza;
   - objetivo: explicar selección jerárquica con más claridad.

## Arquitectura propuesta

### UI

- ampliar `OnboardingController.cs` para que cada `Step` tenga referencia visual;
- añadir en `MainLayout.uxml` un contenedor visual por tarjeta;
- ajustar `Theme.uss` para fijar dimensiones, proporción y alineación;
- si hay animación, limitarla a elementos muy pequeños y controlados.

### Datos

Definir una estructura de paso más rica, por ejemplo:

- `title`
- `description`
- `icon` o `sprite`
- `visualType` (`static`, `animated`)
- `visualId`

### Assets

Preferencia de formato:

- iconos simples: vector o UI icon;
- microilustraciones: PNG o WebP optimizado;
- animaciones: secuencia de sprites o sprite sheet.

No se recomienda GIF en runtime por peso, rendimiento y mantenimiento.

## Archivos implicados

- `desarrollo/unity_project/Assets/Scripts/UI/Panels/OnboardingController.cs`
- `desarrollo/unity_project/Assets/Scripts/UI/Layouts/MainLayout.uxml`
- `desarrollo/unity_project/Assets/UI/Styles/Theme.uss`
- carpeta de assets visuales del proyecto, idealmente una subcarpeta dedicada al onboarding

## Fases de implementación

### Fase 1: definición visual

- cerrar el contenido exacto de cada tarjeta;
- definir si cada paso usa icono, ilustración o animación;
- decidir qué dos pasos reciben tratamiento especial.

### Fase 2: estructura UI

- añadir el contenedor visual en el overlay;
- fijar tamaño de tarjeta y espacio para la imagen;
- verificar legibilidad en desktop y resolución reducida.

### Fase 3: integración de assets

- cargar imágenes o sprites desde assets del proyecto;
- mapear cada paso a su recurso visual;
- verificar que el onboarding no rompa el flujo de apertura/cierre.

### Fase 4: pulido y validación

- revisar contrastes, tamaños y márgenes;
- probar carga inicial en WebGL;
- medir impacto de memoria y tiempo de arranque;
- confirmar que el tutorial sigue siendo coherente con la app real.

## Complejidad estimada

### Opción mínima: icono por tarjeta

- complejidad: baja;
- impacto en build: muy bajo;
- mantenimiento: simple.

### Opción recomendada: icono + microilustración por tarjeta

- complejidad: media;
- impacto en build: bajo a medio;
- mantenimiento: razonable;
- mejor relación claridad/peso.

### Opción extendida: 2 animaciones especiales + resto estático

- complejidad: media-alta;
- impacto en build: bajo a medio si se optimiza bien;
- mantenimiento: más exigente, pero todavía controlable.

### Opción no recomendada: GIF en runtime

- complejidad: media;
- impacto en build: impredecible y normalmente peor;
- mantenimiento: débil;
- no es la mejor opción para WebGL.

## Estimación de peso

Valores orientativos por tarjeta:

- icono simple: prácticamente nulo o muy bajo;
- microilustración PNG/WebP optimizada: 15 a 60 KB;
- ilustración más detallada: 60 a 200 KB;
- animación por sprites: 150 a 600 KB según duración y resolución.

Escenario recomendado:

- 6 tarjetas estáticas ligeras: 0.1 a 0.5 MB aprox.;
- 2 tarjetas especiales animadas: 0.3 a 1.2 MB aprox.;
- impacto total esperado: alrededor de 0.4 a 1.7 MB, dependiendo de la compresión final.

## Riesgos

- exceso de detalle visual en tarjetas pequeñas;
- aumento de tiempo de carga en WebGL si las imágenes no se comprimen;
- desalineación entre tutorial y UI real si el onboarding no se actualiza junto con nuevos modos;
- uso de demasiada animación para explicar algo simple.

## Criterios de aceptación

- cada tarjeta muestra una señal visual clara;
- las dos acciones críticas se entienden sin leer demasiado texto;
- el onboarding sigue siendo legible en pantallas pequeñas;
- la app no gana peso de forma desproporcionada;
- el contenido sigue reflejando la UI real y visible.

## Recomendación final

Implementar primero la versión estática con iconos o microilustraciones por tarjeta. Después, si hace falta reforzar comprensión, añadir animación solo en Orbit & Zoom y Select Parts.

Esa combinación ofrece el mejor equilibrio entre claridad, peso y mantenimiento para este proyecto WebGL.
