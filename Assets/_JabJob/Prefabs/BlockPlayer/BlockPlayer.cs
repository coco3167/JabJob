using UnityEngine;

namespace _JabJob.Prefabs.BlockPlayer
{
	public class BlockPlayer : MonoBehaviour
	{
		public void StopBlockingPlayer()
		{
			gameObject.SetActive(false);
		}
	}
}
