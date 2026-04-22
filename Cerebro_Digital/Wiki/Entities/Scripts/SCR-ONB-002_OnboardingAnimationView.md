---
tipo: script_card
area: unity
estado: activo
trace_id: SCR-ONB-002
script_path: "desarrollo/unity_project/Assets/Scripts/UI/Panels/OnboardingAnimationView.cs"
entregable_ids: ["INF-C04", "MAN-USR-ONBOARDING", "UI-ONB-ANIM"]
resumen: "Renderiza previews animados del onboarding con Painter2D para mostrar gesto, objetivo y respuesta del sistema."
---

# Script Card: OnboardingAnimationView

## Script fuente

- [OnboardingAnimationView](desarrollo/unity_project/Assets/Scripts/UI/Panels/OnboardingAnimationView.cs)

## Rol

- Genera vistas previas animadas para los pasos de onboarding sin cargar escenas pesadas.
- Comunica de forma visual la accion del usuario, el objetivo UI/modelo y la respuesta esperada.
- Mantiene sincronizado el estado de fase para la narrativa guiada del primer uso.

## Conexiones

- [[SCR-ONB-001_OnboardingController]]
- [MainLayout.uxml](desarrollo/unity_project/Assets/Scripts/UI/Layouts/MainLayout.uxml)
- [Theme.uss](desarrollo/unity_project/Assets/UI/Styles/Theme.uss)
- [manual_usuario](docs/manuals/manual_usuario.md)
- [04_desarrollo](Informe_final/chapters/04_desarrollo.tex)

## Enlaces de continuidad

- [[CATALOGO_SCRIPTS_UNITY]]
- [[CATALOGO_SCRIPTS_UNITY_COMPLETO]]
- [[MOC_Documentacion_Tecnica]]
- [[MOC_Conectividad_Total]]