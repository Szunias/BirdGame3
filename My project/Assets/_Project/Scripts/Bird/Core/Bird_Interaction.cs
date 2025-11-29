using BirdGame.Core.Interfaces;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace BirdGame.Bird.Core
{
    /// <summary>
    /// Handles player interaction with IInteractable objects.
    /// Single Responsibility: Detection and interaction management.
    /// </summary>
    public class Bird_Interaction : NetworkBehaviour
    {
        #region Constants
        private const float CACHE_REFRESH_INTERVAL = 0.1f; // Refresh nearby interactables every 0.1s
        #endregion

        #region Serialized Fields
        [Header("Configuration")]
        [SerializeField] private float detectionRadius = 2f;
        [SerializeField] private LayerMask interactableLayer;

        [Header("Events")]
        public UnityEvent<IInteractable> OnInteractionStarted;
        public UnityEvent<IInteractable> OnInteractionCompleted;
        public UnityEvent<IInteractable> OnInteractionCanceled;
        public UnityEvent<float> OnInteractionProgress;
        #endregion

        #region Private Fields
        private IInteractable _currentTarget;
        private bool _isHolding;
        private float _holdTimer;
        private float _cacheRefreshTimer;

        // Cached nearby interactables - refreshed periodically, NOT every frame
        private Collider[] _cachedColliders = new Collider[10];
        private int _cachedColliderCount;
        #endregion

        #region Properties
        public IInteractable CurrentTarget => _currentTarget;
        public bool IsInteracting => _isHolding;
        public float InteractionProgress => _currentTarget != null ? _holdTimer / _currentTarget.HoldDuration : 0f;
        #endregion

        #region Unity Lifecycle
        private void Update()
        {
            if (!IsOwner) return;

            // Refresh detection periodically, NOT every frame
            _cacheRefreshTimer -= Time.deltaTime;
            if (_cacheRefreshTimer <= 0f)
            {
                UpdateTargetDetection();
                _cacheRefreshTimer = CACHE_REFRESH_INTERVAL;
            }

            UpdateHoldProgress();
        }
        #endregion

        #region Detection
        private void UpdateTargetDetection()
        {
            if (_isHolding) return;

            // Use NonAlloc version to avoid garbage allocation
            _cachedColliderCount = interactableLayer == 0
                ? Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, _cachedColliders)
                : Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, _cachedColliders, interactableLayer);

            IInteractable closest = null;
            float closestDist = float.MaxValue;

            for (int i = 0; i < _cachedColliderCount; i++)
            {
                var col = _cachedColliders[i];
                if (col == null) continue;

                // Use TryGetComponent pattern (per guidelines section 4.4)
                if (!col.TryGetComponent<IInteractable>(out var interactable))
                {
                    // Try parent as fallback
                    var parent = col.transform.parent;
                    if (parent != null)
                    {
                        parent.TryGetComponent<IInteractable>(out interactable);
                    }
                }

                if (interactable != null && interactable.CanInteract(gameObject))
                {
                    float dist = Vector3.Distance(transform.position, col.transform.position);
                    if (dist < closestDist)
                    {
                        closest = interactable;
                        closestDist = dist;
                    }
                }
            }

            _currentTarget = closest;
        }
        #endregion

        #region Interaction Logic
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
        #endregion

        #region Server RPCs
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
            // Server handles the actual interaction completion
            var colliders = interactableLayer == 0
                ? Physics.OverlapSphere(transform.position, detectionRadius)
                : Physics.OverlapSphere(transform.position, detectionRadius, interactableLayer);

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
        #endregion

        #region Debug
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
        #endregion
    }
}
