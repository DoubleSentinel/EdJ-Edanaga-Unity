using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coloration : MonoBehaviour
{
    public Color contour;
    public Color fond;
    public Color accent;
    public Color blanc = Color.white;

    private void Awake()
    {
        UpdateColors();
    }

    public void UpdateColors()
    {
        foreach (SpriteRenderer sprite in transform.GetComponentsInChildren<SpriteRenderer>(true))
        {
            switch (sprite.gameObject.tag)
            {
                case "Color:Contour":
                    sprite.color = contour;
                    break;
                case "Color:Fond":
                    sprite.color = fond;
                    break;
                case "Color:Accent":
                    sprite.color = accent;
                    break;
                case "Color:Blanc":
                    sprite.color = blanc;
                    break;
            } 
        }
    }
}
