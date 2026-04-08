using UnityEngine;

namespace Ezereal
{
    public class Restart : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody cartRigidbody;
        [SerializeField] private Transform fallbackResetPoint;

        [Header("Reset Settings")]
        [SerializeField] private float liftAmount = 1.0f;
        [SerializeField] private bool preserveCurrentYaw = false;

        private void Awake()
        {
            if (cartRigidbody == null)
            {
                cartRigidbody = GetComponent<Rigidbody>();
            }
        }

        public void OnRestart()
        {
            if (cartRigidbody == null)
            {
                Debug.LogWarning("Restart: No Rigidbody assigned.");
                return;
            }

            if (fallbackResetPoint == null)
            {
                Debug.LogWarning("Restart: No fallback reset point assigned.");
                return;
            }

            Vector3 targetPosition = fallbackResetPoint.position + Vector3.up * liftAmount;
            Quaternion targetRotation;

            if (preserveCurrentYaw)
            {
                Vector3 flatForward = transform.forward;
                flatForward.y = 0f;

                if (flatForward.sqrMagnitude < 0.001f)
                {
                    flatForward = fallbackResetPoint.forward;
                    flatForward.y = 0f;
                }

                if (flatForward.sqrMagnitude < 0.001f)
                {
                    flatForward = Vector3.forward;
                }

                targetRotation = Quaternion.LookRotation(flatForward.normalized, Vector3.up);
            }
            else
            {
                Vector3 flatForward = fallbackResetPoint.forward;
                flatForward.y = 0f;

                if (flatForward.sqrMagnitude < 0.001f)
                {
                    flatForward = Vector3.forward;
                }

                targetRotation = Quaternion.LookRotation(flatForward.normalized, Vector3.up);
            }

            cartRigidbody.linearVelocity = Vector3.zero;
            cartRigidbody.angularVelocity = Vector3.zero;

            cartRigidbody.position = targetPosition;
            cartRigidbody.rotation = targetRotation;

            cartRigidbody.Sleep();
            cartRigidbody.WakeUp();
        }
    }
}