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

    [Header("Matrix Drag&Drop scene")]
    [SerializeField] private GameObject[] alternativesDnD;
    [SerializeField] private GameObject[] panelsDnD;
    [SerializeField] private GameObject panelLabel;
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
    [SerializeField] private GameObject label_VeryConsistent1_ranking;
    [SerializeField] private GameObject label_VeryConsistent2_ranking;

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

    private ConversationHandler.ConversationEnd[] conversationCallbacks;

    //User preference possible choice
    private enum Prefered
    {
        MCDA,
        Uniformed,
        Informed
    }

    //3.1_Intro_journalist_Chap6
    private void Intro_journalist_Chap6()
    {
        GameEventMessage.SendEvent("ContinueToAlt");
        ShowGo(buttonToConv);
    }

    //3.1.3_Intro_engineer_Chap6
    private void Intro_engineer_Chap6()
    {
        GameEventMessage.SendEvent("ContinueToMatrix");
        ShowGo(altDnDMessage1);
        HideGo(altDnDMessage2);
        ShowGo(buttonToConv1);
        HideGo(buttonToConv2);
    }

    //3.2_Consistent_check_Chap6
    private void Consistent_check_Chap6()
    {
        //Go to the next conversation
        SetConversationIndex(6);
        GameEventMessage.SendEvent("ContinueToNextConv");
    }

    //3.2_Inconsistent_check_Chap6
    private void Inconsistent_check_Chap6()
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
    private void Informed_ranking_Chap6()
    {
        GameEventMessage.SendEvent("ContinueToMatrix");
        HideGo(altDnDMessage1);
        ShowGo(altDnDMessage2);
        HideGo(buttonToConv1);
        EnableFlag = true;
        ToggleDnD();
        EnableOptions(false);
    }

    //3.4.1_Multiple_choices_rankings_Chap6
    private void Multiple_choices_rankings_Chap6()
    {
        fromState = 1; //3.4.2
        AdaptListUI(fromState);
        GameEventMessage.SendEvent("ContinueToList");
    }

    //3.5.1_Consistent_choice_Chap6
    private void Consistent_choice_Chap6()
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

    //3.6_Inconsistent_but_ok_Chap6
    private void Inconsistent_but_ok_Chap6()
    {
        //Go to the next convversation
        SetConversationIndex(8);
        GameEventMessage.SendEvent("ContinueToNextConv");
    }

    //3.7_Outro_Chap6
    private void Outro_Chap6()
    {
        GameEventMessage.SendEvent("ContinueToChapter4");
    }

    void Awake()
    {
        controllers = GameObject.Find("Controllers");
        conversationCallbacks = new ConversationHandler.ConversationEnd[9];
        conversationCallbacks[0] = Intro_journalist_Chap6;
        conversationCallbacks[1] = Intro_engineer_Chap6;
        conversationCallbacks[2] = Consistent_check_Chap6;
        conversationCallbacks[3] = Inconsistent_check_Chap6;
        conversationCallbacks[4] = Informed_ranking_Chap6;
        conversationCallbacks[5] = Multiple_choices_rankings_Chap6;
        conversationCallbacks[6] = Consistent_choice_Chap6;
        conversationCallbacks[7] = Inconsistent_but_ok_Chap6;
        conversationCallbacks[8] = Outro_Chap6;
    }

    private void Start()
    {
        controllers.GetComponent<LanguageHandler>().translateUI();

        panelIds = new string[6];
        DragNdropResMCDA = new int[6];
        DragNdropResInformed = new int[6];

        controllers.GetComponent<TestingEnvironment>().AlternativesMCDA = CalculateMCDA();

        //Get uninformed alternative values from TestingEnvironment
        var alt = controllers.GetComponent<TestingEnvironment>().AlternativesUninformed;
        Array.Clear(dragNdropResUninformed, 0, dragNdropResUninformed.Length);
        dragNdropResUninformed = (int[])alt.Clone();
        

        //Get MCDA alternative values from TestingEnvironment
        var altObj = controllers.GetComponent<TestingEnvironment>().AlternativesMCDA;
        Array.Clear(dragNdropResMCDA, 0, dragNdropResMCDA.Length);
        dragNdropResMCDA = (int[])altObj.Clone();

        //Get informed alternative values from TestingEnvironment
        var altInformed = controllers.GetComponent<TestingEnvironment>().AlternativesInformed;
        Array.Clear(dragNdropResInformed, 0, dragNdropResInformed.Length);
        dragNdropResInformed = (int[])altInformed.Clone();

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
        SetObjectives();
        EnableOptions(true);
    }

    private void Conv()
    {
        var ch = ConversationBubbles[0].GetComponent<ConversationHandler>();
        ch.callback = conversationCallbacks[conversationIndex];
        ch.GenerateConversation(conversationIndex);
        ch.NextConversationSnippet();
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

    //Set objectives texts in the prefab
    private void SetObjectives()
    {
        //Get Objectives values to set the Matrix labels
        var objectives = controllers.GetComponent<TestingEnvironment>().Objectives;

        //Set texts of the objectives description
        for (int i = 0; i < 10; i++)
        {
            panelLabel.gameObject.transform.GetChild(0).GetChild(i).GetComponent<TMPro.TextMeshProUGUI>().text = objectives.ElementAt(i).Value.description;
        }
    }

    private void CheckPriorities()
    {
        ShowGo(buttonToConv2);

        //Reset the array with the Drag&Drop results
        Array.Clear(dragNdropResInformed, 0, dragNdropResInformed.Length);

        //Get informed alternatives values from TestingEnvironment
        var alt = controllers.GetComponent<TestingEnvironment>().AlternativesInformed;

        //Update Drag & Drop results
        for (int i = 0; i < alternativesDnD.Length; i++)
        {
            panelObjectValue = DragDropManager.GetPanelObject(panelIds[i]);
            DragNdropResInformed[i] = Convert.ToInt32($"{panelObjectValue}");
            alt[i] = DragNdropResInformed[i]; //Update new informed alternatives values to TestingEnvironmen
        }  
    }

    //Lock and unlock the drag&drop property of the elements
    private void ToggleDnD()
    {
        for (int i = 0; i < alternativesDnD.Length; i++)
        {
            alternativesDnD[i].GetComponent<ObjectSettings>().LockObject = !EnableFlag;
            if(EnableFlag) //Set default values and position
            {
                //Set DnD default values
                alternativesDnD[i].gameObject.transform.position = panelsDnD[i].gameObject.transform.position;
            }
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
        label_Inconsistent_ranking.gameObject.SetActive(false);
        label_Consistent1_ranking.gameObject.SetActive(false);
        label_Consistent2_ranking.gameObject.SetActive(false);
        label_VeryConsistent1_ranking.gameObject.SetActive(false);
        label_VeryConsistent2_ranking.gameObject.SetActive(false);
        buttonInconsistent_informed.gameObject.SetActive(false);
        button1st_initial.gameObject.SetActive(false);
        button1st_pick_ranking.gameObject.SetActive(false);
        button2orMore_informed.gameObject.SetActive(false);
        buttonInconsistent_MCDA.gameObject.SetActive(false);
        button1st_MCDA.gameObject.SetActive(false);
        button2orMore_MCDA.gameObject.SetActive(false);
        buttonNone.gameObject.SetActive(false);
        buttonNext.gameObject.SetActive(false);
        priorities3.gameObject.SetActive(false);

        if (fromState == 0) //3.5.2 - 1st or 3.5.3 - 2nd or more
        {
            //3.5.2 - 1st
            if (controllers.GetComponent<TestingEnvironment>().ConsistentFirst == true)
            { 
                label_Consistent1_ranking.gameObject.SetActive(true);
                button1st_initial.gameObject.SetActive(true);
                button1st_MCDA.gameObject.SetActive(true);
                controllers.GetComponent<TestingEnvironment>().ConsistentFirst = false;
            }
            else //3.5.3 - 2nd or more
            {
                label_Consistent2_ranking.gameObject.SetActive(true);
                button2orMore_informed.gameObject.SetActive(true);
                button2orMore_MCDA.gameObject.SetActive(true);
            }
        }
        if (fromState == 1) //3.4.2
        {
            label_Inconsistent_ranking.gameObject.SetActive(true);
            buttonInconsistent_informed.gameObject.SetActive(true);
            buttonInconsistent_MCDA.gameObject.SetActive(true);
            buttonNone.gameObject.SetActive(true);
        }
        if (fromState == 2) //3.5.4 Consistent 100%
        {
            label_VeryConsistent1_ranking.gameObject.SetActive(true);
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

    //Set MCDA calculated result
    private int[] CalculateMCDA()
    {
        int[] result = new int[6] { 0, 2, 1, 4, 3, 5 };
        return result;
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
            conversationIndex = index;
        }
    }

    public void StartConversation()
    {
        SetupConversation();
        Conv();
    }

    //Used to allows to consecutive conversations
    public void NextConv()
    {
        GameEventMessage.SendEvent("ContinueToConv");
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
        controllers.GetComponent<TestingEnvironment>().UserPreference = ((Prefered)choice).ToString();
    }

    //Play specific UI sound
    public void PlaySoundUI(string mySoundName)
    {
        controllers.GetComponent<SoundsController>().PlaySoundUI(mySoundName);
    }

    //Play specific Ambiance sound
    public void PlaySoundAmbiance(string mySoundName)
    {
        controllers.GetComponent<SoundsController>().PlaySoundAmbiance(mySoundName);
    }

    //Play specific Music sound
    public void PlaySoundMusic(string mySoundName)
    {
        controllers.GetComponent<SoundsController>().PlaySoundMusic(mySoundName);
    }

    //Enable or disable options wheel
    public void EnableOptions(bool enable)
    {
        controllers.GetComponent<AudioManager>().EnableOptionWheel(enable); //options allowed or not allowed
    }
}
