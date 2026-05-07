---
tipo: guia
fuente: Informe_final
estado: activo
area: sustentacion
tags:
  - tesis
  - defensa
  - preguntas
---

# INF EST 91 Preguntas Dificiles de Defensa

## 1. El proyecto demuestra que el visor 3D mejora la comprension?

Respuesta sugerida:

No lo plantea como demostracion causal fuerte. El diseno es aplicado, formativo y descriptivo. Busca observar diferencias en desempeno de tareas y workload percibido frente a un soporte 2D, y evaluar la usabilidad del prototipo 3D con SUS.

## 2. Por que NASA-TLX si hablan de carga cognitiva?

Respuesta sugerida:

La teoria de carga cognitiva funciona como marco interpretativo. NASA-TLX no mide directamente carga intrinseca, extrinseca y germana; mide carga de trabajo percibida. Por eso el informe usa el termino workload percibido cuando se refiere al instrumento.

## 3. Por que SUS no se aplica al 2D?

Respuesta sugerida:

Porque SUS evalua la usabilidad de un sistema. El prototipo 3D es el sistema interactivo evaluado. El soporte 2D funciona como condicion de referencia para tareas y workload, pero no como sistema equivalente de interaccion.

## 4. Por que Unity Web si existen Three.js y Babylon.js?

Respuesta sugerida:

Porque el objetivo no era solo mostrar un modelo. Se necesitaba integrar UI, materiales, shaders, seleccion, estados, profiling, tooling y build web. Unity fue una decision de equilibrio entre control tecnico y tiempo de desarrollo.

## 5. Unity Web funciona en movil?

Respuesta sugerida:

Unity Web contempla navegadores compatibles de escritorio y algunos moviles, pero el resultado depende de navegador, version, memoria y optimizacion. Por eso el informe habla de compatibilidad esperada, no de soporte universal.

## 6. El modelo termico es cientifico?

Respuesta sugerida:

Es cientificamente honesto si se presenta como visualizacion heuristica aplicada. No es FEA, no es solver calibrado y no reemplaza medicion experimental. Su funcion es apoyar lectura visual de tendencias relativas.

## 7. Por que hay placeholders en resultados?

Respuesta sugerida:

Porque no se deben inventar resultados antes de la build congelada. Los placeholders muestran la estructura que recibira datos reales, fechados y trazables.

## 8. Que pasa si no llegan a 30 participantes?

Respuesta sugerida:

El estudio sigue siendo valido como evaluacion formativa con 8 a 12 participantes, pero los resultados se reportan de forma descriptiva y exploratoria, sin inferencia poblacional.

## 9. Por que hay funciones ocultas?

Respuesta sugerida:

Porque no todo codigo existente debe formar parte del flujo final visible. El informe distingue entre expuesto en UI final, implementado pero oculto, legado/no integrado y trabajo futuro.

## 10. Por que 28 piezas si hay 257 renderers?

Respuesta sugerida:

Porque son capas distintas. Las 28 piezas son entidades canonicas de investigacion. Los 257 renderers/colliders son fragmentos tecnicos de geometria y runtime. Un componente canonico puede estar formado por varios fragmentos geometricos.

## 11. Por que el onboarding fue hecho por codigo?

Respuesta sugerida:

Porque la ayuda inicial debia mantenerse ligera, actualizable y coherente con la UI real. Un video o GIF aumentaria peso y podria quedar desactualizado si cambian los botones o gestos. Con `Painter2D`, la app dibuja actor, gesto, objetivo y respuesta del sistema en runtime.

## 12. Por que la ficha inferior es tan importante?

Respuesta sugerida:

Porque convierte una seleccion visual en lectura tecnica. El usuario no solo ve una geometria resaltada; entiende nombre, categoria, especificaciones, relacion de ensamblaje y nivel de seleccion. Por eso el `bottom sheet` es el puente entre mirar una pieza y comprender su funcion dentro del sistema.

## 13. Que diferencia hay entre pieza madre, subpieza, grupo de hotspot y fastener?

Respuesta sugerida:

Son niveles de lectura. La pieza madre representa el componente principal; la subpieza permite entrar a un nivel interno; el grupo de hotspot resume una zona o sistema funcional; y el fastener representa un sujetador individual o modular. Separarlos evita que la seleccion, el aislamiento y la ficha de informacion hablen de niveles distintos.
