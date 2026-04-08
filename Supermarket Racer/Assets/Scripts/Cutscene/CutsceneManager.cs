using UnityEngine;
using UnityEngine.Playables;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [Header("Cutscene")]
    public PlayableDirector cutsceneDirector;

    [Header("Hide During Cutscene — shown after")]
    public GameObject[] objectsToHide;

    [Header("Hide After Cutscene — hidden after")]
    public GameObject[] objectsToHideAfter;

    [Header("Player Control")]
    public MonoBehaviour playerMovementScript;
    public MonoBehaviour playerLookScript;

    [Header("Skip")]
    public KeyCode skipKey = KeyCode.Space;
    public GameObject skipPromptUI;
    public float holdDuration = 1.5f;       // how long to hold before skip triggers
    public CanvasGroup skipFadeCanvasGroup;  // a black fullscreen CanvasGroup for the fade

    private bool cutsceneFinished = false;
    private float holdTimer = 0f;
    private bool isHolding = false;
    private Coroutine fadeCoroutine;

    void Start()
    {
        if (skipFadeCanvasGroup != null)
        {
            skipFadeCanvasGroup.gameObject.SetActive(true);
            skipFadeCanvasGroup.alpha = 0f;
        }

        StartCoroutine(PlayCutsceneThenStart());
    }

    void Update()
    {
        if (cutsceneFinished) return;

        if (Input.GetKey(skipKey))
        {
            isHolding = true;
            holdTimer += Time.deltaTime;

            // Fade in as player holds
            float progress = Mathf.Clamp01(holdTimer / holdDuration);
            if (skipFadeCanvasGroup != null)
                skipFadeCanvasGroup.alpha = progress;

            if (holdTimer >= holdDuration)
                SkipCutscene();
        }
        else if (isHolding)
        {
            // Released early — reset
            isHolding = false;
            holdTimer = 0f;

            // Fade back out immediately
            if (skipFadeCanvasGroup != null)
                skipFadeCanvasGroup.alpha = 0f;
        }
    }

    IEnumerator PlayCutsceneThenStart()
    {
        SetActive(objectsToHide, false);
        SetActive(objectsToHideAfter, true);

        if (skipPromptUI != null)
            skipPromptUI.SetActive(true);

        DisablePlayerControl(true);

        cutsceneDirector.Play();

        yield return new WaitUntil(() =>
            cutsceneFinished ||
            cutsceneDirector.state != PlayState.Playing);

        EndCutscene();
    }

    void SkipCutscene()
    {
        cutsceneFinished = true;
        cutsceneDirector.Stop();
    }

    void EndCutscene()
    {
        cutsceneFinished = true;

        // Keep screen black during the swap
        if (skipFadeCanvasGroup != null)
            skipFadeCanvasGroup.alpha = 1f;

        SetActive(objectsToHide, true);
        SetActive(objectsToHideAfter, false);

        if (skipPromptUI != null)
            skipPromptUI.SetActive(false);

        DisablePlayerControl(false);

        // Always fade out to gameplay, whether skipped or finished naturally
        StartCoroutine(FadeOutAfterSkip());
    }

    IEnumerator FadeOutAfterSkip()
    {
        if (skipFadeCanvasGroup == null) yield break;

        float elapsed = 0f;
        float fadeDuration = 0.5f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            skipFadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        skipFadeCanvasGroup.alpha = 0f;
    }

    void SetActive(GameObject[] objects, bool active)
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null)
                obj.SetActive(active);
        }
    }

    void DisablePlayerControl(bool disabled)
    {
        if (playerMovementScript != null)
            playerMovementScript.enabled = !disabled;

        if (playerLookScript != null)
            playerLookScript.enabled = !disabled;
    }
}