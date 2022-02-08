using UnityEngine;

using _JabJob.Prefabs.Player.Scripts;

namespace _JabJob.Scripts.Inputs.Interactions
{
	[RequireComponent(typeof(Rigidbody))]
	public class GrabInteraction : Interaction
	{
		public bool CanBeGrabbed = true;
		
		private bool _isGrabbed = false;
		private Rigidbody _rigidbody;
		private Transform _transform;
		public override bool Interact(bool isPressed)
		{
			if (!CanBeGrabbed)
				return false;

			if (isPressed)
				return _isGrabbed;
			
			if (ReferenceEquals(MovementController.Instance, null))
				return false;
			
			if (ReferenceEquals(MovementController.Instance.playerTransform, null))
				return false;

			_isGrabbed = !_isGrabbed;

			_rigidbody.velocity = Vector3.zero;
			_rigidbody.useGravity = !_isGrabbed;
			_rigidbody.constraints = _isGrabbed ? RigidbodyConstraints.FreezeRotation : RigidbodyConstraints.None;

			_transform.parent = _isGrabbed ? MovementController.Instance.playerTransform : null;

			return _isGrabbed;
		}

		private void Start()
		{
			_rigidbody = GetComponent<Rigidbody>();
			_transform = transform;
		}
		
		private void Update()
		{
			if (!_isGrabbed) return;
			
			if (ReferenceEquals(MovementController.Instance, null))
				return;
			
			if (ReferenceEquals(MovementController.Instance.grabPoint, null))
				return;

			_rigidbody.MovePosition(MovementController.Instance.grabPoint.position);
		}
	}
}