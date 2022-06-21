using System;

using UnityEngine;
using UnityEngine.UI;

[Serializable] public class PlayerUIAction
{
    public string name;
    public string message;
    public Sprite gamepadMainImage;
    public Sprite gamepadMovementImage;
    public Sprite keyboardMainImage;
}

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance { private set; get; }

    public GameObject objectHoldingPanel;
    public Image objectHoldingImage;
    public PlayerUIAction[] UIActions;

    private void Awake()
    {
        Instance = this;
        SetImageObjectHolding(null);
    }

    public void SetImageObjectHolding(Sprite image)
    {
        if (ReferenceEquals(image, null))
        {
            objectHoldingPanel.SetActive(false);
        }
        else
        {
            objectHoldingPanel.SetActive(true);
            objectHoldingImage.sprite = image;
        }
    }
}
