using Doozy.Engine;
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
    public GameObject alt0, alt1, alt2, alt3, alt4, alt5, alt6, priorities, buttonToDnd;

    [Header("Drag&Drop result")]
    [SerializeField] private List<string> dragNdrop;

    // Local variables
    private GameObject controllers;

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

        dragNdrop = new List<string>();

        alt0 = GameObject.Find("Alternative0");
        alt1 = GameObject.Find("Alternative1");
        alt2 = GameObject.Find("Alternative2");
        alt3 = GameObject.Find("Alternative3");
        alt4 = GameObject.Find("Alternative4");
        alt5 = GameObject.Find("Alternative5");

        priorities = GameObject.Find("Priorities");
        buttonToDnd = GameObject.Find("Button - ContinueToAltDnD");

        //Default setup
        buttonToDnd.GetComponent<Button>().interactable = false;
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

    public void showGo(GameObject go)
    {
       go.SetActive(true);
    }
    public void HideGo(GameObject go)
    {
        go.SetActive(false);
    }
}
