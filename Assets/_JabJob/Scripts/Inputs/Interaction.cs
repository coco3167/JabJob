using UnityEngine;

namespace _JabJob.Scripts.Inputs
{
	public abstract class Interaction : MonoBehaviour
	{
		public virtual bool Interact(bool isPressed)
		{
			return false;
		}
	}
}