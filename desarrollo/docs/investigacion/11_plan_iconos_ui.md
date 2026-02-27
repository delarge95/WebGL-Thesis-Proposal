# Planificación de Iconos UI con Microinteracciones

**Objetivo:** Desarrollar un sistema de iconografía moderno, minimalista y con microinteracciones "coquetas pero limpias" para la plataforma interactiva WebGL (usando Unity UI Toolkit).

---

## 1. El Formato: ¿SVG o PNG?

**En Unity 6 (UI Toolkit), el SVG es el rey absoluto para UI Moderna.**
Atrás quedaron los días de importar hojas de textura (Sprite Atlases) gigantes en PNG. UI Toolkit (basado en estándares web USS/UXML) soporta **Vector Image assets genéricos**.

*   **¿Por qué SVG?** Al usar Vectores (*VectorGraphics API* nativa de Unity UI Toolkit), el ícono jamás se pixela, sin importar si el usuario abre la aplicación en un teléfono de 5 pulgadas o un monitor 4K. Además, pesan meros kilobytes, crucial para WebGL.
*   **¿En qué formato se animan las microinteracciones?** La animación **no se hace en un archivo de video, GIF ni hojas de sprites**. Los SVGs son estáticos. Las interacciones de *Hover* o *Click* se programan directamente alterando sus propiedades vía CSS (bueno, USS en Unity) o C#.

---

## 2. El Workflow Ideal (Paso a Paso)

Tu pregunta es clave: ¿Cuál es el mejor approach? La respuesta definitiva para un flujo *Technical Art / UI Engineer* es **generar SVGs estáticos y darles vida en el motor:**

1.  **Iteración 1: Concepto Estático (Figma / Illustrator / Generación).** 
    *   Diseñas o generas el set de íconos en blanco y negro (outline/línea fina) de forma estática y minimalista.
    *   Se exportan como pura data geométrica `.SVG`.
2.  **Iteración 2: Importación a Unity (VectorImage).**
    *   Arrastras el SVG a Unity. En el Inspector, lo configuras como `VectorImage` (No como `Sprite`).
    *   Lo asignas como el *Background Image* del elemento VisualElement/Button en tu `MainLayout.uxml`.
3.  **Iteración 3: Microinteracciones vía USS (Estilos) y C#.**
    *   A través de USS Transition (CSS nativo de Unity), le dices al ícono que cuando el mouse pase por encima (`:hover`), escale a `1.1` durante `0.2s` con interpolación `ease-out-bounce`.
    *   Si usas C# y el Experimental `UnityEngine.UIElements.Experimental.ITransitionAnimations`, puedes rotar elementos o cambiar el *tint color* (color del trazo) lentamente.

Con este workflow, garantizas **Peso Cero**, **Calidad 4K Infinita**, y unas interacciones súper fluidas que no cargan el render loop del juego 3D de fondo.

---

## 3. Planificación de Íconos (Inventario Base)

Analizando tu estructura de UI (de *MainLayout.uxml*), aquí está la planeación del set principal de botones e íconos necesarios, con su temática visual de estilo moderno/minimalista y comportamiento:

| ID del Botón (Funcionalidad) | Metáfora Visual (Icono Base) | Microinteracción Propuesta (Hover / Clic) |
| :--- | :--- | :--- |
| **1. Despiece (*Explode*)** | Tres capas cuadradas isométricas separadas por líneas (estilo "stack"). | **Hover:** Las capas se separan ligeramente 2 píxeles en el eje Y (rebote sutil). |
| **2. Visión Térmica (*Thermal*)** | Una llama minimalista estilizada o la forma central de un ojo con un gradiente incrustado. | **Hover:** El trazo blanco se tiñe lentamente de un gradiente naranjal/rojo brillante y palpita al 105% de escala una vez. |
| **3. Aislamiento (*Isolate*)** | Un cuadrado o cubo en el centro irradiando ondas (o una mirilla de enfoque selectivo). | **Hover:** Un marco exterior rota sutilmente o las "ondas" se expanden y se desvanecen. |
| **4. Información (*Data / Specs*)** | Una 'i' moderna dentro de un anillo muy fino o una lista de check diminuta. | **Hover:** Rota el anillo 15 grados, o el punto superior de la "i" salta sutilmente. |
| **5. Play/Assemble (*Ensamblar*)** | Símbolo estándar de Play (Triángulo redondeado) con línea limpia (line art). | **Hover:** Se rellena suavemente el interior (de *lineart* a *solido*) usando la propiedad de `Unity-background-image-tint-color` USS. |
| **6. Navegación (Chevrons)** | Flechas minimalistas (`<` `>`) sin cuerpo, solo el ángulo. | **Hover:** Se deslizan 2 píxeles en la dirección correspondiente de inmediato y regresan suavemente (ease-out). |
| **7. Menú Hamburguesa** | Tres líneas horizontales de esquinas perfectamente redondeadas con mucho espaciado. | **Hover:** Las líneas se separan ligeramente. **Clic:** Animación C# de transición a "X" cruzada. |

**Conclusión del Estilo:** Todo debe ser *Line Art* (Iconos huecos de líneas finas, ideal 1.5px de grosor de trazo constante), en blanco puro con cierta transparencia (alpha 80%). Al interactuar, rellenarse o crecer es la clave del diseño de Apple HIG y Material 3.
