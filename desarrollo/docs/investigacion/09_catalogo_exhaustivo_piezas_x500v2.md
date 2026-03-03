# Catálogo Exhaustivo de Piezas – Holybro X500 V2
## Gemelo Digital para Aplicación WebGL

**Fecha:** 3 de Marzo, 2026  
**Propósito:** Documento maestro de referencia con TODAS las piezas del Holybro X500 V2, mapeadas 1:1 a los campos de `DronePartData.cs` para su uso en la aplicación de visor interactivo.

---

## Campos de `DronePartData` que se deben poblar por pieza

| Campo C# | Tipo | Mostrado en UI | Descripción |
|:---|:---|:---:|:---|
| `partName` | string | ✅ SheetTitle, SelectionIndicator | Nombre legible de la pieza |
| `id` | string | — | Identificador único interno |
| `partType` | string | — | Frame, Motor, Propeller, ESC, etc. |
| `category` | string | ✅ PartCategory | Structure, Propulsion, Electronics, Other |
| `description` | string | ✅ PartDescription | Descripción técnica detallada |
| `function` | string | ✅ PartFunction | Función principal de la pieza |
| `weightKg` | float | ✅ PartWeight | Peso en kilogramos |
| `dimensions` | string | ✅ PartDimensions | Dimensiones físicas |
| `materialType` | string | ✅ PartMaterial | Material principal |
| `materialProperties` | string | — | Propiedades del material |
| `manufacturer` | string | — | Fabricante |
| `partNumber` | string | — | SKU o número de parte |
| `powerConsumption` | float | ✅ PartPower | Consumo en Watts |
| `maxLoad` | float | — | Carga máxima (kg o N) |
| `operatingTemp` | float | ✅ PartTemp | Temperatura de operación típica °C |
| `difficultyLevel` | int (1–5) | ✅ PartDifficulty | Dificultad de instalación |
| `requiredTools` | string[] | ✅ PartTools | Herramientas necesarias |
| `installationTimeMinutes` | float | ✅ PartAssemblyTime | Tiempo estimado de instalación |
| `torqueSpec` | string | — | Especificación de torque |
| `assemblyOrder` | int | — | Orden en secuencia de montaje |
| `prerequisites` | string[] | — | Piezas que deben instalarse antes |
| `connectionTypes` | string[] | — | Tornillo, snap, cable, etc. |
| `screwCount` | int | — | Cantidad de tornillos |
| `screwSize` | string | — | Tamaño de tornillo (M2, M3…) |
| `safetyWarnings` | string[] | — | Advertencias de seguridad |
| `installationTips` | string | — | Consejos de instalación |
| `explosionDirection` | Vector3 | — | Dirección de explosión para vista |
| `explosionDistance` | float | — | Distancia de explosión |
| `explosionPriority` | int | — | Orden de animación de explosión |
| `highlightColor` | Color | — | Color de resaltado al seleccionar |

---

## Especificaciones Globales del Sistema

| Parámetro | Valor |
|:---|:---|
| **Wheelbase** | 500 mm |
| **Cuerpo del frame** | 144 × 144 mm, 2 mm de espesor |
| **Altura tren de aterrizaje** | 215 mm |
| **Separación entre platos** | 28 mm |
| **Peso total (sin batería)** | 610 g |
| **Peso del frame kit** | ~365 g |
| **Tiempo de vuelo** | ~18 min hover (batería 4S 5000 mAh) |
| **Carga útil máxima** | ~1 kg (al 70% throttle) |
| **Batería recomendada** | LiPo 4S 2000–5000 mAh |
| **Firmware** | PX4 / ArduPilot |
| **Tiempo de ensamblaje** | ~30 min (sin soldadura) |

---

## PIEZAS ESTRUCTURALES

---

### 01. Top & Bottom Plates (Platos de Fibra de Carbono)

| Campo | Valor |
|:---|:---|
| **partName** | Carbon Fiber Top & Bottom Plates |
| **id** | `x500v2_plates` |
| **partType** | Frame |
| **category** | Structure |
| **function** | Núcleo estructural del fuselaje; interfaz de montaje para PDB, flight controller, tren y railes |
| **description** | Conjunto de platos superior e inferior de fibra de carbono twill de 2 mm de espesor y 144 × 144 mm. Forman un "sándwich" estructural con 28 mm de separación que encierra la bahía electrónica. Incluyen patrón de taladros para PDB, Pixhawk, railes de carga y monturas. Proporcionan rigidez torsional y protección básica de la electrónica central. |
| **weightKg** | 0.150 |
| **dimensions** | 144 × 144 mm, 2 mm espesor, separación 28 mm |
| **materialType** | Fibra de carbono twill 2 mm |
| **materialProperties** | Ultraligero, alta rigidez torsional, resistente a vibraciones |
| **manufacturer** | Holybro |
| **partNumber** | SKU30120 (parte del frame kit) / #510179 (top), #510180 (bottom) |
| **powerConsumption** | 0 |
| **maxLoad** | Soporta peso total del sistema (~1.6 kg con batería y payload) |
| **operatingTemp** | 80 (rango: −20 a 80 °C) |
| **difficultyLevel** | 2 |
| **requiredTools** | ["Llave hexagonal M3", "Destornillador Phillips"] |
| **installationTimeMinutes** | 5 |
| **torqueSpec** | 0.5 Nm (tornillos M3) |
| **assemblyOrder** | 1 (bottom) / 4 (top) |
| **prerequisites** | [] (bottom) / ["PDB", "Arms", "Landing Gear"] (top) |
| **connectionTypes** | ["Screw"] |
| **screwCount** | 8 (4 por plato) |
| **screwSize** | M3 |
| **safetyWarnings** | ["Bordes de carbono pueden ser cortantes", "No apretar en exceso los tornillos"] |
| **installationTips** | Alinear las guías rectangulares de los brazos antes de apretar. Usar tuercas nyloc para evitar aflojamiento por vibración. |
| **explosionDirection** | (0, 1, 0) top / (0, −1, 0) bottom |
| **explosionDistance** | 0.5 |
| **explosionPriority** | 1 (bottom) / 4 (top) |
| **highlightColor** | Gris carbono (0.3, 0.3, 0.3) |
| **Perfil Térmico** | FRÍO – Componente pasivo. Conducción leve por contacto con PDB. Shader: Azul oscuro en modo térmico. |

