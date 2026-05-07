---
tipo: modulo_estudio
fuente: Informe_final/chapters/04_desarrollo.tex
estado: activo
area: desarrollo
tags:
  - tesis
  - unity
  - arquitectura
  - tooling
  - runtime
---

# INF EST 35 Tooling, Arquitectura Runtime y Verificacion

## Idea central

La app no es solo una escena armada manualmente. Tiene una arquitectura runtime y herramientas de editor que ayudan a garantizar que escena, datos, piezas, UI y visualizacion permanezcan consistentes.

## Arquitectura runtime

Capas principales:

- UI Toolkit y layout UXML;
- `UIManager`;
- `AppStateMachine`;
- controladores de modos;
- managers de escena;
- `ImportedDroneRuntimeBinder`;
- `DroneStateController`;
- gestores termicos y visuales.

## ImportedDroneRuntimeBinder

Este componente es clave. Su funcion es reparar y vincular en runtime el modelo importado:

- normalizar jerarquia;
- conectar anchors;
- asociar piezas con datos;
- asegurar colliders/renderers;
- permitir seleccion y resaltado.

## Tooling de editor

Herramientas defendibles:

- `ProjectSetupWizard`;
- `ImportedDroneCoverageAudit`;
- `ThermalContactGraphBuilderWindow`;
- fixers de configuracion WebGL.

Estas herramientas no aparecen en la UI final, pero sostienen el desarrollo real.

## Controladores UI que deben mencionarse en defensa

### UIDetailsSheet

`UIDetailsSheet` administra la ficha inferior o info panel. Su responsabilidad no es solo abrir y cerrar una tarjeta: sincroniza seleccion, datos descriptivos, barra de previsualizacion, foco de camara y contenido contextual.

Cuando llega una seleccion, el flujo esperado es:

```text
SelectionManager -> PartSelectedEvent -> UIManager -> UIDetailsSheet -> ficha inferior
```

La ficha puede representar niveles distintos:

- pieza madre;
- subpieza;
- grupo de hotspot;
- fastener individual.

Esto explica por que seleccion, camara, highlight e isolate deben operar con la misma lectura semantica. Si no comparten esa lectura, la UI podria mostrar una pieza mientras la escena aisla otra.

### OnboardingController y OnboardingAnimationView

`OnboardingController` decide cuando mostrar la ayuda inicial, que tarjeta esta activa, que texto se presenta y si la demostracion debe usar lenguaje de desktop o mobile.

`OnboardingAnimationView` dibuja la demo con `Painter2D`. En vez de reproducir un video, genera una escena procedural con actor, objetivo y respuesta del sistema.

Este patron tiene tres beneficios tecnicos:

- reduce peso de la build;
- evita assets prerenderizados desactualizados;
- mantiene la ayuda alineada con la UI real.

## Explicaciones complejas

### Manager

Un manager centraliza una responsabilidad: UI, modo visual, seleccion, estado termico, escena o datos. Evita que cada objeto controle todo de forma aislada.

### Runtime binding

Binding significa conectar datos y objetos. En este proyecto, el binding permite que una pieza de la escena sepa que datos, nombre, categoria y comportamiento le corresponden.

Flujo simplificado:

```text
anchor_escena -> id_canonico -> DronePartData -> UI/seleccion/shader
```

Lectura:

- `anchor_escena`: objeto o nodo que existe en Unity.
- `id_canonico`: identificador que permite relacionarlo con el catalogo.
- `DronePartData`: datos descriptivos y funcionales de la pieza.
- `UI/seleccion/shader`: sistemas que consumen esa relacion.

Ejemplo:

Si el usuario selecciona un brazo del frame, el collider detecta la interaccion, el binder permite encontrar su dato canonico y la UI muestra la ficha correcta. Sin binding, el modelo seria visual, pero no semanticamente navegable.

### Jerarquia de seleccion

La seleccion no es plana. El sistema puede leer una interaccion como pieza madre, subpieza, grupo de hotspot o fastener. Esa jerarquia incide en cuatro sistemas:

- ficha inferior: decide que datos mostrar;
- camara: decide que encuadrar;
- highlight: decide que resaltar;
- aislamiento: decide que ocultar y que conservar.

