# Audit de Explode View - FBX final X500 V2

Fecha: 2026-05-11  
Estado: audit tecnico con primer arreglo runtime implementado

## Alcance

Se reviso la implementacion actual de la vista explosionada sobre el flujo del FBX final. El objetivo fue identificar que falta para que la vista sea estetica, legible y coherente con la jerarquia real del dron.

Archivos revisados:

- `Assets/Scripts/Core/Content/ExplodedViewManager.cs`
- `Assets/Scripts/Core/Content/ExplodablePart.cs`
- `Assets/Scripts/Core/Content/AuxiliaryExplodeOffset.cs`
- `Assets/Scripts/Core/Utils/ImportedDroneRuntimeBinder.cs`
- `Assets/Core/Data/X500V2Generated/*.asset`

## Funcionamiento actual

- `ExplodedViewManager` recopila todos los `ExplodablePart` y aplica el mismo `explosionFactor` global.
- Cada `ExplodablePart` interpola su `localPosition` entre la posicion inicial y `initialPosition + explosionDirection * explosionDistance`.
- `DronePartData` ya contiene `explosionDirection`, `explosionDistance` y `explosionPriority`, pero `explosionPriority` no se usa para escalonar la animacion.
- `ImportedDroneRuntimeBinder` conserva los presets existentes y solo repara direcciones/distancias invalidas.
- Los fasteners/subpiezas auxiliares usan `AuxiliaryExplodeOffset`, con offsets pequenos respecto a su pieza madre.

## Hallazgos principales

| Prioridad | Hallazgo | Evidencia | Riesgo UX |
|---|---|---|---|
| Alta | La explosion es global y simultanea | `ExplodedViewManager.UpdateAllParts()` aplica el mismo factor a todos los anchors | La vista puede verse como una nube de piezas en vez de una secuencia de desmontaje |
| Alta | `explosionPriority` existe pero no participa en la interpolacion | `DronePartData.explosionPriority` esta serializado, pero `ExplodablePart.UpdateExplosion()` no lo usa | Se pierde una herramienta ya modelada para ordenar plates, stack, brazos, motores y helices |
| Alta | Varias piezas usan direcciones verticales iguales | Motores y helices usan principalmente `Vector3.up`; plates/stack tambien suben o bajan | Motores, helices, GPS y top stack pueden quedar alineados o superpuestos visualmente |
| Media | Las distancias son presets heredados, no derivadas de bounds finales | El FBX final fue reescalado a `10u`, pero las distancias se mantienen por asset | Algunas piezas se separan demasiado y otras poco, dependiendo de su tamano real |
| Media | No hay postproceso de colisiones/solapes | No existe validacion de bounds en la pose explosionada | Piezas grandes pueden tapar piezas pequenas o fasteners |
| Media | El offset auxiliar depende demasiado de `localPosition.normalized` | `AuxiliaryExplodeOffset` usa posicion local o direccion del anchor | Fasteners cercanos al origen local pueden salir en direcciones poco explicativas |
| Media | No hay modo explosionado contextual por seleccion/hotspot | La vista actual es global | Para aprendizaje de montaje, seria mas claro explotar solo la pieza madre o hotspot activo |
| Baja | La camara no se reajusta a la composicion explosionada | No se detecta en `ExplodedViewManager` un recuadre dedicado | En algunos angulos, la explosion puede quedar fuera de foco o con piezas cortadas |

## Correcciones necesarias recomendadas

1. **Usar `explosionPriority` como curva de secuencia**:
   - Plates/base primero.
   - Stack central despues.
   - Brazos por cuadrante.
   - Motores/helices al final.
   - Fasteners con adelanto leve respecto a su subpieza para comunicar desmontaje.
2. **Crear un `ExplodeLayoutProfile` explicito**:
   - Fuente sugerida: JSON o ScriptableObject.
   - Campos minimos: `partId`, `direction`, `distance`, `priority`, `delay`, `spread`, `contextGroup`.
   - Ventaja: no depender de presets dispersos en muchos `.asset`.
3. **Calcular direcciones desde bounds y centro del dron**:
   - Brazos: outward por cuadrante con componente vertical moderada.
   - Plates: arriba/abajo segun orden fisico.
   - Motors/props: combinar outward del brazo + vertical, no solo `up`.
   - GPS: arriba, pero con desplazamiento lateral pequeno para no tapar top plate.
4. **Validar solapes en la pose final**:
   - Crear audit editor/runtime que evalua bounds a `factor=1`.
   - Reportar pares con interseccion o distancia menor a un margen.
5. **Separar explosion global y contextual**:
   - Global: composicion estetica completa.
   - Hotspot: solo piezas del hotspot activo.
   - Pieza madre: subpiezas + fasteners asociados.
