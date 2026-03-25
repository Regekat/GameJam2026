using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private AudioSource movementLoopSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Sound Clips")]
    [SerializeField] private AudioClip cartMovementSound;
    [SerializeField] private AudioClip brakeSound;
    [SerializeField] private AudioClip pickItemSound;
    [SerializeField] private AudioClip dropIntoCartSound;
    [SerializeField] private AudioClip ambientSupermarketSound;
    [SerializeField] private AudioClip textSound;
    [SerializeField] private AudioClip buttonClickSound;

    [Header("Volumes")]
    [SerializeField][Range(0f, 1f)] private float ambientVolume = 0.5f;
    [SerializeField][Range(0f, 1f)] private float movementVolume = 0.75f;
    [SerializeField][Range(0f, 1f)] private float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        ValidateSources();
        ConfigureSources();
    }

    private void ValidateSources()
    {
        if (ambientSource == null)
        {
            Debug.LogError("[SoundManager] Ambient Source is not assigned.");
        }

        if (movementLoopSource == null)
        {
            Debug.LogError("[SoundManager] Movement Loop Source is not assigned.");
        }

        if (sfxSource == null)
        {
            Debug.LogError("[SoundManager] SFX Source is not assigned.");
        }
    }

    private void ConfigureSources()
    {
        if (ambientSource != null)
        {
            ambientSource.loop = true;
            ambientSource.playOnAwake = false;
            ambientSource.volume = ambientVolume;
        }

        if (movementLoopSource != null)
        {
            movementLoopSource.loop = true;
            movementLoopSource.playOnAwake = false;
            movementLoopSource.volume = movementVolume;
        }

        if (sfxSource != null)
        {
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.volume = sfxVolume;
        }
    }

    public void PlayCartMovementSound()
    {
        if (movementLoopSource == null || cartMovementSound == null)
            return;

        if (movementLoopSource.clip != cartMovementSound)
        {
            movementLoopSource.clip = cartMovementSound;
        }

        if (!movementLoopSource.isPlaying)
        {
            movementLoopSource.Play();
        }
    }

    public void StopCartMovementSound()
    {
        if (movementLoopSource == null)
            return;

        if (movementLoopSource.isPlaying)
        {
            movementLoopSource.Stop();
        }
    }

    public void PlayBrakeSound()
    {
        PlayOneShot(brakeSound);
    }

    public void PlayPickItemSound()
    {
        PlayOneShot(pickItemSound);
    }

    public void PlayDropIntoCartSound()
    {
        PlayOneShot(dropIntoCartSound);
    }

    public void PlayTextSound()
    {
        PlayOneShot(textSound);
    }

    public void PlayButtonClickSound()
    {
        PlayOneShot(buttonClickSound);
    }

    public void PlayAmbientSupermarketSound()
    {
        if (ambientSource == null || ambientSupermarketSound == null)
            return;

        if (ambientSource.clip != ambientSupermarketSound)
        {
            ambientSource.clip = ambientSupermarketSound;
        }

        if (!ambientSource.isPlaying)
        {
            ambientSource.Play();
        }
    }

    public void StopAmbientSupermarketSound()
    {
        if (ambientSource == null)
            return;

        if (ambientSource.isPlaying)
        {
            ambientSource.Stop();
        }
    }

    public void StopAllSounds()
    {
        if (ambientSource != null)
        {
            ambientSource.Stop();
        }

        if (movementLoopSource != null)
        {
            movementLoopSource.Stop();
        }

        if (sfxSource != null)
        {
            sfxSource.Stop();
        }
    }

    public void SetAmbientVolume(float volume)
    {
        ambientVolume = Mathf.Clamp01(volume);

        if (ambientSource != null)
        {
            ambientSource.volume = ambientVolume;
        }
    }

    public void SetMovementVolume(float volume)
    {
        movementVolume = Mathf.Clamp01(volume);

        if (movementLoopSource != null)
        {
            movementLoopSource.volume = movementVolume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);

        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    private void PlayOneShot(AudioClip clip)
    {
        if (sfxSource == null || clip == null)
            return;

        sfxSource.PlayOneShot(clip, sfxVolume);
    }
}