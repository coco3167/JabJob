using UnityEngine;

using _JabJob.Prefabs.Player.Scripts;
using _JabJob.Scripts.Inputs;

namespace _JabJob.Prefabs.Input_Controller.Scripts
{
	public class InteractionController : MonoBehaviour
	{
		public static InteractionController Instance { get; private set; }

		[Header("Interaction Properties")]
		public float interactionDistance = 2f;
		
		private Interaction _lastInteraction;
		private void Start()
		{
			Instance = this;
		}
		
		public void Interact(bool isPressed)
		{
			if (ReferenceEquals(MovementController.Instance, null))
				return;
				
			Ray viewRay = MovementController.Instance.GetViewRay();
			
			if (!Physics.Raycast(viewRay, out RaycastHit raycastHit, interactionDistance))
				return;
			
			bool hasLastInteraction = !ReferenceEquals(_lastInteraction, null);
			Interaction currentInteraction = raycastHit.collider.GetComponent<Interaction>();
			
			Interaction interaction;
			bool isInSight;
			
			if (hasLastInteraction) {
				interaction = _lastInteraction;
				isInSight = ReferenceEquals(interaction, currentInteraction);
			} else {
				interaction = currentInteraction;
				isInSight = true;
			}
			
			if (ReferenceEquals(interaction, null))
				return;

			bool keepInteraction = interaction.Interact(isPressed, isInSight);

			_lastInteraction = keepInteraction
				? interaction
				: null;
		}
	}
}