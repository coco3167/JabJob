using Acron0;

using UnityEngine;
using UnityEngine.Events;

using _JabJob.Scripts.Inputs;

namespace _JabJob.Prefabs.Button.Scripts
{
	internal enum ButtonState
	{
		Idle,
		WaitForEndPress,
		Withdrawal,
		WaitForFullPress,
		WaitForFullPressComplete
	}

	public class ButtonInteraction : Interaction
	{
		public UnityEvent OnButtonPressed;
		
		[Header("Animation Elements")]
		public Transform button;
		public MeshRenderer buttonRenderer;

		[Header("Animation Properties")]
		public float pressDisplacement;
		[Range(0f, 1f)] public float initiatedPressDisplacementPercentage;
		public float pressSpeed;
		public float completedPressDuration;
		public Easings.Functions easingFunction;
		public Material idleMaterial;
		public Material completedMaterial;
		public Material cancelledMaterial;
		
		[Header("Sound Properties")]
		public AudioClip completedSound;
		public AudioClip cancelSound;
		public AudioSource audioSource;
		
		private float _easingTime = 0f;
		private float _easingCompletedPressTime = 0f;
		private ButtonState _state = ButtonState.Idle;
		public override bool Interact(bool isPressed, bool isInSight)
		{
			if (_state != ButtonState.Idle && _state != ButtonState.WaitForEndPress)
				return false;
			
			if (isPressed)
			{
				_state = ButtonState.WaitForEndPress;
				
				LogButtonInteractionState("Initiated");
				return true;
			}

			if (isInSight)
			{
				_state = ButtonState.WaitForFullPress;
				audioSource.PlayOneShot(completedSound);
				
				LogButtonInteractionState("Completed");
				return false;
			}

			_state = ButtonState.Withdrawal;
			audioSource.PlayOneShot(cancelSound);
			buttonRenderer.material = cancelledMaterial;
			
			LogButtonInteractionState("Cancelled");
			return false;
		}

		private void Start()
		{
			buttonRenderer.material = idleMaterial;
		}

		private void Update()
		{
			switch (_state)
			{
				case ButtonState.Idle:
					return;
				case ButtonState.WaitForEndPress:
					_easingTime = Mathf.Min(_easingTime + pressSpeed * Time.deltaTime, initiatedPressDisplacementPercentage);
					goto default;
				case ButtonState.WaitForFullPress:
					_easingTime = Mathf.Min(_easingTime + pressSpeed * Time.deltaTime, 1f);
					
					if (_easingTime >= 1f)
					{
						_state = ButtonState.WaitForFullPressComplete;
						buttonRenderer.material = completedMaterial;
						_easingCompletedPressTime = 0f;
						OnButtonPressed?.Invoke();
					}
					goto default;
				case ButtonState.WaitForFullPressComplete:
					_easingCompletedPressTime += Time.deltaTime;
					
					if (_easingCompletedPressTime >= completedPressDuration)
					{
						_state = ButtonState.Withdrawal;
						buttonRenderer.material = idleMaterial;
						_easingCompletedPressTime = 0f;
					}
					goto default;
				case ButtonState.Withdrawal:
					_easingTime = Mathf.Max(_easingTime - pressSpeed * Time.deltaTime, 0f);
					
					if (_easingTime <= 0f)
					{
						_state = ButtonState.Idle;
						buttonRenderer.material = idleMaterial;
					}
					goto default;
				default:
					float easedTime = Easings.Interpolate(_easingTime, easingFunction);
					button.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(0f, 0f, pressDisplacement), easedTime);
					break;
			}
		}
		
		private void LogButtonInteractionState(string state)
		{
			Debug.Log($"Button Interaction {state}");
		}
	}
}