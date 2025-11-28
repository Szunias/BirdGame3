using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace BirdGame.Bird.Core
{
    public class Bird_Input : MonoBehaviour
    {
        [Header("Input Asset")]
        [SerializeField] private InputActionAsset inputActions;

        [Header("Movement Events")]
        public UnityEvent<Vector2> OnMoveInput;
        public UnityEvent<Vector2> OnLookInput;
        public UnityEvent OnJumpPressed;
        public UnityEvent OnJumpReleased;

        [Header("Action Events")]
        public UnityEvent OnAbility1Pressed;
        public UnityEvent OnAbility2Pressed;
        public UnityEvent OnInteractPressed;
        public UnityEvent OnInteractReleased;
        public UnityEvent OnThrowEggPressed;
        public UnityEvent OnDropEggPressed;

        private InputActionMap _gameplayMap;
        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _interactAction;

        private void Awake()
        {
            if (inputActions == null)
            {
                Debug.LogError($"Bird_Input on {gameObject.name}: InputActionAsset is missing!");
                return;
            }

            var asset = inputActions;
            _gameplayMap = asset.FindActionMap("Gameplay");
            if (_gameplayMap == null) Debug.LogError("Bird_Input: 'Gameplay' map not found!");

            _moveAction = _gameplayMap.FindAction("Move");
            _jumpAction = _gameplayMap.FindAction("Jump");
            _interactAction = _gameplayMap.FindAction("Interact");
        }

        private void OnEnable()
        {
            if (_gameplayMap != null)
            {
                _gameplayMap.Enable();
            }

            if (_jumpAction != null)
            {
                _jumpAction.performed += JumpPerformed;
                _jumpAction.canceled += JumpReleased;
            }

            if (_interactAction != null)
            {
                _interactAction.performed += InteractPerformed;
                _interactAction.canceled += InteractReleased;
            }
        }

        private void OnDisable()
        {
            if (_gameplayMap != null) _gameplayMap.Disable();
            if (_jumpAction != null)
            {
                _jumpAction.performed -= JumpPerformed;
                _jumpAction.canceled -= JumpReleased;
            }
            if (_interactAction != null)
            {
                _interactAction.performed -= InteractPerformed;
                _interactAction.canceled -= InteractReleased;
            }
        }

        private void JumpPerformed(InputAction.CallbackContext ctx)
        {
            OnJumpPressed?.Invoke();
        }

        private void JumpReleased(InputAction.CallbackContext ctx)
        {
            OnJumpReleased?.Invoke();
        }

        private void InteractPerformed(InputAction.CallbackContext ctx)
        {
            OnInteractPressed?.Invoke();
        }

        private void InteractReleased(InputAction.CallbackContext ctx)
        {
            OnInteractReleased?.Invoke();
        }

        private void Update()
        {
            if (_gameplayMap == null) return;

            if (_moveAction != null)
            {
                Vector2 move = _moveAction.ReadValue<Vector2>();
                // Always send input - including zero when keys released
                OnMoveInput?.Invoke(move);
            }
        }
    }
}
