---
tipo: "documento"
estado: "activo"
descripcion: "Guion ejecutivo para la reunion con el asesor, orientado a nivelar estado real de la tesis y acordar ruta de cierre en 20 minutos."
---

# ROADMAP REUNION ASESOR

## Objetivo real de la reunion

Esta reunion no debe usarse para contar todo el historial del proyecto. Debe servir para que el asesor entienda, en muy poco tiempo, que la tesis ya paso de propuesta a prototipo funcional integrado, que ya existe una base documental seria, y que ahora lo importante es acordar la ruta mas eficiente para entregar y sustentar, idealmente el proximo mes.

## Mensaje central que debe quedar claro

El proyecto ya no esta en fase conceptual. Hoy existe un visor tecnico WebGL funcional en Unity para el Holybro X500 V2, con seleccion jerarquica, panel contextual, menus de inspeccion y analisis, modos visuales, sistema termico hibrido, manejo modular de fasteners y onboarding procedural para desktop y mobile. Lo que falta ya no es "inventar el sistema", sino congelarlo, validarlo, cerrar resultados y empaquetar entrega y sustentacion.

## Que debe entender el asesor al salir de la reunion

- La tesis avanzo mucho mas alla de la propuesta inicial.
- La app ya existe como prototipo funcional con varios subsistemas cerrados.
- El principal reto ya no es construir features base, sino cerrar geometria, QA, evidencias y redaccion final.
- Hay una ruta realista para entregar el proximo mes, pero requiere priorizar cierre y evitar nuevas expansiones de alcance.
- Se necesitan decisiones concretas del asesor sobre validacion, alcance final y criterio de cierre.

## Lo que cambio desde la propuesta

En la propuesta el foco estaba en la idea general: un sistema WebGL para visualizacion tecnica interactiva de un dron. Hoy el proyecto ya tiene decisiones de ingenieria y UX bastante aterrizadas:

- ya no se habla de un visor generico, sino de una plataforma concreta: `Holybro X500 V2`;
- ya no se habla solo de navegacion 3D, sino de un flujo completo de uso con seleccion, bottom sheet, menus, analisis y studio;
- ya no se habla solo de render tecnico, sino de una combinacion real de runtime, datos, UI, shaders y tooling;
- ya no se habla solo de modelo CAD, sino del problema real de convertir geometria compleja a una experiencia viable en WebGL;
- ya no se habla solo de piezas, sino tambien de fasteners con metadata por instancia e inspeccion bajo demanda;
- ya no se habla solo de interfaz, sino de onboarding procedural que ensena el uso real del sistema sin videos ni assets pesados.

## Aclaracion clave para la reunion: que significa runtime y tooling

Estas dos palabras pueden sonar muy tecnicas si no se explican bien. Conviene traducirlas asi:

- `Runtime` significa todo lo que pasa mientras la aplicacion esta corriendo y el usuario la esta usando.
- `Tooling` significa las herramientas auxiliares que se construyeron para preparar, verificar, mantener y depurar el proyecto, aunque el usuario final nunca las vea.

La forma mas simple de decirselo al asesor es esta:

- `Runtime` = el comportamiento real de la app cuando uno abre la build y empieza a interactuar.
- `Tooling` = las herramientas de trabajo que hicieron posible ordenar el modelo, validar la escena y sostener la calidad tecnica del sistema.

Esto es importante porque la tesis no solo produjo una interfaz bonita o un modelo 3D visible. Tambien produjo infraestructura tecnica para que ese sistema funcione de forma consistente y pueda cerrarse con menos riesgo.

## Estado actual del proyecto

## 1. App y prototipo funcional

Hoy la app ya permite:

- explorar el dron en 3D;
- seleccionar piezas y subpiezas;
- abrir un panel contextual de detalle;
- usar `Inspect`, `Analyze` y `Studio`;
- activar `Pins`, `Isolate`, `Power`, `Cut`, `Explode` y `Filter`;
- cambiar entre modos visuales como `Realistic`, `X-Ray`, `Solid` y `Thermal`;
- explicar el flujo de uso con un onboarding animado dentro de la propia UI.

Esto significa que el usuario ya puede recorrer el flujo principal completo de la propuesta dentro de una build funcional.

## 2. Subsistemas tecnicos ya desarrollados

### Onboarding (Sistema Procedural)