---

### 02. Tubos de Brazo × 4 (Arm Tubes)

| Campo | Valor |
|:---|:---|
| **partName** | Carbon Fiber Arm Tube |
| **id** | `x500v2_arm_FL` / `_FR` / `_BL` / `_BR` |
| **partType** | Arm |
| **category** | Structure |
| **function** | Soporte y conducto para motor y ESC; transfiere cargas de empuje al chasis |
| **description** | Tubo de fibra de carbono de 16 mm de diámetro con conectores de nylon reforzado en ambos extremos (motor y fuselaje). Cada brazo lleva preinstalado un motor 2216 y un ESC BLHeli-S 20A con cableado interno, permitiendo conexión plug-and-play mediante XT30 a la PDB. Configuración en X a 45° del eje longitudinal. |
| **weightKg** | 0.030 (por brazo, ~120 g total ×4 incluyendo conectores) |
| **dimensions** | ⌀16 mm × ~220 mm largo |
| **materialType** | Tubo de fibra de carbono con conectores de nylon reforzado con fibra |
| **materialProperties** | Alta relación rigidez/peso, absorción parcial de vibraciones |
| **manufacturer** | Holybro |
| **partNumber** | #510177 |
| **powerConsumption** | 0 |
| **maxLoad** | Empuje de motor individual (~1.2 kg por brazo) |
| **operatingTemp** | 80 (rango: −20 a 80 °C) |
| **difficultyLevel** | 2 |
| **requiredTools** | ["Llave hexagonal M3"] |
| **installationTimeMinutes** | 3 (por brazo) |
| **torqueSpec** | 0.5 Nm |
| **assemblyOrder** | 2 |
| **prerequisites** | ["Bottom Plate"] |
| **connectionTypes** | ["Screw", "Snap (nylon connectors)"] |
| **screwCount** | 2 (por brazo) |
| **screwSize** | M3 |
| **safetyWarnings** | ["Verificar orientación CW/CCW del motor preinstalado"] |
| **installationTips** | Las muescas de los conectores de nylon deben alinearse con los alojamientos rectangulares del bottom plate. Los cables XT30 del ESC se introducen por el interior del tubo. |
| **explosionDirection** | (−1, 1, 1) FL / (1, 1, 1) FR / (−1, 1, −1) BL / (1, 1, −1) BR |
| **explosionDistance** | 1.5 |
| **explosionPriority** | 2 |
| **highlightColor** | Gris oscuro (0.25, 0.25, 0.25) |
| **Perfil Térmico** | FRÍO a TIBIO – Conducción térmica desde base del motor y ESC interno. Gradiente: azul en centro, verde-amarillo en extremo motor. |

---

### 03. Tren de Aterrizaje (Landing Gear)

| Campo | Valor |
|:---|:---|
| **partName** | Carbon Fiber Landing Gear |
| **id** | `x500v2_landing_gear` |
| **partType** | Landing |
| **category** | Structure |
| **function** | Soporte al suelo; absorción de impactos de aterrizaje; separación de carga útil inferior |
| **description** | Tren formado por dos patines cruzados con tubos de carbono verticales (16 mm) y barras transversales (10 mm), unidos por T-connectors plásticos reforzados engrosados. Espuma en la base para amortiguación. Proporciona 215 mm de altura libre para gimbals, cámaras o la batería en railes inferiores. |
| **weightKg** | 0.080 |
| **dimensions** | Altura: 215 mm, tubos ⌀16 mm y ⌀10 mm |
| **materialType** | Tubos de fibra de carbono con conectores plásticos reforzados |
| **materialProperties** | Resistente a impactos, ligero, amortiguación por espuma |
| **manufacturer** | Holybro |
| **partNumber** | #510192 |
| **powerConsumption** | 0 |
| **operatingTemp** | 80 (rango: −20 a 80 °C) |
| **difficultyLevel** | 2 |
| **requiredTools** | ["Llave hexagonal M3"] |
| **installationTimeMinutes** | 5 |
| **assemblyOrder** | 3 |
| **prerequisites** | ["Bottom Plate"] |
| **connectionTypes** | ["Screw", "Snap (T-connectors)"] |
| **screwCount** | 4 |
| **screwSize** | M3 |
| **safetyWarnings** | ["Verificar simetría de ambos patines antes de apretar"] |
| **installationTips** | Los T-connectors encajan a presión primero; luego asegurar con tornillos M3. Verificar que la base quede nivelada. |
| **explosionDirection** | (0, −1, 0) |
| **explosionDistance** | 1.2 |
| **explosionPriority** | 3 |
| **highlightColor** | Gris medio (0.35, 0.35, 0.35) |
| **Perfil Térmico** | FRÍO – Sin generación térmica. Azul en mapa de calor. |

---

### 04. Platform Board (Plataforma Superior)

| Campo | Valor |
|:---|:---|
| **partName** | Platform Board |
| **id** | `x500v2_platform_board` |
| **partType** | Frame |
| **category** | Structure |
| **function** | Plataforma de montaje para GPS y companion computer (Raspberry Pi / Jetson Nano) |
| **description** | Placa separada del top plate mediante standoffs M2.5. Incluye taladros preperforados para el módulo GPS M10/M8N y para companion computers populares. Posiciona el GPS alejado del plano de hélices y de los campos electromagnéticos de la PDB. |
| **weightKg** | 0.015 |
| **dimensions** | ~80 × 80 mm (estimado) |
| **materialType** | Fibra de carbono / compuesto plástico |
| **manufacturer** | Holybro |
| **partNumber** | Incluido en SKU30120 |
| **powerConsumption** | 0 |
| **operatingTemp** | 80 (rango: −20 a 80 °C) |
| **difficultyLevel** | 1 |
| **requiredTools** | ["Destornillador M2.5"] |
| **installationTimeMinutes** | 3 |
| **assemblyOrder** | 7 |
| **prerequisites** | ["Top Plate", "Flight Controller"] |
| **connectionTypes** | ["Screw (standoffs M2.5)"] |
| **screwCount** | 4 |
| **screwSize** | M2.5 |
| **installationTips** | Los standoffs elevan la plataforma ~15–20 mm sobre el top plate para aislar magnéticamente el GPS de la PDB. |
| **explosionDirection** | (0, 1, 0) |
| **explosionDistance** | 0.8 |
| **explosionPriority** | 7 |
| **highlightColor** | Gris claro (0.45, 0.45, 0.45) |
| **Perfil Térmico** | FRÍO – Azul. Sin generación térmica propia. |

