using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(AudioSource))]
public class UIMessageRender : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private AudioSource audioSource;

    [Header("Defaults")]
    [SerializeField] private float defaultCharacterDelay = 0.05f;
    [SerializeField] private float defaultFadeInDuration = 0.25f;
    [SerializeField] private float defaultFadeOutDuration = 0.25f;

    private Coroutine currentRoutine;
    private bool isPlaying;
    private bool skipRequested;

    public bool IsPlaying => isPlaying;

    private void Reset()
    {
        targetText = GetComponent<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Awake()
    {
        if (targetText == null)
            targetText = GetComponent<TextMeshProUGUI>();

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        targetText.text = "";
        targetText.maxVisibleCharacters = 0;
        canvasGroup.alpha = 0f;
    }

    public void PlayMessage(UIMessageData messageData)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(PlayMessageRoutine(messageData));
    }

    public void SkipCurrentMessage()
    {
        skipRequested = true;
    }

    public void ClearImmediately()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        isPlaying = false;
        skipRequested = false;

        targetText.text = "";
        targetText.maxVisibleCharacters = 0;
        canvasGroup.alpha = 0f;
    }

    private IEnumerator PlayMessageRoutine(UIMessageData messageData)
    {
        isPlaying = true;
        skipRequested = false;

        targetText.color = messageData.textColor;
        targetText.text = messageData.message;
        targetText.maxVisibleCharacters = 0;
        targetText.ForceMeshUpdate();

        int totalVisibleCharacters = targetText.textInfo.characterCount;

        if (messageData.fadeInDuration > 0f)
        {
            yield return FadeCanvas(0f, 1f, messageData.fadeInDuration);
        }
        else
        {
            canvasGroup.alpha = 1f;
        }

        if (!skipRequested && messageData.startDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(messageData.startDelay);
        }

        if (messageData.useTypewriter)
        {
            for (int i = 0; i <= totalVisibleCharacters; i++)
            {
                if (skipRequested)
                {
                    targetText.maxVisibleCharacters = totalVisibleCharacters;
                    break;
                }

                targetText.maxVisibleCharacters = i;

                if (i > 0 && messageData.letterSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(messageData.letterSound, messageData.letterSoundVolume);
                }

                yield return new WaitForSecondsRealtime(messageData.characterDelay);
            }
        }
        else
        {
            targetText.maxVisibleCharacters = totalVisibleCharacters;
        }

        if (!skipRequested && messageData.holdDuration > 0f)
        {
            yield return new WaitForSecondsRealtime(messageData.holdDuration);
        }

        if (messageData.persistAfterDisplay)
        {
            canvasGroup.alpha = 1f;
            targetText.maxVisibleCharacters = totalVisibleCharacters;

            isPlaying = false;
            currentRoutine = null;
            yield break;
        }

        if (messageData.fadeOutDuration > 0f)
        {
            yield return FadeCanvas(canvasGroup.alpha, 0f, messageData.fadeOutDuration);
        }
        else
        {
            canvasGroup.alpha = 0f;
        }

        targetText.text = "";
        targetText.maxVisibleCharacters = 0;

        isPlaying = false;
        currentRoutine = null;
    }

    private IEnumerator FadeCanvas(float from, float to, float duration)
    {
        float timer = 0f;
        canvasGroup.alpha = from;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / duration);
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        canvasGroup.alpha = to;
    }

    public UIMessageData CreateDefaultMessage(string message, Color color)
    {
        UIMessageData data = new UIMessageData
        {
            message = message,
            textColor = color,
            useTypewriter = true,
            startDelay = 0f,
            characterDelay = defaultCharacterDelay,
            fadeInDuration = defaultFadeInDuration,
            holdDuration = 1f,
            fadeOutDuration = defaultFadeOutDuration,
            letterSound = null,
            letterSoundVolume = 1f,
            persistAfterDisplay = false
        };

        return data;
    }
}

[System.Serializable]
public class UIMessageData
{
    [TextArea(2, 4)]
    public string message = "Message";

    public Color textColor = Color.white;

    public bool useTypewriter = true;
    public float startDelay = 0f;
    public float characterDelay = 0.05f;

    public float fadeInDuration = 0.25f;
    public float holdDuration = 1f;
    public float fadeOutDuration = 0.25f;

    public AudioClip letterSound;
    [Range(0f, 1f)] public float letterSoundVolume = 1f;

    public bool persistAfterDisplay = false;
}