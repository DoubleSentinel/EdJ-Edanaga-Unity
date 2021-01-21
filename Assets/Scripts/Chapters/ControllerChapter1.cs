using DG.Tweening;
using Doozy.Engine;
using Doozy.Engine.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ControllerChapter1 : MonoBehaviour
{
    [Header("2D Scene References")]
    [SerializeField] private GameObject scenePlayer;
    [SerializeField] private GameObject sceneJournalist;
    [SerializeField] private GameObject sceneEngineer;

    [Header("Conversation References")]
    [SerializeField] private GameObject[] ConversationBubbles;

    [Header("Alternative shaders")]
    [SerializeField] private Color HiddenAltColor = new Color(0.37f, 0.58f, 0.82f, 0.7f);
    [SerializeField] private Color VisibleAltColor = new Color(0.37f, 0.58f, 0.82f, 0);
    [SerializeField] private Color VisibleAltColor25 = new Color(0.37f, 0.58f, 0.82f, 0.5f);

    [Header("Drag&Drop scene")]
    [SerializeField] private GameObject[] alternatives;
    [SerializeField] private GameObject[] panels;
    [SerializeField] private GameObject prioritiesIcon;
    [SerializeField] private GameObject buttonToDnd;
    [SerializeField] private GameObject buttonToConv;
    [SerializeField] private GameObject altDnDMessage;
    [SerializeField] private GameObject altDiscoveryMessage;
    private string[] prioIds;
    private GameObject[] PanelsAlt;

    [Header("Drag&Drop result")] [SerializeField]
    private int[] dragNdropRes;
    private string panelObjectValue;
    private bool fisrtDrag = false;

    [Header("Popup Values")]
    [SerializeField] private string popupName = "Popup1";
    [SerializeField] private string title = "Title";
    [SerializeField] private GameObject messageObject;
    [SerializeField] private string message = "Popup message for player";

    // Local variables
    private GameObject controllers;

    // BargainConversation vars
    [HideInInspector] private int conversationIndex = 0;
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
        PanelsAlt = new GameObject[6];

        // Setting alternatives gameobjects and their panels for locking mechanism
        //Get Panels Id name
        for (int i = 0; i < alternatives.Length; i++)
        {
            PanelsAlt[i] = alternatives[i].gameObject.transform.GetChild(2).gameObject;

            if (i == 0)
            {
                NextAlternative(PanelsAlt[i]);
                alternatives[i].GetComponent<UIButton>().Interactable = true;
            }
            else
            {
                HideAlternative(PanelsAlt[i]);
                alternatives[i].GetComponent<UIButton>().Interactable = false;
            }
            //Get Panels Id name
            prioIds[i] = panels[i].gameObject.GetComponent<PanelSettings>().Id;
        }

        //Default Setup
        HideGo(buttonToDnd);
        HideGo(buttonToConv);
        HideGo(altDnDMessage);
        HideGo(altDiscoveryMessage);
        DisableDnD();
    }

    private void SetupHostConversation()
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

    //Disable drag and drop process
    private void DisableDnD()
    {
        //Lock the drag&drop property of the elements
        for (int i = 0; i < alternatives.Length; i++)
        {
            alternatives[i].GetComponent<ObjectSettings>().LockObject = true;
        }
        prioritiesIcon.SetActive(false);
    }

    private void EnableDnD()
    {
        //Disable the alternatives buttons and unlock the drag&drop property of the elements
        for (int i = 0; i < alternatives.Length; i++)
        {
            alternatives[i].GetComponent<UIButton>().Interactable = false;
            alternatives[i].GetComponent<ObjectSettings>().LockObject = false;
        }
        prioritiesIcon.SetActive(true); //Show the icons representing the priorities weight 

        ShowPopup();
    }

    //Show specific alternative
    private void ShowAlternative(GameObject o)
    {
        o.GetComponent<Image>().DOColor(VisibleAltColor, 1f);
    }

    //Shaded alternative to 25%
    private void NextAlternative(GameObject o)
    {
        o.GetComponent<Image>().DOColor(VisibleAltColor25, 1f);
    }

    //Shaded alternative
    private void HideAlternative(GameObject o)
    {
        o.GetComponent<Image>().DOColor(HiddenAltColor, 1f);
    }

    public void CheckPriorities()
    {
        //First DnD action
        if(!fisrtDrag)
        { 
            ShowGo(buttonToConv);
            fisrtDrag = true;
        }

        //Reset the list of the Drag&Drops result
        Array.Clear(DragNdropRes, 0, DragNdropRes.Length);
        
        //Update Drag & Drop results
        for (int i = 0; i < alternatives.Length; i++)
        {
            panelObjectValue = DragDropManager.GetPanelObject(prioIds[i]);
            DragNdropRes[i] = Convert.ToInt32($"{panelObjectValue}");
        }

        //Update uniformed alternative values to TestingEnvironment
        var alt = controllers.GetComponent<TestingEnvironment>().AlternativesUninformed;
        Array.Clear(alt, 0, alt.Length);
        alt = (int[])DragNdropRes.Clone();
    }

    private void ShowPopup()
    {
        //get a clone of the UIPopup, with the given PopupName, from the UIPopup Database 
        UIPopup popup = UIPopup.GetPopup(popupName);

        //make sure that a popup clone was actually created
        if (popup == null)
            return;

        title = "Consignes";
        message = messageObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;
        popup.Data.SetLabelsTexts(title, message);

        popup.Show(); //show the popup
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
    public void SetConversationIndex(int index)
    {
        conversationIndex = index;
    }

    //Enable drag and drop process
    private void StartDnD()
    {
        //Enable DnD buttons
        buttonToDnd.GetComponent<Button>().interactable = true;
        EnableDnD();
    }

    //Display the alternatives with correct shader 
    public void SetOrderAlternatives(int alternativeN)
    {
        ShowAlternative(PanelsAlt[alternativeN]);

        if (alternativeN == alternatives.Length - 1)
        {
            ShowGo(altDnDMessage);
            ShowGo(buttonToDnd);
            buttonToDnd.GetComponent<Button>().interactable = true;
        }
        else
        {
            NextAlternative(PanelsAlt[alternativeN + 1]);
            alternatives[alternativeN + 1].GetComponent<UIButton>().Interactable = true;
        }
    }
}