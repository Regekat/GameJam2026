using UnityEngine;

public class CartCrashSound : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource impactSource;
    [SerializeField] private AudioClip bangCrashClip;
    [SerializeField] private float crashVolumeMultiplier = 2f;

    [Header("Valid Crash Colliders")]
    [SerializeField] private Collider[] crashColliders;

    [Header("Settings")]
    [SerializeField] private float cooldown = 0.1f;
    [SerializeField] private bool debugLogs = true;

    private float lastCrashTime = -999f;

    private void Awake()
    {
        if (impactSource == null)
        {
            Debug.LogError("[CartCrashSound] Impact Source is not assigned.", this);
        }

        if (impactSource != null)
        {
            impactSource.loop = false;
            impactSource.playOnAwake = false;
        }

        if (bangCrashClip == null)
        {
            Debug.LogWarning("[CartCrashSound] Bang crash clip is not assigned.", this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (impactSource == null || bangCrashClip == null)
            return;

        if (Time.time - lastCrashTime < cooldown)
            return;

        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint contact = collision.GetContact(i);
            Collider myCollider = contact.thisCollider;
            Collider otherCollider = contact.otherCollider;

            if (debugLogs)
            {
                string myName = myCollider != null ? myCollider.name : "NULL";
                string otherName = otherCollider != null ? otherCollider.name : "NULL";
                Debug.Log($"[CartCrashSound] Contact {i}: MY collider = {myName}, OTHER collider = {otherName}", this);
            }

            if (IsListedCrashCollider(myCollider))
            {
                if (debugLogs)
                {
                    Debug.Log($"[CartCrashSound] Playing crash sound from {myCollider.name}", this);
                }

                impactSource.PlayOneShot(bangCrashClip, crashVolumeMultiplier);
                lastCrashTime = Time.time;
                return;
            }
        }
    }

    private bool IsListedCrashCollider(Collider candidate)
    {
        if (candidate == null || crashColliders == null)
            return false;

        for (int i = 0; i < crashColliders.Length; i++)
        {
            if (crashColliders[i] == candidate)
                return true;
        }

        return false;
    }
}