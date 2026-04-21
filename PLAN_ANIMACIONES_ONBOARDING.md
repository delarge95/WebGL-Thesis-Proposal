# Plan robusto de animaciones del onboarding

## Estado de implementacion (Abril 21, 2026)

Este plan ya no describe una idea abstracta unicamente. A la fecha existe un MVP implementado en:

- `Assets/Scripts/UI/Panels/OnboardingAnimationView.cs`
- `Assets/Scripts/UI/Panels/OnboardingController.cs`
- `Assets/Scripts/UI/Layouts/MainLayout.uxml`
- `Assets/UI/Styles/Theme.uss`

Estado actual del MVP:

- las `15` cards ya tienen preview procedural runtime;
- `PC` y `Mobile` usan actores distintos (`cursor` vs `ripple/touch`);
- el sistema evita video, GIF, spritesheets y dependencias pesadas;
- el trabajo pendiente ya es de QA visual fino, captura de evidencias y ultimos ajustes de percepcion.

## Objetivo

Convertir el onboarding actual en una secuencia de microanimaciones claras, ligeras y repetibles que expliquen cada accion principal de la app sin usar video, GIF, spritesheets ni media prerenderizada pesada.

La meta no es "decorar" las cards. La meta es que cada card demuestre una accion real de la app con una puesta en escena corta, entendible y consistente entre desktop y mobile.

## Decision oficial

Se adopta de forma oficial la **Opcion A**:

- `PC`: cursor simple de onboarding + anillo de click + reaccion visible del control.
- `Mobile`: ripple de tap + reaccion visible del control.

Quedan fuera de v1:

- GIFs
- videos
- capturas prerenderizadas por card
- spritesheets
- dependencia a `SVG` o `com.unity.vectorgraphics`
- manos ilustradas o gestos fotorealistas

Si mas adelante se desea una variante con assets gestuales, se tratara como un spike separado y no como parte de esta primera implementacion.

## Estado actual del proyecto

Este plan queda aterrizado sobre la implementacion real ya existente:

- `Assets/Scripts/UI/Panels/OnboardingController.cs`
- `Assets/Scripts/UI/Layouts/MainLayout.uxml`
- `Assets/UI/Styles/Theme.uss`
- `Assets/Scripts/Core/Managers/InputManager.cs`
- `Assets/Scripts/Core/Managers/OrbitCameraController.cs`
- `Assets/Scripts/Core/Managers/SelectionManager.cs`
- `Assets/Scripts/Core/Managers/CursorManager.cs`
- `Assets/Scripts/UI/ProceduralIcons/*`

### Lo que ya existe hoy

- Hay un `OnboardingController` con 15 pasos.
- Ya existe un switch `PC/mobile` dentro del overlay.
- Ya hay `cueSequence` por step y un cambio de cue temporizado.
- El onboarding ya puede mostrar glyphs procedurales reutilizables.
- El proyecto ya distingue input de mouse y touch para la camara.
- Existe un sistema de cursor global en runtime para la app.

### Lo que falta hoy

- Las cards aun no tienen una coreografia visual suficientemente descriptiva.
- El cambio actual es sobre todo de texto, cue y algun highlight general.
- Los iconos procedurales estan pensados para `hover/press`, pero el onboarding necesita demos guiadas y autonomas.
- La paridad real de `double tap` para seleccion debe validarse antes de asumirla como feature cerrada en mobile.

### Observaciones tecnicas que deben quedar registradas

- El onboarding actual necesita evolucionar desde `cueSequence` hacia fases visuales reales por step.
- La clase de ocultacion del overlay debe normalizarse:
  - `Theme.uss` define `onboard-overlay--hidden`
  - `OnboardingController.cs` usa `onboard--hidden`
- Esta correccion debe hacerse antes o durante la futura implementacion visual real.

## Principios rectores

- Explicar una accion por card, no varias ideas a la vez.
- La respuesta del sistema debe ser mas visible que el actor de interaccion.
- El loop debe ser corto, limpio y comprensible en una sola mirada.
- La animacion debe apoyar el copy, no competir con el.
- Todo debe ser ligero en memoria y razonable para WebGL.
- Si una accion puede explicarse con UI, no se debe introducir 3D adicional sin necesidad.

