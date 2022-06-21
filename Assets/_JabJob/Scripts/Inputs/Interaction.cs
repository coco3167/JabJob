using UnityEngine;

namespace _JabJob.Scripts.Inputs
{
	public abstract class Interaction : MonoBehaviour
	{
		public virtual string Type => "Interaction";
		public virtual bool Interact(bool isPressed, bool isInSight)
		{
			return false;
		}
	}
}