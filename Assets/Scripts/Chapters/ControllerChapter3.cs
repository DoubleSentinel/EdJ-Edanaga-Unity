using DG.Tweening;
using Doozy.Engine;
using Doozy.Engine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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

    [Header("List scene")]
    [SerializeField] private GameObject[] alternativesInitial;
    [SerializeField] private GameObject[] alternativesInformed;
    [SerializeField] private GameObject[] panelsInitial;
    [SerializeField] private GameObject[] panelsInformed;

    [Header("Matrix Drag&Drop scene")]
    [SerializeField] private GameObject[] alternativesDnD;
    [SerializeField] private GameObject[] panelsDnD;

    private string[] panelIds;
    private string panelObjectValue;

    [Header("Drag&Drop result")]
    [SerializeField] private bool enableFlag;
    [SerializeField] private List<string> dragNdropResUninformed;
    [SerializeField] private List<string> dragNdropResMCDA;
    [SerializeField] private List<string> dragNdropResInformed;

    [SerializeField] private GameObject buttonToConv1;
    [SerializeField] private GameObject buttonToConv2;

    [Header("Ojectives description")]
    [SerializeField] private List<string> texts;

    [Header("Popup Values")] public string PopupName = "Popup1";
    [SerializeField] private string Title = "Title";
    [SerializeField] private GameObject MessageObject;
    [SerializeField] private string Message = "Popup message for player";

    // Local variables
    private GameObject controllers;

    // BargainConversation vars
    //[HideInInspector] public int conversationIndex = 0;
    public int conversationIndex = 0;
    public ConversationHandler.ConversationEnd conversationCallback;

    public List<string> DragNdropResMCDA { get => dragNdropResMCDA; set => dragNdropResMCDA = value; }
    public bool EnableFlag { get => enableFlag; set => enableFlag = value; }
    public List<string> DragNdropResInformed { get => dragNdropResInformed; set => dragNdropResInformed = value; }

    public string fromState = "Consistant0";

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

            //3.2_Consistent_check_Chap6 (if consistant!)
            if (conversationIndex == 2)
            {
                fromState = "Consistant0";
                //AdaptUI(fromState);
                SetConversationIndex(6);
                print("Conversation2");
                GameEventMessage.SendEvent("ContinueToConv");
                GameEventMessage.SendEvent("ContinueToConv");
                //StartConversation();
            }
            //3.2_Inconsistent_check_Chap6 (if not consistant!)
            if (conversationIndex == 3)
            {
                GameEventMessage.SendEvent("ContinueToMatrix");
                HideGo(altDnDMessage1);
                ShowGo(altDnDMessage2);
                HideGo(buttonToConv1);
                EnableFlag = true;
                DisableEnableDnD();
                print("ContinueToMatrix - DnD allowed");
            }

            //3.3.1_Informed_ranking_Chap6
            if (conversationIndex == 4)
            {
                GameEventMessage.SendEvent("ContinueToList");
                print("ContinueToList");
            }
            //3.4.1_Multiple_choices_rankings_Chap6
            if (conversationIndex == 5)
            {
                GameEventMessage.SendEvent("ContinueToList");
                print("ContinueToList");
            }

            //3.5.1_Consistent_choice_Chap6 (if consistant!)
            if (conversationIndex == 6)
            {
                print("Conversation6");
                GameEventMessage.SendEvent("ContinueToList");
                print("ContinueToList");
            }
            //3.6_Inconsistent_but_ok_Chap6 (if not consistant!)
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
        DragNdropResMCDA = new List<string>();
        DragNdropResInformed = new List<string>();

        //Get uninformed alternative values from TestingEnvironment
        var alt = controllers.GetComponent<TestingEnvironment>().AlternativesUninformed;
        dragNdropResUninformed.Clear();
        alt.ForEach((item) => { dragNdropResUninformed.Add((string)item.Clone()); });

        //Get MCDA alternative values from TestingEnvironment
        var altObj = controllers.GetComponent<TestingEnvironment>().AlternativesMCDA;
        dragNdropResMCDA.Clear();
        altObj.ForEach((item) => { dragNdropResMCDA.Add((string)item.Clone()); });
        dragNdropResInformed.Clear();
        altObj.ForEach((item) => { dragNdropResInformed.Add((string)item.Clone()); });

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

        DnD_ResultUninformed();
        DnD_ResultMCDA();

        EnableFlag = false;
        DisableEnableDnD();
        //GetObjectives();
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
        //GameEventMessage.SendEvent("GoToConversation");
        SetupConversation();
        Conv(conversationIndex);
    }

    // --------------------  UI Callables  --------------------------------
    public void SetConversationIndex(int index)
    {
        if (index == 2 && !TestCoherent())
        {
            conversationIndex = index + 1;
        }
        else
        {
            conversationIndex = index;
        }
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

    
    public void DnD_ResultUninformed()
    {
        string alternativeName;
        int alternativeNumber = 0;

        //Set player choice
        for (int i = 0; i < panels.Length; i++)
        {
            alternativeName = dragNdropResUninformed[i].ToString();
            alternativeNumber = Convert.ToInt32($"{alternativeName.Last()}");
            //Set player choice
            alternatives[alternativeNumber].gameObject.transform.position = panels[i].gameObject.transform.position;
        }
        //Set player choice
        for (int i = 0; i < panels.Length; i++)
        {
            alternativeName = dragNdropResMCDA[i].ToString();
            alternativeNumber = Convert.ToInt32($"{alternativeName.Last()}");
            //Set DnD default values (player choice MCDA)
            alternativesDnD[alternativeNumber].gameObject.transform.position = panelsDnD[i].gameObject.transform.position;
        }
    }
    

    public void DnD_ResultMCDA()
    {
        string alternativeName;
        int alternativeNumber = 0;

        //Set player choice
        for (int i = 0; i < panelsInitial.Length; i++)
        {
            alternativeName = dragNdropResUninformed[i].ToString();
            alternativeNumber = Convert.ToInt32($"{alternativeName.Last()}");
            //Fix player choice
            alternativesInitial[alternativeNumber].gameObject.transform.position = panelsInitial[i].gameObject.transform.position;
        }
        //Set player choice
        for (int i = 0; i < panelsInformed.Length; i++)
        {
            alternativeName = dragNdropResMCDA[i].ToString();
            alternativeNumber = Convert.ToInt32($"{alternativeName.Last()}");
            //Fix player choice
            alternativesInformed[alternativeNumber].gameObject.transform.position = panelsInformed[i].gameObject.transform.position;
        }
    }

    public void CheckPriorities()
    {
        ShowGo(buttonToConv2);

        //Reset the list of the Drag&Drops result
        DragNdropResInformed.Clear();

        //Update Drag & Drop results
        for (int i = 0; i < alternativesDnD.Length; i++)
        {
            panelObjectValue = DragDropManager.GetPanelObject(panelIds[i]);
            DragNdropResInformed.Add(panelObjectValue);
        }

        //Set new informed alternatives values to TestingEnvironment
        var alt2 = controllers.GetComponent<TestingEnvironment>().AlternativesInformed;
        alt2.Clear();
        DragNdropResInformed.ForEach((item) => { alt2.Add((string)item.Clone()); });
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

    public bool TestCoherent()
    {
        //If the 4th element of the two lists are the same
        for (int i = 0; i < 4; i++)
        {
            //print (i + ": Unifmed :" + dragNdropResUninformed[i] + " : informed: " + dragNdropResInformed[i]);
            if (dragNdropResUninformed[i].ToLower() != dragNdropResInformed[i].ToLower())
            {
                return false;
            }
        }
        return true;
    }

    public void AdaptUI()
    {

    }

    public void RedoAll()
    {
        controllers.GetComponent<TestingEnvironment>().SkipSwing = false;
        controllers.GetComponent<TestingEnvironment>().SkipTradeOff = false;
        SceneManager.LoadScene("Chapter2.2");
    }
    public void RedoSwing()
    {
        controllers.GetComponent<TestingEnvironment>().SkipSwing = false;
        controllers.GetComponent<TestingEnvironment>().SkipTradeOff = true;
        SceneManager.LoadScene("Chapter2.2");
    }

    //Get objectives texts from TestingEnvironment 
    public void GetObjectives()
    {
        //Get Objectives values to set the Matrix labels
        var objectives = controllers.GetComponent<TestingEnvironment>().Objectives;

        // Reset the list of the texts
        texts.Clear();

        //Get texts of the objectives description
        for (int i = 0; i < 10; i++)
        {
            texts.Add(objectives.ElementAt(i).Value.description);
        }
    }
}