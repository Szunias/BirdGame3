using UnityEngine;

namespace BirdGame.Bird.Core
{
    [RequireComponent(typeof(Bird_Input))]
    [RequireComponent(typeof(Bird_EggCarrier))]
    public class Bird_ThrowInput : MonoBehaviour
    {
        private Bird_Input _input;
        private Bird_EggCarrier _carrier;
        private Bird_ThrowTrajectory _trajectory;

        private void Awake()
        {
            _input = GetComponent<Bird_Input>();
            _carrier = GetComponent<Bird_EggCarrier>();
            _trajectory = GetComponent<Bird_ThrowTrajectory>();
        }

        private void OnEnable()
        {
            _input.OnThrowEggPressed.AddListener(OnThrowPressed);
            _input.OnDropEggPressed.AddListener(OnDropPressed);
        }

        private void OnDisable()
        {
            _input.OnThrowEggPressed.RemoveListener(OnThrowPressed);
            _input.OnDropEggPressed.RemoveListener(OnDropPressed);
        }

        private void OnThrowPressed()
        {
            if (_carrier.CurrentCarryCount == 0) return;

            if (_trajectory != null)
            {
                _trajectory.ThrowWithCurrentTrajectory();
            }
            else
            {
                _carrier.ThrowForward();
            }
        }

        private void OnDropPressed()
        {
            _carrier.DropOne();
        }
    }
}
