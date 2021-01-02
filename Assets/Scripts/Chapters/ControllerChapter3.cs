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

    [Header("Alternatives scene")]
    [SerializeField] private GameObject[] alternatives;
    [SerializeField] private GameObject[] panels;
    [SerializeField] private GameObject prioritiesIcon;
    [SerializeField] private GameObject buttonToDnd;
    [SerializeField] private GameObject buttonToConv;
    [SerializeField] private GameObject altDiscoveryMessage;
    [SerializeField] private GameObject altDnDMessage1;
    [SerializeField] private GameObject altDnDMessage2;

    [Header("Alternatives Drag&Drop scene")]
    [SerializeField] private GameObject[] alternativesDnD;
    [SerializeField] private GameObject[] panelsDnD;

    private string[] panelIds;
    private string panelObjectValue;

    [Header("Drag&Drop result")]
    [SerializeField] private bool enableFlag;
    [SerializeField] private List<string> dragNdropRes;
    [SerializeField] private List<string> newDragNdropRes;
    [SerializeField] private GameObject buttonToConv1;
    [SerializeField] private GameObject buttonToConv2;

    [Header("Popup Values")] public string PopupName = "Popup1";
    [SerializeField] private string Title = "Title";
    [SerializeField] private GameObject MessageObject;
    [SerializeField] private string Message = "Popup message for player";

    // Local variables
    private GameObject controllers;

    // BargainConversation vars
    [HideInInspector] public int conversationIndex = 0;
    public ConversationHandler.ConversationEnd conversationCallback;

    public List<string> NewDragNdropRes { get => newDragNdropRes; set => newDragNdropRes = value; }
    public bool EnableFlag { get => enableFlag; set => enableFlag = value; }

    void Awake()
    {
        controllers = GameObject.Find("Controllers");
        conversationCallback = () => {
            //3.1_Intro_journalist_Chap6
            if (conversationIndex == 0)
            {
                GameEventMessage.SendEvent("ContinueToAlt");
                ShowGo(buttonToConv);
            }
            //3.1.3_Intro_engineer_Chap6
            if (conversationIndex == 1)
            {
                GameEventMessage.SendEvent("ContinueToMatrix");
                ShowGo(altDnDMessage1);
                HideGo(altDnDMessage2);
                ShowGo(buttonToConv1);
                HideGo(buttonToConv2);
                EnableFlag = false;
                DisableEnableDnD();
                print("ContinueToMatrix - DnD not allowed");
            }
            //3.2_Consistent_check_Chap6
            if (conversationIndex == 2)
            {
                GameEventMessage.SendEvent("ContinueToMatrix");
                HideGo(altDnDMessage1);
                ShowGo(altDnDMessage2);
                HideGo(buttonToConv1);
                EnableFlag = true;
                DisableEnableDnD();
                print("ContinueToMatrix - DnD allowed");
            }
            //3.2_Inconsistent_check_Chap6
            if (conversationIndex == 3)
            {
                GameEventMessage.SendEvent("ContinueToList");
                print("ContinueToList");
            }
            //3.3.1_Informed_ranking_Chap6
            if (conversationIndex == 4)
            {
            }
            //3.4.1_Multiple_choices_rankings_Chap6
            if (conversationIndex == 5)
            {
            }
            //3.5.1_Consistent_choice_Chap6
            if (conversationIndex == 6)
            {
            }
            //3.6_Inconsistent_but_ok_Chap6
            if (conversationIndex == 7)
            {
                SetConversationIndex(8);
                GameEventMessage.SendEvent("ContinueToConv");
            }
            //3.7_Outro_Chap6
            if (conversationIndex == 8)
            {
                GameEventMessage.SendEvent("ContinueToChapter4");
                print("ContinueToChapter4");
            }
        };
    }

    private void Start()
    {
        controllers.GetComponent<LanguageHandler>().translateUI();

        panelIds = new string[6];
        NewDragNdropRes = new List<string>();

        //Get panel Id name
        for (int i = 0; i < panelsDnD.Length; i++)
        {
            panelIds[i] = panelsDnD[i].gameObject.GetComponent<PanelSettings>().Id;
        }

        //Default Setup
        HideGo(buttonToDnd);
        HideGo(buttonToConv);
        HideGo(buttonToConv1);
        HideGo(buttonToConv2);
        HideGo(altDiscoveryMessage);
        HideGo(altDnDMessage1);
        HideGo(altDnDMessage2);

        DnD_Result();
        EnableFlag = false;
        DisableEnableDnD();
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
        dragNdropRes = new List<string> { "Alternative5", "Alternative1", "Alternative3", "Alternative2", "Alternative4", "Alternative0"};
        string alternativeName;
        int alternativeNumber = 0;

        //Set player choice
        for (int i = 0; i < panels.Length; i++)
        {
            alternativeName = dragNdropRes[i].ToString();
            alternativeNumber = Convert.ToInt32($"{alternativeName.Last()}");
            print(alternativeNumber);
            //Fix player choice
            alternatives[alternativeNumber].gameObject.transform.position = panels[i].gameObject.transform.position;
            //Set DnD default values (player choice)
            alternativesDnD[alternativeNumber].gameObject.transform.position = panelsDnD[i].gameObject.transform.position;
        }
    }

    public void CheckPriorities()
    {
        //First DnD action
        if (NewDragNdropRes.Count == 0)
            ShowGo(buttonToConv2);

        //Reset the list of the Drag&Drops result
        NewDragNdropRes.Clear();

        //Update Drag & Drop results
        for (int i = 0; i < alternativesDnD.Length; i++)
        {
            panelObjectValue = DragDropManager.GetPanelObject(panelIds[i]);
            NewDragNdropRes.Add(panelObjectValue);
        }
    }

    public void DisableEnableDnD()
    {
        if(!enableFlag)
        { 
            //Lock the drag&drop property of the elements
            for (int i = 0; i < 6; i++)
            {
                alternativesDnD[i].GetComponent<ObjectSettings>().LockObject = true;
            }
        }
        else
        {
            //Unlock the drag&drop property of the elements
            for (int i = 0; i < 6; i++)
            {
                alternativesDnD[i].GetComponent<ObjectSettings>().LockObject = false;
            }

            ShowPopup();
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

    //Get objectives texts from TestingEnvironment 
    public void GetObjectives(int i)
    {
        //Get Objectives values to set the Matrix labels
        var objectives = controllers.GetComponent<TestingEnvironment>().Objectives;
        //results[i].GetValue("Description");
    }

}