using System.Collections.Generic;
using BirdGame.Core.Interfaces;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace BirdGame.Bird.Core
{
    public class Bird_EggCarrier : NetworkBehaviour, IBirdCarrier
    {
        [Header("Configuration")]
        [SerializeField] private int carryCapacity = 3;
        [SerializeField] private Transform carryAttachPoint;
        [SerializeField] private float throwForce = 15f;
        [SerializeField] private float dropForce = 2f;

        [Header("Events")]
        public UnityEvent<ICarryable> OnEggPickedUp;
        public UnityEvent<ICarryable> OnEggDropped;
        public UnityEvent<ICarryable> OnEggThrown;

        private List<ICarryable> _carriedItems = new List<ICarryable>();

        public int CarryCapacity => carryCapacity;
        public int CurrentCarryCount => _carriedItems.Count;
        public Transform CarryAttachPoint => carryAttachPoint;

        public bool CanPickup(ICarryable item)
        {
            if (item == null) return false;
            if (item.IsBeingCarried) return false;
            if (_carriedItems.Count >= carryCapacity) return false;
            return true;
        }

        public bool TryPickup(ICarryable item)
        {
            if (!CanPickup(item)) return false;

            _carriedItems.Add(item);
            item.OnPickedUp(this);
            OnEggPickedUp?.Invoke(item);
            return true;
        }

        public void DropAll()
        {
            if (!IsServer)
            {
                DropAllServerRpc();
                return;
            }

            var dropVelocity = Vector3.down * dropForce;

            for (int i = _carriedItems.Count - 1; i >= 0; i--)
            {
                var item = _carriedItems[i];
                _carriedItems.RemoveAt(i);

                // Notify egg to detach visually
                NotifyDetach(item);

                item.OnDropped(dropVelocity);
                OnEggDropped?.Invoke(item);
            }
        }

        public void DropOne()
        {
            if (!IsServer)
            {
                DropOneServerRpc();
                return;
            }

            if (_carriedItems.Count == 0) return;

            var item = _carriedItems[_carriedItems.Count - 1];
            _carriedItems.RemoveAt(_carriedItems.Count - 1);

            // Notify egg to detach visually
            NotifyDetach(item);

            var dropVelocity = Vector3.down * dropForce;
            item.OnDropped(dropVelocity);
            OnEggDropped?.Invoke(item);
        }

        public void ThrowOne(Vector3 direction, float force)
        {
            if (!IsServer)
            {
                ThrowOneServerRpc(direction, force);
                return;
            }

            if (_carriedItems.Count == 0) return;

            var item = _carriedItems[_carriedItems.Count - 1];
            _carriedItems.RemoveAt(_carriedItems.Count - 1);

            // Notify egg to detach visually
            NotifyDetach(item);

            var throwVelocity = direction.normalized * force;
            item.OnThrown(throwVelocity);
            OnEggThrown?.Invoke(item);
        }

        private void NotifyDetach(ICarryable item)
        {
            item.ClearAttachPoint();
        }

        [ServerRpc]
        private void DropAllServerRpc()
        {
            DropAll();
        }

        [ServerRpc]
        private void DropOneServerRpc()
        {
            DropOne();
        }

        [ServerRpc]
        private void ThrowOneServerRpc(Vector3 direction, float force)
        {
            ThrowOne(direction, force);
        }

        public void ThrowForward()
        {
            ThrowOne(transform.forward + Vector3.up * 0.3f, throwForce);
        }
    }
}
