using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

[Serializable] public class KeyImageInfo
{
    public string name;
    public Sprite sprite;
}

public class LayoutKeyImageUpdater : MonoBehaviour
{
    public Key Key;
    public Image image;
    
    public KeyImageInfo[] KeyImageInfos;

    private void Awake()
    {
        KeyControl keyControl = Keyboard.current.allKeys
            .First(control => control.keyCode == Key);

        image.sprite = KeyImageInfos
            .First(info => info.name == keyControl.displayName).sprite;
    }
}
