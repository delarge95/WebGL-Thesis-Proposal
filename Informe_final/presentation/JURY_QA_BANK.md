# Banco de preguntas de jurado - TwinSight X500

Estado: canonico para preparacion de defensa.
Fecha de actualizacion: 2026-06-11.
Fuente autoritativa: `Informe_final/informe_final.pdf`.
Documento complementario: `Informe_final/presentation/DEFENSE_EVIDENCE_MAP.md`.

## 1. Plantilla de respuesta

Usar esta estructura ante preguntas dificiles:

1. Respuesta directa: si, no, en parte, depende de X.
2. Criterio tecnico o metodologico.
3. Evidencia trazable.
4. Limite honesto.
5. Retorno al aporte de la tesis.

Frase base:

> La afirmacion defendible es X, bajo las condiciones Y, con evidencia Z. Lo que no puedo afirmar es W porque no fue medido o no pertenece al alcance.

## 2. Alcance y contribucion

### Q1. El proyecto es realmente un digital twin?

Respuesta corta:

No. El termino defendible es visual product twin o capa visual-semantica de producto.

Respuesta ampliada:

Un digital twin operacional requiere, como minimo, relacion continua o sincronizada con un activo fisico, telemetria, estados operacionales, reglas de mantenimiento o integracion con sistemas externos. TwinSight X500 no hace eso. Lo que si hace es organizar un ensamblaje complejo en una experiencia WebGL navegable, seleccionable y explicable, con piezas, categorias, modos de inspeccion y medicion tecnica.

Evidencia:

- `Informe_final/chapters/01_introduccion.tex`
- `Informe_final/chapters/03_marco_metodologico.tex`
- `README.md`

Evitar:

- "Si, es un digital twin basico."

### Q2. Entonces cual es la contribucion si no es un gemelo digital?

Respuesta corta:

La contribucion es convertir un hardware complejo en una experiencia web 3D inspeccionable, semanticamente organizada y evaluada formativamente.

Respuesta ampliada:

El aporte combina tres capas. La tecnica: pipeline CAD/Blender/Unity/WebGL, optimizacion geometrica, taxonomia, shaders y profiler. La metodologica: evaluacion con rendimiento, tareas, SUS, NASA-TLX Raw y Think-Aloud. La comunicativa: hacer legibles relaciones espaciales y funcionales que en documentacion 2D requieren reconstruccion mental.

Evidencia:

- `Informe_final/chapters/04_desarrollo.tex`
- `Informe_final/chapters/05_resultados.tex`
- `Informe_final/chapters/06_conclusiones.tex`

Evitar:

- "La contribucion es que se ve bien."

### Q3. Por que escoger el Holybro X500 V2?

Respuesta corta:

Porque es un caso abierto, trazable y suficientemente complejo para probar visualizacion tecnica.

Respuesta ampliada:

El X500 V2 tiene estructura, propulsion, energia, electronica y control, con relaciones espaciales relevantes. Ademas tiene documentacion y recursos publicos, lo que permite justificar el caso sin depender de informacion propietaria restringida.

Evidencia:

- `Informe_final/chapters/01_introduccion.tex`
- Documentacion del fabricante citada en referencias.

Evitar:

- "Porque era el mas facil."

### Q4. Por que no hacer una app nativa en vez de WebGL?

Respuesta corta:

Porque el objetivo academico era demostrar acceso web y despliegue en navegador.

Respuesta ampliada:

Una app nativa podria rendir mejor, pero cambiaria el problema. La tesis se concentra en una capa web de inspeccion tecnica, por lo que WebGL/WebAssembly son parte del reto: controlar memoria, geometria, build y navegadores compatibles.

Evidencia:

- `Informe_final/chapters/01_introduccion.tex`
- `Informe_final/chapters/03_marco_metodologico.tex`

Evitar:

- "WebGL siempre es mejor que nativo."

### Q5. Por que Unity y no Three.js o Babylon.js?

Respuesta corta:

