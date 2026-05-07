---
tipo: modulo_estudio
fuente: Informe_final/chapters/04_desarrollo.tex
estado: activo
capitulo: 4
area: informe-final
tags:
  - tesis
  - desarrollo
  - unity
  - pipeline-3d
  - ux-ui
---

# INF EST 04 Desarrollo e Implementacion

## Idea central

Este capitulo documenta el estado tecnico del desarrollo: que se implemento, que se evaluo durante el proceso y que frentes permanecen abiertos antes del freeze definitivo de la build. Explica como el proyecto paso de modelos CAD y documentacion dispersa a una app web 3D con arquitectura, UI, shaders, modos de inspeccion, tooling y trazabilidad funcional.

La lectura de estudio debe centrarse en la historia tecnica del producto: que problemas aparecieron, que decisiones se tomaron y que quedo visible, oculto o pendiente.

## Estado del desarrollo

El informe final adopta una distincion de estado para evitar sobrepromesas. El capitulo separa:

- ejecutado;
- evaluado;
- descartado o reemplazado;
- pendiente de cierre.

Esta separacion delimita el alcance real del producto y evita presentar como final lo que aun no esta integrado.

## Normalizacion del sistema: 16, 28, 30 y 257

Uno de los puntos mas sensibles es el conteo de piezas.

- 16 piezas: etapa historica temprana, no estado final.
- 28 piezas: taxonomia canonica de investigacion.
- 30 anchors: 28 piezas mas grupos tecnicos de fasteners y misc.
- 257 renderers/colliders/assets: fragmentos tecnicos generados por la importacion y preparacion de escena.

La idea sencilla: una "pieza academica" no siempre equivale a un mesh o renderer. Un motor puede tener varios fragmentos geometricos, pero seguir siendo una sola pieza conceptual.

## Pipeline CAD a runtime

El trabajo no fue simplemente importar un modelo.

Hubo que resolver:

- entrada desde CAD;
- limpieza y remodelado;
- normalizacion de piezas;
- separacion de componentes;
- reduccion geometrica;
- texturas y materiales;
- exportacion;
- binding en Unity;
- reparacion runtime;
- validacion de cobertura.

### Metodos de importacion

El informe presenta una comparacion de rutas:

- STEP directo a Blender;
- exportaciones desde MoI3D con distintas configuraciones;
- reconstruccion parcial o total de piezas;
- rutas descartadas por peso, geometria sucia o dificultad de control.

La defensa fuerte es: se eligieron rutas por criterio tecnico, no por comodidad.

## MAD-T adaptado

MAD-T se usa como marco adaptado de hard-surface. En terminos pedagogicos, sirve para ordenar el proceso:

- analizar la forma;
- dividir el modelo en componentes;
- decidir que se mantiene, que se rehace y que se simplifica;
- reconstruir con limpieza;
- preparar para texturizado y runtime.

Debe aclararse que no se presenta como metodologia academica universal, sino como marco de produccion tecnico-artistica adaptado al caso.

## Sistema modular de tornillos

Los tornillos son piezas repetitivas. Si cada tornillo se modela como geometria unica y pesada, el costo crece sin aportar comprension proporcional.

El sistema modular permite:

- reutilizar geometria;
- mantener lectura visual;
- reducir fragmentacion;
- controlar peso;
- facilitar reemplazos.

Esta decision conecta arte tecnico con optimizacion.

## UX/UI mobile-first

La interfaz con mayor respaldo teorico es la version mobile-first. Esto significa que se penso primero para pantallas tactiles, jerarquia clara y accion principal visible.

Principios usados:

- jerarquia visual;
- accesibilidad tactil;
- agrupacion de acciones por modo;
- ficha inferior para detalle;
- feedback visual de seleccion;
- reduccion de memoria del usuario;
- progresion Hero -> Explore -> seleccion -> bottom sheet -> modos.

La version desktop es funcional, pero debe reconocerse como adaptacion del modelo movil. Esa honestidad fortalece el proyecto porque evita presentar un sistema desktop completamente optimizado si no lo fue.

