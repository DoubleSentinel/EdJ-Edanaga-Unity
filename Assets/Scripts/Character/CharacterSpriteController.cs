using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour
{
    [SerializeField]
    private GameObject symbolChest;
    [SerializeField]
    private GameObject symbolComplete;
    [SerializeField]
    private GameObject fullBody;
    [SerializeField]
    private Transform heads;

    // Local variables
    private GameObject controllers;
    private string head_to_show;
    
    private void Awake()
    {
        controllers = GameObject.Find("Controllers");

        // by default show the character
        symbolChest.SetActive(true); 
        symbolComplete.SetActive(false);
        fullBody.SetActive(true);

        try
        { 
            head_to_show = controllers.GetComponent<TestingEnvironment>().Characters[this.name];

        }
        catch(KeyNotFoundException)
        {
            //Default value for chapter0
            head_to_show = "Otarie";
        }
        
        foreach (Transform child in heads) //Character Heads
		{
		    child.gameObject.SetActive(child.name == head_to_show);
		}
    }

    public void MaximizeSymbol(bool isMaximized)
    {
        symbolChest.SetActive(!isMaximized); 
        symbolComplete.SetActive(isMaximized);
        fullBody.SetActive(!isMaximized);
    }
}
