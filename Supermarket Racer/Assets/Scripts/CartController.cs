using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

namespace Ezereal
{
    public class CartController : MonoBehaviour
    {
        [Header("References")]
        public Rigidbody vehicleRB;
        public WheelCollider frontLeftWheelCollider;
        public WheelCollider frontRightWheelCollider;
        public WheelCollider rearLeftWheelCollider;
        public WheelCollider rearRightWheelCollider;

        [SerializeField] Transform frontLeftWheelMesh;
        [SerializeField] Transform frontRightWheelMesh;
        [SerializeField] Transform rearLeftWheelMesh;
        [SerializeField] Transform rearRightWheelMesh;

        [SerializeField] Transform steeringWheel;

        [Header("UI (Optional)")]
        [SerializeField] TMP_Text currentSpeedTMP;
        [SerializeField] Slider accelerationSlider;

        [Header("Cart Settings")]
        public float maxSpeed = 30f; // Maximum speed in km/h
        public float acceleration = 500f; // How fast it accelerates
        public float brakePower = 1000f; // Braking strength
        public float maxSteerAngle = 40f; // How much it can turn
        public float steeringSpeed = 5f; // How quickly steering responds
        public float decelerationSpeed = 2f; // How fast it slows down naturally
        public float maxSteeringWheelRotation = 360f;

        [Header("Stability")]
        public float uprightForce = 5f; // How strongly it resists tipping
        public Vector3 centerOfMassOffset = new Vector3(0, -0.5f, 0); // Lower = more stable

        [Header("Debug")]
        [SerializeField] float currentSpeed = 0f;
        [SerializeField] float moveInput = 0f; // Forward/backward input
        [SerializeField] float steerInput = 0f; // Left/right input
        [SerializeField] float brakeInput = 0f;
        [SerializeField] float currentSteerAngle = 0f;

        private void Awake()
        {
            if (vehicleRB == null)
            {
                Debug.LogError("VehicleRB reference is missing!");
            }
            else
            {
                // Lower center of mass for stability
                vehicleRB.centerOfMass = centerOfMassOffset;
            }
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Input System callbacks
        void OnAccelerate(InputValue value)
        {
            moveInput = value.Get<float>();
        }

        void OnBrake(InputValue value)
        {
            brakeInput = value.Get<float>();
        }

        void OnSteer(InputValue value)
        {
            steerInput = value.Get<float>();
        }

        private void FixedUpdate()
        {
            CalculateSpeed();
            HandleMovement();
            HandleBraking();
            HandleSteering();
            NaturalSlowdown();
            KeepUpright();
            UpdateWheelMeshes();
            UpdateUI();
        }

        void CalculateSpeed()
        {
            if (vehicleRB != null)
            {
                // Calculate speed in km/h
#if UNITY_6000_0_OR_NEWER
                currentSpeed = Vector3.Dot(vehicleRB.gameObject.transform.forward, vehicleRB.linearVelocity) * 3.6f;
#else
                currentSpeed = Vector3.Dot(vehicleRB.gameObject.transform.forward, vehicleRB.velocity) * 3.6f;
#endif
            }
        }

        void HandleMovement()
        {
            if (moveInput != 0f)
            {
                // Calculate speed limit based on direction
                float speedLimit = moveInput > 0 ? maxSpeed : -maxSpeed;

                // Check if we're within speed limit
                bool canAccelerate = (moveInput > 0 && currentSpeed < maxSpeed) ||
                                    (moveInput < 0 && currentSpeed > -maxSpeed);

                if (canAccelerate)
                {
                    // Apply motor torque to all wheels (shopping cart = AWD-like)
                    float torque = moveInput * acceleration;

                    frontLeftWheelCollider.motorTorque = torque;
                    frontRightWheelCollider.motorTorque = torque;
                    rearLeftWheelCollider.motorTorque = torque;
                    rearRightWheelCollider.motorTorque = torque;
                }
                else
                {
                    // At max speed, no more torque
                    ZeroMotorTorque();
                }
            }
            else
            {
                ZeroMotorTorque();
            }
        }

        void HandleBraking()
        {
            if (brakeInput > 0f)
            {
                // Apply brakes to front wheels
                frontLeftWheelCollider.brakeTorque = brakeInput * brakePower;
                frontRightWheelCollider.brakeTorque = brakeInput * brakePower;
                rearLeftWheelCollider.brakeTorque = brakeInput * brakePower;
                rearRightWheelCollider.brakeTorque = brakeInput * brakePower;
            }
            else
            {
                // Release brakes
                frontLeftWheelCollider.brakeTorque = 0;
                frontRightWheelCollider.brakeTorque = 0;
                rearLeftWheelCollider.brakeTorque = 0;
                rearRightWheelCollider.brakeTorque = 0;
            }
        }

        void HandleSteering()
        {
            // Calculate target steering angle
            float targetSteerAngle = steerInput * maxSteerAngle;

            // Smoothly interpolate to target angle
            currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteerAngle, Time.deltaTime * steeringSpeed);

            // Apply steering to front wheels
            frontLeftWheelCollider.steerAngle = currentSteerAngle;
            frontRightWheelCollider.steerAngle = currentSteerAngle;

            // Rotate steering wheel visual
            RotateSteeringWheel();
        }

