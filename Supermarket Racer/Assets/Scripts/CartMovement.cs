using UnityEngine;
using UnityEngine.InputSystem;

public class CartMovement : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Get keyboard input from new Input System
        Keyboard kb = Keyboard.current;

        if (kb == null) return; // Safety check

        if (kb.wKey.isPressed)
        {
            rb.AddForce(new Vector3(0, 0, 5));
        }

        if (kb.sKey.isPressed)
        {
            rb.AddForce(new Vector3(0, 0, -5));
        }

        if (kb.aKey.isPressed)
        {
            rb.AddForce(new Vector3(-5, 0, 0));
        }

        if (kb.dKey.isPressed)
        {
            rb.AddForce(new Vector3(5, 0, 0));
        }
    }
}