## Restricciones duras de rendimiento

- No animar layout si puede resolverse con `translate`, `scale`, `opacity` o `color`.
- No mover mas de `3` o `4` elementos a la vez dentro de una card.
- No introducir texturas grandes ni librerias nuevas en v1.
- Reutilizar al maximo `VisualElement`, glyphs procedurales y estilos ya presentes.
- No usar simulaciones complejas para thermal, power o transiciones de escena.
- Mantener cada loop entre `1.8 s` y `2.6 s`.

## Gramatica visual comun

Todas las cards deben componerse con la misma estructura temporal.

### Estructura de fase

1. `Anticipation`
   - Duracion: `120-180 ms`
   - El actor se acerca, el target entra en foco o aparece un pre-highlight.

2. `Press / Contact`
   - Duracion: `90-140 ms`
   - Hay contacto explicito: click ring, ripple, punto de contacto o inicio de drag.

3. `Response`
   - Duracion: `220-420 ms`
   - El sistema responde: apertura, glow, select, slide-in, fade, isolate, cambio de estado.

4. `Settle / Hold`
   - Duracion: el resto del loop
   - La escena se estabiliza y deja al usuario leer el resultado.

### Reglas visuales

- El actor nunca debe tapar la respuesta principal.
- Los pulsos deben ser limpios y de alto contraste.
- El color acento del onboarding debe mantenerse coherente con el color actual del overlay.
- El `hold` final debe durar lo suficiente para entender el resultado, no solo el contacto.

## Lenguaje visual por plataforma

## PC

### Actor

- Cursor simple de onboarding.
- No debe ser realista ni depender del cursor del sistema operativo.
- Debe vivir dentro del `preview stage` de la card.

### Contacto

- Anillo de click o pulso radial al contacto.
- El anillo nace en el punto de impacto.
- El control o target se deprime visualmente `2-4 px` equivalentes.

### Reglas

- `Hover` solo aparece cuando agrega contexto real.
- No basar la explicacion en hover salvo donde la UX real lo necesite.
- El cursor debe recorrer distancias cortas y legibles.

## Mobile

### Actor

- Punto de contacto simple, abstracto y funcional.
- No usar mano ilustrada en v1.

### Contacto

- `Tap`: ripple circular limpio.
- `Double tap`: dos pulsos consecutivos con separacion de `140-180 ms`.
- `Drag`: punto de contacto con estela corta o guia de trayectoria.
- `Pinch`: dos puntos de contacto con guias de acercamiento o separacion.

### Reglas

- El gesto debe ser mas simbolico que literal.
- La geometria del gesto debe ser minima para no saturar la stage.
- El gesto nunca debe ocultar el estado final que se quiere ensenar.

## Componentes reutilizables para la futura implementacion

La futura implementacion debe desacoplar datos de onboarding, fases visuales y presentacion.

### Tipos propuestos

```csharp
public enum PlatformVariant
{
    Pc,
    Mobile
}

public enum InteractionActorType
{
    CursorClick,
    CursorDrag,
    CursorWheel,
    TapRipple,
    DoubleTapRipple,
    TouchDrag,
    TouchPinch,
    None
}

public sealed class OnboardingVisualPhase
{
    public string Cue;
    public InteractionActorType Actor;
    public int DurationMs;
    public string TargetId;
    public string ResponseId;
}

public sealed class OnboardingStepAnimationSpec
{
    public string StepId;
    public PlatformVariant Platform;
    public IReadOnlyList<OnboardingVisualPhase> Phases;
    public int LoopDurationMs;
}
```

### Responsabilidades

- `OnboardingController`
  - Navega steps.
  - Cambia plataforma.
  - Dispara el loop correcto.

- `OnboardingAnimationDriver`
  - Ejecuta fases visuales.
  - Coordina actores, targets y respuestas.
  - No depende del comportamiento `hover/press` natural de los glyphs.

- `OnboardingPreviewStage`
  - Contiene el mini escenario de cada card.
  - Reutiliza subcomponentes visuales comunes.

### Subcomponentes reutilizables

- `InteractionActorView`
  - Cursor, ripple, pinch points, drag trail.

- `TargetControlView`
  - Boton, tab, menu item, slider thumb, part chip, hotspot marker.