6. **Mejorar offsets de fasteners**:
   - Preferir normal/direccion de montaje cuando exista.
   - Usar vector desde subpieza asociada hacia fastener cuando no haya normal.
   - Evitar que fasteners compartidos salgan duplicados visualmente en direcciones contradictorias.
7. **Reencuadre de camara para explode**:
   - Al entrar a explode, calcular bounds de la pose explosionada y ajustar target/distancia.
   - Al salir, restaurar camara previa.

## Estado de aceptacion actual

La vista explosionada ya no depende solo de un desplazamiento simultaneo basico. El primer arreglo runtime implementa una secuencia por `explosionPriority` y presets de direccion/distancia mas limpios para los anchors principales. Todavia falta validar visualmente solapes y decidir el siguiente nivel de pulido estetico desde Play Mode.

## Correccion implementada - 2026-05-11

1. **Secuencia por prioridad**:
   - `ExplodedViewManager` calcula `minExplosionPriority` y `maxExplosionPriority` entre piezas no fastener.
   - Cada `ExplodablePart` recibe un factor local derivado de su `explosionPriority`.
   - Las piezas con menor prioridad comienzan antes y las de mayor prioridad se integran progresivamente, evitando que todo el dron explote al mismo tiempo.
2. **Easing local por pieza**:
   - Cada factor local usa `SmoothStep`, por lo que las capas no entran con movimiento lineal brusco.
3. **Presets runtime para anchors principales**:
   - `ImportedDroneRuntimeBinder` ajusta direcciones y distancias para props, motores, brazos, plates, Pixhawk, GPS, power module, rails, landing gear y bateria.
   - Motores y helices dejan de depender de desplazamientos puramente verticales y ahora salen por cuadrante.
4. **Fasteners**:
   - La categoria `Fasteners` queda excluida del calculo de rango de prioridades para que no distorsione la secuencia global.
   - Los fasteners siguen moviendose con su factor suavizado y con offsets auxiliares existentes.

## Proximo pulido recomendado

1. Validar en Play Mode si props/motores necesitan mas separacion lateral o menos altura.
2. Ajustar `priorityTravelWindow` si la secuencia se siente demasiado lenta o demasiado simultanea.
3. Implementar audit de solapes a `explosionFactor=1`.
4. Crear `ExplodeLayoutProfile` persistente si el resultado visual runtime ya queda aprobado.

## Correccion implementada - 2026-05-11 B

1. **Centro comun de explosion**:
   - `ExplodedViewManager` resuelve el origen radial desde `x500v2_bottom_plate`.
   - Si la pieza de centro no existe, usa bounds agregados de piezas no fastener como fallback.
2. **Direccion radial real por pieza**:
   - Cada pieza calcula su direccion como `centro visual de pieza - centro del dron`.
   - El target final ya no depende principalmente de `explosionDirection` serializado, sino de la posicion real de la pieza en la escena.
3. **Target runtime no destructivo**:
   - `ExplodablePart` expone `ConfigureRuntimeExplosionTarget(...)` para recibir un target calculado en world space.
   - No se editan assets ni se reescriben ScriptableObjects para esta correccion.
4. **Fallback para piezas centradas**:
   - Si una pieza coincide practicamente con el centro, se usa una direccion segura: bottom plate/rails/landing hacia abajo, o direccion heredada si existe.

## Criterio de validacion visual actualizado

- Brazos, motores y helices deben alejarse radialmente desde el centro del dron.
- El stack central debe abrirse hacia arriba/abajo segun su ubicacion relativa al bottom plate.
- Ninguna pieza principal debe moverse hacia el centro durante explode.
- Las siguientes correcciones deben enfocarse en tiempos/delays, separacion estetica y solapes, no en la base direccional.

## Correccion implementada - 2026-05-11 C

1. **Frente fisico del dron**:
   - La referencia frontal runtime queda definida por `ZHIJIA-CAMERA-INTEL.001_low`.
   - Los calculos de cuadrante ya no dependen solo de `world X/Z`; ahora usan el vector `bottom plate -> camera` como eje forward y su perpendicular como eje right.
2. **Tubo largo compartido**:
   - `CARBON-FIBER-TUBE300.001/.002` deja de interpretar `.001/.002` como propiedad de cuadrante.
   - Estos sufijos se tratan como instancias Blender de tubos compartidos, no como `BR/FR`.
3. **Offset radial por subpieza de brazo**:
   - En brazos, cada renderer estructural calcula su propia compensacion radial desde el centro del dron.
   - Esto evita que una subpieza compartida cruce el dron siguiendo el movimiento del anchor canonico si su centro real apunta a otro lado.

