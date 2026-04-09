# Auditoría de Formato APA 7
## Propuesta e Informe Final - Tesis WebGL

---

## 1. Estado Actual

### 1.1 Propuesta - bibliografia.tex
Total de referencias: 35

| # | Estado | Referencia | Error Detectado |
|---|--------|------------|-----------------|
| 1 | ⚠️ | Bartlett (2023) | DOI con formato correcto |
| 2 | ✅ | Köhler (1929) | Correcto |
| 3 | ⚠️ | Mayer (2021) | DOI necesita verificación |
| 4 | ⚠️ | three.js (2024) | Información adicional innecesaria |
| 5 | ⚠️ | Nielsen (2000) | Verificar URL |
| 6 | ⚠️ | Sweller (2019) | Verificar DOI |
| 7 | ⚠️ | Unity Technologies (2024) | Dos referencias con mismo año |

### 1.2 Informe Final - 07_referencias.tex
Total de referencias: 45

| # | Estado | Referencia | Error Detectado |
|---|--------|------------|-----------------|
| 1 | ✅ | Bartlett (2023) | Correcto |
| 2 | ✅ | Brooke (1996) | Correcto |
| 3 | ✅ | Hart (1988) | Correcto |
| 4 | ✅ | Unity Technologies (2024a-d) | Formato correcto con subíndices |

---

## 2. Errores Específicos a Corregir

### 2.1 Propuesta - bibliografia.tex

| Línea | Actual | Corrección Requerida |
|-------|--------|---------------------|
| 18 | `\url{https://doi.org/10.1080/...}` | ✅ Ya tiene formato correcto |
| 56 | `...(110,000 stars as of November 2024)` | Eliminar paréntesis con info adicional |
| 52 | DOI Mayer | Verificar que termina en `...4332` no `...4333` |
| 76-78 | Unity Technologies duplicado | Usar formato (2024a), (2024b) |

### 2.2 Verificación de DOIs

```
doi.org/10.1080/13875868.2021.1987375 ✅ Bartlett
doi.org/10.1017/9781108894333 ❌ Mayer (debería ser 9781108894332 o verificar)
doi.org/10.1007/s10648-019-09465-5 ✅ Sweller
doi.org/10.1145/357290.357293 ✅ Cook-Torrance
```

---

## 3. Criterios de Cumplimiento APA 7

### 3.1 Referencias Bibliográficas
- [ ] Orden alfabético por apellido del primer autor
- [ ] Formato de DOI: `https://doi.org/xxxxx` (no usar `doi:`)
- [ ] Años de publicación completos (4 dígitos)
- [ ] Nombres de revistas en cursiva y formato correcto (Title Case)
- [ ] Números de volumen en cursiva
- [ ] Páginas en formato pp. o sin prefijo (APA 7 permite ambos)

### 3.2 Recursos Electrónicos
- [ ] URLs activos y verificados
- [ ] Incluir fecha de acceso solo si no hay fecha de publicación
- [ ] No incluir información adicional innecesaria (estrellas, descargas, etc.)

### 3.3 Múltiples Autores del Mismo Año
- [ ] Usar sufijos a, b, c para diferenciarlos
- [ ] Ejemplo: Unity Technologies (2024a), Unity Technologies (2024b)

---

## 4. Acciones Requeridas

### 4.1 En Propuesta
- [ ] Eliminar "(110,000 stars...)" de referencia three.js (línea 56)
- [ ] Verificar DOI de Mayer (2021)
- [ ] Añadir sufijos a, b en Unity Technologies 2024
- [ ] Verificar que todas las URLs estén activas

### 4.2 En Informe Final
- [ ] Verificar consistencia con Propuesta
- [ ] Revisar orden alfabético completo

---

## 5. Recursos de Referencia

- APA 7th Edition Manual
- Plantilla UNAD disponible en: `External_docs/Plantilla_Normas_APA_7a_Edicion.pdf`
- Generador de DOI: https://doi.org/

---

*Auditoría creada: 2026-04-09*
*Área: APA Format*
