using Doozy.Engine;
using UnityEngine;

public class ControllerChapter2 : MonoBehaviour
{
    [Header("2D Scene References")]

    [SerializeField] private GameObject scenePlayer;
    
    [SerializeField] private GameObject sceneSpeaker1;
    [SerializeField] private GameObject sceneSpeaker2;
    [SerializeField] private GameObject sceneSpeaker3;
    [SerializeField] private GameObject sceneSpeaker4;
    [SerializeField] private GameObject sceneSpeaker5;
    [SerializeField] private GameObject sceneSpeaker6;
    [SerializeField] private GameObject sceneSpeaker7;
    [SerializeField] private GameObject sceneSpeaker8;
    [SerializeField] private GameObject sceneSpeaker9;
    [SerializeField] private GameObject sceneSpeaker10;

    private GameObject sceneSpeaker; //current speaker

    [Header("Conversation References")] public GameObject[] ConversationBubbles;

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
    }

    // --------------------  UI Callables  --------------------------------
    public void SetConversationIndex(int index)
    {
        conversationIndex = index;
        
        //Set speaker face
        switch (index)
        {
            case 1:
                sceneSpeaker = sceneSpeaker1;
                break;
            case 2:
                sceneSpeaker = sceneSpeaker2;
                break;
            case 3:
                sceneSpeaker = sceneSpeaker3;
                break;
            case 4:
                sceneSpeaker = sceneSpeaker4;
                break;
            case 5:
                sceneSpeaker = sceneSpeaker5;
                break;
            case 6:
                sceneSpeaker = sceneSpeaker6;
                break;
            case 7:
                sceneSpeaker = sceneSpeaker7;
                break;
            case 8:
                sceneSpeaker = sceneSpeaker8;
                break;
            case 9:
                sceneSpeaker = sceneSpeaker9;
                break;
            case 10:
                sceneSpeaker = sceneSpeaker10;
                break;
        }
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
        Vector3 speaker = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 5 / 6,
           height));

        scenePlayer.transform.position = new Vector3(player.x, player.y, depth);
        sceneSpeaker.transform.position = new Vector3(speaker.x, speaker.y, depth);

        scenePlayer.SetActive(true);
        sceneSpeaker.SetActive(true);

        var ch = ConversationBubbles[0].GetComponent<ConversationHandler>();
        ch.callback = conversationCallback;
        ch.GenerateConversation(conversationIndex);
        ch.NextConversationSnippet();
    }
}