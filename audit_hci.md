# Auditoría de Interacción Humano-Computadora (HCI) y Psicología Cognitiva

## 1. Evaluación de Rigor Científico
**Veredicto: 🟡 SÓLIDO EN TEORÍA, DÉBIL EN VALIDACIÓN EMPÍRICA.**
El proyecto tiene una base teórica robusta (Sweller, Nielsen, Norman, Hegarty). Sin embargo, existe una disonancia entre el Marco Teórico (que promete medir aprendizaje) y la Metodología (que mide usabilidad).

## 2. Marco Teórico Cognitivo

### A. Teoría de la Carga Cognitiva (Sweller)
*   **Error Crítico:** En `marco_teorico.tex` se afirma que la visualización 3D reduce la *carga intrínseca*.
*   **Corrección Científica:** La carga intrínseca es inherente a la complejidad del material (el dron). No se puede reducir sin simplificar el contenido.
*   **Lo que realmente hace el prototipo:** Reduce la **Carga Extrínseca** (al eliminar la necesidad de rotación mental) y gestiona la intrínseca mediante **Segmentación** (vista explosiva).
*   **Acción:** Corregir redacción en el marco teórico.

### B. Justificación de Vista Explosiva
*   **Evaluación:** Excelente uso de Hutchins (Cognición Distribuida). El argumento de que la cámara orbital es una "prótesis cognitiva" que externaliza la rotación mental es brillante y defendible.

### C. Mejoras Teóricas Sugeridas
*   **Teoría del Aprendizaje Multimedia (Mayer):** Citar el *Principio de Contigüidad Espacial* (hotspots cercanos a la pieza).
*   **Principio de Pre-training (Mayer):** La vista explosiva permite conocer componentes aislados antes de ver el conjunto.

## 3. UX/UI y Usabilidad

### A. Interacción
*   **Evaluación:** Cumple heurísticas de Nielsen. La decisión de bloquear el eje Z (Roll) es correcta según Darken & Sibert.
*   **Riesgo:** Falta definir el "Punto de Pivote" para inspección local.
*   **Sugerencia:** Añadir "Recentrado de cámara al hacer doble click en un hotspot".

### B. Protocolo de Validación (SUS)
*   **Crítica:** El SUS (System Usability Scale) es una herramienta de "higiene". Mide si es fácil de usar, pero **NO** si se aprendió mejor o si bajó la carga cognitiva.
*   **Muestra:** 8-12 usuarios es adecuado para hallar errores de usabilidad (Nielsen), pero **estadísticamente irrelevante** para validar la hipótesis comparativa "3D vs 2D".

## 4. Refinamiento del Protocolo

Para alinear la metodología con la ambición teórica, se recomienda el **Camino de Viabilidad Técnica Reforzada**:

1.  **Mantener SUS:** Para medir usabilidad general.
2.  **Añadir NASA-TLX (Task Load Index):** Cuestionario estándar para medir la carga de trabajo percibida. Esto valida directamente la hipótesis de "Menor Carga Cognitiva" sin necesitar métricas biométricas complejas.
3.  **Reformular Hipótesis:** Cambiar "Menor tasa de error" por "Menor esfuerzo mental percibido (NASA-TLX) y Score SUS > 68".
