using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    public SelectObject selectObject;

    public GameObject leftCrosshair;
    public GameObject rightCrosshair;

    Sprite rightCrosshairSprite;
    Sprite leftCrosshairSprite;

    public void CrosshairLeft()
    {
        leftCrosshair.GetComponent<Image>().enabled = true;
    }

    public void CrosshairRight()
    {
        rightCrosshair.GetComponent<Image>().enabled = true;
    }

    public void DisableLeft()
    {
        leftCrosshair.GetComponent<Image>().enabled = false;
    }

    public void DisableRight()
    {
        rightCrosshair.GetComponent<Image>().enabled = false;
    }


    private void Update()
    {

        if (selectObject.hasSelected)
        {
            CrosshairLeft();
        }
        else
        {
            DisableLeft();
        }

        if (selectObject.frozenObjects.Count > 0)
        {
            CrosshairRight();
        }
        else
        {
            DisableRight();
        }

    }

}
