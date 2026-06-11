using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace WebGL.UI.Panels
{
    public static class AppLanguageManager
    {
        private const string PlayerPrefsKey = "x500v2_language";
        private static readonly Dictionary<string, string> EnToEs = new Dictionary<string, string>
        {
            { "SELECT A PART", "SELECCIONA UNA PIEZA" },
            { "PART DETAILS", "DETALLE DE PIEZA" },
            { "PART INFO", "INFO DE PIEZA" },
            { "EXPLORE", "EXPLORAR" },
            { "DEVICE", "DISPOSITIVO" },
            { "ABOUT", "ACERCA DE" },
            { "EXIT", "SALIR" },
            { "Device", "Dispositivo" },
            { "About", "Acerca de" },
            { "Exit app", "Salir de la app" },
            { "Holybro X500 Base", "Holybro X500 Base" },
            { "Additional Device Slot (Coming Soon)", "Ranura adicional de dispositivo (pr\u00f3ximamente)" },
            { "TwinSight X500 is an interactive 3D technical viewer for the Holybro X500 V2 thesis prototype. Built with Unity WebGL.", "TwinSight X500 es un visor t\u00e9cnico 3D interactivo para el prototipo de tesis Holybro X500 V2. Construido con Unity WebGL." },
            { "Thesis Project \u2014 2026", "Proyecto de tesis \u2014 2026" },
            { "VIEW TUTORIAL", "VER TUTORIAL" },
            { "PERF CAPTURE", "CAPTURA PERF." },
            { "VIEW SOURCE REPOSITORY", "VER REPOSITORIO FUENTE" },
            { "Are you sure you want to stop the experience?", "\u00bfSeguro que quieres detener la experiencia?" },
            { "EXIT APP", "SALIR DE LA APP" },
            { "CANCEL", "CANCELAR" },
            { "INSPECT", "INSPECCIONAR" },
            { "ANALYZE", "ANALIZAR" },
            { "STUDIO", "ESTUDIO" },
            { "CUT", "CORTE" },
            { "EXPLODE", "EXPLOTAR" },
            { "FILTER", "FILTRAR" },
            { "PINS", "PUNTOS" },
            { "ISOLATE", "AISLAR" },
            { "POWER", "ENERG\u00cdA" },
            { "OFF", "APAGADO" },
            { "STARTING", "INICIANDO" },
            { "IDLE", "REPOSO" },
            { "FLYING", "VUELO" },
            { "SHUTDOWN", "APAGANDO" },
            { "CROSS SECTION", "CORTE TRANSVERSAL" },
            { "Cross Section", "Corte transversal" },
            { "PLANE 1 OFFSET", "DESPLAZAMIENTO PLANO 1" },
            { "PLANE 2 OFFSET", "DESPLAZAMIENTO PLANO 2" },
            { "FILTER CATEGORY", "FILTRAR CATEGORIA" },
            { "AIRFRAME", "ESTRUCTURA" },
            { "PROPULSION", "PROPULSI\u00d3N" },
            { "AVIONICS", "AVI\u00d3NICA" },
            { "SENSORS", "SENSORES" },
            { "FASTENERS", "FIJACIONES" },
            { "RENDER MODE", "MODO DE RENDER" },
            { "REALISTIC", "REALISTA" },
            { "X-RAY", "RAYOS X" },
            { "BLUEPRINT", "PLANO" },
            { "SOLID", "S\u00d3LIDO" },
            { "Solid", "S\u00f3lido" },
            { "WIRE", "MALLA" },
            { "GHOST", "FANTASMA" },
            { "THERMAL", "TERMICO" },
            { "ENVIRONMENT", "ENTORNO" },
            { "TIME", "TIEMPO" },
            { "DAY", "D\u00cdA" },
            { "Day", "D\u00eda" },
            { "NIGHT", "NOCHE" },
            { "Night", "Noche" },
            { "SUNSET", "ATARDECER" },
            { "Sunset", "Atardecer" },
            { "COLOR", "COLOR" },
            { "WHITE", "BLANCO" },
            { "White", "Blanco" },
            { "GREY", "GRIS" },
            { "Grey", "Gris" },
            { "GRAY", "GRIS" },
            { "BLACK", "NEGRO" },
            { "Black", "Negro" },
            { "YELLOW", "AMARILLO" },
            { "Yellow", "Amarillo" },
            { "ORANGE", "NARANJA" },
            { "Orange", "Naranja" },
            { "GREEN", "VERDE" },
            { "Green", "Verde" },
            { "BLUE", "AZUL" },
            { "Blue", "Azul" },
            { "PURPLE", "MORADO" },
            { "Purple", "Morado" },
            { "RED", "ROJO" },
            { "Red", "Rojo" },
            { "NEUTRAL", "NEUTRO" },
            { "STUDIO LIGHT", "LUZ DE ESTUDIO" },
            { "Studio Light", "Luz de estudio" },
            { "LIGHT ROTATION", "ROTACI\u00d3N DE LUZ" },
            { "Light Rotation", "Rotaci\u00f3n de luz" },
            { "OBJECT INTENSITY", "INTENSIDAD DEL OBJETO" },
            { "Object Intensity", "Intensidad del objeto" },
            { "BACKGROUND TONE", "TONO DEL FONDO" },
            { "Background Tone", "Tono del fondo" },
            { "Render Mode", "Modo de render" },
            { "Toggle Hotspots", "Mostrar/ocultar puntos" },
            { "Isolate Selected Part", "Aislar pieza seleccionada" },
            { "Toggle Drone Power", "Encender/apagar dron" },
            { "Exploded View", "Vista explosionada" },
            { "Filter Categories", "Filtrar categorias" },
            { "Toggle Render Mode", "Cambiar modo de render" },
            { "View", "Vista" },
            { "Propulsion System", "Sistema de propulsi\u00f3n" },
            { "Power Distribution", "Distribuci\u00f3n de energ\u00eda" },
            { "Flight Controller", "Control de vuelo" },
            { "GPS & Compass", "GPS y br\u00fajula" },
            { "Battery", "Bater\u00eda" },
            { "IDENTIFICATION", "IDENTIFICACION" },
            { "SPECIFICATIONS", "ESPECIFICACIONES" },
            { "ASSEMBLY", "ENSAMBLAJE" },
            { "KEY & REFERENCES", "CLAVE Y REFERENCIAS" },
            { "CONTAINED PARTS", "PIEZAS CONTENIDAS" },
            { "PARENT ASSEMBLY", "PIEZA MADRE" }
        };

        private static readonly Dictionary<string, string> EsToEn = BuildReverseMap();
        private static string currentLanguageCode = LoadInitialLanguage();

        public static event Action<string> LanguageChanged;

        public static string CurrentLanguageCode => currentLanguageCode;
        public static bool IsSpanish => string.Equals(currentLanguageCode, "es", StringComparison.OrdinalIgnoreCase);
        public static string ToggleButtonText => IsSpanish ? "EN" : "ES";

        public static void ToggleLanguage()
        {
            SetLanguage(IsSpanish ? "en" : "es");
        }

        public static void SetLanguage(string languageCode)
        {
            string normalized = NormalizeLanguage(languageCode);
            if (string.Equals(currentLanguageCode, normalized, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            currentLanguageCode = normalized;
            PlayerPrefs.SetString(PlayerPrefsKey, currentLanguageCode);
            PlayerPrefs.Save();
            LanguageChanged?.Invoke(currentLanguageCode);
        }

        public static string TranslateStatic(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

            string trimmed = text.Trim();
            if (IsSpanish)
            {
                return EnToEs.TryGetValue(trimmed, out string spanish) ? spanish : text;
            }

            return EsToEn.TryGetValue(trimmed, out string english) ? english : text;
        }

        public static void ApplyStaticText(VisualElement root)
        {
            if (root == null)
            {
                return;
            }

            root.Query<Label>().ForEach(label =>
            {
                if (label != null)
                {
                    label.text = TranslateStatic(label.text);
                }
            });

            root.Query<Button>().ForEach(button =>
            {
                if (button == null)
                {
                    return;
                }

                if (!string.IsNullOrWhiteSpace(button.tooltip))
                {
                    button.tooltip = TranslateStatic(button.tooltip);
                }

                if (string.Equals(button.name, "HeroLanguageBtn", StringComparison.Ordinal))
                {
                    return;
                }

                button.text = TranslateStatic(button.text);
            });

            root.Query<Foldout>().ForEach(foldout =>
            {
                if (foldout != null)
                {
                    foldout.text = TranslateStatic(foldout.text);
                }
            });
        }

        public static string SelectPartPrompt()
        {
            return IsSpanish ? "SELECCIONA UNA PIEZA" : "SELECT A PART";
        }

        public static string PartDetailsTitle()
        {
            return IsSpanish ? "DETALLE DE PIEZA" : "PART DETAILS";
        }

        private static string LoadInitialLanguage()
        {
            return NormalizeLanguage(PlayerPrefs.GetString(PlayerPrefsKey, "en"));
        }

        private static string NormalizeLanguage(string languageCode)
        {
            return string.Equals(languageCode, "es", StringComparison.OrdinalIgnoreCase) ? "es" : "en";
        }

        private static Dictionary<string, string> BuildReverseMap()
        {
            var reverse = new Dictionary<string, string>();
            foreach (var pair in EnToEs)
            {
                if (!reverse.ContainsKey(pair.Value))
                {
                    reverse.Add(pair.Value, pair.Key);
                }
            }

            return reverse;
        }
    }
}
