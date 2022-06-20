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
		public LayerMask layerMask;
		
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
			
			bool hasLastInteraction = !ReferenceEquals(_lastInteraction, null);

			if (Physics.Raycast(viewRay, out RaycastHit raycastHit, interactionDistance, layerMask))
			{
				Interaction currentInteraction = raycastHit.collider.GetComponent<Interaction>();
				
				if (hasLastInteraction)
				{
					bool isInSight = ReferenceEquals(_lastInteraction, currentInteraction);
					
					_lastInteraction = _lastInteraction.Interact(isPressed, isInSight)
						? _lastInteraction
						: null;
					return;
				}
				
				if (ReferenceEquals(null, currentInteraction))
					return;
				
				_lastInteraction = currentInteraction.Interact(isPressed, true)
					? currentInteraction
					: null;
				return;
			}
			
			if (!hasLastInteraction)
				return;
			
			_lastInteraction = _lastInteraction.Interact(isPressed, false)
				? _lastInteraction
				: null;
		}
	}
}