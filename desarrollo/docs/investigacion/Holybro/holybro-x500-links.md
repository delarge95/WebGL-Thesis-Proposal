# Documentación y recursos para gemelo digital Holybro X500 V2

## 1. Documentación oficial Holybro (kit y frame)

- **PX4 Development Kit – X500 v2 (Holybro Docs)**  
  URL: https://docs.holybro.com/drone-development-kit/px4-development-kit-x500v2  
  Contenido: descripción del kit de desarrollo X500 V2 con Pixhawk 6C/6X, lista de componentes incluidos, materiales del frame, especificaciones mecánicas (wheelbase 500 mm, separación entre plates, altura del tren, peso del frame y del dron) y diagrama general del sistema.[web:7]  
  Uso: fuente de referencia global para entender la arquitectura del kit que estás replicando, qué hardware trae y cómo se organiza.

- **PX4 Development Kit – X500 v2 (Descargas oficiales)**  
  URL: https://docs.holybro.com/drone-development-kit/px4-development-kit-x500v2/download  
  Contenido: paquete `Holybro_X500_V2_3D Print.zip`, archivo `x500v2-frame.step` con el modelo CAD del frame y `AIR2216II_Motor_3D.STEP` con el modelo 3D del motor AIR2216II (KV920).[web:48]  
  Uso: geometría de referencia para tu gemelo digital; permite obtener medidas exactas de plates, brazos, tren y motor, así como generar mallas, colliders y LODs en Unity/WebGL.

- **Página de descargas global Holybro**  
  URL: https://holybro.com/pages/downloads  
  Contenido: índice de los manuales y guías de Holybro, incluyendo “X500 V2 Frame Kit Assembly Guide”, “X500 Frame Kit Assembly Guide”, manuales de S500 V2 y otros.[web:3]  
  Uso: punto central para descargar los PDFs de montaje oficiales del frame y otros componentes relacionados; útil para documentación y verificación de procesos de montaje.

- **PX4 Development Kit – X500 v2 (Holybro Store / tiendas espejo)**  
  URLs:  
  - https://holybro.com/products/px4-development-kit-x500-v2  
  - https://www.pixhawk.store/docs/drone-development-kit-px4-development-kit-x500v2-1740  
  - https://druav.com/products/holybro-x500-v2-px4-development-kit  
  Contenido: ficha comercial detallada con “Includes…”, especificando frame X500 V2, motores 2216 KV920, ESC BLHeli‑S 20 A, PDB XT60/XT30, power module PM02/PM02D, GPS M8N o M10, radio SiK, receptor RC, hélices 1045, etc.[web:8][web:50][web:52]  
  Uso: corroborar el bill of materials de alto nivel del kit que representa tu gemelo digital y elegir la combinación exacta de controlador de vuelo / GPS / radio para tu modelo.

## 2. Guías de montaje y manuales

- **X500 V2 Frame Kit Assembly Guide (PDF, inglés)**  
  URL: https://www.scribd.com/document/886886667/Holybro-X500-V2-Frame-Kit-Assembly-Guide-En  
  Contenido: guía de montaje del frame X500 V2 con despiece numerado de piezas (top plate, bottom plate, arm tubes, landing gear, platform board, rails, battery board, herrajes) y pasos de ensamblaje en fotografías y dibujos.[web:6]  
  Uso: definir la jerarquía mecánica del modelo (orden de montaje y contacto entre partes) y servir como referencia visual para la animación de “exploded view”.

- **Manuales de frame X500 / X500 V2 (Holybro Downloads)**  
  URL: https://holybro.com/pages/downloads  
  Contenido: versiones PDF oficiales de “X500 Kit_AssemblyManual.pdf”, “X500 Frame Kit Assembly Guide” y “X500 V2 Frame Kit Assembly Guide”, accesibles desde la página de descargas de Holybro.[web:3]  
  Uso: trabajar offline con los manuales, extraer ilustraciones o esquemas y validar aspectos concretos de montaje (posición del tren, routing de cables, fijación de rails, etc.).

