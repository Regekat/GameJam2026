using UnityEngine;

public class UIFollow : MonoBehaviour
{
    public Transform target;
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (target == null || cam == null) return;

        // Follow position only
        transform.position = target.position;

        // Face camera
        transform.forward = cam.forward;
    }
}