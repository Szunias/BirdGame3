using BirdGame.Core.Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace BirdGame.Egg
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Egg_Base : NetworkBehaviour, ICarryable, IPoolable
    {
        [Header("Configuration")]
        [SerializeField] private float weight = 1f;
        [SerializeField] private float throwerIgnoreTime = 0.5f;

        private Rigidbody _rigidbody;
        private Collider _collider;
        private IBirdCarrier _currentCarrier;
        private Transform _attachPoint;
        private Collider _throwerCollider;
        private float _ignoreCollisionTimer;

        private NetworkVariable<bool> _isBeingCarried = new NetworkVariable<bool>(
            false,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
        );

        public float Weight => weight;
        public bool IsBeingCarried => _isBeingCarried.Value;
        public Transform Transform => transform;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        public override void OnNetworkSpawn()
        {
            _isBeingCarried.OnValueChanged += OnCarriedStateChanged;

            // Server controls physics, clients are kinematic
            if (!IsServer)
            {
                _rigidbody.isKinematic = true;
            }
        }

        public override void OnNetworkDespawn()
        {
            _isBeingCarried.OnValueChanged -= OnCarriedStateChanged;
        }

        private void LateUpdate()
        {
            // Follow attach point when being carried (on all clients)
            if (_isBeingCarried.Value && _attachPoint != null)
            {
                transform.position = _attachPoint.position;
                transform.rotation = _attachPoint.rotation;
            }

            // Re-enable collision with thrower after timer
            if (_throwerCollider != null && _ignoreCollisionTimer > 0)
            {
                _ignoreCollisionTimer -= Time.deltaTime;
                if (_ignoreCollisionTimer <= 0)
                {
                    Physics.IgnoreCollision(_collider, _throwerCollider, false);
                    _throwerCollider = null;
                }
            }
        }

        private void OnCarriedStateChanged(bool previous, bool current)
        {
            _collider.enabled = !current;

            if (IsServer)
            {
                _rigidbody.isKinematic = current;
            }
        }

        public void SetAttachPoint(Transform attachPoint)
        {
            _attachPoint = attachPoint;
        }

        public void ClearAttachPoint()
        {
            _attachPoint = null;
        }

        public void OnPickedUp(IBirdCarrier carrier)
        {
            _currentCarrier = carrier;

            if (IsServer)
            {
                _isBeingCarried.Value = true;
            }
        }

        public void OnDropped(Vector3 dropVelocity)
        {
            _currentCarrier = null;
            _attachPoint = null;

            if (IsServer)
            {
                _isBeingCarried.Value = false;
                _rigidbody.linearVelocity = dropVelocity;
            }
        }

        public void OnThrown(Vector3 throwVelocity)
        {
            // Store thrower collider to ignore collision temporarily
            if (_currentCarrier != null)
            {
                var carrierTransform = _currentCarrier.CarryAttachPoint;
                if (carrierTransform != null)
                {
                    _throwerCollider = carrierTransform.GetComponentInParent<Collider>();
                    if (_throwerCollider != null)
                    {
                        Physics.IgnoreCollision(_collider, _throwerCollider, true);
                        _ignoreCollisionTimer = throwerIgnoreTime;
                    }
                }
            }

            _currentCarrier = null;
            _attachPoint = null;

            if (IsServer)
            {
                _isBeingCarried.Value = false;
                _rigidbody.linearVelocity = throwVelocity;
            }
        }

        // IPoolable implementation
        public void OnSpawn()
        {
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            _collider.enabled = true;
            _attachPoint = null;

            if (IsServer)
            {
                _isBeingCarried.Value = false;
                _rigidbody.isKinematic = false;
            }
        }

        public void OnDespawn()
        {
            _currentCarrier = null;
            _attachPoint = null;
        }
    }
}
