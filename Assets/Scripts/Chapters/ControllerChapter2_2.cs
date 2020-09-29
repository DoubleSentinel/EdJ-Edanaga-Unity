using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine.Nody;
using Doozy.Engine.UI;
using UnityEngine;

public class ControllerChapter2_2 : MonoBehaviour
{
    // Scene references
    [SerializeField] private GameObject sceneHost;
    [SerializeField] private GameObject scenePlayer;
    [SerializeField] private GameObject[] sceneFamilies;

    [SerializeField] private GameObject tradeoffLeftBattlerUIPosition;
    [SerializeField] private GameObject tradeoffRightBattlerUIPosition;

    [SerializeField] private GameObject HostBargainConversationBubble;
    [SerializeField] private GameObject TradeoffBattleConversationBubble;


    // Local variables
    private GameObject controllers;

    private BackendAPI m_api;

    private List<(GameObject, GameObject)> tradeoffs;
    private int currentTradeOffPair;

    // Unity calls Awake after all active GameObjects in the Scene are initialized
    void Awake()
    {
        controllers = GameObject.Find("Controllers");

        currentTradeOffPair = -1;
        tradeoffs = new List<(GameObject, GameObject)>();

        m_api = controllers.GetComponent<BackendAPI>();

        controllers.GetComponent<LanguageHandler>().translateUI();

        foreach (GameObject o in GameObject.FindGameObjectsWithTag("DialogBubble"))
        {
            print(o.name);
            o.GetComponent<ConversationHandler>().FetchConversations();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // --------------------  UI Callables  --------------------------------
    public void ClearCharacters()
    {
        foreach (GameObject character in GameObject.FindGameObjectsWithTag("Character"))
        {
            character.transform.position = Vector3.forward;
            character.SetActive(false);
        }
    }

    // View - 2.2.1/6/8 - Bargain conversation setup
    public void SetupBargainConversation()
    {
        float height = Screen.height * 0.8f / 2f;
        float depth = 1f;
        scenePlayer.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 2 / 3,
            height,
            depth));
        sceneHost.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 4,
            height,
            depth));
        scenePlayer.SetActive(true);
        sceneHost.SetActive(true);

        HostBargainConversationBubble.GetComponent<ConversationHandler>().GenerateConversation(0);
        HostBargainConversationBubble.GetComponent<ConversationHandler>().NextConversationSnippet();
    }

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
                    tablePosition.x + passage * offset * (Math.Abs(offset % 2) < 0.01 ? 1 : -1),
                    tablePosition.y + passage * offset * (Math.Abs(offset % 2) < 0.01 ? -1 : 1),
                    depth));
                objective.gameObject.SetActive(true);
                passage++;
            }
        }
    }

    // TradeOff
    public void PrepareTradeOffs(GameObject family)
    {
        currentTradeOffPair = -1;
        tradeoffs.Clear();
        tradeoffs.Add((family.transform.GetChild(0).gameObject, family.transform.GetChild(1).gameObject));
        if (family.transform.childCount == 3)
        {
            tradeoffs.Add((family.transform.GetChild(1).gameObject, family.transform.GetChild(2).gameObject));
        }
    }

    public void NextTradeOff()
    {
        if (currentTradeOffPair < tradeoffs.Count - 1)
        {
            currentTradeOffPair++;
            ClearCharacters();

            ShowTradeoffBattler(tradeoffs[currentTradeOffPair].Item1, tradeoffLeftBattlerUIPosition);
            ShowTradeoffBattler(tradeoffs[currentTradeOffPair].Item2, tradeoffRightBattlerUIPosition);

            TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().GenerateConversation(
                $"2.2.3_Battles_obj{tradeoffs[currentTradeOffPair].Item1.name.Last()}vsobj{tradeoffs[currentTradeOffPair].Item2.name.Last()}");
            TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().NextConversationSnippet();
        }
        else
        {
            GameObject.Find("Graph Controller - Chapter2.2").GetComponent<GraphController>()
                .GoToNodeByName("2.2.2/7 - Tables");
        }
    }

    private void ShowTradeoffBattler(GameObject objective, GameObject tradeoffLeftBattlerUiPosition)
    {
        objective.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(
            tradeoffLeftBattlerUiPosition.transform.position.x,
            tradeoffLeftBattlerUiPosition.transform.position.y,
            1.0f
        ));
        objective.SetActive(true);
    }
}