using BirdGame.Core.Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace BirdGame.Egg
{
    [RequireComponent(typeof(Egg_Base))]
    public class Egg_Splat : NetworkBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float blindDuration = 2f;
        [SerializeField] private float minImpactVelocity = 5f;

        private Egg_Base _egg;
        private bool _hasHit;

        private void Awake()
        {
            _egg = GetComponent<Egg_Base>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!IsServer) return;
            if (_egg.IsBeingCarried) return;
            if (_hasHit) return;

            // Check impact velocity
            if (collision.relativeVelocity.magnitude < minImpactVelocity) return;

            // Check if hit a blindable target
            if (collision.gameObject.TryGetComponent<IBlindable>(out var blindable))
            {
                _hasHit = true;
                blindable.ApplyBlind(blindDuration);
                DestroySelf();
            }
        }

        private void DestroySelf()
        {
            // TODO: Replace with pool despawn when Mgr_Pool is implemented
            if (IsServer && NetworkObject != null)
            {
                NetworkObject.Despawn();
            }
        }

        public override void OnNetworkSpawn()
        {
            _hasHit = false;
        }
    }
}
