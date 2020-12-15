using Doozy.Engine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ControllerChapter2_1 : MonoBehaviour
{
    [Header("2D Scene References")]
    private GameObject sceneObjective;

    [SerializeField] private GameObject scenePlayer;
    [SerializeField] private GameObject[] sceneObjectives;
    //[SerializeField] private GameObject[] hosts;
    private int currentCallPair;

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
        currentCallPair = -1;
        controllers = GameObject.Find("Controllers");
        conversationCallback = () => { GameEventMessage.SendEvent("ContinueToTown"); };
    }

    private void Start()
    {
        controllers.GetComponent<LanguageHandler>().translateUI();
        sceneObjectives = new GameObject[10];
        //hosts = new GameObject[10];
        //PrepareCall();
    }

    public void SetConversationIndex(int index)
    {
        conversationIndex = index;
       
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
        sceneObjective.transform.position = new Vector3(host.x, host.y, depth);
        scenePlayer.SetActive(true);
        sceneObjective.SetActive(true);

        var ch = ConversationBubbles[0].GetComponent<ConversationHandler>();
        ch.callback = hostConversationCallback;
        ch.GenerateConversation(hostConversationIndex);
        ch.NextConversationSnippet();
    }

    private void StartCall()
    {
        ClearCharacters();

        sceneObjective = sceneObjectives[conversationIndex];
        
        string title = "";
        title = $"2.1.2_Dialogue_objective{conversationIndex}";

        GetComponent<ConversationHandler>().GenerateConversation(title);
        GetComponent<ConversationHandler>().NextConversationSnippet();
    }
}