- `SystemResponseView`
  - Glow, outline, fade, panel peek, panel open, submenu open, isolate state.

- `CueLabelView`
  - Texto corto sincronizado con la fase.

## Regla de consistencia

El onboarding no debe depender solo de glyphs dibujados para comunicar el paso. Cada card debe tener:

- actor de interaccion
- target reconocible
- respuesta del sistema
- cue corto

Si falta una de estas cuatro piezas, la card queda subexplicada.

## Storyboard por card

Cada card debe documentarse asi:

- `Actor`
- `Target`
- `Respuesta`
- `Fases`
- `Loop`
- `Notas`

## 1. Navigate

- `Actor PC`: cursor con RMB drag, wheel y MMB drag.
- `Actor Mobile`: one-finger drag, two-finger pinch y two-finger pan.
- `Target`: stage de navegacion / modelo simplificado.
- `Respuesta`: orbit, zoom y pan visibles sobre el modelo o su representacion.
- `Fases`:
  - F1 `ORBIT`
  - F2 `ZOOM`
  - F3 `PAN`
- `Loop`: `2.4 s`
- `Notas`:
  - El reset view solo aparece como icono o microcue secundario.
  - No intentar mostrar demasiada precision fisica; la prioridad es claridad conceptual.

## 2. Select / Deselect

- `Actor PC`: cursor click sobre pieza, segundo click sobre subpieza, click fuera.
- `Actor Mobile`: tap sobre pieza, segundo tap sobre subpieza, tap fuera.
- `Target`: cluster de piezas jerarquicas.
- `Respuesta`: outline del parent, luego foco de subpieza, luego retroceso un nivel.
- `Fases`:
  - F1 `SELECT PARENT`
  - F2 `SELECT SUBPIECE`
  - F3 `BACK ONE LEVEL`
- `Loop`: `2.5 s`
- `Notas`:
  - El cambio jerarquico debe verse en la escena, no solo en el copy.
  - La pieza activa debe ser evidente por contraste y contexto atenuado.

## 3. Part Info

- `Actor`: click o tap sobre pieza y luego sobre tab/panel.
- `Target`: pieza seleccionada + tab `PART INFO`.
- `Respuesta`: seleccion visible, tab destacado, panel en `peek`, luego panel abierto.
- `Fases`:
  - F1 `SELECT PART`
  - F2 `FOCUS PART INFO`
  - F3 `OPEN PANEL`
- `Loop`: `2.3 s`
- `Notas`:
  - La apertura debe mostrar `peek` antes del `slide-in` completo.

## 4. Inspect

- `Actor`: click o tap sobre el menu `INSPECT`.
- `Target`: boton de modo principal.
- `Respuesta`: apertura progresiva de `PINS`, `ISOLATE`, `POWER`.
- `Fases`:
  - F1 `PRESS INSPECT`
  - F2 `OPEN SUBMENU`
  - F3 `HOLD ACTIVE`
- `Loop`: `2.0 s`
- `Notas`:
  - El menu activo debe permanecer resaltado durante el hold.

## 5. Pins

- `Actor`: click o tap sobre `PINS`.
- `Target`: control de pins y hotspot group.
- `Respuesta`: pins visibles, luego hotspot group resaltado.
- `Fases`:
  - F1 `PINS ON`
  - F2 `HOTSPOT GROUP`
- `Loop`: `2.0 s`
- `Notas`:
  - Usar brillo suave, no destello agresivo.

## 6. Isolate

- `Actor`: click o tap sobre `ISOLATE`, y segundo contacto como retorno.
- `Target`: pieza o grupo seleccionado.
- `Respuesta`: fade del resto, foco del grupo activo, luego restauracion parcial.
- `Fases`:
  - F1 `ISOLATE`
  - F2 `FOCUSED VIEW`
  - F3 `BACK ONE LEVEL`
- `Loop`: `2.4 s`
- `Notas`:
  - El retorno debe sentirse como deshacer parcial, no como reset total.
  - El copy debe anotar que la paridad final de `double tap` para mobile queda como validacion tecnica pendiente.

## 7. Power

