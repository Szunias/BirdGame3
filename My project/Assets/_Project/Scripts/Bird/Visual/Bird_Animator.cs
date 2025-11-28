using BirdGame.Bird.Core;
using UnityEngine;

namespace BirdGame.Bird.Visual
{
    [RequireComponent(typeof(Animator))]
    public class Bird_Animator : MonoBehaviour
    {
        [SerializeField] private Bird_Movement movement;

        private Animator _animator;

        private static readonly int IsFlying = Animator.StringToHash("IsFlying");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            if (movement == null) movement = GetComponentInParent<Bird_Movement>();
        }

        private void OnEnable()
        {
            if (movement != null)
            {
                movement.OnTakeoff.AddListener(OnTakeoff);
                movement.OnLand.AddListener(OnLand);
            }
        }

        private void OnDisable()
        {
            if (movement != null)
            {
                movement.OnTakeoff.RemoveListener(OnTakeoff);
                movement.OnLand.RemoveListener(OnLand);
            }
        }

        private void OnTakeoff() => _animator.SetBool(IsFlying, true);
        private void OnLand() => _animator.SetBool(IsFlying, false);
    }
}
