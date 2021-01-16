using DG.Tweening;
using Doozy.Engine;
using Doozy.Engine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControllerChapter1 : MonoBehaviour
{
    [Header("2D Scene References")] [SerializeField]
    private GameObject scenePlayer;

    [SerializeField] private GameObject sceneJournalist;
    [SerializeField] private GameObject sceneEngineer;

    [Header("Conversation References")] public GameObject[] ConversationBubbles;

    [Header("Drag&Drop scene")] [SerializeField]
    private GameObject[] alternatives;

    [SerializeField] private GameObject[] priorities;
    private string newPrioIds;

    [SerializeField] private GameObject prioritiesIcon;
    [SerializeField] private GameObject buttonToDnd;
    [SerializeField] private GameObject buttonToConv;
    [SerializeField] private GameObject altDnDMessage;
    [SerializeField] private GameObject altDiscoveryMessage;

    private string[] prioIds;
    public GameObject[] Panels;
    private GameObject NewPanels;

    public Color HiddenAltColor = new Color(0.37f, 0.58f, 0.82f, 0.7f);
    public Color VisibleAltColor = new Color(0.37f, 0.58f, 0.82f, 0);
    public Color VisibleAltColor25 = new Color(0.37f, 0.58f, 0.82f, 0.5f);

    [Header("Drag&Drop result")] [SerializeField]
    private int[] dragNdropRes;
    private string panelObjectValue;

    [Header("Popup Values")] public string PopupName = "Popup1";

    //[SerializeField] private GameObject TitleObject;
    [SerializeField] private string Title = "Title";
    [SerializeField] private GameObject MessageObject;
    [SerializeField] private string Message = "Popup message for player";

    // Local variables
    private GameObject controllers;

    // BargainConversation vars
    [HideInInspector] public int conversationIndex = 0;
    public ConversationHandler.ConversationEnd conversationCallback;

    public int[] DragNdropRes { get => dragNdropRes; set => dragNdropRes = value; }

    void Awake()
    {
        controllers = GameObject.Find("Controllers");
        conversationCallback = () => {
            if (conversationIndex == 0)
            {
                GameEventMessage.SendEvent("ContinueToAlt");
                ShowGo(altDiscoveryMessage);
            }
            else
            {
                GameEventMessage.SendEvent("ContinueToChapter2.1");
            }
        };
    }

    private void Start()
    {
        controllers.GetComponent<LanguageHandler>().translateUI();

        DragNdropRes = new int[6];
        prioIds = new string[6];
        Panels = new GameObject[6];

        // Setting alternatives gameobjects and panels for locking mechanism
        for (int i = 0; i < alternatives.Length; i++)
        {
            Panels[i] = alternatives[i].gameObject.transform.GetChild(2).gameObject;

            if (i == 0)
            {
                NextAlternative(Panels[i]);
                alternatives[i].GetComponent<UIButton>().Interactable = true;
            }
            else
            {
                HideAlternative(Panels[i]);
                alternatives[i].GetComponent<UIButton>().Interactable = false;
            }
        }

        //Get Priority Id name
        for (int i=0; i < priorities.Length; i++)
        {
            prioIds[i] = priorities[i].gameObject.GetComponent<PanelSettings>().Id;
        }

        //Default Setup
        HideGo(buttonToDnd);
        HideGo(buttonToConv);
        HideGo(altDnDMessage);
        HideGo(altDiscoveryMessage);
        DisableDnD();
    }

    // --------------------  UI Callables  --------------------------------
    public void SetConversationIndex(int index)
    {
        conversationIndex = index;
    }

    //Not used in the this chapter
    public void ClearCharacters()
    {
        foreach (GameObject character in GameObject.FindGameObjectsWithTag("Character"))
        {
            character.transform.position = new Vector3(11, 0, 1);
            character.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

    public void SetupHostConversation()
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

        var ch = ConversationBubbles[0].GetComponent<ConversationHandler>();
        ch.callback = conversationCallback;
        ch.GenerateConversation(conversationIndex);
        ch.NextConversationSnippet();
    }

    public void SetOrderAlternatives(int alternativeN)
    {
        ShowAlternative(Panels[alternativeN]);

        if (alternativeN == alternatives.Length - 1)
        {
            ShowGo(altDnDMessage);
            ShowGo(buttonToDnd);
            buttonToDnd.GetComponent<Button>().interactable = true;
        }
        else
        {
            NextAlternative(Panels[alternativeN+1]);
            alternatives[alternativeN+1].GetComponent<UIButton>().Interactable = true;
        }
    }

    public void StartDnD()
    {
        //Enable DnD buttons
        buttonToDnd.GetComponent<Button>().interactable = true;
        //Enable DnD
        EnableDnD();
    }

    //Show specific alternative
    public void ShowAlternative(GameObject o)
    {
        o.GetComponent<Image>().DOColor(VisibleAltColor, 1f);
    }

    //Shaded alternative to 25%
    public void NextAlternative(GameObject o)
    {
        o.GetComponent<Image>().DOColor(VisibleAltColor25, 1f);
    }

    //Shaded alternative
    public void HideAlternative(GameObject o)
    {
        o.GetComponent<Image>().DOColor(HiddenAltColor, 1f);
    }

    public void CheckPriorities()
    {
        //First DnD action
        if (DragNdropRes.Length == 0)
            ShowGo(buttonToConv);

        //Reset the list of the Drag&Drops result
        Array.Clear(DragNdropRes, 0, DragNdropRes.Length);

        //Update Drag & Drop results
        for (int i = 0; i < alternatives.Length; i++)
        {
            panelObjectValue = DragDropManager.GetPanelObject(prioIds[i]);
            DragNdropRes[i] = Convert.ToInt32($"{panelObjectValue}");
        }

        //Set Alternative values to TestingEnvironment
        var alt = controllers.GetComponent<TestingEnvironment>().AlternativesUninformed;
        //alt.Clear();
        //DragNdropRes.ForEach((item) => { alt.Add((string)item.Clone()); });
        Array.Clear(alt, 0, alt.Length);
        alt = (int[])DragNdropRes.Clone();
    }

    public void DisableDnD()
    {
        //Lock the drag&drop property of the elements
        for (int i = 0; i < 6; i++)
        {
            alternatives[i].GetComponent<ObjectSettings>().LockObject = true;
        }

        prioritiesIcon.SetActive(false);
    }

    public void EnableDnD()
    {
        //Disable the alternatives buttons
        for (int i = 0; i < 6; i++)
        {
            alternatives[i].GetComponent<UIButton>().Interactable = false;
        }

        //Unlock the drag&drop property of the elements
        for (int i = 0; i < 6; i++)
        {
            alternatives[i].GetComponent<ObjectSettings>().LockObject = false;
        }

        prioritiesIcon.SetActive(true); //Show the icons representing the priorities weight 

        ShowPopup();
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