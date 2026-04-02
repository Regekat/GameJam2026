using UnityEngine;

public class FridgeManager : MonoBehaviour
{
    public static FridgeManager Instance { get; private set; }

    private FridgeDoor currentlyOpenDoor = null;

    void Awake()
    {
        Instance = this;
    }

    public void RequestOpen(FridgeDoor requestingDoor)
    {
        // Close the currently open door first
        if (currentlyOpenDoor != null && currentlyOpenDoor != requestingDoor)
            currentlyOpenDoor.Close();

        currentlyOpenDoor = requestingDoor;
        requestingDoor.Open();
    }

    public void NotifyClosed(FridgeDoor door)
    {
        if (currentlyOpenDoor == door)
            currentlyOpenDoor = null;
    }
}