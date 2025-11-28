using BirdGame.Core.Interfaces;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace BirdGame.Nest.Core
{
    [RequireComponent(typeof(Nest_Core))]
    [RequireComponent(typeof(Nest_DetectionZone))]
    public class Nest_Deposit : NetworkBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool autoDeposit = true;

        [Header("Events")]
        public UnityEvent<GameObject> OnEggDeposited;

        private Nest_Core _nest;
        private Nest_DetectionZone _detectionZone;

        private void Awake()
        {
            _nest = GetComponent<Nest_Core>();
            _detectionZone = GetComponent<Nest_DetectionZone>();
        }

        private void OnEnable()
        {
            _detectionZone.OnBirdEntered.AddListener(OnAllyEntered);
        }

        private void OnDisable()
        {
            _detectionZone.OnBirdEntered.RemoveListener(OnAllyEntered);
        }

        public void TryDepositAllEggs(GameObject bird)
        {
            if (!IsServer) return;
            if (!_detectionZone.IsAlly(bird)) return;
            if (!bird.TryGetComponent<IBirdCarrier>(out var carrier)) return;

            int deposited = 0;
            while (carrier.CurrentCarryCount > 0 && !_nest.IsFull)
            {
                var egg = carrier.RemoveOneForDeposit();
                if (egg != null && _nest.TryDepositEgg())
                {
                    PlaceEggInNest(egg);
                    deposited++;
                }
            }

            if (deposited > 0)
            {
                OnEggDeposited?.Invoke(bird);
                NotifyDepositClientRpc(deposited);
            }
        }

        private void OnAllyEntered(GameObject bird)
        {
            if (!IsServer) return;
            if (!autoDeposit) return;

            TryDepositAllEggs(bird);
        }

        private void PlaceEggInNest(ICarryable egg)
        {
            if (egg.Transform == null) return;

            egg.Transform.SetParent(_nest.transform);
            egg.Transform.localPosition = _nest.GetRandomEggPosition();

            if (egg.Transform.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = true;
            }
            if (egg.Transform.TryGetComponent<Collider>(out var col))
            {
                col.enabled = false;
            }
        }

        [ClientRpc]
        private void NotifyDepositClientRpc(int count)
        {
        }
    }
}
