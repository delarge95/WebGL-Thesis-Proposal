# Assets de Audio — WebGL Drone Viewer

> **Versión:** 0.1 (Placeholder)  
> **Estado:** 🔲 PENDIENTE — Los archivos de audio aún no han sido creados/adquiridos  
> **Última actualización:** Julio 2025  
> **Referencia:** `Assets/Audio/AUDIO_REQUIREMENTS.md`, `AudioManager.cs`

---

## 1. Resumen

El sistema de audio del visor se gestiona mediante `AudioManager.cs` (PersistentSingleton) con dos canales: efectos de sonido (SFX) y música ambiente. Los volúmenes son configurables por el usuario y se persisten entre sesiones.

**Presupuesto total de audio:** < 1.5 MB

---

## 2. Inventario de Audio Requerido

### 2.1 Sonidos de Interfaz (UI Sounds)

| #   | Clip              | Variable en AudioManager | Duración | Formato    | Tamaño Máx. | Estado       |
| --- | ----------------- | ------------------------ | -------- | ---------- | ----------- | ------------ |
| 1   | Hover sobre botón | `uiHoverClip`            | ~50 ms   | OGG Vorbis | 15 KB       | 🔲 Pendiente |
| 2   | Click de botón    | `uiClickClip`            | ~100 ms  | OGG Vorbis | 20 KB       | 🔲 Pendiente |
| 3   | Feedback de éxito | `successClip`            | ~300 ms  | OGG Vorbis | 30 KB       | 🔲 Pendiente |
| 4   | Feedback de error | `errorClip`              | ~200 ms  | OGG Vorbis | 25 KB       | 🔲 Pendiente |
| 5   | Toggle ON/OFF     | —                        | ~80 ms   | OGG Vorbis | 15 KB       | 🔲 Pendiente |
| 6   | Panel abre/cierra | —                        | ~150 ms  | OGG Vorbis | 20 KB       | 🔲 Pendiente |
| 7   | Tooltip aparece   | —                        | ~50 ms   | OGG Vorbis | 10 KB       | 🔲 Pendiente |

### 2.2 Transiciones y Animaciones

| #   | Clip                   | Variable en AudioManager | Duración | Formato    | Tamaño Máx. | Estado       |
| --- | ---------------------- | ------------------------ | -------- | ---------- | ----------- | ------------ |
| 8   | Explosionado (expand)  | `explosionClip`          | ~500 ms  | OGG Vorbis | 50 KB       | 🔲 Pendiente |
| 9   | Transición de modo     | `transitionClip`         | ~300 ms  | OGG Vorbis | 40 KB       | 🔲 Pendiente |
| 10  | Whoosh (cámara rápida) | —                        | ~200 ms  | OGG Vorbis | 30 KB       | 🔲 Pendiente |
| 11  | Snap / selección pieza | —                        | ~100 ms  | OGG Vorbis | 20 KB       | 🔲 Pendiente |

### 2.3 Simulación del Dron

| #   | Clip                      | Duración     | Formato    | Tamaño Máx. | Estado       |
| --- | ------------------------- | ------------ | ---------- | ----------- | ------------ |
| 12  | Motor idle (loop)         | 2-3 s (loop) | OGG Vorbis | 150 KB      | 🔲 Pendiente |
| 13  | Motor aceleración         | ~1 s         | OGG Vorbis | 80 KB       | 🔲 Pendiente |
| 14  | Motor apagado             | ~500 ms      | OGG Vorbis | 50 KB       | 🔲 Pendiente |
| 15  | Pitido electrónico (boot) | ~300 ms      | OGG Vorbis | 30 KB       | 🔲 Pendiente |

### 2.4 Ambiente

| #   | Clip                   | Variable en AudioManager | Duración       | Formato    | Tamaño Máx. | Estado       |
| --- | ---------------------- | ------------------------ | -------------- | ---------- | ----------- | ------------ |
| 16  | Música ambiente (loop) | `musicSource`            | 30-60 s (loop) | OGG Vorbis | 500 KB      | 🔲 Pendiente |

---

## 3. Especificaciones Técnicas

### 3.1 Formato de Exportación

| Parámetro   | Valor                                 |
| ----------- | ------------------------------------- |
| Formato     | OGG Vorbis (preferido) o MP3 128 kbps |
| Sample Rate | 44.1 kHz                              |
| Channels    | Mono (UI/SFX), Stereo (ambiente)      |
| Bit Depth   | 16-bit                                |

