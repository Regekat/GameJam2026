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

    private void Update()
    {
        HandleHighlight();
        HandleGrabInput();
    }

    void HandleHighlight()
    {
        // If holding something, remove highlight + UI
        if (currentGrabbable != null)
        {
            DisableCurrentOutline();
            DisableCurrentUI();
            return;
        }

        if (Physics.Raycast(playerCameraTransform.position,
                            playerCameraTransform.forward,
                            out RaycastHit hit,
                            pickupDistance,
                            pickUpLayerMask))
        {
            if (hit.transform.TryGetComponent(out ObjectGrabbable grabbable))
            {
                // --- OUTLINE ---
                Outline outline = hit.transform.GetComponent<Outline>();

                if (outline != null && currentOutline != outline)
                {
                    DisableCurrentOutline();
                    currentOutline = outline;
                    currentOutline.enabled = true;
                }

                // --- UI ---
                ItemUI itemUI = hit.transform.GetComponent<ItemUI>();

                if (itemUI != null && currentItemUI != itemUI)
                {
                    DisableCurrentUI();
                    currentItemUI = itemUI;
                    currentItemUI.Show();
                }

                return;
            }
        }

        DisableCurrentOutline();
        DisableCurrentUI();
    }

    void HandleGrabInput()
    {
        // HOLD to grab
        if (Input.GetMouseButtonDown(0))
        {
            if (currentGrabbable == null)
            {
                TryGrab();
            }
        }

        // RELEASE to drop
        if (Input.GetMouseButtonUp(0))
        {
            if (currentGrabbable != null)
            {
                currentGrabbable.Drop();
                currentGrabbable = null;
            }
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