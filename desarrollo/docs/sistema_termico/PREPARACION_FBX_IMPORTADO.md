# Preparacion del FBX importado

## Objetivo

Dejar `x500v2_Drone` listo para selection, isolate, filter, explode, cut y thermal sin editar a mano decenas de objetos en la jerarquia.

## Flujo oficial

1. Ejecutar `Tools > Import Drone Model Into Scene`.
2. Ejecutar `Tools > Thermal > Prepare Imported Drone For Thermal Test`.
3. Guardar la escena.
4. Entrar en Play Mode.
5. Validar el flujo funcional completo.

## Que hace la preparacion

- Genera o actualiza `DronePartData` canonicos desde `x500v2_parts_data.json`.
- Crea o reutiliza anchors canonicos para las 28 piezas del solver.
- Reagrupa subpiezas y auxiliares bajo el anchor correcto.
- Anade `ExplodablePart`, `MaterialController` y `HighlightSystem`.
- Anade colliders seleccionables por renderer.
- Marca la jerarquia como `SelectablePart`.
- Clasifica renderers auxiliares con `PartRenderCategory`.
- Distingue `Fasteners` y `Misc` como categorias visuales publicas.
- Anade `ImportedDroneRuntimeBinder` al root para recachear managers en runtime.

## Regla de categorias

- `Structure`, `Propulsion` y `Electronics` siguen siendo las categorias canonicas de pieza.
- `Fasteners` y `Misc` son categorias visuales auxiliares para filter e isolate de grupo.
- `Fasteners` y `Misc` no son nodos termicos independientes en la V1.
- En thermal heredan la temperatura del ensamblaje padre.

## Checklist despues de preparar

Verificar en la jerarquia:

- existe `x500v2_Drone`
- existe `ImportedDroneRuntimeBinder` en el root
- existen anchors con `ExplodablePart`
- los hijos importados quedaron reagrupados bajo anchors canonicos
- los renderers tienen `PartRenderCategory`
- los colliders de seleccion existen en la geometria relevante

## Checklist funcional en Play Mode

### Inspect

- `POWER` cambia de estado visual
- aparece el panel de carga
- el slider mueve la carga
- el dron pasa por `OFF`, `STARTING`, `IDLE`, `FLYING`

### Analyze

- `CUT` recorta el dron completo
- `EXPLODE` separa ensamblajes sin sacar props de pantalla
- `FILTER` responde a `ALL`, `STRUCTURE`, `PROPULSION`, `ELECTRONICS`, `FASTENERS`, `MISC`
- `ISOLATE` opera sobre la pieza canonica completa

### Studio

- `THERMAL` muestra la leyenda con gradiente real
- las piezas canonicas y sus hijos cambian de color
- el calentamiento responde al estado de energia y a la carga
- `BLUEPRINT` mantiene lectura limpia a distancia

## Cuando repetir la preparacion

Repetir `Prepare Imported Drone For Thermal Test` si:

- se reimporto el FBX
- se renombraron piezas
- se cambio la jerarquia
- se agregaron o quitaron subcomponentes
- se actualizo el dataset canonico

## Notas

- El flujo CAD bruto sigue siendo experimental y no reemplaza esta ruta.
- La escena oficial de validacion es `MainScene_Final.unity`.
- El solver oficial sigue operando sobre 28 piezas canonicas.
