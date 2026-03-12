# Sistema Termico del Drone

## Objetivo

Definir e implementar una simulacion termica hibrida, fisicamente consistente y visualmente creible para el Holybro X500 V2 dentro del prototipo Unity WebGL.

El sistema no buscara resolver un FEA termico completo en tiempo real sobre la malla visual final. En su lugar, combinara:

- una simulacion reducida por componentes y contactos termicos,
- una visualizacion espacial sobre piezas criticas,
- y una capa de preprocesado offline para mantener el runtime liviano en WebGL movil.

## Decisiones Cerradas

- El objetivo es una aproximacion fisica defendible, no una simulacion cuantitativa certificable.
- La simulacion sera cualitativa avanzada, con comportamiento tendencial consistente.
- Debe cubrir todo el dron, pero priorizando subsistemas criticos.
- La transferencia principal sera por conduccion en solidos en contacto.
- Habra conveccion simplificada al ambiente.
- La radiacion queda fuera en V1 por costo computacional.
- La simulacion correra siempre en segundo plano.
- Cambiar de modo visual o activar exploded view no reiniciara la simulacion.
- La vista explosionada no cambiara la fisica, solo la lectura visual.
- El sistema debe sostener WebGL movil con un minimo practico de 23 FPS.

## Piezas Criticas de V1

Las piezas con prioridad de propagacion espacial y respuesta termica mas rica en la primera iteracion seran:

- 4 motores
- 4 ESC
- bateria
- PDB
- power module
- Pixhawk
- 4 brazos
- top plate
- bottom plate

Piezas pequenas o de bajo impacto termico podran:

- heredar temperatura de la pieza padre,
- usar regiones simplificadas,
- o quedar capadas artificialmente en V1.

## Arquitectura Hibrida

### 1. Capa fisica reducida

La base del sistema sera una red termica por componentes. Cada pieza relevante se modela como un nodo con:

- temperatura actual,
- temperatura de equilibrio segun estado/carga,
- capacidad termica efectiva,
- perdida al ambiente,
- y enlaces de conduccion con piezas vecinas.

Modelo general:

```text
C_i dT_i/dt = Q_i(u, estado) + sum_j G_ij (T_j - T_i) - H_i (T_i - T_amb)
```

Donde:

- `C_i = m_i * c_p,i`
- `Q_i` es la generacion interna de calor
- `G_ij` es la conductancia termica entre piezas en contacto
- `H_i` agrupa perdidas por conveccion simplificada
- `T_amb` es la temperatura ambiente base

### 2. Capa de visualizacion espacial

La simulacion visual no correra sobre toda la malla final en runtime. Se hara asi:

- Solver fisico por componente o subcomponente critico
- Propagacion espacial visual sobre malla termica simplificada
- Transferencia de temperaturas a vertices/regiones/texels segun costo

Objetivo visual:

- el calor debe verse viajando desde la fuente,
- con gradiente espacial,
- respetando direccion y cercania del origen,
- sin limitarse a un color uniforme por pieza cuando la pieza sea critica.

### 3. Capa de interaccion

La activacion se conectara al sistema de estados del dron:

- `Power` enciende y apaga el sistema
- un slider `0%-100%` representa carga sostenida del sistema
- los estados discretos siguen existiendo: `Off`, `StartingUp`, `Idle`, `Flying`, `ShuttingDown`

Interpretacion del slider:

- `0%`: idle termico
- `35%-45%`: hover nominal
- `70%-100%`: carga alta sostenida

### 4. Capa de preprocesado offline

La capa offline es obligatoria para mantener rendimiento.

Se usara para:

- detectar contactos entre piezas,
- generar o editar el grafo termico,
- preparar regiones termicas de piezas criticas,
- calcular distancias o pesos de propagacion,
- y empaquetar assets ligeros para runtime.

## Modelo Inicial de Fenomenos

### Fuentes de calor primarias

- motores
- ESCs
- bateria

### Fuentes secundarias

- Pixhawk
- PDB
- power module
- radio
- GPS

### Receptores/disipadores

- brazos
- top plate
- bottom plate
- estructura restante

### Conduccion

Aproximacion base:

```text
G_ij ~= k_eff * A_contacto / L_efectiva
```

### Conveccion

Aproximacion base:

```text
H_i ~= h_i * A_expuesta
```

En V1:

- `h_i` sera simplificado por clase de pieza
- motores y zonas barridas por helices tendran enfriamiento mas alto
- la estructura tendra enfriamiento moderado por superficie expuesta

### Radiacion

No entra en V1.

## Datos Disponibles

Fuentes actuales del proyecto:

- `desarrollo/docs/investigacion/Holybro/x500v2_parts_data.json`
- `desarrollo/docs/investigacion/Holybro/x500v2_blender_synced_parts.json`
- documentacion oficial y reportes en `desarrollo/docs/investigacion/Holybro/`

Campos termicos ya existentes en el dataset principal:

- `operatingTempMin`
- `operatingTempMax`
- `thermalHover`
- `thermalPeak`
- `thermalWarmupSeconds`

Esto permite levantar una primera simulacion coherente incluso antes de completar la biblioteca de materiales termicos.

## Biblioteca Termica Requerida

Todavia falta consolidar, para cada familia de materiales:

- conductividad termica `k`
- densidad `rho`
- calor especifico `cp`
- difusividad termica cuando aporte valor
- emisividad si en una iteracion futura llega a usarse

Familias minimas:

- aluminio CNC / anodizado
- acero
- cobre
- FR4
- fibra de carbono / compuestos
- nylon / plasticos reforzados
- LiPo
- siliconas / espumas / pads

## Integracion con el Proyecto Actual

Estado actual confirmado:

