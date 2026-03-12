# Matriz de Actualizacion Documental del Sistema Termico

## Objetivo

Definir que documentos deben actualizarse, con que frecuencia y bajo que criterio de evidencia cada vez que se avance en la funcionalidad de simulacion termica del dron.

## Reglas Generales

- No registrar como final una funcionalidad que aun este en fase de cimiento, prototipo o validacion parcial.
- No escribir metricas, rendimiento o resultados experimentales sin datos reales medidos.
- Diferenciar siempre entre `implementado`, `integrado en UI`, `en validacion` y `proyectado`.
- Toda ecuacion, conversion o constante relevante para el sistema termico debe pasar por el workflow de WolframAlpha antes de fijarse en codigo o documentacion tecnica.

## Actualizacion Progresiva por Iteracion

### Siempre actualizar cuando haya avance tecnico real

- `Informe_final/Desarrollo_App/BITACORA.md`
- `Informe_final/Desarrollo_App/CHANGELOG.md`
- `desarrollo/docs/sistema_termico/README.md`
- `desarrollo/docs/sistema_termico/wolfram_verificaciones.md`
- `Informe_final/Desarrollo_App/Documentacion_Tecnica/08_Sistema_Termico_Hibrido.md`

## Actualizacion por Hito Tecnico

### Actualizar cuando una etapa quede funcional y estable

- `Informe_final/Manual_tecnico/manual_tecnico.tex`
- `Informe_final/chapters/04_desarrollo.tex`
- `Informe_final/Desarrollo_App/TECHNOLOGY_STACK.md`
- `portafolio_personal/README.md`

## Actualizacion Condicionada por Estado de Producto

### Actualizar cuando la experiencia ya sea visible y usable en build

- `Informe_final/Manual_de_usuario/manual_usuario.tex`

## Actualizacion Solo con Evidencia Validada

### No actualizar hasta tener datos reales

- `Informe_final/chapters/05_resultados.tex`
- `Informe_final/chapters/06_conclusiones.tex`

## Actualizacion de Portafolio

### Cuando una pieza ya tenga valor de showcase

- Crear o extender breakdowns en `portafolio_personal/documentacion/`.
- Prioridad alta cuando exista arquitectura clara, visualizacion convincente, workflow diferenciador o una solucion tecnica reusable.

## Estado Actual del Sistema Termico

A fecha de esta matriz, el sistema termico debe documentarse como:

- cimiento funcional de un subsistema termico hibrido
- solver reducido por componentes
- vista termica alimentada por datos reales por pieza
- propagacion espacial visual basica
- slider de carga sostenida conectado al estado del dron
- preprocesado offline del grafo de contactos en desarrollo

## Siguiente Etapa Documentable

La siguiente etapa que, al completarse, debe gatillar actualizacion mayor en manual tecnico y capitulo 4 es el preprocesado offline del grafo de contactos termicos y la asignacion de perfiles termicos a las piezas criticas del X500 V2.