---

### 05. Sistema de Railes y Soporte de Batería

| Campo | Valor |
|:---|:---|
| **partName** | Rail System & Battery Mount |
| **id** | `x500v2_rails_battery` |
| **partType** | Frame |
| **category** | Structure |
| **function** | Montaje de batería y cargas útiles inferiores (gimbals, cámaras) |
| **description** | Dos barras de fibra de carbono de ⌀10 mm × 250 mm montadas bajo el bottom plate. Incluye battery mounting board ajustable y agrandado con rubber hangers y dos correas (straps) de velcro para fijar baterías LiPo 4S de hasta 5000 mAh. |
| **weightKg** | 0.025 |
| **dimensions** | 2× barras ⌀10 mm × 250 mm |
| **materialType** | Fibra de carbono (barras) + plástico reforzado (soporte) |
| **manufacturer** | Holybro |
| **partNumber** | Incluido en SKU30120 |
| **powerConsumption** | 0 |
| **operatingTemp** | 80 |
| **difficultyLevel** | 1 |
| **requiredTools** | ["Llave hexagonal M3", "Correas de batería"] |
| **installationTimeMinutes** | 3 |
| **assemblyOrder** | 8 |
| **prerequisites** | ["Bottom Plate", "Landing Gear"] |
| **connectionTypes** | ["Screw", "Velcro Strap"] |
| **screwCount** | 4 |
| **screwSize** | M3 |
| **safetyWarnings** | ["Asegurar firmemente la batería antes de volar"] |
| **installationTips** | Centrar la batería para mantener el CG. El board ajustable permite acomodar baterías de distintos tamaños. |
| **explosionDirection** | (0, −1, 0) |
| **explosionDistance** | 1.0 |
| **explosionPriority** | 8 |
| **highlightColor** | Gris cálido (0.4, 0.38, 0.35) |
| **Perfil Térmico** | FRÍO – Azul. La batería (si se modela aparte) tendría perfil tibio (verde-amarillo). |

---

## PIEZAS ELÉCTRICAS Y ELECTRÓNICAS

---

### 06. Power Distribution Board (PDB)

| Campo | Valor |
|:---|:---|
| **partName** | Power Distribution Board |
| **id** | `x500v2_pdb` |
| **partType** | PDB |
| **category** | Electronics |
| **function** | Bus central de distribución de potencia desde batería hacia ESCs y periféricos |
| **description** | Placa de distribución con entrada XT60 para batería principal y múltiples salidas XT30 para ESCs y periféricos de alta corriente. Capacidad de hasta 100A en ráfaga. Se monta directamente sobre el bottom plate, centrada. Elimina soldadura gracias al sistema plug-and-play de conectores XT30. Actúa como hub de potencia puro sin conversión de tensión. |
| **weightKg** | 0.020 |
| **dimensions** | ~50 × 50 mm (estimado, formato PCB cuadrado) |
| **materialType** | PCB FR4 con pistas de cobre de alta corriente y conectores XT60/XT30 |
| **materialProperties** | Alta conductividad, resistente al calor |
| **manufacturer** | Holybro |
| **partNumber** | Incluido en SKU30120 |
| **powerConsumption** | ~5 W (pérdidas por resistencia en pistas de cobre) |
| **maxLoad** | 100 A (ráfaga) |
| **operatingTemp** | 80 (rango: −20 a 80 °C) |
| **difficultyLevel** | 2 |
| **requiredTools** | ["Destornillador Phillips", "Llave hexagonal M3"] |
| **installationTimeMinutes** | 3 |
| **assemblyOrder** | 2 |
| **prerequisites** | ["Bottom Plate"] |
| **connectionTypes** | ["Screw", "XT60 Plug", "XT30 Plug"] |
| **screwCount** | 4 |
| **screwSize** | M3 |
| **safetyWarnings** | ["No conectar batería hasta completar todo el cableado", "Verificar polaridad XT60"] |
| **installationTips** | Orientar la XT60 hacia la parte posterior del frame. Conectar los cables XT30 de los ESC antes de cerrar con el top plate. |
| **explosionDirection** | (0, −0.5, 0) |
| **explosionDistance** | 0.4 |
| **explosionPriority** | 2 |
| **highlightColor** | Naranja (1.0, 0.5, 0.0) |
| **Perfil Térmico** | TIBIO a MEDIO – Naranja/amarillo. Fluyen hasta 80A; las pistas de cobre disipan calor moderado de forma uniforme. Zona más caliente: nodos de conexión XT. |

---

### 07. Power Module PM02 V3

| Campo | Valor |
|:---|:---|
| **partName** | Power Module PM02 V3 |
| **id** | `x500v2_power_module` |
| **partType** | PowerModule |
| **category** | Electronics |
| **function** | Regulador 5V para el Pixhawk y sensor de voltaje/corriente del sistema |
| **description** | Módulo de alimentación insertado en serie entre batería (XT60) y PDB. Proporciona alimentación regulada de 5.2V al bus de potencia del Pixhawk y sensa la corriente y voltaje del sistema para telemetría. Compatible con LiPo hasta 12S. Conectado al Pixhawk por cable dedicado (GH 1.25mm 6-pin). |
| **weightKg** | 0.028 |
| **dimensions** | ~55 × 17 × 8 mm |
| **materialType** | PCB FR4 con componentes SMD y conectores XT60 |
| **manufacturer** | Holybro |
| **partNumber** | PM02 V3 |
| **powerConsumption** | 1.5 (consumo propio del regulador) |
| **operatingTemp** | 70 (rango: −20 a 70 °C) |
| **difficultyLevel** | 2 |
| **requiredTools** | ["Bridas de nylon", "Pad adhesivo 3M"] |
| **installationTimeMinutes** | 2 |
| **assemblyOrder** | 2 |
| **prerequisites** | ["PDB"] |
| **connectionTypes** | ["XT60 Plug (in-line)", "GH 1.25mm cable to Pixhawk"] |
| **screwCount** | 0 |
| **safetyWarnings** | ["No exceder 12S", "Verificar conexión antes de energizar"] |
| **installationTips** | Fijar con pad adhesivo cerca de la PDB dentro de la bahía central. El cable de datos va al puerto POWER1 del Pixhawk. |
| **explosionDirection** | (0, −0.3, 0.2) |
| **explosionDistance** | 0.3 |
| **explosionPriority** | 2 |
| **highlightColor** | Naranja claro (1.0, 0.65, 0.2) |
| **Perfil Térmico** | TIBIO – Verde-amarillo. El regulador DC-DC genera calor leve durante operación continua. |

