# Analisis de Problemas de la App

Fecha: 2026-03-10

## Contexto

Se realizo un analisis del codigo para identificar la causa raiz de los siguientes problemas reportados en la app:

- Los submenus de Cut, Filter y Explode se cierran al hacer click afuera.
- La vista explosiva se desactiva al cambiar de menu a Studio.
- El cut no funciona correctamente en Blueprint cuando entra el postprocesado.
- El boton X del info panel no se ve, pero su area sigue cerrando el panel.
- El info panel se esta cerrando por vias no deseadas.
- La camara mezcla paneo con zoom/rotacion cuando no deberia.

Tambien se dejo registrado el estado del repositorio asociado a documentacion e investigacion:

- Commit publicado: `da76889`
- Mensaje: `docs: reorganize Holybro research and add Blender optimization assets`

## Archivos Clave Revisados

- `desarrollo/unity_project/Assets/Scripts/UI/UIManager.cs`
- `desarrollo/unity_project/Assets/Scripts/UI/Panels/UIModeController.cs`
- `desarrollo/unity_project/Assets/Scripts/UI/Panels/AnalyzeModeHandler.cs`
- `desarrollo/unity_project/Assets/Scripts/UI/Panels/UIDetailsSheet.cs`
- `desarrollo/unity_project/Assets/Scripts/UI/Panels/UIEnvironmentPanel.cs`
- `desarrollo/unity_project/Assets/Scripts/UI/Panels/OnboardingController.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Managers/OrbitCameraController.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Managers/SelectionManager.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Managers/CrossSectionManager.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Managers/ViewModeManager.cs`
- `desarrollo/unity_project/Assets/Scripts/Core/Content/ExplodedViewManager.cs`
- `desarrollo/unity_project/Assets/Shaders/Blueprint.shader`
- `desarrollo/unity_project/Assets/UI/Styles/Theme.uss`
- `desarrollo/unity_project/Assets/Scripts/UI/Layouts/MainLayout.uxml`

## Diagnostico

### 1. Submenus que se cierran al hacer click afuera

#### Sintoma

Los submenus de Analyze como Cut, Filter y Explode se colapsan cuando el usuario hace click fuera del submenu.

#### Causa raiz

El cierre no viene de una logica propia del submenu sino del flujo de seleccion global:

1. El root UI esta configurado con `picking-mode="Ignore"` en `MainLayout.uxml`, por lo que el click fuera del submenu cae al mundo 3D.
2. `SelectionManager` interpreta ese click como click en background y publica `PartSelectedEvent(null)`.
3. `UIManager.OnPartSelected(...)` ejecuta `_modeController.CloseAllMenus()` para cualquier seleccion que no venga de hotspot.
4. `UIModeController.CloseAllMenus()` llama a `GetHandler(_activeMode)?.Deactivate()` y eso colapsa el estado visual del modo activo.

#### Conclusión

Los submenus no se estan cerrando por un "click afuera" intencional de UI, sino por un efecto colateral del sistema de seleccion 3D.

### 2. Explode se apaga al cambiar a Studio

#### Sintoma

La vista explosiva desaparece al pasar de Analyze a Studio.

#### Causa raiz

Hay dos mecanismos que la apagan:

1. `ExplodedViewManager.OnStateChanged(...)` fuerza `SetExplosionFactor(0f)` en cualquier estado distinto de `AppState.ExplodedView`.
2. `UIModeController.SyncWithAppState(...)` hace `_analyze.SetExplodeState(false)` cuando el nuevo estado no es `ExplodedView`.

En otras palabras, Explode esta modelado como un estado exclusivo del `AppStateMachine` en vez de un overlay persistente que pueda coexistir con Studio.

#### Conclusión

El problema no es visual sino de arquitectura de estado: Explode depende de un estado global mutuamente excluyente con Studio.

### 3. Cut no funciona bien en Blueprint con postprocesado

#### Sintoma

El corte parece actuar sobre el mesh base, pero el resultado visual Blueprint con edge detection no respeta correctamente el cut final.

#### Causa raiz

El shader `Blueprint.shader` si implementa clipping en sus passes principales, usando `_GlobalClipPlane` y `_GlobalClipEnabled`.

El problema aparece en los passes auxiliares usados por el postproceso:

