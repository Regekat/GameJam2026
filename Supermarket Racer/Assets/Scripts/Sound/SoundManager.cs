using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Sound Clips")]
    [SerializeField] private AudioClip pickItemSound;
    [SerializeField] private AudioClip dropIntoCartSound;
    [SerializeField] private AudioClip ambientSupermarketSound;
    [SerializeField] private AudioClip textSound;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip groceryListOpenSound;
    [SerializeField] private AudioClip groceryListCloseSound;

    [Header("Volumes")]
    [SerializeField][Range(0f, 1f)] private float ambientVolume = 0.5f;
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

    private void Start()
    {
        PlayAmbientSupermarketSound();
    }

    private void ValidateSources()
    {
        if (ambientSource == null)
        {
            Debug.LogError("[SoundManager] Ambient Source is not assigned.");
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

        if (sfxSource != null)
        {
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.volume = sfxVolume;
        }
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

    public void PlayGroceryListOpenSound()
    {
        PlayOneShot(groceryListOpenSound);
    }

    public void PlayGroceryListCloseSound()
    {
        PlayOneShot(groceryListCloseSound);
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

    private void OnValidate()
    {
        if (ambientSource != null)
        {
            ambientSource.volume = ambientVolume;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }
}