---

### 08. Pixhawk 6C Flight Controller

| Campo | Valor |
|:---|:---|
| **partName** | Pixhawk 6C Autopilot |
| **id** | `x500v2_pixhawk6c` |
| **partType** | FlightController |
| **category** | Electronics |
| **function** | Procesamiento de vuelo, fusión de sensores (IMU, barómetro), control de motores y navegación autónoma |
| **description** | Controlador de vuelo con procesador dual-core STM32H743 (Cortex-M7), IMUs redundantes (ICM-42688-P + BMI055), barómetro MS5611, e interfaces UART/CAN/I2C/SPI/PWM. Ejecuta firmware PX4 o ArduPilot. Genera comandos PWM/DShot para los 4 ESCs, gestiona modos de vuelo (Stabilized, Position, Mission), y procesa datos del GPS, telemetría y receptor RC. Carcasa de aluminio. |
| **weightKg** | 0.035 |
| **dimensions** | 84.8 × 44 × 12.4 mm (con carcasa) |
| **materialType** | PCB FR4, carcasa de aluminio anodizado |
| **materialProperties** | Procesador de alto rendimiento, sensores redundantes, carcasa disipadora |
| **manufacturer** | Holybro |
| **partNumber** | Pixhawk 6C |
| **powerConsumption** | 2.5 |
| **maxLoad** | N/A |
| **operatingTemp** | 70 (rango: −20 a 70 °C) |
| **difficultyLevel** | 3 |
| **requiredTools** | ["Destornillador M2", "Pad de espuma anti-vibración", "Pulsera antiestática"] |
| **installationTimeMinutes** | 10 |
| **torqueSpec** | 0.3 Nm |
| **assemblyOrder** | 5 |
| **prerequisites** | ["Top Plate", "PDB", "Power Module"] |
| **connectionTypes** | ["Adhesive pad / Standoffs M2", "GH 1.25mm cables (power, GPS, telem, RC)"] |
| **screwCount** | 4 (si usa standoffs) |
| **screwSize** | M2 |
| **safetyWarnings** | ["Manipular con protección antiestática", "Alinear flecha con frente del frame", "No energizar sin conexiones verificadas"] |
| **installationTips** | Montar centrado sobre el top plate con la flecha de referencia apuntando al frente del dron. Usar pad de espuma para aislar vibraciones. Los cables de alimentación, ESC y GPS se enrutan hacia abajo por pasos de cable en el top plate. |
| **explosionDirection** | (0, 1, 0) |
| **explosionDistance** | 1.0 |
| **explosionPriority** | 5 |
| **highlightColor** | Verde (0.0, 0.8, 0.2) |
| **Perfil Térmico** | TIBIO – Naranja leve. CPU dual-core genera calor estable bajo procesamiento continuo de sensores. La carcasa de aluminio actúa como disipador. |

---

### 09. Módulo GPS M10 con Brújula

| Campo | Valor |
|:---|:---|
| **partName** | Holybro M10 GPS Module |
| **id** | `x500v2_gps_m10` |
| **partType** | GPS |
| **category** | Electronics |
| **function** | Posicionamiento 3D, velocidad, rumbo; brújula (magnetómetro) para referencia de orientación absoluta |
| **description** | Módulo GNSS multi-constelación basado en u-blox M10 (GPS, Galileo, GLONASS, BeiDou concurrentes). Antena cerámica de alto ganancia 25×25×4 mm. Incluye magnetómetro IST8310, LED RGB tricolor, buzzer y safety switch integrados. Baud rate: 115200, 5 Hz. Conectado al Pixhawk por UART/I2C (JST GHR 1.25mm 10-pin). Montado sobre mástil de fibra de carbono incluido. |
| **weightKg** | 0.032 |
| **dimensions** | ⌀50 × 14.4 mm (módulo), mástil de carbono (~100 mm) |
| **materialType** | PCB con antena cerámica GNSS, carcasa plástica, mástil de fibra de carbono |
| **materialProperties** | Recepción multiband, batería de respaldo para warm starts |
| **manufacturer** | Holybro |
| **partNumber** | SKU12040 |
| **powerConsumption** | 0.5 (< 200 mA a 5V) |
| **operatingTemp** | 70 (rango: −40 a 80 °C) |
| **difficultyLevel** | 2 |
| **requiredTools** | ["Destornillador M2.5"] |
| **installationTimeMinutes** | 5 |
| **assemblyOrder** | 7 |
| **prerequisites** | ["Platform Board", "Pixhawk 6C"] |
| **connectionTypes** | ["Screw M2.5", "JST GHR 1.25mm 10-pin cable"] |
| **screwCount** | 2 |
| **screwSize** | M2.5 |
| **safetyWarnings** | ["Alejar de fuentes de interferencia magnética (PDB, cables de alta corriente)"] |
| **installationTips** | Montar centrado o ligeramente retrasado en la platform board, sobre el mástil de carbono. El mástil aleja el GPS del plano de hélices y de campos electromagnéticos. Orientación del conector: verificar parámetro COMPASS_ORIENT en ArduPilot si la calibración falla. |
| **explosionDirection** | (0, 1, 0) |
| **explosionDistance** | 1.5 |
| **explosionPriority** | 7 |
| **highlightColor** | Azul claro (0.2, 0.6, 1.0) |
| **Perfil Térmico** | MUY FRÍO – Azul oscuro. Consumo mínimo (~0.5 W). Sin generación térmica significativa. |

