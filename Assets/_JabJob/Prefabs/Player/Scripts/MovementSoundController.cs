using UnityEngine;

using _JabJob.Prefabs.Input_Controller.Scripts;

namespace _JabJob.Prefabs.Player.Scripts
{
    public class MovementSoundController : MonoBehaviour
    {
        [SerializeField]
        private float volumeIncrease, volumeDecrease, volumeLimitWalk;

        private AudioSource _audioSource;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            float movementSpeed = InputController.Instance.MoveInputValue.normalized.magnitude;
            float baseVolume = _audioSource.volume;
            
            _audioSource.volume = movementSpeed == 0
                ? baseVolume - volumeDecrease < 0
                    ? 0
                    : baseVolume - volumeDecrease
                    
                : baseVolume + volumeIncrease < volumeLimitWalk
                    ? baseVolume + volumeIncrease
                    : !InputController.Instance.IsRunning
                        ? volumeLimitWalk
                        : baseVolume + volumeIncrease < 1
                            ? baseVolume + volumeIncrease
                            : 1;
        }
    }
}