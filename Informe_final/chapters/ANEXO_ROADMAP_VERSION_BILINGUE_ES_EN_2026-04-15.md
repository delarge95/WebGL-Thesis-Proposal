# Anexo: Roadmap de Version Bilingue ES/EN para la Aplicacion WebGL

## 1. Proposito y Justificacion Academica
Este roadmap define la evolucion de la aplicacion desde una UI principal en ingles hacia una version bilingue (espanol/ingles) con localizacion estructurada (i18n/l10n), manteniendo consistencia tecnica, trazabilidad y calidad de experiencia de usuario.

La decision se justifica en cuatro ejes:
- Alcance internacional: la version en ingles mejora transferencia, visibilidad y reutilizacion del artefacto en contextos academicos y profesionales globales.
- Validez local: la disponibilidad en espanol mejora accesibilidad para usuarios hispanohablantes y escenarios de capacitacion local.
- Rigor metodologico: i18n formal reduce deuda tecnica frente a traducciones ad hoc en strings embebidos.
- Sostenibilidad: habilita escalar a nuevos idiomas con costo marginal decreciente.

## 2. Estado Base (Corte 2026-04-15)
- Copy runtime de la app normalizado a ingles en datos y UI principal.
- Politica uniforme para datos faltantes: N/A.
- Evidencia de consistencia disponible en auditorias tecnicas de Holybro.

Base tecnica relevante:
- UI superior: EnhancedInfoPanel.
- UI inferior: UIDetailsSheet + MainLayout.uxml.
- Datos: ScriptableObjects en X500V2Generated.
- Textos auxiliares: DronePartData.GetFullDescription y export CSV de BOM.

## 3. Objetivo de la Version Bilingue
### Objetivo general
Implementar seleccion de idioma ES/EN con persistencia de preferencia, cobertura de textos de interfaz y contenido dinamico, sin regresiones funcionales ni de rendimiento.

### Objetivos especificos
- Externalizar textos de UI y mensajes a un sistema de claves i18n.
- Traducir contenido de datos (descripcion, funcion, materiales, tips) en ambos idiomas.
- Mantener coherencia terminologica tecnico-industrial entre ES y EN.
- Incorporar validacion automatizada de cobertura de traducciones.

## 4. Alcance
### En alcance
- UI textual: etiquetas, placeholders, botones, mensajes de estado, tooltips.
- Datos mostrados al usuario: descripciones y campos tecnicos por pieza.
- Exportables textuales (CSV BOM y descripciones generadas) con opcion de idioma.
- Persistencia de idioma en sesion y recarga.

### Fuera de alcance (fase inicial)
- Doblaje/locucion o audio multiidioma.
- Traduccion automatica en tiempo real mediante servicios externos.
- Soporte RTL (right-to-left) para idiomas no latinos.

## 5. Arquitectura Propuesta de i18n/l10n
### 5.1 Estructura de recursos
- Crear catalogos por idioma:
  - Assets/Resources/i18n/en.json
  - Assets/Resources/i18n/es.json
- Estructura por namespaces:
  - ui.panels.*
  - ui.buttons.*
  - ui.labels.*
  - ui.messages.*
  - data.parts.<id>.*

### 5.2 Servicio central
- Implementar I18nService con responsabilidades:
  - Carga de catalogos por idioma.
  - Resolucion por clave con fallback en cadena (idioma seleccionado -> en -> clave literal).
  - Formateo con placeholders tipados.
  - Evento OnLanguageChanged para refresco de vistas activas.

### 5.3 Integracion UI
- Sustituir literales hardcoded por claves i18n en:
  - EnhancedInfoPanel
  - UIDetailsSheet
  - UXML de MainLayout (labels estaticos)
- Mantener reglas de formato de unidades separadas de la traduccion:
  - Peso: g/kg
  - Faltantes: N/A

### 5.4 Integracion de datos de piezas
- Estrategia recomendada (faseada):
  - Fase A: tabla de overrides traducidos por ID y campo (description, function, materialType, materialProperties).
  - Fase B: migracion completa a estructura bilingue en metadata.

## 6. Gobernanza Terminologica
- Crear glosario tecnico ES/EN versionado (ej. docs/glossary_ui_tech_es_en.md).
- Definir estilo editorial:
  - Voz imperativa para acciones de UI.
  - Terminologia consistente para fasteners, frame, avionics, power, thermal.
  - No mezclar abreviaturas locales ambiguas; usar N/A para no disponible.
