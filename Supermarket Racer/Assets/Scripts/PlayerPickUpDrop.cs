using UnityEngine;

public class PlayerPickUpDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickUpLayerMask;
    [SerializeField] private float pickupDistance = 2f;

    private ObjectGrabbable currentGrabbable;
    private Outline currentOutline;
    private ItemUI currentItemUI;
    private FridgeDoor currentDoor;

    private void Update()
    {
        HandleHighlight();
        HandleGrabInput();
    }

    void HandleHighlight()
    {
        if (currentGrabbable != null)
        {
            DisableCurrentOutline();
            DisableCurrentUI();
            ClearDoorHighlight();
            return;
        }

        if (Physics.Raycast(playerCameraTransform.position,
                            playerCameraTransform.forward,
                            out RaycastHit hit,
                            pickupDistance))
        {
            // Hit a grabbable
            if (hit.transform.TryGetComponent(out ObjectGrabbable grabbable))
            {
                ClearDoorHighlight();

                Outline outline = hit.transform.GetComponent<Outline>();
                if (outline != null && currentOutline != outline)
                {
                    DisableCurrentOutline();
                    currentOutline = outline;
                    currentOutline.enabled = true;
                }

                ItemUI itemUI = hit.transform.GetComponent<ItemUI>();
                if (itemUI != null && currentItemUI != itemUI)
                {
                    DisableCurrentUI();
                    currentItemUI = itemUI;
                    currentItemUI.Show();
                }
                return;
            }

            // Hit a door
            if (hit.transform.TryGetComponent(out FridgeDoor door))
            {
                if (currentDoor != door)
                {
                    ClearDoorHighlight();
                    currentDoor = door;
                    currentDoor.SetOutline(true);
                }

                DisableCurrentOutline();
                DisableCurrentUI();
                return;
            }
        }

        DisableCurrentOutline();
        DisableCurrentUI();
        ClearDoorHighlight();
    }

    void HandleGrabInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (currentGrabbable == null)
            {
                if (currentDoor != null)
                {
                    currentDoor.TryToggle();
                    return;
                }
                TryGrab();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currentGrabbable != null)
            {
                currentGrabbable.Drop();
                currentGrabbable = null;
            }
        }
    }

    void ClearDoorHighlight()
    {
        if (currentDoor != null)
        {
            currentDoor.SetOutline(false);
            currentDoor = null;
        }
    }

    void DisableCurrentOutline()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }
    }

    void DisableCurrentUI()
    {
        if (currentItemUI != null)
        {
            currentItemUI.Hide();
            currentItemUI = null;
        }
    }

    void TryGrab()
    {
        if (Physics.Raycast(playerCameraTransform.position,
                            playerCameraTransform.forward,
                            out RaycastHit hit,
                            pickupDistance,
                            pickUpLayerMask))
        {
            if (hit.transform.TryGetComponent(out ObjectGrabbable grabbable))
            {
                grabbable.Grab(objectGrabPointTransform);
                currentGrabbable = grabbable;
                DisableCurrentOutline();
            }
        }
    }
}