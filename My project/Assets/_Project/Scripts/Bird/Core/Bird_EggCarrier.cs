using System.Collections.Generic;
using BirdGame.Core.Interfaces;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace BirdGame.Bird.Core
{
    /// <summary>
    /// Handles egg carrying, dropping, and throwing.
    /// Single Responsibility: Egg inventory management.
    /// </summary>
    public class Bird_EggCarrier : NetworkBehaviour, IBirdCarrier
    {
        #region Constants
        private const float THROW_UP_COMPONENT = 0.3f;
        #endregion

        #region Serialized Fields
        [Header("Configuration")]
        [SerializeField] private int carryCapacity = 3;
        [SerializeField] private Transform carryAttachPoint;
        [SerializeField] private float throwForce = 15f;
        [SerializeField] private float dropForce = 2f;

        [Header("Events")]
        public UnityEvent<ICarryable> OnEggPickedUp;
        public UnityEvent<ICarryable> OnEggDropped;
        public UnityEvent<ICarryable> OnEggThrown;
        #endregion

        #region Private Fields
        private List<ICarryable> _carriedItems = new List<ICarryable>();
        #endregion

        #region Properties
        public int CarryCapacity => carryCapacity;
        public int CurrentCarryCount => _carriedItems.Count;
        public Transform CarryAttachPoint => carryAttachPoint;
        #endregion

        #region Pickup
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
        #endregion

        #region Drop
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

            NotifyDetach(item);

            var dropVelocity = Vector3.down * dropForce;
            item.OnDropped(dropVelocity);
            OnEggDropped?.Invoke(item);
        }
        #endregion

        #region Throw
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

            NotifyDetach(item);

            var throwVelocity = direction.normalized * force;
            item.OnThrown(throwVelocity);
            OnEggThrown?.Invoke(item);
        }

        public void ThrowForward()
        {
            ThrowOne(transform.forward + Vector3.up * THROW_UP_COMPONENT, throwForce);
        }
        #endregion

        #region Helper Methods
        private void NotifyDetach(ICarryable item)
        {
            item.ClearAttachPoint();
        }

        public ICarryable RemoveOneForDeposit()
        {
            if (_carriedItems.Count == 0) return null;

            var item = _carriedItems[_carriedItems.Count - 1];
            _carriedItems.RemoveAt(_carriedItems.Count - 1);
            item.ClearAttachPoint();

            return item;
        }
        #endregion

        #region Server RPCs
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
        #endregion
    }
}