- `Actor`: click o tap sobre `POWER`.
- `Target`: power button + estado del dron.
- `Respuesta`: OFF -> ON, luego `IDLE/FLYING`, luego thermal run si aplica.
- `Fases`:
  - F1 `POWER ON`
  - F2 `STATE ACTIVE`
  - F3 `THERMAL RUN`
- `Loop`: `2.5 s`
- `Notas`:
  - No usar animacion pesada de calor.
  - Resolver con gradiente procedural, pulso o desplazamiento de color.

## 8. Analyze

- `Actor`: click o tap sobre `ANALYZE`.
- `Target`: boton de modo principal.
- `Respuesta`: apertura de `CUT`, `EXPLODE`, `FILTER`.
- `Fases`:
  - F1 `PRESS ANALYZE`
  - F2 `OPEN SUBMENU`
  - F3 `HOLD ACTIVE`
- `Loop`: `2.0 s`

## 9. Cut

- `Actor`: click o tap sobre controles de eje y angulo.
- `Target`: plano(s) de corte y controles asociados.
- `Respuesta`: un plano, luego dos planos, luego plano inclinado.
- `Fases`:
  - F1 `ONE PLANE`
  - F2 `TWO PLANES`
  - F3 `INCLINED PLANE`
- `Loop`: `2.6 s`
- `Notas`:
  - Mantener geometria limpia, abstracta y facil de leer.

## 10. Explode

- `Actor`: arrastre corto o interaccion sobre slider.
- `Target`: slider y ensamblaje.
- `Respuesta`: separacion progresiva del ensamblaje y hold del estado expandido.
- `Fases`:
  - F1 `EXPLODE OFF`
  - F2 `SLIDER MOVE`
  - F3 `EXPLODE ON`
- `Loop`: `2.2 s`
- `Notas`:
  - El desplazamiento debe conservar legibilidad jerarquica.

## 11. Filter

- `Actor`: click o tap sobre chip o boton de categoria.
- `Target`: controles de filtro + categoria elegida.
- `Respuesta`: filtros apagados, luego categoria activa, luego aislamiento visual.
- `Fases`:
  - F1 `FILTERS OFF`
  - F2 `SELECT FILTER`
  - F3 `ISOLATE CATEGORY`
- `Loop`: `2.3 s`
- `Notas`:
  - La categoria activa debe quedar inequivocamente destacada.

## 12. Studio

- `Actor`: click o tap sobre `STUDIO`.
- `Target`: boton de modo principal.
- `Respuesta`: apertura de `RENDER MODE`, `ENVIRONMENT`, `LIGHTING`.
- `Fases`:
  - F1 `PRESS STUDIO`
  - F2 `OPEN SUBMENU`
  - F3 `HOLD ACTIVE`
- `Loop`: `2.0 s`

## 13. Render Mode

- `Actor`: click o tap sobre selector de modo.
- `Target`: chips o botones de render.
- `Respuesta`: `REALISTIC`, luego `X-RAY/SOLID`, luego `THERMAL`.
- `Fases`:
  - F1 `REALISTIC`
  - F2 `X-RAY`
  - F3 `THERMAL`
- `Loop`: `2.4 s`
- `Notas`:
  - La dependencia con `POWER` debe mostrarse con una senal breve, no con texto largo.

## 14. Environment

- `Actor`: click o tap sobre presets.
- `Target`: bloques de `STUDIO`, `TIME` y `COLOR`.
- `Respuesta`: cambio de ambiente en tres grupos cortos y ordenados.
- `Fases`:
  - F1 `STUDIO LOOK`
  - F2 `DAY / NIGHT / SUNSET`
  - F3 `COLOR PRESETS`
- `Loop`: `2.6 s`
- `Notas`:
  - No saturar la preview con demasiadas variaciones a la vez.

## 15. Lighting

- `Actor`: drag corto sobre sliders.
- `Target`: `LIGHT ROTATION`, `OBJECT INTENSITY`, `BACKGROUND TONE`.
- `Respuesta`: cambio sutil pero legible sobre el area afectada.
- `Fases`:
  - F1 `ROTATION`
  - F2 `INTENSITY`
  - F3 `BACKGROUND`
- `Loop`: `2.4 s`
- `Notas`:
  - El usuario debe poder asociar cada slider con su efecto.

