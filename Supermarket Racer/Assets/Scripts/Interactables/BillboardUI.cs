using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        // Make UI face camera
        transform.forward = cam.forward;
    }
}