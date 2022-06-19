using UnityEngine;

using _JabJob.Prefabs.Player.Scripts;
using _JabJob.Prefabs.Object_Collision_Zone_Highlight.Scripts;

namespace _JabJob.Scripts.Inputs.Interactions
{
	[RequireComponent(typeof(Rigidbody))]
	public class GrabInteraction : Interaction
	{
		public bool CanBeGrabbed = true;
		public bool Throwable = true;
		public bool AlignWhenGrabbed = true;

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

			if (isPressed)
			{
				if (_isGrabbed)
					_releaseStartTime = Time.time;
				return _isGrabbed;
			}

			if (ReferenceEquals(MovementController.Instance, null))
				return false;
			
			if (ReferenceEquals(MovementController.Instance.playerTransform, null))
				return false;

			_isGrabbed = !_isGrabbed;

			if (_isGrabbed)
			{
				_rigidbody.velocity = Vector3.zero;
			}
			else
			{
				UpdateReleaseForce();

				_rigidbody.velocity = _releaseForce;
				
				_releaseStartTime = -1f;
				
				UpdateCollisionZoneHighlight();
			}
			
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

		private void FixedUpdate()
		{
			if (!_isGrabbed) return;
			
			if (ReferenceEquals(MovementController.Instance, null))
				return;
			
			if (ReferenceEquals(MovementController.Instance.grabPoint, null))
				return;
			
			if (AlignWhenGrabbed)
				_rigidbody.MoveRotation(Quaternion.identity);
			
			if (_releaseStartTime != -1f)
				UpdateReleaseForce();
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

			if (_releaseAngle <= 10f)
			{
				_releaseAngle = 0f;
			}

			Ray ray = new Ray(
				viewRayOrigin,
				Quaternion.Euler(new Vector3(
					90f - _releaseAngle,
					viewDirectionEulerAngles.y,
					viewDirectionEulerAngles.z
				)) * Vector3.forward
			);

			if (Physics.Raycast(ray, out RaycastHit hit, MovementController.Instance.throwMaxDistance))
			{
				_releaseHitRotation = Quaternion.LookRotation(hit.normal) * _releaseComplementaryHitRotation;
				_releaseHitPosition = hit.point;
			}
			else
			{
				_releaseHitRotation = Quaternion.identity;
				_releaseHitPosition = ray.origin
				                      + ray.direction.normalized
				                      * MovementController.Instance.throwMaxDistance;
			}
			
			float distance = Vector3.Distance(_transform.position, _releaseHitPosition);

			Debug.DrawRay(_transform.position, _releaseHitPosition - _transform.position, Color.red);
			_releaseForce = ((_releaseHitPosition + Vector3.up * 0.5f) - _transform.position).normalized * 3.3f * distance;
			_releaseForce *= 0.8f + _releaseForce.y / Mathf.Sqrt(Mathf.Pow(_releaseForce.x, 2f) + Mathf.Pow(_releaseForce.z, 2f)) / 2f;

			UpdateCollisionZoneHighlight();
		}

		private void UpdateCollisionZoneHighlight()
		{
			if (_releaseAngle < 10f || _releaseStartTime == -1f)
			{
				ObjectCollisionZoneHighlight.Disable();
			}
			else
			{
				ObjectCollisionZoneHighlight.Enable();
				ObjectCollisionZoneHighlight.SetPositionAndRotation(_releaseHitPosition, _releaseHitRotation);
			}
		}
	}
}