## Firma visual por familia

Para evitar que todas las cards se sientan iguales, cada familia debe compartir una firma:

- `Navigation`
  - mas desplazamiento del actor
  - mas trail o recorrido

- `Selection / Info`
  - mas outline, focus y jerarquia

- `Mode menus`
  - mas apertura y persistencia de estado activo

- `Action tools`
  - mas cambio de estado visible del sistema

- `Studio`
  - mas transiciones de ambiente, material y luz

## Reglas de reutilizacion

- Reutilizar el `preview stage` ya existente.
- Reutilizar glyphs procedurales cuando ayuden, pero no depender solo de ellos.
- Los actores de interaccion deben ser componentes genericos compartidos entre cards.
- Los targets deben ser construidos como bloques visuales simples, no mini pantallas distintas para cada paso.

## Regla sobre SVG y gestos ilustrados

En v1 no se incorporan `SVG` ni paquetes de vector graphics.

Motivos:

- se evita agregar dependencia nueva al proyecto
- se reduce el riesgo de compatibilidad o tuning extra para WebGL
- la representacion abstracta actual ya es suficiente para explicar la interaccion

Si en una fase posterior se desea explorar assets gestuales:

- hacerlo en un spike separado
- medir build, memoria y claridad visual
- no bloquear la implementacion principal del onboarding por ese experimento

## Fases de implementacion sugeridas

### Fase 1. Infraestructura

- Separar step data de demo visual.
- Crear `OnboardingAnimationDriver`.
- Normalizar la clase de ocultacion del overlay.

### Fase 2. Lenguaje de interaccion

- Implementar actor de cursor para PC.
- Implementar ripple, drag point y pinch points para mobile.
- Implementar respuestas base: press, glow, open, fade, outline.

### Fase 3. Cards nucleares

- `Navigate`
- `Select / Deselect`
- `Part Info`
- `Inspect`

Estas cards deben quedar perfectas antes de escalar al resto.

### Fase 4. Menus y herramientas

- `Pins`
- `Isolate`
- `Power`
- `Analyze`
- `Cut`
- `Explode`
- `Filter`
- `Studio`
- `Render Mode`
- `Environment`
- `Lighting`

## Criterios de aceptacion

La solucion se considera correcta si cumple lo siguiente:

- Cada card se entiende sin releer el texto.
- El actor de interaccion es claro pero secundario frente a la respuesta del sistema.
- Desktop y mobile comunican la misma accion con convenciones visuales distintas pero equivalentes.
- No depende de video, GIF ni media pesada.
- No agrega dependencias nuevas al proyecto para v1.
- Mantiene un costo de memoria bajo.
- El loop de cada card se percibe limpio y no confuso.

## Validacion y pruebas

### Desktop

- `click`, `double click`, `hover` y apertura de menus son distinguibles.
- El cursor no distrae ni se siente caricaturesco.
- El target y la respuesta quedan claros en menos de 3 segundos.

### Mobile

- `tap`, `double tap`, `drag` y `pinch` se diferencian entre si.
- La demo no depende de una mano ilustrada.
- Los puntos de contacto y ripple son suficientes para explicar la accion.

### Consistencia con la app real

- El onboarding no debe prometer una interaccion que aun no este validada en el runtime real.
- En particular, la equivalencia de `double tap` para seleccion y aislamiento debe verificarse antes de cerrarla como comportamiento final de mobile.

### Rendimiento

- Sin caidas perceptibles por el overlay.
- Sin incremento de peso por media nueva.
- Sin introducir complejidad innecesaria en WebGL.

## Conclusiones

La direccion correcta para este onboarding no es usar mas assets. La direccion correcta es usar mejor los elementos ya disponibles y estructurarlos como microdemostraciones.

La Opcion A ofrece el mejor equilibrio entre:

- claridad
- estetica
- costo tecnico
- rendimiento
- consistencia con el lenguaje actual de UI Toolkit

La implementacion futura debe apoyarse en un lenguaje visual abstracto y preciso:

- cursor simple para PC
- ripple y puntos de contacto para mobile
- respuesta del sistema como protagonista

Con esto el onboarding puede verse mas claro, mas elegante y mas didactico sin subir el peso del proyecto.
