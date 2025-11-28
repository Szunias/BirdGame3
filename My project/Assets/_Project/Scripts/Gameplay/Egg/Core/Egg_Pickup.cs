using BirdGame.Core.Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace BirdGame.Gameplay.Egg
{
    [RequireComponent(typeof(Egg_Base))]
    public class Egg_Pickup : NetworkBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float pickupRadius = 2f;
        [SerializeField] private LayerMask birdLayer;

        private Egg_Base _egg;

        private void Awake()
        {
            _egg = GetComponent<Egg_Base>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsServer) return;
            if (_egg.IsBeingCarried) return;

            if (other.TryGetComponent<IBirdCarrier>(out var carrier))
            {
                if (carrier.CanPickup(_egg))
                {
                    HandlePickup(carrier);
                }
            }
        }

        private void HandlePickup(IBirdCarrier carrier)
        {
            if (carrier.TryPickup(_egg))
            {
                AttachToCarrierClientRpc(carrier.CarryAttachPoint.GetComponent<NetworkObject>().NetworkObjectId);
            }
        }

        [ClientRpc]
        private void AttachToCarrierClientRpc(ulong carrierNetworkId)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(carrierNetworkId, out var carrierNetObj))
            {
                var attachPoint = carrierNetObj.GetComponent<IBirdCarrier>()?.CarryAttachPoint;
                if (attachPoint != null)
                {
                    transform.SetParent(attachPoint);
                    transform.localPosition = Vector3.zero;
                    transform.localRotation = Quaternion.identity;
                }
            }
        }

        [ClientRpc]
        public void DetachFromCarrierClientRpc()
        {
            transform.SetParent(null);
        }
    }
}
