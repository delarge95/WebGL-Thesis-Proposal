# Paquete de Validación con Usuarios

Este directorio contiene los documentos para aplicar la prueba de usabilidad y carga de trabajo percibida del prototipo WebGL del Holybro X500 V2. La aplicación debe seguir este orden para mantener consistencia metodológica y evitar sesgos.

## Documentos para entregar al participante

1. `01_CONSENTIMIENTO_INFORMADO.md`  
   Debe leerse y aceptarse antes de iniciar cualquier tarea.

2. `02_HOJA_INSTRUCCIONES_PARTICIPANTE.md`  
   Explica la dinámica general de la prueba, la condición 3D, la condición 2D y la verbalización tipo Think-Aloud.

3. `04_CUESTIONARIO_NASA_TLX_PARTICIPANTE.md`  
   Se diligencia después de cada condición evaluada: una vez para el visor 3D y una vez para el soporte 2D, si ambas condiciones se aplican.

4. `03_CUESTIONARIO_SUS_PARTICIPANTE.md`  
   Se diligencia solo después de usar el visor 3D.

## Documentos para uso del evaluador

- `GUIA_TAREAS_VALIDACION.md`: protocolo de tareas, secuencias AB/BA y criterios de registro.
- `PROTOCOLO_THINK_ALOUD.md`: guía para verbalización concurrente y codificación cualitativa.
- `05_FORMATO_REGISTRO_MODERADOR.md`: hoja de registro de sesión, tiempos para T1-T3, éxito, errores, ayudas y observaciones.
- `06_GUIA_MEDICIONES_TECNICAS_WEBGL.md`: metodo operativo para medir FPS, frame time, memoria, carga, escenarios, dispositivos, navegador y evidencias tecnicas de la build WebGL mediante el boton `Download JSON + CSV` del profiler interno.
- `CUESTIONARIO_SUS.md`: versión completa con cálculo e interpretación para el evaluador.
- `CUESTIONARIO_NASA_TLX.md`: versión completa con cálculo e interpretación para el evaluador.

## Orden recomendado de aplicación

1. Asignar código anónimo de participante.
2. Leer consentimiento informado y resolver dudas.
3. Registrar datos técnicos de sesión.
4. Asignar secuencia `AB` o `BA`.
5. Aplicar condición 1 y registrar tareas, incluyendo tiempos para T1-T3.
6. Aplicar NASA-TLX Raw para condición 1.
7. Aplicar condición 2, si corresponde, y registrar tareas, incluyendo tiempos para T1-T3.
8. Aplicar NASA-TLX Raw para condición 2.
9. Aplicar SUS solo para el visor 3D.
10. Registrar comentarios finales y memo breve del moderador.

La T4 se mantiene como tarea exploratoria guiada y no se cronometra; su lectura se conserva en la observación cualitativa y el protocolo Think-Aloud.

## Medicion tecnica de la build WebGL

La validacion tecnica se documenta en `06_GUIA_MEDICIONES_TECNICAS_WEBGL.md`. Debe aplicarse sobre una build publicada o congelada, registrando dispositivo, navegador, resolucion, escenario y archivos JSON/CSV descargados desde el profiler interno. Estos datos alimentan las tablas de KPIs del capitulo 5 del informe final y no deben mezclarse con los instrumentos subjetivos de usabilidad o carga de trabajo percibida.

## Criterios éticos mínimos

- La participación debe ser voluntaria.
- No se debe registrar el nombre real del participante en las tablas de resultados.
- El participante puede retirarse en cualquier momento sin consecuencia.
- La prueba evalúa el prototipo, no la capacidad del participante.
- Cualquier grabación de pantalla, audio o fotografía requiere autorización explícita.
- Los resultados deben reportarse de forma agregada o anonimizada.
