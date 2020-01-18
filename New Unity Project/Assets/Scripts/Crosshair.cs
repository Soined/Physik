using UnityEngine;
using System.Collections;

public class Crosshair : MonoBehaviour
{

    public Texture crosshairRed;
    public Texture crosshairWhite;
    public Texture crosshairYellow;
    public Texture crosshairGreen;

    public CrosshairColor standardCrosshair;

    public float size;

    private Texture currentTex;

    private void Start()
    {
        switch (standardCrosshair)
        {
            case CrosshairColor.red:
                currentTex = crosshairRed;
                break;
            case CrosshairColor.yellow:
                currentTex = crosshairYellow;
                break;
            case CrosshairColor.green:
                currentTex = crosshairGreen;
                break;
            case CrosshairColor.white:
                currentTex = crosshairWhite;
                break;
        }
    }

    public void ChangeCrosshairColor(CrosshairColor col)
    {
        switch(col)
        {
            case CrosshairColor.red:
                currentTex = crosshairRed;
                break;
            case CrosshairColor.yellow:
                currentTex = crosshairYellow;
                break;
            case CrosshairColor.green:
                currentTex = crosshairGreen;
                break;
            case CrosshairColor.white:
                currentTex = crosshairWhite;
                break;
        }
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(Screen.width / 2 - size / 2, Screen.height / 2 - size / 2, size, size), currentTex);
    }

}

public enum CrosshairColor
{
    red,
    white,
    yellow,
    green
}
