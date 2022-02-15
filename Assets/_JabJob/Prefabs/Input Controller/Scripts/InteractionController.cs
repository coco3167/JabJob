using UnityEngine;

using _JabJob.Prefabs.Player.Scripts;
using _JabJob.Scripts.Inputs;

namespace _JabJob.Prefabs.Input_Controller.Scripts
{
	public class InteractionController : MonoBehaviour
	{
		public static InteractionController Instance { get; private set; }

		
		public float interactionDistance = 2f;
		
		private Interaction _currentInteraction;
		private void Start()
		{
			Instance = this;
		}
		
		public void Interact(bool isPressed)
		{
			Interaction interaction;
			
			if (ReferenceEquals(_currentInteraction, null))
			{
				if (ReferenceEquals(MovementController.Instance, null))
					return;
				
				if (!Physics.Raycast(MovementController.Instance.GetViewRay(), out RaycastHit raycastHit, interactionDistance))
					return;
				
				interaction = raycastHit.collider.transform.GetComponent<Interaction>();

				if (ReferenceEquals(interaction, null))
					return;
			}
			else
			{
				interaction = _currentInteraction;
			}

			bool g = interaction.Interact(isPressed);

			_currentInteraction = g ? interaction : null;
		}
	}
}