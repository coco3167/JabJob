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
		private Interaction _currentLookingInteraction;
		
		private void Start()
		{
			Instance = this;
		}

		private void Update()
		{
			if (ReferenceEquals(MovementController.Instance, null))
				return;
				
			Ray viewRay = MovementController.Instance.GetViewRay();
			
			if (Physics.Raycast(viewRay, out RaycastHit raycastHit, interactionDistance, layerMask))
			{
				Interaction currentLookingInteraction = raycastHit.collider.GetComponent<Interaction>();

				if (!ReferenceEquals(currentLookingInteraction, _currentLookingInteraction))
				{
					if (!ReferenceEquals(_currentLookingInteraction, null))
					{
						LoseInteractionView(_currentLookingInteraction);
					}

					if (!ReferenceEquals(currentLookingInteraction, null))
					{
						EnterInteractionView(currentLookingInteraction);
					}
				}

				_currentLookingInteraction = currentLookingInteraction;
				return;
			}

			if (!ReferenceEquals(_currentLookingInteraction, null))
			{
				LoseInteractionView(_currentLookingInteraction);
			}
			
			_currentLookingInteraction = null;
		}

		private void LoseInteractionView(Interaction interaction)
		{
			Debug.Log($"Lose {interaction.Type}");
			if (interaction.Type == "Object")
			{
				PlayerUI.Instance.SetActionPlayerUIState("grab", false);
			}
			else
			{
				PlayerUI.Instance.SetActionPlayerUIState("interact", false);
			}
		}

		private void EnterInteractionView(Interaction interaction)
		{
			Debug.Log($"Enter {interaction.Type}");
			if (interaction.Type == "Object")
			{
				PlayerUI.Instance.SetActionPlayerUIState("grab", true);
			}
			else
			{
				PlayerUI.Instance.SetActionPlayerUIState("interact", true);
			}
		}

		public void Interact(bool isPressed)
		{
			bool hasLastInteraction = !ReferenceEquals(_lastInteraction, null);
			bool hasCurrentLookingInteraction = !ReferenceEquals(_currentLookingInteraction, null);

			if (hasLastInteraction)
			{
				bool isInSight = ReferenceEquals(_lastInteraction, _currentLookingInteraction);
					
				_lastInteraction = _lastInteraction.Interact(isPressed, isInSight)
					? _lastInteraction
					: null;
					
				return;
			}
			
			if (!hasCurrentLookingInteraction) return;
				
			_lastInteraction = _currentLookingInteraction.Interact(isPressed, true)
				? _currentLookingInteraction
				: null;
		}
	}
}