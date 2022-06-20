using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance { private set; get; }

    public GameObject objectHoldingPanel;
    public Image objectHoldingImage;

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
