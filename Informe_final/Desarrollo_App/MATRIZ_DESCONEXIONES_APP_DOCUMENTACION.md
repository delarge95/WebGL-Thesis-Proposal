# Matriz Maestra de Desconexiones App vs Documentacion

| Afirmacion documental | Estado real en codigo/UI | Accion correctiva | Destino final |
| --- | --- | --- | --- |
| El sistema final trabaja con `16` piezas | La taxonomia canonica del proyecto usa `28` piezas, `30` anchors y `257` renderers/colliders | reemplazar la cifra final por la convencion `28 / 30 / 257`; dejar `16` solo como antecedente historico explicado | cuerpo principal |
| El manual de usuario documenta orbit con clic izquierdo y pan con clic derecho | `OrbitCameraController` usa clic derecho para orbit y clic central para pan | reescribir controles oficiales | cuerpo principal |
| El manual de usuario documenta `F3`, `F12`, `1-6`, `E` y otros atajos | no hay evidencia de esos atajos como flujo visible final | eliminar esos atajos del manual activo | cuerpo principal |
| La seleccion abre un panel lateral derecho | la ficha activa es un `bottom sheet` | corregir narrativa y capturas | cuerpo principal |
| El Hero muestra `DJI Phantom 4 Pro`, `Boston Dynamics Spot` y `CubeSat 3U` | placeholders ajenos al caso de estudio | reemplazar por copy honesto del Holybro X500 V2 | cuerpo principal |
| El boton `VIEW PROJECT ON GITHUB` no tenia wiring | la UI final lo mostraba sin controlador | agregar `HeroGithubBtn` y abrir el repositorio | cuerpo principal |
| Inspect promete `Measure` en tooltip | el boton de medicion esta oculto en UXML | corregir tooltip a `Pins, Isolate, Power` | cuerpo principal |
| El catalogo de piezas es parte del flujo final visible | `PartCatalogUI` existe, pero no forma parte del flujo visible de cierre | moverlo a legado/no integrado | anexo / limitaciones |
| `SettingsPanel` es una pantalla activa del producto final | existe codigo, pero no esta visible en la build final | quitarla del manual y documentarla como no integrada | anexo / limitaciones |
| El audio hace parte del producto final | no hay clips de audio en `Assets` | reclasificar audio como trabajo futuro mientras no exista integracion real | trabajo futuro |
| La propuesta y el informe estaban igual de formalizados metodologicamente | la propuesta estaba mas debil que el informe | reforzar la propuesta con variables, muestra, protocolo y analisis | cuerpo principal |
| El manual tecnico listaba modulos inexistentes (`WebGLOptimizer`, `TooltipSystem`, etc.) | esos nombres no corresponden al estado real final | eliminarlos del manual tecnico y de la arquitectura activa | cuerpo principal |
| `MeasurementTool` es una feature final visible | existe en codigo, pero no se expone en la UI final | documentarla como implementada pero oculta | anexo / limitaciones |
| Todos los view modes estan visibles en Studio | `ViewModeManager` implementa 7, pero la UI final no expone todos | separar modos visibles vs ocultos | cuerpo principal + anexo |
| Houdini es el pipeline oficial final | el pipeline final defendible se apoya en Blender + Unity; Houdini queda evaluado/experimental | reclasificar Houdini | anexo / trabajo futuro |
