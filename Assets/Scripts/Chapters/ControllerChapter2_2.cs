using Doozy.Engine;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControllerChapter2_2 : MonoBehaviour
{
    [Header("2D Scene References")] [SerializeField]
    private GameObject sceneHost;

    [SerializeField] private GameObject scenePlayer;
    [SerializeField] private GameObject[] sceneFamilies;


    [Header("Conversation References")] public GameObject[] ConversationBubbles;
    

    // Local variables
    private GameObject controllers;
    
    // Flags
    public bool IsTradeOff { get; set; }

    // BargainConversation vars
    [HideInInspector] public int hostConversationIndex = 0;
    [HideInInspector] public int groupedConversationIndex = 0;
    public ConversationHandler.ConversationEnd hostConversationCallback;
    public ConversationHandler.ConversationEnd groupedConversationCallback;

    // Unity calls Awake after all active GameObjects in the Scene are initialized
    void Awake()
    {
        IsTradeOff = true;
        controllers = GameObject.Find("Controllers");
        hostConversationCallback = () => { GameEventMessage.SendEvent("GoToTables"); };
    }

    private void Start()
    {
        foreach (GameObject o in ConversationBubbles)
        {
            o.GetComponent<ConversationHandler>().FetchConversations();
        }
        controllers.GetComponent<LanguageHandler>().translateUI();
    }

    // --------------------  UI Callables  --------------------------------
    public void HideBackground(GameObject background)
    {
        background.SetActive(false);
    }
    public void ShowBackground(GameObject background)
    {
        background.SetActive(true);
    }

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
        Vector3 player = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 2 / 3,
            height));
        Vector3 host = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 4,
            height));
        scenePlayer.transform.position = new Vector3(player.x, player.y, depth);
        sceneHost.transform.position = new Vector3(host.x, host.y, depth);
        scenePlayer.SetActive(true);
        sceneHost.SetActive(true);

        var ch =  ConversationBubbles[0].GetComponent<ConversationHandler>();
        ch.callback = hostConversationCallback;
        ch.GenerateConversation(hostConversationIndex);
        ch.NextConversationSnippet();
    }

    public void SetupGroupedConversation()
    {
        float height = Screen.height * 0.75f / 2f;
        float depth = -1f;
        Vector3 player = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 2 / 3,
            height));
        Vector3 host = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 4,
            height));
        scenePlayer.transform.position = new Vector3(player.x, player.y, depth);
        sceneHost.transform.position = new Vector3(host.x, host.y, depth);
        scenePlayer.SetActive(true);
        sceneHost.SetActive(true);

        var ch =  ConversationBubbles[2].GetComponent<ConversationHandler>();
        ch.callback = groupedConversationCallback;
        ch.GenerateConversation(groupedConversationIndex);
        ch.NextConversationSnippet();
    }

    // View - 2.2.2/7 - Tables
    public void SetupBargainTables()
    {
        float depth = 1f;
        float offset = 20f;
        foreach (GameObject family in sceneFamilies)
        {
            int passage = 1;
            foreach (Transform objective in family.transform)
            {
                Vector3 tablePosition = GameObject.Find("UI" + family.name).transform.position;
                objective.position = Camera.main.ScreenToWorldPoint(new Vector3(
                    tablePosition.x + passage * offset * (passage % 2 < 0.01 ? 1 : -1),
                    tablePosition.y + passage * offset * (passage % 2 < 0.01 ? 1 : -1),
                    depth));
                objective.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                objective.gameObject.SetActive(true);
                objective.GetComponent<CharacterSpriteController>().MaximizeSymbol(true);
                passage++;
            }
        }
    }
    
    // View - TradeOff - 2.2.3/5 - Battle
    public void StartActivityWithFamily(GameObject family)
    {
        if (IsTradeOff)
        {
            GameEventMessage.SendEvent("GoToTradeOffs");
            GetComponent<TradeOff>().PrepareTradeOffs(family);
        }
        else
        {
            GameEventMessage.SendEvent("GoToSwing");
        }
    }


    // View - TradeOff - 2.2.6 - Results
    
    // Utility UI methods
    public void DeactivateFamilySelector(GameObject caller)
    {
        caller.GetComponent<EventTrigger>().enabled = false;
    }
}