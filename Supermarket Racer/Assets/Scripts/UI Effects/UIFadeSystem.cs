using System.Collections;
using UnityEngine;

public class UIFadeSystem : MonoBehaviour
{
    [Header("Fade Panels")]
    [SerializeField] private GameObject blackPanelObject;
    [SerializeField] private GameObject whitePanelObject;

    [SerializeField] private CanvasGroup blackCanvasGroup;
    [SerializeField] private CanvasGroup whiteCanvasGroup;

    [Header("Startup Overlay")]
    [SerializeField] private GameObject startupOverlayPanel;

    [Header("Fade Settings")]
    [SerializeField] private float defaultFadeDuration = 1f;

    private Coroutine currentFadeCoroutine;

    private void Awake()
    {
        SetupPanel(blackPanelObject, blackCanvasGroup);
        SetupPanel(whitePanelObject, whiteCanvasGroup);
    }

    private void SetupPanel(GameObject panelObject, CanvasGroup canvasGroup)
    {
        if (panelObject == null)
        {
            Debug.LogError("[UIFadeSystem] A panel GameObject is missing.");
            return;
        }

        if (canvasGroup == null)
        {
            Debug.LogError($"[UIFadeSystem] CanvasGroup is missing on panel: {panelObject.name}");
            return;
        }

        canvasGroup.alpha = 1f;
        panelObject.SetActive(false);
    }

    public void DisableStartupOverlay()
    {
        if (startupOverlayPanel == null)
        {
            Debug.LogWarning("[UIFadeSystem] startupOverlayPanel is not assigned.");
            return;
        }

        startupOverlayPanel.SetActive(false);
    }

    public void FadeToBlack()
    {
        FadeToBlack(defaultFadeDuration);
    }

    public void FadeFromBlack()
    {
        FadeFromBlack(defaultFadeDuration);
    }

    public void FadeToWhite()
    {
        FadeToWhite(defaultFadeDuration);
    }

    public void FadeFromWhite()
    {
        FadeFromWhite(defaultFadeDuration);
    }

    public void FadeToBlack(float duration)
    {
        StartNewFade(FadeTo(blackPanelObject, blackCanvasGroup, duration));
    }

    public void FadeFromBlack(float duration)
    {
        StartNewFade(FadeFrom(blackPanelObject, blackCanvasGroup, duration, "black"));
    }

    public void FadeToWhite(float duration)
    {
        StartNewFade(FadeTo(whitePanelObject, whiteCanvasGroup, duration));
    }

    public void FadeFromWhite(float duration)
    {
        StartNewFade(FadeFrom(whitePanelObject, whiteCanvasGroup, duration, "white"));
    }

    private void StartNewFade(IEnumerator fadeCoroutine)
    {
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = StartCoroutine(fadeCoroutine);
    }

    private IEnumerator FadeTo(GameObject panelObject, CanvasGroup canvasGroup, float duration)
    {
        if (panelObject == null || canvasGroup == null)
        {
            yield break;
        }

        canvasGroup.alpha = 0f;
        panelObject.SetActive(true);

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / duration;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        currentFadeCoroutine = null;
    }

    private IEnumerator FadeFrom(GameObject panelObject, CanvasGroup canvasGroup, float duration, string colourName)
    {
        if (panelObject == null || canvasGroup == null)
        {
            yield break;
        }

        if (!panelObject.activeSelf)
        {
            Debug.LogWarning($"[UIFadeSystem] Cannot fade from {colourName} because the panel is disabled.");
            yield break;
        }

        canvasGroup.alpha = 1f;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / duration;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        panelObject.SetActive(false);
        currentFadeCoroutine = null;
    }
}