# Auditoría de Lógica Matemática
## Propuesta e Informe Final - Tesis WebGL

---

## 1. Propósito

Verificar la correcta formulación de las expresiones matemáticas, fórmulas y ecuaciones presentes en los trabajos escritos, asegurando consistencia teórica y notación apropiada.

---

## 2. Revisión de Fórmulas en Marco Teórico

### 2.1 Teoría de Carga Cognitiva

| Fórmula | Documento | Estado | Observación |
|---------|-----------|--------|-------------|
| $\sim$4 elementos (Cowan, 2001) | Propuesta | ✅ | Referencia correcta |
| Capacidad memoria trabajo | Propuesta | ✅ | Definición conceptual |

### 2.2 Optimización Gráfica

| Fórmula | Documento | Estado | Verificación |
|---------|-----------|--------|--------------|
| $P_{total} = \sum_{i=1}^{n} P_i \leq 100,000$ | Propuesta | ✅ | Notación correcta |
| $\rho = \frac{R_{texture}}{A_{UV}} = 10.24 \frac{px}{cm}$ | Propuesta | ✅ | Dimensionalmente correcta |
| $D_{optimizado} = \frac{D_{original}}{N_{instances}}$ | Propuesta | ⚠️ | Revisar: debería ser $D_{optimizado} = D_{original} - N_{instances} + 1$ para batching |
| $T_{frame} = T_{CPU} + T_{GPU} < 33.33ms$ | Propuesta | ✅ | Suma correcta (33.33ms = 30 FPS) |

### 2.3 Renderizado PBR

| Fórmula | Documento | Estado | Verificación |
|---------|-----------|--------|--------------|
| $f_{spec} = \frac{D \cdot F \cdot G}{4(\mathbf{n} \cdot \mathbf{l})(\mathbf{n} \cdot \mathbf{v})}$ | Propuesta | ✅ | BRDF Cook-Torrance correcta |
| $F = F_0 + (1 - F_0)(1 - \cos\theta)^5$ | Propuesta | ✅ | Schlick approximation correcta |
| $D_{GGX} = \frac{\alpha^2}{\pi((\mathbf{n} \cdot \mathbf{h})^2(\alpha^2 - 1) + 1)^2}$ | Propuesta | ✅ | GGX NDF correcta |
| $E_{reflected} + E_{absorbed} = E_{incident}$ | Propuesta | ✅ | Conservación de energía |

---

## 3. Fórmulas en Metodología

### 3.1 Sistema Usability Scale (SUS)

| Fórmula | Documento | Estado |
|---------|-----------|--------|
| $SUS = 2.5 \times \sum_{i=1}^{10} s_i$ | Propuesta/Informe | ✅ |

**Verificación de cálculo**:
- $s_i$ para ítems impares: $s_i = Valor - 1$ (rango 0-4)
- $s_i$ para ítems pares: $s_i = 5 - Valor$ (rango 0-4)
- Suma máxima: $10 \times 4 = 40$
- SUS máximo: $40 \times 2.5 = 100$ ✅

### 3.2 NASA-TLX

| Fórmula | Documento | Estado |
|---------|-----------|--------|
| $NASA-TLX_{score} = \frac{1}{6} \sum_{d=1}^{6} D_d$ | Propuesta/Informe | ✅ |

**Verificación**:
- $D_d$ rango: 0-100 por dimensión
- Suma máxima: $6 \times 100 = 600$
- Score máximo: $600 / 6 = 100$ ✅

---

## 4. Análisis de Errores Encontrados

### 4.1 Draw Calls - Fórmula Incorrecta

**Actual en Propuesta**:
$$D_{optimizado} = \frac{D_{original}}{N_{instances}}$$

**Corrección recomendada**:
$$D_{optimizado} \approx D_{original} \times \frac{1}{N_{batches}}$$

O más simple:
$$D_{optimizado} < 50 \text{ (target)}$$

**Razón**: La reducción de draw calls no es una división directa por instancias, sino que depende del batching y GPU instancing. La fórmula actual es simplista.

---

## 5. Símbolos y Notación

### 5.1 Verificación de Símbolos

| Símbolo | Significado | Uso | Estado |
|---------|-------------|-----|--------|
| $\mathbf{n}$ | Normal de superficie | shading | ✅ |
| $\mathbf{l}$ | Vector luz | shading | ✅ |
| $\mathbf{v}$ | Vector vista | shading | ✅ |
| $\mathbf{h}$ | Vector half | shading | ✅ |
| $\alpha$ | roughness² | shading | ✅ |
| $\theta$ | Ángulo | Fresnel | ✅ |
| $\rho$ | Densidad texel | Texturas | ✅ |
| $P$ | Polígonos | Métricas | ✅ |
| $D$ | Draw calls | Métricas | ✅ |
| $T$ | Tiempo (frame) | Métricas | ✅ |

### 5.2 Unidades

| Métrica | Unidad | Correcto |
|---------|--------|----------|
| Polígonos | triángulos | ✅ |
| Textel density | px/cm | ✅ |
| Frame time | ms | ✅ |
| FPS | frames/segundo | ✅ |
| VRAM | MB | ✅ |
| TTI | segundos | ✅ |

---

## 6. Consistencia entre Propuesta e Informe

| Fórmula | Propuesta | Informe Final | Coherencia |
|---------|-----------|---------------|------------|
| SUS | ✅ | ✅ | ✅ |
| NASA-TLX | ✅ | ✅ | ✅ |
| Presupuesto poligonal | ✅ | ✅ | ✅ |
| Frame time | ✅ | ✅ | ✅ |
| BRDF Cook-Torrance | ✅ | ✅ | ✅ |
| Fresnel Schlick | ✅ | ✅ | ✅ |

---

## 7. Plan de Acción

### 7.1 Corrección Requerida
- [ ] Corregir fórmula de draw calls en `Propuesta/sections/marco_teorico.tex`

### 7.2 Verificaciones Adicionales
- [ ] Revisar que todas las referencias a fórmulas sean consistentes
- [ ] Verificar que los valores numéricos en las fórmulas sean realistas

---

## 8. Recomendaciones

### 8.1 Mejora en la Presentación
- Añadir explicación de variables para cada fórmula
- Incluir referencias a las fuentes teóricas de cada ecuación

### 8.2 Precisión Matemática
- La fórmula de draw calls debe reflejar que es un límite, no una ecuación calculable
- Considerar añadir margen de seguridad a los KPIs

---

*Auditoría creada: 2026-04-09*
*Área: Mathematical Logic*
