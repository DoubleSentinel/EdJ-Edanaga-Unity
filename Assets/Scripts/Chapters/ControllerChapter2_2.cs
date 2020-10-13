using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine.Nody;
using Doozy.Engine.UI;
using TMPro;
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

    private List<(GameObject, GameObject)> m_familyTradeoffs;
    private int currentTradeOffPair;

    // Unity calls Awake after all active GameObjects in the Scene are initialized
    void Awake()
    {
        controllers = GameObject.Find("Controllers");

        currentTradeOffPair = -1;
        m_familyTradeoffs = new List<(GameObject, GameObject)>();

        m_api = controllers.GetComponent<BackendAPI>();

        controllers.GetComponent<LanguageHandler>().translateUI();

        foreach (GameObject o in GameObject.FindGameObjectsWithTag("DialogBubble"))
        {
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
            character.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
            character.SetActive(false);
        }
    }

    // View - 2.2.1/6/8 - Bargain conversation setup
    public void SetupBargainConversation()
    {
        float height = Screen.height * 0.8f / 2f;
        float depth = -0.9677734f;
        Vector3 player = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 2 / 3,
                                     height));
        Vector3 host = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 4,
                                   height));
        scenePlayer.transform.position = new Vector3(player.x, player.y, depth);
        sceneHost.transform.position = new Vector3(host.x, host.y, depth);
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
                    tablePosition.x + passage * offset * (passage % 2 < 0.01 ? 1 : -1),
                    tablePosition.y + passage * offset * (passage % 2 < 0.01 ? -1 : 1),
                    depth));
                objective.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                objective.gameObject.SetActive(true);
                passage++;
            }
        }
    }

    // TradeOff
    public void PrepareTradeOffs(GameObject family)
    {
        currentTradeOffPair = -1;
        m_familyTradeoffs.Clear();
        m_familyTradeoffs.Add((family.transform.GetChild(0).gameObject, family.transform.GetChild(1).gameObject));
        if (family.transform.childCount == 3)
        {
            m_familyTradeoffs.Add((family.transform.GetChild(1).gameObject, family.transform.GetChild(2).gameObject));
        }
    }

    public void NextTradeOff()
    {
        if (currentTradeOffPair < m_familyTradeoffs.Count - 1)
        {
            currentTradeOffPair++;
            ClearCharacters();

            string leftObjectiveName = m_familyTradeoffs[currentTradeOffPair].Item1.name;
            string rightObjectiveName = m_familyTradeoffs[currentTradeOffPair].Item2.name;
            ShowTradeoffBattler(m_familyTradeoffs[currentTradeOffPair].Item1, tradeoffLeftBattlerUIPosition);
            ShowTradeoffBattler(m_familyTradeoffs[currentTradeOffPair].Item2, tradeoffRightBattlerUIPosition);
            UpdateSliderLabels(leftObjectiveName, rightObjectiveName);
            // This isn't great but due to time constraints I had to generate the string here instead of creating a proper 
            // structure that handles these associations
            TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().GenerateConversation(
                $"2.2.3_Battles_obj{leftObjectiveName.Last()}vsobj{rightObjectiveName.Last()}");
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

    private void UpdateSliderLabels(string leftObjectiveName, string rightObjectiveName)
    {
        // Representation sliders
        //    Best value
        tradeoffLeftBattlerUIPosition.transform.GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>().text =
            controllers.GetComponent<TestingEnvironment>().Objectives[leftObjectiveName.ToLower()].best.ToString();
        tradeoffRightBattlerUIPosition.transform.GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>().text =
            controllers.GetComponent<TestingEnvironment>().Objectives[rightObjectiveName.ToLower()].best.ToString();
        //    worst value
        tradeoffLeftBattlerUIPosition.transform.GetChild(1).GetChild(4).GetComponent<TextMeshProUGUI>().text =
            controllers.GetComponent<TestingEnvironment>().Objectives[leftObjectiveName.ToLower()].worst.ToString();
        tradeoffRightBattlerUIPosition.transform.GetChild(1).GetChild(4).GetComponent<TextMeshProUGUI>().text =
            controllers.GetComponent<TestingEnvironment>().Objectives[rightObjectiveName.ToLower()].worst.ToString();
        //    unit value
        tradeoffLeftBattlerUIPosition.transform.GetChild(1).GetChild(5).GetComponent<TextMeshProUGUI>().text =
            controllers.GetComponent<TestingEnvironment>().Objectives[leftObjectiveName.ToLower()].unit;
        tradeoffRightBattlerUIPosition.transform.GetChild(1).GetChild(5).GetComponent<TextMeshProUGUI>().text =
            controllers.GetComponent<TestingEnvironment>().Objectives[rightObjectiveName.ToLower()].unit;
        
        // Compromise sliders
        //    Best value
        tradeoffLeftBattlerUIPosition.transform.GetChild(2).GetChild(3).GetComponent<TextMeshProUGUI>().text =
            controllers.GetComponent<TestingEnvironment>().Objectives[rightObjectiveName.ToLower()].best.ToString();
        tradeoffRightBattlerUIPosition.transform.GetChild(2).GetChild(3).GetComponent<TextMeshProUGUI>().text =
            controllers.GetComponent<TestingEnvironment>().Objectives[leftObjectiveName.ToLower()].best.ToString();
        //    worst value
        tradeoffLeftBattlerUIPosition.transform.GetChild(2).GetChild(4).GetComponent<TextMeshProUGUI>().text =
            controllers.GetComponent<TestingEnvironment>().Objectives[rightObjectiveName.ToLower()].worst.ToString();
        tradeoffRightBattlerUIPosition.transform.GetChild(2).GetChild(4).GetComponent<TextMeshProUGUI>().text =
            controllers.GetComponent<TestingEnvironment>().Objectives[leftObjectiveName.ToLower()].worst.ToString();
        //    unit value
        tradeoffLeftBattlerUIPosition.transform.GetChild(2).GetChild(5).GetComponent<TextMeshProUGUI>().text =
            controllers.GetComponent<TestingEnvironment>().Objectives[rightObjectiveName.ToLower()].unit;
        tradeoffRightBattlerUIPosition.transform.GetChild(2).GetChild(5).GetComponent<TextMeshProUGUI>().text =
            controllers.GetComponent<TestingEnvironment>().Objectives[leftObjectiveName.ToLower()].unit;
    }
}