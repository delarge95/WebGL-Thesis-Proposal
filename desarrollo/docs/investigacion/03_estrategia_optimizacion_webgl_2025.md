# 🔬 Investigación Técnica: Estrategia de Optimización WebGL 2025

**Fecha:** 30 de Noviembre, 2025  
**Autor:** Alexander Woodcock Salomón  
**Proyecto:** Prototipo WebGL Tesis  
**Estado:** Aprobado para Implementación

---

## 1. Resumen Ejecutivo
La optimización para WebGL en 2025 requiere un enfoque híbrido: gestión agresiva de memoria (debido a los límites del navegador) y uso de formatos de compresión modernos (**ASTC**, **Brotli**). El objetivo es un tiempo de carga < 3s (Shell) y estabilidad en dispositivos móviles con 4GB de RAM o menos.

---

## 2. Compresión de Texturas: El Estándar 2025

Las texturas son el mayor consumidor de VRAM y ancho de banda.

### ASTC (Adaptive Scalable Texture Compression)
*   **Por qué:** Es el formato nativo para hardware móvil moderno (iOS/Android) y soportado por la mayoría de navegadores de escritorio modernos vía extensiones WebGL.
*   **Configuración Recomendada:**
    *   **Normal Maps:** ASTC 4x4 o 5x5 (Alta calidad).
    *   **Albedo/Roughness:** ASTC 6x6 o 8x8 (Balance peso/calidad).
    *   **Fallback:** Configurar DXT/BC7 para navegadores de escritorio antiguos si es estrictamente necesario, pero priorizar ASTC para el target móvil.
*   **Crunch Compression:** Usar "Use Crunch Compression" sobre el formato base para reducir el tamaño de descarga (aunque aumenta ligeramente el tiempo de carga por descompresión en CPU).

---

## 3. Gestión de Memoria (The WebGL Heap)

WebGL no tiene acceso directo a toda la RAM del dispositivo; vive dentro de un "Heap" de memoria asignado por el navegador.

### Configuración en Player Settings
1.  **Memory Growth Mode:** `Linear`.
    *   Permite que la aplicación pida más memoria si la necesita, en lugar de reservar un bloque gigante al inicio (que puede fallar en móviles).
2.  **Initial Memory Size:** 256MB.
    *   Mantener bajo para asegurar que la página cargue rápido.
3.  **Maximum Memory Size:** 2048MB.
    *   Límite de seguridad para evitar crasheos del navegador.

### Garbage Collection (GC)
*   **Problema:** En WebGL, el GC puede causar "tirones" (spikes) notables.
*   **Estrategia:**
    *   **Object Pooling:** Obligatorio para cualquier objeto que se cree/destruya (balas, efectos, etiquetas UI).
    *   **Evitar LINQ:** Genera basura (garbage) excesiva.
    *   **String Concatenation:** Usar `StringBuilder` en lugar de `+` en loops.

---

## 4. Asset Delivery: Addressables vs. Asset Bundles

Para cumplir el KPI de "Carga Shell < 3s", no podemos cargar todo el modelo al inicio.

### Estrategia: Addressables
1.  **Grupo "Preload":** UI base, Skybox de baja resolución, Scripts esenciales. (Se descarga con el build).
2.  **Grupo "Drone_Shell":** Modelo Low-Poly del fuselaje y texturas 1K. (Se descarga inmediatamente después del inicio).
3.  **Grupo "Drone_Details":** Motores, interior, texturas 4K. (Se descargan en segundo plano o bajo demanda).

**Configuración de Build:**
*   **Compression:** `Brotli` (Mejor ratio que Gzip). Requiere servidor HTTPS configurado correctamente (GitHub Pages lo soporta).
*   **Code Stripping:** `High`. (Cuidado: usar `link.xml` para preservar clases usadas solo por reflexión, común en DOTween).

---

## 5. Herramientas de Profiling y Debugging

1.  **Unity Memory Profiler:**
    *   Usar para detectar texturas duplicadas o assets no descargados de memoria.
2.  **Chrome DevTools (Performance Tab):**
    *   Monitorizar el uso de memoria del proceso del navegador.
    *   Identificar "Main Thread Stalls" causados por scripts pesados.
3.  **Spector.js:**
    *   Extensión de navegador para capturar frames WebGL.
    *   Útil para ver exactamente qué comandos de dibujo (Draw Calls) se están enviando y en qué orden.

---

## 6. Checklist de Publicación
- [ ] **Strip Engine Code:** Activado.
- [ ] **Data Caching:** Activado (IndexedDB) para cargas rápidas en visitas subsecuentes.
- [ ] **Exceptions:** `None` (o `Explicitly Thrown` solo en debug). `Full` es demasiado lento.
- [ ] **Debug Symbols:** Desactivados en release.

---

## 7. Referencias
*   Unity Documentation: WebGL Memory Management.
*   Khronos Group: ASTC Specification.
*   Herramienta: CrazyGames WebGL Optimizer.
