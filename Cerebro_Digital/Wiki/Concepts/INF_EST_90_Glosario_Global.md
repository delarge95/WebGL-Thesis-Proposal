---
tipo: glosario
fuente: Informe_final
estado: activo
area: sustentacion
tags:
  - tesis
  - glosario
  - conceptos
---

# INF EST 90 Glosario Global

## Hardware complejo

Sistema con muchas piezas y subsistemas relacionados: estructura, energia, electronica y control. En el proyecto, el Holybro X500 V2 funciona como caso de estudio de hardware complejo.

## CAD

Diseno asistido por computador. Son modelos creados para ingenieria o manufactura, muchas veces demasiado pesados para usarse directamente en tiempo real.

## Pipeline

Cadena de pasos que transforma un insumo en un resultado. Aqui: CAD -> limpieza -> optimizacion -> materiales -> Unity -> WebGL.

## Technical Art

Disciplina puente entre arte, programacion y optimizacion. En este proyecto permite convertir modelos tecnicos en assets usables dentro de una app web interactiva.

## WebGL

Tecnologia del navegador que permite renderizado 3D acelerado por GPU.

## WebAssembly

Formato binario para ejecutar codigo compilado dentro del navegador. Unity lo usa en builds web para ejecutar la logica del proyecto.

## Unity Web

Plataforma de Unity para publicar aplicaciones en navegador. Usa WebGL y WebAssembly, con compatibilidad dependiente del navegador, dispositivo y optimizacion.

## IL2CPP

Cadena de Unity que convierte codigo administrado hacia C++ como paso intermedio dentro del proceso de compilacion. En target web, ese flujo termina en WebAssembly.

## PBR

Renderizado basado en fisica. Modelo que representa materiales con reglas coherentes de luz, color, rugosidad y reflexion.

## BRDF

Funcion matematica que describe como una superficie refleja luz segun angulo de entrada, angulo de salida y propiedades del material.

## Shader

Programa que corre en GPU y define como se ve una superficie o efecto visual.

## Blueprint

Modo visual analitico que representa el modelo con lectura similar a plano tecnico, priorizando contornos, contraste y forma sobre realismo material.

## Draw call

Orden de dibujo enviada a la GPU. Muchas draw calls pueden reducir rendimiento.

## Frame time

Tiempo que tarda el sistema en generar un cuadro. Para 30 FPS, el frame time debe estar cerca o por debajo de 33.33 ms.

## FPS

Cuadros por segundo. Indica fluidez visual. En este proyecto, 30 FPS es una meta operativa minima.

## NASA-TLX Raw

Instrumento que estima carga de trabajo percibida mediante seis dimensiones. Raw significa que se usa promedio simple sin ponderacion pareada.

## SUS

System Usability Scale. Cuestionario de 10 items que estima usabilidad percibida de un sistema.

## Think-Aloud

Metodo donde el participante verbaliza lo que piensa durante la tarea. Ayuda a explicar dudas, errores y estrategias de uso.

## Onboarding

Guia inicial integrada en la app. En este proyecto se dibuja por codigo para explicar gestos, seleccion, paneles y modos sin depender de videos o GIFs.

## Painter2D

API de UI Toolkit usada para dibujar formas vectoriales en runtime. Se emplea en iconos procedurales y demos de onboarding.

## Bottom sheet

Panel inferior desplegable que muestra informacion contextual de la seleccion sin cubrir por completo el modelo 3D.

## Info panel

Vista de informacion de la pieza o grupo seleccionado. En la app se materializa como ficha inferior con identificacion, especificaciones y datos de ensamblaje.

## Pieza madre

Componente principal al que pertenece una seleccion. Puede contener subpiezas o fasteners asociados.

## Subpieza

Nivel interno de una pieza madre. Permite una lectura mas especifica del ensamblaje sin cambiar la taxonomia canonica completa.

## Hotspot

Marcador visual sobre el modelo que guia al usuario hacia un punto o grupo funcional de interes.

## Fastener

Sujetador o tornillo. En el proyecto se trata con catalogos y familias modulares para reducir geometria repetida y permitir inspeccion bajo demanda.

## DSR

Design Science Research. Enfoque de investigacion usado para construir y evaluar artefactos que resuelven problemas practicos.

## MVP

Producto minimo viable. Version suficiente para probar el concepto principal sin incluir todas las funciones imaginables.

## Build congelada

Version estable del producto usada para medir y reportar resultados. Evita mezclar datos de versiones distintas.
