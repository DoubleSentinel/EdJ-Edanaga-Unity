﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Doozy.Engine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeOff : MonoBehaviour
{
    [Header("Backgrounds References")] [SerializeField]
    private GameObject backgroundTradeOff;

    [SerializeField] private GameObject backgroundTradeOffFinals;

    [Header("UI and 2D References")] [SerializeField]
    private GameObject tradeOffFinalists;

    [SerializeField] private GameObject tradeoffLeftBattlerUIPosition;
    [SerializeField] private GameObject tradeoffRightBattlerUIPosition;
    [SerializeField] private GameObject TradeoffBattleConversationBubble;

    // Environment
    private GameObject controllers;

    // UI
    private GameObject tradeOffLoserUI;
    private GameObject tradeOffWinnerUI;
    private GameObject leftRepresentationSlider;
    private GameObject leftCompromiseSlider;
    private GameObject rightRepresentationSlider;
    private GameObject rightCompromiseSlider;

    // 2D
    private GameObject lastClone;

    // Families
    private List<(GameObject, GameObject)> m_familyTradeoffs;
    private int currentTradeOffPair;

    // TradeOff calculations variables
    private List<(string, float)> tradeOffWeightMatrix;
    private object[] tradeOffResult;
    private string winnerName;
    private string loserName;

    // Finals
    private bool finals = false;

    private void Awake()
    {
        controllers = GameObject.Find("Controllers");

        currentTradeOffPair = -1;
        m_familyTradeoffs = new List<(GameObject, GameObject)>();

        leftRepresentationSlider = GameObject.Find("LeftBattlerRepresentationSlider");
        leftCompromiseSlider = GameObject.Find("LeftBattlerCompromiseSlider");

        rightRepresentationSlider = GameObject.Find("RightBattlerRepresentationSlider");
        rightCompromiseSlider = GameObject.Find("RightBattlerCompromiseSlider");

        tradeOffWeightMatrix = new List<(string, float)>();
        tradeOffResult = new object[2];
        HideTradeOffUI();
    }

    public void PrepareTradeOffs(GameObject family)
    {
        currentTradeOffPair = -1;
        m_familyTradeoffs.Clear();
        for (int i = 0; i < family.transform.childCount - 1; i++)
        {
            var left = family.transform.GetChild(i).gameObject;
            var right = family.transform.GetChild(i + 1).gameObject;
            m_familyTradeoffs.Add((left, right));
        }
    }


    public void NextTradeOff()
    {
        // this condition exists only when a tradeoff winner has been selected
        if (tradeOffLoserUI != null)
        {
            // setting selected winner result
            var winnerData = controllers.GetComponent<TestingEnvironment>().Objectives[winnerName.ToLower()];
            var winner2DCharacter = m_familyTradeoffs[currentTradeOffPair].Item1.name.ToLower() == winnerData.name
                ? m_familyTradeoffs[currentTradeOffPair].Item1
                : m_familyTradeoffs[currentTradeOffPair].Item2;
            var slider = tradeOffLoserUI.transform.GetChild(2).GetComponent<Slider>();

            tradeOffResult[0] = winnerName;
            tradeOffResult[1] = ConvertSliderValue(slider, winnerData);

            // TODO: Change this to take the higher weight when there are multiple battles in one family
            if (!finals)
            {
                // Copying Finalist to Finalist group
                var familyName = winner2DCharacter.transform.parent.name;
                try
                {
                    controllers.GetComponent<TestingEnvironment>().TradeOffResults.Add(familyName, tradeOffResult);
                }
                catch (ArgumentException)
                {
                    controllers.GetComponent<TestingEnvironment>().TradeOffResults[familyName] = tradeOffResult;
                    Destroy(lastClone);
                }

                lastClone = Instantiate(winner2DCharacter, tradeOffFinalists.transform);
                lastClone.name = lastClone.name.Remove(10);
                var map = new int[4][];
                map[0] = new[]{ 0, 1};
                map[1] = new[]{ 2, 3, 4};
                map[2] = new[]{ 5, 6, 7};
                map[3] = new[]{ 8, 9};
                for (int i = 0;i<map.Length;i++)
                {
                    if (map[i].Contains(int.Parse(lastClone.name.Last().ToString())))
                        lastClone.transform.SetSiblingIndex(i);
                }
                lastClone.SetActive(false);
            }
            tradeOffLoserUI = null;
        }

        if (currentTradeOffPair < m_familyTradeoffs.Count - 1)
        {
            currentTradeOffPair++;
            GetComponent<ControllerChapter2_2>().ClearCharacters();
            ShowTradeOffBackground(finals ? backgroundTradeOffFinals : backgroundTradeOff, true);

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
            // If all tradeoffs have been done and these aren't the finals, go to the finals conversation else back to tables
            if (controllers.GetComponent<TestingEnvironment>().TradeOffResults.Count == 4 && !finals)
            {
                GameEventMessage.SendEvent("GoToFinalsConversation");
                GetComponent<ControllerChapter2_2>().conversationIndex = 1;
                ShowTradeOffBackground(finals ? backgroundTradeOffFinals : backgroundTradeOff, true);
                finals = true;
                GetComponent<ControllerChapter2_2>().conversationCallback = () => 
                {
                    GameEventMessage.SendEvent("GoToFinals");
                    PrepareTradeOffs(tradeOffFinalists);
                    ShowTradeOffBackground(finals ? backgroundTradeOffFinals : backgroundTradeOff, true);
                };
            }
            else
            {
                GameEventMessage.SendEvent("GoToTradeOffResults");
                ShowTradeOffBackground(finals ? backgroundTradeOffFinals : backgroundTradeOff, false);
            }
        }
    }
    //    // Called by the Button moving onto the next Tradeoff Pair (Next)
    //    // and by the TradeOff Battle view show event
    //    public void NextTradeOff()
    //    {
    //        // this condition exists only when a tradeoff winner has been selected
    //        if (tradeOffLoserUI != null && tradeOffWinnerUI != null)
    //        {
    //            // add the loser and winner's data to tradeOffResults list with their slider value
    //            var loserSlider = tradeOffLoserUI.transform.GetChild(2).GetComponent<Slider>();
    //            var winnerSlider = tradeOffWinnerUI.transform.GetChild(2).GetComponent<Slider>();
    //            tradeOffWeightMatrix.Add((winnerName, winnerSlider.value));
    //            tradeOffWeightMatrix.Add((loserName, loserSlider.value));
    //        }

    //        // while the current family still has tradeOffs to do
    //        if (currentTradeOffPair < m_familyTradeoffs.Count - 1)
    //        {
    //            currentTradeOffPair++;
    //            GetComponent<ControllerChapter2_2>().ClearCharacters();
    //            StartTradeOffPair();
    //        }
    //        else
    //        {
    //            // 
    //            {
    //                // var winnerData = controllers.GetComponent<TestingEnvironment>().Objectives[winnerName.ToLower()];
    //                // var loserData = controllers.GetComponent<TestingEnvironment>().Objectives[loserName.ToLower()];
    //                //// var winner2DCharacter = m_familyTradeoffs[currentTradeOffPair].Item1.name.ToLower() == winnerData.name
    //                ////     ? m_familyTradeoffs[currentTradeOffPair].Item1
    //                ////     : m_familyTradeoffs[currentTradeOffPair].Item2;
    //                // var loserSlider = tradeOffLoserUI.transform.GetChild(2).GetComponent<Slider>();
    //                // var winnerSlider = tradeOffWinnerUI.transform.GetChild(2).GetComponent<Slider>();
    //                // 

    //                // // TODO: Change this to take the higher weight when there are multiple battles in one family
    //                // if (!finals)
    //                // {
    //                //     // Copying Finalist to Finalist group
    //                //     var familyName = winner2DCharacter.transform.parent.name;
    //                //     try
    //                //     {
    //                //         controllers.GetComponent<TestingEnvironment>().TradeOffResults.Add(familyName, tradeOffResult);
    //                //     }
    //                //     catch (ArgumentException)
    //                //     {
    //                //         controllers.GetComponent<TestingEnvironment>().TradeOffResults[familyName] = tradeOffResult;
    //                //         Destroy(lastClone);
    //                //     }

    //                //     lastClone = Instantiate(winner2DCharacter, tradeOffFinalists.transform);
    //                //     lastClone.name = lastClone.name.Remove(10);
    //                //     var map = new int[4][];
    //                //     map[0] = new[]{ 0, 1};
    //                //     map[1] = new[]{ 2, 3, 4};
    //                //     map[2] = new[]{ 5, 6, 7};
    //                //     map[3] = new[]{ 8, 9};
    //                //     for (int i = 0;i<map.Length;i++)
    //                //     {
    //                //         if (map[i].Contains(int.Parse(lastClone.name.Last().ToString())))
    //                //             lastClone.transform.SetSiblingIndex(i);
    //                //     }
    //                //     lastClone.SetActive(false);
    //                // }
    //                // tradeOffLoserUI = null;
    //                // tradeOffWinnerUI = null;
    //            }
    //            // If all tradeoffs have been done and these aren't the finals, go to the finals conversation else back to tables
    //            if (controllers.GetComponent<TestingEnvironment>().TradeOffResults.Count == 4 && !finals)
    //            {
    //                GameEventMessage.SendEvent("GoToFinalsConversation");
    //                GetComponent<ControllerChapter2_2>().conversationIndex = 1;
    //                finals = true;
    //                GetComponent<ControllerChapter2_2>().HostBargainConversationBubble
    //                    .GetComponent<ConversationHandler>()
    //                    .callback = () =>
    //                {
    //                    GameEventMessage.SendEvent("GoToFinals");
    //                    PrepareTradeOffs(tradeOffFinalists);
    //                };
    //            }
    //            else
    //            {
    //                GameEventMessage.SendEvent("GoToTables");
    //                ShowTradeOffBackground(finals ? backgroundTradeOffFinals : backgroundTradeOff, false);
    //            }
    //        }
    //    }

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
        tradeOffWinnerUI = Equals(caller.transform.parent.gameObject, tradeoffRightBattlerUIPosition)
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
        //TODO: replace these with ToggleTradeOffUI
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
    private void StartTradeOffPair()
    {
        ShowTradeOffBackground(finals ? backgroundTradeOffFinals : backgroundTradeOff, true);
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

    private void ShowTradeOffBackground(GameObject background, bool isShown)
    {
        background.SetActive(isShown);
    }

    private float ConvertSliderValue(Slider slider, Objective loserData)
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
        objective.GetComponent<CharacterSpriteController>().MaximizeSymbol(false);
        objective.SetActive(true);
    }

    private void UpdateTradeOffLoserCompromiseSlider(Objective winnerData)
    {
        var slider = tradeOffLoserUI.transform.GetChild(2).gameObject;
        //    update best/worst labels
        slider.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = winnerData.best.ToString();
        slider.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = winnerData.worst.ToString();
        //    update slider handle label
        slider.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
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
        leftCompromiseSlider.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            rightObjective.worst.ToString();
        rightCompromiseSlider.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
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

    // TODO: rewrite this to toggle with fades
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
        var slider = handleLabel.transform.parent.parent.parent.GetComponent<Slider>();
        handleLabel.GetComponent<TextMeshProUGUI>().text =
            String.Format("{0:000.0}", ConvertSliderValue(slider, winnerData).ToString());
    }

    public void ToggleHandleLabel(GameObject handleLabel)
    {
        var cg = handleLabel.GetComponent<CanvasGroup>();
        cg.DOFade(Math.Abs(cg.alpha) < 0.01 ? 1 : 0, 0.2f);
    }
}