- **Manual en español – X500 V2 PX4 Professional Development Drone Kit**  
  URL: https://manuals.plus/es/holybro/x500-v2-px4-professional-development-drone-kit-manual  
  Contenido: traducción extensa al castellano del manual de instalación del kit de desarrollo X500 V2, con descripción de componentes, conexiones y recomendaciones de seguridad.[web:14][web:16]  
  Uso: documentar tu aplicación y tu tesis en español, y contrastar términos técnicos o recomendaciones de montaje sin barrera de idioma.

## 3. Guías PX4 y BOM tipo "spreadsheet"

- **PX4 Guide – Holybro X500 V2 (Pixhawk 5X Build)**  
  URL: https://docs.px4.io/main/en/frames_multicopter/holybro_x500V2_pixhawk5x  
  Contenido: guía PX4 para el X500 V2 con Pixhawk 5X, incluye:  
  - Tabla BOM (Bill of Materials) con frame kit, motores 2216 KV880, ESC BLHeli‑S 20A, PDB, propellers 1045, Pixhawk, GPS M8N, power module, radio SiK, etc.  
  - Tablas de hardware (kit hardware, electronics) con columnas similares a una hoja de cálculo.  
  - Pasos de montaje con fotos, incluyendo la secuencia de colocar PDB, cerrar plates y posicionar el flight controller y GPS.[web:10]  
  Uso: fuente equivalente a un spreadsheet oficial para poblar tu base de datos de piezas y para entender la relación entre frame y electrónica en el ecosistema PX4.

- **PX4 Guide – Holybro X500 V2 + Pixhawk 6C (PX4 Dev Kit)**  
  URL: https://docs.px4.io/main/en/frames_multicopter/holybro_x500v2_pixhawk6c  
  Contenido: guía de montaje y configuración del PX4 Development Kit X500 V2 con Pixhawk 6C, reutiliza el mismo frame y estructura de PDB/motores/ESC.[web:5]  
  Uso: asegura que tu gemelo digital coincide con la configuración recomendada por PX4 en cuanto a puertos, número de motores, salidas PWM y dispositivos auxiliares.

- **NextPilot – Holybro X500 V2 + Pixhawk 6C (mirror)**  
  URL: https://www.nextpilot.org/manual/09.%E6%9C%BA%E5%9E%8B%E8%B0%83%E5%8F%82/frames_multicopter/holybro_x500v2_pixhawk6c.html  
  Contenido: copia actualizada de la guía PX4, a veces con secciones más detalladas sobre railes, battery board y fijación de módulos.[web:49]  
  Uso: redundancia en caso de cambios en PX4 Docs y fuente adicional para detalles concretos de montaje mecánico.

## 4. CAD/STEP/STL del frame y componentes mecánicos

- **Descargas CAD oficiales X500 V2 (frame + motor)**  
  URL: https://docs.holybro.com/drone-development-kit/px4-development-kit-x500v2/download  
  Contenido:  
  - `x500v2-frame.step`: modelo CAD completo del frame (top/bottom plates, brazos, tren, rails y battery board).  
  - `AIR2216II_Motor_3D.STEP`: modelo 3D del motor AIR2216II 920KV (idéntico al usado en el X500 V2 Dev Kit).  
  - `Holybro_X500_V2_3D Print.zip`: paquete con piezas imprimibles relacionadas con el X500 V2.[web:48]  
  Uso: base geométrica para el gemelo digital: escalado exacto, visualización de volúmenes, colisionadores y simulación de masas; el STEP del motor facilita una representación fiel del conjunto propulsor.

- **Marathon OS – Holybro X500 Drone Kit (modelo CAD paramétrico)**  
  URL: https://marathon-os.com/library/holybro-x500-drone-kit-standard-680e32c74366ea6baf6e91ac  
  Contenido: modelo CAD paramétrico del X500, con vistas técnicas y descarga en formatos CAD habituales.[web:46]  
  Uso: alternativa de referencia geométrica para comparar con el modelo oficial y, si lo necesitas, para simplificar piezas para rendimiento en WebGL.

