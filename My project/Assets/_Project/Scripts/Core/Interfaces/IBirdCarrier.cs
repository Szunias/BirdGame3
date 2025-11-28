using UnityEngine;

namespace BirdGame.Core.Interfaces
{
    public interface IBirdCarrier
    {
        int CarryCapacity { get; }
        int CurrentCarryCount { get; }
        Transform CarryAttachPoint { get; }

        bool CanPickup(ICarryable item);
        bool TryPickup(ICarryable item);
        void DropAll();
        void DropOne();
        void ThrowOne(Vector3 direction, float force);
    }
}
