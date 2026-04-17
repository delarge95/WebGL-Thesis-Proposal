<%\*
const activeFile = app.workspace.getActiveFile();
const selection = tp.file.selection();

if (!activeFile || activeFile.extension !== "pdf") {
new Notice("Error: abre un PDF y selecciona texto antes de ejecutar esta plantilla.");
return;
}

const rawTitle = activeFile.basename || "Fuente";
const nowYear = tp.date.now("YYYY");
const author = await tp.system.prompt("Autor (apellido)", "Autor");
const idea = await tp.system.prompt("Concepto atomico", "Concepto");

if (!idea) {
new Notice("Operacion cancelada: falta nombre conceptual.");
return;
}

const safeIdea = idea.replace(/[^a-zA-Z0-9 _-]/g, "").trim().replace(/\s+/g, "_");
const safeAuthor = (author || "Autor").replace(/[^a-zA-Z0-9_-]/g, "");
const fileName = `Lit_${nowYear}_${safeAuthor}_${safeIdea}`;
const folderPath = "Investigacion/Notas_Conceptuales";

const content = `---
tipo: nota_literatura
origen: "${activeFile.path}"
autor: "${author || "Autor"}"
anio: "${nowYear}"
latex_citekey: "${safeAuthor}${nowYear}${safeIdea}"
estado: borrador

---

#investigacion #literatura

# ${idea}

## Contexto de Extraccion

> ${selection || "(Sin seleccion detectada)"}

## Analisis e Integracion para Tesis

- Aporte tecnico:
- Relacion con Unity/WebGL:
- Riesgos o limites:
  `;

await tp.file.create_new(content, fileName, false, app.vault.getAbstractFileByPath(folderPath));
new Notice(`Nota creada: ${fileName}`);
%>