### Onboarding procedural

El onboarding es una capa de primer uso que ensena el lenguaje de la app antes de pedirle al usuario que resuelva tareas. No debe entenderse como un video de ayuda agregado al final, sino como una extension del sistema UI.

Su valor esta en tres puntos:

- reduce incertidumbre inicial sobre orbita, paneo, seleccion y modos;
- mantiene coherencia visual con la interfaz porque las demos se dibujan por codigo;
- evita depender de GIFs, videos o assets externos que aumentarian peso y mantenimiento.

En la app, el onboarding se apoya en `OnboardingController` y `OnboardingAnimationView`. La segunda pieza usa `Painter2D` para dibujar actor, objetivo y respuesta del sistema: cursor o dedo, gesto, boton, slider, ripple o cambio visual. Esto permite explicar interacciones de escritorio y movil desde el mismo lenguaje grafico.

### Info panel, ficha inferior y jerarquia de seleccion

La ficha inferior o `bottom sheet` es el centro semantico de la experiencia. No solo muestra texto: convierte una seleccion visual en informacion tecnica usable.

La ficha debe explicar:

- que objeto fue seleccionado;
- a que categoria pertenece;
- que datos de identificacion, especificacion y ensamblaje existen;
- si el usuario esta leyendo una pieza madre, una subpieza, un grupo de hotspot o un fastener;
- cuando algun campo aparece como `N/A` porque el dato no existe en `DronePartData`.

Esta jerarquia evita una confusion frecuente en defensa: no todo clic equivale a una pieza canonica completa. Una misma interaccion puede resolver niveles distintos de lectura. Por eso la ficha, el highlight, la camara y el aislamiento deben mantenerse sincronizados.

## Iconografia procedural y microinteracciones

El proyecto no solo diseno botones. Desarrollo iconos y microinteracciones en codigo.

Esto importa porque:

- evita depender de assets externos dispersos;
- permite coherencia visual;
- facilita estados activos/inactivos;
- habilita animaciones de feedback;
- hace que la UI sea parte del sistema tecnico.

Temas para estudiar:

- Painter2D;
- iconos procedurales;
- estados hover/active;
- animaciones por resortes;
- feedback visual.

## Arquitectura operativa

La arquitectura tiene varias capas:

- bootstrap;
- managers;
- UI Toolkit;
- control de escena;
- binding de piezas;
- modos de visualizacion;
- subsistema termico;
- tooling de editor.

La idea fuerte es que la app no depende solo de objetos puestos a mano en escena. Tiene saneamiento runtime mediante `ImportedDroneRuntimeBinder`, garantia de managers y reparacion de import.

## Flujo visible final

El flujo que se documenta como final visible es:

- Hero;
- Explore;
- seleccion de pieza;
- bottom sheet;
- Inspect;
- Analyze;
- Studio;
- filtros;
- thermal legend;
- explode;
- cross-section.

Capacidades no visibles, como measurement o paneles legacy, deben quedar como ocultas o no integradas.

## Shaders y entornos

Los modos visuales no son decoracion. Cada uno ayuda a leer el ensamblaje de otra forma:

- Realistic: lectura material realista.
- X-Ray: lectura interior o jerarquica.
- Blueprint: lectura tecnica, casi de plano.
- SolidColor: segmentacion limpia.
- Wireframe: estructura geometrica.
- Ghosted: transparencia y relacion espacial.
- Thermal: tendencia termica relativa.

### Modo Blueprint

Blueprint convierte el dron en un lenguaje mas cercano a plano tecnico: contornos, contraste y lectura analitica. Sirve para entender forma y estructura sin depender del realismo de materiales.

### Entornos

Los presets Day, Night y Sunset modifican contexto visual. No deben venderse como simulacion fisica de iluminacion real; son configuraciones de estudio para revisar el modelo bajo distintas condiciones visuales.

## Subsistema termico

El subsistema termico debe defenderse con cuidado:

- no es FEA;
- no es medicion experimental;
- no es solver fisico calibrado;
- si es visualizacion heuristica aplicada.

