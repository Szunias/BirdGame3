using UnityEngine;

namespace BirdGame.Core.Interfaces
{
    public interface ICarryable
    {
        float Weight { get; }
        bool IsBeingCarried { get; }
        Transform Transform { get; }

        void OnPickedUp(IBirdCarrier carrier);
        void OnDropped(Vector3 dropVelocity);
        void OnThrown(Vector3 throwVelocity);
    }
}
