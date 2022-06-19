using UnityEngine;

using Acron0;

namespace _JabJob.Prefabs.Door.Scripts
{
    public enum DoorOpeningSide
    {
        PushFromA,
        PushFromB
    }
    
    public class DoorMovementController : MonoBehaviour
    {
        public bool IsOpen = false;
        public GameObject[] StaticLightCutoutsPlanes;
    
        [Header("Properties")]
        public float openingDuration = 2f;
        public DoorOpeningSide openingSide;
        public Easings.Functions easingFunction;

        private float _openingTime = 0f;
        private Transform _transform;

        private void Start()
        {
            _transform = transform;
            foreach (var staticLightCutoutsPlane in StaticLightCutoutsPlanes)
            {
                Destroy(staticLightCutoutsPlane);
            }
        }

        private void Update()
        {
            if (IsOpen && _openingTime >= openingDuration || !IsOpen && _openingTime <= 0f)
                return;
            
            _openingTime += Time.deltaTime * (IsOpen ? 1 : -1);
            _openingTime = Mathf.Clamp(_openingTime, 0, openingDuration);

            float t = _openingTime / openingDuration;

            float easedT = Easings.Interpolate(t, easingFunction);

            Vector3 localEulerRotation = _transform.localRotation.eulerAngles;
            _transform.localRotation = Quaternion.Euler(
                new Vector3(
                    localEulerRotation.x,
                    easedT * (openingSide == DoorOpeningSide.PushFromA ? -1 : 1) * 90,
                    localEulerRotation.z
                )
            );
        }

        public void ToggleOpening()
        {
            IsOpen = !IsOpen;
        }
    }
}