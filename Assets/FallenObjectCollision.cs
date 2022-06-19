using System;
using UnityEngine;

public class FallenObjectCollision : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        GatherFallenTrain gft = other.GetComponentInChildren<GatherFallenTrain>(true);
        
        if (gft != null)
        {
            gft.Respawn();
        }
    }
}
