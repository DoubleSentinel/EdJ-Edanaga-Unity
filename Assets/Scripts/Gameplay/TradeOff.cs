﻿using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Doozy.Engine;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;
using MathNet.Numerics.LinearAlgebra;

public class TradeOff : MonoBehaviour
{
    [Header("Backgrounds References")] [SerializeField]
    private GameObject backgroundTradeOff;

    [SerializeField] private GameObject backgroundTradeOffFinals;

    [Header("UI and 2D References")] [SerializeField]
    private GameObject tradeOffFinalists;

    private List<GameObject> tradeOffFinalistsList = new List<GameObject>();

    [SerializeField] private GameObject tradeoffLeftBattlerUIPosition;
    [SerializeField] private GameObject tradeoffRightBattlerUIPosition;
    [SerializeField] private GameObject leftRepresentationSlider;
    [SerializeField] private GameObject leftCompromiseSlider;
    [SerializeField] private GameObject rightRepresentationSlider;
    [SerializeField] private GameObject rightCompromiseSlider;
    [SerializeField] private GameObject[] tradeOffStartUIElements;
    [SerializeField] private GameObject TradeoffBattleConversationBubble;
    
    [Header("Results UI References")]
    [SerializeField] private GameObject resultListItemPrefab;
    [SerializeField] private Sprite[] resultListIcons;

    // Environment
    private GameObject controllers;
    private TestingEnvironment test_env;

    // UI
    private GameObject tradeOffLoserUI;
    private GameObject tradeOffWinnerUI;

    // Families
    private List<(GameObject, GameObject)> m_familyTradeoffs;
    private int currentTradeOffPair;

    // TradeOff calculations variables
    private List<(string, double[])> sliderValues;
    public List<(string, double)> objectiveWeightsFamilyA;
    public List<(string, double)> objectiveWeightsFamilyB;
    public List<(string, double)> objectiveWeightsFamilyC;
    public List<(string, double)> objectiveWeightsFamilyD;
    public List<(string, double)> globalWeights;
    private string winnerName;
    private string loserName;

    // Flags
    private bool finals = false;

    private void Awake()
    {
        controllers = GameObject.Find("Controllers");
        test_env = controllers.GetComponent<TestingEnvironment>();

        currentTradeOffPair = -1;
        m_familyTradeoffs = new List<(GameObject, GameObject)>();

        HideTradeOffUI();
        // initial situation where selection buttons are not visible
        // this allows design to be handled on inspector without having to change object
        // state there
        tradeoffLeftBattlerUIPosition.transform.GetChild(0).gameObject.SetActive(false);
        tradeoffRightBattlerUIPosition.transform.GetChild(0).gameObject.SetActive(false);

        // TradeOff weight calculation
        sliderValues = new List<(string, double[])>();
        
        // Initializing lists
        objectiveWeightsFamilyA = new List<(string, double)>
        {
            ("objective0", 0.0d),
            ("objective1", 0.0d),
        };
        
        objectiveWeightsFamilyB = new List<(string, double)>
        {
            ("objective2", 0.0d),
            ("objective3", 0.0d),
            ("objective4", 0.0d),
        };

        objectiveWeightsFamilyC = new List<(string, double)>
        {
            ("objective5", 0.0d),
            ("objective6", 0.0d),
            ("objective7", 0.0d),
        };

        objectiveWeightsFamilyD = new List<(string, double)>
        {
            ("objective8", 0.0d),
            ("objective9", 0.0d),
        };

        globalWeights = new List<(string, double)>
        {
            ("objective0", 0.0d),
            ("objective0", 0.0d),
            ("objective0", 0.0d),
            ("objective0", 0.0d),
        };

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
        if (tradeOffLoserUI != null && tradeOffWinnerUI != null)
        {
            // setting selected winner result
            var leftname = m_familyTradeoffs[currentTradeOffPair].Item1.name.ToLower();
            var rightname = m_familyTradeoffs[currentTradeOffPair].Item2.name.ToLower();

            var rightCompromise = rightCompromiseSlider.GetComponent<Slider>();
            var rightRepresentation = rightRepresentationSlider.GetComponent<Slider>();
            var leftCompromise = leftCompromiseSlider.GetComponent<Slider>();
            var leftRepresentation = leftRepresentationSlider.GetComponent<Slider>();
            
            sliderValues.Add((leftname, new[]
            {
                Convert.ToDouble(leftRepresentation.value / leftRepresentation.maxValue),
                Convert.ToDouble(leftCompromise.value / leftCompromise.maxValue)
            }));

            sliderValues.Add((rightname, new[]
            {
                Convert.ToDouble(rightRepresentation.value / rightRepresentation.maxValue),
                Convert.ToDouble(rightCompromise.value / rightCompromise.maxValue)
            }));

            tradeOffLoserUI = null;
            tradeOffWinnerUI = null;
        }

        // if there still are tradeOffs to do
        if (currentTradeOffPair < m_familyTradeoffs.Count - 1)
        {
            currentTradeOffPair++;
            StartTradeOffPair();
        }
        else
        {
            if (!finals)
            {
                var familyName = m_familyTradeoffs[0].Item1.transform.parent.name;
                GameObject winner2D = null;
                string highestWeightedName = "";
                switch (familyName)
                {
                    case "FamilyA":
                        CalculateLocalWeights(objectiveWeightsFamilyA);
                        highestWeightedName = objectiveWeightsFamilyA[0].Item1;
                        break;
                    case "FamilyB":
                        CalculateLocalWeights(objectiveWeightsFamilyB);
                        highestWeightedName = objectiveWeightsFamilyB[0].Item1;
                        break;
                    case "FamilyC":
                        CalculateLocalWeights(objectiveWeightsFamilyC);
                        highestWeightedName = objectiveWeightsFamilyC[0].Item1;
                        break;
                    case "FamilyD":
                        CalculateLocalWeights(objectiveWeightsFamilyD);
                        highestWeightedName = objectiveWeightsFamilyD[0].Item1;
                        break;
                }

                foreach (var familyTradeoff in m_familyTradeoffs)
                {
                    if (familyTradeoff.Item1.name == highestWeightedName)
                        winner2D = familyTradeoff.Item1;
                    else
                        winner2D = familyTradeoff.Item2;
                }

                CloneForFinals(winner2D);
                sliderValues.Clear();
            }
            else
            {
                CalculateLocalWeights(globalWeights);
                CalculateClassification();
            }

            TradeOffSceneForwarding();
        }
    }