- `DepthOnly`
- `DepthNormals`

Esos passes no estan haciendo el mismo clipping. La consecuencia es:

1. La geometria visible principal puede quedar recortada.
2. Pero el postproceso de edge detection sigue leyendo profundidad y normales del mesh sin recorte.
3. El contorno postprocesado recompone visualmente el objeto como si no estuviera cortado.

#### Conclusión

La incoherencia esta entre el pass principal de Blueprint y sus passes auxiliares de depth/normals que alimentan el edge detection.

### 4. La X del info panel desaparecio, pero sigue cerrando

#### Sintoma

La X no se ve, pero si se hace click donde deberia estar, el panel se cierra.

#### Causa raiz

El boton sigue existiendo en el arbol UI como `SheetCloseBtn`, por eso el hitbox funciona. El problema es visual.

La implementacion actual depende de texto unicode (`✕`) dentro del boton, no de un icono dedicado o una composicion visual controlada.

Eso vuelve fragil su render si cambia alguno de estos factores:

- fuente efectiva aplicada al boton
- color heredado o contraste
- tamaño o centrado del texto
- reglas globales de estilo para botones o labels

#### Conclusión

La X no desaparecio funcionalmente; desaparecio visualmente por depender de un glyph de texto en vez de un icono estable.

### 5. El info panel se esta cerrando por vias no deseadas

#### Comportamiento deseado

El info panel solo debe cerrarse si:

- se hace click en la X
- se selecciona otro item del bottom menu bar
- se hace doble click en el background

#### Causas actuales

Hoy existen mas vias de cierre:

1. `UIDetailsSheet` registra click en `.sheet-header` para hacer toggle abierto/cerrado.
2. `UIManager._modeController.OnAnyModeActivated` cierra el sheet al activar un modo.
3. `OnPartDoubleClicked(...)` ya cierra correctamente en background, pero convive con otras reglas que sobran.

#### Conclusión

La politica de cierre del panel no esta centralizada. El panel se puede cerrar por toggles de conveniencia heredados, no por reglas UX estrictas.

### 6. Paneo de camara mezcla translacion con zoom o rotacion

#### Sintoma

Al mover el modelo verticalmente, la camara se acerca o se aleja cuando solo deberia panear.

#### Causas raiz detectadas

##### 6.1 Desalineacion entre onboarding y controles reales

`OnboardingController` dice:

`Right-click drag to pan.`

Pero `OrbitCameraController` implementa:

- click derecho: orbit
- click medio: pan

Si el usuario sigue la instruccion del onboarding, hace orbit con repivot y no paneo.

##### 6.2 En touch, pan y pinch se procesan simultaneamente

Con dos dedos, `OrbitCameraController` calcula en el mismo flujo:

- desplazamiento del centro para pan
- cambio de distancia entre dedos para zoom

Eso significa que un gesto vertical con dos dedos puede introducir ruido de pinch y disparar zoom aunque la intencion haya sido solo panear.

##### 6.3 El click derecho repivota antes de orbitar

En mouse, `PickPivot()` se llama al iniciar el click derecho. Si el usuario esperaba panear, el sistema puede recentrar el pivot y producir una sensacion adicional de acercamiento/alejamiento al orbitar.

#### Conclusión

El problema mezcla UX y logica de gesto: los controles comunicados no coinciden con los implementados, y en touch no existe una separacion estricta entre pan y zoom.

## Problemas y Causantes Resumidos

### Submenus

- Problema: se cierran al click afuera.
- Causante: `PartSelectedEvent(null)` de background dispara `CloseAllMenus()` desde `UIManager`.

### Explode

- Problema: se desactiva al cambiar a Studio.
- Causante: arquitectura de estado exclusiva basada en `AppState.ExplodedView`.

### Cut + Blueprint

- Problema: el corte no se refleja correctamente en el resultado final con postproceso.
- Causante: passes `DepthOnly` y `DepthNormals` de Blueprint no aplican el mismo clipping que el pass principal.

### X del info panel

- Problema: no se ve, pero sigue clickeable.
- Causante: implementacion visual fragil basada en texto unicode.

### Cierre del info panel

- Problema: se cierra por mas vias de las permitidas.
- Causante: hay toggles de cierre distribuidos entre `UIDetailsSheet` y `UIManager`.

