using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Doozy.Engine;
using Doozy.Engine.Extensions;
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
    [SerializeField] private GameObject tradeOffFinalists;

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

    // TradeOff organization
    private List<(GameObject, GameObject)> m_familyTradeoffs;
    private int currentTradeOffPair;

    // TradeOff Scoring vars
    private GameObject tradeOffLoserUI;
    private object[] tradeOffResult;
    private string winnerName;
    private string loserName;
    
    // BargainConversation vars
    private int conversationIndex = 0;
    private bool finals = false;

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

        // 0= tradeoff winner name, 1= tradeoff loser value
        tradeOffResult = new object[2];

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

        HostBargainConversationBubble.GetComponent<ConversationHandler>().GenerateConversation(conversationIndex);
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
                    tablePosition.y + passage * offset * (passage % 2 < 0.01 ? 1 : -1),
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
        for (int i = 0; i < family.transform.childCount-1; i++)
        {
            var left = family.transform.GetChild(i).gameObject;
            var right = family.transform.GetChild(i + 1).gameObject;
            m_familyTradeoffs.Add((left, right));
        }
    }


    // Called by the Button moving onto the next Tradeoff Pair (Next)
    // and by the TradeOff Battle view show event
    public void NextTradeOff()
    {
        if (tradeOffLoserUI != null)
        {
            // setting selected winner result
            var winnerData = controllers.GetComponent<TestingEnvironment>().Objectives[winnerName.ToLower()];
            var winner2DCharacter = m_familyTradeoffs[currentTradeOffPair].Item1.name.ToLower() == winnerData.name
                ? m_familyTradeoffs[currentTradeOffPair].Item1
                : m_familyTradeoffs[currentTradeOffPair].Item2;
            var slider = tradeOffLoserUI.transform.GetChild(2).GetComponent<Slider>();

            tradeOffResult[0] = winnerName;
            tradeOffResult[1] = CalculateUserInput(slider, winnerData);

            // TODO: Change this to take the higher weight when there are multiple battles in one family
            var familyName = winner2DCharacter.transform.parent.name;
            try
            {
                controllers.GetComponent<TestingEnvironment>().TradeOffResults.Add(familyName, tradeOffResult);
            }
            catch (ArgumentException)
            {
                controllers.GetComponent<TestingEnvironment>().TradeOffResults[familyName] = tradeOffResult;
            }

            // this is shitty be careful go to Instantiate instead
           // var winnerClone = winner2DCharacter.Clone();
           // winnerClone.transform.SetParent(tradeOffFinalists.transform);

            tradeOffLoserUI = null;
        }

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
            string title = "";
            if (finals)
            {
                title = $"2.2.5_FinalBattles_obj{leftObjectiveName.Last()}_vs_obj{rightObjectiveName.Last()}";
            }
            else
            {
                title = $"2.2.3_Battles_obj{leftObjectiveName.Last()}_vs_obj{rightObjectiveName.Last()}";
            }

            TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().winnerLoserReplacement = null;
            TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().GenerateConversation(title);
            TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().NextConversationSnippet();
            TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().callback = ToggleSelectionButtons;

            ToggleNextTradeOffButton();
        }
        else
        {
            // If all tradeoffs have been done, go to the finals conversation else back to tables
            if (controllers.GetComponent<TestingEnvironment>().TradeOffResults.Count == 4)
            {
                GameEventMessage.SendEvent("GoToFinalsConversation");
                conversationIndex = 1;
                finals = true;
                HostBargainConversationBubble.GetComponent<ConversationHandler>().callback = () =>
                {
                    GameEventMessage.SendEvent("GoToFinals");
                    PrepareTradeOffs(tradeOffFinalists);
                };
            }
            else
            {
                GameEventMessage.SendEvent("GoToTables");
            }
        }
    }

    // Called by the Left/RightBattlerSelectButtons on the TradeOff Battle View
    public void SelectTradeOffWinner(GameObject caller)
    {
        winnerName = caller.name == "LeftBattlerSelectButton"
            ? m_familyTradeoffs[currentTradeOffPair].Item1.name
            : m_familyTradeoffs[currentTradeOffPair].Item2.name;
        loserName = caller.name == "RightBattlerSelectButton"
            ? m_familyTradeoffs[currentTradeOffPair].Item1.name
            : m_familyTradeoffs[currentTradeOffPair].Item2.name;
        tradeOffLoserUI = Equals(caller.transform.parent.gameObject, tradeoffLeftBattlerUIPosition)
            ? tradeoffRightBattlerUIPosition
            : tradeoffLeftBattlerUIPosition;

        TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().winnerLoserReplacement =
            new[] {winnerName, loserName};
        TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().callback = EndTradeOffConversation;
        TradeoffBattleConversationBubble.GetComponent<ConversationHandler>()
            .GenerateConversation("2.2.3_Battles_After_Selection");
        TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().NextConversationSnippet();

        UpdateTradeOffLoserCompromiseSlider(controllers.GetComponent<TestingEnvironment>()
            .Objectives[winnerName.ToLower()]);
        ToggleSelectionButtons();
    }

    // ---------------------------- Callback Functions on conversation Ends ------------
    private void EndTradeOffConversation()
    {
        // show tradeoff ui and activate the loser's compromise slider
        tradeoffLeftBattlerUIPosition.GetComponent<CanvasGroup>().DOFade(1, 0.2f);
        tradeoffRightBattlerUIPosition.GetComponent<CanvasGroup>().DOFade(1, 0.2f);
        tradeOffLoserUI.transform.GetChild(2).GetComponent<Slider>().interactable = true;
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
    private float CalculateUserInput(Slider slider, Objective loserData)
    {
        var step = (loserData.best - loserData.worst) / slider.maxValue;
        return slider.value * step + loserData.worst;
    }

    private void ShowTradeoffBattler(GameObject objective, GameObject tradeOffUIPosition)
    {
        objective.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(
            tradeOffUIPosition.transform.position.x,
            tradeOffUIPosition.transform.position.y,
            1.0f
        ));
        objective.SetActive(true);
    }

    private void UpdateTradeOffLoserCompromiseSlider(Objective winnerData)
    {
        var slider = tradeOffLoserUI.transform.GetChild(2).gameObject;
        //    update best/worst labels
        slider.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = winnerData.best.ToString();
        slider.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = winnerData.worst.ToString();
        //    update slider handle label
        slider.transform.GetChild(6).GetComponent<TextMeshProUGUI>().text =
            winnerData.worst.ToString();
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
        //    worst value labels
        leftCompromiseSlider.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text =
            rightObjective.worst.ToString();
        rightCompromiseSlider.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text =
            leftObjective.worst.ToString();
        //    slider handle value labels
        leftCompromiseSlider.transform.GetChild(6).GetComponent<TextMeshProUGUI>().text =
            rightObjective.worst.ToString();
        rightCompromiseSlider.transform.GetChild(6).GetComponent<TextMeshProUGUI>().text =
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
        var go = GameObject.Find("NextTradeOff");
        var btn = go.GetComponent<Button>();
        var cg = btn.GetComponent<CanvasGroup>();
        cg.DOFade(Math.Abs(cg.alpha) < 0.01 ? 1 : 0, 0.2f);
        btn.interactable = !btn.interactable;
    }

    // -------------------- Utility UI callabales  -----------------------------
    public void DeactivateButton(GameObject caller)
    {
        caller.GetComponent<Button>().interactable = false;
    }

    public void UpdateUserSelection(GameObject handleLabel)
    {
        var winnerData = controllers.GetComponent<TestingEnvironment>().Objectives[winnerName.ToLower()];
        var slider = handleLabel.transform.parent.GetComponent<Slider>();
        handleLabel.GetComponent<TextMeshProUGUI>().text =
            String.Format("{0:000.0}", CalculateUserInput(slider, winnerData).ToString());
    }

    public void ToggleHandleLabel(GameObject handleLabel)
    {
        var cg = handleLabel.GetComponent<CanvasGroup>();
        cg.DOFade(Math.Abs(cg.alpha) < 0.01 ? 1 : 0, 0.2f);
    }
}