Ejemplo:

Un clic desde geometria directa puede seleccionar una pieza madre. Un clic posterior puede descender a una subpieza. Un hotspot puede abrir una lectura grupal. Un fastener puede promoverse a su root para evitar seleccionar solo una submalla temporal.

### Auditoria de cobertura

Verifica si las piezas esperadas tienen datos, anchors, renderers, colliders y relaciones correctas. Es una forma de evitar que la documentacion diga que algo existe pero la escena no lo tenga funcional.

Preguntas que responde:

- La pieza canonica existe en datos?
- Tiene anchor en escena?
- Tiene renderer visible?
- Tiene collider para seleccion?
- Tiene categoria y nombre coherente?
- Esta expuesta en UI final, oculta, legacy o futura?

Modelo de cobertura:

```text
cobertura = elementos_validos / elementos_esperados
```

Ejemplo:

Si hay 28 piezas canonicas y 28 tienen datos, pero solo 26 tienen collider correcto, la cobertura de datos no es igual a la cobertura de interaccion. Esa distincion evita conclusiones falsas.

### Build WebGL

La build web empaqueta codigo, datos, assets y configuracion para ejecutarse en navegador. Debe medirse porque no basta con que funcione dentro del editor.

Lo que cambia frente al editor:

- memoria disponible y fragmentacion del navegador;
- compresion y descarga de assets;
- rendimiento real de GPU/CPU del dispositivo;
- limitaciones de WebAssembly y WebGL;
- comportamiento tactil y resolucion efectiva.

Por eso la evidencia final debe venir de build congelada, no solo de Play Mode.

### Separacion runtime, tooling y legado

La documentacion tecnica debe separar cuatro estados:

- expuesto en UI final: lo que el usuario puede usar en la build;
- implementado pero oculto: existe en codigo, pero no esta en el flujo final;
- codigo legado/no integrado: permanece en repo, pero no sostiene la experiencia final;
- trabajo futuro: idea o extension aun no implementada.

En defensa:

> Esta separacion protege la tesis de sobreprometer. El informe vende evidencia reproducible, no inventario de aspiraciones.

## Terminos importantes

- Runtime: ejecucion de la app.
- Editor tooling: herramientas usadas dentro del editor para preparar o auditar.
- Manager: componente coordinador.
- Binder: componente que conecta datos con objetos.
- Anchor: nodo de escena usado como punto de referencia.
- Collider: volumen usado para seleccion/interaccion.
- Renderer: componente que dibuja geometria.
- Coverage audit: auditoria de cobertura.
- Build: version compilada.
- WebGL template: plantilla de salida web.
- UIDetailsSheet: controlador de ficha inferior e informacion contextual.
- OnboardingController: controlador de ayuda inicial.
- OnboardingAnimationView: vista procedural de demos de onboarding.
- PartSelectedEvent: evento que comunica una seleccion de pieza.
- Peek bar: barra resumida que aparece cuando la ficha esta cerrada.

## Preguntas dificiles de defensa

### Por que hablar de herramientas que no usa el usuario?

Porque hacen parte del desarrollo. Una tesis de Ingenieria Multimedia debe documentar no solo la interfaz final, sino el pipeline que hizo viable el producto.

### Que evita el runtime binder?

Evita depender de una escena perfecta armada a mano. Si la importacion cambia, el binder ayuda a recuperar consistencia funcional.

### Como se sabe que la app y la documentacion coinciden?

Mediante auditorias de cobertura, matriz de desconexiones, validacion funcional y catalogos canonicos.

### Por que el info panel es arquitectura y no solo UI?

Porque consume datos, eventos, seleccion, camara y estado visual. Si falla, no solo falla una tarjeta grafica: se rompe la relacion entre geometria, datos y lectura del usuario.

## Fuentes relacionadas

- [[INF_EST_04_Desarrollo_Implementacion]]
- [[CATALOGO_SCRIPTS_UNITY]]
- [[CATALOGO_SCRIPTS_UNITY_COMPLETO]]
- [[MATRIZ_TRAZABILIDAD_ENTREGABLES_COMPLETA]]
