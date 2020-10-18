using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Doozy.Engine.Nody;
using Doozy.Engine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

    private GameObject leftRepresentationSlider;
    private GameObject leftCompromiseSlider;

    private GameObject rightRepresentationSlider;
    private GameObject rightCompromiseSlider;

    // Local variables
    private GameObject controllers;
    private BackendAPI m_api;

    private List<(GameObject, GameObject)> m_familyTradeoffs;
    private int currentTradeOffPair;

    private GameObject tradeOffLoser;

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

        leftRepresentationSlider = GameObject.Find("LeftBattlerRepresentationSlider");
        leftCompromiseSlider = GameObject.Find("LeftBattlerCompromiseSlider");

        rightRepresentationSlider = GameObject.Find("RightBattlerRepresentationSlider");
        rightCompromiseSlider = GameObject.Find("RightBattlerCompromiseSlider");

        HideTradeOffUI();
    }


    // --------------------  UI Callables  --------------------------------
    public void ClearCharacters()
    {
        foreach (GameObject character in GameObject.FindGameObjectsWithTag("Character"))
        {
            character.transform.position = Vector3.forward;
            character.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            character.SetActive(false);
        }
    }

    // View - 2.2.1/6/8 - Bargain conversation setup
    public void SetupBargainConversation()
    {
        float height = Screen.height * 0.8f / 2f;
        float depth = -1f;
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
                    tablePosition.y + passage * offset * (passage % 2 < 0.01 ? -1 : 1),
                    depth));
                objective.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                objective.gameObject.SetActive(true);
                passage++;
            }
        }
    }

    // ---------------------- TradeOff -----------------------------------------
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

    // Called by the Button moving onto the next Tradeoff Pair (Next)
    // and by the TradeOff Battle view show event
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
            UpdateTradeOffSliders(leftObjectiveName, rightObjectiveName);
            // This isn't great but due to time constraints I had to generate the string here instead of creating a proper 
            // structure that handles these associations
            TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().winnerLoserReplacement = null;
            TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().GenerateConversation(
                $"2.2.3_Battles_obj{leftObjectiveName.Last()}vsobj{rightObjectiveName.Last()}");
            TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().NextConversationSnippet();
            TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().callback = ToggleSelectionButtons;
            
            ToggleNextTradeOffButton();
        }
        else
        {
            GameObject.Find("Graph Controller - Chapter2.2").GetComponent<GraphController>()
                .GoToNodeByName("2.2.2/7 - Tables");
        }
    }

    // Called by the Left/RightBattlerSelectButtons on the TradeOff Battle View
    public void SelectTradeOffWinner(GameObject caller)
    {
        string winner = caller.name == "LeftBattlerSelectButton"
            ? m_familyTradeoffs[currentTradeOffPair].Item1.name
            : m_familyTradeoffs[currentTradeOffPair].Item2.name;
        string loser = caller.name == "RightBattlerSelectButton"
            ? m_familyTradeoffs[currentTradeOffPair].Item1.name
            : m_familyTradeoffs[currentTradeOffPair].Item2.name;
        tradeOffLoser = Equals(caller.transform.parent.gameObject, tradeoffLeftBattlerUIPosition)
            ? tradeoffRightBattlerUIPosition
            : tradeoffLeftBattlerUIPosition;

        TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().winnerLoserReplacement =
            new[]{winner, loser};
        TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().callback = EndTradeOffConversation;
        TradeoffBattleConversationBubble.GetComponent<ConversationHandler>()
            .GenerateConversation("2.2.3_Battles_After_Selection");
        TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().NextConversationSnippet();

        ToggleSelectionButtons();
    }

    // ---------------------------- Callback Functions on conversation Ends ------------
    private void EndTradeOffConversation()
    {
        tradeoffLeftBattlerUIPosition.GetComponent<CanvasGroup>().DOFade(1, 0.2f);
        tradeoffRightBattlerUIPosition.GetComponent<CanvasGroup>().DOFade(1, 0.2f);
        tradeOffLoser.transform.GetChild(2).GetComponent<Slider>().interactable = true;
        ToggleNextTradeOffButton();
    }

    private void ToggleSelectionButtons()
    {
        var left = tradeoffLeftBattlerUIPosition.transform.GetChild(0);
        var right = tradeoffRightBattlerUIPosition.transform.GetChild(0);
        left.gameObject.SetActive(!left.gameObject.activeSelf);
        right.gameObject.SetActive(!right.gameObject.activeSelf);
        left.GetComponent<CanvasGroup>().DOFade(left.gameObject.activeSelf ? 1 : 0, 0.2f);
        right.GetComponent<CanvasGroup>().DOFade(right.gameObject.activeSelf ? 1 : 0, 0.2f);
    }

    // ---------------------------- Utility methods ----------------------------------------
    private void ShowTradeoffBattler(GameObject objective, GameObject tradeOffUIPosition)
    {
        objective.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(
            tradeOffUIPosition.transform.position.x,
            tradeOffUIPosition.transform.position.y,
            1.0f
        ));
        objective.SetActive(true);
    }

    private void UpdateTradeOffSliders(string leftObjectiveName, string rightObjectiveName)
    {
        Objective leftObjective =
            controllers.GetComponent<TestingEnvironment>().Objectives[leftObjectiveName.ToLower()];
        Objective rightObjective =
            controllers.GetComponent<TestingEnvironment>().Objectives[rightObjectiveName.ToLower()];
        // Representation sliders labels
        //    Best value
        leftRepresentationSlider.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text =
            leftObjective.best.ToString();
        rightRepresentationSlider.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text =
            rightObjective.best.ToString();
        //    worst value
        leftRepresentationSlider.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text =
            leftObjective.worst.ToString();
        rightRepresentationSlider.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text =
            rightObjective.worst.ToString();
        //    unit value
        leftRepresentationSlider.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text =
            leftObjective.unit;
        rightRepresentationSlider.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text =
            rightObjective.unit;

        // Compromise sliders
        //    Best value
        leftCompromiseSlider.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text =
            rightObjective.best.ToString();
        leftCompromiseSlider.GetComponent<Slider>().maxValue = 20;
        rightCompromiseSlider.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text =
            leftObjective.best.ToString();
        rightCompromiseSlider.GetComponent<Slider>().maxValue = 20;
        //    worst value
        leftCompromiseSlider.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text =
            rightObjective.worst.ToString();
        rightCompromiseSlider.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text =
            leftObjective.worst.ToString();
        //    unit value
        leftCompromiseSlider.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text =
            rightObjective.unit;
        rightCompromiseSlider.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text =
            leftObjective.unit;
        //    default value
        leftCompromiseSlider.GetComponent<Slider>().value = 0;
        rightCompromiseSlider.GetComponent<Slider>().value = 0;
        leftCompromiseSlider.GetComponent<Slider>().interactable = false;
        rightCompromiseSlider.GetComponent<Slider>().interactable = false;
        
        HideTradeOffUI();
    }
    
    private void HideTradeOffUI()
    {
        tradeoffLeftBattlerUIPosition.GetComponent<CanvasGroup>().alpha = 0;
        tradeoffRightBattlerUIPosition.GetComponent<CanvasGroup>().alpha = 0;
        tradeoffLeftBattlerUIPosition.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;
        tradeoffRightBattlerUIPosition.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;
        tradeoffLeftBattlerUIPosition.transform.GetChild(0).gameObject.SetActive(false);
        tradeoffRightBattlerUIPosition.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void ToggleNextTradeOffButton()
    {
        var btn = GameObject.Find("NextTradeOff").GetComponent<CanvasGroup>();
        btn.DOFade(Math.Abs(btn.alpha) < 0.01 ? 1 : 0, 0.2f);
    }
}