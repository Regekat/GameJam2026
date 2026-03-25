using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Loading")]
    [SerializeField] private string gameplaySceneName;
    [SerializeField] private UIFadeSystem fadeSystem;
    [SerializeField] private float sceneLoadDelay = 1f;

    [Header("Menu State")]
    [SerializeField] private bool freezeTimeOnStart = true;
    [SerializeField] private bool forceCursorVisible = true;

    private bool isLoadingScene = false;

    private void Awake()
    {
        if (freezeTimeOnStart)
        {
            Time.timeScale = 0f;
        }

        ForceCursorState();
    }

    private void Start()
    {
        ForceCursorState();
    }

    private void Update()
    {
        if (forceCursorVisible)
        {
            ForceCursorState();
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    private void ForceCursorState()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnPlayButtonPressed()
    {
        if (isLoadingScene)
            return;

        if (string.IsNullOrWhiteSpace(gameplaySceneName))
        {
            Debug.LogError("[MainMenuManager] Gameplay scene name is not assigned.");
            return;
        }

        StartCoroutine(PlayAndLoadRoutine());
    }

    public void OnQuitButtonPressed()
    {
        Application.Quit();
        Debug.Log("[MainMenuManager] Quit requested.");
    }

    private IEnumerator PlayAndLoadRoutine()
    {
        isLoadingScene = true;

        ForceCursorState();

        if (fadeSystem != null)
        {
            fadeSystem.FadeToBlack(sceneLoadDelay);
        }
        else
        {
            Debug.LogWarning("[MainMenuManager] No UIFadeSystem assigned. Loading scene without fade.");
        }

        yield return new WaitForSecondsRealtime(sceneLoadDelay);

        Time.timeScale = 1f;
        SceneManager.LoadScene(gameplaySceneName);
    }
}