Unity se eligio por integracion de pipeline, UI, materiales, shaders, profiling y build web dentro de un entorno unificado.

Respuesta ampliada:

Three.js o Babylon.js son alternativas validas, pero habrian requerido mayor integracion manual entre escena, tooling, profiling, UI y pipeline de assets. Para este proyecto, Unity ofrecio un equilibrio entre desarrollo C#, URP, UI Toolkit, manejo de escenas, importacion, shaders y despliegue WebGL.

Evidencia:

- `Informe_final/chapters/01_introduccion.tex`, justificacion.
- `Informe_final/chapters/04_desarrollo.tex`.

Evitar:

- "Three.js no sirve."

## 3. Metodologia y muestra

### Q6. Por que usar Design Science Research?

Respuesta corta:

Porque la tesis construye y evalua un artefacto digital para resolver un problema practico.

Respuesta ampliada:

DSR es apropiado cuando el conocimiento se produce mediante diseno, construccion, demostracion y evaluacion de un artefacto. Aqui el artefacto es la app WebGL; el problema es la comprension e inspeccion de hardware complejo; y la evaluacion combina datos tecnicos, tareas, SUS, NASA-TLX Raw y Think-Aloud.

Evidencia:

- `Informe_final/chapters/03_marco_metodologico.tex`

Evitar:

- "Use DSR porque es flexible."

### Q7. La muestra de 12 participantes es suficiente?

Respuesta corta:

Es suficiente para evaluacion formativa y descriptiva, no para inferencia poblacional fuerte.

Respuesta ampliada:

La tesis no afirma generalizacion estadistica. Usa una muestra no probabilistica de 12 participantes anonimizados con perfiles afines, dentro del escenario minimo operativo definido. Sirve para detectar patrones de uso, workload percibido, usabilidad y fricciones cualitativas. Para inferencia poblacional o comparacion experimental robusta se requeriria una muestra mayor y diseno estadistico mas fuerte.

Evidencia:

- `Informe_final/chapters/03_marco_metodologico.tex`, muestra.
- `Informe_final/chapters/05_resultados.tex`, resultados con usuarios.

Evitar:

- "Si, 12 representa a todos los usuarios."

### Q8. Por que no llegaron a 30 participantes?

Respuesta corta:

Treinta era escenario deseable; el informe declara 12 como muestra formativa real y ajusta la interpretacion.

Respuesta ampliada:

El documento diferencia meta deseable y muestra efectiva. Lo metodologicamente importante es no interpretar los datos como inferencia poblacional. Con 12 participantes se reportan resultados descriptivos, triangulados con Think-Aloud y datos tecnicos.

Evidencia:

- `Informe_final/chapters/03_marco_metodologico.tex`

Evitar:

- "No importa la muestra."

### Q9. Hubo contrabalanceo entre 3D y 2D?

Respuesta corta:

Si. Se uso secuencia AB/BA por alternancia simple.

Respuesta ampliada:

Los codigos impares siguieron AB, visor 3D primero y soporte 2D despues. Los codigos pares siguieron BA, soporte 2D primero y visor 3D despues. Esto reduce sesgo de aprendizaje o fatiga sin introducir asignacion subjetiva.

Evidencia:

- `Informe_final/chapters/03_marco_metodologico.tex`, diseno comparativo.
- `Informe_final/chapters/05_resultados.tex`, tabla por participante.

Evitar:

- "Todos hicieron primero el 3D."

### Q10. Las tareas 3D y 2D eran equivalentes?

Respuesta corta:

Eran equivalentes en objetivo funcional, no en medio de interaccion.

Respuesta ampliada:

La condicion 2D uso un paquete documental fijo con informacion del mismo sistema. La condicion 3D uso el visor interactivo. Las tareas buscaban ubicar, interpretar, relacionar y analizar visualmente informacion equivalente, variando solo el medio.

Evidencia:

- `Informe_final/chapters/03_marco_metodologico.tex`, condicion 2D y protocolo de tareas.

Evitar:

