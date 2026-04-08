using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebGL.Core.Events;
using WebGL.Core.Utils;

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
        [SerializeField] private float idlePropellerFactor = 0.3f;
        [SerializeField] private float hoverAmplitude = 0.1f;
        [SerializeField] private float hoverFrequency = 1f;
        [SerializeField] private float flightThresholdNormalized = 0.35f;
        [SerializeField] private float armedLoadFloor = 0.2f;

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
        private float currentPropellerSpeed;
        private float targetPropellerSpeed;
        private float systemLoadFactor;
        private float loadCommandNormalized;
        private Coroutine stateCoroutine;

        public DroneState CurrentState => currentState;
        public bool IsOn => currentState != DroneState.Off && currentState != DroneState.ShuttingDown;
        public float LoadCommandNormalized => loadCommandNormalized;

        public float SystemLoadFactor
        {
            get => systemLoadFactor;
            private set
            {
                float clamped = Mathf.Clamp01(value);
                if (Mathf.Approximately(systemLoadFactor, clamped))
                {
                    return;
                }

                systemLoadFactor = clamped;
                OnSystemLoadChanged?.Invoke(systemLoadFactor);
            }
        }

        public event Action<DroneState> OnStateChanged;
        public event Action<float> OnSystemLoadChanged;
        public event Action<float> OnLoadCommandChanged;

        protected override void Awake()
        {
            base.Awake();
            droneAudioSource = gameObject.GetComponent<AudioSource>();
            if (droneAudioSource == null)
            {
                droneAudioSource = gameObject.AddComponent<AudioSource>();
            }

            droneAudioSource.loop = true;
            droneAudioSource.spatialBlend = 1f;
        }

        private void Start()
        {
            AutoAssignBindings();
            CacheOriginalPosition();
            SetLightsColor(Color.red);
        }

        private void Update()
        {
            UpdatePropellers();

            if (currentState == DroneState.Flying && droneRoot != null)
            {
                float hover = Mathf.Sin(Time.time * hoverFrequency * Mathf.PI * 2f) * hoverAmplitude;
                droneRoot.localPosition = originalPosition + Vector3.up * hover;
            }
        }

        public void ConfigureRuntimeBindings(Transform rootTransform, Transform[] detectedPropellers)
        {
            if (rootTransform != null)
            {
                droneRoot = rootTransform;
            }

            if (detectedPropellers != null && detectedPropellers.Length > 0)
            {
                propellers = detectedPropellers;
            }

            CacheOriginalPosition();
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

        public void TurnOn()
        {
            if (currentState != DroneState.Off)
            {
                return;
            }

            AutoAssignBindings();
            if (stateCoroutine != null) StopCoroutine(stateCoroutine);
            stateCoroutine = StartCoroutine(StartupSequence());
        }

        public void TurnOff()
        {
            if (currentState == DroneState.Off || currentState == DroneState.ShuttingDown)
            {
                return;
            }

            if (stateCoroutine != null) StopCoroutine(stateCoroutine);
            stateCoroutine = StartCoroutine(ShutdownSequence());
        }

        public void SetLoadCommand(float normalized)
        {
            float clamped = Mathf.Clamp01(normalized);
            if (Mathf.Approximately(loadCommandNormalized, clamped))
            {
                return;
            }

            loadCommandNormalized = clamped;
            OnLoadCommandChanged?.Invoke(loadCommandNormalized);

            if (IsOn && currentState != DroneState.StartingUp)
            {
                ApplyLoadCommand();
            }
        }

        public void SetFlying(bool flying)
        {
            SetLoadCommand(flying ? Mathf.Max(loadCommandNormalized, 0.6f) : 0f);
        }

        private void AutoAssignBindings()
        {
            if (droneRoot == null)
            {
                GameObject rootObject = GameObject.Find("x500v2_Drone");
                if (rootObject != null)
                {
                    droneRoot = rootObject.transform;
                }
            }

            if (propellers == null || propellers.Length == 0)
            {
                List<Transform> detected = new List<Transform>();
                if (droneRoot != null)
                {
                    foreach (Transform child in droneRoot.GetComponentsInChildren<Transform>(true))
                    {
                        if (child == null) continue;
                        string name = child.name.ToLowerInvariant();
                        if (name.StartsWith("x500v2_prop_", StringComparison.Ordinal))
                        {
                            detected.Add(child);
                        }
                    }
                }

                if (detected.Count > 0)
                {
                    propellers = detected.ToArray();
                }
            }
        }

        private void CacheOriginalPosition()
        {
            if (droneRoot != null)
            {
                originalPosition = droneRoot.localPosition;
            }
        }

        private void UpdatePropellers()
        {
            currentPropellerSpeed = Mathf.Lerp(currentPropellerSpeed, targetPropellerSpeed, Time.deltaTime * 5f);

            if (propellers == null)
            {
                return;
            }

            for (int i = 0; i < propellers.Length; i++)
            {
                Transform propeller = propellers[i];
                if (propeller == null) continue;

                float direction = i % 2 == 0 ? 1f : -1f;
                propeller.Rotate(Vector3.forward, currentPropellerSpeed * direction * Time.deltaTime, Space.Self);
            }
        }

        private IEnumerator StartupSequence()
        {
            SetState(DroneState.StartingUp);

            if (startupSound != null)
            {
                droneAudioSource.PlayOneShot(startupSound);
            }

            SetLightsColor(Color.yellow);

            float timer = 0f;
            while (timer < startupDuration)
            {
                timer += Time.deltaTime;
                float t = timer / Mathf.Max(startupDuration, 0.01f);
                targetPropellerSpeed = Mathf.Lerp(0f, propellerMaxSpeed * idlePropellerFactor, t);
                yield return null;
            }

            SetLightsColor(Color.green);
            SetState(DroneState.Idle);
            ApplyLoadCommand();
            PlayDroneSound(idleSound);

            NotificationManager.Instance?.ShowNotification("Drone is now online");
        }

        private IEnumerator ShutdownSequence()
        {
            bool wasFlying = currentState == DroneState.Flying;
            SetState(DroneState.ShuttingDown);

            if (shutdownSound != null)
            {
                droneAudioSource.PlayOneShot(shutdownSound);
            }

            if (droneRoot != null && wasFlying)
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

            SetLightsColor(Color.yellow);
            SetParticles(false);

            float timer2 = 0f;
            float startSpeed = targetPropellerSpeed;
            while (timer2 < shutdownDuration)
            {
                timer2 += Time.deltaTime;
                float t = 1f - (timer2 / Mathf.Max(shutdownDuration, 0.01f));
                targetPropellerSpeed = startSpeed * t;
                yield return null;
            }

            targetPropellerSpeed = 0f;
            SetLightsColor(Color.red);
            droneAudioSource.Stop();
            SetState(DroneState.Off);

            NotificationManager.Instance?.ShowNotification("Drone is now offline");
        }

        private void ApplyLoadCommand()
        {
            if (!IsOn || currentState == DroneState.StartingUp)
            {
                return;
            }

            float loadFactor = Mathf.Lerp(armedLoadFloor, 1f, loadCommandNormalized);
            bool shouldFly = loadCommandNormalized > flightThresholdNormalized;

            if (shouldFly && currentState != DroneState.Flying)
            {
                SetState(DroneState.Flying);
                PlayDroneSound(flyingSound != null ? flyingSound : idleSound);
                SetParticles(true);
            }
            else if (!shouldFly && currentState != DroneState.Idle)
            {
                SetState(DroneState.Idle);
                PlayDroneSound(idleSound);
                SetParticles(false);
            }

            if (currentState == DroneState.Idle)
            {
                float idleBlend = flightThresholdNormalized > 0f
                    ? Mathf.Clamp01(loadCommandNormalized / flightThresholdNormalized)
                    : 0f;
                targetPropellerSpeed = propellerMaxSpeed * Mathf.Lerp(idlePropellerFactor, idlePropellerFactor * 1.6f, idleBlend);
            }
            else if (currentState == DroneState.Flying)
            {
                float flightBlend = Mathf.InverseLerp(flightThresholdNormalized, 1f, loadCommandNormalized);
                targetPropellerSpeed = propellerMaxSpeed * Mathf.Lerp(0.58f, 1f, flightBlend);
            }

            SystemLoadFactor = loadFactor;
        }

        private void SetState(DroneState newState)
        {
            currentState = newState;
            OnStateChanged?.Invoke(newState);

            if (newState == DroneState.Off || newState == DroneState.ShuttingDown)
            {
                SystemLoadFactor = 0f;
            }
            else if (newState == DroneState.StartingUp)
            {
                SystemLoadFactor = 0.1f;
            }

            Debug.Log($"[DroneState] {newState}");
        }

        private void SetLightsColor(Color color)
        {
            if (statusLights == null)
            {
                return;
            }

            foreach (Light light in statusLights)
            {
                if (light != null)
                {
                    light.color = color;
                }
            }
        }

        private void SetParticles(bool active)
        {
            if (thrusterParticles == null)
            {
                return;
            }

            foreach (ParticleSystem ps in thrusterParticles)
            {
                if (ps == null) continue;
                if (active) ps.Play();
                else ps.Stop();
            }
        }

        private void PlayDroneSound(AudioClip clip)
        {
            if (clip == null)
            {
                return;
            }

            droneAudioSource.clip = clip;
            droneAudioSource.Play();
        }
    }
}
