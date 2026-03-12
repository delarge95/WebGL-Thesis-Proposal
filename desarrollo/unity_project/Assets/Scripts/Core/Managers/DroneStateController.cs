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

        [Header("Thermal / Load")]
        [SerializeField, Range(0f, 1f)] private float systemLoadFactor = 0.35f;
        [SerializeField, Range(0f, 1f)] private float idleLoadFloor = 0.2f;
        [SerializeField, Range(0f, 1f)] private float hoverLoadFloor = 0.45f;

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
        private Coroutine stateCoroutine;

        public DroneState CurrentState => currentState;
        public bool IsOn => currentState != DroneState.Off && currentState != DroneState.ShuttingDown;
        public float SystemLoadFactor => systemLoadFactor;

        public event Action<DroneState> OnStateChanged;
        public event Action<float> OnSystemLoadChanged;

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

            ApplyLoadToAnimationTargets();
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

        private void UpdatePropellers()
        {
            currentPropellerSpeed = Mathf.Lerp(currentPropellerSpeed, targetPropellerSpeed, Time.deltaTime * 5f);

            if (propellers == null)
            {
                return;
            }

            foreach (Transform propeller in propellers)
            {
                if (propeller != null)
                {
                    propeller.Rotate(Vector3.up, currentPropellerSpeed * Time.deltaTime);
                }
            }
        }

        public void TurnOn()
        {
            if (currentState != DroneState.Off)
            {
                return;
            }

            if (stateCoroutine != null)
            {
                StopCoroutine(stateCoroutine);
            }

            stateCoroutine = StartCoroutine(StartupSequence());
        }

        public void TurnOff()
        {
            if (currentState == DroneState.Off || currentState == DroneState.ShuttingDown)
            {
                return;
            }

            if (stateCoroutine != null)
            {
                StopCoroutine(stateCoroutine);
            }

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
            if (!IsOn)
            {
                return;
            }

            if (flying && currentState == DroneState.Idle)
            {
                SetState(DroneState.Flying);
                PlayDroneSound(flyingSound);
                SetParticles(true);
            }
            else if (!flying && currentState == DroneState.Flying)
            {
                SetState(DroneState.Idle);
                PlayDroneSound(idleSound);
                SetParticles(false);
            }
        }

        public void SetSystemLoad(float normalizedLoad)
        {
            float clamped = Mathf.Clamp01(normalizedLoad);
            bool loadChanged = !Mathf.Approximately(systemLoadFactor, clamped);

            systemLoadFactor = clamped;

            if (loadChanged)
            {
                OnSystemLoadChanged?.Invoke(systemLoadFactor);
                EventBus.Publish(new ThermalLoadChangedEvent(systemLoadFactor));
            }

            if (currentState == DroneState.Idle || currentState == DroneState.Flying)
            {
                bool shouldFly = systemLoadFactor >= hoverLoadFloor;
                if (shouldFly != (currentState == DroneState.Flying))
                {
                    SetFlying(shouldFly);
                }
            }

            ApplyLoadToAnimationTargets();
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
                float t = timer / startupDuration;
                targetPropellerSpeed = Mathf.Lerp(0f, propellerMaxSpeed * 0.3f, t);
                yield return null;
            }

            SetLightsColor(Color.green);
            SetState(DroneState.Idle);
            PlayDroneSound(idleSound);
            SetSystemLoad(Mathf.Max(systemLoadFactor, idleLoadFloor));

            NotificationManager.Instance?.ShowNotification("Drone is now online");
        }

        private IEnumerator ShutdownSequence()
        {
            bool wasFlying = currentState == DroneState.Flying;
            float shutdownStartSpeed = Mathf.Max(currentPropellerSpeed, targetPropellerSpeed);

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
            while (timer2 < shutdownDuration)
            {
                timer2 += Time.deltaTime;
                float t = timer2 / shutdownDuration;
                targetPropellerSpeed = Mathf.Lerp(shutdownStartSpeed, 0f, t);
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
            ApplyLoadToAnimationTargets();
            OnStateChanged?.Invoke(newState);
            Debug.Log($"[DroneState] {newState}");
        }

        private void ApplyLoadToAnimationTargets()
        {
            float load = Mathf.Clamp01(systemLoadFactor);

            switch (currentState)
            {
                case DroneState.Off:
                case DroneState.ShuttingDown:
                    targetPropellerSpeed = 0f;
                    break;

                case DroneState.StartingUp:
                    break;

                case DroneState.Idle:
                    {
                        float idleBlend = Mathf.InverseLerp(0f, Mathf.Max(hoverLoadFloor, 0.01f), Mathf.Max(load, idleLoadFloor));
                        targetPropellerSpeed = propellerMaxSpeed * Mathf.Lerp(0.18f, 0.35f, idleBlend);
                        break;
                    }

                case DroneState.Flying:
                    {
                        float flyingLoad = Mathf.Max(load, hoverLoadFloor);
                        targetPropellerSpeed = propellerMaxSpeed * Mathf.Lerp(0.45f, 1f, flyingLoad);
                        break;
                    }
            }
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
                if (ps != null)
                {
                    if (active)
                    {
                        ps.Play();
                    }
                    else
                    {
                        ps.Stop();
                    }
                }
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