- `DroneStateController` ya expone carga sostenida y estados de encendido reutilizables para la simulacion.
- `Thermal.shader` ya recibe parametros termicos por pieza y soporta gradiente espacial basico.
- `ViewModeManager` sigue cambiando materiales, pero ahora la capa termica se alimenta desde `ThermalViewController`.
- `ThermalSimulationManager` ya existe como solver reducido V1 por componentes y contactos heuristico.

Por tanto, la arquitectura base quedara compuesta por:

- `ThermalSimulationManager`
- `ThermalViewController`
- `ThermalSurfaceProfile`
- `ThermalContactGraphAsset`
- eventos termicos
- ampliacion de `DronePartData`
- extension de `DroneStateController` para exponer carga sostenida

## Estado de Implementacion - Marzo 12, 2026

Ya esta implementado:

- contrato termico explicito en `DronePartData`
- solver por componentes en `ThermalSimulationManager`
- control de carga sostenida en `DroneStateController`
- slider de carga en `InspectModeHandler` y `MainLayout.uxml`
- puente termico a shader en `ThermalViewController`
- metadata serializable y enlaces en `ThermalContactGraphAsset`
- herramienta de editor `ThermalContactGraphBuilderWindow` para generar grafos offline

Sigue pendiente:

- calibrar el grafo sobre la geometria final retopologizada
- conectar assets termicos precomputados a las escenas finales del X500 V2
- asignar perfiles espaciales manuales a las piezas criticas
- medir el comportamiento visual/rendimiento en build objetivo
## Estrategia de Implementacion por Etapas

### Etapa 0. Fundacion

Estado: completada.

- carpeta tecnica y workflow local creados
- skill/workflow local para verificacion con WolframAlpha definido
- `DronePartData` alineado con contrato termico
- `ThermalSimulationManager` creado
- `DroneStateController` conectado a carga sostenida

### Etapa 1. Solver por componentes

Estado: completada en base funcional.

- nodos termicos levantados desde `ExplodablePart + DronePartData`
- inicializacion de temperaturas
- calentamiento por fuentes
- enfriamiento al ambiente
- conduccion por enlaces termicos

### Etapa 2. Grafo de contactos

Estado: implementada en authoring, pendiente de calibracion final.

- `ThermalContactGraphAsset` ya existe como asset serializable
- `ThermalContactGraphBuilderWindow` ya genera contactos candidatos offline
- el fallback heuristico sigue disponible mientras se calibra el asset final
- falta migrar a geometria final retopologizada del X500 V2

### Etapa 3. Visualizacion termica real

Estado: parcial.

- `ThermalViewController` ya alimenta el shader con temperatura real por pieza
- existe propagacion espacial visual basica
- falta asignacion refinada de perfiles por pieza critica y leyenda visible en UI

### Etapa 4. Refinamiento por superficie

Pendiente.

- regiones o vertices para brazos, motores, ESC, plates y bateria
- soporte a malla termica simplificada mas precisa
- degradacion elegante para movil

### Etapa 5. Validacion

Pendiente.

- comparar tendencias con fichas tecnicas y supuestos del sistema
- documentar ecuaciones, limites y simplificaciones en el informe
- medir rendimiento y comportamiento visual en builds objetivo

## Workflow de Verificacion con WolframAlpha

Todos los calculos que afecten ecuaciones, unidades, constantes o derivaciones del sistema termico deben verificarse con WolframAlpha antes de fijarse en codigo o documentacion.

Workflow obligatorio:

1. Normalizar la ecuacion en una forma clara y con unidades explicitas.
2. Expresar supuestos: ambiente, carga, material, dimensiones.
3. Verificar la forma cerrada o el resultado numerico en WolframAlpha.
4. Registrar la consulta y el resultado en la documentacion o en el commit si la derivacion es importante.
5. Solo despues llevar el resultado a Unity o al informe.

Endpoints oficiales utiles:

- Sitio web: `https://www.wolframalpha.com/input?i=...`
- Short Answers API: `http://api.wolframalpha.com/v1/result`
- Full Results API: `http://api.wolframalpha.com/v2/query`
- LLM API: `https://www.wolframalpha.com/api/v1/llm-api`

Referencias oficiales:

- [Wolfram|Alpha APIs Overview](https://products.wolframalpha.com/api)
- [Short Answers API](https://products.wolframalpha.com/short-answers-api/documentation)
- [Full Results API](https://products.wolframalpha.com/api/documentation)
- [LLM API](https://products.wolframalpha.com/llm-api/documentation)

## Regla de Honestidad Tecnica

No se presentara esta V1 como:

- simulacion CFD,
- FEA termico completo,
- o solucion industrial de termodinamica de alta fidelidad.

Si se presentara como:

- una simulacion hibrida,
- fisicamente inspirada y consistente,
- con conduccion entre componentes, conveccion simplificada,
- y visualizacion espacial creible para un gemelo digital tecnico en WebGL.

## Proximos Pasos Inmediatos

- calibrar el asset generado por `ThermalContactGraphBuilderWindow` sobre la geometria final
- asignar `ThermalSurfaceProfile` a las piezas criticas reales del X500 V2
- alimentar el solver con propiedades termicas mas cercanas a materiales reales
- incorporar la leyenda termica visible y la lectura fija en grados Celsius

## Estado Base Implementado

Quedo sembrada una base termica ya util para seguir iterando:

- solver termico reducido por componentes (`ThermalSimulationManager`)
- capa visual termica por pieza (`ThermalViewController`)
- perfiles de espacializacion para piezas criticas (`ThermalSurfaceProfile`)
- slider de carga sostenida conectado al encendido del dron
- grafo termico serializable con metadata de build (`ThermalContactGraphAsset`)
- preprocesador offline en editor (`ThermalContactGraphBuilderWindow`)