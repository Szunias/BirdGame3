using Unity.Netcode;
using UnityEngine;

namespace BirdGame.Bird.Core
{
    /// <summary>
    /// Handles throw trajectory visualization and prediction.
    /// Single Responsibility: Trajectory calculation and display.
    /// </summary>
    [RequireComponent(typeof(Bird_EggCarrier))]
    public class Bird_ThrowTrajectory : NetworkBehaviour
    {
        #region Constants
        private const float GRAVITY_MULTIPLIER = 0.5f;
        private const float QUADRATIC_MULTIPLIER = 2f;
        private const int QUADRATIC_DISCRIMINANT_MULTIPLIER = 4;
        #endregion

        #region Serialized Fields
        [Header("Trajectory Settings")]
        [SerializeField] private float throwForce = 15f;
        [SerializeField] private float throwAngle = 35f;
        [SerializeField] private int trajectoryResolution = 30;
        [SerializeField] private float trajectoryTimeStep = 0.05f;

        [Header("References")]
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform throwOrigin;
        #endregion

        #region Private Fields
        private Bird_EggCarrier _carrier;
        private Vector3[] _trajectoryPoints;
        private bool _isAiming;
        #endregion

        #region Properties
        public Vector3 ThrowVelocity { get; private set; }
        public bool IsAiming => _isAiming;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _carrier = GetComponent<Bird_EggCarrier>();
            _trajectoryPoints = new Vector3[trajectoryResolution];

            if (lineRenderer != null)
            {
                lineRenderer.positionCount = trajectoryResolution;
                lineRenderer.enabled = false;
            }
        }

        private void Update()
        {
            if (!IsOwner) return;
            if (!_isAiming) return;

            UpdateTrajectory();
        }
        #endregion

        #region Aiming
        public void StartAiming()
        {
            if (_carrier.CurrentCarryCount == 0) return;

            _isAiming = true;

            if (lineRenderer != null)
            {
                lineRenderer.enabled = true;
            }
        }

        public void StopAiming()
        {
            _isAiming = false;

            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
        }

        public void ThrowWithCurrentTrajectory()
        {
            var direction = CalculateThrowDirection();
            var velocity = direction * throwForce;

            _carrier.ThrowOne(velocity.normalized, velocity.magnitude);
            StopAiming();
        }
        #endregion

        #region Trajectory Calculation
        private void UpdateTrajectory()
        {
            var origin = throwOrigin != null ? throwOrigin.position : transform.position;
            var direction = CalculateThrowDirection();
            ThrowVelocity = direction * throwForce;

            CalculateTrajectoryPoints(origin, ThrowVelocity);

            if (lineRenderer != null)
            {
                lineRenderer.SetPositions(_trajectoryPoints);
            }
        }

        private Vector3 CalculateThrowDirection()
        {
            var forward = transform.forward;
            var upComponent = Mathf.Sin(throwAngle * Mathf.Deg2Rad);
            var forwardComponent = Mathf.Cos(throwAngle * Mathf.Deg2Rad);

            return (forward * forwardComponent + Vector3.up * upComponent).normalized;
        }

        private void CalculateTrajectoryPoints(Vector3 origin, Vector3 velocity)
        {
            var gravity = Physics.gravity;

            for (int i = 0; i < trajectoryResolution; i++)
            {
                float t = i * trajectoryTimeStep;
                _trajectoryPoints[i] = origin + velocity * t + GRAVITY_MULTIPLIER * gravity * t * t;
            }
        }

        public Vector3 PredictLandingPosition()
        {
            var origin = throwOrigin != null ? throwOrigin.position : transform.position;
            var velocity = CalculateThrowDirection() * throwForce;
            var gravity = Physics.gravity;

            // Solve quadratic for y = 0 (ground level)
            float a = GRAVITY_MULTIPLIER * gravity.y;
            float b = velocity.y;
            float c = origin.y;

            float discriminant = b * b - QUADRATIC_DISCRIMINANT_MULTIPLIER * a * c;
            if (discriminant < 0) return origin;

            float t = (-b - Mathf.Sqrt(discriminant)) / (QUADRATIC_MULTIPLIER * a);
            if (t < 0)
            {
                t = (-b + Mathf.Sqrt(discriminant)) / (QUADRATIC_MULTIPLIER * a);
            }

            return origin + velocity * t + GRAVITY_MULTIPLIER * gravity * t * t;
        }
        #endregion
    }
}
