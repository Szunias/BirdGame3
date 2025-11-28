using BirdGame.Core.Interfaces;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace BirdGame.Nest.Core
{
    [RequireComponent(typeof(Nest_Core))]
    [RequireComponent(typeof(Nest_DetectionZone))]
    public class Nest_Stealable : NetworkBehaviour, IStealable, IInteractable
    {
        [Header("Configuration")]
        [SerializeField] private float stealDuration = 2f;

        [Header("Events")]
        public UnityEvent<GameObject> OnStealStarted;
        public UnityEvent<GameObject> OnStealSucceeded;
        public UnityEvent<GameObject> OnStealCanceled;

        private Nest_Core _nest;
        private Nest_DetectionZone _detectionZone;
        private GameObject _currentThief;

        public float StealDuration => stealDuration;
        public float HoldDuration => stealDuration;

        private void Awake()
        {
            _nest = GetComponent<Nest_Core>();
            _detectionZone = GetComponent<Nest_DetectionZone>();
        }

        public bool CanSteal(GameObject thief)
        {
            if (_nest.IsEmpty) return false;
            if (_currentThief != null && _currentThief != thief) return false;
            if (!_detectionZone.IsEnemy(thief)) return false;
            return true;
        }

        public bool CanInteract(GameObject interactor) => CanSteal(interactor);

        public void OnStealStart(GameObject thief)
        {
            _currentThief = thief;
            OnStealStarted?.Invoke(thief);
        }

        public void OnInteractionStart(GameObject interactor) => OnStealStart(interactor);

        public void OnStealComplete(GameObject thief)
        {
            if (!IsServer) return;
            if (_currentThief != thief) return;

            if (_nest.TryStealEgg())
            {
                if (thief.TryGetComponent<IBirdCarrier>(out var carrier))
                {
                    // TODO: Spawn egg and give to carrier
                    OnStealSucceeded?.Invoke(thief);
                }
            }

            _currentThief = null;
        }

        public void OnInteractionComplete(GameObject interactor) => OnStealComplete(interactor);

        public void OnStealCancel(GameObject thief)
        {
            if (_currentThief != thief) return;

            _currentThief = null;
            OnStealCanceled?.Invoke(thief);
        }

        public void OnInteractionCancel(GameObject interactor) => OnStealCancel(interactor);
    }
}