    private void CloneForFinals(GameObject winner2D)
    {
        GameObject clone = Instantiate(winner2D, transform);
        clone.name = clone.name.Remove(10);
        tradeOffFinalistsList.Add(clone);
        clone.SetActive(false);

        // sorting children by name once all finalists have been chosen
        if (tradeOffFinalistsList.Count == 4)
        {
            var orderedFinalists = tradeOffFinalistsList.OrderBy(x => int.Parse(x.name.Last().ToString()));
            foreach (var go in orderedFinalists)
            {
                go.transform.SetParent(tradeOffFinalists.transform);
            }

            tradeOffFinalistsList.Clear();
        }
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
        GetComponent<ControllerChapter2_2>().ClearCharacters();
        if (finals)
            ShowFinalsBackground(true);
        else
            ShowTradeOffBackground(true);
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

        TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().tradeOffWinnerLoserReplacement = null;
        TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().GenerateConversation(title);
        TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().NextConversationSnippet();
        TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().callback = ToggleSelectionButtons;

        ToggleNextTradeOffButton();
    }

    private void TradeOffSceneForwarding()
    {
        // If all finalists are ready to be weighted
        if (tradeOffFinalists.transform.childCount == 4)
        {
            if (!finals)
            {
                GameEventMessage.SendEvent("GoToFinalsIntroConversation");
                ShowTradeOffBackground(true);
                finals = true;
                // setting up callback after conversation to move onto final tradeoffs
                GetComponent<ControllerChapter2_2>().groupedConversationCallback = () =>
                {
                    GameEventMessage.SendEvent("GoToTradeOffFinals");
                    PrepareTradeOffs(tradeOffFinalists);
                    ShowTradeOffBackground(false);
                    ShowFinalsBackground(true);
                };
            }
            else
            {
                GameEventMessage.SendEvent("GoToResultConversation");
                GetComponent<ControllerChapter2_2>().hostConversationIndex = 1;
                ShowFinalsBackground(false);

                // setting up callback after conversation to move onto results screen
                GetComponent<ControllerChapter2_2>().hostConversationCallback = () =>
                {
                    GameEventMessage.SendEvent("GoToTradeOffResults");
                };
            }
        }
        else
        {
            GameEventMessage.SendEvent("GoToTables");
            ShowTradeOffBackground(false);
        }

        HideTradeOffUI();
    }

