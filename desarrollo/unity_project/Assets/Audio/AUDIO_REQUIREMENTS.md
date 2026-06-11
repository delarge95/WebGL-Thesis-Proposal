# Audio Assets Requirements
## TwinSight X500 - Sound Design Specification

---

## Overview

This document specifies the audio assets needed for the WebGL drone viewer prototype. All audio must be:
- Royalty-free or CC0 licensed
- Optimized for web (small file size)
- WebGL compatible (MP3 or OGG format)

---

## Audio Categories

### 1. UI Sounds (Required)

| Sound | Description | Duration | Priority |
|-------|-------------|----------|----------|
| `ui_click.mp3` | Button click, general action | 0.1-0.2s | High |
| `ui_hover.mp3` | Subtle hover feedback | 0.05-0.1s | Medium |
| `ui_select.mp3` | Part selection confirmation | 0.2-0.3s | High |
| `ui_deselect.mp3` | Deselection, close panel | 0.1-0.2s | Medium |
| `ui_toggle.mp3` | Toggle switch sound | 0.1-0.2s | Medium |
| `ui_error.mp3` | Error/invalid action | 0.3-0.5s | Low |
| `ui_success.mp3` | Success/confirmation | 0.3-0.5s | Low |

### 2. Transition Sounds (Required)

| Sound | Description | Duration | Priority |
|-------|-------------|----------|----------|
| `transition_explode.mp3` | Parts separating (mechanical) | 0.5-1.0s | High |
| `transition_collapse.mp3` | Parts reassembling | 0.5-1.0s | High |
| `transition_viewmode.mp3` | View mode change (subtle whoosh) | 0.3-0.5s | Medium |
| `transition_slide.mp3` | Panel slide in/out | 0.2-0.4s | Low |

### 3. Drone Simulation (Optional but Recommended)

| Sound | Description | Duration | Priority |
|-------|-------------|----------|----------|
| `drone_startup.mp3` | Motors powering up | 1.5-2.0s | Medium |
| `drone_idle.mp3` | Motors running (loopable) | 3.0-5.0s | Medium |
| `drone_shutdown.mp3` | Motors powering down | 1.0-1.5s | Medium |
| `propeller_spin.mp3` | Single propeller (loopable) | 1.0s | Low |

### 4. Ambient (Optional)

| Sound | Description | Duration | Priority |
|-------|-------------|----------|----------|
| `ambient_tech.mp3` | Subtle electronic ambience | 30-60s loop | Low |

---

## Technical Specifications

### Format Requirements

```
Primary Format: MP3
├── Bitrate: 128 kbps (stereo) or 64 kbps (mono)
├── Sample Rate: 44.1 kHz
└── Channels: Mono (UI), Stereo (ambient/drone)

Alternative Format: OGG (better quality, slightly larger)
├── Quality: 5-7
└── Sample Rate: 44.1 kHz
```

### File Size Limits

| Category | Max Size per File | Total Category |
|----------|-------------------|----------------|
| UI Sounds | 50 KB | 200 KB |
| Transitions | 100 KB | 300 KB |
| Drone Simulation | 200 KB | 500 KB |
| Ambient | 500 KB | 500 KB |
| **Total Audio Budget** | - | **1.5 MB** |

---

## Sound Style Guide

### UI Sounds
- **Style**: Clean, modern, subtle
- **Tone**: Neutral to slightly positive
- **Characteristics**: 
  - No musical elements
  - Soft attack, quick decay
  - High-frequency focused (not bassy)
- **Reference**: iOS/macOS system sounds

### Transition Sounds
- **Style**: Mechanical, technical
- **Tone**: Professional, industrial
- **Characteristics**:
  - Suggest physical movement
  - Slight metallic quality
  - Appropriate to 3D visualization context

### Drone Sounds
- **Style**: Realistic FPV drone
- **Tone**: Technical, authentic
- **Characteristics**:
  - Electric motor whine
  - Multiple frequency layers
  - Loopable for sustained playback

