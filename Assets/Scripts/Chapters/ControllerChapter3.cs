﻿using DG.Tweening;
using Doozy.Engine;
using Doozy.Engine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControllerChapter3 : MonoBehaviour
{
    [Header("2D Scene References")] [SerializeField]
    private GameObject scenePlayer;

    [SerializeField] private GameObject sceneJournalist;
    [SerializeField] private GameObject sceneEngineer;

    [Header("Conversation References")]
    public GameObject[] ConversationBubbles;

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

    [Header("Drag&Drop result")] [SerializeField]
    private List<string> dragNdropRes;
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

    public List<string> DragNdropRes { get => dragNdropRes; set => dragNdropRes = value; }

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

        DragNdropRes = new List<string>();
        prioIds = new string[6];
        Panels = new GameObject[6];

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

    private void Call(int conversationIndex)
    {
        string title = "";
        var ch = ConversationBubbles[0].GetComponent<ConversationHandler>();
        title = ch.GetConversationTitle(conversationIndex);
        ch.callback = conversationCallback;
        ch.GenerateConversation(conversationIndex);
        ch.NextConversationSnippet();
    }

    public void StartConversation()
    {
        GameEventMessage.SendEvent("GoToConversation");
        SetupConversation();
        Call(conversationIndex);
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

        var ch = ConversationBubbles[0].GetComponent<ConversationHandler>();
        ch.callback = conversationCallback;
        ch.GenerateConversation(conversationIndex);
        ch.NextConversationSnippet();
    }

    public void StartDnD()
    {
        //Enable DnD buttons
        buttonToDnd.GetComponent<Button>().interactable = true;
        //Enable DnD
        EnableDnD();
    }

    public void CheckPriorities()
    {
        //First DnD action
        if (DragNdropRes.Count == 0)
            ShowGo(buttonToConv);

        //Reset the list of the Drag&Drops result
        DragNdropRes.Clear();

        //Update Drag & Drop results
        for (int i = 0; i < alternatives.Length; i++)
        {
            panelObjectValue = DragDropManager.GetPanelObject(prioIds[i]);
            DragNdropRes.Add(panelObjectValue);
        }
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