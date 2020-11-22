using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerChapter1 : MonoBehaviour
{
    [Header("2D Scene References")]
    [SerializeField]
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

    void Awake()
    {
        controllers = GameObject.Find("Controllers");
        //hostConversationCallback = () => { GameEventMessage.SendEvent("GoToTables"); };
    }

    private void Start()
    {
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
}
