---
tipo: modulo_estudio
fuente: Informe_final/chapters/04_desarrollo.tex
estado: activo
area: desarrollo
tags:
  - tesis
  - pipeline-cad
  - mad-t
  - blender
  - unity
---

# INF EST 30 Pipeline CAD, MAD-T y Saneamiento Geometrico

## Idea central

El pipeline CAD no fue una tarea de conversion automatica. Fue un proceso de traduccion tecnica: tomar informacion de ingenieria, pesada y pensada para manufactura, y convertirla en geometria explicable, navegable y viable para WebGL.

La pregunta de fondo es: que se debe preservar para que el dron siga siendo reconocible y tecnicamente util, y que se debe simplificar para que funcione en navegador.

## Proceso completo explicado

### 1. Lectura del CAD

El modelo CAD original sirve como fuente de forma y ensamblaje, pero no necesariamente como asset final. CAD suele priorizar precision de manufactura; WebGL prioriza lectura visual, bajo peso y rendimiento.

### 2. Evaluacion de ruta de entrada

El proyecto considero varias rutas:

- STEP directo a Blender para inspeccion rapida.
- STEP -> MoI3D -> Blender para controlar mejor la teselacion.
- Remodelado sobre referencia CAD cuando la malla no era reusable.
- Sustitucion modular para piezas repetidas como tornillos.

### 3. Decision por pieza

Cada pieza se clasifica segun su utilidad:

- usable con limpieza;
- usable solo como referencia;
- rehecha por completo;
- modelada desde cero;
- reemplazada por modulo repetible.

### 4. Limpieza y reconstruccion

Aqui entran operaciones como:

- eliminar geometria invisible;
- corregir caras problematicas;
- reducir densidad innecesaria;
- separar piezas semanticas;
- preparar materiales;
- asegurar pivotes y jerarquias utiles.

### 5. Salida a Unity

El objetivo final no es una malla bonita en Blender. El objetivo es que Unity pueda:

- renderizarla;
- seleccionarla;
- vincularla con datos;
- aislarla;
- filtrarla;
- mostrarla en modos visuales;
- auditarla.

## MAD-T adaptado

MAD-T se toma como una guia de produccion hard-surface:

- Modeling: reconstruir o modelar piezas.
- Automation: automatizar repeticion.
- Decimation: reducir complejidad.
- Triangulation: cerrar geometria para motor.

No es la metodologia cientifica del proyecto. La metodologia cientifica es DSR. MAD-T es un marco tecnico de produccion.

## Sistema modular de fasteners

Los tornillos son un problema clasico: visualmente parecen pequenos, pero pueden multiplicar vertices, piezas y costo de mantenimiento.

La solucion modular agrupa:

- 20 familias;
- 168 instancias;
- 9 entradas de reconciliacion.

Esto permite que cada tornillo no sea tratado como pieza academica independiente, sino como instancia de una familia tecnica.

## Explicaciones complejas

### Teselacion

CAD representa muchas superficies como NURBS o superficies matematicas. Para renderizar en tiempo real, esas superficies deben convertirse en triangulos. Ese proceso es la teselacion.

Si la teselacion es demasiado fina, el modelo queda pesado. Si es demasiado gruesa, pierde forma. El pipeline busca un equilibrio.

Modelo mental:

```text
calidad_runtime = legibilidad_visual / costo_geometrico
```

No es una ecuacion fisica, sino una regla de decision. Una pieza puede tener mucha precision CAD y aun asi ser mala para WebGL si su costo geometrico no mejora la lectura del usuario.

Ejemplo del proyecto:

Un borde redondeado pequeno puede ser importante en manufactura, pero no necesariamente cambia la comprension del ensamblaje. Si ese borde multiplica triangulos en decenas de tornillos, conviene simplificarlo o reemplazarlo por una familia modular.

### Decimation

Decimation reduce poligonos. No siempre es suficiente porque puede romper bordes, curvas o piezas tecnicas. Por eso a veces es mejor rehacer una pieza que solo simplificarla automaticamente.

Decision practica:

```text
si decimation rompe silueta o seleccion -> reconstruir pieza
si decimation conserva silueta y reduce costo -> aceptar con revision
```

La silueta es critica porque el usuario reconoce piezas por contorno, proporcion y ubicacion. Un ahorro de triangulos que destruye lectura tecnica no es optimizacion; es degradacion.

### Triangulacion

La GPU termina dibujando triangulos. Triangular antes de exportar evita que cada software decida de forma distinta como partir caras complejas. Esto mejora previsibilidad.

### Por que no conservar todo el CAD

Porque el CAD contiene detalle de manufactura que no siempre aporta a la inspeccion web. El criterio no es conservarlo todo, sino conservar lo que sostiene reconocimiento, lectura espacial y trazabilidad tecnica.

### Sistema modular de tornillos explicado

Los tornillos tienen alto riesgo de "costo invisible": cada uno parece pequeno, pero cientos de instancias pueden sumar geometria, materiales, colliders y mantenimiento.

Modelo de ahorro:

```text
costo_total_sin_modularidad = numero_instancias * costo_tornillo_unico
costo_total_modular = costo_familia + numero_instancias * costo_instancia
```

Lectura:

- `costo_tornillo_unico`: cada tornillo se guarda como asset independiente.
- `costo_familia`: se crea una geometria base reutilizable.
- `costo_instancia`: cada copia conserva transformacion, ubicacion y metadatos con menor duplicacion.

En defensa:

> La modularidad permite preservar la lectura de tornilleria sin tratar cada tornillo como una pieza academica independiente.

## Terminos importantes

- CAD: modelo tecnico de ingenieria o manufactura.
- STEP: formato usado para intercambiar datos CAD.
- MoI3D: herramienta usada para convertir y controlar salida poligonal desde CAD.
- Teselacion: conversion de superficies CAD a poligonos.
- Retopologia: reconstruccion de geometria con mejor flujo y menor peso.
- Decimation: reduccion automatica o semiautomatica de poligonos.
- Triangulacion: conversion final de caras a triangulos.
- Proxy: geometria ligera que reemplaza una pieza pesada.
- Fastener: tornillo, tuerca, arandela o sujetador.
- Instancia: copia de una familia geometrica reutilizada.
- Reconciliacion: correspondencia entre nombres, familias, escena y datos.

## Preguntas dificiles de defensa

### Por que algunas piezas se rehacen en vez de importarse?

Porque importar no garantiza una malla apta para tiempo real. Si la malla CAD genera geometria sucia, pesada o dificil de seleccionar, rehacerla puede producir un asset mas estable y defendible.

### No se pierde precision al optimizar?

Se pierde detalle geométrico no esencial, pero se preserva legibilidad tecnica. El proyecto no pretende manufacturar desde el visor; pretende inspeccionar y comprender el ensamblaje.

### Como se justifica el sistema modular de tornillos?

Por repeticion y costo. Muchas instancias comparten forma y funcion. Agruparlas por familia reduce geometria, mejora mantenimiento y conserva metadatos utiles.

## Fuentes relacionadas

- [[INF_EST_04_Desarrollo_Implementacion]]
- [[Optimizacion_CAD_WebGL]]
- [[Pipeline_Modelado_Dron]]
- [[Investigacion_Holybro_X500v2]]
- `desarrollo/docs/investigacion/Holybro/CAD-to-Unity WebGL Optimization  Complete Technical Blueprint for Drone Visualization.md`
- `desarrollo/docs/investigacion/Holybro/CAD_Fastener_Optimization_Plan.md`
