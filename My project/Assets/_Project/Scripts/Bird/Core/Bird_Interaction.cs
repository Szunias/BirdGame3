using BirdGame.Core.Interfaces;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace BirdGame.Bird.Core
{
    public class Bird_Interaction : NetworkBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float detectionRadius = 2f;
        [SerializeField] private LayerMask interactableLayer;

        [Header("Events")]
        public UnityEvent<IInteractable> OnInteractionStarted;
        public UnityEvent<IInteractable> OnInteractionCompleted;
        public UnityEvent<IInteractable> OnInteractionCanceled;
        public UnityEvent<float> OnInteractionProgress;

        private IInteractable _currentTarget;
        private bool _isHolding;
        private float _holdTimer;

        public IInteractable CurrentTarget => _currentTarget;
        public bool IsInteracting => _isHolding;
        public float InteractionProgress => _currentTarget != null ? _holdTimer / _currentTarget.HoldDuration : 0f;

        private void Update()
        {
            if (!IsOwner) return;

            UpdateTargetDetection();
            UpdateHoldProgress();
        }

        private void UpdateTargetDetection()
        {
            if (_isHolding) return;

            var colliders = Physics.OverlapSphere(transform.position, detectionRadius, interactableLayer);

            IInteractable closest = null;
            float closestDist = float.MaxValue;

            foreach (var col in colliders)
            {
                if (col.TryGetComponent<IInteractable>(out var interactable))
                {
                    if (interactable.CanInteract(gameObject))
                    {
                        float dist = Vector3.Distance(transform.position, col.transform.position);
                        if (dist < closestDist)
                        {
                            closest = interactable;
                            closestDist = dist;
                        }
                    }
                }
            }

            _currentTarget = closest;
        }

        private void UpdateHoldProgress()
        {
            if (!_isHolding || _currentTarget == null) return;

            _holdTimer += Time.deltaTime;
            OnInteractionProgress?.Invoke(InteractionProgress);

            if (_holdTimer >= _currentTarget.HoldDuration)
            {
                CompleteInteraction();
            }
        }

        public void StartInteraction()
        {
            if (_currentTarget == null) return;
            if (!_currentTarget.CanInteract(gameObject)) return;

            _isHolding = true;
            _holdTimer = 0f;
            _currentTarget.OnInteractionStart(gameObject);
            OnInteractionStarted?.Invoke(_currentTarget);

            StartInteractionServerRpc();
        }

        public void CancelInteraction()
        {
            if (!_isHolding) return;

            _currentTarget?.OnInteractionCancel(gameObject);
            OnInteractionCanceled?.Invoke(_currentTarget);

            _isHolding = false;
            _holdTimer = 0f;

            CancelInteractionServerRpc();
        }

        private void CompleteInteraction()
        {
            if (_currentTarget == null) return;

            CompleteInteractionServerRpc();

            _currentTarget.OnInteractionComplete(gameObject);
            OnInteractionCompleted?.Invoke(_currentTarget);

            _isHolding = false;
            _holdTimer = 0f;
            _currentTarget = null;
        }

        [ServerRpc]
        private void StartInteractionServerRpc()
        {
            // Server can validate interaction here
        }

        [ServerRpc]
        private void CancelInteractionServerRpc()
        {
            // Server can handle cancellation here
        }

        [ServerRpc]
        private void CompleteInteractionServerRpc()
        {
            // Server handles the actual pickup
            var colliders = Physics.OverlapSphere(transform.position, detectionRadius, interactableLayer);

            foreach (var col in colliders)
            {
                if (col.TryGetComponent<IInteractable>(out var interactable))
                {
                    if (interactable.CanInteract(gameObject))
                    {
                        interactable.OnInteractionComplete(gameObject);
                        break;
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
