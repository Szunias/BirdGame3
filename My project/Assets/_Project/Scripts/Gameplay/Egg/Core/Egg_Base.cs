using BirdGame.Core.Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace BirdGame.Gameplay.Egg
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Egg_Base : NetworkBehaviour, ICarryable, IPoolable
    {
        [Header("Configuration")]
        [SerializeField] private float weight = 1f;
        [SerializeField] private float pickupRadius = 1.5f;

        private Rigidbody _rigidbody;
        private Collider _collider;
        private IBirdCarrier _currentCarrier;

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

        private void OnCarriedStateChanged(bool previous, bool current)
        {
            _collider.enabled = !current;

            if (IsServer)
            {
                _rigidbody.isKinematic = current;
            }
        }

        public void OnPickedUp(IBirdCarrier carrier)
        {
            _currentCarrier = carrier;

            if (IsServer)
            {
                _isBeingCarried.Value = true;
                _rigidbody.linearVelocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
            }
        }

        public void OnDropped(Vector3 dropVelocity)
        {
            _currentCarrier = null;

            if (IsServer)
            {
                _isBeingCarried.Value = false;
                _rigidbody.linearVelocity = dropVelocity;
            }
        }

        public void OnThrown(Vector3 throwVelocity)
        {
            _currentCarrier = null;

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

            if (IsServer)
            {
                _isBeingCarried.Value = false;
                _rigidbody.isKinematic = false;
            }
        }

        public void OnDespawn()
        {
            _currentCarrier = null;
        }
    }
}