- **STL de comunidad para accesorios X500 V2**  
  - Holybro X500 V2 Battery Holder (Thingiverse): https://www.thingiverse.com/thing:6895456  
  - Búsquedas generales de piezas X500 en Yeggi: https://www.yeggi.com/q/x500+drone+parts+stl+file/  
  Contenido: diseños de soportes de batería, adaptadores y otros accesorios impresos en 3D creados por la comunidad.[web:53][web:51]  
  Uso: inspiración para variantes personalizadas en tu gemelo digital (por ejemplo, configuraciones alternativas de battery tray), manteniendo separado lo “oficial” de los mods.

## 5. CAD/STEP de componentes electrónicos (no estructurales)

- **Modelo CAD del motor AIR2216II 920KV (eléctrico/propulsor)**  
  URL: incluido en el paquete de descargas del X500 V2 Dev Kit – `AIR2216II_Motor_3D.STEP`  
  Contenido: CAD detallado del motor AIR2216II (igual a los motores 2216 KV920 del kit), con carcasa, base y eje.[web:48][web:74]  
  Uso: representación fiel del motor dentro de cada brazo para tu visor 3D, permitiendo modelos de disipación térmica o visualización de estados de carga.

- **Pixhawk 6C – modelo CAD 3D oficial**  
  URL (Holybro Docs, Pixhawk 6C): https://docs.holybro.com/autopilot/pixhawk-6c/download  
  Contenido:  
  - `Pixhawk6C-3D-CAD.stp`: modelo 3D del flight controller Pixhawk 6C.  
  - `EXT-8P-V5.STEP`: CAD del adaptador PWM asociado.[web:72]  
  Uso: modelar el controlador de vuelo como sólido realista en tu gemelo digital, con sus dimensiones y conectores exactos, y situarlo correctamente sobre el top plate.

- **Pixhawk 6C – modelos CAD adicionales (3Dfindit)**  
  URL: https://www.3dfindit.com/en/cad-bim-library/manufacturer/pixhawk-6c?path=holybro%2Fflight20controller%2Fpixhawk206c  
  Contenido: modelos CAD descargables del Pixhawk 6C en múltiples formatos (STEP, SOLIDWORKS, etc.).[web:70]  
  Uso: si necesitas variantes de nivel de detalle o formatos específicos para tu pipeline (por ejemplo, conversión directa a formatos CAD compatibles con tus herramientas de modelado).

- **Pixhawk baseboards – modelos CAD simples**  
  URL: https://docs.holybro.com/autopilot/pixhawk-baseboards/download  
  Contenido: archivos `Pixhawk-Standard-Base-3D-simple.stp`, `Pixhawk-Mini-Base-3D-simple.stp` y otros.[web:71]  
  Uso: referencia geométrica para placas base y adaptadores que puedan formar parte de futuras extensiones de tu gemelo digital (por ejemplo, si añades companion computer).

- **Pixhawk 6C Mini – modelos CAD**  
  URL: https://docs.holybro.com/autopilot/pixhawk-6c-mini/download  
  Contenido: `Pixhawk6c-mini-3D-CAD.stp` y `Pixhawk6C_modelB.stp`, modelos 3D de las variantes mini.[web:69]  
  Uso: útil si en el futuro quieres ampliar el proyecto a otras variantes de hardware con footprint más pequeño manteniendo la misma lógica visual.

*(No se han encontrado STEP/STL oficiales específicos para el módulo GPS M10 ni para la radio SiK del kit X500 V2; sin embargo, el módulo M10 suele venir con un soporte de carbono cuyo CAD puede estar incluido en algunos paquetes de accesorios, y el volumen aproximado puede inferirse de sus dimensiones en las fichas técnicas.)*

