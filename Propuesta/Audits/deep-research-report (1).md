# Auditoría de cumplimiento APA 7 según la UNAD en la propuesta corregida

## Resumen ejecutivo

El documento **no está aún “totalmente correcto”** bajo el instructivo APA 7 de la entity["organization","Universidad Nacional Abierta y a Distancia","distance university colombia"] (UNAD). Los problemas más relevantes son:

- **Formato global APA‑UNAD**: el instructivo exige (y recomienda) parámetros concretos (doble espacio, sin espacios entre párrafos, Times New Roman 12, títulos sin numeración). En el PDF actual hay señales de **no alineación** (p. ej., encabezados con “Fase 1…”, y el documento está maquetado en LaTeX con apariencia/espaciado/fuente no verificables como Times 12). citeturn2view0  
- **Citas en texto (dos autores)**: el instructivo UNAD muestra explícitamente **“y”** en cita parentética de dos autores (no “&”). En la propuesta aparecen varias citas parentéticas con “&”. citeturn5view0  
- **Lista de referencias**: hay mejoras (DOIs ya corregidos en algunas entradas), pero persisten **incumplimientos típicos de APA 7**: títulos en **Title Case** en artículos/libros donde APA pide **sentence case** (lo cual, además, coincide con los ejemplos del instructivo). citeturn4view2turn3view2  
- **Algunas URLs se parten dentro del dominio** en el render del PDF (p. ej. “docs.uni / ty3d.com”). Esto puede ser solo salto de línea visual, pero si introduce espacios/guiones invisibles al copiar/pegar, deja referencias técnicamente rotas. El instructivo trata el DOI como URL (sin etiqueta “DOI:”), por lo que el enlace debe quedar íntegro. citeturn4view2turn3view2  

Conclusión: el documento está **cerca**, pero requiere una pasada sistemática de normalización (citas en texto + capitalización APA de títulos + adecuación formal UNAD de encabezados/espaciado/fuente).

## Marco normativo aplicable: “Norma APA 7ª Edición” UNAD

Para UNAD, el **instructivo institucional** indica explícitamente:

- **Interlineado 2.0**, sin espacios entre párrafos, y paginación en esquina superior derecha. citeturn2view0  
- **Fuente recomendada (UNAD): Times New Roman 12**. citeturn2view0  
- **Alineado a la izquierda sin justificar** y **sangría de primera línea (1,27 cm)**; cada título de primer nivel inicia en página nueva. citeturn2view0  
- **“Los títulos no se etiquetan con letras o números.”** citeturn2view0  
- En citación de **dos autores** el instructivo usa **“y”** en cita parentética y narrativa (tabla de estilos de cita: “(Pinedo y Soria, 2008, p.17)”). citeturn5view0  
- Para DOI: el instructivo indica que los DOI se presentan con formato de URL y que la etiqueta “DOI:” ya no es necesaria. citeturn4view2turn3view2  

## Hallazgos de no conformidad en citación dentro del texto

### Uso de “&” en citas parentéticas de dos autores

**Norma UNAD:** En cita parentética con **dos autores**, el instructivo presenta la conjunción **“y”**. citeturn5view0  

**Evidencia en la propuesta (ejemplos localizados):**
- “(Hart & Staveland, 1988)” (sección de variables).  
- “(Ericsson & Simon, 1993)” (análisis cualitativo).  
- “(Cook & Torrance, 1982; …)” (PBR).  
- “(… Sauro & Lewis, 2016)” (interpretación SUS).

**Corrección requerida (regla UNAD):**
- Reemplazar **“&” → “y”** en **todas** las citas parentéticas de dos autores:  
  - (Hart y Staveland, 1988)  
  - (Ericsson y Simon, 1993)  
  - (Cook y Torrance, 1982)  
  - (Sauro y Lewis, 2016)

### Consistencia “et al.” y separadores

La propuesta usa “et al.” para 3+ autores y usa “;” para separar varias fuentes en un mismo paréntesis, lo cual es consistente con práctica APA; no se detecta un conflicto directo con el instructivo en lo mostrado. (Aun así, conviene homogeneizar idioma: si el documento completo está en español, mantener “et al.” como forma latina estándar y mantener consistencia tipográfica.)

## Hallazgos de no conformidad en lista de referencias

### Capitalización de títulos: Title Case vs sentence case

El instructivo UNAD ejemplifica títulos de artículos en **sentence case** (p. ej., “Gestión del conocimiento y tecnologías…”), coherente con APA 7. citeturn4view2turn3view2  

En la propuesta, múltiples entradas están en **Title Case** (cada palabra importante en mayúscula), lo cual es típico de estilos BibTeX no‑APA y **no corresponde** al estándar APA 7.

**Entradas a corregir (patrones claros):**
- Artículos clásicos en inglés (p. ej., “The Magical Number Seven, Plus or Minus Two: Some Limits on Our Capacity for Processing Information.”) → debe pasar a **“The magical number seven, plus or minus two: Some limits on our capacity for processing information.”**  
- Libros (“The Design of Everyday Things: Revised and Expanded Edition.”) → **sentence case** (manteniendo nombres propios).  
- Páginas web (“Why You Only Need to Test with 5 Users.”) → **sentence case**.

**Corrección requerida:**
- Convertir **títulos de obras** (libros, artículos, capítulos, páginas web) a **sentence case**.  
- Mantener mayúsculas solo para: primera palabra, primera palabra tras “:”, nombres propios y siglas.

