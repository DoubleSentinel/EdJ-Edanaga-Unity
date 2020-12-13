using DG.Tweening;
using Doozy.Engine;
using Doozy.Engine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControllerChapter1 : MonoBehaviour
{
    [Header("2D Scene References")]

    [SerializeField] private GameObject scenePlayer;
    [SerializeField] private GameObject sceneJournalist;
    [SerializeField] private GameObject sceneEngineer;

    [Header("Conversation References")] public GameObject[] ConversationBubbles;

    [Header("Drag&Drop scene")]

    [SerializeField] private GameObject[] alternatives;
    [SerializeField] private GameObject[] priorities;
    
    [SerializeField] private GameObject prioritiesIcon;
    [SerializeField] private GameObject buttonToDnd;
    [SerializeField] private GameObject buttonToConv;

    private string[] prioIds;
    private List<GameObject> Panels = new List<GameObject>();
    
    public Color HiddenAltColor = new Color(0.37f, 0.58f, 0.82f, 0.7f);
    public Color VisibleAltColor = new Color(0.37f, 0.58f, 0.82f, 0);
    public Color VisibleAltColor25 = new Color(0.37f, 0.58f, 0.82f, 0.5f);
    
    [Header("Drag&Drop result")]
    [SerializeField] private List<string> dragNdropRes;

    [Header("Popup Values")]
    public string PopupName = "Popup1";
    //[SerializeField] private GameObject TitleObject;
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
        conversationCallback = () => { GameEventMessage.SendEvent("ContinueToAlt"); };
    }

    private void Start()
    {
        controllers.GetComponent<LanguageHandler>().translateUI();

        dragNdropRes = new List<string>();
        
        for (int i=0; i< alternatives.Length; i++)
        {
            //Set Panels
            Panels[i] = alternatives[i].gameObject.transform.GetChild(2).gameObject;
        }
        for (int i = 0; i < priorities.Length; i++)
        {
            //Set Priority Id
            prioIds[i] = priorities[i].GetComponent<PanelSettings>().Id;
        }
        
        /*
        prioId2 = priorities[1].GetComponent<PanelSettings>().Id;
        
        //Get Panels
        
        Panel1 = alternatives[0].gameObject.transform.GetChild(2).gameObject;
        */
        
        //Default Setup
        SetOrderAlternatives(0);
        HideGo(buttonToDnd);
        HideGo(buttonToConv);
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
        switch (alternativeN)
        {
            case 1:
                ShowAlternative(Panels[0]);
                NextAlternative(Panels[1]);
                alternatives[1].GetComponent<UIButton>().Interactable = true;
                break;
            case 2:
                ShowAlternative(Panels[1]);
                NextAlternative(Panels[2]);
                alternatives[2].GetComponent<UIButton>().Interactable = true;

                break;
            case 3:
                ShowAlternative(Panels[2]);
                NextAlternative(Panels[3]);
                alternatives[3].GetComponent<UIButton>().Interactable = true;
                break;
            case 4:
                ShowAlternative(Panels[3]);
                NextAlternative(Panels[4]);
                alternatives[4].GetComponent<UIButton>().Interactable = true;
                break;
            case 5:
                ShowAlternative(Panels[4]);
                NextAlternative(Panels[5]);
                alternatives[5].GetComponent<UIButton>().Interactable = true;
                break;
            case 6:
                ShowAlternative(Panels[5]);
                alternatives[5].GetComponent<UIButton>().Interactable = false;
                //Show Button - Go to Dnd
                ShowGo(buttonToDnd);
                buttonToDnd.GetComponent<Button>().interactable = true;
                break;

            default:
                //Just the first alternative is interractable, the others not
                NextAlternative(Panels[0]);
                HideAlternative(Panels[1]);
                HideAlternative(Panels[2]);
                HideAlternative(Panels[3]);
                HideAlternative(Panels[4]);
                HideAlternative(Panels[5]);
                alternatives[0].GetComponent<UIButton>().Interactable = true;
                alternatives[1].GetComponent<UIButton>().Interactable = false;
                alternatives[2].GetComponent<UIButton>().Interactable = false;
                alternatives[3].GetComponent<UIButton>().Interactable = false;
                alternatives[4].GetComponent<UIButton>().Interactable = false;
                alternatives[5].GetComponent<UIButton>().Interactable = false;
                break;
        }
    }

    public void StartDnD()
    {
        //Show alternatives
        //SetOrderAlternatives(7);

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
        if (dragNdropRes.Count == 0)
            ShowGo(buttonToConv);

        //Reset the list of the Drag&Drops result
        dragNdropRes.Clear();

        //Add result of the Drag&Drop into the list
        string PrioAlt = DragDropManager.GetPanelObject(prioIds[0]);
        dragNdropRes.Add(PrioAlt);
        PrioAlt = DragDropManager.GetPanelObject(prioIds[1]);
        dragNdropRes.Add(PrioAlt);
        PrioAlt = DragDropManager.GetPanelObject(prioIds[2]);
        dragNdropRes.Add(PrioAlt);
        PrioAlt = DragDropManager.GetPanelObject(prioIds[3]);
        dragNdropRes.Add(PrioAlt);
        PrioAlt = DragDropManager.GetPanelObject(prioIds[4]);
        dragNdropRes.Add(PrioAlt);
        PrioAlt = DragDropManager.GetPanelObject(prioIds[5]);
        dragNdropRes.Add(PrioAlt);
    }

    public void DisableDnD()
    {
        //Lock the drag&drop property of the elements
        alternatives[0].GetComponent<ObjectSettings>().LockObject = true;
        alternatives[1].GetComponent<ObjectSettings>().LockObject = true;
        alternatives[2].GetComponent<ObjectSettings>().LockObject = true;
        alternatives[3].GetComponent<ObjectSettings>().LockObject = true;
        alternatives[4].GetComponent<ObjectSettings>().LockObject = true;
        alternatives[5].GetComponent<ObjectSettings>().LockObject = true;

        prioritiesIcon.SetActive(false);
    }

    public void EnableDnD()
    {
        //Disable the alternatives buttons
        alternatives[0].GetComponent<UIButton>().Interactable = false;
        alternatives[1].GetComponent<UIButton>().Interactable = false;
        alternatives[2].GetComponent<UIButton>().Interactable = false;
        alternatives[3].GetComponent<UIButton>().Interactable = false;
        alternatives[4].GetComponent<UIButton>().Interactable = false;
        alternatives[5].GetComponent<UIButton>().Interactable = false;

        //Unlock the drag&drop property of the elements
        alternatives[0].GetComponent<ObjectSettings>().LockObject = false;
        alternatives[1].GetComponent<ObjectSettings>().LockObject = false;
        alternatives[2].GetComponent<ObjectSettings>().LockObject = false;
        alternatives[3].GetComponent<ObjectSettings>().LockObject = false;
        alternatives[4].GetComponent<ObjectSettings>().LockObject = false;
        alternatives[5].GetComponent<ObjectSettings>().LockObject = false;

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
    /*
    public void toogle(GameObject go)
    {
        var ddc = go.GetComponent<Text>();
        ddc.enabled = !ddc.enabled;
    }
    */
}
