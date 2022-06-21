using UnityEngine;

using _JabJob.Prefabs.Player.Scripts;
using _JabJob.Prefabs.Object_Collision_Zone_Highlight.Scripts;

namespace _JabJob.Scripts.Inputs.Interactions
{
	[RequireComponent(typeof(Rigidbody))]
	public class GrabInteraction : Interaction
	{
		public override string Type => "Object";
		
		public bool CanBeGrabbed = true;
		public bool Throwable = true;
		public bool AlignWhenGrabbed = true;
		public Sprite objectSprite;

		public LayerMask layerMask;

		private bool _isGrabbed;
		
		private Rigidbody _rigidbody;
		private Transform _transform;

		private float _releaseStartTime = -1f;
		private Vector3 _releaseForce;
		private float _releaseAngle;
		private Vector3 _releaseHitPosition;
		private Quaternion _releaseHitRotation;

		private readonly Quaternion _releaseComplementaryHitRotation = Quaternion.Euler(90f, 0f, 0f);
		
		public override bool Interact(bool isPressed, bool isInSight)
		{
			if (!CanBeGrabbed)
				return false;
			
			if (ReferenceEquals(MovementController.Instance, null))
				return false;
			
			if (ReferenceEquals(MovementController.Instance.playerTransform, null))
				return false;
			
			if (_isGrabbed)
			{
				if (isPressed)
				{
					// Object Start Release
					_releaseStartTime = Time.time;
					ObjectCollisionZoneHighlight.Enable();
					
					return true;
				}
				
				// Object End Release
				ObjectCollisionZoneHighlight.Disable();

				_isGrabbed = false;
				_rigidbody.useGravity = true;
				_rigidbody.velocity = _releaseForce;
				_rigidbody.constraints = RigidbodyConstraints.None;
				_transform.SetParent(null, true);
				if (!ReferenceEquals(null, PlayerUI.Instance))
				{
					PlayerUI.Instance.SetImageObjectHolding(null);
					PlayerUI.Instance.SetActionPlayerUIState("throw", false);
				}

				_releaseStartTime = -1f;
				return false;
			}
			
			if (isPressed)
			{
				return true;
			}

			if (!isInSight)
				return false;
				
			// Object Grabbed
			_isGrabbed = true;
			_rigidbody.useGravity = false;
			_rigidbody.velocity = Vector3.zero;
			_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
			_transform.SetParent(MovementController.Instance.playerTransform, true);
			if (AlignWhenGrabbed)
				_transform.rotation = Quaternion.identity;

			if (!ReferenceEquals(null, PlayerUI.Instance))
			{
				PlayerUI.Instance.SetImageObjectHolding(objectSprite);
				PlayerUI.Instance.SetActionPlayerUIState("throw", true);
			}

			return true;
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

			if (_releaseStartTime < -0.5f)
				return;

			UpdateReleaseForce();
			UpdateCollisionZoneHighlight();
		}

		private void UpdateReleaseForce()
		{
			if (!Throwable)
			{
				_releaseForce = Vector3.zero;
				return;
			}

			float releaseTime = Time.time - _releaseStartTime;
			
			Ray viewRay = MovementController.Instance.GetViewRay();
			
			Vector3 viewRayOrigin = viewRay.origin;
			Vector3 viewRayDirection = viewRay.direction;
			Vector3 viewDirectionEulerAngles = Quaternion.LookRotation(viewRayDirection).eulerAngles;
			float viewDirectionEulerAnglesXFromBottom = (viewDirectionEulerAngles.x > 180f
				? 450f
				: 90f
			) - viewDirectionEulerAngles.x;

			_releaseAngle = Mathf.Min(
				releaseTime * 1f / MovementController.Instance.throw1DegDuration,
				viewDirectionEulerAnglesXFromBottom
			);

			_releaseAngle = _releaseAngle <= 10f
				? 0f
				: _releaseAngle;

			Ray ray = new Ray(
				viewRayOrigin,
				Quaternion.Euler(new Vector3(
					90f - _releaseAngle,
					viewDirectionEulerAngles.y,
					viewDirectionEulerAngles.z
				)) * Vector3.forward
			);

			float distance;
			if (Physics.Raycast(ray, out RaycastHit hit, MovementController.Instance.throwMaxDistance, layerMask))
			{
				_releaseHitRotation = Quaternion.LookRotation(hit.normal) * _releaseComplementaryHitRotation;
				_releaseHitPosition = hit.point;
				distance = Vector3.Distance(_transform.position, _releaseHitPosition);
			}
			else
			{
				_releaseHitRotation = Quaternion.LookRotation(ray.direction, Vector3.up)
				                      * Quaternion.Euler(-90f, 0f, 0f);
				_releaseHitPosition = ray.origin
				                      + ray.direction.normalized
				                      * MovementController.Instance.throwMaxDistance;
				distance = MovementController.Instance.throwMaxDistance;
			}
			
			_releaseForce = ((_releaseHitPosition + Vector3.up * 0.5f) - _transform.position).normalized * 3.3f * distance;
			_releaseForce *= 0.8f + _releaseForce.y / Mathf.Sqrt(Mathf.Pow(_releaseForce.x, 2f) + Mathf.Pow(_releaseForce.z, 2f)) / 2f;
		}

		private void UpdateCollisionZoneHighlight()
		{
			ObjectCollisionZoneHighlight.SetPositionAndRotation(_releaseHitPosition, _releaseHitRotation);
		}
	}
}