## 6. Especificaciones técnicas de motores, ESC, GPS y hélices

- **Motor 2216‑920KV (Air Gear 450II / AIR2216II)**  
  URL: https://www.ligpower.com/product/air-gear-450II-combo-set.html  
  Contenido: especificaciones completas del motor AIR2216II KV920 y del ESC AIR 20A (corrientes máximas, potencias, curvas empuje–corriente con hélice T1045, temperaturas de operación y pesos).[web:74]  
  Uso: base numérica precisa para el cálculo de potencia, empuje y disipación térmica en el gemelo digital (especialmente útil para tus campos `Power_Consumption_Watts` y rangos de temperatura).

- **Holybro S500 V2 Motor 2216‑920KV (como repuesto oficial)**  
  URL: https://robu.in/product/holybro-s500-v2-motor-2216-920kv-cw/  
  Contenido: especificaciones de motor 2216‑920KV para S500 V2 (peso, rango de voltaje, compatibilidad con hélices 10x4.5).[web:24]  
  Uso: confirmación adicional de peso y rango de operación de los motores de la misma familia que el X500 V2.

- **Holybro BLHeli‑S 20A ESC (repuestos)**  
  URLs:  
  - https://mgsl.in/products/holybro-blheli-s-20a-esc  
  - https://www.flyingtech.co.uk/product/holybro-blheli-s-esc-20a-2-4s-speed-controller/  
  Contenido: especificaciones del ESC (20A continuos, 30A ráfaga, 2–4S, sin BEC, peso, dimensiones físicas).[web:26][web:29]  
  Uso: estimar disipación térmica y límites de corriente para el modelado energético de tu twin.

- **Holybro M10 GPS (ficha oficial)**  
  URL: https://holybro.com/products/m10-gps  
  Contenido: características del módulo M10 (GNSS Ublox M10, consumo <200 mA a 5 V, precisión, dimensiones φ50 x 14.4 mm, peso 32 g, temperatura de operación −40 a 80 °C).[web:81][web:75]  
  Uso: parámetros exactos de consumo y rango térmico del módulo GPS para tu mapa de calor y tu simulación de consumo.

- **Holybro M10 GPS – ficha alternativa**  
  URL: https://rcdrone.top/ms/products/holybro-m10-gps-module  
  Contenido: especificaciones tabuladas del M10 (tensión de entrada, consumo, dimensiones, peso) en formato tabla comparativa M10 vs M9N.[web:78]  
  Uso: corroborar los datos de la ficha oficial y disponer de una tabla fácilmente exportable a hojas de cálculo.

- **S500 ARF V2 – hélices y combo propulsor**  
  URL: https://www.getfpv.com/holybro-s500-arf-v2-480mm-quadcopter-platform-frame-kit.html  
  Contenido: confirma el uso de hélices 1045 y motores 2216 con ESC 20A en un frame similar.[web:28]  
  Uso: referencia cruzada para las hélices del X500 V2 y para validar el peso típico de hélices 1045.

## 7. Material audiovisual de referencia

- **Holybro X500v2 Build / Initial Setup (ejemplo)**  
  URL: https://www.youtube.com/watch?v=eOvRDTr4e6Y  
  Contenido: vídeo de montaje y configuración inicial, incluyendo conexión de GPS CAN, brújula, receptor RC y calibración básica.[web:47]  
  Uso: verificar en 3D la posición real de cada componente electrónico en el frame para ajustar tus offsets y jerarquía en la vista explosionada.

- **Pixhawk 6C – vídeo de configuración (painless360)**  
  URL: https://www.youtube.com/watch?v=_ketmb8u2UI  
  Contenido: explica instalación y flasheo del Pixhawk 6C, conexiones de GPS y PWM, etc.[web:73]  
  Uso: entender el wiring real del controlador de vuelo y su interacción con ESC, GPS y radio, para que tu UI pueda representar también estados lógicos (arming, modos, etc.).
