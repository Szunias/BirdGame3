using BirdGame.Core.Data;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using Unity.Cinemachine;

namespace BirdGame.Bird.Core
{
    /// <summary>
    /// Handles bird movement including ground and flight mechanics.
    /// Single Responsibility: Player movement and physics.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Bird_Input))]
    public class Bird_Movement : NetworkBehaviour
    {
        #region Constants
        private const float GROUNDED_VELOCITY = -2f;
        private const float MOVEMENT_THRESHOLD = 0.01f;
        private const float JUMP_GRAVITY_MULTIPLIER = 2f;
        #endregion

        #region Serialized Fields
        [Header("Configuration")]
        [SerializeField] private Data_BirdStats stats;
        [SerializeField] private CinemachineCamera playerCamera;

        [Header("Events")]
        public UnityEvent OnTakeoff;
        public UnityEvent OnLand;
        #endregion

        #region Private Fields
        private CharacterController _controller;
        private Bird_Input _input;
        private Vector3 _velocity;
        private Vector2 _moveInput;

        private bool _isFlying;
        private bool _isSpaceHeld;
        private bool _isDiveHeld;
        private float _jumpHoldTime;

        private Transform _cameraTransform;
        #endregion

        #region Unity Lifecycle
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

        private void Update()
        {
            if (!IsOwner || stats == null) return;

            CheckFlightStart();
            ApplyGravity();
            ApplyMovement();

            _controller.Move(_velocity * Time.deltaTime);
        }
        #endregion

        #region Input Subscription
        private void SubscribeInput()
        {
            _input.OnMoveInput.AddListener(OnMove);
            _input.OnJumpPressed.AddListener(OnJumpPressed);
            _input.OnJumpReleased.AddListener(OnJumpReleased);
            _input.OnDivePressed.AddListener(OnDivePressed);
            _input.OnDiveReleased.AddListener(OnDiveReleased);
        }

        private void UnsubscribeInput()
        {
            _input.OnMoveInput.RemoveListener(OnMove);
            _input.OnJumpPressed.RemoveListener(OnJumpPressed);
            _input.OnJumpReleased.RemoveListener(OnJumpReleased);
            _input.OnDivePressed.RemoveListener(OnDivePressed);
            _input.OnDiveReleased.RemoveListener(OnDiveReleased);
        }

        private void OnMove(Vector2 input) => _moveInput = input;

        private void OnJumpPressed()
        {
            _isSpaceHeld = true;
            _jumpHoldTime = 0f;

            // Normal Jump immediately on press if grounded
            if (_controller.isGrounded && stats != null)
            {
                _velocity.y = Mathf.Sqrt(JUMP_GRAVITY_MULTIPLIER * stats.gravity * stats.jumpHeight);
            }
        }

        private void OnJumpReleased()
        {
            _isSpaceHeld = false;
            _jumpHoldTime = 0f;

            // Stop flying immediately if space released (unless diving)
            if (_isFlying && !_isDiveHeld)
            {
                StopFlight();
            }
        }

        private void OnDivePressed()
        {
            _isDiveHeld = true;
        }

        private void OnDiveReleased()
        {
            _isDiveHeld = false;

            // Stop flying if neither space nor dive is held
            if (_isFlying && !_isSpaceHeld)
            {
                StopFlight();
            }
        }
        #endregion

