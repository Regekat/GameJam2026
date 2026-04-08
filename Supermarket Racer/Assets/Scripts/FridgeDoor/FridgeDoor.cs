using UnityEngine;

public class FridgeDoor : MonoBehaviour
{
    private Animator animator;
    private Outline outline;
    private bool isOpen = false;

    private static readonly int OpenHash = Animator.StringToHash("Open");
    private static readonly int CloseHash = Animator.StringToHash("Close");

    void Start()
    {
        animator = GetComponent<Animator>();
        outline = GetComponent<Outline>();

        if (outline != null)
            outline.enabled = false;
    }

    public void SetOutline(bool enabled)
    {
        if (outline != null)
            outline.enabled = enabled;
    }

    public void TryToggle()
    {
        if (isOpen)
        {
            Close();
        }
        else
        {
            // Ask manager if allowed to open
            FridgeManager.Instance.RequestOpen(this);
        }
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;
        animator.SetTrigger(OpenHash);
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayFridgeDoorOpenSound();
    }

    public void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        animator.SetTrigger(CloseHash);
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayFridgeDoorCloseSound();
        FridgeManager.Instance.NotifyClosed(this);
    }

    public bool IsOpen => isOpen;
}