    private void CalculateLocalWeights(List<(string, double)> family)
    {
        var order = sliderValues.Count / 2 + 1;
        
        Matrix<double> coefficients = Matrix<double>.Build.Dense(order, order, 0d);
        for (int i = 0; i < order; i++)
        {
            coefficients[order-1, i] = 1;
        }
        
        Vector<double> equation_right_hand_side = Vector<double>.Build.Dense(sliderValues.Count/2 + 1, 1);

        for (int i = 0; i < sliderValues.Count/2; i++)
        {
            equation_right_hand_side[i] = 0;
            coefficients[i, i] = sliderValues[i].Item2[0] - sliderValues[i + 1].Item2[1];
            coefficients[i, i + 1] = sliderValues[i].Item2[1] - sliderValues[i + 1].Item2[0];
        }
        
        var results = coefficients.Solve(equation_right_hand_side);
        for (int i = 0; i < family.Count; i++)
        {
            family[i] = (family[i].Item1, results[i]);
        }
        
        family.Sort((x, y) => x.Item2.CompareTo(y.Item2));
    }

    private void CalculateClassification()
    {
         var classification = test_env.TradeOffClassification;
         classification.Clear();
         // FamilyA
         classification.Add(
             objectiveWeightsFamilyA[0].Item1,
             objectiveWeightsFamilyA[0].Item2 * globalWeights[0].Item2);
         classification.Add(
             objectiveWeightsFamilyA[1].Item1,
             objectiveWeightsFamilyA[1].Item2 * globalWeights[0].Item2);

         // FamilyB
         classification.Add(
             objectiveWeightsFamilyB[0].Item1,
             objectiveWeightsFamilyB[0].Item2 * globalWeights[1].Item2);
         classification.Add(
             objectiveWeightsFamilyB[1].Item1,
             objectiveWeightsFamilyB[1].Item2 * globalWeights[1].Item2);
         classification.Add(
             objectiveWeightsFamilyB[2].Item1,
             objectiveWeightsFamilyB[2].Item2 * globalWeights[1].Item2);

         // FamilyC
         classification.Add(
             objectiveWeightsFamilyC[0].Item1,
             objectiveWeightsFamilyC[0].Item2 * globalWeights[2].Item2);
         classification.Add(
             objectiveWeightsFamilyC[1].Item1,
             objectiveWeightsFamilyC[1].Item2 * globalWeights[2].Item2);
         classification.Add(
             objectiveWeightsFamilyC[2].Item1,
             objectiveWeightsFamilyC[2].Item2 * globalWeights[2].Item2);

         // FamilyD
         classification.Add(
             objectiveWeightsFamilyD[0].Item1,
             objectiveWeightsFamilyD[0].Item2 * globalWeights[3].Item2);
         classification.Add(
             objectiveWeightsFamilyD[1].Item1,
             objectiveWeightsFamilyD[1].Item2 * globalWeights[3].Item2);
    }

    private void ShowTradeOffBackground(bool isShown)
    {
        backgroundTradeOff.SetActive(isShown);
    }

    private void ShowFinalsBackground(bool isShown)
    {
        backgroundTradeOffFinals.SetActive(isShown);
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
        slider.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = $"{winnerData.best:0.0}";
        slider.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = $"{winnerData.worst:0.0}";
        //    update slider handle label
        slider.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            $"{winnerData.worst:0.0}";
    }

    private void UpdateTradeOffSliders(string leftObjectiveName, string rightObjectiveName)
    {
        Objective leftObjective =
            test_env.Objectives[leftObjectiveName.ToLower()];
        Objective rightObjective =
            test_env.Objectives[rightObjectiveName.ToLower()];
        // Representation sliders labels
        //    Best value
        leftRepresentationSlider.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text =
            $"{leftObjective.best:0.0}";
        rightRepresentationSlider.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text =
            $"{rightObjective.best:0.0}";
        //    worst value
        leftRepresentationSlider.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text =
            $"{leftObjective.worst:0.0}";
        rightRepresentationSlider.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text =
            $"{rightObjective.worst:0.0}";
        //    unit value
        leftRepresentationSlider.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text =
            leftObjective.unit;
        rightRepresentationSlider.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text =
            rightObjective.unit;

        // Compromise sliders
        //    Best value
        leftCompromiseSlider.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text =
            $"{rightObjective.best:0.0}";
        leftCompromiseSlider.GetComponent<Slider>().maxValue = 20;
        rightCompromiseSlider.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text =
            $"{leftObjective.best:0.0}";
        rightCompromiseSlider.GetComponent<Slider>().maxValue = 20;
        //    worst value labels
        leftCompromiseSlider.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text =
            $"{rightObjective.worst:0.0}";
        rightCompromiseSlider.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text =
            $"{leftObjective.worst:0.0}";
        //    slider handle value labels
        leftCompromiseSlider.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            $"{rightObjective.worst:0.0}";
        rightCompromiseSlider.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            $"{leftObjective.worst:0.0}";
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
    }

