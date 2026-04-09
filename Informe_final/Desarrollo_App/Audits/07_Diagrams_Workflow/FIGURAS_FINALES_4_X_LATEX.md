# Figuras Finales Numeradas 4.1-4.13 (Listas para LaTeX)

Nota:

- Ajusta rutas de imagen segun donde exportes cada Mermaid; en esta entrega ya quedaron como PDFs vectoriales.
- Convencion sugerida de nombres de archivo: fig_4_1.pdf, fig_4_2.pdf, ...

## Mapeo oficial de figuras

- 4.1 Arquitectura por capas del sistema (C4 simplificado)
- 4.2 Flujo de comunicacion EventBus (Publish/Subscribe)
- 4.3 Maquina de estados de la aplicacion (AppStateMachine)
- 4.4 Flujo de seleccion de piezas (Input -> Raycast -> Evento -> UI)
- 4.5 Pipeline de shaders por modo de visualizacion
- 4.6 Flujo de datos DronePartData
- 4.7 Subsistema termico hibrido (V1)
- 4.8 Restricciones WebGL y mitigaciones
- 4.9 Herramientas de ensamblaje proyectadas (estado por modulo)
- 4.10 Despliegue y artefactos (Build a Hosting)
- 4.11 Plantilla de resultados SUS (estructura de analisis)
- 4.12 Plantilla de resultados NASA-TLX (estructura de analisis)
- 4.13 Plantilla de KPIs tecnicos (estructura de medicion)

Ruta editorial final sugerida:

- figures/chapter4/fig_4_1.pdf
- figures/chapter4/fig_4_2.pdf
- figures/chapter4/fig_4_3.pdf
- figures/chapter4/fig_4_4.pdf
- figures/chapter4/fig_4_5.pdf
- figures/chapter4/fig_4_6.pdf
- figures/chapter4/fig_4_7.pdf
- figures/chapter4/fig_4_8.pdf
- figures/chapter4/fig_4_9.pdf
- figures/chapter4/fig_4_10.pdf
- figures/chapter4/fig_4_11.pdf
- figures/chapter4/fig_4_12.pdf
- figures/chapter4/fig_4_13.pdf

## Snippets LaTeX listos para pegar

```tex
% Figura 4.1
\begin{figure}[H]
    \centering
    \includegraphics[width=\textwidth]{figures/chapter4/fig_4_1.pdf}
    \caption{Arquitectura por capas del sistema (C4 simplificado).}
    \label{fig:4_1_arquitectura_capas}
\end{figure}

% Figura 4.2
\begin{figure}[H]
    \centering
    \includegraphics[width=\textwidth]{figures/chapter4/fig_4_2.pdf}
    \caption{Flujo de comunicacion EventBus (Publish/Subscribe).}
    \label{fig:4_2_eventbus}
\end{figure}

% Figura 4.3
\begin{figure}[H]
    \centering
    \includegraphics[width=\textwidth]{figures/chapter4/fig_4_3.pdf}
    \caption{Maquina de estados de la aplicacion (AppStateMachine).}
    \label{fig:4_3_app_state_machine}
\end{figure}

% Figura 4.4
\begin{figure}[H]
    \centering
    \includegraphics[width=\textwidth]{figures/chapter4/fig_4_4.pdf}
    \caption{Flujo de seleccion de piezas: Input, Raycast, evento y sincronizacion de UI.}
    \label{fig:4_4_flujo_seleccion}
\end{figure}

% Figura 4.5
\begin{figure}[H]
    \centering
    \includegraphics[width=\textwidth]{figures/chapter4/fig_4_5.pdf}
    \caption{Pipeline de shaders por modo de visualizacion.}
    \label{fig:4_5_pipeline_shaders}
\end{figure}

% Figura 4.6
\begin{figure}[H]
    \centering
    \includegraphics[width=\textwidth]{figures/chapter4/fig_4_6.pdf}
    \caption{Flujo de datos DronePartData desde asset hasta interfaz.}
    \label{fig:4_6_flujo_dronepartdata}
\end{figure}

% Figura 4.7
\begin{figure}[H]
    \centering
    \includegraphics[width=\textwidth]{figures/chapter4/fig_4_7.pdf}
    \caption{Arquitectura del subsistema termico hibrido (V1).}
    \label{fig:4_7_subsistema_termico}
\end{figure}

% Figura 4.8
\begin{figure}[H]
    \centering
    \includegraphics[width=\textwidth]{figures/chapter4/fig_4_8.pdf}
    \caption{Restricciones WebGL y mitigaciones tecnicas aplicadas.}
    \label{fig:4_8_webgl_mitigaciones}
\end{figure}

% Figura 4.9
\begin{figure}[H]
    \centering
    \includegraphics[width=\textwidth]{figures/chapter4/fig_4_9.pdf}
    \caption{Estado de herramientas de ensamblaje proyectadas por modulo.}
    \label{fig:4_9_ensamblaje_estado}
\end{figure}

% Figura 4.10
\begin{figure}[H]
    \centering
    \includegraphics[width=\textwidth]{figures/chapter4/fig_4_10.pdf}
    \caption{Flujo de despliegue y artefactos desde build hasta hosting y QA.}
    \label{fig:4_10_despliegue_artefactos}
\end{figure}

% Figura 4.11
\begin{figure}[H]
    \centering
    \includegraphics[width=\textwidth]{figures/chapter4/fig_4_11.pdf}
    \caption{Plantilla metodologica para resultados SUS.}
    \label{fig:4_11_sus_template}
\end{figure}

% Figura 4.12
\begin{figure}[H]
    \centering
    \includegraphics[width=\textwidth]{figures/chapter4/fig_4_12.pdf}
    \caption{Plantilla metodologica para resultados NASA-TLX.}
    \label{fig:4_12_nasatlx_template}
\end{figure}

% Figura 4.13
\begin{figure}[H]
    \centering
    \includegraphics[width=\textwidth]{figures/chapter4/fig_4_13.pdf}
    \caption{Plantilla de medicion de KPIs tecnicos y brecha contra objetivos.}
    \label{fig:4_13_kpi_template}
\end{figure}
```
