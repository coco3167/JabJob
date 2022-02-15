using UnityEngine;
using UnityEngine.AI;

namespace _JabJob.Prefabs.Spider.Scripts
{
    public class Spider : MonoBehaviour
    {
        private Transform _player;
        private NavMeshAgent _agent;

        private void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            _agent.SetDestination(_player.position);
        }
        
        public void Spawn()
        {
            gameObject.SetActive(true);
        }
    }
}
