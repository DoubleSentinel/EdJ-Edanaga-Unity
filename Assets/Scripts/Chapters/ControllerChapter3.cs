using Doozy.Engine;
using Doozy.Engine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
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

    [SerializeField] private GameObject objectivesObject;

    [Header("Matrix Drag&Drop scene")]
    [SerializeField] private GameObject[] alternativesDnD;
    [SerializeField] private GameObject[] panelsDnD;
    private string[] panelIds;
    private string panelObjectValue;

    [Header("Drag&Drop result")]
    [SerializeField] private int[] dragNdropResUninformed;
    [SerializeField] private int[] dragNdropResMCDA;
    [SerializeField] private int[] dragNdropResInformed;

    [SerializeField] private GameObject buttonToConv1;
    [SerializeField] private GameObject buttonToConv2;

    [Header("List scene design")]
    public Color BackgroundColor = new Color(0.37f, 0.58f, 0.82f, 0.7f);
    public Color FillColor = new Color(0.37f, 0.58f, 0.82f, 0.7f);
    [SerializeField] private GameObject resultListItemPrefab;
    [SerializeField] private GameObject resultList1;
    [SerializeField] private GameObject resultList2;
    [SerializeField] private GameObject resultList3;

    [Header("List scene elements")]
    [SerializeField] private GameObject priorities1;
    [SerializeField] private GameObject priorities2;
    [SerializeField] private GameObject priorities3;
    [SerializeField] private GameObject label_Inconsistent_ranking;
    [SerializeField] private GameObject label_Consistent1_ranking;
    [SerializeField] private GameObject label_Consistent2_ranking;

    [SerializeField] private GameObject buttonInconsistent_informed;
    [SerializeField] private GameObject button1st_initial;
    [SerializeField] private GameObject button1st_pick_ranking;
    [SerializeField] private GameObject button2orMore_informed;
    [SerializeField] private GameObject buttonInconsistent_MCDA;
    [SerializeField] private GameObject button1st_MCDA;
    [SerializeField] private GameObject button2orMore_MCDA;
    [SerializeField] private GameObject buttonNone;
    [SerializeField] private GameObject buttonNext;

    [Header("Popup Values")]
    [SerializeField] private string popupName = "Popup1";
    [SerializeField] private string title = "Title";
    [SerializeField] private GameObject messageObject;
    [SerializeField] private string message = "Popup message for player";

    // Local variables
    private GameObject controllers;

    // BargainConversation vars
    private int conversationIndex = 0;
    public ConversationHandler.ConversationEnd conversationCallback;

    public int[] DragNdropResMCDA { get => dragNdropResMCDA; set => dragNdropResMCDA = value; }
    private bool EnableFlag { get; set; }
    public int[] DragNdropResInformed { get => dragNdropResInformed; set => dragNdropResInformed = value; }

    private int fromState = 0;
    private int[] alternativeNumber1;
    private int[] alternativeNumber2;
    private List<string> alternativesDescription = new List<string>();

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
            }
            //3.2_Consistent_check_Chap6
            if (conversationIndex == 2)
            {
                //Go to the next conversation
                SetConversationIndex(6);
                GameEventMessage.SendEvent("ContinueToNextConv");
            }
            //3.2_Inconsistent_check_Chap6
            if (conversationIndex == 3)
            {
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
                GameEventMessage.SendEvent("ContinueToMatrix");
                HideGo(altDnDMessage1);
                ShowGo(altDnDMessage2);
                HideGo(buttonToConv1);
                EnableFlag = true;
                ToggleDnD();
            }
            //3.4.1_Multiple_choices_rankings_Chap6
            if (conversationIndex == 5)
            {
                fromState = 1; //3.4.2
                AdaptListUI(fromState);
                GameEventMessage.SendEvent("ContinueToList");
            }
            //3.5.1_Consistent_choice_Chap6
            if (conversationIndex == 6)
            {
                if (TestAreTheSame())
                {
                    SetPreferedUser(0); //Consistent 100%
                    fromState = 2; //3.5.4
                }
                else
                {
                    fromState = 0; //3.5.2 or 3.5.3
                }
                AdaptListUI(fromState);
                GameEventMessage.SendEvent("ContinueToList");
            }
            //3.6_Inconsistent_but_ok_Chap6 (if not consistent!)
            if (conversationIndex == 7)
            {
                //Go to the next convversation
                SetConversationIndex(8);
                GameEventMessage.SendEvent("ContinueToNextConv");
            }
            //3.7_Outro_Chap6
            if (conversationIndex == 8)
            {
                GameEventMessage.SendEvent("ContinueToChapter4");
            }
        };
    }

    private void Start()
    {
        controllers.GetComponent<LanguageHandler>().translateUI();

        panelIds = new string[6];
        DragNdropResMCDA = new int[6];
        DragNdropResInformed = new int[6];

        //Get uninformed alternative values from TestingEnvironment
        var alt = controllers.GetComponent<TestingEnvironment>().AlternativesUninformed;
        Array.Clear(dragNdropResUninformed, 0, dragNdropResUninformed.Length);
        dragNdropResUninformed = (int[])alt.Clone();
        

        //Get MCDA alternative values from TestingEnvironment
        var altObj = controllers.GetComponent<TestingEnvironment>().AlternativesMCDA;
        Array.Clear(dragNdropResMCDA, 0, dragNdropResMCDA.Length);
        dragNdropResMCDA = (int[])altObj.Clone();

        Array.Clear(dragNdropResInformed, 0, dragNdropResInformed.Length);
        dragNdropResInformed = (int[])altObj.Clone();

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

        EnableFlag = false; //Disable Drag and Drop 
        ToggleDnD();
        //SetObjectives();
    }

    private void StartConversation()
    {
        SetupConversation();
        Conv(conversationIndex);
    }

    private void NextConv()
    {
        GameEventMessage.SendEvent("ContinueToConv");
    }

    public void SetConversationIndex(int index)
    {
        if (index == 2 && !TestCoherent())
        {
            controllers.GetComponent<TestingEnvironment>().ConsistentFirst = false;
            conversationIndex = index + 1;
        }
        else
        {
            conversationIndex = index;
        }
    }

    private void SetupConversation()
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

    //User prefrence choice
    private enum Prefered
    {
        MCDA,
        Uniformed,
        Informed
    }


    private void DnD_ResultInitial()
    {
        string alternativeName;
        int alternativeNumber = 0;

        alternativesDescription = new List<string>();

        //Set uninformed alternatives
        for (int i = 0; i < panels.Length; i++)
        {
            alternativeName = dragNdropResUninformed[i].ToString();
            alternativeNumber = Convert.ToInt32($"{alternativeName.Last()}");
            //Set player choice
            alternatives[alternativeNumber].gameObject.transform.position = panels[i].gameObject.transform.position;
        }
        //Set MCDA alternatives
        for (int i = 0; i < panels.Length; i++)
        {
            alternativeName = dragNdropResMCDA[i].ToString();
            alternativeNumber = Convert.ToInt32($"{alternativeName.Last()}");
            //Set DnD default values
            alternativesDnD[alternativeNumber].gameObject.transform.position = panelsDnD[i].gameObject.transform.position;
        }

        //Save alternatives description
        alternativesDescription.Clear();
        // Getting alternatives gameobjects description
        for (int i = 0; i < alternatives.Length; i++)
        {
            string text = alternatives[i].gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text;
            alternativesDescription.Add(text);
        }
    }

    //Set objectives texts from TestingEnvironment 
    private void SetObjectives()
    {
        //Get Objectives values to set the Matrix labels
        var objectives = controllers.GetComponent<TestingEnvironment>().Objectives;

        //Set texts of the objectives description
        for (int i = 0; i < 10; i++)
        {
            objectivesObject.transform.GetChild(i).GetComponent<TMPro.TextMeshProUGUI>().text = objectives.ElementAt(i).Value.description;
        }
    }

    private void CheckPriorities()
    {
        ShowGo(buttonToConv2);

        //Reset the array with the Drag&Drop results
        Array.Clear(dragNdropResInformed, 0, dragNdropResInformed.Length);

        //Update Drag & Drop results
        for (int i = 0; i < alternativesDnD.Length; i++)
        {
            panelObjectValue = DragDropManager.GetPanelObject(panelIds[i]);
            DragNdropResInformed[i] = Convert.ToInt32($"{panelObjectValue}");
        }

        //Update new informed alternatives values to TestingEnvironment
        var alt = controllers.GetComponent<TestingEnvironment>().AlternativesInformed;
        Array.Clear(alt, 0, alt.Length);
        alt = (int[])DragNdropResInformed.Clone();  
    }

    //Lock and unlock the drag&drop property of the elements
    private void ToggleDnD()
    {
        for (int i = 0; i < alternativesDnD.Length; i++)
        {
            alternativesDnD[i].GetComponent<ObjectSettings>().LockObject = !EnableFlag;
        }
        if (EnableFlag)
            ShowPopup();
    }

    private void ShowPopup()
    {
        //Getting a clone of the UIPopup, with the given PopupName, from the UIPopup Database 
        UIPopup popup = UIPopup.GetPopup(popupName);

        title = "Consignes";
        message = messageObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;
        popup.Data.SetLabelsTexts(title, message);

        popup.Show(); //show the popup
    }

    //Return true if the 4 first elements of the two Array are the same
    private bool TestCoherent()
    {
        return dragNdropResUninformed.Take(4).SequenceEqual(dragNdropResInformed.Take(4));
    }

    //Return true if the all the elements of the two Array are the same
    private bool TestAreTheSame()
    {
        return dragNdropResUninformed.SequenceEqual(dragNdropResInformed);
    }

    //Adapt UI view depending of the current state
    private void AdaptListUI(int fromState)
    {
        label_Inconsistent_ranking.gameObject.SetActive(false); //0
        label_Consistent1_ranking.gameObject.SetActive(false); //1
        label_Consistent2_ranking.gameObject.SetActive(false); //2 Used
        buttonInconsistent_informed.gameObject.SetActive(false); //3
        button1st_initial.gameObject.SetActive(false); //4
        button1st_pick_ranking.gameObject.SetActive(false); //5 Not used
        button2orMore_informed.gameObject.SetActive(false); //6 Used
        buttonInconsistent_MCDA.gameObject.SetActive(false); //7
        button1st_MCDA.gameObject.SetActive(false); //8
        button2orMore_MCDA.gameObject.SetActive(false); //9 Used
        buttonNone.gameObject.SetActive(false); //10
        buttonNext.gameObject.SetActive(false); //11
        //priorities1.gameObject.SetActive(false);
        //priorities2.gameObject.SetActive(false);
        priorities3.gameObject.SetActive(false);

        if (fromState == 0) //3.5.2 - 1st or 3.5.3 - 2nd or more
        {
            //3.5.2 - 1st
            if (controllers.GetComponent<TestingEnvironment>().ConsistentFirst == true)
            { 
                label_Consistent1_ranking.gameObject.SetActive(true); //1
                button1st_initial.gameObject.SetActive(true); //4
                button1st_MCDA.gameObject.SetActive(true); //8
                controllers.GetComponent<TestingEnvironment>().ConsistentFirst = false;
            }
            else //3.5.3 - 2nd or more
            {
                label_Consistent2_ranking.gameObject.SetActive(true); //2
                button2orMore_informed.gameObject.SetActive(true); //5
                button2orMore_MCDA.gameObject.SetActive(true); //8
            }

        }
        if (fromState == 1) //3.4.2
        {
            label_Inconsistent_ranking.gameObject.SetActive(true); //0
            buttonInconsistent_informed.gameObject.SetActive(true); //3
            buttonInconsistent_MCDA.gameObject.SetActive(true); //7
            buttonNone.gameObject.SetActive(true); //10
        }
        if (fromState == 2) //3.5.4 Consistent 100%
        {
            label_Consistent1_ranking.gameObject.SetActive(true); //1
            buttonNext.gameObject.SetActive(true); //11
            priorities1.gameObject.SetActive(false);
            priorities2.gameObject.SetActive(false);
            priorities3.gameObject.SetActive(true);

        }
        DnD_ResultInitial();
        DnD_ResultAdapt(fromState);
    }

    //Adapt the results lists to display
    private void DnD_ResultAdapt(int caseState)
    {
        //Prepare lists to display
        switch (caseState)
        {
            case 0:
                //Consistent - 1st try
                if (controllers.GetComponent<TestingEnvironment>().ConsistentFirst == true)
                {
                    alternativeNumber1 = dragNdropResUninformed;
                    alternativeNumber2 = dragNdropResMCDA;
                }
                else
                { 
                    alternativeNumber1 = dragNdropResInformed;
                    alternativeNumber2 = dragNdropResMCDA;
                }
                break;
            case 1:
                alternativeNumber1 = dragNdropResInformed;
                alternativeNumber2 = dragNdropResMCDA;
                break;
            case 2:
                alternativeNumber1 = dragNdropResMCDA;
                alternativeNumber2 = dragNdropResMCDA;
                break;
            default:
                break;
        }

        if(caseState==2)
        {
            //Display just one list
            UpdateResultList(resultList3, alternativeNumber1);
        }
        else
        {
            UpdateResultList(resultList1, alternativeNumber1);
            UpdateResultList(resultList2, alternativeNumber2);

        }
    }

    //Create the visualisation of the alternatives 
    private void UpdateResultList(GameObject resultList, int[] alternativeNames)
    {
        foreach (Transform child in resultList.transform)
        {
            Destroy(child.gameObject);
        }

        // creating the visual alternatives list with prefab
        foreach (int alternativeNumber in alternativeNames)
        {
            var resultItem = Instantiate(resultListItemPrefab, resultList.transform);
            string alternativeDescription = alternativesDescription[alternativeNumber];
            resultItem.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                $"{alternativeDescription}";

            // Set background color
            resultItem.GetComponent<Image>().color = BackgroundColor;
            // Set filled color
            resultItem.transform.GetChild(0).GetComponent<Image>().color = FillColor;

            var rt = resultItem.transform.GetChild(0).GetComponent<RectTransform>();
            rt.localScale = new Vector3(1, rt.localScale.y, rt.localScale.z);
        }
    }

    private void ShowGo(GameObject go)
    {
        go.SetActive(true);
    }

    private void HideGo(GameObject go)
    {
        go.SetActive(false);
    }

    // --------------------  UI Callables  --------------------------------

    public void Conv(int conversationIndex) //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    {
        var ch = ConversationBubbles[0].GetComponent<ConversationHandler>();
        ch.callback = conversationCallback;
        ch.GenerateConversation(conversationIndex);
        ch.NextConversationSnippet();
    }

    //Redo Swing and TradeOff
    public void RedoAll()
    {
        controllers.GetComponent<TestingEnvironment>().SkipSwing = false;
        controllers.GetComponent<TestingEnvironment>().SkipTradeOff = false;
        SceneManager.LoadScene("Chapter2.2");
    }

    //Redo Swing
    public void RedoSwing()
    {
        controllers.GetComponent<TestingEnvironment>().SkipSwing = false;
        controllers.GetComponent<TestingEnvironment>().SkipTradeOff = true;
        SceneManager.LoadScene("Chapter2.2");
    }

    //Update the User preference in TestingEnvironment
    public void SetPreferedUser(int choice)
    {
        switch (choice)
        {
            case 0:
                if (controllers.GetComponent<TestingEnvironment>().ConsistentFirst == true)
                {
                    //If 100% consistent
                    controllers.GetComponent<TestingEnvironment>().UserPreference = Prefered.MCDA;
                }
                else
                {
                    //If 100% consistent - 2nd or more
                    controllers.GetComponent<TestingEnvironment>().UserPreference = "MCDA";
                }
                break;
            case 1:
                controllers.GetComponent<TestingEnvironment>().UserPreference = "Uninformed";
                break;
            case 2:
                controllers.GetComponent<TestingEnvironment>().UserPreference = "MCDA";
                break;
            case 3:
                controllers.GetComponent<TestingEnvironment>().UserPreference = "Informed";
                break;
            default:
                print("SetPreferedUser : choice not allowed!");
                break;
        }
    }
}