    private void ShowTradeOffUI()
    {
        tradeoffLeftBattlerUIPosition.GetComponent<CanvasGroup>().alpha = 1;
        tradeoffRightBattlerUIPosition.GetComponent<CanvasGroup>().alpha = 1;
        tradeoffLeftBattlerUIPosition.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 1;
        tradeoffRightBattlerUIPosition.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 1;
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
    public void ResetTradeOffs()
    {
        GameEventMessage.SendEvent("GoToTitleChapter3");
        foreach (var element in tradeOffStartUIElements)
        {
            element.GetComponent<EventTrigger>().enabled = true;
        }

        foreach (Transform child in tradeOffFinalists.transform)
        {
            Destroy(child.gameObject);
        }

        objectiveWeightsFamilyA.Clear();
        objectiveWeightsFamilyB.Clear();
        objectiveWeightsFamilyC.Clear();
        objectiveWeightsFamilyD.Clear();

        test_env.TradeOffClassification.Clear();
        sliderValues.Clear();

        GetComponent<ControllerChapter2_2>().isTradeOff = true;
        GetComponent<ControllerChapter2_2>().hostConversationIndex = 0;
        GetComponent<ControllerChapter2_2>().hostConversationCallback =
            () => { GameEventMessage.SendEvent("GoToTables"); };

        finals = false;
        currentTradeOffPair = -1;

        ShowFinalsBackground(false);
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
        tradeOffWinnerUI = Equals(caller.transform.parent.gameObject, tradeoffRightBattlerUIPosition)
            ? tradeoffRightBattlerUIPosition
            : tradeoffLeftBattlerUIPosition;

        TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().tradeOffWinnerLoserReplacement =
            new[] {winnerName, loserName};
        TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().callback = EndTradeOffConversation;
        TradeoffBattleConversationBubble.GetComponent<ConversationHandler>()
            .GenerateConversation("2.2.3_Battles_After_Selection");
        TradeoffBattleConversationBubble.GetComponent<ConversationHandler>().NextConversationSnippet();

        UpdateTradeOffLoserCompromiseSlider(test_env
            .Objectives[winnerName.ToLower()]);
        ToggleSelectionButtons();
        ShowTradeOffUI();
    }

    public void UpdateUserSelection(GameObject handleLabel)
    {
        var winnerData = test_env.Objectives[winnerName.ToLower()];
        var slider = handleLabel.transform.parent.parent.parent.GetComponent<Slider>();
        handleLabel.GetComponent<TextMeshProUGUI>().text = $"{ConvertSliderValue(slider, winnerData):0.0}";
    }

    public void ToggleHandleLabel(GameObject handleLabel)
    {
        var cg = handleLabel.GetComponent<CanvasGroup>();
        cg.DOFade(Math.Abs(cg.alpha) < 0.01 ? 1 : 0, 0.2f);
    }

    public void UpdateResultList(GameObject resultList)
    {
        var results = test_env.TradeOffClassification;
        var objectives = test_env.Objectives;
        foreach (Transform child in resultList.transform)
        {
            Destroy(child.gameObject);
        }

        // creating the visual list with the given prefab
        foreach (KeyValuePair<string, double> result in results.OrderByDescending(x => x.Value))
        {
            var resultItem = Instantiate(resultListItemPrefab, resultList.transform);
            var resultData = objectives[result.Key];
            var goRef = GameObject.Find(ConversationHandler.FirstLetterToUpper(result.Key));

            resultItem.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                $"{resultData.description} ({result.Value * 100:0.0}%)";

            resultItem.transform.GetChild(2).GetComponent<Image>().sprite =
                resultListIcons[int.Parse(result.Key.Last().ToString())];

            // background color
            resultItem.GetComponent<Image>().color = goRef.GetComponent<Coloration>().fond;
            // fill color
            resultItem.transform.GetChild(0).GetComponent<Image>().color = goRef.GetComponent<Coloration>().contour;

            var rt = resultItem.transform.GetChild(0).GetComponent<RectTransform>();
            rt.localScale = new Vector3((float) result.Value, rt.localScale.y, rt.localScale.z);
        }
    }
}