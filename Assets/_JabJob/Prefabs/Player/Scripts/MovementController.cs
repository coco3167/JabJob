using UnityEngine;

using _JabJob.Prefabs.Input_Controller.Scripts;
using _JabJob.Scripts.Inputs;

namespace _JabJob.Prefabs.Player.Scripts
{
	[RequireComponent(typeof(CharacterController))]
	public class MovementController : MonoBehaviour
	{
		public static MovementController Instance { get; private set; }
		
		
		[Header("Transforms")]
		public Transform playerTransform;
		public Transform playerBodyTransform;
		public Transform cameraHolder;
		public Transform grabPoint;
		
		[Header("Properties")]
		public float walkSpeed = 10f;
		public float runSpeed = 17f;
		public float gamepadRotationSpeed = 150f;
		public float mouseRotationSpeed = 25f;
		
		private CharacterController _characterController;
		private float _cameraRotationX = 90f;
		private float _verticalVelocity = 0f;

		private void Start()
		{
			Instance = this;
			
			_characterController = GetComponent<CharacterController>();
			
			playerTransform ??= transform;
		}
		
		private void Update()
		{
			VerticalTranslation();
			Translate();
			Rotate();
		}

		private void Translate()
		{
			if (ReferenceEquals(InputController.Instance, null) || ReferenceEquals(playerBodyTransform, null)) return;
			
			float movingSpeed = InputController.Instance.IsRunning
				? runSpeed
				: walkSpeed;

			Vector3 movement = playerBodyTransform.rotation * new Vector3(
				InputController.Instance.MoveInputValue.x,
				0,
				InputController.Instance.MoveInputValue.y
			) * movingSpeed * Time.deltaTime;
			
			_characterController.Move(movement);
			_characterController.Move(new Vector3(0, _verticalVelocity, 0));
		}

		private void VerticalTranslation()
		{
			_verticalVelocity += Physics.gravity.y * 0.005f * Time.deltaTime;
			
			if (Physics.Raycast(playerTransform.position, Vector3.down, 0.2f))
			{
				_verticalVelocity = InputController.Instance.IsJumping ? 30f : 0f;
			}
		}

		private void Rotate()
		{
			if (ReferenceEquals(InputController.Instance, null) || ReferenceEquals(playerBodyTransform, null)) return;
			
			Vector3 localEulerRotation = playerTransform.localRotation.eulerAngles;
			
			float rotationSpeed = InputController.Instance.InputType == InputType.Gamepad
				? gamepadRotationSpeed
				: mouseRotationSpeed;
			
			playerTransform.localRotation = Quaternion.Euler(new Vector3(
				localEulerRotation.x,
				localEulerRotation.y
				+ InputController.Instance.RotateInputValue.x
				* rotationSpeed
				* Time.deltaTime,
				localEulerRotation.z
			));
			_cameraRotationX -= InputController.Instance.RotateInputValue.y * rotationSpeed * Time.deltaTime;
			_cameraRotationX = Mathf.Clamp(_cameraRotationX, 10, 170);
			cameraHolder.localRotation = Quaternion.Euler(new Vector3(_cameraRotationX, 0, 0));
		}
		
		public Ray GetViewRay()
		{
			Vector3 direction = Quaternion.Euler(
				_cameraRotationX + 90,
				playerTransform.localRotation.eulerAngles.y,
				0
			) * Vector3.back;
			
			return new Ray(cameraHolder.position, direction);
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawRay(GetViewRay());
		}
	}
}