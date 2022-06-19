using System;
using System.Collections;
using System.Collections.Generic;
using _JabJob.Prefabs.Input_Controller.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class ExitGame : MonoBehaviour
{
    private void Start()
    {
        Invoke("quit", 30f);
    }

    private void quit()
    {
        Application.Quit();
    }
}
