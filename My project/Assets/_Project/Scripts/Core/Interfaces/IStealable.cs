using UnityEngine;

namespace BirdGame.Core.Interfaces
{
    public interface IStealable
    {
        float StealDuration { get; }
        bool CanSteal(GameObject thief);
        void OnStealStart(GameObject thief);
        void OnStealComplete(GameObject thief);
        void OnStealCancel(GameObject thief);
    }
}
