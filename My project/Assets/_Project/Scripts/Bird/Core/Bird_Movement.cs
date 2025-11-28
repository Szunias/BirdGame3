using BirdGame.Core.Data;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using Unity.Cinemachine;

namespace BirdGame.Bird.Core
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Bird_Input))]
    public class Bird_Movement : NetworkBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private Data_BirdStats stats;
        [SerializeField] private CinemachineCamera playerCamera;

        [Header("Events")]
        public UnityEvent OnTakeoff;
        public UnityEvent OnLand;

        private CharacterController _controller;
        private Bird_Input _input;
        private Vector3 _velocity;
        private Vector2 _moveInput;

        private bool _isFlying;
        private bool _isSpaceHeld;
        private float _jumpHoldTime;

        private Transform _cameraTransform;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<Bird_Input>();
            if (playerCamera == null) playerCamera = GetComponentInChildren<CinemachineCamera>();
        }

        public override void OnNetworkSpawn()
        {
            if (Camera.main != null) _cameraTransform = Camera.main.transform;

            if (IsOwner)
            {
                if (playerCamera != null)
                {
                    playerCamera.gameObject.SetActive(true);
                    playerCamera.Follow = transform;
                    playerCamera.LookAt = transform;
                }
                SubscribeInput();
            }
            else
            {
                if (playerCamera != null) playerCamera.gameObject.SetActive(false);
                enabled = false;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner) UnsubscribeInput();
        }

        private void SubscribeInput()
        {
            _input.OnMoveInput.AddListener(OnMove);
            _input.OnJumpPressed.AddListener(OnJumpPressed);
            _input.OnJumpReleased.AddListener(OnJumpReleased);
        }

        private void UnsubscribeInput()
        {
            _input.OnMoveInput.RemoveListener(OnMove);
            _input.OnJumpPressed.RemoveListener(OnJumpPressed);
            _input.OnJumpReleased.RemoveListener(OnJumpReleased);
        }

        private void OnMove(Vector2 input) => _moveInput = input;

        private void OnJumpPressed()
        {
            _isSpaceHeld = true;
            _jumpHoldTime = 0f;

            // Normal Jump immediately on press if grounded
            if (_controller.isGrounded && stats != null)
            {
                _velocity.y = Mathf.Sqrt(2f * stats.gravity * stats.jumpHeight);
            }
        }

        private void OnJumpReleased()
        {
            _isSpaceHeld = false;
            _jumpHoldTime = 0f;

            // Stop flying immediately if space released
            if (_isFlying)
            {
                StopFlight();
            }
        }

        private void Update()
        {
            if (!IsOwner || stats == null) return;

            CheckFlightStart();
            ApplyGravity();
            ApplyMovement();

            _controller.Move(_velocity * Time.deltaTime);
        }

        private void CheckFlightStart()
        {
            // Allow flight start from ground or air - just hold space long enough
            if (stats.canFly && _isSpaceHeld && !_isFlying)
            {
                _jumpHoldTime += Time.deltaTime;
                if (_jumpHoldTime >= stats.flightHoldThreshold)
                {
                    StartFlight();
                }
            }
        }

        private void ApplyMovement()
        {
            if (_cameraTransform == null) return;

            Vector3 moveDir = Vector3.zero;

            // Camera Vectors (always flatten for WASD logic relative to camera look)
            Vector3 camForward = _cameraTransform.forward;
            Vector3 camRight = _cameraTransform.right;

            if (_isFlying)
            {
                // --- FLIGHT ---
                // Use full 3D camera vector for "W" to allow flying UP/DOWN by looking
                moveDir = (camForward * _moveInput.y + camRight * _moveInput.x).normalized;

                if (_moveInput.sqrMagnitude > 0.01f)
                {
                    // Determine speed based on vertical direction
                    float speed = stats.flightSpeed;
                    if (moveDir.y > 0.3f) speed = stats.climbSpeed;      // Flying up
                    else if (moveDir.y < -0.3f) speed = stats.diveSpeed; // Flying down

                    Vector3 targetVelocity = moveDir * speed;
                    _velocity = Vector3.MoveTowards(_velocity, targetVelocity, stats.acceleration * Time.deltaTime);

                    // Rotate to face move direction
                    Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, stats.flightRotationSpeed * Time.deltaTime);
                }
                else
                {
                    // Hover / Slow down horizontally, maintain Y for flightGravity effect
                    Vector3 hoverTarget = new Vector3(0f, _velocity.y, 0f);
                    _velocity = Vector3.MoveTowards(_velocity, hoverTarget, stats.acceleration * Time.deltaTime);
                }
            }
            else
            {
                // --- GROUND ---
                // Flatten vectors for ground movement
                camForward.y = 0; camForward.Normalize();
                camRight.y = 0; camRight.Normalize();

                moveDir = (camForward * _moveInput.y + camRight * _moveInput.x).normalized;

                float currentY = _velocity.y;
                Vector3 targetHorizontal = moveDir * stats.groundSpeed;
                Vector3 currentHorizontal = new Vector3(_velocity.x, 0, _velocity.z);

                Vector3 newHorizontal = Vector3.MoveTowards(currentHorizontal, targetHorizontal, stats.acceleration * Time.deltaTime);
                _velocity = new Vector3(newHorizontal.x, currentY, newHorizontal.z);

                if (moveDir.sqrMagnitude > 0.01f)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), stats.groundRotationSpeed * Time.deltaTime);
                }
            }
        }

        private void ApplyGravity()
        {
            if (_isFlying)
            {
                // Apply flight gravity (usually 0 for full control, or small value for gradual sink)
                _velocity.y -= stats.flightGravity * Time.deltaTime;
            }
            else
            {
                // Falling / Grounded
                if (_controller.isGrounded && _velocity.y < 0)
                {
                    // Small downward force to keep CharacterController grounded on slopes
                    _velocity.y = -2f;
                }
                else
                {
                    _velocity.y -= stats.gravity * Time.deltaTime;
                }
            }
        }

        private void StartFlight()
        {
            _isFlying = true;
            // Small boost to indicate flight start
            _velocity.y = Mathf.Max(_velocity.y, stats.takeoffBoost);
            OnTakeoff?.Invoke();
        }

        private void StopFlight()
        {
            _isFlying = false;
            OnLand?.Invoke();
        }

        public void SetStats(Data_BirdStats newStats) => stats = newStats;
    }
}
