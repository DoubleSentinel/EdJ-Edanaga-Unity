using DG.Tweening;
using Doozy.Engine;
using Doozy.Engine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControllerChapter3 : MonoBehaviour
{
    [Header("2D Scene References")]
    [SerializeField] private GameObject scenePlayer;
    [SerializeField] private GameObject sceneJournalist;
    [SerializeField] private GameObject sceneEngineer;

    [Header("Conversation References")]
    public GameObject[] ConversationBubbles;

    [Header("Drag&Drop Alt scene")]
    [SerializeField] private GameObject[] alternatives;
    [SerializeField] private GameObject[] panels;
    [SerializeField] private GameObject prioritiesIcon;
    [SerializeField] private GameObject buttonToDnd;
    [SerializeField] private GameObject buttonToConv;
    [SerializeField] private GameObject altDiscoveryMessage;
    [SerializeField] private GameObject altDnDMessage1;
    [SerializeField] private GameObject altDnDMessage2;

    [Header("Drag&Drop result")]
    [SerializeField]
    private GameObject temp;

    [Header("Popup Values")] public string PopupName = "Popup1";
    [SerializeField] private string Title = "Title";
    [SerializeField] private GameObject MessageObject;
    [SerializeField] private string Message = "Popup message for player";

    // Local variables
    private GameObject controllers;

    // BargainConversation vars
    [HideInInspector] public int conversationIndex = 0;
    public ConversationHandler.ConversationEnd conversationCallback;

    void Awake()
    {
        controllers = GameObject.Find("Controllers");
        conversationCallback = () => {
            if (conversationIndex == 0)
            {
                GameEventMessage.SendEvent("ContinueToAlt");
                //ShowGo(altDiscoveryMessage);
                ShowGo(buttonToConv);
                ShowGo(buttonToDnd);
            }
            if (conversationIndex == 1)
            {
                GameEventMessage.SendEvent("ContinueToMatrix");
                print("ContinueToMatrix");
            } 
        };
    }

    private void Start()
    {
        controllers.GetComponent<LanguageHandler>().translateUI();

        //Default Setup
        HideGo(buttonToDnd);
        HideGo(buttonToConv);
        HideGo(altDiscoveryMessage);
        HideGo(altDnDMessage1);
        HideGo(altDnDMessage2);

        DnD_Result(); //Return player choice 
    }

    private void Conv(int conversationIndex)
    {
        var ch = ConversationBubbles[0].GetComponent<ConversationHandler>();
        ch.callback = conversationCallback;
        ch.GenerateConversation(conversationIndex);
        ch.NextConversationSnippet();
    }

    public void StartConversation()
    {
        GameEventMessage.SendEvent("GoToConversation");
        SetupConversation();
        Conv(conversationIndex);
    }

    // --------------------  UI Callables  --------------------------------
    public void SetConversationIndex(int index)
    {
        conversationIndex = index;
    }

    public void SetupConversation()
    {
        float height = Screen.height * 0.75f / 2f;
        float depth = -1f;
        Vector3 player = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 4,
            height));
        Vector3 journalist = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 2 / 3,
            height));
        Vector3 engineer = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 5 / 6,
            height));

        scenePlayer.transform.position = new Vector3(player.x, player.y, depth);
        sceneJournalist.transform.position = new Vector3(journalist.x, journalist.y, depth);
        sceneEngineer.transform.position = new Vector3(engineer.x, engineer.y, depth);

        scenePlayer.SetActive(true);
        sceneJournalist.SetActive(true);
        sceneEngineer.SetActive(true);
    }
   
    public void DnD_Result()
    {
        List<string> dragNdropRes = new List<string> { "Alternative5", "Alternative1", "Alternative3", "Alternative2", "Alternative4", "Alternative0"};
        string alternativeName;
        int alternativeNumber = 0;

        //Set player choice
        for (int i = 0; i < panels.Length; i++)
        {
            /*
            temp = alternatives.Where(obj => obj.name == dragNdropRes[i].ToString()).SingleOrDefault().GetComponent<GameObject>();
            temp = GameObject.Find(dragNdropRes[i].ToString()).GetComponent<GameObject>();
            temp = GameObject.Find("Alterantive0");
            */
            alternativeName = dragNdropRes[i].ToString();
            alternativeNumber = Convert.ToInt32($"{alternativeName.Last()}");
            print(alternativeNumber);
            alternatives[alternativeNumber].gameObject.transform.position = panels[i].gameObject.transform.position;
        }

    }
    
    public void ShowPopup()
    {
        //get a clone of the UIPopup, with the given PopupName, from the UIPopup Database 
        UIPopup popup = UIPopup.GetPopup(PopupName);

        //make sure that a popup clone was actually created
        if (popup == null)
            return;

        Title = "Consignes";
        Message = MessageObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;
        popup.Data.SetLabelsTexts(Title, Message);

        popup.Show(); //show the popup
    }

    public void ShowGo(GameObject go)
    {
        go.SetActive(true);
    }

    public void HideGo(GameObject go)
    {
        go.SetActive(false);
    }
}