- Existe un onboarding MVP procedural implementado con la API **`Painter2D`** de Unity UI Toolkit.
- **¿Qué es Painter2D?** Es una interfaz de dibujo vectorial en tiempo real (similar al Canvas de HTML5) que permite dibujar líneas, curvas y primitivas geométricas directamente en la UI.
- **¿Cómo se animó?** En lugar de importar videos pesados (.mp4) o GIFs que colapsan la memoria en WebGL, las animaciones se programaron lógicamente matemáticamente 100% en **C# (C-Sharp)**. El código interpola posiciones a lo largo del tiempo para dibujar cursores de ratón, gestos de swipe, y clics directamente sobre la pantalla frame a frame.
- **Ventaja técnica:** Esto pesa cero kilobytes en disco, tiene resolución infinita (no se pixela) y permite separar las cards en variantes lógicas para `PC` y `Mobile`.
- Cubre `15` cards (pantallas de tutorial) explicando controles reales con loops iterativos (runtime).
- Ya esta integrado a la UI real y listo para QA visual fino.

### Fasteners

- Existe un contrato de datos para familias e instancias de fasteners.
- Ya hay metadata por tornillo, catalogos reconciliados y binding runtime.
- El sistema usa proxies ligeros en reposo y detalle bajo demanda al seleccionar.
- La seleccion, isolate, camera y details sheet ya fueron ajustados para que la inspeccion de fasteners funcione de manera coherente.

### Sistema termico

- Existe un solver termico reducido y un modo `Thermal` conectado al estado operativo.
- Ya hay grafo termico, shader, leyenda y comportamiento visual integrado.
- El sistema no se presenta como FEA, sino como visualizacion tecnica hibrida para inspeccion.

### Runtime y tooling

- Hay saneamiento de jerarquia importada.
- Hay auditoria de cobertura de escena.
- Hay pipeline de setup para escena importada.
- Hay trazabilidad entre datos, runtime, UI y documentacion.

#### Como explicar esta seccion al asesor

La idea no es decir simplemente "tambien hice tooling". Lo correcto es explicarle que, para que el prototipo funcione como producto tecnico y no como una demo fragil, fue necesario construir una capa de soporte que organiza la escena, verifica consistencia y automatiza tareas repetitivas.

#### 1. Saneamiento de jerarquia importada

Que significa:

Cuando un modelo complejo entra a Unity desde CAD o Blender, muchas veces la escena llega desordenada: piezas mal agrupadas, nombres inconsistentes, objetos huerfanos o componentes que no quedan donde la app espera encontrarlos.

Que se hizo:

Se construyo logica de runtime para reparar esa jerarquia al cargar la escena, reubicar elementos problematicos, reconstruir grupos sinteticos como fasteners y misc, y volver a conectar sistemas que dependen de esa estructura.

Por que importa:

Sin ese saneamiento, la seleccion, los filtros, el isolate, los hotspots, el sistema termico y el details sheet pueden fallar o comportarse de forma incoherente. En otras palabras, esta capa convierte una escena importada en una escena utilizable por la app.

Como decirlo en la reunion:

"No solo importamos un modelo. Tambien construimos una capa que corrige y reorganiza la escena en runtime para que los subsistemas de seleccion, UI y analisis funcionen de manera consistente."

#### 2. Auditoria de cobertura de escena

Que significa:

Es un mecanismo para revisar si todas las piezas que deberian existir en la escena realmente estan presentes, si tienen renderer, collider, anclaje correcto y si la taxonomia usada en datos coincide con lo que hay en Unity.

Que se hizo:

Se desarrollaron chequeos y reportes para identificar piezas faltantes, anclajes rotos, renderers huerfanos, discrepancias de nomenclatura y problemas de cobertura entre catalogo, escena y runtime.

Por que importa:

Esto evita trabajar "a ciegas". Permite saber objetivamente si la escena esta bien montada o si todavia hay errores estructurales antes de culpar a la UI o a la logica de negocio.

Como decirlo en la reunion:

"Construimos auditorias para verificar que la escena importada realmente coincide con el catalogo y con la estructura que necesita la app. Eso nos permite medir cobertura real y no depender solo de revision manual."

#### 3. Pipeline de setup para escena importada

Que significa:

Es el conjunto de pasos que prepara una escena nueva o reimportada para que quede lista para usarse dentro del prototipo.

Que se hizo:

Se automatizaron tareas de preparacion como crear managers necesarios, reconstruir configuracion minima, vincular el dron importado con los sistemas de runtime, refrescar datos y dejar la escena lista para pruebas.

Por que importa:

Sin este pipeline, cada reimport del modelo implicaria mucho trabajo manual y aumentaria el riesgo de errores humanos. Con el setup automatizado, el proyecto se vuelve mas reproducible y mas sostenible.

Como decirlo en la reunion:

"No dependemos de configurar todo manualmente cada vez que cambia el modelo. Ya existe un pipeline de setup que prepara la escena para que la app pueda volver a operar de forma estable."

#### 4. Trazabilidad entre datos, runtime, UI y documentacion

Que significa:

Significa que lo que existe en los archivos de datos, en la escena, en la interfaz y en la documentacion no esta desconectado. Hay una relacion verificable entre esas capas.

Que se hizo:

Se alinearon catalogos, assets, scripts, fichas de UI, documentos tecnicos, bitacora, changelog y notas de Obsidian para que una pieza o subsistema pueda seguirse desde su definicion de datos hasta su comportamiento visible en la app.

Por que importa:

Esto reduce contradicciones entre "lo que el documento dice" y "lo que la build realmente hace". Tambien facilita explicarle al asesor, al jurado o a un evaluador donde esta la evidencia real de cada avance.

Como decirlo en la reunion:

"Ya no estamos trabajando con codigo por un lado y documento por otro. Hoy existe trazabilidad entre datos, escena, comportamiento runtime, interfaz y soporte documental."

#### Resumen corto para decirlo de corrido

Si el asesor pregunta de forma general por `runtime y tooling`, la respuesta sintetica podria ser:

"Con runtime me refiero al comportamiento real de la app cuando esta corriendo: seleccion, UI, modos visuales, fasteners, onboarding y logica de escena. Con tooling me refiero a las herramientas que construimos para preparar, auditar y mantener ese sistema: setup, verificacion de cobertura, saneamiento de escena y trazabilidad tecnica. Es decir, no solo se hizo una app visible, tambien se hizo la infraestructura para que esa app sea mantenible y verificable."

## 3. Informe final y documentos

El trabajo documental tambien esta bastante adelantado:

- el informe final en LaTeX ya tiene base fuerte y compilable;
- el capitulo de desarrollo ya refleja el pipeline real, no solo la idea inicial;
- los resultados aun dependen del cierre de build y de las evidencias finales;
- existen manual tecnico y manual de usuario;
- existe bitacora, changelog y sesion log actualizados;
- existe trazabilidad en Obsidian entre scripts, entregables y evidencia.

En otras palabras, no solo hay software. Ya hay ecosistema de soporte para cerrar un trabajo de grado serio.

## 4. Lo que sigue pendiente

Lo pendiente ya esta bastante claro y es acotable:

- congelar la geometria final optimizada;
- cerrar QA visual y funcional de la build;
- capturar evidencia definitiva de uso, performance y UI;
- diligenciar resultados con datos reales y no con placeholders;
- cerrar conclusiones, version final de informe y materiales de sustentacion.

## Cuello de botella real actual

El cuello de botella mas importante ya no es de UX o de arquitectura base. Es de cierre:

- optimizacion final del modelo y fasteners;
- congelamiento tecnico de la build;
- validacion final y evidencia;
- definicion exacta del alcance de resultados exigidos para entregar.

Este es el punto que conviene explicarle al asesor con total honestidad.

## Guion recomendado para una reunion de 20 minutos

## 0 a 2 minutos - Apertura

- Mensaje: "Quiero actualizarlo porque el proyecto ya avanzo mucho mas alla de la propuesta. Hoy ya existe un prototipo funcional y la idea de esta reunion es mostrarle el estado real y acordar la ruta mas eficiente para entregar y sustentar."
- Resultado buscado: poner la reunion en modo de alineacion y cierre, no de exploracion abierta.

## 2 a 6 minutos - Que se construyo realmente

- Mensaje: mostrar que ya existe un sistema integrado y no un conjunto suelto de pruebas.
- Mostrar: flujo principal de la app.
- Enfatizar: seleccion, panel de detalle, menus `Inspect/Analyze/Studio`, modo termico, fasteners y onboarding.
- Idea fuerza: la tesis ya tiene artefacto funcional.

## 6 a 10 minutos - Novedades fuertes frente a la propuesta

- Mensaje: explicar que el valor del proyecto no solo esta en "mostrar un dron", sino en resolver CAD complejo, UX guiada y visualizacion tecnica dentro de restricciones reales de WebGL.
- Mostrar:
  - onboarding procedural;
  - manejo de fasteners;
  - modo termico;
  - trazabilidad y tooling.
- Idea fuerza: hubo profundizacion tecnica real, no solo pulido superficial.

## 10 a 13 minutos - Estado del informe y evidencia

- Mensaje: el informe no esta en blanco ni desalineado del software.
- Mostrar:
  - estructura del informe;
  - avance del capitulo de desarrollo;
  - existencia de manuales y documentos de soporte;
  - matriz o capa de trazabilidad en Obsidian.