### Camara

- Problema: el paneo mezcla traslacion con zoom/rotacion.
- Causante: mismatch entre onboarding y controles reales, mas mezcla simultanea de pan+pinch en touch.

## Plan de Solucion Paso a Paso

### Fase 1. Corregir cierre de submenus sin tocar todavia la logica de render

1. Revisar `UIManager.OnPartSelected(...)`.
2. Eliminar el cierre automatico global de menus cuando llega `PartSelectedEvent(null)` o una seleccion normal.
3. Mantener el cierre de subpaneles solo para eventos explicitos de navegacion del modo.
4. Verificar que `Cut`, `Filter` y `Explode` permanezcan abiertos al:
   - click en background
   - seleccionar una pieza
   - deselect

### Fase 2. Desacoplar Explode del AppState exclusivo

1. Revisar `ExplodedViewManager.OnStateChanged(...)`.
2. Eliminar la regla que forza `SetExplosionFactor(0f)` al salir de `AppState.ExplodedView`.
3. Revisar `UIModeController.SyncWithAppState(...)`.
4. Separar el estado visual de Explode del estado global del modo.
5. Hacer que Explode persista al cambiar entre Analyze y Studio.
6. Mantener el reset solo cuando el usuario cierre explicitamente explode o haga reset view.

### Fase 3. Arreglar Blueprint + Cut

1. Editar `Blueprint.shader`.
2. Aplicar clipping tambien en los passes `DepthOnly` y `DepthNormals`.
3. Verificar que el edge detection reciba profundidad y normales ya recortadas.
4. Validar en runtime:
   - corte en Realistic
   - corte en Blueprint
   - Blueprint con edge detection activado

### Fase 4. Corregir politica de cierre del info panel

1. Eliminar el toggle por click en `.sheet-header` dentro de `UIDetailsSheet`.
2. Centralizar las reglas de cierre permitidas:
   - click en la X
   - doble click en background
   - activacion de otro item del bottom bar
3. Confirmar que seleccionar otra pieza no cierre el panel innecesariamente.
4. Confirmar que click simple en background no lo cierre.

### Fase 5. Restaurar visualmente la X

1. Revisar `MainLayout.uxml` y `Theme.uss`.
2. Reemplazar la X basada en glyph por una solucion mas robusta:
   - icono procedimental, o
   - label interno controlado por estilo, o
   - fondo/vector dedicado
3. Ajustar contraste para fondos oscuros y claros.
4. Verificar visibilidad real en `ui-light-bg` y fondo oscuro.

### Fase 6. Corregir input de camara

1. Decidir el esquema final de controles de mouse:
   - opcion A: mantener click derecho = orbit y click medio = pan, actualizando onboarding
   - opcion B: mover pan al click derecho si eso es lo esperado por UX
2. En touch, separar gesto de pan del gesto de pinch:
   - si cambia poco la distancia entre dedos, solo pan
   - si cambia por encima de un umbral, solo zoom
3. Evitar que pan vertical induzca zoom por ruido de pinch.
4. Revisar si `PickPivot()` debe ejecutarse siempre en right-click o solo en orbit deliberado.
5. Actualizar onboarding para que refleje el control real.

### Fase 7. Validacion final

Ejecutar una matriz de pruebas manual:

1. Abrir Cut y hacer click en background.
2. Abrir Filter y seleccionar una pieza.
3. Activar Explode y cambiar a Studio.
4. Activar Blueprint y mover el plano de corte.
5. Abrir info panel y probar:
   - click en header
   - click en X
   - click simple en background
   - doble click en background
   - cambio de item del bottom bar
6. Probar paneo vertical con mouse.
7. Probar paneo con touch de dos dedos.

## Recomendacion de Orden de Implementacion

Para minimizar regresiones, el orden recomendado es:

1. Submenus
2. Info panel
3. Persistencia de Explode
4. Blueprint + Cut
5. Camara + onboarding
6. Pruebas integradas

## Notas de Git

Se dejaron fuera del commit actual algunos archivos locales no deseados:

- `.cursor/`
- `blender_files/drone_00(step).blend@`
- `blender_files/drone_00(stp).blend@`
- `blender_files/drone_00.blend2`

Estos no hacen parte del analisis ni del material listo para publicarse.
