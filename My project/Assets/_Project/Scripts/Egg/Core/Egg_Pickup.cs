using BirdGame.Core.Interfaces;
using Unity.Netcode;
using UnityEngine;

namespace BirdGame.Egg
{
    [RequireComponent(typeof(Egg_Base))]
    public class Egg_Pickup : NetworkBehaviour
    {
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
                // Find NetworkObject on carrier's root (Bird prefab)
                var carrierTransform = carrier.CarryAttachPoint;
                if (carrierTransform == null) return;

                var networkObj = carrierTransform.GetComponentInParent<NetworkObject>();
                if (networkObj == null) return;

                AttachToCarrierClientRpc(networkObj.NetworkObjectId);
            }
        }

        [ClientRpc]
        private void AttachToCarrierClientRpc(ulong carrierNetworkId)
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(carrierNetworkId, out var carrierNetObj))
            {
                var carrier = carrierNetObj.GetComponent<IBirdCarrier>();
                if (carrier?.CarryAttachPoint != null)
                {
                    // Don't use SetParent - instead set attach point for following
                    _egg.SetAttachPoint(carrier.CarryAttachPoint);
                }
            }
        }

        [ClientRpc]
        public void DetachFromCarrierClientRpc()
        {
            _egg.ClearAttachPoint();
        }
    }
}
