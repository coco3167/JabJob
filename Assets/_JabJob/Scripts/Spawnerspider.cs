using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnerspider : MonoBehaviour
{
    private bool enter = false;
    public GameObject spider;
    public Transform transform;
    private void OnTriggerEnter(Collider other)
    {
        if (!enter && other.CompareTag("Player"))
        {
            spider.SetActive(true);
            enter = true;
        }
    }
}