## Validacion pendiente

- Reimportar o dejar que Unity rebinde la escena para que la jerarquia reciba la nueva regla de `CARBON-FIBER-TUBE300`.
- Probar especificamente `CARBON-FIBER-TUBE300.002_low`: debe moverse hacia su lado/frente real, sin atravesar el centro hacia la derecha.
- Confirmar visualmente si el fallback de piezas sobre el eje medio debe quedar como prioridad izquierda o si conviene crear una capa compartida frontal/posterior explicita.

## Correccion implementada - 2026-05-11 D

1. **Mapa explicito por familia de brazo**:
   - Se reemplazo la lectura generica de sufijos `.001/.002/.003/.004` por una tabla por familia Blender.
   - `HMX5V-JIBI-JIA-MUJU.001` y `.002` quedan en el mismo brazo fisico.
   - `HMX5V-JIBI-JIA-MUJU.003` y `.004` quedan en el mismo brazo fisico.
   - Las familias `BAN-DJ-DIAN-F2`, `HMX5V-DIGAI-DIANJIZUO-MUJU`, `HMX5V-GUAN-DINGWEI`, `HMX5V-ZUO-DJ-MUJU`, `JIA-GUAN` y `DJ-2216-KV880` usan correspondencias explicitas derivadas de posicion real.
2. **Correccion de anchors de brazo**:
   - El binder runtime puede mover una subpieza de un anchor `x500v2_arm_*` a otro si el mapa explicito lo exige.
   - Esta regla se limita a brazos para no deshacer anchors canonicos estables en otras piezas.
3. **Offset planar en explode**:
   - La normal radial de subpiezas estructurales se proyecta sobre el plano horizontal antes de calcular el desplazamiento.
   - Esto evita que piezas elevadas salgan hacia arriba cuando deberian salir hacia el mismo lado que su par fisico.
4. **Delay por dependencia fisica**:
   - Las prioridades runtime quedan ordenadas desde piezas externas/desmontables hacia estructura principal.
   - El objetivo es mantener el delay agradable actual, pero con lectura de desmontaje: props/bateria/electronica antes que brazos y bottom plate.

## Criterio de validacion visual actualizado D

- `HMX5V-JIBI-JIA-MUJU.002_low` debe salir en la misma direccion general que `HMX5V-JIBI-JIA-MUJU.001_low`, no hacia arriba.
- `HMX5V-JIBI-JIA-MUJU.004_low` debe salir en la misma direccion general que `HMX5V-JIBI-JIA-MUJU.003_low`, no hacia arriba.
- Las subpiezas de brazo deben pertenecer al brazo fisico indicado por posicion real, no al sufijo Blender aislado.
- La secuencia debe seguir siendo escalonada, pero ahora con dependencia fisica: periferia/electronica desmontable primero, estructura principal despues.

## Correccion implementada - 2026-05-11 E

1. **Fasteners primero**:
   - Se agrego un retraso inicial para piezas no fastener.
   - Los fasteners, incluyendo anchors independientes y fasteners visibles dentro de piezas madre, pueden iniciar su salida antes del desplazamiento de la pieza principal.
   - Los anchors `Fasteners` completan su recorrido en una ventana inicial propia para que no terminen de desplazarse al mismo tiempo que las estructuras grandes.
2. **Direccion por eje de montaje**:
   - Los fasteners usan el eje mas probable de su mesh/transform como referencia.
   - Tornillos: salen hacia arriba si su eje es vertical o hacia afuera si el eje es lateral.
   - Tuercas: salen hacia abajo/opuesto respecto al eje de apriete.
   - Separadores/standoffs: usan salida axial corta y controlada.
3. **Offsets manuales por subpieza de brazo**:
   - `BAN-DJ-DIAN-F2`: separacion vertical positiva con componente radial.
   - `HMX5V-ZUO-DJ-MUJU`: salida superior del conjunto de motor.
   - `HMX5V-DIGAI-DIANJIZUO-MUJU`: salida inferior/opuesta al soporte superior.
   - `HMX5V-JIBI-JIA-MUJU`: separacion tardia de abrazaderas; la mitad superior sube y la inferior baja segun posicion real.
   - `JIA-GUAN` y `HMX5V-GUAN-DINGWEI`: separacion menor para no contaminar la lectura principal del brazo.
4. **Correccion radial independiente**:
   - El offset radial por renderer ya no pisa el offset fisico manual.
   - `AuxiliaryExplodeOffset` suma un offset fisico principal y una correccion radial, evitando acumulaciones por reconstrucciones de cache.