> Nota técnica (LaTeX/BibTeX): si estás usando BibTeX/BibLaTeX, esto suele requerir ajustar el estilo bibliográfico (APA) y/o proteger nombres propios con llaves en el .bib para evitar “downcasing” indebido.

### Integridad y “ruptura” de URLs/DOIs por saltos de línea

En el PDF, varias URLs aparecen partidas dentro del dominio (p. ej. “docs.uni / ty3d.com”; “https://do / i.org/…”). En APA, se admite salto de línea, pero el enlace debe quedar **sin inserción de guiones o espacios** que lo corrompan. El instructivo trata el DOI como URL, por lo que **la exactitud del enlace es parte del formato correcto**. citeturn3view2turn4view2  

**Corrección recomendada (técnica de composición):**
- En Word: evitar guionado automático en URLs.  
- En LaTeX: usar `\url{}`/`\href{}` correctamente y revisar el resultado final (que no inserte caracteres extra al copiar).

### Referencia web de entity["organization","Nielsen Norman Group","ux consultancy firm"] y riesgo de “fuente no primaria”

Formato APA (web) está bien estructurado (Autor. Fecha. Título. Sitio. URL), pero recuerda: si este documento busca un estándar de tesis, **esa pieza es secundaria** y suele complementarse mejor con fuentes académicas primarias sobre tamaño muestral. Esto no es “formato APA”, pero sí “calidad bibliográfica”. (Si decides mantenerla, al menos debe quedar perfectamente en sentence case.)

## Hallazgos de formato general UNAD (no solo referencias)

### Numeración en encabezados (“Fase 1…”, “Fase 2…”) vs regla UNAD

El instructivo UNAD indica explícitamente: **“Los títulos no se etiquetan con letras o números.”** citeturn2view0  

En la propuesta, aparecen encabezados tipo “Fase 1: …”, “Fase 2: …”. Si UNAD exige cumplimiento literal, esto es **no conforme**.

**Corrección recomendada:**
- Cambiar encabezados a formato sin número:  
  - “Fase de análisis y fundamentación”  
  - “Fase de pipeline de activos y visualización”  
  - etc.  
- Si necesitas conservar el orden, hacerlo en el cuerpo del texto con una lista (no en el título).

### Portada: capitalización del título

El instructivo UNAD ejemplifica título de portada en minúsculas con mayúscula solo en nombres propios/siglas. citeturn2view0  

Tu portada usa estilo tipo Title Case (“Diseño y Desarrollo…”). Para ajuste estricto UNAD, debería ir en **sentence case** (manteniendo siglas: Web 3D, WebAssembly, etc.).

### Fuente e interlineado

El instructivo recomienda Times New Roman 12 para UNAD y exige doble espacio. citeturn2view0  

Tu PDF está generado con LaTeX (metadatos típicos). Sin verificación visual tipográfica, **no puedo afirmar** que cumple Times New Roman 12 y doble espacio. Si el jurado aplica el instructivo de forma literal, esto puede ser un motivo de corrección formal.

**Acción concreta recomendada:** generar una versión en la plantilla UNAD o ajustar LaTeX para que replique: Times New Roman 12, doble espacio, y estilos de título (sin numeración).

## Correcciones listas para aplicar

### Corrección masiva de citas parentéticas “&” → “y” (texto)

Buscar y reemplazar en el cuerpo (no en bibliografía):
- `(X & Y, AAAA)` → `(X y Y, AAAA)`
- `(…; X & Y, AAAA)` → `(…; X y Y, AAAA)`

### Plantillas APA 7 UNAD para referencias (para pegar)

A continuación dejo plantillas (no “citas finales”), para que las adaptes a cada entrada; el DOI debe ir como enlace URL y sin etiqueta, según el instructivo. citeturn3view2turn4view2  

```text
Artículo con DOI:
Apellido, Inicial(es). (Año). Título del artículo en sentence case. Título de la revista, volumen(número), pp–pp. https://doi.org/xxxx

Página web con autor y fecha:
Autor/Organización. (Año, día de mes). Título de la página en sentence case. Nombre del sitio. URL

Libro:
Apellido, Inicial(es). (Año). Título del libro en sentence case (edición). Editorial.
```

### Ejemplos de corrección (solo capitalización) sobre tus entradas

```text
Miller, G. A. (1956). The magical number seven, plus or minus two: Some limits on our capacity for processing information. Psychological Review, 63(2), 81–97.

Nielsen, J. (2000). Why you only need to test with 5 users. Nielsen Norman Group. [URL]
```

(En el segundo ejemplo, mantén la URL exacta ya presente en tu referencia; aquí la omito para no introducir errores.)

## Recomendación final y criterio de “todo correcto”

Bajo el estándar UNAD, “todo correcto” implica dos capas:

1) **Conformidad formal**: portada (sentence case), tipografía/interlineado, encabezados sin numeración, y reglas de citación (incluida la conjunción “y” para dos autores). citeturn2view0turn5view0  
2) **Conformidad bibliográfica APA**: títulos en sentence case, DOI como URL sin etiqueta, URLs sin corrupción por saltos, y referencias completas (sin omisiones). citeturn3view2turn4view2  

En tu versión actual, lo más crítico para cerrar es: **(i) reemplazar “&” por “y” en citas de dos autores**, **(ii) normalizar titles a sentence case en toda la bibliografía**, y **(iii) asegurar que el documento se ajusta a la plantilla/estilo UNAD (sin numeración de títulos y con doble espacio/Times 12)**. citeturn2view0turn5view0