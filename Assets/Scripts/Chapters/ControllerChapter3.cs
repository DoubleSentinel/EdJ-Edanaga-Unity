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
    [SerializeField] private GameObject[] alternatives1;
    [SerializeField] private GameObject[] alternatives2;
    [SerializeField] private GameObject[] panels1;
    [SerializeField] private GameObject[] panels2;

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

    [Header("List elemts")]
    [SerializeField] private GameObject label_Inconsistent_ranking;
    [SerializeField] private GameObject label_Consistent1_ranking;
    [SerializeField] private GameObject label_Consistent2_ranking;

    [SerializeField] private GameObject ButtonInconsistent_informed;
    [SerializeField] private GameObject Button1st_initial;
    [SerializeField] private GameObject Button1st_pick_ranking;
    [SerializeField] private GameObject Button2orMore_informed;
    [SerializeField] private GameObject ButtonInconsistent_MCDA;
    [SerializeField] private GameObject Button1st_MCDA;
    [SerializeField] private GameObject Button2orMore_MCDA;
    [SerializeField] private GameObject ButtonNone;

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

    public int fromState = 0;
    private List<string> alternativeNames1;
    private List<string> alternativeNames2;

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
                print("ContinueToMatrix - DnD not allowed");
                GameEventMessage.SendEvent("ContinueToMatrix");
                ShowGo(altDnDMessage1);
                HideGo(altDnDMessage2);
                ShowGo(buttonToConv1);
                HideGo(buttonToConv2);
                EnableFlag = false;
                DisableEnableDnD();
            }
            //3.2_Consistent_check_Chap6
            if (conversationIndex == 2)
            {
                print("ConversationConsistent");
                //Go to the next conversation
                SetConversationIndex(6);
                GameEventMessage.SendEvent("ContinueToNextConv");
            }
            //3.2_Inconsistent_check_Chap6
            if (conversationIndex == 3)
            {
                print("ConversationInconsistent");
                if (!EnableFlag) //1st DnD or coming back from 3.4
                {
                    //Go to the next convversation
                    SetConversationIndex(4);
                    GameEventMessage.SendEvent("ContinueToNextConv");
                }
                else
                {
                    //Go to the next convversation
                    SetConversationIndex(5);
                    GameEventMessage.SendEvent("ContinueToNextConv");
                }
            }
            //3.3.1_Informed_ranking_Chap6
            if (conversationIndex == 4)
            {
                print("ContinueToMatrix - DnD allowed");
                GameEventMessage.SendEvent("ContinueToMatrix");
                HideGo(altDnDMessage1);
                ShowGo(altDnDMessage2);
                HideGo(buttonToConv1);
                EnableFlag = true;
                DisableEnableDnD();
            }
            //3.4.1_Multiple_choices_rankings_Chap6
            if (conversationIndex == 5)
            {
                fromState = 1; //3.4.2
                AdaptListUI(fromState);
                GameEventMessage.SendEvent("ContinueToList");
                print("ContinueToList");
            }
            //3.5.1_Consistent_choice_Chap6
            if (conversationIndex == 6)
            {
                print("ContinueToList");
                fromState = 0; //3.5.2 or 3.5.3
                AdaptListUI(fromState);
                GameEventMessage.SendEvent("ContinueToList");
            }
            //3.6_Inconsistent_but_ok_Chap6 (if not consistant!)
            if (conversationIndex == 7)
            {
                print("ContinueToEnd");
                //Go to the next convversation
                SetConversationIndex(8);
                GameEventMessage.SendEvent("ContinueToNextConv");
            }
            //3.7_Outro_Chap6
            if (conversationIndex == 8)
            {
                GameEventMessage.SendEvent("ContinueToChapter4");
                print("ContinueToChapter4");
            }
        };
    }

    public void NextConv()
    {
        GameEventMessage.SendEvent("ContinueToConv");
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

        DnD_ResultInitial();

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
        SetupConversation();
        Conv(conversationIndex);
    }

    // --------------------  UI Callables  --------------------------------
    public void SetConversationIndex(int index)
    {
        if (index == 2 && !TestCoherent())
        {
            controllers.GetComponent<TestingEnvironment>().ConsistentFirst = false;
            conversationIndex = index + 1;
        }
        else
        {
            if (TestAreTheSame())
            {
                SetPreferedUser(0); //Consistent 100%
                conversationIndex = 8;
            }
            else
            {
                conversationIndex = index;
            }
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


    public void DnD_ResultInitial()
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
        if (!enableFlag)
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

    public bool TestAreTheSame()
    { 
        return dragNdropResUninformed.SequenceEqual(dragNdropResInformed);
    }

    public void AdaptListUI(int fromState)
    {
        label_Inconsistent_ranking.gameObject.SetActive(false); //0
        label_Consistent1_ranking.gameObject.SetActive(false); //1
        label_Consistent2_ranking.gameObject.SetActive(false); //2 Used
        ButtonInconsistent_informed.gameObject.SetActive(false); //3
        Button1st_initial.gameObject.SetActive(false); //4
        Button1st_pick_ranking.gameObject.SetActive(false); //5 Not used
        Button2orMore_informed.gameObject.SetActive(false); //6 Used
        ButtonInconsistent_MCDA.gameObject.SetActive(false); //7
        Button1st_MCDA.gameObject.SetActive(false); //8
        Button2orMore_MCDA.gameObject.SetActive(false); //9 Used
        ButtonNone.gameObject.SetActive(false); //10
        //.transform.localScale = new Vector3(0, 0, 0);
        //.gameObject.GetComponent<Renderer>().enabled = false;

        if (fromState == 0) //3.5.2 - 1st or 3.5.3 - 2nd or more
        {
            if (controllers.GetComponent<TestingEnvironment>().ConsistentFirst == true)
            { 
                label_Consistent1_ranking.gameObject.SetActive(true); //1
                Button1st_initial.gameObject.SetActive(true); //4
                Button1st_MCDA.gameObject.SetActive(true); //8
                controllers.GetComponent<TestingEnvironment>().ConsistentFirst = false;
            }
            else
            {
                label_Consistent2_ranking.gameObject.SetActive(true); //2
                Button2orMore_informed.gameObject.SetActive(true); //5
                Button2orMore_MCDA.gameObject.SetActive(true); //8
            }

        }
        if (fromState == 1) //3.4.2
        {
            label_Inconsistent_ranking.gameObject.SetActive(true); //0
            ButtonInconsistent_informed.gameObject.SetActive(true); //3
            ButtonInconsistent_MCDA.gameObject.SetActive(true); //7
            ButtonNone.gameObject.SetActive(true); //10
        }
        DnD_ResultAdapt(fromState);
    }

    public void DnD_ResultAdapt(int caseState)
    {
        string alternativeName;
        int alternativeNumber = 0;

        switch (caseState)
        {
            case 0:
                if (controllers.GetComponent<TestingEnvironment>().ConsistentFirst == true)
                {
                    alternativeNames1 = dragNdropResUninformed;
                    alternativeNames2 = dragNdropResMCDA;
                }
                else
                { 
                    alternativeNames1 = dragNdropResInformed;
                    alternativeNames2 = dragNdropResMCDA;
                }
                break;
            case 1:
                alternativeNames1 = dragNdropResInformed.ToList();
                alternativeNames2 = dragNdropResMCDA;
                break;
            default:
                break;
        }

        //Set player choice
        for (int i = 0; i < panels1.Length; i++)
        {
            alternativeName = alternativeNames1[i];
            alternativeNumber = Convert.ToInt32($"{alternativeName.Last()}");
            //Fix player choice
            alternatives1[alternativeNumber].gameObject.transform.position = panels1[i].gameObject.transform.position;
        }

        //Set player choice
        for (int i = 0; i < panels2.Length; i++)
        {
            alternativeName = alternativeNames2[i];
            alternativeNumber = Convert.ToInt32($"{alternativeName.Last()}");
            //Fix player choice
            alternatives2[alternativeNumber].gameObject.transform.position = panels2[i].gameObject.transform.position;
        }
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

    public void SetPreferedUser(int choice)
    {
        switch (choice)
        {
            case 0:
                if (controllers.GetComponent<TestingEnvironment>().ConsistentFirst == true)
                {
                    controllers.GetComponent<TestingEnvironment>().PreferedUser = "Consistant1st";
                }
                else
                {
                    controllers.GetComponent<TestingEnvironment>().PreferedUser = "Consistant2ndOrMore";
                }
                break;
            case 1:
                controllers.GetComponent<TestingEnvironment>().PreferedUser = "Uninformed";
                break;
            case 2:
                controllers.GetComponent<TestingEnvironment>().PreferedUser = "MCDA";
                break;
            case 3:
                controllers.GetComponent<TestingEnvironment>().PreferedUser = "Informed";
                break;
            default:
                break;
        }
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
 