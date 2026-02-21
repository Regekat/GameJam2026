using UnityEngine;

public class PlayerPickUpDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickUpLayerMask;
    [SerializeField] private float pickupDistance = 2f;

    private ObjectGrabbable currentGrabbable;
    private Outline currentOutline;

    private void Update()
    {
        HandleHighlight();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentGrabbable == null)
            {
                TryGrab();
            }
            else
            {
                currentGrabbable.Drop();
                currentGrabbable = null;
            }
        }
    }

    void HandleHighlight()
    {
        // If holding something, remove highlight
        if (currentGrabbable != null)
        {
            if (currentOutline != null)
                currentOutline.enabled = false;

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
                Outline outline = hit.transform.GetComponent<Outline>();

                if (outline != null)
                {
                    if (currentOutline != outline)
                    {
                        DisableCurrentOutline();
                        currentOutline = outline;
                        currentOutline.enabled = true;
                    }
                }

                return;
            }
        }

        DisableCurrentOutline();
    }

    void DisableCurrentOutline()
    {
        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
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