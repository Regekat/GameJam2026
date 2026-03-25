using System.Collections;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [SerializeField] private UIFadeSystem fadeSystem;
    [SerializeField] private UIMessageQueue messageQueue;
    [SerializeField] private PauseManager pauseManager;

    [Header("End Screen Buttons")]
    [SerializeField] private GameObject restartButtonObject;
    [SerializeField] private GameObject mainMenuButtonObject;

    [Header("Debug Testing")]
    [SerializeField] private bool enableDebugStateTesting = true;

    [Header("Opening Countdown")]
    [SerializeField] private bool runOpeningCountdownOnStart = true;

    public enum GameState
    {
        Playing,
        Won,
        Lost
    }

    [SerializeField] private GameState currentState = GameState.Playing;

    public GameState CurrentState => currentState;
    public bool HasGameEnded => currentState != GameState.Playing;
    public bool HasPlayerWon => currentState == GameState.Won;
    public bool HasPlayerLost => currentState == GameState.Lost;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        SetEndButtonsActive(false);
        LockGameplayCursor();
    }

    private void OnEnable()
    {
        if (messageQueue != null)
        {
            messageQueue.OnQueueFinished += HandleMessageQueueFinished;
        }
    }

    private void OnDisable()
    {
        if (messageQueue != null)
        {
            messageQueue.OnQueueFinished -= HandleMessageQueueFinished;
        }
    }

    private void Start()
    {
        SetEndButtonsActive(false);

        if (runOpeningCountdownOnStart)
        {
            StartCoroutine(OpeningCountdownRoutine());
        }
    }

    private void Update()
    {
        HandleDebugStateTesting();
    }

    private IEnumerator OpeningCountdownRoutine()
    {
        if (pauseManager == null)
        {
            Debug.LogWarning("[GameStateManager] pauseManager is not assigned.");
            yield break;
        }

        if (messageQueue == null)
        {
            Debug.LogWarning("[GameStateManager] messageQueue is not assigned.");
            yield break;
        }

        if (fadeSystem == null)
        {
            Debug.LogWarning("[GameStateManager] fadeSystem is not assigned.");
            yield break;
        }

        pauseManager.PauseGame();
        messageQueue.PlayCountdown();

        while (messageQueue.IsProcessingQueue)
        {
            yield return null;
        }

        fadeSystem.DisableStartupOverlay();
        pauseManager.UnpauseGame();
    }

    private void HandleDebugStateTesting()
    {
        if (!enableDebugStateTesting)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("[GameStateManager] Debug trigger: WIN");
            TriggerWin();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("[GameStateManager] Debug trigger: LOSS");
            TriggerLoss();
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log("[GameStateManager] Debug trigger: RESET TO PLAYING");
            ResetStateToPlaying();
        }
    }

    public void TriggerWin()
    {
        if (currentState == GameState.Won)
            return;

        if (currentState == GameState.Lost)
        {
            Debug.Log("[GameStateManager] Win ignored because game is already lost.");
            return;
        }

        currentState = GameState.Won;
        Debug.Log("[GameStateManager] GAME WON");

        HandleWin();
    }

    public void TriggerLoss()
    {
        if (currentState == GameState.Lost)
            return;

        if (currentState == GameState.Won)
        {
            Debug.Log("[GameStateManager] Loss ignored because game is already won.");
            return;
        }

        currentState = GameState.Lost;
        Debug.Log("[GameStateManager] GAME LOST");

        HandleLoss();
    }

    public void ResetStateToPlaying()
    {
        currentState = GameState.Playing;
        SetEndButtonsActive(false);
        LockGameplayCursor();
        Debug.Log("[GameStateManager] State reset to PLAYING");
    }

    public void ResetGameState()
    {
        currentState = GameState.Playing;
        SetEndButtonsActive(false);
        LockGameplayCursor();

        if (pauseManager != null)
        {
            pauseManager.UnpauseGame();
        }

        Debug.Log("[GameStateManager] Reset to Playing.");
    }

    private void HandleWin()
    {
        SetEndButtonsActive(false);

        if (fadeSystem != null)
        {
            fadeSystem.FadeToWhite();
        }
        else
        {
            Debug.LogWarning("[GameStateManager] fadeSystem is not assigned.");
        }

        if (messageQueue != null)
        {
            messageQueue.PlayCheckedOut(Color.black);
        }
        else
        {
            Debug.LogWarning("[GameStateManager] messageQueue is not assigned.");
        }
    }

    private void HandleLoss()
    {
        SetEndButtonsActive(false);

        if (fadeSystem != null)
        {
            fadeSystem.FadeToBlack();
        }
        else
        {
            Debug.LogWarning("[GameStateManager] fadeSystem is not assigned.");
        }

        if (messageQueue != null)
        {
            messageQueue.PlayGameOver(Color.red);
        }
        else
        {
            Debug.LogWarning("[GameStateManager] messageQueue is not assigned.");
        }
    }

    private void HandleMessageQueueFinished()
    {
        if (currentState == GameState.Won || currentState == GameState.Lost)
        {
            ShowEndScreenButtons();
        }
    }

    private void ShowEndScreenButtons()
    {
        UnlockMenuCursor();
        SetEndButtonsActive(true);
    }

    private void SetEndButtonsActive(bool active)
    {
        if (restartButtonObject != null)
        {
            restartButtonObject.SetActive(active);
        }

        if (mainMenuButtonObject != null)
        {
            mainMenuButtonObject.SetActive(active);
        }
    }

    private void UnlockMenuCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void LockGameplayCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}