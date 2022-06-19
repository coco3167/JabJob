using UnityEngine;
using UnityEngine.Events;

namespace _JabJob.Prefabs.Light_Switch.Scripts
{
    public class LightSwitch : MonoBehaviour
    {
        public AudioSource audioSource;
        public UnityEvent OnSwitchOn;
        public Transform switchTransform;
        
        
        private bool _isOn;
        
        private void OnTriggerEnter(Collider other)
        {
            if (_isOn)
                return;
            
            if (!other.CompareTag("Toy"))
                return;
            
            _isOn = true;
            
            audioSource.Play();
            
            switchTransform.localRotation = Quaternion.Euler(-127f, 90, -90);
            
            OnSwitchOn.Invoke();
        }
    }
}