Su valor esta en comunicar tendencias relativas de carga y temperatura a nivel de componentes.

## Tooling de editor

Herramientas como `ProjectSetupWizard`, `ImportedDroneCoverageAudit` y `ThermalContactGraphBuilderWindow` muestran que hubo trabajo de pipeline, no solo runtime.

El tooling ayuda a:

- auditar cobertura;
- generar datos;
- revisar relaciones;
- reducir trabajo manual;
- aumentar trazabilidad.

## Preguntas dificiles de defensa

### Por que hay funciones ocultas si el sistema esta desarrollado?

Porque no todo codigo existente debe exponerse en la UI final. Se priorizo un flujo visible estable y verificable. Lo oculto queda documentado como implementado pero no expuesto, legado o trabajo futuro.

### Por que la UI desktop no tiene el mismo rigor que mobile?

Porque el diseno se centro en mobile-first. La version desktop es funcional y utilizable, pero se mantiene como adaptacion. Reconocerlo evita sobreprometer.

### El sistema termico es cientificamente valido?

Es valido como visualizacion heuristica si se presenta con ese alcance. No seria valido venderlo como FEA o como modelo termico calibrado.

### Por que usar MAD-T si no es una metodologia academica tradicional?

Porque se usa como marco tecnico de produccion hard-surface adaptado, no como metodologia cientifica principal. La metodologia academica es DSR.

## Fuentes y notas relacionadas

- [[Pipeline_Modelado_Dron]]
- [[Optimizacion_CAD_WebGL]]
- [[Estrategia_Shaders_WebGL]]
- [[Sistema_Iconos_Procedurales_UI]]
- [[Estabilidad_y_Migracion_Unity6]]
- [[Investigacion_Holybro_X500v2]]
- [[Fisica_Termica_Dron]]
- [[MOC_UX_UI_Complete]]
- [[INF_EST_30_Pipeline_CAD_MAD_T]]
- [[INF_EST_31_UX_UI_Mobile_First]]
- [[INF_EST_32_Iconografia_Procedural_Microinteracciones]]
- [[INF_EST_33_Shaders_ViewModes_Entornos]]
- [[INF_EST_34_Sistema_Termico_Hibrido]]
- [[INF_EST_35_Tooling_Arquitectura_Runtime]]

## Explicaciones complejas dentro de la seccion

### Por que 28 no contradice 257

Los 28 son piezas canonicas de investigacion. Los 257 son unidades tecnicas de render/collider/asset. Una pieza canonica puede estar compuesta por muchas unidades tecnicas. Por eso ambos conteos son verdaderos, pero responden a preguntas distintas.

### Runtime binding

Runtime binding significa que, cuando la app corre, conecta objetos importados con datos y comportamientos. Esto es importante porque la importacion CAD puede cambiar jerarquias o nombres; el binder estabiliza la relacion entre escena y catalogo.

### Estado funcional

El informe usa capas de estado para evitar sobreprometer:

- expuesto en UI final;
- implementado pero oculto;
- codigo legado/no integrado;
- trabajo futuro.

## Terminos importantes de la seccion

- Anchor: nodo de escena que representa o agrupa una pieza.
- Renderer: componente que dibuja una malla.
- Collider: volumen usado para seleccion o interaccion.
- Bottom sheet: ficha inferior de informacion.
- Info panel: superficie de detalle que convierte seleccion en lectura tecnica.
- Onboarding: ayuda inicial que ensena controles y modos dentro de la app.
- Painter2D: API usada para dibujar iconos y demos procedurales en UI Toolkit.
- Hero: pantalla o bloque inicial de entrada.
- Inspect: modo de inspeccion de pieza.
- Analyze: modo de analisis con corte, explode y filtros.
- Studio: modo visual y de entorno.
- Fastener: tornillo o sujetador.
- Subpieza: nivel interno de una pieza madre que puede recibir lectura o aislamiento especifico.
- Hotspot: marcador visual que permite entrar a una lectura de punto o grupo funcional.
- Tooling: herramientas auxiliares de desarrollo.
- Runtime: momento en que la app esta ejecutandose.
