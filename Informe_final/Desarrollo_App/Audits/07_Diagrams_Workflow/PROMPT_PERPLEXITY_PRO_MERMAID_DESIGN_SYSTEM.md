# Prompt Perplexity Pro - Deep Research Mermaid Design System

Usa este prompt completo en Perplexity Pro:

---

Actua como principal investigador experto en visualizacion de informacion, diagramacion tecnica y diseno de sistemas visuales para documentacion de ingenieria.

Contexto:

- Proyecto academico de tesis tecnica (ingenieria de software + visualizacion WebGL).
- Ya existen 13 diagramas Mermaid (arquitectura, eventos, estados, flujo de seleccion, shaders, datos, termico, restricciones WebGL, despliegue y plantillas de resultados).
- Necesitamos una investigacion exhaustiva para convertirlos en un sistema visual profesional, coherente y altamente legible.

Objetivo principal:

- Entregar una guia de referencia "state of the art" para construir diagramas Mermaid coherentes, consistentes y especialmente esteticos, con un sistema de diseno reusable donde cada color y estilo tenga semantica estable.

Entregables requeridos:

1. Fundamentos de legibilidad para diagramas tecnicos:

- Principios perceptuales (jerarquia, contraste, agrupacion, direccion visual, densidad).
- Buenas practicas de flujo (minimizar cruces, limitar longitudes, ortogonalidad, agrupacion por capas).
- Criterios medibles de calidad visual.

2. Sistema de diseno Mermaid completo:

- Paleta semantica con tokens (rol de color fijo por tipo de nodo/conexion).
- Tipografia, tamanos, espaciado, grosor de borde, estilo de flechas.
- Reglas de uso de colores para: capa UI, orquestacion, servicios, datos, eventos, estados, riesgo/restriccion, mitigacion, resultados.
- Version clara y version oscura del mismo sistema.

3. Estilo de conexiones:

- Estándar para tipo de flecha por semantica (flujo funcional, evento, dependencia auxiliar, validacion, fallback).
- Convenciones de etiquetas de aristas (verbos permitidos, longitud maxima, formato).

4. Patrones por tipo de diagrama:

- Flowchart arquitectonico.
- Maquina de estados.
- Pipeline secuencial.
- Causa -> mitigacion -> resultado.
- Plantillas de resultados (SUS, NASA-TLX, KPIs).

5. Anti-patrones:

- Errores comunes que afectan legibilidad y coherencia.
- Como detectarlos y corregirlos.

6. Guia de implementacion en Mermaid:

- Bloques init recomendados.
- classDef estandar y nomenclatura de clases visuales.
- Snippets listos para copiar.
- Compatibilidad en VS Code Markdown preview (incluyendo limitaciones de parser).

7. Benchmark visual:

- Comparar 3 estilos de referencia (minimal tecnico, elegante academico, corporativo ingenieria).
- Pros y contras de cada uno.
- Recomendar 1 estilo final para tesis con justificacion.

8. Plan de migracion para 13 diagramas existentes:

- Estrategia paso a paso para aplicar el sistema sin romper semantica.
- Checklist de validacion final por diagrama.

Formato de salida obligatorio:

- Secciones claras y accionables.
- Tablas con tokens de color y semantica.
- Reglas concretas tipo "Si X, usar Y".
- Snippets Mermaid listos para uso inmediato.
- Citas y fuentes recientes y confiables (UX, visualizacion, documentacion tecnica, Mermaid).

Profundidad esperada:

- Nivel deep research.
- Exhaustivo, no superficial.
- Orientado a ejecucion real sobre 13 diagramas de tesis.

---

Sugerencia de cierre para Perplexity:
"Al final, dame una Design System Spec v1.0 en formato checklist implementable en menos de 2 horas sobre diagramas Mermaid existentes."