---

## Free Audio Sources

### Recommended Sites (All Free, Commercial Use OK)

| Source | URL | License |
|--------|-----|---------|
| **Freesound.org** | freesound.org | CC0/CC-BY |
| **Mixkit** | mixkit.co/free-sound-effects | Free |
| **Pixabay** | pixabay.com/sound-effects | Pixabay License |
| **Zapsplat** | zapsplat.com | Free with attribution |
| **SoundBible** | soundbible.com | Various |

### Specific Recommendation: freesound.org

Search queries for each sound:

```
ui_click:       "button click interface" OR "UI click"
ui_hover:       "hover subtle" OR "mouse hover"
ui_select:      "select confirm" OR "pick up item"
ui_toggle:      "switch toggle" OR "checkbox"
transition:     "mechanical slide" OR "servo motor"
drone:          "drone motor" OR "quadcopter" OR "FPV motors"
ambient:        "electronic ambient minimal"
```

---

## Implementation in Unity

### AudioManager Configuration

```csharp
// Audio clips to assign in Inspector
[Header("UI Sounds")]
public AudioClip clickSound;
public AudioClip hoverSound;
public AudioClip selectSound;
public AudioClip deselectSound;

[Header("Transitions")]
public AudioClip explodeSound;
public AudioClip collapseSound;
public AudioClip viewModeSound;

[Header("Drone")]
public AudioClip droneStartup;
public AudioClip droneIdle;  // Set to loop
public AudioClip droneShutdown;

[Header("Settings")]
[Range(0, 1)] public float masterVolume = 0.7f;
[Range(0, 1)] public float uiVolume = 0.5f;
[Range(0, 1)] public float sfxVolume = 0.8f;
```

### Audio Import Settings (Unity)

```
UI Sounds:
├── Load Type: Decompress On Load
├── Compression Format: ADPCM
├── Quality: 70%
└── Sample Rate: Preserve Sample Rate

Transitions/Drone:
├── Load Type: Compressed In Memory
├── Compression Format: Vorbis
├── Quality: 70%
└── Sample Rate: Preserve Sample Rate

Ambient (if used):
├── Load Type: Streaming
├── Compression Format: Vorbis
├── Quality: 50%
└── Sample Rate: Preserve Sample Rate
```

---

## File Naming Convention

```
audio/
├── ui/
│   ├── ui_click.mp3
│   ├── ui_hover.mp3
│   ├── ui_select.mp3
│   ├── ui_deselect.mp3
│   ├── ui_toggle.mp3
│   ├── ui_error.mp3
│   └── ui_success.mp3
├── transitions/
│   ├── transition_explode.mp3
│   ├── transition_collapse.mp3
│   ├── transition_viewmode.mp3
│   └── transition_slide.mp3
├── drone/
│   ├── drone_startup.mp3
│   ├── drone_idle.mp3
│   └── drone_shutdown.mp3
└── ambient/
    └── ambient_tech.mp3
```

---

## Quick Download Links (Examples from Freesound)

> Note: Verify licenses before use. These are examples only.

### UI Clicks (Search: "button click ui")
- https://freesound.org/search/?q=button+click+ui

### Mechanical Transitions (Search: "servo mechanical")
- https://freesound.org/search/?q=servo+mechanical

### Drone Motors (Search: "drone motor fpv")
- https://freesound.org/search/?q=drone+motor+fpv

---

## Audio Off/Mute

Always include:
- Master volume slider (0-100%)
- Mute toggle button
- Remember user preference (PlayerPrefs)

```csharp
public void SetMasterVolume(float volume)
{
    masterVolume = volume;
    AudioListener.volume = volume;
    PlayerPrefs.SetFloat("MasterVolume", volume);
}
```

---

*Audio Specification Version: 1.0*
*Total Budget: ~1.5 MB*
*Priority Files: 7 UI + 2 Transition = 9 essential*