- "Eran identicas en todos los aspectos." No lo son, porque el medio cambia.

### Q11. Por que T4 no fue cronometrada?

Respuesta corta:

Porque T4 era una tarea exploratoria guiada, no una tarea de tiempo comparable.

Respuesta ampliada:

T1, T2 y T3 si registraron tiempos. T4 se uso para analisis visual guiado, donde interesaba observar uso de herramientas como Explode, Cut, X-Ray o Thermal, no medir velocidad. Cronometrarla habria mezclado exploracion, aprendizaje y comparacion temporal de forma metodologicamente debil.

Evidencia:

- `Informe_final/chapters/05_resultados.tex`, desempeno y tiempos.

Evitar:

- "Se nos olvido cronometrarla."

### Q12. Por que hablar de carga cognitiva si usaron NASA-TLX?

Respuesta corta:

La teoria de carga cognitiva es marco interpretativo; NASA-TLX Raw mide carga de trabajo percibida.

Respuesta ampliada:

No se afirma que NASA-TLX mida directamente carga intrinseca, extrinseca y germana. Se usa para workload subjetivo. La teoria permite interpretar por que una interfaz 3D podria reducir trabajo de reconstruccion espacial, pero la medicion reportada es carga de trabajo percibida.

Evidencia:

- `Informe_final/chapters/03_marco_metodologico.tex`, variables e instrumentos.
- `Informe_final/chapters/05_resultados.tex`, discusion.

Evitar:

- "NASA mide carga cognitiva exacta."

## 4. SUS, NASA-TLX Raw y Think-Aloud

### Q13. Por que SUS solo se aplico al prototipo 3D?

Respuesta corta:

Porque SUS evalua usabilidad de un sistema interactivo; el soporte 2D funciono como condicion de referencia, no como sistema equivalente.

Respuesta ampliada:

SUS mide percepcion global de usabilidad del sistema. Aplicarlo al paquete 2D podria producir una comparacion metodologicamente ambigua, porque no es una interfaz interactiva equivalente. La comparacion entre 3D y 2D se apoya en tareas, NASA-TLX Raw y observacion.

Evidencia:

- `Informe_final/chapters/03_marco_metodologico.tex`
- `Informe_final/chapters/05_resultados.tex`

Evitar:

- "SUS compara 3D vs 2D."

### Q14. Que significa SUS 91,88?

Respuesta corta:

Indica percepcion de usabilidad muy favorable del prototipo 3D en esta muestra.

Respuesta ampliada:

El promedio SUS fue 91,88, mediana 95, minimo 60 y maximo 100. Se compara con 68 como referencia historica del instrumento, no como corte absoluto de aprobado/reprobado. Por el tamano de muestra, se interpreta descriptivamente.

Evidencia:

- `Informe_final/chapters/05_resultados.tex`

Evitar:

- "91,88 prueba que el producto es universalmente usable."

### Q15. Que significa que NASA 3D sea 8,69 y 2D sea 19,89?

Respuesta corta:

Que en esta muestra la carga de trabajo percibida fue menor con el visor 3D.

Respuesta ampliada:

NASA-TLX Raw se calculo por condicion. El promedio 3D fue 8,69 y el 2D fue 19,89, con diferencia pareada media de 11,19. En los 12 casos el valor fue menor en 3D. La interpretacion es descriptiva, no inferencial.

Evidencia:

- `Informe_final/chapters/05_resultados.tex`

Evitar:

- "Demuestra cientificamente que 3D siempre reduce carga."

### Q16. Como manejaron la dimension rendimiento en NASA-TLX?

Respuesta corta:

Se diligencio con orientacion invertida: 0 = desempeno perfecto, 100 = desempeno deficiente.

Respuesta ampliada:

Esto conserva la direccion del promedio global: valores menores indican menor carga o mejor percepcion. La nota aparece en metodologia y resultados, para evitar que rendimiento se interprete al reves.

Evidencia:

- `Informe_final/chapters/03_marco_metodologico.tex`
- `Informe_final/chapters/05_resultados.tex`

