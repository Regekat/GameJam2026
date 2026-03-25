using UnityEngine;


/// <summary>
/// This script is meant to handle game states: win, loss, playing, paused. All of these functions
/// should eventually pass through here, and this script should be called whenever any of these
/// game states need to be changed or set.
/// </summary>

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [SerializeField] private UIFadeSystem fadeSystem;
    [SerializeField] private UIMessageQueue messageQueue;

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
    }

    public void TriggerWin()
    {
        if (currentState == GameState.Won)
            return;

        if (currentState == GameState.Lost)
        {
            Debug.Log("Win attempt ignored because the game is already lost.");
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
            Debug.Log("Loss attempt ignored because the player already won.");
            return;
        }

        currentState = GameState.Lost;
        Debug.Log("[GameStateManager] GAME LOST");

        HandleLoss();
    }

    private void HandleWin()
    {
        //Do nothing for now. Eventually, show victory UI, disable controls
        fadeSystem.FadeToWhite();
        messageQueue.PlayCheckedOut(Color.black);

    }

    private void HandleLoss()
    {
        //Do nothing for now. Eventually, show game over UI, disable controls
        fadeSystem.FadeToBlack();
        messageQueue.PlayGameOver(Color.red);
    }
}