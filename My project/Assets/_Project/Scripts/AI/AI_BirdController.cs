using UnityEngine;
using UnityEngine.Events;
using BirdGame.Core.Data;

namespace BirdGame.AI
{
    /// <summary>
    /// Base AI controller for NPC birds. Handles smooth, natural movement.
    /// Single Responsibility: AI movement and physics.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class AI_BirdController : MonoBehaviour
    {
        #region Constants
        private const float GROUNDED_VELOCITY = -2f;
        private const float GRAVITY_LERP_SPEED = 2f;
        private const float BANKING_LERP_SPEED = 3f;
        private const float VELOCITY_THRESHOLD = 0.1f;
        private const float APPROACH_DISTANCE_FLIGHT = 5f;
        private const float APPROACH_DISTANCE_GROUND = 3f;
        private const float VERTICAL_THRESHOLD = 0.3f;
        private const float EULER_ANGLE_WRAP = 180f;
        private const float FULL_ROTATION = 360f;
        private const float BANK_MULTIPLIER = 0.5f;
        private const float STOP_VELOCITY_MULTIPLIER = 0.5f;
        #endregion

        #region Serialized Fields
        [Header("Configuration")]
        [SerializeField] private Data_BirdStats stats;

        [Header("AI Movement Settings")]
        [SerializeField] private float aiSpeedMultiplier = 0.5f;
        [SerializeField] private float aiAcceleration = 8f;
        [SerializeField] private float aiRotationSpeed = 3f;

        [Header("Natural Flight")]
        [SerializeField] private bool enableBobbing = true;
        [SerializeField] private float bobFrequency = 2f;
        [SerializeField] private float bobAmplitude = 0.3f;
        [SerializeField] private float bankingAngle = 15f;

        [Header("Events")]
        public UnityEvent OnFlightStarted;
        public UnityEvent OnFlightEnded;
        #endregion

        #region Private Fields
        private CharacterController _controller;
        private Vector3 _velocity;
        private Vector3 _smoothVelocity;
        private bool _isFlying;
        private float _bobTimer;
        private float _targetYaw;
        #endregion

        #region Properties
        public bool IsFlying => _isFlying;
        public Data_BirdStats Stats => stats;
        public Vector3 Velocity => _velocity;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _bobTimer = Random.Range(0f, Mathf.PI * 2f); // Random phase offset
        }

        private void Start()
        {
            _isFlying = true;
        }

