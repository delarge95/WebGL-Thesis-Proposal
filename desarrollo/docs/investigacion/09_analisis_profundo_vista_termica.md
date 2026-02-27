# Análisis Profundo: Vista Térmica (Heatmap) en Gemelos Digitales WebGL

**Fecha:** 26 de Febrero, 2026
**Contexto:** Justificación teórica y técnica para la defensa de tesis frente a la implementación de una "Vista de Calor" en un entorno interactivo WebGL (Unity).

---

## 1. ¿Qué es y por qué incluirlo? (El Fundamento Teórico)

En el ámbito de la ingeniería y la robótica, un visualizador 3D tradicional (CAD) se limita a ser una **representación espacial** (geometría y dimensiones). Al integrar una **Vista Térmica**, la aplicación da el salto cognitivo de ser un simple "modelo 3D" a convertirse en un **Gemelo Digital (Digital Twin)** guiado por datos (Data-Driven).

**¿Por qué es vital en hardware de alto rendimiento?**
Los drones como el Holybro X500 o plataformas Freestyle operan extrayendo cantidades brutales de amperaje de baterías LiPo hacia Controladores de Velocidad (ESCs) y motores *brushless*. 
*   **El problema:** El factor restrictivo #1 en el diseño de drones no es el espacio, sino la **disipación térmica**. Si un ESC alcanza los $120^\circ C$, el MOSFET se funde y el dron cae.
*   **La solución en tu tesis:** Poder visualizar la "temperatura operativa" (*Operating Temp*) de las distintas piezas de forma gráfica permite entender instantáneamente **puntos críticos de falla térmica (Cuellos de botella)** en el ensamblaje.

**Apoyo Teórico (Carga Cognitiva de Sweller):**
Si presentas una tabla de texto con temperaturas, el cerebro del ingeniero debe leer el número ("$95^\circ C$"), compararlo con un umbral mental tolerado ("¿Es más de $80^\circ$? Sí") y localizar mentalmente esa pieza en el cuadricóptero. **Esta es una altísima Carga Extrínseca.**
Convertir esos datos en un **mapa de gradiente visual (azul = seguro, rojo/blanco = crítico)** aprovecha la percepción visual pre-atencional. El ingeniero o supervisor detecta el peligro en fracciones de segundo sin procesar números, enfocando su cognición en la *toma de decisiones* (ej. rediseñar el flujo de aire).

---

## 2. ¿Para qué me sirve? (Casos de Uso Académicos e Industriales)

Un jurado o inversionista querrá saber la aplicación práctica. Aquí tienes los beneficios concretos:

1.  **Diagnóstico Predictivo / Mantenimiento:** Un estudiante puede usar la app para ver que los ESCs centrales se sobrecalientan a mayor velocidad que los motores expuestos al flujo de aire de las hélices, demostrando por qué ciertos diseños montan los ESCs en los brazos (al aire) y no apilados en el centro.
2.  **Visualización Multidimensional de Datos:** Demuestras que tu aplicación WebGL tiene la capacidad de abstraer variables no visibles a simple vista (la temperatura, que es radiación infrarroja invisible). Esto eleva la tesis de "dibujo 3D" a "software de análisis visual de datos de hardware".
3.  **Diferenciador contra el CAD Tradicional (Mayo/Autodesk):** Como vimos antes, los CADs muestran planos crudos. Tu visor WebGL tiene una capa de renderizado interactivo que **fusiona datos lógicos y gráficos en tiempo real**, algo que Mayo no hace nativamente a menos que le inyectes módulos masivos de simulación de fluidos térmica (CFD), los cuales nunca correrían en un teléfono por Web.
4.  **Escalabilidad Futura (Conexión IoT):** Implementar la lógica del *Shading Térmico* demuestra que tu plataforma está preparada para un futuro donde un dron físico en vuelo envíe su telemetría térmica vía WiFi, y tu visor web se actualice *en vivo* parpadeando en rojo el motor que está fallando.

---

## 3. ¿Cómo se ejecuta? (Implementación Técnica en Unity / WebGL)

Esta es la mejor parte: implementar la vista térmica es extremadamente eficiente y **no perjudica el rendimiento** de la aplicación móvil (tus 50,000 tris). 

Aquí está la arquitectura de ejecución:

### A. La Capa de Datos (C#)
1.  En tu clase `DronePartData.cs` (que ya tienes estructurada), cada componente como el motor o el ESC almacena una variable `operatingTemp` (ej. 65°C) o `heatGenerationFactor`.
2.  Cuando el usuario presiona el botón "THERMAL" en la UI (`MainLayout.uxml`), el `SelectionManager` o un `RenderFeature` global ordena un cambio de visualización.

### B. La Capa Gráfica (Shader Graph / Technical Art)
1.  **NO** cambias los modelos 3D y **NO** cargas texturas extra enormes.
2.  Un *Global Shader Variable* (Variable Global del Shader) cambia el estado de 0 a 1.
3.  En el *Material* de cada pieza, un **Custom Shader** creado en *Shader Graph* activa una rama visual:
    *   **Paso 1:** El Shader lee el script C# para tomar el valor térmico de la pieza (Normalizado del 0.0 al 1.0, donde 0 es $20^\circ C$ ambiente, y 1 es $\geq 100^\circ C$ crítico).
    *   **Paso 2:** Pasa ese valor (ej. 0.8) a través de una rampa de color (Gradient) simulando visión FLIR.
    *   **Paso 3:** La malla de la pieza cambia de su color original PBR (Fibra de carbono mate) a un color auto-iluminado (*Unlit* o *Emissive*) brillante (ej. Rojo anaranjado quemado).
    
### C. Eficiencia Máxima (Costo Computacional Cercano a Cero)
Como todo el cálculo térmico visual se hace por interpolación matemática de color en el fragmento del píxel directamente dentro de la GPU del celular del usuario (HLSL/GLSL), **la vista térmica corre literalmente a la misma velocidad y cantidad de cuadros por segundo (FPS) que la vista normal.** 
Añade cero carga extra a la memoria RAM. Es la solución perfecta y elegante del *Technical Art* moderno para interfaces WebGL inmersivas.
