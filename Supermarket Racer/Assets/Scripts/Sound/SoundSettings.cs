using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundSettings : MonoBehaviour
{
    public static SoundSettings Instance { get; private set; }

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Exposed Parameter Names")]
    [SerializeField] private string musicParameter = "MusicVolume";
    [SerializeField] private string ambienceParameter = "AmbienceVolume";
    [SerializeField] private string dialogueParameter = "DialogueVolume";
    [SerializeField] private string sfxParameter = "SFXVolume";
    [SerializeField] private string uiParameter = "UIVolume";

    [Header("Optional UI Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider ambienceSlider;
    [SerializeField] private Slider dialogueSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider uiSlider;

    [Header("Optional Menu Root")]
    [SerializeField] private GameObject soundSettingsMenuRoot;

    private const string MusicPrefKey = "MusicVolume";
    private const string AmbiencePrefKey = "AmbienceVolume";
    private const string DialoguePrefKey = "DialogueVolume";
    private const string SFXPrefKey = "SFXVolume";
    private const string UIPrefKey = "UIVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllVolumes();
    }

    private void Start()
    {
        ApplyLoadedVolumesToSliders();
        HookUpSliders();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            UnhookSliders();
        }
    }

    private void HookUpSliders()
    {
        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(SetMusicVolume);

        if (ambienceSlider != null)
            ambienceSlider.onValueChanged.AddListener(SetAmbienceVolume);

        if (dialogueSlider != null)
            dialogueSlider.onValueChanged.AddListener(SetDialogueVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        if (uiSlider != null)
            uiSlider.onValueChanged.AddListener(SetUIVolume);
    }

    private void UnhookSliders()
    {
        if (musicSlider != null)
            musicSlider.onValueChanged.RemoveListener(SetMusicVolume);

        if (ambienceSlider != null)
            ambienceSlider.onValueChanged.RemoveListener(SetAmbienceVolume);

        if (dialogueSlider != null)
            dialogueSlider.onValueChanged.RemoveListener(SetDialogueVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);

        if (uiSlider != null)
            uiSlider.onValueChanged.RemoveListener(SetUIVolume);
    }

    public void SetMusicVolume(float sliderValue)
    {
        SetVolume(musicParameter, sliderValue, MusicPrefKey);
    }

    public void SetAmbienceVolume(float sliderValue)
    {
        SetVolume(ambienceParameter, sliderValue, AmbiencePrefKey);
    }

    public void SetDialogueVolume(float sliderValue)
    {
        SetVolume(dialogueParameter, sliderValue, DialoguePrefKey);
    }

    public void SetSFXVolume(float sliderValue)
    {
        SetVolume(sfxParameter, sliderValue, SFXPrefKey);
    }

    public void SetUIVolume(float sliderValue)
    {
        SetVolume(uiParameter, sliderValue, UIPrefKey);
    }

    private void SetVolume(string parameterName, float sliderValue, string prefKey)
    {
        if (audioMixer == null)
        {
            Debug.LogWarning("SoundSettings: No AudioMixer assigned.");
            return;
        }

        float clampedValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        float decibels = Mathf.Log10(clampedValue) * 20f;

        audioMixer.SetFloat(parameterName, decibels);
        PlayerPrefs.SetFloat(prefKey, clampedValue);
        PlayerPrefs.Save();
    }

    private void LoadAllVolumes()
    {
        float music = PlayerPrefs.GetFloat(MusicPrefKey, 1f);
        float ambience = PlayerPrefs.GetFloat(AmbiencePrefKey, 1f);
        float dialogue = PlayerPrefs.GetFloat(DialoguePrefKey, 1f);
        float sfx = PlayerPrefs.GetFloat(SFXPrefKey, 1f);
        float ui = PlayerPrefs.GetFloat(UIPrefKey, 1f);

        ApplyVolume(musicParameter, music);
        ApplyVolume(ambienceParameter, ambience);
        ApplyVolume(dialogueParameter, dialogue);
        ApplyVolume(sfxParameter, sfx);
        ApplyVolume(uiParameter, ui);
    }

    private void ApplyLoadedVolumesToSliders()
    {
        if (musicSlider != null)
            musicSlider.value = PlayerPrefs.GetFloat(MusicPrefKey, 1f);

        if (ambienceSlider != null)
            ambienceSlider.value = PlayerPrefs.GetFloat(AmbiencePrefKey, 1f);

        if (dialogueSlider != null)
            dialogueSlider.value = PlayerPrefs.GetFloat(DialoguePrefKey, 1f);

        if (sfxSlider != null)
            sfxSlider.value = PlayerPrefs.GetFloat(SFXPrefKey, 1f);

        if (uiSlider != null)
            uiSlider.value = PlayerPrefs.GetFloat(UIPrefKey, 1f);
    }

    private void ApplyVolume(string parameterName, float sliderValue)
    {
        if (audioMixer == null)
        {
            Debug.LogWarning("SoundSettings: No AudioMixer assigned.");
            return;
        }

        float clampedValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);
        float decibels = Mathf.Log10(clampedValue) * 20f;
        audioMixer.SetFloat(parameterName, decibels);
    }

    public void OpenMenu()
    {
        if (soundSettingsMenuRoot != null)
        {
            soundSettingsMenuRoot.SetActive(true);
        }
    }

    public void CloseMenu()
    {
        if (soundSettingsMenuRoot != null)
        {
            soundSettingsMenuRoot.SetActive(false);
        }
    }

    public void ToggleMenu()
    {
        if (soundSettingsMenuRoot != null)
        {
            soundSettingsMenuRoot.SetActive(!soundSettingsMenuRoot.activeSelf);
        }
    }

    public bool IsMenuOpen()
    {
        return soundSettingsMenuRoot != null && soundSettingsMenuRoot.activeSelf;
    }
}