- Flujo de aprobacion:
  - Autor tecnico -> revisor UX -> aprobacion final.

## 7. Plan por Fases
## Fase 0: Baseline y Preparacion (1 semana)
- Congelar inventario de strings y mapear ubicacion por archivo.
- Marcar criticidad (bloqueante/no bloqueante) por string.
- Entregable: matriz de cobertura inicial.

## Fase 1: Infraestructura i18n (1 semana)
- Implementar I18nService y contrato de consumo.
- Crear catalogos base en/es para UI critica.
- Integrar selector de idioma y persistencia (PlayerPrefs o equivalente).
- Entregable: cambio de idioma funcional en shell UI.

## Fase 2: Migracion UI Principal (1-2 semanas)
- Migrar EnhancedInfoPanel y UIDetailsSheet.
- Migrar UXML estatico y mensajes de estado.
- Entregable: 100% UI principal sin strings hardcoded.

## Fase 3: Contenido de Datos (2 semanas)
- Traducir campos dinamicos de piezas por ID.
- Resolver estrategia para textos de fasteners y notas CAD.
- Entregable: cobertura bilingue del contenido mostrado al usuario.

## Fase 4: Validacion Integral (1 semana)
- QA funcional, visual y linguistica.
- Pruebas de regresion y performance.
- Entregable: informe de calidad y checklist de release.

## Fase 5: Cierre Academico (0.5 semana)
- Documentar resultados, limitaciones y trabajo futuro.
- Adjuntar evidencias y metricas al informe final.

## 8. Cronograma Sugerido
Duracion total estimada: 5.5 a 7.5 semanas.

Dependencias criticas:
- Fase 2 depende de Fase 1.
- Fase 3 depende de modelo de datos definido en Fase 1.
- Fase 4 requiere cierre funcional de Fases 2 y 3.

## 9. Riesgos y Mitigaciones
- Riesgo: strings hardcoded residuales en rutas poco usadas.
  - Mitigacion: auditoria automatica de patrones y bloqueo en CI.
- Riesgo: inconsistencia terminologica ES/EN.
  - Mitigacion: glosario versionado + revision editorial.
- Riesgo: degradacion de rendimiento por recarga de catálogos.
  - Mitigacion: cache en memoria y carga diferida por namespace.
- Riesgo: desalineacion entre textos de datos y UI.
  - Mitigacion: pruebas de snapshot y validacion de placeholders.

## 10. Estrategia de Pruebas
### 10.1 Pruebas unitarias
- Resolucion de claves con fallback.
- Formateo de placeholders por idioma.
- Persistencia y restauracion de idioma.

### 10.2 Pruebas de integracion
- Cambio de idioma en runtime refrescando paneles abiertos.
- Verificacion de textos dinamicos por seleccion de pieza.
- Export CSV BOM en idioma seleccionado.

### 10.3 Pruebas E2E
- Flujo completo de inspeccion de pieza en ES y EN.
- Validacion de coherencia visual en WebGL build.

### 10.4 Pruebas de calidad linguistica
- Muestreo estratificado por categoria de pieza.
- Validacion de legibilidad, consistencia y precision tecnica.

## 11. Metricas de Exito
- Cobertura i18n UI: >= 99% de strings en catalogo.
- Cobertura i18n datos visibles: >= 98%.
- Defectos linguistico-funcionales criticos en QA: 0.
- Tiempo de cambio de idioma percibido: <= 150 ms promedio.
- Regression funcional en paneles principales: 0.

## 12. Criterios de Aceptacion para Entrega
- Selector de idioma operativo (ES/EN) y persistente.
- No existen strings hardcoded visibles en UI principal.
- Datos de piezas visibles en ambos idiomas con fallback seguro.
- Exportables textuales respetan idioma activo.
- Evidencia documental de pruebas y cobertura adjunta al informe.

## 13. Evidencias Sugeridas para el Informe Final
- Matriz de strings antes/despues (hardcoded vs i18n).
- Capturas comparativas ES/EN por panel.
- Reporte de cobertura de claves y faltantes.
- Registro de defectos y resolucion.
- Resultados de pruebas de usabilidad por idioma.

## 14. Recomendacion de Implementacion
Aplicar estrategia incremental: primero infraestructura y UI critica, luego contenido de datos. Esta secuencia minimiza riesgo, facilita validacion continua y mantiene control metodologico para una entrega academica robusta.
