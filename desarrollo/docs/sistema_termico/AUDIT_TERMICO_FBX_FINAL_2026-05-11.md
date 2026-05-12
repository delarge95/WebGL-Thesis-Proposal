---
tipo: audit
estado: implementado_pendiente_validacion_visual
fecha: 2026-05-11
---
# Audit termico del FBX final importado

## Contexto

El FBX final fue importado en Unity con `252` meshes, escala dominante `10u -> 10u`, `4` helices normalizadas, `161` fasteners normalizados y audit de escena `0 errores / 0 warnings`.

La revision se enfoco en el comportamiento termico observado en Play Mode: poca transferencia visible en frames y plates, `TOP-PLATE-X500-V5.001_low` sin calentamiento perceptible en zonas de contacto y `MIANKE-PIXHAWK6C-LV-C1.001_low` visualmente mas caliente de lo esperado.

## Hallazgos

1. **Grafo canonico con nodos ausentes en el FBX final**:
   - El grafo `ThermalCanonicalContactGraph.asset` conserva enlaces hacia piezas canonicas documentadas pero no presentes como geometria runtime actual, por ejemplo `x500v2_pdb`, `x500v2_esc_*` y `x500v2_rc_receiver`.
   - Cuando un extremo del enlace no existe en la escena, el solver descartaba el enlace. Esto podia cortar rutas de conduccion utiles hacia plates, rails o stack central.
2. **Visualizacion demasiado canonica para subpiezas granulares**:
   - Los renderers bajo una misma pieza canonica heredaban la misma temperatura visual.
   - En Pixhawk, subpiezas tipo tapa/carcasa como `MIANKE-PIXHAWK6C-LV-C1` se veian tan calientes como la electronica interna aunque no representen el principal generador termico.
3. **Placas tratadas como superficies uniformes**:
   - `top_plate`, `bottom_plate` y `platform_board` podian terminar con patron uniforme.
   - Con una variacion promedio baja, el usuario no percibia calentamiento localizado en zonas de contacto.
4. **Hotspot visual dependiente de un renderer parcial**:
   - Para ubicar el foco de transferencia, el sistema podia tomar el centro de un renderer individual en lugar de los bounds completos de la pieza canonica.
   - Con el FBX granular, esto vuelve inestable la lectura espacial del calor.

## Correcciones implementadas

1. **Puentes termicos para nodos canonicos ausentes**:
   - Si un nodo documentado del grafo no existe en runtime, el solver conecta sus vecinos presentes con una conductancia reducida.
   - La intencion es conservar rutas fisicas aproximadas sin recrear proxies visuales que ya fueron eliminados del FBX final.
2. **Enlaces suplementarios para el runtime final**:
   - Se agregaron enlaces directos conservadores entre power module, Pixhawk, platform board, top plate, bottom plate, rails y landing gear.
   - Estos enlaces complementan el grafo canonico cuando la geometria final no contiene todas las piezas intermedias de la version documental.
3. **Temperatura visual por subpieza**:
   - La temperatura fisica del solver sigue siendo canonica, pero la visualizacion aplica escala por subpieza.
   - `MIANKE` y `DIKE` de Pixhawk se atenuan frente a PCB/IMU; conectores y gomas tambien tienen escala visual menor.
4. **Placas con patron radial y hotspot dinamico**:
   - Las plates y carriers estructurales usan un foco visual basado en el vecino conectado mas caliente.
   - El efecto afecta la lectura visual del shader, no altera la energia del solver.
5. **Bounds canonicos completos para hotspots**:
   - `ThermalViewController` ahora cachea bounds agregados por pieza canonica.
   - El foco de contacto se calcula con la pieza completa y no con el primer renderer encontrado.

## Limitaciones vigentes

- El sistema sigue siendo un modelo reducido cualitativo, no FEA ni CFD.
- Los refuerzos visuales de contacto no son medicion experimental; solo mejoran la correspondencia perceptual con zonas de conduccion esperables.
- La validacion final debe hacerse en Play Mode con el dron encendido, carga entre `60%` y `100%`, modo `Thermal` activo y comparacion visual entre motores, arms, plates, Pixhawk y power module.

## Checklist de aceptacion visual

- Los motores deben seguir siendo las fuentes mas calientes bajo carga.
- Arms y frames deben calentarse de forma gradual, no instantanea ni completamente uniforme.
- `TOP-PLATE-X500-V5.001_low` debe mostrar calentamiento localizado cerca del stack central o vecinos calientes.
- `MIANKE-PIXHAWK6C-LV-C1.001_low` debe verse menos caliente que subpiezas electronicas internas como PCB/IMU.
- Las piezas de goma/foam no deben aparecer como fuentes termicas intensas salvo por contacto visual leve.
- La vista termica no debe depender de proxies eliminados del FBX final.
