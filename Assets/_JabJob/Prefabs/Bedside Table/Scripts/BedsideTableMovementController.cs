using UnityEngine;

using Acron0;

namespace _JabJob.Prefabs.Bedside_Table.Scripts
{
    public class BedsideTableMovementController : MonoBehaviour
    {
        public bool IsOpen = false;

        public Transform drawerTransform;
        
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

            Vector3 drawerTransformLocalPosition = drawerTransform.localPosition;
            drawerTransform.localPosition = new Vector3(
                Mathf.Lerp(-0.008749432f, -0.22f, easedT),
                drawerTransformLocalPosition.y,
                drawerTransformLocalPosition.z
            );
        }
        
        public void ToggleOpening()
        {
            IsOpen = !IsOpen;
        }
    }
}