Evitar:

- "Se promedio sin revisar direccion de escala."

### Q17. Que aporta Think-Aloud si ya tienen cuestionarios?

Respuesta corta:

Explica el por que detras de las cifras.

Respuesta ampliada:

SUS y NASA dan una lectura numerica. Think-Aloud registra verbalizaciones, dudas, recuperaciones y fricciones. En el estudio, la categoria comprension espacial aparecio en 11/12 participantes y navegacion/control en 10/12, lo que ayuda a interpretar por que el 3D fue percibido como menos demandante y donde todavia hay friccion.

Evidencia:

- `Informe_final/chapters/05_resultados.tex`, resultados cualitativos.

Evitar:

- "Think-Aloud prueba estadisticamente."

### Q18. Que significa el efecto techo en completitud?

Respuesta corta:

Que ambas condiciones permitieron completar las tareas, por lo que la completitud no diferencia fuertemente 3D y 2D.

Respuesta ampliada:

Las cuatro tareas se completaron 12/12 en ambas condiciones. Por eso la discusion no afirma que solo el 3D permita completar. El valor diferencial aparece en menor workload percibido, menores tiempos en T1-T3 y verbalizaciones de comprension espacial.

Evidencia:

- `Informe_final/chapters/05_resultados.tex`, discusion.

Evitar:

- "El 2D fracaso."

## 5. Resultados y estadistica

### Q19. Por que no aplicaron estadistica inferencial?

Respuesta corta:

Porque la muestra y el diseno se definieron como formativos, descriptivos y no probabilisticos.

Respuesta ampliada:

Aplicar pruebas inferenciales podria sonar mas fuerte, pero seria metodologicamente riesgoso si luego se extrapola. La tesis prefiere reportar medias, medianas, diferencias descriptivas, dispersion y triangulacion cualitativa. Una fase futura con mayor muestra podria aplicar analisis inferencial.

Evidencia:

- `Informe_final/chapters/03_marco_metodologico.tex`
- `Informe_final/chapters/05_resultados.tex`

Evitar:

- "No hacia falta analizar datos."

### Q20. Los tiempos T1-T3 prueban que el 3D es mejor?

Respuesta corta:

Muestran una ventaja descriptiva en esta muestra, no una prueba universal.

Respuesta ampliada:

El total medio T1-T3 fue 20,58 s en 3D y 54,00 s en 2D, con diferencia de 33,42 s. Esto apoya la lectura de menor esfuerzo operacional, pero se interpreta junto con NASA y Think-Aloud, no como causalidad generalizable.

Evidencia:

- `Informe_final/chapters/05_resultados.tex`

Evitar:

- "Prueba superioridad absoluta."

### Q21. Como interpretan el participante con SUS 60?

Respuesta corta:

Como un caso de friccion dentro de una distribucion generalmente favorable.

Respuesta ampliada:

El participante 006 tuvo SUS 60 y mayor NASA 3D que otros casos, aunque aun mantuvo menor NASA en 3D que en 2D. Este caso es util porque muestra que la app no fue perfecta para todos y respalda mejoras futuras en iconos, navegacion movil y seleccion de piezas pequenas.

Evidencia:

- `Informe_final/chapters/05_resultados.tex`, tabla por participante.

Evitar:

- "Es un dato atipico que se ignora."

### Q22. La validacion demuestra aprendizaje?

Respuesta corta:

No directamente. Demuestra desempeno, workload percibido, usabilidad y verbalizaciones de comprension espacial durante tareas.

Respuesta ampliada:

Para medir aprendizaje haria falta pretest/postest, retencion o transferencia posterior. La tesis no llega a ese nivel. Si se habla de comprension, se hace como comprension situada durante la tarea, apoyada por Think-Aloud y desempeno, no como aprendizaje longitudinal.

Evidencia:

- `Informe_final/chapters/03_marco_metodologico.tex`
- `Informe_final/chapters/06_conclusiones.tex`

