# Guia de Retopologia por Pieza — Holybro X500 V2

## Objetivo

Definir la topologia de malla requerida para cada pieza del dron segun su funcion termica en el sistema de simulacion. Las piezas criticas necesitan mallas mas densas y limpias para soportar gradientes de temperatura visualmente creibles.

## Criterios de Clasificacion

| Tier | Descripcion | Topologia | Polys estimados | UV |
|------|-------------|-----------|----------------:|-----|
| **Critico** | Fuentes de calor principales o sumideros con gradientes visibles | Quad-dominant, densidad aumentada cerca de hotspots, loops de soporte | 1500–4000 | Unwrap limpio, sin solapamiento, padding 4px min |
| **Secundario** | Piezas con conduccion pasiva o contacto termico directo | Mix quad/tri aceptable, topologia limpia sin ngons | 500–1500 | Unwrap funcional, solapamiento simetrico permitido |
| **Minimo** | Piezas sin rol termico significativo | Low-poly agresivo, tris permitidos | 200–600 | Atlas packing, auto-unwrap aceptable |

---

## Clasificacion por Pieza

### Tier Critico — Fuentes de calor y sumideros principales

#### Motores (×4): `x500v2_motor_FL`, `_FR`, `_BL`, `_BR`

- **Funcion termica**: Fuente de calor primaria. Pico ~90°C bajo carga alta.
- **Patron visual**: Radial — calor concentrado en centro del bobinado, enfriamiento en extremos.
- **Topologia requerida**:
  - Quads limpios en cilindro principal
  - Edge loops concentricos en zona del estator (centro del motor)
  - Densidad mayor en la mitad inferior (zona de contacto con brazo/ESC)
  - Min 8 segmentos radiales en seccion circular
- **Polys**: 2000–3000
- **UV**: Planar Y para caras superior/inferior, cilindrica para cuerpo

#### ESCs (×4): `x500v2_esc_FL`, `_FR`, `_BL`, `_BR`

- **Funcion termica**: Fuente de calor secundaria. MOSFETs alcanzan ~90°C.
- **Patron visual**: Axial — calor concentrado en MOSFETs (centro de la PCB), disipacion hacia extremos de la placa.
- **Topologia requerida**:
  - Quad planar limpio con subdivision adicional en zona central (donde se ubican MOSFETs)
  - Edge loops paralelos al eje largo
  - Evitar triangulos en caras frontales
- **Polys**: 600–1200
- **UV**: Planar Z, una isla por cara visible

#### Bateria: `x500v2_battery`

- **Funcion termica**: Fuente de calor interna por resistencia. Rango operativo estrecho 0–45°C. Warmup lento (15s).
- **Patron visual**: Radial — calor uniforme con concentracion interna, enfriamiento en extremos.
- **Topologia requerida**:
  - Caja subdividida con edge loops en cortes transversales (min 3 cortes)
  - Quads limpios en las 6 caras
  - Densidad ligeramente mayor en extremos donde contacta con rails
- **Polys**: 800–1500
- **UV**: Box projection limpio

#### Brazos (×4): `x500v2_arm_FL`, `_FR`, `_BL`, `_BR`

- **Funcion termica**: Sumidero estructural. Conducen calor de motor/ESC hacia placas centrales. Alta exposicion al aire.
- **Patron visual**: Axial — gradiente lineal de extremo caliente (motor) a extremo frio (placa central).
- **Topologia requerida**:
  - Tubo cuadrado con subdivisiones a lo largo del eje principal (min 6 segmentos longitudinales)
  - Edge loops uniformemente distribuidos para gradiente suave
  - Quads obligatorios
- **Polys**: 400–800 por brazo
- **UV**: Cilindrica desenvuelta, una isla continua

### Tier Secundario — Contacto termico pasivo

#### Placa inferior: `x500v2_bottom_plate`