- Idea fuerza: el cierre documental ya esta encaminado.

## 13 a 16 minutos - Lo que falta para entregar

- Mensaje: ya se ve con claridad la ultima milla.
- Explicar:
  - freeze de build;
  - evidencia final;
  - resultados;
  - empaque de entrega;
  - preparacion de sustentacion.
- Idea fuerza: el trabajo pendiente es finito y priorizable.

## 16 a 19 minutos - Decisiones que se le deben pedir al asesor

- Confirmar el alcance final aceptable para entrega.
- Confirmar si la validacion esperada es tecnica, piloto o formal con muestra.
- Confirmar si el foco debe estar en cerrar un prototipo robusto con evidencia de uso o si aun espera una expansion mayor del artefacto.
- Confirmar la ruta y fechas internas para entregar y sustentar el proximo mes.

## 19 a 20 minutos - Cierre

- Mensaje: "Si estamos alineados en alcance y validacion, yo puedo concentrarme en cierre, evidencia y defensa, en lugar de seguir expandiendo features."
- Resultado buscado: salir con decisiones y no solo con comentarios generales.

## Material que conviene mostrar en la reunion

Si hay pantalla disponible, esta es la secuencia mas eficiente:

1. Build o escena funcional de la app.
2. Onboarding dentro de la app.
3. Un ejemplo de seleccion + bottom sheet.
4. Un ejemplo de `Inspect` o `Analyze`.
5. Un ejemplo del modo `Thermal`.
6. Un ejemplo del sistema de fasteners.
7. El informe final y su estructura.
8. La capa de trazabilidad en Obsidian.

## Lo que no conviene hacer en la reunion

- No listar todos los bugs corregidos.
- No explicar commit por commit.
- No profundizar en Blender, shaders o matematica salvo que el asesor lo pregunte.
- No abrir demasiados documentos a la vez.
- No vender como cerrado lo que todavia depende del freeze y la evidencia final.

## Preguntas concretas que conviene hacerle al asesor

1. Con el estado actual del prototipo, considera viable enfocar el cierre en congelar build, recoger evidencia y terminar informe, sin abrir nuevos frentes funcionales?
2. Que tipo de validacion espera ver para considerar suficiente el capitulo de resultados?
3. Ve la tesis mas fuerte como prototipo funcional con evaluacion tecnica y UX, o espera una comparacion experimental mas amplia?
4. Cual seria, para usted, el minimo aceptable para entregar y sustentar el proximo mes?
5. Entre app, informe y sustentacion, donde ve hoy el mayor riesgo?

## Ruta recomendada para entregar el proximo mes

## Fase 1 - Cierre tecnico inmediato

- congelar geometria y fasteners;
- cerrar QA del onboarding y del flujo principal;
- estabilizar build candidata.

## Fase 2 - Evidencia y resultados

- capturar screenshots, clips y evidencia funcional;
- medir performance y documentar observaciones;
- diligenciar resultados con datos reales.

## Fase 3 - Cierre editorial

- terminar conclusiones;
- revisar consistencia entre propuesta, desarrollo y resultados;
- cerrar manuales y anexos;
- preparar version final del informe.

## Fase 4 - Sustentacion

- crear guion de demo;
- condensar narrativa de problema, solucion, aporte y resultados;
- ensayar una defensa breve y clara.

## Riesgos que deben hacerse visibles al asesor

- Riesgo de expandir mas el alcance en lugar de cerrar.
- Riesgo de dejar la validacion para demasiado tarde.
- Riesgo de que resultados y evidencia no se congelen a tiempo.
- Riesgo de que el criterio de "suficiente para entregar" no quede explicitado en esta reunion.

## Mitigacion recomendada

- acordar que desde ahora el foco es cierre, no expansion;
- priorizar freeze de build y evidencia sobre nuevas features;
- convertir esta reunion en punto de decision sobre validacion y alcance final;
- trabajar con un cronograma corto de entrega, revisable semanalmente.

## Frase de cierre sugerida

"La propuesta ya se convirtio en un prototipo funcional con soporte documental real. Lo que necesito en esta etapa es confirmar con usted cual es la ruta mas eficiente de cierre para entregar y sustentar el proximo mes sin dispersar el esfuerzo."

## Criterio de exito de esta reunion

La reunion habra sido exitosa si al final queda claro:

- que el proyecto ya avanzo a un estado funcional serio;
- que ya existe una ruta realista de cierre;
- que el asesor valida el enfoque de priorizar entrega y sustentacion;
- y que salen definidas las decisiones minimas para no seguir trabajando a ciegas.
