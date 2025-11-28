using UnityEngine;

namespace BirdGame.Bird.Core
{
    [RequireComponent(typeof(Bird_Input))]
    [RequireComponent(typeof(Bird_Interaction))]
    public class Bird_InteractionInput : MonoBehaviour
    {
        private Bird_Input _input;
        private Bird_Interaction _interaction;

        private void Awake()
        {
            _input = GetComponent<Bird_Input>();
            _interaction = GetComponent<Bird_Interaction>();
        }

        private void OnEnable()
        {
            _input.OnInteractPressed.AddListener(OnInteractPressed);
            _input.OnInteractReleased.AddListener(OnInteractReleased);
        }

        private void OnDisable()
        {
            _input.OnInteractPressed.RemoveListener(OnInteractPressed);
            _input.OnInteractReleased.RemoveListener(OnInteractReleased);
        }

        private void OnInteractPressed()
        {
            _interaction.StartInteraction();
        }

        private void OnInteractReleased()
        {
            _interaction.CancelInteraction();
        }
    }
}
