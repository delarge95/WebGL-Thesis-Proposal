using UnityEngine;
using WebGL.Core.Utils;
using WebGL.Core.Events;

namespace WebGL.Core.Managers
{
    public class AudioManager : PersistentSingleton<AudioManager>
    {
        [Header("Sources")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource musicSource;

        [Header("Clips")]
        [SerializeField] private AudioClip uiHoverClip;
        [SerializeField] private AudioClip uiClickClip;
        [SerializeField] private AudioClip explosionClip;
        [SerializeField] private AudioClip transitionClip;
        [SerializeField] private AudioClip errorClip;
        [SerializeField] private AudioClip successClip;

        [Header("Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float masterVolume = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float sfxVolume = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float musicVolume = 0.5f;

        public float MasterVolume => masterVolume;
        public float SFXVolume => sfxVolume;
        public float MusicVolume => musicVolume;

        protected override void Awake()
        {
            base.Awake();
            if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
            if (musicSource == null) 
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
            }

            LoadVolumeSettings();
        }

        private void LoadVolumeSettings()
        {
            masterVolume = SaveSystem.LoadVolume();
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            ApplyVolumes();
        }

        private void ApplyVolumes()
        {
            sfxSource.volume = sfxVolume * masterVolume;
            musicSource.volume = musicVolume * masterVolume;
        }

        public void SetMasterVolume(float value)
        {
            masterVolume = Mathf.Clamp01(value);
            SaveSystem.SaveVolume(masterVolume);
            ApplyVolumes();
        }

        public void SetSFXVolume(float value)
        {
            sfxVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            ApplyVolumes();
        }

        public void SetMusicVolume(float value)
        {
            musicVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            ApplyVolumes();
        }

        public void PlayHover()
        {
            PlaySFX(uiHoverClip);
        }

        public void PlayClick()
        {
            PlaySFX(uiClickClip);
        }

        public void PlayExplosionSound()
        {
            PlaySFX(explosionClip);
        }

        public void PlayTransition()
        {
            PlaySFX(transitionClip);
        }

        public void PlayError()
        {
            PlaySFX(errorClip);
        }

        public void PlaySuccess()
        {
            PlaySFX(successClip);
        }

        private void PlaySFX(AudioClip clip)
        {
            if (clip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
            }
        }

        public void PlayMusic(AudioClip music)
        {
            if (musicSource == null) return;
            musicSource.clip = music;
            musicSource.Play();
        }

        public void StopMusic()
        {
            if (musicSource != null) musicSource.Stop();
        }

        public void PauseMusic()
        {
            if (musicSource != null) musicSource.Pause();
        }

        public void ResumeMusic()
        {
            if (musicSource != null) musicSource.UnPause();
        }
    }
}
