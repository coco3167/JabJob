using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using _JabJob.Scripts.Inputs;

namespace _JabJob.Prefabs.Input_Controller.Scripts
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputController : MonoBehaviour
    {
        public static InputController Instance { get; private set; }
        

        public InputType InputType { get; private set; } = InputType.KeyboardAndMouse;
        public UnityEvent<InputType> OnInputTypeChanged = new UnityEvent<InputType>();
        
        public Vector2 MoveInputValue { get; private set; } = Vector2.zero;
        public Vector2 RotateInputValue =>
            InputType == InputType.KeyboardAndMouse && Cursor.lockState != CursorLockMode.Locked
                ? Vector2.zero
                : _rotateInputValue;
        
        public bool IsRunning { get; private set; }
        public bool IsJumping { get; private set; }
        
        private Vector2 _rotateInputValue = Vector2.zero;
        private float _captureCameraInputValue = 0f;
        private string _currentControlScheme = "";
        private PlayerInput _playerInput;

        private void Awake()
        {
            Instance = this;
            
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            UpdateInputType();
        }
        
        private void UpdateInputType()
        {
            if (_playerInput.currentControlScheme == _currentControlScheme) return;
            _currentControlScheme = _playerInput.currentControlScheme;
            
            InputType = _playerInput.currentControlScheme switch
            {
                "KeyboardAndMouse" => InputType.KeyboardAndMouse,
                "Gamepad" => InputType.Gamepad,
                _ => InputType
            };
            
            if (InputType == InputType.Gamepad)
                Cursor.lockState = CursorLockMode.Locked;
            
            OnInputTypeChanged.Invoke(InputType);
        }

        public void OnCaptureCamera(InputValue inputValue)
        {
            float value = inputValue.Get<float>();

            if (Mathf.Abs(_captureCameraInputValue) < 0.5f && Mathf.Abs(value) > 0.5f)
                Cursor.lockState = value > 0 ? CursorLockMode.Locked : CursorLockMode.None;

            _captureCameraInputValue = value;
        }

        public void OnRotateCamera(InputValue inputValue)
        {
            _rotateInputValue = inputValue.Get<Vector2>();
        }
        public void OnCharacterMove(InputValue inputValue)
        {
            MoveInputValue = inputValue.Get<Vector2>();
        }
        
        public void OnCharacterRunning(InputValue inputValue)
        {
            IsRunning = inputValue.Get<float>() > 0.5f;
        }
        public void OnCharacterJumping(InputValue inputValue)
        {
            // TODO: Uncomment this when jump has been repaired
            //IsJumping = inputValue.Get<float>() > 0.5f;
        }
        
        public void OnCharacterInteract(InputValue inputValue)
        {
            if (!ReferenceEquals(InteractionController.Instance, null))
                InteractionController.Instance.Interact(inputValue.isPressed);
        }
    }
}