using PathCreation.Examples;
using UnityEngine;

namespace _JabJob.Prefabs.Spider.Scripts
{
    public class Spider : MonoBehaviour
    {
        private Transform _torch;
        private Transform _transform;
        private PathFollower _pathFollower;
        private Animator _animator;
        
        private bool _traveling;
        private float _targetTravelSpeed;
        private readonly int _speedPropertyId = Animator.StringToHash("Speed");

        private void Start()
        {
            _torch = GameObject.FindGameObjectWithTag("Torch").transform;
            _transform = transform;
            _pathFollower = GetComponent<PathFollower>();
            _animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            float distance = Vector3.Distance(_transform.position, _torch.position);
            
            if (!_traveling && distance < 4f)
                _traveling = true;
            if (_traveling && distance > 9f)
                _traveling = false;
            
            _targetTravelSpeed = _traveling
                ? 5f
                : 0f;
            
            _pathFollower.speed = Mathf.Lerp(_pathFollower.speed, _targetTravelSpeed, Time.deltaTime * 3f);
            _animator.SetFloat(_speedPropertyId, _pathFollower.speed);
        }
    }
}
