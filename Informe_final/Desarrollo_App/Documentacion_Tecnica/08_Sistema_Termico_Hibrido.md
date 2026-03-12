# 08. Sistema Termico Hibrido

## Estado

Activo, en desarrollo tecnico. No debe describirse todavia como simulacion termodinamica completa ni como FEA en tiempo real.

## Objetivo

Implementar una simulacion termica hibrida, fisicamente consistente y visualmente creible para el dron Holybro X500 V2 en Unity WebGL.

## Alcance de la V1

- solver termico reducido por componentes
- conduccion entre piezas en contacto
- conveccion simplificada al ambiente
- vista termica alimentada por temperatura calculada por pieza
- propagacion espacial visual basica en piezas criticas
- control de carga sostenida mediante slider asociado al estado del dron
- authoring offline del grafo de contactos termicos

## Componentes Principales

### ThermalSimulationManager

Responsable del solver por componentes. Construye nodos termicos, evalua carga efectiva, calcula equilibrio, enfriamiento y conduccion.

### ThermalViewController

Puente entre solver y render. Lee temperaturas y envia parametros al shader termico por `MaterialPropertyBlock`.

### ThermalSurfaceProfile

Perfil opcional por pieza critica. Permite definir hotspot, direccion y propagacion visual sin modificar la fisica global.

### ThermalContactGraphAsset

Asset que almacena nodos, metadata de build y enlaces termicos entre piezas.

### ThermalContactGraphBuilderWindow

Herramienta de editor que analiza `ExplodablePart` en la escena, estima cercania/contacto por bounds y genera un `ThermalContactGraphAsset` reutilizable en runtime.

### Thermal.shader

Shader termico ajustado para recibir banda de temperatura normalizada, modo de propagacion visual, hotspot local, direccion axial local, spread, edge cooling y propagation.

## Integracion con el Sistema Existente

- `DroneStateController` expone `SystemLoadFactor` y eventos de carga.
- `InspectModeHandler` conecta el slider de carga con `DroneStateController`.
- `MainLayout.uxml` incorpora `PowerLoadSlider` y `PowerLoadValue`.
- `SceneBootstrapper` asegura la presencia de `ThermalSimulationManager` y `ThermalViewController` en runtime.
- `ThermalContactGraphBuilderWindow` habilita un workflow de preprocesado offline sin tocar escenas serializadas.

## Limitaciones Actuales

- el grafo offline actual se basa en bounds, no aun en contactos refinados de la geometria final retopologizada
- no hay validacion cuantitativa cerrada contra termografias reales
- no se han asignado aun perfiles manuales refinados a todas las piezas criticas
- la simulacion actual prioriza credibilidad visual y estabilidad WebGL sobre exactitud numerica de alta fidelidad

## Siguiente Hito

Calibrar el grafo termico sobre la geometria final del X500 V2, asignar `ThermalSurfaceProfile` a piezas criticas y reemplazar el fallback runtime donde ya exista asset precomputado.