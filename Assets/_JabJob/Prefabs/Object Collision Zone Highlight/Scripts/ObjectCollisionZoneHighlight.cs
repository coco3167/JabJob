using System;
using UnityEngine;

namespace _JabJob.Prefabs.Object_Collision_Zone_Highlight.Scripts
{
    public class ObjectCollisionZoneHighlight : MonoBehaviour
    {
        private static ObjectCollisionZoneHighlight _instance;

        private void Start()
        {
            _instance = this;
            Disable();
        }

        public static void Enable()
        {
            if (ReferenceEquals(_instance, null)) return;
            if (_instance.gameObject.activeSelf) return;
            _instance.gameObject.SetActive(true);
        }
        
        public static void Disable()
        {
            if (ReferenceEquals(_instance, null)) return;
            if (!_instance.gameObject.activeSelf) return;
            _instance.gameObject.SetActive(false);
        }
        
        public static void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            if (ReferenceEquals(_instance, null)) return;
            Transform t = _instance.transform;
            
            t.position = position;
            t.rotation = rotation;
        }
    }
}