        void NaturalSlowdown()
        {
            // If not accelerating or braking, slow down naturally
            if (moveInput == 0 && brakeInput == 0 && vehicleRB != null)
            {
#if UNITY_6000_0_OR_NEWER
                vehicleRB.linearVelocity = Vector3.Lerp(vehicleRB.linearVelocity, Vector3.zero, Time.deltaTime * decelerationSpeed);
#else
                vehicleRB.velocity = Vector3.Lerp(vehicleRB.velocity, Vector3.zero, Time.deltaTime * decelerationSpeed);
#endif
            }
        }

        void KeepUpright()
        {
            if (vehicleRB != null)
            {
                // Get current rotation
                Quaternion currentRotation = transform.rotation;

                // Create upright rotation (only keep Y rotation for heading)
                Vector3 euler = currentRotation.eulerAngles;
                Quaternion targetRotation = Quaternion.Euler(0, euler.y, 0);

                // Smoothly rotate towards upright
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.fixedDeltaTime * uprightForce);
            }
        }

        void UpdateWheelMeshes()
        {
            UpdateWheel(frontLeftWheelCollider, frontLeftWheelMesh);
            UpdateWheel(frontRightWheelCollider, frontRightWheelMesh);
            UpdateWheel(rearLeftWheelCollider, rearLeftWheelMesh);
            UpdateWheel(rearRightWheelCollider, rearRightWheelMesh);
        }

        void UpdateWheel(WheelCollider col, Transform mesh)
        {
            if (mesh != null)
            {
                col.GetWorldPose(out Vector3 position, out Quaternion rotation);
                mesh.SetPositionAndRotation(position, rotation);
            }
        }

        void RotateSteeringWheel()
        {
            if (steeringWheel != null)
            {
                float currentXAngle = steeringWheel.transform.localEulerAngles.x;

                // Map steer angle to steering wheel rotation
                float normalizedSteerAngle = Mathf.Clamp(currentSteerAngle, -maxSteerAngle, maxSteerAngle);
                float rotation = Mathf.Lerp(maxSteeringWheelRotation, -maxSteeringWheelRotation,
                                           (normalizedSteerAngle + maxSteerAngle) / (2 * maxSteerAngle));

                steeringWheel.localRotation = Quaternion.Euler(currentXAngle, 0, rotation);
            }
        }

        void ZeroMotorTorque()
        {
            frontLeftWheelCollider.motorTorque = 0;
            frontRightWheelCollider.motorTorque = 0;
            rearLeftWheelCollider.motorTorque = 0;
            rearRightWheelCollider.motorTorque = 0;
        }

        void UpdateUI()
        {
            // Update speed display
            if (currentSpeedTMP != null)
            {
                currentSpeedTMP.text = Mathf.Abs(currentSpeed).ToString("F0");
            }

            // Update acceleration slider
            if (accelerationSlider != null)
            {
                accelerationSlider.value = Mathf.Lerp(accelerationSlider.value, Mathf.Abs(moveInput), Time.deltaTime * 15f);
            }
        }
    }
}