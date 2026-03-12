# Indice de Codigo Termico

## Objetivo

Este archivo sirve como punto de entrada rapido para revisar y modificar la implementacion termica en Unity sin depender del visor del chat.

## Carpetas Base

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\`
- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Editor\Thermal\`

## Orden Recomendado de Lectura

1. `ThermalSimulationManager.cs`
2. `ThermalViewController.cs`
3. `ThermalContactGraphBuilderWindow.cs`
4. `ThermalContactGraphAsset.cs`
5. `ThermalSurfaceProfile.cs`
6. `DroneStateController.cs`
7. `InspectModeHandler.cs`
8. `Thermal.shader`

## Mapa del Sistema

### 1. Solver principal

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalSimulationManager.cs`
- Es el nucleo fisico de la V1.
- Construye nodos termicos desde `ExplodablePart + DronePartData`.
- Calcula temperatura de equilibrio, enfriamiento al ambiente y conduccion entre piezas.

### 2. Puente entre simulacion y render

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalViewController.cs`
- Lee temperaturas del solver.
- Localiza renderers y les inyecta parametros termicos por `MaterialPropertyBlock`.
- Decide el patron visual por pieza: uniforme, radial o axial.

### 3. Authoring offline del grafo

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Editor\Thermal\ThermalContactGraphBuilderWindow.cs`
- Recorre `ExplodablePart` del dron en escena.
- Estima cercania y area de contacto a partir de bounds.
- Genera un `ThermalContactGraphAsset` reutilizable en runtime.

### 4. Grafo de contactos

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalContactGraphAsset.cs`
- Define enlaces termicos, metadata de build y lista de nodos.
- Es la base serializable para reemplazar el fallback heuristico.

### 5. Perfil espacial por pieza

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Thermal\ThermalSurfaceProfile.cs`
- Permite refinar visualmente una pieza concreta sin tocar el solver global.
- Define hotspot, direccion, spread, edge cooling y propagacion.

## Archivos Externos Relacionados

### Estado y carga del dron

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\Core\Managers\DroneStateController.cs`
- Expone `SystemLoadFactor`.
- Cambia entre `Off`, `StartingUp`, `Idle`, `Flying` y `ShuttingDown`.
- El slider de carga y la simulacion termica dependen de este archivo.

### UI de potencia

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\UI\Panels\InspectModeHandler.cs`
- Conecta el boton de power y el slider de carga con `DroneStateController`.

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Scripts\UI\Layouts\MainLayout.uxml`
- Contiene `PowerLoadSlider` y `PowerLoadValue`.

### Shader termico

- `E:\WebGL_tesis\desarrollo\unity_project\Assets\Shaders\Thermal.shader`
- Recibe los parametros espaciales enviados por `ThermalViewController`.
- Pinta el gradiente termico visible por pieza.

## Si quieres cambiar...

### Fisica termica global

Empieza por:

- `ThermalSimulationManager.cs`

### Contactos entre piezas

Empieza por:

- `ThermalContactGraphBuilderWindow.cs`
- `ThermalContactGraphAsset.cs`
- y luego `ThermalSimulationManager.cs`

### Aspecto visual del calor

Empieza por:

- `ThermalViewController.cs`
- `ThermalSurfaceProfile.cs`
- `Thermal.shader`

### Slider de potencia o comportamiento de encendido

Empieza por:

- `DroneStateController.cs`
- `InspectModeHandler.cs`
- `MainLayout.uxml`

## Estado Actual de la Implementacion

Ya existe:

- solver termico reducido por componentes
- vista termica alimentada por temperatura real por pieza
- perfil espacial basico por hotspot/direccion
- slider de carga sostenida conectado al dron
- asset del grafo termico con metadata de build
- herramienta offline para generar contactos candidatos desde la escena

Todavia falta:

- calibracion del grafo sobre la geometria final
- perfiles termicos reales asignados a todas las piezas criticas
- leyenda termica visible en UI
- calibracion con datos mas cercanos a materiales reales