        #region Flight Logic
        private void CheckFlightStart()
        {
            // Allow flight start from ground or air - just hold space long enough
            if (stats.canFly && (_isSpaceHeld || _isDiveHeld) && !_isFlying)
            {
                _jumpHoldTime += Time.deltaTime;
                if (_jumpHoldTime >= stats.flightHoldThreshold)
                {
                    StartFlight();
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
        #endregion

        #region Movement
        private void ApplyMovement()
        {
            if (_cameraTransform == null) return;

            // Camera Vectors (always flatten for WASD logic relative to camera look)
            Vector3 camForward = _cameraTransform.forward;
            Vector3 camRight = _cameraTransform.right;

            if (_isFlying)
            {
                ApplyFlightMovement(camForward, camRight);
            }
            else
            {
                ApplyGroundMovement(camForward, camRight);
            }
        }

        private void ApplyFlightMovement(Vector3 camForward, Vector3 camRight)
        {
            // Flatten camera vectors for horizontal movement (WASD = horizontal only)
            Vector3 flatForward = camForward;
            flatForward.y = 0;
            flatForward.Normalize();
            Vector3 flatRight = camRight;
            flatRight.y = 0;
            flatRight.Normalize();

            // Horizontal movement from WASD
            Vector3 horizontalDir = (flatForward * _moveInput.y + flatRight * _moveInput.x).normalized;

            // Calculate target horizontal velocity
            Vector3 targetHorizontal = horizontalDir * stats.flightSpeed;

            // Smoothly move towards target horizontal velocity
            float currentY = _velocity.y;
            Vector3 currentHorizontal = new Vector3(_velocity.x, 0, _velocity.z);
            Vector3 newHorizontal = Vector3.MoveTowards(currentHorizontal, targetHorizontal, stats.acceleration * Time.deltaTime);

            _velocity = new Vector3(newHorizontal.x, currentY, newHorizontal.z);

            // Rotate to face movement direction
            if (horizontalDir.sqrMagnitude > MOVEMENT_THRESHOLD)
            {
                Quaternion targetRotation = Quaternion.LookRotation(horizontalDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, stats.flightRotationSpeed * Time.deltaTime);
            }
        }

        private void ApplyGroundMovement(Vector3 camForward, Vector3 camRight)
        {
            // Flatten vectors for ground movement
            camForward.y = 0; camForward.Normalize();
            camRight.y = 0; camRight.Normalize();

            Vector3 moveDir = (camForward * _moveInput.y + camRight * _moveInput.x).normalized;

            float currentY = _velocity.y;
            Vector3 targetHorizontal = moveDir * stats.groundSpeed;
            Vector3 currentHorizontal = new Vector3(_velocity.x, 0, _velocity.z);

            Vector3 newHorizontal = Vector3.MoveTowards(currentHorizontal, targetHorizontal, stats.acceleration * Time.deltaTime);
            _velocity = new Vector3(newHorizontal.x, currentY, newHorizontal.z);

            if (moveDir.sqrMagnitude > MOVEMENT_THRESHOLD)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), stats.groundRotationSpeed * Time.deltaTime);
            }
        }
        #endregion

        #region Physics
        private void ApplyGravity()
        {
            if (_isFlying)
            {
                // Vertical flight control
                if (_isSpaceHeld && !_isDiveHeld)
                {
                    // Climb up - accelerate towards climbSpeed
                    _velocity.y = Mathf.MoveTowards(_velocity.y, stats.climbSpeed, stats.acceleration * Time.deltaTime);
                }
                else if (_isDiveHeld && !_isSpaceHeld)
                {
                    // Dive down - accelerate towards negative diveSpeed
                    _velocity.y = Mathf.MoveTowards(_velocity.y, -stats.diveSpeed, stats.acceleration * Time.deltaTime);
                }
                else if (_isSpaceHeld && _isDiveHeld)
                {
                    // Both held - maintain altitude (hover)
                    _velocity.y = Mathf.MoveTowards(_velocity.y, 0f, stats.acceleration * Time.deltaTime);
                }
                else
                {
                    // Neither held - apply flight gravity (gentle descent or hover based on flightGravity value)
                    _velocity.y -= stats.flightGravity * Time.deltaTime;
                }
            }
            else
            {
                // Falling / Grounded
                if (_controller.isGrounded && _velocity.y < 0)
                {
                    // Small downward force to keep CharacterController grounded on slopes
                    _velocity.y = GROUNDED_VELOCITY;
                }
                else
                {
                    _velocity.y -= stats.gravity * Time.deltaTime;
                }
            }
        }
        #endregion

        #region Public Methods
        public void SetStats(Data_BirdStats newStats) => stats = newStats;
        #endregion
    }
}
