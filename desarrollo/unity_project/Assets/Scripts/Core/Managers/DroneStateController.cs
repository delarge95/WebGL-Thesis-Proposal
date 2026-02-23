using System;
using System.Collections;
using UnityEngine;
using WebGL.Core.Utils;
using WebGL.Core.Events;

namespace WebGL.Core.Managers
{
    public enum DroneState
    {
        Off,
        StartingUp,
        Idle,
        Flying,
        ShuttingDown
    }

    public class DroneStateController : Singleton<DroneStateController>
    {
        [Header("State")]
        [SerializeField] private DroneState currentState = DroneState.Off;

        [Header("Animations")]
        [SerializeField] private float startupDuration = 2f;
        [SerializeField] private float shutdownDuration = 1.5f;
        [SerializeField] private float propellerMaxSpeed = 2000f;
        [SerializeField] private float hoverAmplitude = 0.1f;
        [SerializeField] private float hoverFrequency = 1f;

        [Header("Audio")]
        [SerializeField] private AudioClip startupSound;
        [SerializeField] private AudioClip idleSound;
        [SerializeField] private AudioClip flyingSound;
        [SerializeField] private AudioClip shutdownSound;

        [Header("References")]
        [SerializeField] private Transform droneRoot;
        [SerializeField] private Transform[] propellers;
        [SerializeField] private Light[] statusLights;
        [SerializeField] private ParticleSystem[] thrusterParticles;

        private AudioSource droneAudioSource;
        private Vector3 originalPosition;
        private float currentPropellerSpeed = 0f;
        private float targetPropellerSpeed = 0f;
        private Coroutine stateCoroutine;

        public DroneState CurrentState => currentState;
        public bool IsOn => currentState != DroneState.Off && currentState != DroneState.ShuttingDown;

        public event Action<DroneState> OnStateChanged;

        protected override void Awake()
        {
            base.Awake();
            droneAudioSource = gameObject.AddComponent<AudioSource>();
            droneAudioSource.loop = true;
            droneAudioSource.spatialBlend = 1f;
        }

        private void Start()
        {
            if (droneRoot != null)
            {
                originalPosition = droneRoot.localPosition;
            }
            SetLightsColor(Color.red);
        }

        private void Update()
        {
            // Animate propellers
            UpdatePropellers();

            // Hover animation when flying
            if (currentState == DroneState.Flying && droneRoot != null)
            {
                float hover = Mathf.Sin(Time.time * hoverFrequency * Mathf.PI * 2) * hoverAmplitude;
                droneRoot.localPosition = originalPosition + Vector3.up * hover;
            }
        }

        private void UpdatePropellers()
        {
            // Smooth propeller speed
            currentPropellerSpeed = Mathf.Lerp(currentPropellerSpeed, targetPropellerSpeed, Time.deltaTime * 5f);

            // Rotate propellers
            if (propellers != null)
            {
                foreach (var propeller in propellers)
                {
                    if (propeller != null)
                    {
                        propeller.Rotate(Vector3.up, currentPropellerSpeed * Time.deltaTime);
                    }
                }
            }
        }

        public void TurnOn()
        {
            if (currentState != DroneState.Off) return;
            
            if (stateCoroutine != null) StopCoroutine(stateCoroutine);
            stateCoroutine = StartCoroutine(StartupSequence());
        }

        public void TurnOff()
        {
            if (currentState == DroneState.Off || currentState == DroneState.ShuttingDown) return;

            if (stateCoroutine != null) StopCoroutine(stateCoroutine);
            stateCoroutine = StartCoroutine(ShutdownSequence());
        }

        public void TogglePower()
        {
            if (currentState == DroneState.Off)
            {
                TurnOn();
            }
            else if (currentState == DroneState.Idle || currentState == DroneState.Flying)
            {
                TurnOff();
            }
        }

        public void SetFlying(bool flying)
        {
            if (!IsOn) return;

            if (flying && currentState == DroneState.Idle)
            {
                SetState(DroneState.Flying);
                targetPropellerSpeed = propellerMaxSpeed;
                PlayDroneSound(flyingSound);
                SetParticles(true);
            }
            else if (!flying && currentState == DroneState.Flying)
            {
                SetState(DroneState.Idle);
                targetPropellerSpeed = propellerMaxSpeed * 0.3f;
                PlayDroneSound(idleSound);
                SetParticles(false);
            }
        }

        private IEnumerator StartupSequence()
        {
            SetState(DroneState.StartingUp);
            
            // Play startup sound
            if (AudioManager.Instance != null && startupSound != null)
            {
                droneAudioSource.PlayOneShot(startupSound);
            }

            // Animate lights blinking
            SetLightsColor(Color.yellow);

            // Ramp up propellers
            float timer = 0f;
            while (timer < startupDuration)
            {
                timer += Time.deltaTime;
                float t = timer / startupDuration;
                targetPropellerSpeed = Mathf.Lerp(0f, propellerMaxSpeed * 0.3f, t);
                yield return null;
            }

            // Ready
            SetLightsColor(Color.green);
            SetState(DroneState.Idle);
            PlayDroneSound(idleSound);

            NotificationManager.Instance?.ShowNotification("Drone is now online");
        }

        private IEnumerator ShutdownSequence()
        {
            SetState(DroneState.ShuttingDown);

            // Play shutdown sound
            if (shutdownSound != null)
            {
                droneAudioSource.PlayOneShot(shutdownSound);
            }

            // Return to original position
            if (droneRoot != null && currentState == DroneState.Flying)
            {
                float timer = 0f;
                Vector3 startPos = droneRoot.localPosition;
                while (timer < shutdownDuration * 0.5f)
                {
                    timer += Time.deltaTime;
                    droneRoot.localPosition = Vector3.Lerp(startPos, originalPosition, timer / (shutdownDuration * 0.5f));
                    yield return null;
                }
            }

            // Ramp down propellers
            SetLightsColor(Color.yellow);
            SetParticles(false);
            float timer2 = 0f;
            while (timer2 < shutdownDuration)
            {
                timer2 += Time.deltaTime;
                float t = 1f - (timer2 / shutdownDuration);
                targetPropellerSpeed = propellerMaxSpeed * 0.3f * t;
                yield return null;
            }

            targetPropellerSpeed = 0f;
            SetLightsColor(Color.red);
            droneAudioSource.Stop();
            SetState(DroneState.Off);

            NotificationManager.Instance?.ShowNotification("Drone is now offline");
        }

        private void SetState(DroneState newState)
        {
            currentState = newState;
            OnStateChanged?.Invoke(newState);
            Debug.Log($"[DroneState] {newState}");
        }

        private void SetLightsColor(Color color)
        {
            if (statusLights == null) return;
            foreach (var light in statusLights)
            {
                if (light != null)
                {
                    light.color = color;
                }
            }
        }

        private void SetParticles(bool active)
        {
            if (thrusterParticles == null) return;
            foreach (var ps in thrusterParticles)
            {
                if (ps != null)
                {
                    if (active) ps.Play();
                    else ps.Stop();
                }
            }
        }

        private void PlayDroneSound(AudioClip clip)
        {
            if (clip == null) return;
            droneAudioSource.clip = clip;
            droneAudioSource.Play();
        }
    }
}
