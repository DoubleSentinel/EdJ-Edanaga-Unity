using Doozy.Engine;
using UnityEngine;

public class ControllerChapter2_3 : MonoBehaviour
{
    [Header("2D Scene References")]
    [SerializeField] private GameObject sceneHost;
    [SerializeField] private GameObject scenePlayer;
    
    
    [Header("Conversation References")]
    [HideInInspector] public int hostConversationIndex = 0;
    public GameObject[] ConversationBubbles;
    public ConversationHandler.ConversationEnd hostConversationCallback;

    private GameObject controllers;
    
    void Awake()
    {
        controllers = GameObject.Find("Controllers");
        hostConversationCallback = () => { GameEventMessage.SendEvent("GoToResults"); };
    }
    private void Start()
    {
        controllers.GetComponent<LanguageHandler>().translateUI();
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
}
