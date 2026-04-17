---
tipo: nota_consolidada
area: webgl
estado: activo
trace_id: STUB-NR-066
aliases:
  - "IL2CPP_Backend_Config.md"
resumen: "Configuracion del backend IL2CPP para compilacion WebGL"
---

# IL2CPP Backend Config

## Proposito

Esta nota consolida la configuracion de IL2CPP como backend de compilacion para WebGL, incluyendo su papel en la conversion de IL a C++ y el tramo posterior del toolchain.

## Puntos clave

- Il2CPP no es el target final Web.
- Su salida intermedia alimenta el flujo de compilacion posterior.
- La configuracion debe ser coherente con memoria, stripping y compatibilidad del navegador.

## Criterios de validacion

1. Revisar que el backend seleccionado coincida con la version objetivo de Unity.
2. Documentar flags y opciones relevantes del Player Settings.
3. Verificar que el cambio no rompa el perfil de memoria ni el arranque.

## Relacion con el grafo

- `MOC_WebGL_Build_Pipeline`
- `MOC_Consolidacion_WebGL`
- `WEBGL_BUILD_SETTINGS.md`

## Enlaces de continuidad

- [[MOC_Consolidacion_Stubs_NoResueltas]]
- [[MOC_Conectividad_Total]]

