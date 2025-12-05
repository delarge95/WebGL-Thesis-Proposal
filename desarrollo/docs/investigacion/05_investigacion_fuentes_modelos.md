# 🕵️ Investigación Exhaustiva: Fuentes de Modelos 3D y Problemática de Propiedad Intelectual

**Fecha:** 30 de Noviembre, 2025  
**Autor:** Alexander Woodcock Salomón  
**Contexto:** Selección de Activo Principal para Tesis de Ingeniería

---

## 1. Introducción
Esta investigación tiene como objetivo identificar un modelo 3D (CAD o Poligonal) con la complejidad técnica necesaria para un proyecto de "Optimización WebGL". Se han analizado fuentes gratuitas, de pago y, con fines puramente académicos, canales de distribución no autorizada para comprender la realidad del mercado de activos digitales.

---

## 2. Análisis Académico: La Problemática de la Piratería de Activos 3D
En el mercado de Tech Art, la "piratería de modelos" es una realidad que afecta la sostenibilidad de la industria.

### 2.1. Canales de Distribución "Dudosa" (Grey Market)
La investigación identifica que modelos de alta gama (normalmente de $100-$300 USD) circulan en:
*   **Canales de Telegram:** Grupos privados donde se comparten "leaks" de assets de tiendas como CGTrader o Unreal Marketplace.
*   **Foros "Warez" 3D:** Sitios (ej. *GFXPeers*, *CGPersia*) que operan bajo la premisa de "compartir para estudiar", pero infringen derechos de explotación comercial.
*   **Riesgos Académicos y Profesionales:**
    *   **Legal:** El uso de estos assets en un producto comercial o tesis pública puede derivar en demandas por DMCA.
    *   **Técnico:** Los archivos suelen estar corruptos, desactualizados o contener malware en los scripts adjuntos.
    *   **Ético:** Desincentiva a los creadores originales (Ingenieros y Artistas 3D) de seguir publicando material de calidad.

**Postura del Proyecto:** Se reconoce la existencia de estas fuentes como una problemática de mercado, pero se rechaza su uso. **Todos los assets del proyecto serán licenciados legalmente** para garantizar la validez ética y legal del título de ingeniería.

---

## 3. Tabla Comparativa de Opciones de Modelos

A continuación, se presentan las opciones viables encontradas, clasificadas por su origen y legitimidad.

| Nombre del Modelo | Fuente | Tipo | Precio | Detalles Técnicos | Veredicto |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **DJI Mavic 3 Cine** | **CGTrader** | **Pago (Oficial)** | ~$49 USD | Alta fidelidad, topología limpia, UVs listos. Ideal para render, requiere optimización masiva. | **Recomendado (Calidad/Precio)** |
| **Generic Quadcopter** | **GrabCAD** | **Gratuito (Comunidad)** | $0 | Formato STEP/IGES (CAD Puro). Geometría NURBS perfecta, pero sin UVs ni materiales. | **Excelente para Tesis (Reto Técnico)** |
| **Sci-Fi Drone Kitbash** | **ArtStation** | **Pago (Indie)** | ~$15 USD | Modelo conceptual. Topología mid-poly. Bueno para juegos, malo para "Ingeniería Inversa". | Descartado (Poco rigor técnico) |
| **"Leaked" DJI Model** | *Telegram/Foros* | *Dudosa (Piratería)* | *$0 (Ilegal)* | Archivo de origen desconocido. Posiblemente rip de un juego o scan de baja calidad. | **DESCARTADO (Riesgo Legal)** |
| **Pixhawk Reference** | **GitHub** | **Open Source** | $0 | Archivos de PCB y carcasa reales. Máxima fidelidad de ingeniería electrónica. | **Excelente para Componentes Internos** |

---

## 4. Análisis Detallado de Opciones Seleccionadas

### Opción A: GrabCAD (La Ruta del Ingeniero)
*   **Ventaja:** Los archivos vienen en formatos de ingeniería real (.STEP, .SLDPRT). Al importarlos a Blender/Unity, la malla es "perfecta" matemáticamente, pero horrible poligonalmente (millones de triángulos).
*   **Valor de Tesis:** El proceso de convertir un archivo STEP a un activo WebGL optimizado es un **caso de éxito técnico** impresionante para un portafolio.

### Opción B: CGTrader (La Ruta del Artista)
*   **Ventaja:** El modelo ya "parece" un modelo 3D estándar. Ahorra tiempo en limpieza inicial.
*   **Desventaja:** Puede que la topología no sea apta para deformación o animación compleja sin trabajo previo.

---

## 5. Recomendación Final

Para maximizar el valor académico y de portafolio, se sugiere:

1.  **Fuente Principal:** Descargar un modelo **CAD Gratuito de GrabCAD** (ej. un diseño open source complejo). Esto justifica el capítulo de "Conversión de Datos CAD".
2.  **Componentes Internos:** Usar referencias de **Pixhawk (Open Source)** para modelar las placas electrónicas con fidelidad.
3.  **Licenciamiento:** Al usar fuentes Open Source (CC BY-SA) y GrabCAD (Community Free), el proyecto es 100% legal y publicable sin costes adicionales.

**Decisión:** Proceder con la búsqueda de un modelo específico en **GrabCAD** que tenga licencia abierta.
