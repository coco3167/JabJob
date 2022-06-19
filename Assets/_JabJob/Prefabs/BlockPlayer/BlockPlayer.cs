using UnityEngine;

public class BlockPlayer : MonoBehaviour
{
    public void StopBlockingPlayer()
    {
       gameObject.SetActive(false);
    }
}