5. **Movimiento compuesto para piezas dependientes**:
   - `ExplodablePart` suma un offset primario de pieza madre y un offset secundario de desmontaje local.
   - Fasteners independientes usan el recorrido de su pieza madre mas su salida axial.
   - Motores y helices usan el recorrido radial del brazo mas una separacion vertical propia.
6. **Refinamiento de separaciones del extremo del brazo**:
   - Se incremento la distancia axial de tornillos para liberar la union con mayor claridad.
   - `BAN-DJ-DIAN-F2` queda mas arriba que `HMX5V-ZUO-DJ-MUJU` para mantener distancia vertical y evitar que vuelvan a tocarse visualmente.
7. **Parent espacial de movimiento**:
   - Los fasteners conservan su metadata semantica, pero el movimiento puede seguir a la pieza mas cercana/contactada en la escena.
   - Esto permite que tornillos de top plate suban con la top plate despues de liberar su eje, aunque su catalogo granular tenga otra asignacion canonica.
8. **Correcciones por caso critico**:
   - `HMX5V-JIBI-JIA-MUJU.001` y `.005` bajan explicitamente.
   - Motores y helices toman como arrastre el brazo mas cercano por bounds reales, no solo el sufijo del id.
   - `JIA-GUAN`, rubber/grommets y `HMX5V-GUAN-DINGWEI` evitan la correccion radial temprana que generaba zigzag.
   - `GUAN-CHENG` y rails/battery mount quedan orientados a desmontaje descendente.

## Criterio de validacion visual actualizado E

- Los fasteners deben liberar visualmente las uniones antes de que salgan piezas grandes.
- Despues de liberar la union, los fasteners deben seguir el recorrido de su pieza principal, no quedarse congelados en el aire.
- Tornillos y tuercas deben separarse en sentidos coherentes, no como un bloque radial uniforme.
- Motores y helices deben separarse verticalmente, pero conservar el desplazamiento radial del brazo correspondiente.
- Motores no deben avanzar hacia el centro si el brazo correspondiente se aleja del centro.
- `JIA-GUAN` debe viajar inicialmente con `CARBON-FIBER-TUBE300`/rubber y separarse despues, sin zigzag.
- El rail system and battery mount debe salir hacia abajo antes de separar sus subcomponentes.
- En un brazo, helices/motor/soportes deben conservar una separacion vertical suficiente para analisis, mientras el tubo conserva lectura radial desde el centro del dron.
- Las abrazaderas `HMX5V-JIBI-JIA-MUJU` deben separarse despues de liberar la tornilleria asociada y sin atravesar el centro del dron.
- La pose final debe minimizar contactos o solapes entre piezas, priorizando claridad de desmontaje frente a simetria puramente estetica.

## Correccion implementada - 2026-05-11 F

1. **Clearance final por bounds**:
   - `ExplodedViewManager` agrega una pasada iterativa de separacion minima en la pose final.
   - La regla se aplica a piezas canonicas principales y conserva bloqueado el `x500v2_bottom_plate` como referencia central.
   - Objetivo: evitar que piezas principales queden pegadas al finalizar, aun cuando sus direcciones radiales sean cercanas.
2. **Extremo de brazo con mas espacio vertical**:
   - Motores y helices suman mayor separacion secundaria respecto al brazo.
   - `BAN-DJ-DIAN-F2` reduce su offset local para no competir con la posicion final del motor.
   - Objetivo: mantener una lectura limpia entre propeller, motor, mount y soporte azul.
3. **Rubber/grommets siguiendo el tubo**:
   - Los grommets dejan de entrar en la logica de spacer/standoff vertical.
   - Si son fasteners primitivos, heredan el vector planar del tubo/brazo antes de aplicar su separacion local.
   - Objetivo: impedir recorridos inversos o por eje incorrecto.
4. **`GUAN-CHENG` reasignado a landing gear**:
   - Se elimina de `x500v2_rails_battery`.
   - Se incorpora a `x500v2_landing_gear` en `holybro_selection_hierarchy.json` y fallback runtime.
   - Objetivo: que seleccion, filtros, termica y explode respeten su rol fisico dentro del tren de aterrizaje.

## Criterio de validacion visual actualizado F

- `GUAN-CHENG` debe salir hacia abajo como parte del tren de aterrizaje, no como rail/battery.
- Rubber/grommets deben desplazarse con la direccion general del tubo correspondiente y separarse despues, sin invertir el eje.
- `BAN-DJ-DIAN-F2` no debe quedar pegada al motor en la pose final.
- Motores y helices deben conservar el recorrido radial del brazo y separarse verticalmente con espacio suficiente para inspeccion.
- La pose final debe mostrar margen entre piezas principales; si persiste un solape menor en subpiezas auxiliares, ajustar distancia por perfil especifico antes de tocar la regla global.