Evitar:

- "El prototipo ensena mejor a largo plazo."

## 6. Pipeline, geometria y runtime

### Q23. Por que 95 617 triangulos y tambien 229 054?

Respuesta corta:

Porque son metricas de niveles distintos.

Respuesta ampliada:

95 617 triangulos corresponde al activo base optimizado exportado. 229 054 triangulos estimados corresponde a la escena runtime instrumentada por el profiler, que incluye instancias, proxies, assets de apoyo y renderers activos. No son cifras equivalentes y no deben leerse como contradiccion.

Evidencia:

- `Informe_final/chapters/05_resultados.tex`
- `Informe_final/validacion/07_TABLAS_RENDIMIENTO_WEBGL_MEDICIONES.tex`

Evitar:

- "Una de las dos esta mal."

### Q24. Por que hay 28 piezas, 30 anchors y 257 renderers?

Respuesta corta:

Porque son capas diferentes: semantica, operativa y geometrica.

Respuesta ampliada:

Las 28 piezas canonicas son la taxonomia academica. Los 30 anchors son nodos de escena que agregan grupos tecnicos como fasteners y miscelaneos. Los 257 renderers/colliders representan la fragmentacion geometrica/runtime. Un componente semantico puede estar compuesto por varios fragmentos renderizables.

Evidencia:

- `Informe_final/chapters/04_desarrollo.tex`

Evitar:

- "Son 257 piezas."

### Q25. Que tan automatico fue el pipeline CAD?

Respuesta corta:

Fue mixto: automatizacion para tareas repetitivas y revision manual para calidad de activo.

Respuesta ampliada:

El pipeline uso rutas como Blender, MoI3D, STEPper, remodelado, saneamiento, proxies y scripts. Automatizar todo habria sido riesgoso por ambiguedad de geometria CAD, transformaciones, piezas repetidas y fasteners. La tesis documenta tooling, pero no afirma que todo el pipeline sea automatico y universal.

Evidencia:

- `Informe_final/chapters/04_desarrollo.tex`

Evitar:

- "Todo se optimiza automaticamente."

### Q26. El modelo optimizado pierde fidelidad tecnica?

Respuesta corta:

Pierde detalle de manufactura innecesario, pero conserva legibilidad formal y relaciones relevantes para inspeccion.

Respuesta ampliada:

El objetivo no era fabricar desde la malla optimizada, sino inspeccionar visualmente en web. Por eso se conservan silueta, relaciones y lectura de piezas, apoyadas por materiales, normales y AO. El CAD original conserva otro tipo de fidelidad, pero no es viable como runtime directo.

Evidencia:

- `Informe_final/chapters/04_desarrollo.tex`
- `Informe_final/chapters/05_resultados.tex`

Evitar:

- "No se pierde nada."

### Q27. Que pasa con la tornilleria y fasteners?

Respuesta corta:

Se manejan con taxonomia, proxies y reconstruccion modular limitada, especialmente para tornillos.

Respuesta ampliada:

Los fasteners repetitivos pueden ser muy costosos geometricamente. El proyecto diferencia fastener group, proxies, casos originales y reconstruccion modular de tornillos. No todos los sujetadores se convierten en detalle modular final; la asignacion a piezas madre requiere manifest y revision cuando hay ambiguedad.

Evidencia:

- `Informe_final/chapters/04_desarrollo.tex`

Evitar:

- "Todos los fasteners estan resueltos con detalle final perfecto."

## 7. Rendimiento, WebGL y build

### Q28. La app cumple 30 FPS?

Respuesta corta:

Cumple o se acerca segun dispositivo y escenario; no en todos los moviles de limite inferior.

Respuesta ampliada:

El informe reporta rendimiento por dispositivo, navegador, resolucion y escenario. En escritorio fue estable, en gama media-alta/alta fue favorable o cercano, y en un limite inferior Android fue navegable pero debajo de 30 FPS. La lectura es viabilidad con limites, no garantia universal.

Evidencia:

