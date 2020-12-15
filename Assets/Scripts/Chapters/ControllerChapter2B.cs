using Doozy.Engine;
using UnityEngine;

public class ControllerChapter2 : MonoBehaviour
{
    [Header("2D Scene References")]
    private GameObject sceneHost;

    [SerializeField] private GameObject scenePlayer;
    [SerializeField] private GameObject[] sceneFamilies;

    [Header("Conversation References")] public GameObject[] ConversationBubbles;

    // Local variables
    private GameObject controllers;

    // BargainConversation vars
    [HideInInspector] public int conversationIndex = 0;
    public ConversationHandler.ConversationEnd conversationCallback;

    // BargainConversation vars
    [HideInInspector] public int hostConversationIndex = 0;
    [HideInInspector] public int groupedConversationIndex = 0;
    public ConversationHandler.ConversationEnd hostConversationCallback;
    public ConversationHandler.ConversationEnd groupedConversationCallback;

    void Awake()
    {
        controllers = GameObject.Find("Controllers");
        conversationCallback = () => { GameEventMessage.SendEvent("ContinueToAlt"); };
    }

    private void Start()
    {
        controllers.GetComponent<LanguageHandler>().translateUI();
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
        Vector3 player = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 2 / 3,
            height));
        Vector3 host = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 4,
            height));
        scenePlayer.transform.position = new Vector3(player.x, player.y, depth);
        sceneHost.transform.position = new Vector3(host.x, host.y, depth);
        scenePlayer.SetActive(true);
        sceneHost.SetActive(true);

        var ch = ConversationBubbles[0].GetComponent<ConversationHandler>();
        ch.callback = hostConversationCallback;
        ch.GenerateConversation(hostConversationIndex);
        ch.NextConversationSnippet();
    }

    private void StartCallPair()
    {
        GetComponent<ControllerChapter2>().ClearCharacters();
        
        string leftObjectiveName = m_familyTradeoffs[currentTradeOffPair].Item1.name;
        string rightObjectiveName = m_familyTradeoffs[currentTradeOffPair].Item2.name;
        ShowTradeoffBattler(m_familyTradeoffs[currentTradeOffPair].Item1, tradeoffLeftBattlerUIPosition);
        ShowTradeoffBattler(m_familyTradeoffs[currentTradeOffPair].Item2, tradeoffRightBattlerUIPosition);
        UpdateTradeOffSliders(leftObjectiveName, rightObjectiveName);
        // This isn't great but due to time constraints I had to generate the string here instead of creating a proper 
        // structure that handles these associations

        string title = "";
        title = $"2.1.2_Dialogue_objective{leftObjectiveName.Last()}_vs_obj{rightObjectiveName.Last()}";
        
        TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().winnerLoserReplacement = null;
        TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().GenerateConversation(title);
        TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().NextConversationSnippet();
        TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().callback = ToggleSelectionButtons;

        ToggleNextTradeOffButton();
    }
}