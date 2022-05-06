using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class BlinkingLight : MonoBehaviour
{
    public GameObject RedLight;
    public GameObject BlueLight;
    
    public int blueOnProba;
    public int blueLowProba;
    public int blueHighProba;

    public int redOnProba;
    public int redLowProba;
    public int redHighProba;

    private Random _random = new Random();


    private void Start()
    {
        BlueLight.SetActive(false);
        RedLight.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        LightOn(BlueLight, blueOnProba, blueLowProba, blueHighProba);
        LightOn(RedLight, redOnProba, redLowProba, redHighProba);
    }

    private void LightOn(GameObject light, int lightProba, int lightLowProba, int lightHighProba)
    {
        int proba = _random.Next(100);

        if (!light.activeSelf && proba < lightProba)
        {
            light.SetActive(true);
            Debug.Log(lightProba - proba);
            StartCoroutine(LightOff(light, _random.Next(lightLowProba,lightHighProba)/100f));
        }
    }

    IEnumerator LightOff(GameObject light, float timer)
    {
        yield return new WaitForSeconds(timer);
        light.SetActive(false);
    }
}
