using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coloration : MonoBehaviour
{
    [SerializeField] private Color contour;
    [SerializeField] private Color fond;
    [SerializeField] private Color accent;
    private Color blanc = Color.white;

    private void Awake()
    {
        foreach (SpriteRenderer sprite in transform.GetComponentsInChildren<SpriteRenderer>())
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
