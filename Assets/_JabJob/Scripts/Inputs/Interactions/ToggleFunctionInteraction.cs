using UnityEngine;
using UnityEngine.Events;

namespace _JabJob.Scripts.Inputs.Interactions
{
	public class ToggleFunctionInteraction : Interaction
	{
		public UnityEvent OnInteract = new UnityEvent(); 
		
		private bool _canInteract = true;
		
		public override bool Interact(bool isPressed)
		{
			if (!_canInteract)
				return false;

			if (isPressed)
				return false;
			
			Debug.Log("hello");

			OnInteract?.Invoke();

			return false;
		}

		public void SetInteractivity(bool interactivity)
		{
			_canInteract = interactivity;
		}
	}
}