### 3.2 Import Settings en Unity

| Tipo            | Force Mono | Load Type            | Compression | Quality |
| --------------- | ---------- | -------------------- | ----------- | ------- |
| UI Sounds       | ✅         | Decompress On Load   | Vorbis      | 70%     |
| Transiciones    | ✅         | Decompress On Load   | Vorbis      | 70%     |
| Simulación Dron | ✅         | Compressed In Memory | Vorbis      | 60%     |
| Ambiente        | ❌         | Compressed In Memory | Vorbis      | 50%     |

### 3.3 Presupuesto por Categoría

| Categoría       | Clips  | Presupuesto              |
| --------------- | ------ | ------------------------ |
| UI Sounds       | 7      | ~135 KB                  |
| Transiciones    | 4      | ~140 KB                  |
| Simulación Dron | 4      | ~310 KB                  |
| Ambiente        | 1      | ~500 KB                  |
| **Total**       | **16** | **~1,085 KB (< 1.5 MB)** |

---

## 4. Estructura de Archivos

```
Assets/Audio/
├── AUDIO_REQUIREMENTS.md    # ✅ Existe — Especificaciones detalladas
├── SFX/
│   ├── UI/
│   │   ├── hover.ogg        # 🔲 Pendiente
│   │   ├── click.ogg        # 🔲 Pendiente
│   │   ├── success.ogg      # 🔲 Pendiente
│   │   ├── error.ogg        # 🔲 Pendiente
│   │   ├── toggle.ogg       # 🔲 Pendiente
│   │   ├── panel.ogg        # 🔲 Pendiente
│   │   └── tooltip.ogg      # 🔲 Pendiente
│   ├── Transitions/
│   │   ├── explode.ogg      # 🔲 Pendiente
│   │   ├── transition.ogg   # 🔲 Pendiente
│   │   ├── whoosh.ogg       # 🔲 Pendiente
│   │   └── snap.ogg         # 🔲 Pendiente
│   └── Drone/
│       ├── motor_idle.ogg   # 🔲 Pendiente
│       ├── motor_accel.ogg  # 🔲 Pendiente
│       ├── motor_off.ogg    # 🔲 Pendiente
│       └── boot_beep.ogg    # 🔲 Pendiente
└── Music/
    └── ambient_loop.ogg     # 🔲 Pendiente
```

---

## 5. Fuentes Potenciales

<!-- TODO: Definir fuentes de los assets de audio -->

| Opción                            | Licencia      | Notas                              |
| --------------------------------- | ------------- | ---------------------------------- |
| Freesound.org                     | CC0 / CC BY   | Amplia biblioteca de SFX gratuitos |
| Pixabay Audio                     | Royalty-free  | Sin atribución requerida           |
| Generación propia (Audacity/LMMS) | Original      | Control total sobre el sonido      |
| Unity Asset Store                 | Varía         | Packs de SFX WebGL-optimized       |
| AI-generated (Stable Audio, Suno) | Verificar ToS | Rápido para prototipos             |

---

## 6. Integración con AudioManager

```csharp
// AudioManager.cs — Clips a asignar en el Inspector
[Header("UI Sounds")]
[SerializeField] private AudioClip uiHoverClip;
[SerializeField] private AudioClip uiClickClip;
[SerializeField] private AudioClip successClip;
[SerializeField] private AudioClip errorClip;

[Header("Transitions")]
[SerializeField] private AudioClip explosionClip;
[SerializeField] private AudioClip transitionClip;

// Uso:
AudioManager.Instance.PlayClick();
AudioManager.Instance.PlayHover();
AudioManager.Instance.PlayExplosionSound();
AudioManager.Instance.PlayTransition();
AudioManager.Instance.PlayError();
AudioManager.Instance.PlaySuccess();
AudioManager.Instance.PlayMusic();
```

---

## 7. Notas

- ⚠️ **WebGL requiere interacción del usuario** para iniciar la reproducción de audio (política de autoplay de navegadores).
- ⚠️ **No hay streaming** en WebGL — todos los clips se cargan completamente en memoria.
- Los clips de UI deben ser muy cortos (< 300 ms) para no interferir con la experiencia.
- La música ambiente debe tener un loop limpio (sin corte audible al reiniciar).
- El `AudioManager` ya maneja volúmenes Master/SFX/Music con persistencia via `PlayerPrefs`.