---

### 10. Radio de Telemetría SiK V3

| Campo | Valor |
|:---|:---|
| **partName** | SiK Telemetry Radio V3 (Air Unit) |
| **id** | `x500v2_telemetry_radio` |
| **partType** | Radio |
| **category** | Electronics |
| **function** | Enlace de datos bidireccional dron-estación de tierra para telemetría, misiones y parámetros |
| **description** | Módulo de radio SiK V3 en banda 433 o 915 MHz. Potencia de salida ~100 mW. Protocolo MAVLink transparente. Conectado al Pixhawk por UART (puerto TELEM1). Incluye unidad aérea y unidad de tierra pareadas de fábrica. Antena omnidireccional de goma. Alcance típico ~500 m en condiciones normales. |
| **weightKg** | 0.015 |
| **dimensions** | ~50 × 25 × 10 mm (sin antena) |
| **materialType** | PCB FR4 con módulo RF y carcasa plástica ligera |
| **manufacturer** | Holybro |
| **partNumber** | SiK Telemetry Radio V3 |
| **powerConsumption** | 1.0 (durante transmisión continua) |
| **operatingTemp** | 60 (rango: −20 a 60 °C) |
| **difficultyLevel** | 1 |
| **requiredTools** | ["Brida de nylon", "Cinta adhesiva 3M"] |
| **installationTimeMinutes** | 2 |
| **assemblyOrder** | 6 |
| **prerequisites** | ["Pixhawk 6C"] |
| **connectionTypes** | ["UART cable (GH 1.25mm 6-pin)", "SMA antenna connector"] |
| **screwCount** | 0 |
| **safetyWarnings** | ["No transmitir sin antena conectada (daño al módulo RF)"] |
| **installationTips** | Fijar en lateral del frame o bajo la platform board con la antena orientada hacia arriba o lateralmente para máxima cobertura. Conectar al puerto TELEM1 del Pixhawk. |
| **explosionDirection** | (0.5, 1, 0.3) |
| **explosionDistance** | 0.8 |
| **explosionPriority** | 6 |
| **highlightColor** | Cyan (0.0, 0.9, 0.9) |
| **Perfil Térmico** | TIBIO BAJO – Verde-azul. Calentamiento leve durante transmisión RF continua. |

---

## PIEZAS DE PROPULSIÓN

---

### 11. Motor Brushless 2216 KV920 × 4

| Campo | Valor |
|:---|:---|
| **partName** | Holybro 2216 KV920 Brushless Motor |
| **id** | `x500v2_motor_FL` / `_FR` / `_BL` / `_BR` |
| **partType** | Motor |
| **category** | Propulsion |
| **function** | Generación de empuje principal; convierte energía eléctrica en rotación mecánica |
| **description** | Motor brushless outrunner tipo 2216 (22 mm diámetro estator, 16 mm altura) con KV 920 rpm/V. Optimizado para hélices 1045 y LiPo 3–4S. Corrientes típicas: 3.5–16.2 A en 4S (14.8V). Potencia pico por motor: ~240 W. Empuje máximo: varios cientos de gramos por motor. Carcasa de aleación de aluminio mecanizado CNC con eje de acero y bobinados de cobre esmaltado. Patrón de montaje: 16×16 mm y 19×19 mm. Preinstalado en el brazo con conector XT30. 2 CW + 2 CCW. |
| **weightKg** | 0.063 |
| **dimensions** | ⌀28 mm × ~32 mm altura |
| **materialType** | Aleación de aluminio mecanizado CNC, acero (eje), cobre esmaltado (bobinados) |
| **materialProperties** | Disipación térmica por carcasa metálica, imanes N52 |
| **manufacturer** | Holybro |
| **partNumber** | Motor 2216-920KV-CW / CCW |
| **powerConsumption** | 240 (pico a 16.2A × 14.8V) |
| **maxLoad** | ~1.2 kg empuje por motor (estimado con hélice 1045 a 4S) |
| **operatingTemp** | 80 (rango: −20 a 80 °C; pico térmico hasta ~95 °C en carga sostenida) |
| **difficultyLevel** | 2 |
| **requiredTools** | ["Llave de vaso M3", "Loctite (frenador de rosca)"] |
| **installationTimeMinutes** | 5 (por motor, preinstalado) |
| **torqueSpec** | 0.5 Nm |
| **assemblyOrder** | 2 (preinstalado en brazo) |
| **prerequisites** | ["Arm Tube"] |
| **connectionTypes** | ["Screw M3 (4× patrón 16×16 mm)", "XT30 cable to ESC"] |
| **screwCount** | 4 (por motor) |
| **screwSize** | M3 |
| **safetyWarnings** | ["NUNCA tocar con hélice montada y batería conectada", "Verificar rotación CW/CCW antes de vuelo", "Motor se calienta significativamente en vuelo prolongado"] |
| **installationTips** | Preinstalado en brazo de fábrica. Si se reemplaza, aplicar Loctite en los 4 tornillos M3. Verificar sentido de giro: FL y BR = CW, FR y BL = CCW (configuración PX4 estándar quad-X). |
| **explosionDirection** | (0, 1, 0) |
| **explosionDistance** | 2.0 |
| **explosionPriority** | 9 |
| **highlightColor** | Azul oscuro (0.2, 0.2, 0.8) |
| **Perfil Térmico** | **PUNTO CALIENTE PRINCIPAL – ROJO INTENSO**. Zona de mayor generación térmica del dron. 240W pico. Los bobinados de cobre generan calor por efecto Joule (I²R). La carcasa de aluminio actúa como disipador principal. En hover estable: ~50–65 °C. En carga máxima sostenida: hasta ~95 °C. |

---

### 12. ESC BLHeli-S 20A × 4