- `Informe_final/chapters/05_resultados.tex`
- `Informe_final/validacion/07_TABLAS_RENDIMIENTO_WEBGL_MEDICIONES.tex`

Evitar:

- "Siempre corre a 30 FPS."

### Q29. Por que usar cache cargado en pruebas?

Respuesta corta:

Porque la evaluacion de interaccion y rendimiento runtime necesitaba estabilizar la condicion de carga.

Respuesta ampliada:

La cache cargada reduce variabilidad inicial por descarga o red. El informe lo declara para trazabilidad. No debe confundirse con promesa de primera carga instantanea. Si se evalua carga inicial en futuro, se debe medir aparte con cache fria y condiciones de red controladas.

Evidencia:

- `Informe_final/chapters/03_marco_metodologico.tex`
- `Informe_final/chapters/05_resultados.tex`

Evitar:

- "La cache no importa."

### Q30. Que tan reproducibles son las mediciones WebGL?

Respuesta corta:

Son reproducibles bajo condiciones registradas: build, dispositivo, navegador, resolucion, cache y escenario.

Respuesta ampliada:

El profiler interno exporta JSON/CSV por escenario. Pero WebGL depende del entorno. La reproducibilidad no significa obtener identico FPS en todo equipo, sino poder repetir protocolo y comparar con contexto.

Evidencia:

- `Informe_final/validacion/06_GUIA_MEDICIONES_TECNICAS_WEBGL.md`
- `Telemetria/Mediciones_WebGL/`

Evitar:

- "Cualquier persona obtendra el mismo valor."

### Q31. La compatibilidad movil esta garantizada?

Respuesta corta:

No. Esta evaluada de forma acotada en dispositivos y navegadores especificos.

Respuesta ampliada:

Unity WebGL puede ejecutarse en navegadores compatibles, pero memoria, GPU, navegador y sistema operativo afectan la experiencia. Por eso la tesis habla de compatibilidad esperada y validada por casos, no universal.

Evidencia:

- `Informe_final/chapters/01_introduccion.tex`
- `Informe_final/chapters/05_resultados.tex`

Evitar:

- "Mobile first significa que corre perfecto en todo movil."

### Q32. Por que el escritorio se llama adaptacion funcional?

Respuesta corta:

Porque el diseno se desarrollo con enfoque mobile-first y desktop se mantuvo funcional para demostracion.

Respuesta ampliada:

El informe reconoce que desktop no recibio una experiencia especifica con el mismo nivel de diseno sistemico. Funciona y permite demostrar, pero no se sobrevende como UI desktop plenamente optimizada.

Evidencia:

- `Informe_final/chapters/01_introduccion.tex`
- `Informe_final/chapters/04_desarrollo.tex`

Evitar:

- "Desktop esta tan trabajado como mobile."

## 8. UI, demo y repo

### Q33. Que features son publicas y cuales no?

Respuesta corta:

Publico defendible: Hero, Explore, seleccion, bottom sheet, Inspect, Analyze, Studio, explode, cut, filtros y Thermal heuristico.

Respuesta ampliada:

El README clasifica capacidades como visible_ui, implementado_oculto, legacy, futuro o requiere_evidencia. Measurement, BOM, annotations, connection tools o modulos historicos no deben mostrarse como flujo final si no estan publicados y evaluados.

Evidencia:

- `README.md`
- `Informe_final/presentation/DEMO_SCRIPT.md`

Evitar:

- "Todo lo que existe en codigo es alcance final."

### Q34. Que hacer si el jurado ve en el repo algo que no esta en la UI?

Respuesta corta:

Explicar que el informe final es la fuente autoritativa del alcance evaluado.

Respuesta ampliada:

Un repositorio puede contener codigo legacy, tooling, pruebas o capacidades ocultas. La defensa distingue lo visible y evaluado de lo implementado oculto o historico. La tesis se juzga por el artefacto final, anexos, manuales y build publicada.

Evidencia:

- `README.md`, Capability Status.
- `Informe_final/informe_final.pdf`.

