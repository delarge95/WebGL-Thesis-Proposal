# 🚁 Investigación y Análisis: Familia Holybro (S500, X500 V2, X650)

**Fecha:** 25 de Febrero, 2026
**Propósito:** Análisis de hardware de alto rendimiento para modelado y visualización WebGL con características de despiece (Exploded View).

A continuación se detalla una investigación exhaustiva de las tres variantes principales de la familia de desarrollo Holybro, orientada a su aplicación como "Gemelos Digitales" en el entorno de la tesis.

---

## 1. Análisis Comparativo de Modelos

### A. Holybro S500 V2 (El Estándar Académico)
El S500 V2 es la plataforma de desarrollo más accesible, ampliamente usada en universidades y laboratorios de investigación (investigación de visión artificial, enjambres).

*   **Chasis y Dimensiones:** Wheelbase (distancia entre ejes) de 480mm. Peso del kit $\sim 782g$.
*   **Materiales Estructurales:**
    *   **Brazos:** Compuesto de Poliamida-Nylon ultra resistente, *reforzado internamente con una varilla de fibra de carbono*. Tienen un diseño de "Ala" (inclinación) para mejorar la aerodinámica.
    *   **Platos (Top/Bottom):** Fibra de carbono de 1.5mm.
    *   **Tren de Aterrizaje:** Tubos de fibra de carbono (16mm y 10mm) unidos por T-connectors de plástico reforzado.
*   **Despiece Electrónico (Componentes Reales a Modelar):**
    *   Power Distribution Board (PDB) integrada (100A Burst).
    *   Pixhawk 4, Pixhawk 6C o Pix32.
    *   Módulo M10 GPS (con su mástil característico).
    *   Radio de Telemetría Holybro SiK (433/915 MHz).
    *   Motores 2216 KV920 y ESCs BLHeli S 20A.
*   **Veredicto 3D:** Es el **más fácil de modelar** de forma optimizada. Los brazos de nylon inyectado tienen formas geométricas simples pero interesantes visualmente. La topología puede reducirse mucho, ahorrando *tris* para WebGL.

### B. Holybro X500 V2 (El Caballo de Batalla Profesional)
Es una versión más robusta y "cuadrada" del S500, orientado a integración de cargas útiles (cámaras de profundidad, companion computers).

*   **Chasis y Dimensiones:** Wheelbase 500mm. Marco central 144x144mm.
*   **Materiales Estructurales:**
    *   **Brazos:** Tubos redondos 100% de **Fibra de Carbono** (16mm).
    *   **Soportes/Conectores:** Nylon reforzado con fibra (conectores de motor al tubo).
    *   **Platos (Top/Bottom):** Fibra de carbono pura de 2mm.
*   **Despiece Estructural Destacado:** Su principal característica es un sistema de rieles gemelos inferiores (dos varillas de 10mm x 250mm) para montar baterías pesadas y payloads.
*   **Veredicto 3D:** Técnicamente se ve más "Industrial" que el S500. La estética de tubos redondos de carbono texturizados en PBR es muy atractiva para un portafolio de *Technical Art*, mostrando luces especulares cilíndricas.

### C. Holybro X650 (Heavy Lifter / Desarrollo Industrial)
La versión grande, diseñada para levantar hasta 4.3kg de carga adicional.

*   **Chasis y Dimensiones:** Wheelbase enorme de 650mm.
*   **Materiales Estructurales:**
    *   **Brazos:** Tubos gruesos de **fibra de carbono de 20mm**.
    *   **Monturas Plegables:** Su gran diferenciador físico son **soportes de Aluminio** mecanizado CNC que permiten plegar los brazos.
*   **Despiece Electrónico Destacado:** Usa motores masivos (T-Motor MN4014 330KV), ESCs Tekko32 F4 45A, y placas de vuelo de última generación como Pixhawk 6X. Viene con rieles de 320mm.
*   **Veredicto 3D:** Es masivo. Replicar los soportes de aluminio mecanizado añadirá una riqueza visual increíble (contraste entre carbono mate y metal brillante), **pero** aumentará el número de polígonos drásticamente debido a las bisagras y juntas.

---

## 2. Recomendación Definitiva para la Tesis: **Holybro X500 V2**

Tras contrastar los tres modelos contra los *KPIs* de tu tesis (Carga cognitiva y optimización de WebGL $<$ 50,000 tris), **el Holybro X500 V2 es el ganador indiscutible.**

**Razones:**
1.  **Dificultad vs Impacto Visual:** Modelar los tubos redondos de fibra de carbono y monturas de nylon plano del **X500 V2** es más eficiente poligonalmente que el complejo aluminio plegable del X650 o las aspas curvas del S500.
2.  **Topología Educativa Perfecta:** Tiene espacio entre platos, permitiendo una "Vista Explosionada" (Exploded View) limpia y espaciada donde puedes separar: (1) Riles y Batería, (2) Plato Inferior (PDB), (3) Brazos tubulares, (4) Top Plate + Flight Controller + GPS Mástil.
3.  **Legalidad y Precisión:** Al ser hardware de plataforma abierta, sus dimensiones y manuales están disponibles públicamente, cumpliendo al 100% el carácter académico del trabajo.

---

## 3. Evaluación: ¿Vista de Calor / Térmica en el Visor?

Preguntas si es relevante añadir una **Vista de Calor (Thermal/Heatmap)**.

**Respuesta Rápida:** SÍ. Es una característica de alto nivel (*Killer Feature*) para la propuesta de Ingeniería Multimedia que elevará la nota de la tesis.

**Justificación Académica e Industrial:**
1.  **Alineación con el Propósito:** En HW de alto rendimiento, el sobrecalentamiento de motores y ESCs limita el vuelo. Evaluar el calor es diagnóstico técnico real.
2.  **Valor de Technical Art (Shaders):** Demostrar la habilidad de cambiar a un modo "Térmico" prueba tu dominio de Unity. En tu UI (`MainLayout.uxml`, línea 208) **ya tienes implementado el botón de "THERMAL"**.
3.  **Implementación WebGL (Rendimiento):** Una vista térmica **NO** requiere mallas adicionales ni texturas pesadas. En términos de Shader Graph (Technical Art), requiere implementar una rampa de color interpolada por valores asignados a cada material (Ej: Asignar un valor térmico de $95^\circ$ C al motor $\rightarrow$ Color Rojo, $30^\circ$ C al chasis $\rightarrow$ Color Azul oscuro) que es extremadamente ligero ($< 1ms$ de tiempo de cómputo en la GPU). 
4.  **Carga Cognitiva:** Usar colores rojo/azul para temperatura es un "Affordance" psicológico (asociación intuitiva) que reduce esfuerzo mental; encaja perfecto en tu marco teórico.

**Integración en datos existentes:**
Si te fijas en tu `DroneAssembler.cs`, tú mismo ya preparaste el terreno para esto:
```csharp
CreatePart(..., powerConsumption: 180f, operatingTemp: 65f, ...) // Las variables YA están ahí.
```
La creación del *Thermal Shader* tomará esa variable real de la ficha técnica y la pintará sobre el modelo. Es la demostración máxima de que no es solo un modelo 3D bonito, sino un gemelo digital guiado por datos (Data-Driven Digital Twin).
