using System;
using UnityEngine;
using UnityEngine.Events;

namespace _JabJob.Prefabs.Player_Detector.Scripts
{
    public class PlayerDetector : MonoBehaviour
    {
        public UnityEvent OnPlayerDetected = new UnityEvent();
        public bool canCallMultipleTimes = false;
        public uint maxDetectionCount = 2;
        
        private uint _detectionCount = 0;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }
            
            if (canCallMultipleTimes && _detectionCount >= maxDetectionCount)
            {
                return;
            }
            
            if (!canCallMultipleTimes && _detectionCount > 0)
            {
                return;
            }
            
            _detectionCount++;
            OnPlayerDetected.Invoke();
        }
    }
}