Evitar:

- "Si esta en codigo, entonces cuenta como entregado."

### Q35. Por que usar video de demo y no solo vivo?

Respuesta corta:

Porque el video reduce riesgo y la demo viva puede usarse como confirmacion breve.

Respuesta ampliada:

En defensa, el demo debe ser evidencia, no improvisacion. Un video local de la build real permite mostrar flujo completo sin depender de red, latencia o fallos del navegador. Si el entorno esta estable, una microdemo viva refuerza autenticidad.

Evidencia:

- `Informe_final/presentation/DEMO_SCRIPT.md`
- `Informe_final/presentation/ASSETS_REQUIREMENTS.md`

Evitar:

- "El video reemplaza evidencia tecnica." No; solo muestra flujo.

### Q36. Que pasa si falla WebGL durante la defensa?

Respuesta corta:

Pasar al video local y explicar que la evidencia es la secuencia de build real ya grabada.

Respuesta ampliada:

No conviene hacer troubleshooting en pantalla. La frase preparada es: "Para no gastar tiempo en troubleshooting, paso al recorrido grabado. La evidencia que quiero mostrar es esta." Luego se conecta con resultados y profiler.

Evidencia:

- `Informe_final/presentation/DEMO_SCRIPT.md`

Evitar:

- Reparar en vivo durante varios minutos.

## 9. Thermal y analisis visual

### Q37. El sistema Thermal tiene validez cientifica?

Respuesta corta:

Tiene validez como visualizacion heuristica, no como modelo fisico calibrado.

Respuesta ampliada:

Thermal apoya lectura relativa por componentes y comunicacion tecnica. No es FEA, no usa telemetria real y no debe usarse para diagnostico fisico. Su evolucion futura podria incluir calibracion experimental, sensores o simulacion fisica.

Evidencia:

- `Informe_final/chapters/01_introduccion.tex`
- `Informe_final/chapters/06_conclusiones.tex`

Evitar:

- "Calcula temperatura real."

### Q38. Si Thermal no es fisico, por que incluirlo?

Respuesta corta:

Porque aporta una lectura visual de estado relativo y demuestra extensibilidad de modos analiticos.

Respuesta ampliada:

El objetivo de Thermal en esta fase es pedagogico y comunicativo: hacer que el usuario piense por componentes, zonas y posibles estados. Su valor no esta en predecir fallas, sino en mostrar como la app puede ofrecer lecturas tecnicas complementarias.

Evidencia:

- `Informe_final/chapters/04_desarrollo.tex`
- `Informe_final/chapters/05_resultados.tex`

Evitar:

- "Es solo decorativo." Tampoco; tiene funcion visual-analitica.

## 10. Etica, datos personales y publicacion

### Q39. Hay datos personales en la validacion?

Respuesta corta:

El informe reporta datos anonimizados y agregados; no debe publicarse informacion personal identificable.

Respuesta ampliada:

Los participantes se identificaron por codigos anonimos. La caracterizacion de perfiles esta agregada. Cualquier registro crudo con datos sensibles o trazas identificables debe permanecer fuera de la superficie publica.

Evidencia:

- `Informe_final/chapters/03_marco_metodologico.tex`, consideraciones eticas.
- `README.md`, Public Repository Boundary.

Evitar:

- "Estan todos los datos crudos publicos."

### Q40. Como aseguran coherencia entre informe, app, presentacion y GitHub?

Respuesta corta:

Con una fuente autoritativa y una frontera publica: informe final primero, README como limite publico, demo solo con flujo visible.

Respuesta ampliada:

La presentacion toma cifras del capitulo 5, metodologia del capitulo 3, decisiones tecnicas del capitulo 4 y alcance del README. Los archivos de defensa no deben introducir claims nuevos. Si algo no esta en informe/anexos/build/profiler, se marca como futuro o no se afirma.

Evidencia:

- `DEFENSE_EVIDENCE_MAP.md`
- `README.md`
- `Informe_final/informe_final.pdf`