| Campo | Valor |
|:---|:---|
| **partName** | Holybro BLHeli-S 20A ESC |
| **id** | `x500v2_esc_FL` / `_FR` / `_BL` / `_BR` |
| **partType** | ESC |
| **category** | Propulsion |
| **function** | Conmutación de potencia para motor brushless; control de velocidad de rotación |
| **description** | Regulador electrónico de velocidad BLHeli-S con MCU EFM8BB21. 20A continuos, 30A ráfaga (~10 s). LiPo 2–4S (7.2–16.8V). Sin BEC integrado. Soporta protocolos PWM, Oneshot125, Oneshot42 y DShot (150/300/600). Velocidad máxima: 500k eRPM. Preinstalado dentro/junto al brazo con conectores XT30. Actúa como etapa de potencia puente entre PDB y motor. |
| **weightKg** | 0.007 |
| **dimensions** | ~25 × 13 × 5 mm |
| **materialType** | PCB FR4 con MOSFETs de silicio y pistas de cobre |
| **materialProperties** | Conmutación de alta frecuencia, tamaño compacto |
| **manufacturer** | Holybro |
| **partNumber** | BLHeli-S ESC 20A |
| **powerConsumption** | 300 (transferencia máxima: 20A × 14.8V = 296W; consumo propio: ~1W) |
| **maxLoad** | 20 A (continuo), 30 A (ráfaga ~10 s) |
| **operatingTemp** | 80 (rango: −20 a 80 °C; pico hasta ~90 °C) |
| **difficultyLevel** | 2 |
| **requiredTools** | ["Ninguna (plug-and-play vía XT30)"] |
| **installationTimeMinutes** | 1 (plug-and-play) |
| **assemblyOrder** | 2 (preinstalado en brazo) |
| **prerequisites** | ["Arm Tube", "Motor"] |
| **connectionTypes** | ["XT30 Plug (to PDB)", "3-wire solder (to motor, prefab)", "PWM/DShot wire (to Pixhawk PWM out)"] |
| **screwCount** | 0 |
| **safetyWarnings** | ["No exceder 30A por más de 10 segundos", "Verificar que no haya cortocircuito antes de conectar batería", "Posible thermal throttling si ventilación es insuficiente"] |
| **installationTips** | Preinstalado en brazo. El cable XT30 se introduce por el interior del tubo de carbono y se conecta a la PDB. El cable de señal PWM/DShot va al puerto correspondiente del Pixhawk (MAIN 1–4). |
| **explosionDirection** | Misma que su brazo correspondiente |
| **explosionDistance** | 1.8 |
| **explosionPriority** | 9 |
| **highlightColor** | Rojo oscuro (0.6, 0.1, 0.1) |
| **Perfil Térmico** | **FOCO ROJO INTERNO**. Segunda fuente térmica más intensa. Los MOSFETs conmutan altas corrientes a alta frecuencia. Ubicación interior del brazo dificulta ventilación. En vuelo sostenido: ~45–65 °C. En ráfaga máxima: hasta ~90 °C. Riesgo de thermal throttling si la ventilación es deficiente. |

---

### 13. Hélices 1045 × 4 (+ 2 repuesto)

| Campo | Valor |
|:---|:---|
| **partName** | Holybro 1045 Propeller |
| **id** | `x500v2_prop_FL` / `_FR` / `_BL` / `_BR` |
| **partType** | Propeller |
| **category** | Propulsion |
| **function** | Conversión de par motor en empuje vertical aerodinámico |
| **description** | Hélice fija tipo 1045 (10 pulgadas diámetro, 4.5° pitch). Suministrada en juegos de 6 piezas (2 CW + 2 CCW + 2 repuesto). Material: compuesto plástico reforzado (nylon/ABS). Optimizada para motores 2216 en configuración 4S. Define junto con el KV del motor el punto de operación, empuje máximo y eficiencia propulsiva. |
| **weightKg** | 0.008 |
| **dimensions** | ⌀254 mm (10 in) × pitch 114 mm (4.5 in) |
| **materialType** | Compuesto plástico reforzado (nylon o ABS reforzado) |
| **materialProperties** | Resistente a fatiga, ligero, balanceo de fábrica |
| **manufacturer** | Holybro |
| **partNumber** | 1045 Propeller CW / CCW |
| **powerConsumption** | 0 (consumo absorbido por motor) |
| **operatingTemp** | 70 (rango: −20 a 70 °C) |
| **difficultyLevel** | 1 |
| **requiredTools** | ["Llave de hélice (incluida)"] |
| **installationTimeMinutes** | 1 (por hélice) |
| **assemblyOrder** | 10 (último paso) |
| **prerequisites** | ["Motor"] |
| **connectionTypes** | ["Nut (self-locking prop nut)"] |
| **screwCount** | 1 (tuerca autoblocante por hélice) |
| **safetyWarnings** | ["SIEMPRE retirar hélices antes de calibración o trabajo en el dron", "Verificar CW/CCW correcto en cada motor", "Inspeccionar grietas antes de cada vuelo"] |
| **installationTips** | La hélice CW va en motores CW (FL/BR), la CCW en motores CCW (FR/BL). Apretar la tuerca autoblocante firmemente a mano. El kit trae 2 hélices de repuesto. |
| **explosionDirection** | (0, 1, 0) |
| **explosionDistance** | 2.5 |
| **explosionPriority** | 10 |
| **highlightColor** | Azul claro (0.3, 0.5, 0.9) |
| **Perfil Térmico** | FRÍO – Azul. Sin generación térmica propia. Se enfría por rotación (convección forzada). |

---

## ACCESORIOS Y HERRAJES

---

### 14. Batería LiPo 4S (Recomendada, no incluida)

