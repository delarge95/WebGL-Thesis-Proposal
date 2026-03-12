# Indice del Sistema Termico

## Nota Importante

Los archivos de este sistema no estan vacios en disco.
Si algun enlace del chat o del visor interno abre un archivo vacio, toma este indice como fuente de verdad y abre las rutas manualmente desde VS Code o desde el explorador del proyecto.

## Carpeta Base

`E:\WebGL_tesis\desarrollo\docs\sistema_termico\`

## Orden Recomendado de Apertura

1. Arquitectura general
2. Workflow de verificacion matematica
3. Solver termico base
4. Capa visual termica
5. Perfil espacial por pieza
6. Integracion UI y control de carga
7. Shader termico

## Archivos Clave

### 1. Arquitectura y documentacion

- `E:\WebGL_tesis\desarrollo\docs\sistema_termico\README.md`
  - Documento principal del sistema termico.
  - Contiene arquitectura, modelo fisico, decisiones cerradas, etapas y estado base implementado.

- `E:\WebGL_tesis\desarrollo\docs\sistema_termico\wolfram_verificaciones.md`
  - Registro de verificaciones matematicas con WolframAlpha.
  - Aqui debe quedar trazada cada conversion o derivacion relevante.

- `E:\WebGL_tesis\desarrollo\docs\sistema_termico\wolfram_query_conductance_factor.json`
  - Primer artefacto generado por el workflow de Wolfram.
  - Guarda la consulta usada para revisar el factor geometrico de conduccion.

### 2. Workflow / Skill local

- `E:\WebGL_tesis\.agent\skills\wolfram-thermal-verifier\SKILL.md`
  - Skill local para verificar ecuaciones, unidades y aproximaciones termicas.

- `E:\WebGL_tesis\.agent\skills\wolfram-thermal-verifier\references\wolfram-workflow.md`
  - Guia de uso del workflow de WolframAlpha.

- `E:\WebGL_tesis\.agent\skills\wolfram-thermal-verifier\scripts\wolfram_verify.py`
  - Script para generar consultas y, si existe `WOLFRAM_APP_ID`, consultar los endpoints oficiales.

- `E:\WebGL_tesis\.agent\workflows\thermal_math_verification.md`
  - Workflow resumido del proceso de verificacion matematica.

### 3. Codigo base del sistema termico

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalSimulationManager.cs`
  - Solver termico reducido por componentes.
  - Maneja nodos, enlaces, temperatura ambiente, carga, enfriamiento y conduccion heuristica.

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalContactGraphAsset.cs`
  - Asset para el grafo de contactos termicos.
  - Sera la base del preprocesado offline en la siguiente etapa.

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalViewController.cs`
  - Puente entre simulacion y render.
  - Lee temperaturas del solver y las envía al shader por pieza.

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalSurfaceProfile.cs`
  - Perfil espacial por pieza.
  - Define patron de calor, hotspot, direccion, spread y propagacion visual.

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Events\ThermalEvents.cs`
  - Eventos del subsistema termico.

### 4. Integracion con el dron y la escena

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Managers\DroneStateController.cs`
  - Controla estados `Off`, `StartingUp`, `Idle`, `Flying`, `ShuttingDown`.
  - Expone `SystemLoadFactor` y sincroniza carga con animacion/estado.

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Utils\SceneBootstrapper.cs`
  - Asegura la carga de `ThermalSimulationManager` y `ThermalViewController` en escena.

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Data\DronePartData.cs`
  - Ya tiene campos termicos agregados para la V1.

### 5. UI y control de potencia

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\UI\Panels\InspectModeHandler.cs`
  - Conecta el boton de power y el slider de carga sostenida con `DroneStateController`.

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\UI\Layouts\MainLayout.uxml`
  - Contiene `PowerLoadSlider` y `PowerLoadValue`.

### 6. Render termico

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Shaders\Thermal.shader`
  - Shader termico actualizado.
  - Ahora recibe parametros espaciales por pieza: modo, hotspot, direccion, spread y propagacion.

## Archivos Minimos para Revisar Primero

Si quieres inspeccionar lo mas importante sin perderte, abre estos cinco en este orden:

1. `E:\WebGL_tesis\desarrollo\docs\sistema_termico\README.md`
2. `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalSimulationManager.cs`
3. `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalViewController.cs`
4. `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Managers\DroneStateController.cs`
5. `E:\WebGL_tesis\desarrollo\unity_project\Assets\Shaders\Thermal.shader`

## Si el visor vuelve a fallar

Haz esto:

1. Abre el archivo desde el arbol del proyecto, no desde el enlace del chat.
2. Si el panel de diff muestra cosas raras, recarga la ventana o reabre el workspace.
3. Usa este indice como punto de entrada manual.
4. No asumas que "archivo vacio" significa perdida real hasta comprobar la ruta en disco.

## Proximo Paso Recomendado

Cuando confirmes que puedes abrir bien estos archivos desde VS Code, seguimos con la siguiente etapa tecnica:

- preprocesado offline del grafo de contactos
- perfiles termicos reales para motores, ESC, bateria y brazos
- leyenda termica visible en UI
