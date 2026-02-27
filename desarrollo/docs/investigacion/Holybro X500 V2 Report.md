# Gemelo digital Holybro X500 V2 – Especificaciones de hardware

## Fuentes oficiales y documentación

- **Descargas oficiales Holybro (manuales y guías X500/X500 V2)**: sección de descargas con los PDF de montaje de marcos X500/X500 V2 y otros kits, incluyendo "X500 V2 Frame Kit Assembly Guide" y manuales de kits de desarrollo.[^1][^2]
- **Holybro PX4 Development Kit – X500 v2 (ficha oficial)**: descripción completa del kit X500 V2 con Pixhawk 6C, lista de contenidos, materiales (fibra de carbono, conectores de nylon reforzado), especificaciones mecánicas (wheelbase 500 mm, altura tren, separación entre plates, peso 610 g) y descarga de archivo CAD 3D del frame.[^3][^4][^5]
- **PX4 Guide – Holybro X500 V2 ARF Kit (Pixhawk 5X build)**: guía paso a paso de montaje del X500 V2 ARF kit, con bill of materials, despiece del frame (top/bottom plate, brazos en tubo de 16 mm, tren de aterrizaje, railes de 10 mm), PDB, motores 2216 KV880, ESC BLHeli‑S 20 A, hélices 1045 y electrónica asociada.[^6][^7]
- **PX4 Guide – Holybro X500 V2 + Pixhawk 6C (PX4 Dev Kit)**: guía de montaje y configuración PX4 específica para el Dev Kit X500 V2 con Pixhawk 6C, utilizando el mismo frame y arquitectura de PDB/motores/ESC que el ARF.[^8]
- **Getting Started Build Guide – Holybro Docs (X500v2)**: guía de inicio para el PX4 Development Kit X500 v2, enlazada desde la documentación oficial de Holybro para el kit de desarrollo.[^9][^3]
- **Manual de montaje X500 V2 (inglés, PDF)**: guía de montaje del frame X500 V2 alojada en Scribd, que reproduce el folleto oficial de Holybro con numeración de piezas (top plate #510179, bottom plate #510180, tubos de brazo #510177, landing gear #510192, etc.).[^10]
- **Manual en español X500 v2 PX4 Professional Development Drone Kit**: traducción no oficial pero detallada del manual de instalación del X500 v2 publicada en Manuals.plus, útil como referencia en castellano para el flujo de montaje.[^11][^12]
- **PX4 Autopilot – página de producto Holybro X500 v2**: ficha de proyecto PX4 para el Holybro X500 v2, confirmando el uso de módulo GPS M10, radio de telemetría SiK V3 433/915 MHz, frame kit X500 V2 y motores Holybro 2216 KV920 en las revisiones más recientes.[^13]
- **Tiendas y distribuidores que confirman especificaciones del ARF**: FlyingTech X500 V2 ARF Kit (motores 2216 KV880, ESC BLHeli‑S 20 A, PDB XT60/XT30), DroneDynamics y BalticDrones (Pixhawk 6C o 6X, M10 GPS, SiK Telemetry, frame de 610 g, frame kit ≈365 g, motores 2216 KV920, ESC 20 A, hélices 1045, PDB XT60/XT30, CAD/3D print file referenciado como "Holybro_X500_V2_3D Print").[^14][^5][^15]
- **Páginas de repuestos Holybro y S500 V2**: confirman la familia de motores (2216‑880KV y 2216‑920KV), ESC BLHeli‑S 20 A y hélices 1045 usados en S500 V2 y X500 V2, así como las opciones de repuesto.[^16][^17][^18]
- **Ficha de motor Holybro 2216‑920KV (S500 V2)**: especifica un diámetro de motor de 28 mm, peso ≈63 g y compatibilidad 3–4S, útil como referencia directa para el peso y rango de operación del motor 2216 usado en X500 V2.[^19]
- **Ficha de ESC Holybro BLHeli‑S 20 A**: ESC de 20 A continuos y 30 A en ráfaga, sin BEC, para LiPo 2–4S, usado como repuesto para X500/X500 V2; se confirma también en distribuidores como FlyingTech.[^20][^21]
- **Referencias adicionales de consumo de motor 2216 KV880**: discusiones técnicas y resúmenes de kits Holybro S500 V2 dan corrientes típicas de 3,5–16,2 A para motores 2216 KV880 en configuración 4S (útiles para estimar potencia máxima).[^22]

## Notas sobre inferencias y aproximaciones

- El peso total del frame kit X500 V2 (sin electrónica) se reporta en ≈365 g, y el peso del dron vacío (sin batería) en ≈610 g, a partir de fichas de Holybro y distribuidores. La masa de cada subconjunto estructural (plates, brazos, tren) se ha repartido proporcionalmente para fines del gemelo digital (no son valores oficiales desglosados).[^4][^5][^15][^3]
- Las versiones antiguas de X500 V2 ARF usan motores Holybro 2216 KV880, mientras que los kits PX4 Development Kit X500 v2 más recientes con Pixhawk 6C listan motores 2216 KV920; en ambos casos se mantiene el mismo factor de forma 2216 y ESC BLHeli‑S 20 A.[^5][^6][^14][^13]
- No se encontró una ficha pública del módulo GPS Holybro M10 específico del kit X500 V2, pero Holybro y distribuidores confirman el uso de un módulo M10 con brújula integrada; se han estimado el peso y consumo a partir de módulos GNSS M8/M10 equivalentes.[^4][^5][^13]
- Las potencias máximas en W para motores y ESC se han estimado a partir de la corriente máxima indicada (16,2 A reportados para 2216 KV880 en 4S) y el límite continuo del ESC (20 A, LiPo 2–4S), multiplicando por el voltaje nominal de 4S (14,8 V).[^21][^22][^20]
- Para Flight Controllers (Pixhawk 4 / Pixhawk 6C), módulo GPS M10 y radio de telemetría SiK, se han usado consumos típicos de esa clase de hardware (1–3 W) coherentes con la arquitectura PX4, escalados conservadoramente para mapear disipación térmica en el gemelo digital.

## Datos de piezas (JSON para `DronePartData`)

```json
[
  {
    "Categoria": "Chasis_Top_Bottom_Plates",
    "Nombre_Comercial": "Holybro X500 V2 Frame Kit - Carbon Fiber Top & Bottom Plates",
    "Material_Principal": "Fibra de carbono twill 2 mm",
    "Peso_gramos": 150,
    "Operating_Temp_C_Min_Max": "-20 - 80",
    "Power_Consumption_Watts": 0,
    "Breve_Descripcion_Tecnica": "Conjunto de top y bottom plates de fibra de carbono de 144 x 144 mm y 2 mm de espesor que forman el núcleo estructural del fuselaje del X500 V2. Sirven de interfaz de montaje para la PDB, el flight controller, el tren de aterrizaje y los railes de carga, garantizando rigidez torsional y protección básica de la electrónica central."
  },
  {
    "Categoria": "Arm_Tubes",
    "Nombre_Comercial": "Holybro X500 V2 Arm - 16 mm Carbon Fiber Tube with Nylon Connectors",
    "Material_Principal": "Tubo de fibra de carbono de 16 mm con conectores de nylon reforzado",
    "Peso_gramos": 120,
    "Operating_Temp_C_Min_Max": "-20 - 80",
    "Power_Consumption_Watts": 0,
    "Breve_Descripcion_Tecnica": "Conjunto de cuatro brazos en tubo de fibra de carbono de 16 mm de diámetro con conectores de nylon reforzado en extremos de motor y fuselaje. Transfieren las cargas de empuje y momento de los motores al chasis, proporcionando una alta relación rigidez/peso y absorción parcial de vibraciones."
  },
  {
    "Categoria": "Landing_Gear",
    "Nombre_Comercial": "Holybro X500 V2 Landing Gear - Carbon Fiber Tubes with Plastic Connectors",
    "Material_Principal": "Tubos de fibra de carbono de 16 mm y 10 mm con conectores plásticos reforzados",
    "Peso_gramos": 80,
    "Operating_Temp_C_Min_Max": "-20 - 80",
    "Power_Consumption_Watts": 0,
    "Breve_Descripcion_Tecnica": "Tren de aterrizaje formado por tubos de fibra de carbono de 16 y 10 mm con conectores plásticos reforzados en configuración de patín. Proporciona la altura libre (~215 mm) necesaria para carga útil y protege el fuselaje ante impactos de toma de contacto y pequeñas irregularidades del terreno."
  },
  {
    "Categoria": "Motor",
    "Nombre_Comercial": "Holybro 2216 KV880 / KV920 Brushless Motor",
    "Material_Principal": "Aleación de aluminio mecanizado y acero, bobinados de cobre esmaltado",
    "Peso_gramos": 63,
    "Operating_Temp_C_Min_Max": "-20 - 80",
    "Power_Consumption_Watts": 240,
    "Breve_Descripcion_Tecnica": "Motor brushless externo tipo 2216 con KV 880–920 optimizado para hélices 1045 y LiPo 4S, con peso aproximado de 63 g y diámetro de carcasa ~28 mm. Proporciona el empuje principal del cuadricóptero, operando con corrientes típicas hasta ~16 A en vuelo y empujes máximos del orden de varios cientos de gramos por motor según la hélice y el régimen de giro."
  },
  {
    "Categoria": "ESC",
    "Nombre_Comercial": "Holybro BLHeli-S 20A ESC",
    "Material_Principal": "PCB FR4 con encapsulados de silicio y disipadores de cobre",
    "Peso_gramos": 7,
    "Operating_Temp_C_Min_Max": "-20 - 80",
    "Power_Consumption_Watts": 300,
    "Breve_Descripcion_Tecnica": "Regulador electrónico de velocidad BLHeli-S de 20 A continuos (30 A ráfaga) para LiPo 2–4S, sin BEC integrado y con soporte de protocolos PWM, Oneshot y DShot. Conmuta la potencia suministrada al motor brushless 2216 manteniendo un control de par y velocidad eficiente, actuando como etapa de potencia puente entre la PDB y cada motor."
  },
  {
    "Categoria": "PDB",
    "Nombre_Comercial": "Holybro X500 V2 Power Distribution Board (XT60 + XT30)",
    "Material_Principal": "PCB FR4 con pistas de cobre de alta corriente y conectores XT60/XT30",
    "Peso_gramos": 20,
    "Operating_Temp_C_Min_Max": "-20 - 80",
    "Power_Consumption_Watts": 0,
    "Breve_Descripcion_Tecnica": "Placa de distribución de potencia con entrada XT60 para batería principal y salidas XT30 para ESC y periféricos de alta corriente. Distribuye de forma centralizada la energía del paquete LiPo 4S hacia los cuatro ESC, módulos auxiliares y el power module sin realizar conversión activa de tensión, actuando como bus de potencia del sistema."
  },
  {
    "Categoria": "Flight_Controller_Pixhawk_4_6C",
    "Nombre_Comercial": "Pixhawk 4 / Pixhawk 6C Autopilot",
    "Material_Principal": "PCB FR4 con encapsulados electrónicos, carcasa plástica",
    "Peso_gramos": 35,
    "Operating_Temp_C_Min_Max": "-20 - 70",
    "Power_Consumption_Watts": 2.5,
    "Breve_Descripcion_Tecnica": "Controlador de vuelo de la familia Pixhawk (H7/H7 dual-core en el caso de Pixhawk 6C) con IMUs redundantes, sensores barométricos y entradas/salidas PWM/UART CAN. Ejecuta el firmware PX4 o ArduPilot, realiza fusión de sensores y genera los comandos PWM/DShot para los ESC, además de gestionar modos de vuelo autónomo y enlaces con radio y telemetría."
  },
  {
    "Categoria": "GPS_Module",
    "Nombre_Comercial": "Holybro M10 GPS Module with Integrated Compass",
    "Material_Principal": "PCB con antena cerámica GNSS, encapsulados electrónicos y carcasa plástica",
    "Peso_gramos": 25,
    "Operating_Temp_C_Min_Max": "-20 - 70",
    "Power_Consumption_Watts": 0.5,
    "Breve_Descripcion_Tecnica": "Módulo GNSS basado en receptor de la serie M10 con antena cerámica y brújula integrada, conectado al flight controller por UART/I2C. Proporciona posición 3D, velocidad y rumbo absoluto, sirviendo como referencia primaria para navegación, control de posición y operaciones autónomas basadas en waypoints."
  },
  {
    "Categoria": "Telemetry_Radio",
    "Nombre_Comercial": "Holybro SiK Telemetry Radio V3 433/915 MHz (Air Unit)",
    "Material_Principal": "PCB FR4 con módulo RF y carcasa plástica ligera",
    "Peso_gramos": 15,
    "Operating_Temp_C_Min_Max": "-20 - 60",
    "Power_Consumption_Watts": 1.0,
    "Breve_Descripcion_Tecnica": "Módulo de radio telemétrica SiK V3 en banda 433 o 915 MHz que establece un enlace bidireccional entre el dron y la estación de tierra. Transmite telemetría en tiempo real, permite la carga de misiones y el ajuste de parámetros del autopiloto, actuando como canal de datos primario en entornos de desarrollo y pruebas."
  },
  {
    "Categoria": "Propeller",
    "Nombre_Comercial": "Holybro 1045 Propeller (10x4.5)",
    "Material_Principal": "Compuesto plástico reforzado (nylon o ABS reforzado)",
    "Peso_gramos": 8,
    "Operating_Temp_C_Min_Max": "-20 - 70",
    "Power_Consumption_Watts": 0,
    "Breve_Descripcion_Tecnica": "Hélice fija tipo 1045 (10 x 4,5 pulgadas) optimizada para motores 2216 en configuración 4S, suministrada en juegos de CW/CCW. Convierte el par del motor en empuje vertical y define junto con el KV del motor el punto de operación, empuje máximo y eficiencia propulsiva del cuadricóptero."
  }
]
```

## Detalle técnico por subconjunto

### Chasis: top & bottom plates

El frame kit X500 V2 utiliza top y bottom plates de fibra de carbono de 144 x 144 mm y 2 mm de grosor, con patrones de taladros para la PDB, Pixhawk, railes de carga y monturas de cámara, y un peso total de frame (incluyendo brazos, tren y herrajes) de ~365 g. Las plates forman un “sándwich” estructural que encierra la bahía electrónica y proporciona el plano de referencia para la geometría del gemelo digital (separación entre plates ≈28 mm, wheelbase total 500 mm).[^15][^3][^6][^5][^4]

### Tubería de los brazos

Los cuatro brazos consisten en tubos de fibra de carbono de 16 mm de diámetro con conectores de nylon reforzado, que se fijan en alojamientos rectangulares mecanizados en el bottom plate y se bloquean con tornillería M3. Cada brazo integra preinstalados un motor 2216 y un ESC BLHeli‑S 20 A que discurre internamente por el tubo hasta la PDB, lo que simplifica el cableado y mejora la estética del modelo en el visor 3D.[^10][^3][^6][^14][^5]

### Tren de aterrizaje

El tren está formado por dos patines cruzados montados con tubos de carbono de 16 mm (verticales) y 10 mm (barras transversales), unidos mediante conectores plásticos reforzados y espuma en la base para amortiguar vibraciones. Proporciona una altura al suelo de unos 215 mm, suficiente para albergar gimbals o cámaras sobre los railes inferiores, y transfiere las cargas de impacto al conjunto de plates y brazos sin comprometer la geometría de la hélice.[^3][^6][^14][^5][^15]

### Motores 2216 KV880/KV920

Las versiones ARF originales del X500 V2 montan motores Holybro 2216 KV880, mientras que los PX4 Development Kit con Pixhawk 6C/6X más recientes especifican motores 2216 KV920. Un motor Holybro 2216‑920KV para S500/X500 pesa alrededor de 63 g, tiene un diámetro de ~28 mm y está diseñado para operar con LiPo 3–4S y hélices 10x4,5, lo que permite inferir masas y comportamiento térmico equivalentes para el gemelo digital. Datos de consumo de un kit S500 V2 con motores 2216 KV880 indican corrientes entre 3,5 A y 16,2 A por motor en 4S, lo que sitúa la potencia pico por motor en el orden de 200–250 W a 14,8 V.[^17][^6][^14][^19][^22][^5][^13]

### ESCs BLHeli-S 20 A

El X500 V2 ARF y los kits de desarrollo utilizan ESCs Holybro BLHeli‑S de 20 A continuos y 30 A en ráfaga, diseñados para LiPo 2–4S, con MCU EFM8BB21 y soporte de protocolos PWM, Oneshot y DShot, y sin BEC integrado. Estos ESC tienen un tamaño compacto (del orden de 25 x 13 mm) y un peso típico de 5–7 g, permitiendo su alojamiento dentro de los brazos de 16 mm y asegurando una disipación térmica adecuada cuando se modela un rango operativo de temperatura de hasta ~80 °C en el mapa de calor.[^23][^6][^14][^20][^21]

### Power Distribution Board (PDB)

La PDB del X500 V2 proporciona un conector XT60 para la batería principal y múltiples XT30 para ESC y periféricos, eliminando la necesidad de soldaduras para el usuario final. Se monta en el bottom plate y actúa como bus central de alimentación, desde el que salen los cables hacia los ESC de cada brazo y hacia el power module PM02/PM02D que mide tensión y corriente del sistema para el Pixhawk.[^8][^6][^14][^5][^3][^4]

### Flight Controller Pixhawk 4 / Pixhawk 6C

Los X500 V2 ARF clásicos se suministran habitualmente con Pixhawk 4 o Pixhawk 5X, mientras que el PX4 Development Kit – X500 v2 incorpora Pixhawk 6C o 6X, manteniendo la misma arquitectura de montaje sobre el top plate mediante adhesivo o standoffs. El controlador de vuelo se coloca centrado sobre el top plate, con la flecha alineada con el morro del frame, utilizando un power module (PM02/PM02D/PM02 V3) para alimentar el bus de 5 V y sensar la corriente del sistema, lo que justifica un consumo combinado del orden de 2–3 W para el autopiloto en el gemelo digital.[^6][^5][^13][^8][^3]

### Módulo GPS M8/M10

Los primeros X500 V2 ARF suelen incluir un módulo GPS M8N con brújula integrada, mientras que los PX4 Development Kit X500 v2 con Pixhawk 6C/6X listan explícitamente un módulo M10 GPS Module. El módulo se monta sobre la platform board superior, fijado con tornillería M2.5, separado del flight controller para minimizar interferencias magnéticas y conectado por UART/I2C, con un consumo típico inferior a 0,5 W y masa del orden de 20–30 g.[^13][^15][^3][^4][^6]

### Radio de telemetría SiK 433/915 MHz

Los kits X500 V2 incluyen una pareja de radios Holybro SiK Telemetry Radio V3 en 433 o 915 MHz, una unidad aérea y una unidad de estación de tierra, que proporcionan un enlace de datos de baja latencia para PX4/ArduPilot. La unidad aérea se conecta al Pixhawk mediante UART, soporta telemetría bidireccional, carga de misiones y ajuste de parámetros, y típicamente opera con potencias de salida del orden de 100 mW, lo que implica consumos alrededor de 1 W en transmisión continua.[^5][^4][^13]

### Hélices 1045

El ARF kit especifica hélices 1045 (10x4,5) suministradas en juegos de 4–6 piezas, con versiones CW y CCW, optimizadas para los motores 2216 KV880/KV920 en 4S. Hélices 1045 estándar en material plástico reforzado tienen masas típicas de 7–9 g por pieza, por lo que un valor nominal de 8 g por hélice es adecuado para el cálculo de inercias de giro y distribución de masa en el gemelo digital.[^18][^14][^6]

## Workflow de apilado en la bahía central

### Arquitectura estructural básica

- **Separación de plates y geometría**: La distancia fija entre top y bottom plates es de aproximadamente 28 mm, con un wheelbase de 500 mm y un tren de aterrizaje que proporciona una altura de unos 215 mm sobre el suelo.[^15][^3][^4][^5]
- **Ubicación de la bahía electrónica**: La bahía central se define como el volumen entre las dos plates de carbono, donde se alojan la PDB, el power module y el cableado principal, mientras que el flight controller se monta sobre la cara superior del top plate.[^8][^6]

### Orden típico de apilado vertical (de abajo hacia arriba)

1. **Bottom plate**: Es la base rígida en la que se fijan el tren de aterrizaje, los brazos y la PDB; incorpora taladros para la XT60 holder, standoffs y clips del sistema de railes.[^10][^6]
2. **PDB y power module (PM02/PM02D/PM02 V3)**: La PDB se atornilla directamente al bottom plate, generalmente centrada, con la entrada XT60 orientada hacia la parte posterior o lateral según el manual. El power module se inserta en serie entre la batería (XT60) y la PDB y se fija con bridas o pads adhesivos cerca de la PDB dentro de la misma bahía.[^3][^6][^5]
3. **Paso de cables de los ESC por los brazos**: Desde cada brazo, los pares de potencia de los ESC (con conectores XT30 preinstalados) se introducen por el interior del brazo y emergen en la bahía central, donde se conectan a los puertos XT30 de la PDB antes de cerrar el sándwich de plates.[^14][^6][^3]
4. **Cierre del conjunto con top plate**: Una vez fijados brazos, tren y PDB, se coloca el top plate alineando las guías rectangulares de los brazos y asegurándolo mediante tornillería M3 con tuercas nyloc, encapsulando la PDB y el power module en la bahía.[^6][^10]
5. **Montaje del flight controller sobre el top plate**: El Pixhawk (4/5X/6C) se fija centrado sobre la cara superior del top plate usando almohadillas adhesivas o standoffs, con la flecha de referencia orientada hacia el frente del frame. Desde aquí se enrutan hacia abajo los cables de alimentación y señales (power module, ESC, telemetría, RC, etc.) a través de los pasos de cable previstos en el top plate.[^8][^6]
6. **Ubicación de la radio de telemetría y receptor RC**: El receptor RC suele fijarse junto al Pixhawk, sobre el top plate o en uno de los laterales del frame, mientras que la unidad aérea de la radio SiK se sitúa en un lateral o bajo la platform board, con antena orientada hacia arriba o lateralmente para máxima cobertura.[^5][^3]
7. **Platform board y módulo GPS**: Por encima del top plate, separado mediante standoffs M2.5, se monta la platform board, que dispone de taladros para el GPS y un posible companion computer (Raspberry Pi/Jetson Nano). El módulo GPS M8/M10 se coloca centrado o ligeramente retrasado en la platform board, con el mástil y la carcasa alejados del plano de hélices y de los campos magnéticos generados por la PDB y los cables de alta corriente.[^13][^3][^6]
8. **Rails inferiores y batería**: Bajo el bottom plate se instalan las barras de rail de 10 mm y el battery mounting board con sus hangers de goma, sobre los que se fija la batería mediante straps; aunque estructuralmente están fuera de la bahía central, son críticos para la distribución de masas y la animación de la vista explosionada.[^3][^6][^5]

### Implicaciones para la vista explosionada paramétrica

- **Eje de referencia Z**: Puede definirse el plano del bottom plate como Z = 0, con la PDB y power module en Z ≈ 2–4 mm, el top plate en Z ≈ 28 mm, el Pixhawk en Z ≈ 35–40 mm y la platform board + GPS en Z ≈ 45–55 mm; por debajo, los railes y la batería ocupan un rango de Z negativo (~‑30 a ‑50 mm), lo que facilita una animación de explosión vertical limpia.[^6][^5][^3]
- **Jerarquía mecánica**: Para la lógica de la UI, la jerarquía puede representarse como: frame_root → bottom_plate → (PDB, power_module, rail_system) → arms → landing_gear → top_plate → (flight_controller, RC_receiver, telemetry_radio) → platform_board → GPS_module → companion_computer. Esto refleja la secuencia real de montaje y permite animar desmontajes coherentes mecánicamente.[^8][^3][^6]
- **Rutas de cableado en el gemelo digital**: Los cables de alta corriente (batería–PM–PDB–ESC) discurren principalmente por la bahía central y el interior de los brazos, mientras que los buses de señal (UART, I2C, CAN) se concentran en la parte superior alrededor del Pixhawk y la platform board. Modelar estos haces de cables como volúmenes diferenciados en la vista explosionada ayuda a comunicar rutas de potencia frente a rutas de datos.[^3][^6]

---

## References

1. [Downloads – Holybro Store](https://holybro.com/pages/downloads) - QAV250 Kit Quick Start Guide · S500 V2 Kit_Assembly Manual · X500 Kit_Assembly Manual · X500 Frame K...

2. [Drone Dev Kit - Holybro Docs](https://docs.holybro.com/company/user-manuals/drone-dev-kit) - pdf. PDF. Download Open. 4MB. X500-Kit_AssemblyManual.pdf. PDF. Download Open. 646KB. Holybro_X500_F...

3. [PX4 Development Kit - X500v2 - Holybro Docs](https://docs.holybro.com/drone-development-kit/px4-development-kit-x500v2) - It is quick and easy to assemble (~30 minutes) without the need for soldering, so you can spend more...

4. [PX4 Development Kit - X500 v2 – Holybro Store](https://holybro.com/products/px4-development-kit-x500-v2) - An affordable, lightweight, and robust carbon fiber professional development drone kit with the late...

5. [FPV Kit Holybro X500 V2 kit Pixhawk 6C PM02 M10 GPS 433MHZ ...](https://balticdrones.eu/products/fpv-kit-holybro-x500-v2-kit-pixhawk-6c-pm02-m10-gps-433mhz-radio) - - Reliable data link: The SiK telemetry radio (433 MHz) provides stable two-way communication for te...

6. [Holybro X500 V2 (Pixhawk 5X Build) | PX4 Guide (main)](https://docs.px4.io/main/en/frames_multicopter/holybro_x500V2_pixhawk5x) - This topic provides full instructions for building the Holybro X500 V2 ARF Kit and configuring PX4 u...

7. [Holybro X500 V2 (Pixhawk 5X Build) | PX4 Guide (main)](https://docs.px4.io/main/zh/frames_multicopter/holybro_x500V2_pixhawk5x) - Holybro Motors - 2216 KV880 x6 (superseded - check spare parts list for current version). Holybro BL...

8. [Holybro X500 V2 + Pixhawk 6C (PX4 Dev Kit) | PX4 Guide (main)](https://docs.px4.io/main/en/frames_multicopter/holybro_x500v2_pixhawk6c) - This topic provides full instructions for building the Holybro X500 V2 ARF Kit, also known as the Ho...

9. [Getting Started Build Guide - Holybro Docs](https://docs.holybro.com/drone-development-kit/px4-development-kit-x500v2/getting-started-build-guide) - Drone Development Kit · PX4 Development Kit - X500v2. Getting Started Build Guide. Build Guide & PX4...

10. [Holybro X500 V2 Frame Kit Assembly Guide en | PDF - Scribd](https://www.scribd.com/document/886886667/Holybro-X500-V2-Frame-Kit-Assembly-Guide-En) - Holybro X500 V2 Frame Kit Assembly Guide En - Free download as PDF File (.pdf), Text File (.txt) or ...

11. [Guía de instalación del kit de desarrollo profesional para drones ...](https://manuals.plus/es/holybro/x500-v2-px4-professional-development-drone-kit-manual) - Aprenda a ensamblar el kit de desarrollo profesional para drones X500 v2 PX4 con este manual de usua...

12. [Manuales y guías de usuario de Holybro - Manuals+ - Manuals.plus](https://manuals.plus/es/category/holybro) - Guía de montaje del X500 v2 Kit de dron de desarrollo profesional X500 v2 PX4 *Guía de instalación d...

13. [Holybro X500 v2 - PX4 Autopilot](https://px4.io/project/s500-v2-2-2/) - M10 GPS Module; SiK Telemetry Radio V3 433/915MHz; X500 V2 Frame Kit (SKU30120); Preinstalled Items:...

14. [Holybro X500 V2 Quadcopter Frame Kit - Flying Tech](https://www.flyingtech.co.uk/product/holybro-x500-v2-quadcopter-frame-kit/) - The X500 V2 is an affordable, lightweight, and robust carbon fibre professional drone kit that is ea...

15. [Holybro PX4 Development Kit X500 V2 (Pixhawk 6C + M10 GPS + ...](https://dronedynamics.ca/products/holybro-px4-development-kit-x500-v2-pixhawk-6c-m10-gps-915mhz-telemtry) - Specifications: Wheelbase: 500mm; Motor mount pattern: 16x16mm & 19x19mm; Frame Body: 144x144mm, 2mm...

16. [Holybro S500 V2 + Pixhawk 4 Build | PX4 Guide (main)](https://docs.px4.io/main/en/frames_multicopter/holybro_s500_v2_pixhawk4) - Motors - 2216 KV880（ V2 Update）; Propeller 1045（ V2 Update）; Pixhawk4 GPS; Fully assembled Power Man...

17. [Spare Parts-S500 V2 Kit - Holybro](https://holybro.com/products/spare-parts-s500-v2-kit) - Motor 2216-920KV-CW (1PC). Motor 2216-920KV-CCW (1PC). S500 V2-BLHeli S 20A ESC(1PC). Motor 2216-880...

18. [Holybro S500 ARF V2- 480mm Quadcopter Platform - Frame Kit](https://www.getfpv.com/holybro-s500-arf-v2-480mm-quadcopter-platform-frame-kit.html) - Specifications. Arms use high strength plastics; Motors: 2216 KV880; Propeller: 1045; Power Manageme...

19. [Holybro S500 V2 Motor 2216-920KV-CW - Robu.in](https://robu.in/product/holybro-s500-v2-motor-2216-920kv-cw/) - Holybro S500 V2 Motor 2216-920KV-CW ; Motor Diameter(mm): 28 ; Motor Weight(gm): 63 ; Max Lipo Cell ...

20. [Holybro BLHeli S 20A ESC - MG Super Labs](https://mgsl.in/products/holybro-blheli-s-20a-esc) - Description

21. [Holybro BLHeli S ESC 20A 2-4S Speed Controller - Flying Tech](https://www.flyingtech.co.uk/product/holybro-blheli-s-esc-20a-2-4s-speed-controller/) - ESC maximum speed is 500k eRPM. ○ BLHeli-S firmware is designed for superior performance in multirot...

22. [Holybro S500 V2 Kit : r/diydrones - Reddit](https://www.reddit.com/r/diydrones/comments/p6e9rg/holybro_s500_v2_kit/) - Engine: 2216 KV880 · Current consumption motor: 3.5 - 16.2 A · Engine speed: 0.07 - 0.18 Nm · Engine...

23. [BLHeli-S FPV ESC 20A Brushless Speed Controller](https://naylampmechatronics.com/drivers/971-blheli-s-fpv-esc-20a-brushless-speed-controller.html) - ESPECIFICACIONES TÉCNICAS · Voltaje: 7.2V a 22.2V (Baterías de 2S a 6S) · Corriente continua máxima:...