| Campo | Valor |
|:---|:---|
| **partName** | LiPo Battery 4S 5000mAh |
| **id** | `x500v2_battery` |
| **partType** | Battery |
| **category** | Electronics |
| **function** | Fuente de energía primaria del sistema completo (14.8V nominal) |
| **description** | Paquete de baterías LiPo de 4 celdas en serie (4S, 14.8V nominal, 16.8V carga completa). Capacidad recomendada: 2000–5000 mAh. La de 5000 mAh proporciona ~18 min de hover sin payload adicional. Conector XT60 para PDB. Se fija al battery mount con correas de velcro. No incluida en el kit. |
| **weightKg** | 0.520 (estimado para 5000 mAh) |
| **dimensions** | ~140 × 50 × 45 mm (típico 4S 5000 mAh) |
| **materialType** | Polímero de litio (LiPo) con envoltura de termocontraíble |
| **manufacturer** | Varía (recomendación del fabricante) |
| **powerConsumption** | 0 (es fuente, no consumidor) |
| **operatingTemp** | 45 (rango: 0 a 45 °C para descarga segura) |
| **difficultyLevel** | 1 |
| **requiredTools** | ["Correas de velcro"] |
| **installationTimeMinutes** | 1 |
| **assemblyOrder** | 9 |
| **prerequisites** | ["Rail System", "PDB"] |
| **connectionTypes** | ["XT60 Plug", "Velcro Strap"] |
| **safetyWarnings** | ["NUNCA perforar o deformar", "Almacenar en bolsa ignífuga (LiPo safe bag)", "No descargar por debajo de 3.3V/celda", "Cargar solo con cargador balanceado"] |
| **installationTips** | Centrar sobre el battery mount para mantener el centro de gravedad. Conectar el XT60 al power module, NO directamente a la PDB. |
| **explosionDirection** | (0, −1, −0.3) |
| **explosionDistance** | 1.2 |
| **explosionPriority** | 9 |
| **highlightColor** | Naranja (1.0, 0.5, 0.0) |
| **Perfil Térmico** | TIBIO a MEDIO – Verde-amarillo. La descarga genera calor interno por resistencia interna. En vuelo agresivo (altas corrientes): puede alcanzar 45–55 °C. |

---

### 15. Receptor RC (Opcional, no incluido)

| Campo | Valor |
|:---|:---|
| **partName** | RadioMaster R81 RC Receiver |
| **id** | `x500v2_rc_receiver` |
| **partType** | Receiver |
| **category** | Electronics |
| **function** | Recepción de señales de control manual del piloto desde transmisor RC |
| **description** | Receptor RC compatible con protocolo D8/D16 (FrSky) o ELRS según variante. Conectado al Pixhawk por SBUS/PPM (puerto RC IN). Permite al piloto controlar manualmente el dron. Se fija junto al Pixhawk sobre el top plate. Opcional para el kit; recomendado para desarrollo y pruebas. |
| **weightKg** | 0.005 |
| **dimensions** | ~25 × 15 × 8 mm |
| **materialType** | PCB miniatura con antena integrada y carcasa termocontraíble |
| **manufacturer** | RadioMaster |
| **partNumber** | R81 |
| **powerConsumption** | 0.2 |
| **operatingTemp** | 60 (rango: −10 a 60 °C) |
| **difficultyLevel** | 1 |
| **requiredTools** | ["Cinta adhesiva 3M", "Brida de nylon"] |
| **installationTimeMinutes** | 2 |
| **assemblyOrder** | 6 |
| **prerequisites** | ["Pixhawk 6C"] |
| **connectionTypes** | ["SBUS/PPM cable to Pixhawk RC IN port"] |
| **screwCount** | 0 |
| **safetyWarnings** | ["Verificar binding con transmisor antes del vuelo", "Mantener antenas despejadas"] |
| **installationTips** | Fijar junto al Pixhawk con las antenas en V a 90°. Conectar al puerto RC IN del Pixhawk. |
| **explosionDirection** | (0.3, 1, 0) |
| **explosionDistance** | 0.7 |
| **explosionPriority** | 6 |
| **highlightColor** | Magenta (0.8, 0.2, 0.8) |
| **Perfil Térmico** | MUY FRÍO – Azul. Consumo de apenas ~0.2W. Sin generación térmica notable. |

---

### 16. Herrajes y Tornillería (Hardware Kit)

| Campo | Valor |
|:---|:---|
| **partName** | Assembly Hardware Kit |
| **id** | `x500v2_hardware_kit` |
| **partType** | Hardware |
| **category** | Other |
| **function** | Fijación mecánica de todos los subconjuntos del dron |
| **description** | Conjunto de toda la tornillería incluida en el frame kit: tornillos M3 (varios largos), tuercas nyloc M3, standoffs M2.5, arandelas, bridas de nylon, pads adhesivos 3M de doble cara, espuma anti-vibración, tuercas de hélice autoblocantes, y herramientas manuales incluidas (llaves hexagonales M3, M2.5, M2). |
| **weightKg** | 0.020 |
| **dimensions** | Variados |
| **materialType** | Acero zincado (tornillos), nylon (tuercas nyloc, bridas), aluminio (standoffs) |
| **manufacturer** | Holybro |
| **partNumber** | Incluido en SKU30120 |
| **powerConsumption** | 0 |
| **operatingTemp** | 80 |
| **difficultyLevel** | 1 |
| **installationTimeMinutes** | 0 (usado durante todo el montaje) |
| **assemblyOrder** | 0 (transversal) |
| **highlightColor** | Gris claro (0.6, 0.6, 0.6) |
| **Perfil Térmico** | N/A – No se modela individualmente en el mapa térmico. |

---

## RESUMEN: MAPA TÉRMICO PARA SHADER

| Pieza | Temp. Hover (°C) | Temp. Pico (°C) | Color Shader |
|:---|:---:|:---:|:---|
| **Motores 2216** | 50–65 | ~95 | 🔴 **Rojo intenso** |
| **ESCs BLHeli-S** | 45–65 | ~90 | 🔴 Rojo-naranja |
| **PDB** | 35–45 | ~60 | 🟠 Naranja |
| **Power Module** | 30–40 | ~55 | 🟡 Amarillo-verde |
| **Pixhawk 6C** | 35–45 | ~60 | 🟠 Naranja leve |
| **Batería 4S** | 30–45 | ~55 | 🟡 Amarillo |
| **Radio SiK** | 25–35 | ~45 | 🟢 Verde-azul |
| **GPS M10** | 20–30 | ~40 | 🔵 Azul |
| **RC Receiver** | 20–25 | ~35 | 🔵 Azul oscuro |
| **Chasis/Plates** | Ambiente | ~35 | 🔵 Azul oscuro |
| **Brazos** | Amb.+grad. | ~45 (extremo motor) | 🔵→🟢 Gradiente |
| **Tren/Railes** | Ambiente | Amb. | 🔵 Azul oscuro |
| **Hélices** | Ambiente | Amb. | 🔵 Azul (enfriamiento por convección) |

