using UnityEngine;

namespace BirdGame.Core.Interfaces
{
    public interface IInteractable
    {
        float HoldDuration { get; }
        bool CanInteract(GameObject interactor);
        void OnInteractionStart(GameObject interactor);
        void OnInteractionComplete(GameObject interactor);
        void OnInteractionCancel(GameObject interactor);
    }
}
