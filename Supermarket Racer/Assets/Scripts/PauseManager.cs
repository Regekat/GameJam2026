using System;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    public static event Action<bool> OnPauseStateChanged;

    [Header("Debug")]
    [SerializeField] private bool logPauseChanges = true;
    [SerializeField] private bool enableDebugKeybinds = true;

    private bool isPaused;

    public bool IsPaused => isPaused;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        HandleDebugKeybinds();
    }

    private void HandleDebugKeybinds()
    {
        if (!enableDebugKeybinds)
            return;

        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            UnpauseGame();
        }
    }

    public void PauseGame()
    {
        if (isPaused)
            return;

        isPaused = true;
        Time.timeScale = 0f;

        if (logPauseChanges)
        {
            Debug.Log("[PauseManager] Game paused.");
        }

        OnPauseStateChanged?.Invoke(true);
    }

    public void UnpauseGame()
    {
        if (!isPaused)
            return;

        isPaused = false;
        Time.timeScale = 1f;

        if (logPauseChanges)
        {
            Debug.Log("[PauseManager] Game unpaused.");
        }

        OnPauseStateChanged?.Invoke(false);
    }

    public void SetPaused(bool pause)
    {
        if (pause)
        {
            PauseGame();
        }
        else
        {
            UnpauseGame();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Time.timeScale = 1f;
        }
    }
}