---

## DIAGRAMA DE CONEXIONES ELÉCTRICAS

```
┌─────────────┐
│ LiPo 4S     │
│ (14.8V)     │
│   XT60 out  │
└──────┬──────┘
       │ XT60
┌──────▼──────┐    GH 1.25mm 6-pin     ┌──────────────┐
│ Power Module│─────── Data ──────────▶│ PIXHAWK 6C   │
│ PM02 V3     │   (V+I sensing)        │  POWER1 port │
│   XT60 out  │                        │              │
└──────┬──────┘                        │ MAIN OUT 1-4 │──▶ PWM/DShot ──▶ ESC 1-4
       │ XT60                          │ TELEM1       │──▶ UART ──▶ SiK Radio
┌──────▼──────┐                        │ GPS          │──▶ UART/I2C ──▶ M10 GPS
│    PDB      │                        │ RC IN        │──▶ SBUS ──▶ RC Receiver
│ (XT60 in)   │                        └──────────────┘
│             │
│ XT30 out ×4 │
└──┬──┬──┬──┬─┘
   │  │  │  │   XT30 (por brazo)
   ▼  ▼  ▼  ▼
┌────┐ ┌────┐ ┌────┐ ┌────┐
│ESC1│ │ESC2│ │ESC3│ │ESC4│   (BLHeli-S 20A)
│FL  │ │FR  │ │BL  │ │BR  │
└──┬─┘ └──┬─┘ └──┬─┘ └──┬─┘
   │      │      │      │     3-phase wires
   ▼      ▼      ▼      ▼
┌────┐ ┌────┐ ┌────┐ ┌────┐
│ M1 │ │ M2 │ │ M3 │ │ M4 │   (2216 KV920)
│ CW │ │CCW │ │CCW │ │ CW │
└──┬─┘ └──┬─┘ └──┬─┘ └──┬─┘
   │      │      │      │
   ▼      ▼      ▼      ▼
 [PROP] [PROP] [PROP] [PROP]   (1045 CW/CCW)
```

---

## JERARQUÍA DE MONTAJE (Assembly Sequence)

```
Orden │ Pieza                         │ Padre en la Jerarquía
──────┼───────────────────────────────┼────────────────────────
  1   │ Bottom Plate                  │ Frame Root (Z=0)
  2   │ PDB + Power Module            │ Bottom Plate
  2   │ Arms ×4 (con ESC+Motor)       │ Bottom Plate
  3   │ Landing Gear                  │ Bottom Plate
  4   │ Top Plate                     │ (cierra sándwich)
  5   │ Pixhawk 6C                    │ Top Plate (cara superior)
  6   │ RC Receiver + SiK Radio       │ Top Plate (junto al Pixhawk)
  7   │ Platform Board                │ Top Plate (elevada por standoffs)
  7   │ GPS M10                       │ Platform Board
  8   │ Rail System + Battery Mount   │ Bottom Plate (inferior)
  9   │ Batería LiPo                  │ Battery Mount
 10   │ Hélices ×4                    │ Motores (ÚLTIMO PASO)
```

---

## ARCHIVOS CAD DISPONIBLES PARA CONVERSIÓN 3D

| Archivo | Contenido | Fuente | Uso en Pipeline |
|:---|:---|:---|:---|
| `x500v2-frame.step` | Frame completo (plates, brazos, tren, railes) | [Holybro Downloads](https://docs.holybro.com/drone-development-kit/px4-development-kit-x500v2/download) | Base geométrica principal → Blender → FBX → Unity |
| `AIR2216II_Motor_3D.STEP` | Motor 2216 KV920 detallado | Mismo paquete de descargas | Modelo de motor → Retopología → LODs |
| `Pixhawk6C-3D-CAD.stp` | Pixhawk 6C con carcasa | [Pixhawk 6C Downloads](https://docs.holybro.com/autopilot/pixhawk-6c/download) | Flight controller → simplificar para WebGL |
| `Holybro_X500_V2_3D Print.zip` | Piezas imprimibles / accesorios | Holybro Downloads | Referencia para variantes y mounts |

---

## FUENTES VERIFICADAS

1. [Holybro PX4 Dev Kit X500 V2 – Docs oficiales](https://docs.holybro.com/drone-development-kit/px4-development-kit-x500v2)
2. [PX4 Guide – X500 V2 Pixhawk 5X Build](https://docs.px4.io/main/en/frames_multicopter/holybro_x500V2_pixhawk5x)
3. [PX4 Guide – X500 V2 Pixhawk 6C Build](https://docs.px4.io/main/en/frames_multicopter/holybro_x500v2_pixhawk6c)
4. [Holybro M10 GPS – Producto](https://holybro.com/products/m10-gps)
5. [Holybro Store – PX4 Dev Kit X500 V2](https://holybro.com/products/px4-development-kit-x500-v2)
6. [BalticDrones – X500 V2 Kit](https://balticdrones.eu/products/fpv-kit-holybro-x500-v2-kit-pixhawk-6c-pm02-m10-gps-433mhz-radio)
7. [DroneDynamics – X500 V2 Kit](https://dronedynamics.ca/products/holybro-px4-development-kit-x500-v2-pixhawk-6c-m10-gps-915mhz-telemtry)
8. [FlyingTech – X500 V2 Frame Kit](https://www.flyingtech.co.uk/product/holybro-x500-v2-quadcopter-frame-kit/)
9. [FlyingTech – BLHeli-S 20A ESC](https://www.flyingtech.co.uk/product/holybro-blheli-s-esc-20a-2-4s-speed-controller/)
10. [Robu.in – Motor 2216-920KV](https://robu.in/product/holybro-s500-v2-motor-2216-920kv-cw/)
11. [Scribd – Frame Kit Assembly Guide PDF](https://www.scribd.com/document/886886667/Holybro-X500-V2-Frame-Kit-Assembly-Guide-En)
12. [Reddit r/diydrones – S500 V2 Motor Data](https://www.reddit.com/r/diydrones/comments/p6e9rg/holybro_s500_v2_kit/)
