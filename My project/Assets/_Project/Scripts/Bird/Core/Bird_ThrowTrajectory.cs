using Unity.Netcode;
using UnityEngine;

namespace BirdGame.Bird.Core
{
    [RequireComponent(typeof(Bird_EggCarrier))]
    public class Bird_ThrowTrajectory : NetworkBehaviour
    {
        [Header("Trajectory Settings")]
        [SerializeField] private float throwForce = 15f;
        [SerializeField] private float throwAngle = 35f;
        [SerializeField] private int trajectoryResolution = 30;
        [SerializeField] private float trajectoryTimeStep = 0.05f;

        [Header("References")]
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Transform throwOrigin;

        private Bird_EggCarrier _carrier;
        private Vector3[] _trajectoryPoints;
        private bool _isAiming;

        public Vector3 ThrowVelocity { get; private set; }
        public bool IsAiming => _isAiming;

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
                _trajectoryPoints[i] = origin + velocity * t + 0.5f * gravity * t * t;
            }
        }

        public Vector3 PredictLandingPosition()
        {
            var origin = throwOrigin != null ? throwOrigin.position : transform.position;
            var velocity = CalculateThrowDirection() * throwForce;
            var gravity = Physics.gravity;

            // Solve quadratic for y = 0 (ground level)
            float a = 0.5f * gravity.y;
            float b = velocity.y;
            float c = origin.y;

            float discriminant = b * b - 4 * a * c;
            if (discriminant < 0) return origin;

            float t = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
            if (t < 0)
            {
                t = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
            }

            return origin + velocity * t + 0.5f * gravity * t * t;
        }
    }
}
