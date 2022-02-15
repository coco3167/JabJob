using UnityEngine;

using Acron0;

namespace _JabJob.Prefabs.Bedside_Table.Scripts
{
    public class BedsideTableMovementController : MonoBehaviour
    {
        public bool IsOpen = false;

        public Transform drawerParentTransform;
        public Rigidbody drawerRigidbody;
        
        [Header("Properties")]
        public float openingDuration = 2f;
        public Easings.Functions easingFunction;
        
        private float _openingTime = 0f;

        private void Update()
        {
            _openingTime += Time.deltaTime * (IsOpen ? 1 : -1);
            _openingTime = Mathf.Clamp(_openingTime, 0, openingDuration);

            float t = _openingTime / openingDuration;

            float easedT = Easings.Interpolate(t, easingFunction);

            drawerRigidbody.MovePosition(drawerParentTransform.position + drawerParentTransform.rotation * new Vector3(
                Mathf.Lerp(-0.008749432f, -0.22f, easedT),
                0,
                0
            ));
            drawerRigidbody.MoveRotation(drawerParentTransform.rotation * Quaternion.Euler(-90, 0, 0));
        }
        
        public void ToggleOpening()
        {
            IsOpen = !IsOpen;
        }
    }
}
