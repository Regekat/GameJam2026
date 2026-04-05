using UnityEngine;

public class CartSoundManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody cartRigidbody;
    [SerializeField] private AudioSource rollingSource;
    [SerializeField] private AudioSource impactSource;

    [Header("Rolling Clip")]
    [SerializeField] private AudioClip rollingClip;

    [Header("Optional One-Shots")]
    [SerializeField] private AudioClip brakeClip;
    [SerializeField] private AudioClip bumpClip;

    [Header("Speed Detection")]
    [SerializeField] private bool useRigidbodyVelocity = true;
    [SerializeField] private float minSpeedToPlay = 0.15f;
    [SerializeField] private float maxExpectedSpeed = 10f;

    [Header("Rolling Sound")]
    [SerializeField] private float minVolume = 0.05f;
    [SerializeField] private float maxVolume = 0.8f;
    [SerializeField] private float minPitch = 0.85f;
    [SerializeField] private float maxPitch = 1.2f;
    [SerializeField] private float volumeLerpSpeed = 8f;
    [SerializeField] private float pitchLerpSpeed = 8f;
    [SerializeField] private bool playIn3D = true;

    [Header("Brake Detection")]
    [SerializeField] private bool enableBrakeSound = true;
    [SerializeField] private KeyCode brakeKey = KeyCode.Space;
    [SerializeField] private float minSpeedForBrakeSound = 0.5f;
    [SerializeField] private float brakeCooldown = 0.2f;

    [Header("Impact Detection")]
    [SerializeField] private bool enableImpactSound = true;
    [SerializeField] private float impactCooldown = 0.1f;

    private Vector3 lastPosition;
    private float currentSpeed;
    private float lastBrakeTime = -999f;
    private float lastImpactTime = -999f;

    private void Awake()
    {
        if (cartRigidbody == null)
        {
            cartRigidbody = GetComponentInParent<Rigidbody>();
        }

        if (rollingSource == null)
        {
            Debug.LogError("[CartSoundManager] Rolling Source is not assigned.", this);
        }

        if (impactSource == null)
        {
            Debug.LogWarning("[CartSoundManager] Impact Source is not assigned. Impact/brake one-shots will not play.", this);
        }

        if (rollingSource != null)
        {
            rollingSource.loop = true;
            rollingSource.playOnAwake = false;
            rollingSource.clip = rollingClip;
            rollingSource.spatialBlend = playIn3D ? 1f : 0f;
            rollingSource.volume = 0f;
            rollingSource.pitch = minPitch;
        }

        if (impactSource != null)
        {
            impactSource.loop = false;
            impactSource.playOnAwake = false;
            impactSource.spatialBlend = playIn3D ? 1f : 0f;
        }

        lastPosition = transform.position;
    }

    private void Update()
    {
        UpdateSpeed();
        UpdateRollingLoop();
        DetectBrakeInput();
    }

    private void UpdateSpeed()
    {
        if (useRigidbodyVelocity && cartRigidbody != null)
        {
            Vector3 flatVelocity = cartRigidbody.linearVelocity;
            flatVelocity.y = 0f;
            currentSpeed = flatVelocity.magnitude;
            return;
        }

        Vector3 currentPosition = transform.position;
        Vector3 delta = currentPosition - lastPosition;
        delta.y = 0f;

        currentSpeed = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
        lastPosition = currentPosition;
    }

    private void UpdateRollingLoop()
    {
        if (rollingSource == null || rollingClip == null)
            return;

        if (currentSpeed > minSpeedToPlay)
        {
            if (!rollingSource.isPlaying)
            {
                rollingSource.clip = rollingClip;
                rollingSource.Play();
            }

            float speed01 = Mathf.Clamp01(currentSpeed / Mathf.Max(maxExpectedSpeed, 0.01f));
            float targetVolume = Mathf.Lerp(minVolume, maxVolume, speed01);
            float targetPitch = Mathf.Lerp(minPitch, maxPitch, speed01);

            rollingSource.volume = Mathf.Lerp(rollingSource.volume, targetVolume, volumeLerpSpeed * Time.deltaTime);
            rollingSource.pitch = Mathf.Lerp(rollingSource.pitch, targetPitch, pitchLerpSpeed * Time.deltaTime);
        }
        else
        {
            rollingSource.volume = Mathf.Lerp(rollingSource.volume, 0f, volumeLerpSpeed * Time.deltaTime);
            rollingSource.pitch = Mathf.Lerp(rollingSource.pitch, minPitch, pitchLerpSpeed * Time.deltaTime);

            if (rollingSource.isPlaying && rollingSource.volume <= 0.01f)
            {
                rollingSource.Stop();
            }
        }
    }

    private void DetectBrakeInput()
    {
        if (!enableBrakeSound || brakeClip == null || impactSource == null)
            return;

        if (!Input.GetKeyDown(brakeKey))
            return;

        if (Time.time - lastBrakeTime < brakeCooldown)
            return;

        if (currentSpeed < minSpeedForBrakeSound)
            return;

        impactSource.PlayOneShot(brakeClip);
        lastBrakeTime = Time.time;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public void ForcePlayBrake()
    {
        if (impactSource != null && brakeClip != null)
        {
            impactSource.PlayOneShot(brakeClip);
            lastBrakeTime = Time.time;
        }
    }

    public void StopRollingLoopImmediate()
    {
        if (rollingSource != null)
        {
            rollingSource.Stop();
            rollingSource.volume = 0f;
            rollingSource.pitch = minPitch;
        }
    }

    public void NotifyCollision()
    {
        if (!enableImpactSound || bumpClip == null || impactSource == null)
            return;

        if (Time.time - lastImpactTime < impactCooldown)
            return;

        Debug.Log("Crash detected, playing");
        impactSource.PlayOneShot(bumpClip);
        lastImpactTime = Time.time;
    }
}