using Doozy.Engine;
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
    public GameObject alternativesView;
    public GameObject alt0, alt1, alt2, alt3, alt4, alt5, prio1, prio2, prio3, prio4, prio5, prio6;
    public GameObject priorities, buttonToDnd, buttonToConv;

    public string prioId1, prioId2, prioId3, prioId4, prioId5, prioId6;

    [Header("Drag&Drop result")]
    [SerializeField] private List<string> dragNdropRes;

    // Local variables
    private GameObject controllers;
    //public SpriteRenderer rendIn, rendOut; //FadeInand FadeOut
    public Image rendIn, rendOut; //FadeInand FadeOut

    // Flags
    //public bool IsTradeOff { get; set; }

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

        //Lock the drag&drop of the elements
        /*
        ObjectSettings[] objectsettings = alternativesView.gameObject.GetComponents<ObjectSettings>();

        for (int i = 0; i < objectsettings.Length; i++)
        {
            objectsettings[i].LockObject = true;
        }
        */

        dragNdropRes = new List<string>();

        alt0 = GameObject.Find("Alternative0"); //Mettre dans Awake();
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

        prioId1 = prio1.GetComponent<PanelSettings>().Id;
        prioId2 = prio2.GetComponent<PanelSettings>().Id;
        prioId3 = prio3.GetComponent<PanelSettings>().Id;
        prioId4 = prio4.GetComponent<PanelSettings>().Id;
        prioId5 = prio5.GetComponent<PanelSettings>().Id;
        prioId6 = prio6.GetComponent<PanelSettings>().Id;

        priorities = GameObject.Find("Priorities");
        buttonToDnd = GameObject.Find("Button - ContinueToAltDnD");
        buttonToConv = GameObject.Find("Button - ContinueToConv"); 

        //Default setup
        buttonToDnd.GetComponent<Button>().interactable = false;
        HideGo(buttonToConv);
        DisableDnD();
    }

    public void DisableDnD()
    {
        /*
        ObjectSettings[] objectsettings = alternativesView.gameObject.GetComponents<ObjectSettings>();
        for (int i = 0; i < objectsettings.Length; i++)
        {
            objectsettings[i].LockObject = false;
        }
        */

        //Lock the drag&drop property of the elements
        alt0.GetComponent<ObjectSettings>().LockObject = true;
        alt1.GetComponent<ObjectSettings>().LockObject = true;
        alt2.GetComponent<ObjectSettings>().LockObject = true;
        alt3.GetComponent<ObjectSettings>().LockObject = true;
        alt4.GetComponent<ObjectSettings>().LockObject = true;
        alt5.GetComponent<ObjectSettings>().LockObject = true;

        priorities.SetActive(false);
    }

    public void EnableDnD()
    {
        /*
        ObjectSettings[] objectsettings = alternativesView.gameObject.GetComponents<ObjectSettings>();
        for (int i = 0; i < objectsettings.Length; i++)
        {
            objectsettings[i].LockObject = false;
        }
        */

        //Unlock the drag&drop property of the elements
        alt0.GetComponent<ObjectSettings>().LockObject = false;
        alt1.GetComponent<ObjectSettings>().LockObject = false;
        alt2.GetComponent<ObjectSettings>().LockObject = false;
        alt3.GetComponent<ObjectSettings>().LockObject = false;
        alt4.GetComponent<ObjectSettings>().LockObject = false;
        alt5.GetComponent<ObjectSettings>().LockObject = false;

        priorities.SetActive(true);
    }

    public void CheckPriorities()
    {
        //First DnD
        if (dragNdropRes == null)
            ShowGo(buttonToConv);

        //Reset the list
        dragNdropRes.Clear();

        string PrioAlt = DragDropManager.GetPanelObject(prioId1);
        //Debug.Log("Priorité 1 :" + PrioObject.ToString());
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
                startFadeIn();
                startFadeOut();
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



    // --------------------  UI Callables  --------------------------------
    /*
    public void HideBackground(GameObject background)
    {
        background.SetActive(false);
    }

    public void ShowBackground(GameObject background)
    {
        background.SetActive(true);
    }
    */

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
}
