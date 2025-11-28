using BirdGame.Core.Interfaces;
using BirdGame.Egg.UI;
using Unity.Netcode;
using UnityEngine;

namespace BirdGame.Egg
{
    [RequireComponent(typeof(Egg_Base))]
    public class Egg_Pickup : NetworkBehaviour, IInteractable
    {
        [Header("Configuration")]
        [SerializeField] private float holdDuration = 1f;

        [Header("UI")]
        [SerializeField] private Egg_ProgressUI progressUI;

        private Egg_Base _egg;
        private GameObject _currentInteractor;
        private float _interactionTimer;

        public float HoldDuration => holdDuration;

        private void Awake()
        {
            _egg = GetComponent<Egg_Base>();
        }

        public bool CanInteract(GameObject interactor)
        {
            if (_egg.IsBeingCarried) return false;
            if (_currentInteractor != null && _currentInteractor != interactor) return false;

            if (interactor.TryGetComponent<IBirdCarrier>(out var carrier))
            {
                return carrier.CanPickup(_egg);
            }

            return false;
        }

        public void OnInteractionStart(GameObject interactor)
        {
            _currentInteractor = interactor;
            _interactionTimer = 0f;
            progressUI?.Show();
        }

        private void Update()
        {
            if (_currentInteractor == null) return;

            _interactionTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(_interactionTimer / holdDuration);
            progressUI?.SetProgress(progress);
        }

        public void OnInteractionComplete(GameObject interactor)
        {
            _currentInteractor = null;
            progressUI?.Hide();

            if (!IsServer) return;

            if (interactor.TryGetComponent<IBirdCarrier>(out var carrier))
            {
                HandlePickup(carrier);
            }
        }

        public void OnInteractionCancel(GameObject interactor)
        {
            if (_currentInteractor == interactor)
            {
                _currentInteractor = null;
                progressUI?.Hide();
            }
        }

        private void HandlePickup(IBirdCarrier carrier)
        {
            if (carrier.TryPickup(_egg))
            {
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
