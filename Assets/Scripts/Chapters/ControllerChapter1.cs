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
    /*
    [SerializeField] private GameObject alt0;
    [SerializeField] private GameObject alt1;
    [SerializeField] private GameObject alt2;
    [SerializeField] private GameObject alt3;
    [SerializeField] private GameObject alt4;
    [SerializeField] private GameObject alt5;
    
    [SerializeField] private GameObject prio1;
    [SerializeField] private GameObject prio2;
    [SerializeField] private GameObject prio3;
    [SerializeField] private GameObject prio4;
    [SerializeField] private GameObject prio5;
    [SerializeField] private GameObject prio6;
    */
    [SerializeField] private GameObject prioritiesIcon;
    [SerializeField] private GameObject buttonToDnd;
    [SerializeField] private GameObject buttonToConv;

    private string prioId1, prioId2, prioId3, prioId4, prioId5, prioId6;
    private GameObject Panel1, Panel2, Panel3, Panel4, Panel5, Panel6;

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
    //public SpriteRenderer rendIn, rendOut; //FadeInand FadeOut
    //public Image rendIn, rendOut; //FadeInand FadeOut

    // BargainConversation vars
    [HideInInspector] public int conversationIndex = 0;
    public ConversationHandler.ConversationEnd conversationCallback;

    void Awake()
    {
        controllers = GameObject.Find("Controllers");
        conversationCallback = () => { GameEventMessage.SendEvent("ContinueToAlt"); };
        /*
        alt0 = GameObject.Find("Alternative0");
        alt1 = GameObject.Find("Alternative1");
        alt2 = GameObject.Find("Alternative2");
        alt3 = GameObject.Find("Alternative3");
        alt4 = GameObject.Find("Alternative4");
        alt5 = GameObject.Find("Alternative5");

        prio1 = GameObject.Find("Priority1");
        prio2 = GameObject.Find("Priority2");
        prio3 = GameObject.Find("Priority3");
        prio4 = GameObject.Find("Priority4");
        prio5 = GameObject.Find("Priority5");
        prio6 = GameObject.Find("Priority6");
        
        prioritiesIcon = GameObject.Find("Priorities");

        buttonToDnd = GameObject.Find("Button - ContinueToAltDnD");
        buttonToConv = GameObject.Find("Button - ContinueToConv");
        */
 
    }

    private void Start()
    {
        controllers.GetComponent<LanguageHandler>().translateUI();

        dragNdropRes = new List<string>();

        //Get Priority Id
        prioId1 = priorities[0].GetComponent<PanelSettings>().Id;
        prioId2 = priorities[1].GetComponent<PanelSettings>().Id;
        prioId3 = priorities[2].GetComponent<PanelSettings>().Id;
        prioId4 = priorities[3].GetComponent<PanelSettings>().Id;
        prioId5 = priorities[4].GetComponent<PanelSettings>().Id;
        prioId6 = priorities[5].GetComponent<PanelSettings>().Id;

        //Get Panels
        Panel1 = alternatives[0].gameObject.transform.GetChild(2).gameObject;
        Panel2 = alternatives[1].gameObject.transform.GetChild(2).gameObject;
        Panel3 = alternatives[2].gameObject.transform.GetChild(2).gameObject;
        Panel4 = alternatives[3].gameObject.transform.GetChild(2).gameObject;
        Panel5 = alternatives[4].gameObject.transform.GetChild(2).gameObject;
        Panel6 = alternatives[5].gameObject.transform.GetChild(2).gameObject;

        //Default Setup
        SetOrderAlternatives(0);

        buttonToDnd.GetComponent<Button>().interactable = false;
        HideGo(buttonToConv);
        DisableDnD();
    }

    public void ShowAlternative(GameObject o)
    {
        o.GetComponent<Image>().DOColor(new Color(0.37f, 0.58f, 0.82f, 0), 1f);
    }

    public void HideAlternative(GameObject o)
    {
        o.GetComponent<Image>().DOColor(new Color(0.37f, 0.58f, 0.82f, 0.7f), 1f);
    }

    public void SetOrderAlternatives(int alternativeN)
    {
        switch (alternativeN)
        {
            case 1:
                HideAlternative(Panel1);
                ShowAlternative(Panel2);
                alternatives[0].GetComponent<UIButton>().Interactable = false;
                alternatives[1].GetComponent<UIButton>().Interactable = true;
                break;
            case 2:
                HideAlternative(Panel2);
                ShowAlternative(Panel3);
                alternatives[1].GetComponent<UIButton>().Interactable = false;
                alternatives[2].GetComponent<UIButton>().Interactable = true;

                break;
            case 3:
                HideAlternative(Panel3);
                ShowAlternative(Panel4);
                alternatives[2].GetComponent<UIButton>().Interactable = false;
                alternatives[3].GetComponent<UIButton>().Interactable = true;
                break;
            case 4:
                HideAlternative(Panel4);
                ShowAlternative(Panel5);
                alternatives[3].GetComponent<UIButton>().Interactable = false;
                alternatives[4].GetComponent<UIButton>().Interactable = true;
                break;
            case 5:
                HideAlternative(Panel5);
                ShowAlternative(Panel6);
                alternatives[4].GetComponent<UIButton>().Interactable = false;
                alternatives[5].GetComponent<UIButton>().Interactable = true;
                break;
            case 6:
                HideAlternative(Panel6);
                //Shoe Button - Go to Dnd
                alternatives[5].GetComponent<UIButton>().Interactable = false;
                buttonToDnd.GetComponent<Button>().interactable = true;
                break;

            case 7:
                ShowAlternative(Panel1);
                ShowAlternative(Panel2);
                ShowAlternative(Panel3);
                ShowAlternative(Panel4);
                ShowAlternative(Panel5);
                ShowAlternative(Panel6);

                break;

            default:
                ShowAlternative(Panel1);
                HideAlternative(Panel2);
                HideAlternative(Panel3);
                HideAlternative(Panel4);
                HideAlternative(Panel5);
                HideAlternative(Panel6);
                //Disable button
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
        SetOrderAlternatives(7);

        //Disable buttons
        buttonToDnd.GetComponent<Button>().interactable = true;
        
        //Enable DnD
        EnableDnD();
    }

/*
    public void FageInOut(int sourceN)
    {
        switch (sourceN)
        {
            case 1:
                //rendIn = alt1.GetComponentInChildren<SpriteRenderer>();
                rendIn = alt1.gameObject.transform.GetChild(1).GetComponent<Image>();
                Color c = rendIn.material.color;
                c.a = 0f;
                rendIn.material.color = c;
                //rendOut = alt0.GetComponentInChildren<SpriteRenderer>();
                rendOut = alt0.gameObject.transform.GetChild(1).GetComponent<Image>();
                //startFadeIn();
                //startFadeOut();
                break;

            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;

        }
    }

    IEnumerator FadeIn()
    {
        for (float f = 0.05f; f <= 1; f += 0.05f)
        {
            Color c = rendIn.material.color;
            c.a = f;
            rendIn.material.color = c;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void startFadeIn()
    {
        StartCoroutine("FadeIn");
    }

    IEnumerator FadeOut()
    {
        for (float f = 1f; f >= -0.05f; f -= 0.05f)
        {
            Color c = rendOut.material.color;
            c.a = f;
            rendOut.material.color = c;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void startFadeOut()
    {
        StartCoroutine("FadeOut");
    }
*/

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

    public void toogle(GameObject go)
    {
        var ddc = go.GetComponent<DDCanvas>();
        ddc.enabled = !ddc.enabled;
    }

    public void ShowGo(GameObject go)
    {
       go.SetActive(true);
    }
    public void HideGo(GameObject go)
    {
        go.SetActive(false);
    }

    public void CheckPriorities()
    {
        //First DnD
        if (dragNdropRes.Count == 0)
            ShowGo(buttonToConv);

        //Reset the list
        dragNdropRes.Clear();

        //Add result of the Drag&Drop into the list
        string PrioAlt = DragDropManager.GetPanelObject(prioId1);
        dragNdropRes.Add(PrioAlt);
        PrioAlt = DragDropManager.GetPanelObject(prioId2);
        dragNdropRes.Add(PrioAlt);
        PrioAlt = DragDropManager.GetPanelObject(prioId3);
        dragNdropRes.Add(PrioAlt);
        PrioAlt = DragDropManager.GetPanelObject(prioId4);
        dragNdropRes.Add(PrioAlt);
        PrioAlt = DragDropManager.GetPanelObject(prioId5);
        dragNdropRes.Add(PrioAlt);
        PrioAlt = DragDropManager.GetPanelObject(prioId6);
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
        //Disable button (to be sure!)
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

        prioritiesIcon.SetActive(true);

        ShowPopup();
    }

    public void ShowPopup()
    {
        //get a clone of the UIPopup, with the given PopupName, from the UIPopup Database 
        UIPopup popup = UIPopup.GetPopup(PopupName);

        //make sure that a popup clone was actually created
        if (popup == null)
            return;

        //popup.Data.SetLabelsTexts(Title, Message);
        //Title = TitleObject.GetComponent<Text>().ToString();
        //Message = MessageObject.GetComponent<Text>().ToString();
        Title = "Consignes";
        //Message = "Cliquez sur l’alternative que vous préférez, maintenez le bouton de la souris pressé et déplacez l’image en haut du classement, avant de relâcher le bouton de la souris. De la même manière, glissez-déposez votre deuxième alternative préférée à la seconde place, et ainsi de suite jusqu’à obtenir le classement de votre choix !";
        //Message = MessageObject.GetComponent<Text>().ToString();
        Message = MessageObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text;
        //Message = MessageObject.GetComponent<TMPro.TextMeshProUGUI>().text;
        popup.Data.SetLabelsTexts(Title, Message);

        popup.Show(); //show the popup
    }

}
