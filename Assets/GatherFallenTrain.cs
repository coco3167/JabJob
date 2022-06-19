using UnityEngine;

public class GatherFallenTrain : MonoBehaviour
{
    private bool _gatheringEnabled = true;
    public Vector3 respawnPosition;

    public void DisableGathering()
    {
        _gatheringEnabled = false;
    }

    public void Respawn()
    {
        if (!_gatheringEnabled)
            return;
        
        transform.position = respawnPosition;
    }
}