Evitar:

- "La presentacion puede decirlo aunque no este en el informe."

## 11. Preguntas de extrapolacion y futuro

### Q41. Como evolucionaria a digital shadow?

Respuesta corta:

Con un twin manifest por pieza, datos historicos o simulados, eventos por componente y trazabilidad de estado.

Respuesta ampliada:

El paso natural no es declarar un digital twin completo, sino formalizar identificadores independientes de Unity, conectar datos historicos o simulados, registrar estados por pieza y validar comportamiento. Eso seria digital shadow si existe flujo de datos desde el activo o sistema externo hacia el modelo.

Evidencia:

- `Informe_final/chapters/06_conclusiones.tex`

Evitar:

- "Solo falta conectar sensores."

### Q42. Que seria necesario para mantenimiento predictivo?

Respuesta corta:

Datos operacionales reales, historial de fallas, modelos de diagnostico, validacion fisica e integracion con procesos de mantenimiento.

Respuesta ampliada:

La app actual puede servir como capa visual, pero mantenimiento predictivo requiere mucho mas: sensores, series temporales, etiquetas de falla, umbrales, modelos, validacion y uso operativo. Por eso se deja como futuro.

Evidencia:

- `Informe_final/chapters/01_introduccion.tex`
- `Informe_final/chapters/06_conclusiones.tex`

Evitar:

- "Thermal ya permite mantenimiento predictivo."

### Q43. Que mejorarian primero con mas tiempo?

Respuesta corta:

Ampliaria muestra y dispositivos, puliria navegacion movil/iconos/seleccion de piezas pequenas y formalizaria un manifest independiente.

Respuesta ampliada:

Las mejoras se derivan de resultados. Think-Aloud senalo fricciones con iconos, navegacion movil y piezas pequenas. La capa tecnica pide mas dispositivos y pruebas de cache fria. La evolucion academica pide mayor muestra y, si se busca madurez twin, twin manifest y datos operacionales.

Evidencia:

- `Informe_final/chapters/05_resultados.tex`
- `Informe_final/chapters/06_conclusiones.tex`

Evitar:

- "No cambiaria nada."

### Q44. Como lo llevaria a portafolio laboral?

Respuesta corta:

Separaria caso academico y caso profesional: menos anexos, mas demo, breakdown tecnico y resultados verificables.

Respuesta ampliada:

Para portafolio conviene mostrar problema, rol, pipeline, capturas, video, arquitectura simplificada, decisiones de optimizacion, retos y resultados. No se deben publicar datos personales, notas internas ni claims academicos no trazados. El README publico ya funciona como frontera inicial.

Evidencia:

- `README.md`
- `Informe_final/presentation/ASSETS_REQUIREMENTS.md`

Evitar:

- Convertir la tesis en marketing exagerado.

## 12. Respuestas de seguridad cuando no se sabe

### Caso: piden una cifra no recordada

Respuesta:

> No quiero improvisar una cifra. La cifra defendible esta en el capitulo 5 o en el anexo de mediciones. Lo que si puedo afirmar es la tendencia: [explicar tendencia sin inventar numero].

### Caso: preguntan por una feature no visible

Respuesta:

> Esa capacidad pertenece a codigo interno, prototipo anterior o trabajo futuro. El flujo final evaluado y defendible es el que aparece en informe, README y demo: Hero, Explore, seleccion, bottom sheet, Inspect, Analyze y Studio.

### Caso: preguntan si el resultado generaliza

Respuesta:

> No generaliza poblacionalmente. Es una evaluacion formativa, descriptiva y trazada. Para generalizar haria falta una muestra mayor, mas dispositivos y analisis inferencial.

### Caso: preguntan por errores del proyecto

Respuesta:

> El proyecto tuvo limites claros: muestra pequena, compatibilidad movil acotada, desktop como adaptacion funcional, T4 no cronometrada y Thermal heuristico. Lo importante es que esos limites estan declarados y no se presentan como resultados cerrados.