        private void Update()
        {
            if (stats == null) return;
            if (_controller == null) return;

            ApplyGravity();
            ApplyBobbing();
            ApplyBanking();

            // Smooth the velocity for more natural movement
            _smoothVelocity = Vector3.Lerp(_smoothVelocity, _velocity, aiAcceleration * Time.deltaTime);

            _controller.Move(_smoothVelocity * Time.deltaTime);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Move towards a target position smoothly.
        /// </summary>
        public void MoveTowards(Vector3 targetPosition, bool fly = true)
        {
            if (stats == null) return;

            Vector3 direction = targetPosition - transform.position;
            float distance = direction.magnitude;

            if (fly)
            {
                HandleFlightMovement(direction, distance);
            }
            else
            {
                HandleGroundMovement(direction, distance);
            }

            ApplyRotation();
        }

        /// <summary>
        /// Stop movement smoothly.
        /// </summary>
        public void Stop()
        {
            _velocity = Vector3.Lerp(_velocity, Vector3.zero, aiAcceleration * STOP_VELOCITY_MULTIPLIER * Time.deltaTime);
        }

        /// <summary>
        /// Check if close enough to a position.
        /// </summary>
        public bool IsNearPosition(Vector3 position, float threshold = 1f)
        {
            return Vector3.Distance(transform.position, position) <= threshold;
        }

        public void SetStats(Data_BirdStats newStats) => stats = newStats;
        #endregion

        #region Private Movement Methods
        private void HandleFlightMovement(Vector3 direction, float distance)
        {
            if (!_isFlying) StartFlight();

            Vector3 moveDir = direction.normalized;

            // Calculate base speed with AI multiplier
            float baseSpeed = stats.flightSpeed * aiSpeedMultiplier;

            // Adjust speed based on vertical direction
            if (moveDir.y > VERTICAL_THRESHOLD)
            {
                baseSpeed = stats.climbSpeed * aiSpeedMultiplier;
            }
            else if (moveDir.y < -VERTICAL_THRESHOLD)
            {
                baseSpeed = stats.diveSpeed * aiSpeedMultiplier;
            }

            // Slow down when approaching target
            if (distance < APPROACH_DISTANCE_FLIGHT)
            {
                baseSpeed *= Mathf.Clamp01(distance / APPROACH_DISTANCE_FLIGHT);
            }

            Vector3 targetVelocity = moveDir * baseSpeed;

            // Smooth acceleration
            _velocity = Vector3.Lerp(_velocity, targetVelocity, aiAcceleration * Time.deltaTime);
        }

        private void HandleGroundMovement(Vector3 direction, float distance)
        {
            if (_isFlying) StopFlight();

            Vector3 flatDir = new Vector3(direction.x, 0f, direction.z).normalized;
            float baseSpeed = stats.groundSpeed * aiSpeedMultiplier;

            if (distance < APPROACH_DISTANCE_GROUND)
            {
                baseSpeed *= Mathf.Clamp01(distance / APPROACH_DISTANCE_GROUND);
            }

            Vector3 targetHorizontal = flatDir * baseSpeed;

            float currentY = _velocity.y;
            Vector3 currentHorizontal = new Vector3(_velocity.x, 0f, _velocity.z);
            Vector3 newHorizontal = Vector3.Lerp(currentHorizontal, targetHorizontal, aiAcceleration * Time.deltaTime);

            _velocity = new Vector3(newHorizontal.x, currentY, newHorizontal.z);
        }

        private void ApplyRotation()
        {
            Vector3 lookDir = new Vector3(_velocity.x, 0f, _velocity.z);
            if (lookDir.sqrMagnitude > VELOCITY_THRESHOLD)
            {
                _targetYaw = Mathf.Atan2(lookDir.x, lookDir.z) * Mathf.Rad2Deg;
                float currentYaw = transform.eulerAngles.y;
                float newYaw = Mathf.LerpAngle(currentYaw, _targetYaw, aiRotationSpeed * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0f, newYaw, transform.eulerAngles.z);
            }
        }
        #endregion

        #region Physics
        private void ApplyGravity()
        {
            if (_isFlying)
            {
                // Gentle float - no harsh gravity changes
                _velocity.y = Mathf.Lerp(_velocity.y, 0f, GRAVITY_LERP_SPEED * Time.deltaTime);
            }
            else
            {
                if (_controller.isGrounded && _velocity.y < 0f)
                {
                    _velocity.y = GROUNDED_VELOCITY;
                }
                else
                {
                    _velocity.y -= stats.gravity * Time.deltaTime;
                }
            }
        }

        private void ApplyBobbing()
        {
            if (!_isFlying || !enableBobbing) return;

            _bobTimer += Time.deltaTime * bobFrequency;

            // Add vertical bobbing to smooth velocity
            float bob = Mathf.Sin(_bobTimer) * bobAmplitude;
            _smoothVelocity.y += bob;
        }

        private void ApplyBanking()
        {
            if (!_isFlying) return;

            // Calculate banking based on turning
            float currentYaw = transform.eulerAngles.y;
            float yawDiff = Mathf.DeltaAngle(currentYaw, _targetYaw);

            // Bank into turns
            float targetBank = Mathf.Clamp(yawDiff * BANK_MULTIPLIER, -bankingAngle, bankingAngle);
            float currentBank = transform.eulerAngles.z;
            if (currentBank > EULER_ANGLE_WRAP)
            {
                currentBank -= FULL_ROTATION;
            }

            float newBank = Mathf.Lerp(currentBank, targetBank, BANKING_LERP_SPEED * Time.deltaTime);

            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                transform.eulerAngles.y,
                newBank
            );
        }
        #endregion

        #region Flight State
        private void StartFlight()
        {
            _isFlying = true;
            OnFlightStarted?.Invoke();
        }

        private void StopFlight()
        {
            _isFlying = false;
            OnFlightEnded?.Invoke();
        }
        #endregion
    }
}