- **Funcion termica**: Hub de conduccion central. Conecta brazos, PDB, rails de bateria.
- **Topologia**: Plano con agujeros de montaje. Quads limpios, densidad uniforme.
- **Polys**: 600–1000
- **UV**: Planar Y

#### Placa superior: `x500v2_top_plate`

- **Funcion termica**: Conecta a Pixhawk, platform board, radio, receiver. Conduccion moderada.
- **Topologia**: Plano con agujeros. Similar a placa inferior.
- **Polys**: 600–1000
- **UV**: Planar Y

#### PDB: `x500v2_pdb`

- **Funcion termica**: ElectronicsHigh. Pistas de cobre de alta corriente. Contacto directo con placa inferior.
- **Topologia**: Plano fino con subdivision en zona central.
- **Polys**: 300–600
- **UV**: Planar Z

#### Power Module: `x500v2_power_module`

- **Funcion termica**: ElectronicsHigh. Regulador de voltaje, disipa calor moderado.
- **Topologia**: Caja simple con subdivision en cara superior (componentes).
- **Polys**: 300–500
- **UV**: Box projection

#### Pixhawk 6C: `x500v2_pixhawk6c`

- **Funcion termica**: ElectronicsHigh. Procesador de vuelo genera calor continuo.
- **Topologia**: Caja con detalle en cara superior (conectores, LEDs).
- **Polys**: 500–800
- **UV**: Box limpio

#### Rails de bateria: `x500v2_rails_battery`

- **Funcion termica**: Conductor de contacto entre placa inferior y bateria.
- **Topologia**: Extrusion simple con edge loops en puntos de contacto.
- **Polys**: 200–400
- **UV**: Cilindrica

### Tier Minimo — Sin rol termico significativo

#### Platform Board: `x500v2_platform_board`

- **Polys**: 200–400
- **UV**: Planar auto

#### GPS M10: `x500v2_gps_m10`

- **Polys**: 300–500
- **UV**: Planar auto

#### Telemetry Radio: `x500v2_telemetry_radio`

- **Polys**: 200–400
- **UV**: Planar auto

#### RC Receiver: `x500v2_rc_receiver`

- **Polys**: 200–300
- **UV**: Planar auto

#### Landing Gear: `x500v2_landing_gear`

- **Funcion termica**: Sumidero pasivo con alta exposicion al aire. Sin fuente de calor propia.
- **Polys**: 400–800
- **UV**: Cilindrica auto

#### Propellers (×4): `x500v2_propeller_FL`, `_FR`, `_BL`, `_BR`

- **Funcion termica**: Ninguna directa. No se calientan significativamente.
- **Polys**: 200–400
- **UV**: Planar auto

---

## Resumen de Presupuesto

| Tier | Piezas | Polys por pieza | Total estimado |
|------|-------:|----------------:|---------------:|
| Critico | 13 (4M + 4E + 1B + 4A) | 600–3000 | ~18,000 |
| Secundario | 6 | 200–1000 | ~4,000 |
| Minimo | 9 | 200–500 | ~3,000 |
| **Total** | **28** | — | **~25,000** |

Este presupuesto es compatible con el target de rendimiento WebGL movil a 23+ FPS.

---

## Reglas Generales

1. **No ngons en la exportacion final**. Blender puede tenerlos durante edicion pero deben resolverse antes de exportar.
2. **Quads obligatorios en Tier Critico**. El shader termico proyecta gradientes que se ven mal con triangulaciones irregulares.
3. **Edge loops en zonas de contacto**. Si una pieza toca otra (ej: motor sobre brazo), la zona de contacto debe tener edge loops definidos para que el gradiente visual sea suave.
4. **Orientar la topologia al flujo de calor**. En piezas axiales (brazos, ESCs), los edge loops deben ser perpendiculares al eje largo. En piezas radiales (motores, bateria), loops concentricos.
5. **Escala en Blender**. Exportar con la misma escala que el modelo actual en Unity. Verificar que las bounds de cada pieza coincidan con las usadas por el `ThermalContactGraphBuilderWindow`.
