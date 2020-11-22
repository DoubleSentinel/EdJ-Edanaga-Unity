using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerChapter1 : MonoBehaviour
{
    // Local variables
    private GameObject controllers;

    void Awake()
    {
        controllers = GameObject.Find("Controllers");
        //hostConversationCallback = () => { GameEventMessage.SendEvent("GoToTables"); };
    }

    private void Start()
    {
        controllers.GetComponent<LanguageHandler>().translateUI();
    }
}
