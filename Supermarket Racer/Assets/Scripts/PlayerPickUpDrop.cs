using UnityEngine;

public class PlayerPickUpDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickUpLayerMask;

    private ObjectGrabbable ObjectGrabbable;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (ObjectGrabbable == null)
            {
                float pickupDistance = 2f; ;
                if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickupDistance, pickUpLayerMask))

                    if (raycastHit.transform.TryGetComponent(out ObjectGrabbable objectGrabbable))
                    {
                        objectGrabbable.Grab(objectGrabPointTransform);
                        ObjectGrabbable = objectGrabbable;
                    }
            }
            else
            {
                ObjectGrabbable.Drop();
                ObjectGrabbable = null;
            }

        }
    }
}

