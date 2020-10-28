using System;
using System.Collections;
using System.Collections.Generic;
using Shapes2D;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour
{
    [SerializeField]
    private GameObject symbolChest;
    [SerializeField]
    private GameObject symbolComplete;
    [SerializeField]
    private GameObject fullBody;

    private void Awake()
    {
        // by default show the character
        symbolChest.SetActive(true); 
        symbolComplete.SetActive(false);
        fullBody.SetActive(true);
    }

    public void MaximizeSymbol(bool isMaximized)
    {
        print("maximized");
        symbolChest.SetActive(!isMaximized); 
        symbolComplete.SetActive(isMaximized);
        fullBody.SetActive(!isMaximized);
    }
}
