# Assets de Audio — TwinSight X500

> **Versión:** 0.1 (línea futura no integrada)
> **Estado:** Alcance no integrado en la build evaluada
> **Última actualización:** Junio 2026
> **Referencia:** `Assets/Audio/AUDIO_REQUIREMENTS.md`, `AudioManager.cs`

---

## 1. Resumen

El sistema de audio se conserva como línea de trabajo futura. No forma parte del flujo visible ni de la build defendible del cierre actual; por tanto, no debe presentarse en la demo pública ni como capacidad final implementada. La arquitectura prevista se gestiona mediante `AudioManager.cs` (PersistentSingleton) con dos canales: efectos de sonido (SFX) y música ambiente. Los volúmenes serían configurables por el usuario y se persistirían entre sesiones.

**Presupuesto total de audio:** < 1.5 MB

---

## 2. Inventario de Audio Requerido

### 2.1 Sonidos de Interfaz (UI Sounds)

| #   | Clip              | Variable en AudioManager | Duración | Formato    | Tamaño Máx. | Estado       |
| --- | ----------------- | ------------------------ | -------- | ---------- | ----------- | ------------ |
| 1   | Hover sobre botón | `uiHoverClip`            | ~50 ms   | OGG Vorbis | 15 KB       | No integrado |
| 2   | Click de botón    | `uiClickClip`            | ~100 ms  | OGG Vorbis | 20 KB       | No integrado |
| 3   | Feedback de éxito | `successClip`            | ~300 ms  | OGG Vorbis | 30 KB       | No integrado |
| 4   | Feedback de error | `errorClip`              | ~200 ms  | OGG Vorbis | 25 KB       | No integrado |
| 5   | Toggle ON/OFF     | —                        | ~80 ms   | OGG Vorbis | 15 KB       | No integrado |
| 6   | Panel abre/cierra | —                        | ~150 ms  | OGG Vorbis | 20 KB       | No integrado |
| 7   | Tooltip aparece   | —                        | ~50 ms   | OGG Vorbis | 10 KB       | No integrado |

### 2.2 Transiciones y Animaciones

| #   | Clip                   | Variable en AudioManager | Duración | Formato    | Tamaño Máx. | Estado       |
| --- | ---------------------- | ------------------------ | -------- | ---------- | ----------- | ------------ |
| 8   | Explosionado (expand)  | `explosionClip`          | ~500 ms  | OGG Vorbis | 50 KB       | No integrado |
| 9   | Transición de modo     | `transitionClip`         | ~300 ms  | OGG Vorbis | 40 KB       | No integrado |
| 10  | Whoosh (cámara rápida) | —                        | ~200 ms  | OGG Vorbis | 30 KB       | No integrado |
| 11  | Snap / selección pieza | —                        | ~100 ms  | OGG Vorbis | 20 KB       | No integrado |

### 2.3 Simulación del Dron

| #   | Clip                      | Duración     | Formato    | Tamaño Máx. | Estado       |
| --- | ------------------------- | ------------ | ---------- | ----------- | ------------ |
| 12  | Motor idle (loop)         | 2-3 s (loop) | OGG Vorbis | 150 KB      | No integrado |
| 13  | Motor aceleración         | ~1 s         | OGG Vorbis | 80 KB       | No integrado |
| 14  | Motor apagado             | ~500 ms      | OGG Vorbis | 50 KB       | No integrado |
| 15  | Pitido electrónico (boot) | ~300 ms      | OGG Vorbis | 30 KB       | No integrado |

### 2.4 Ambiente

| #   | Clip                   | Variable en AudioManager | Duración       | Formato    | Tamaño Máx. | Estado       |
| --- | ---------------------- | ------------------------ | -------------- | ---------- | ----------- | ------------ |
| 16  | Música ambiente (loop) | `musicSource`            | 30-60 s (loop) | OGG Vorbis | 500 KB      | No integrado |

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
│   │   ├── hover.ogg        # No integrado
│   │   ├── click.ogg        # No integrado
│   │   ├── success.ogg      # No integrado
│   │   ├── error.ogg        # No integrado
│   │   ├── toggle.ogg       # No integrado
│   │   ├── panel.ogg        # No integrado
│   │   └── tooltip.ogg      # No integrado
│   ├── Transitions/
│   │   ├── explode.ogg      # No integrado
│   │   ├── transition.ogg   # No integrado
│   │   ├── whoosh.ogg       # No integrado
│   │   └── snap.ogg         # No integrado
│   └── Drone/
│       ├── motor_idle.ogg   # No integrado
│       ├── motor_accel.ogg  # No integrado
│       ├── motor_off.ogg    # No integrado
│       └── boot_beep.ogg    # No integrado
└── Music/
    └── ambient_loop.ogg     # No integrado
```

---

## 5. Fuentes Potenciales

Las fuentes de audio deberán seleccionarse en una fase posterior con licencia compatible con distribución web y documentación académica.

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

