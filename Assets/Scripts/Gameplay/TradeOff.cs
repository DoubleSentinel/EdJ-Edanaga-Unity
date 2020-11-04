using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DG.Tweening;
using Doozy.Engine;
using Doozy.Engine.Utils.ColorModels;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct IntermediateTradeOffPair
{
    public IntermediateTradeOffPair(float winnerSlider, Objective winner, Objective loser)
    {
        this.winner = winner;
        this.winnerSlider = winnerSlider;
        this.loser = loser;
        winnerWeight = 0f;
        loserWeight = 0f;
    }

    public Objective winner;
    public Objective loser;
    public float winnerWeight;
    public float loserWeight;

    public float winnerSlider;
}

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
    [SerializeField] private GameObject[] tradeOffButtons;
    [SerializeField] private GameObject resultListItemPrefab;

    // Environment
    private GameObject controllers;

    // UI
    private GameObject tradeOffLoserUI;
    private GameObject tradeOffWinnerUI;
    private GameObject leftRepresentationSlider;
    private GameObject leftCompromiseSlider;
    private GameObject rightRepresentationSlider;
    private GameObject rightCompromiseSlider;

    // Families
    private List<(GameObject, GameObject)> m_familyTradeoffs;
    private int currentTradeOffPair;

    // TradeOff calculations variables
    private List<IntermediateTradeOffPair> tradeOffWeightMatrix;
    public List<(string, float)> objectiveWeightsFamilyA;
    public List<(string, float)> objectiveWeightsFamilyB;
    public List<(string, float)> objectiveWeightsFamilyC;
    public List<(string, float)> objectiveWeightsFamilyD;
    public List<(string, float)> globalWeights;
    private string winnerName;
    private string loserName;

    // Flags
    private bool finals = false;

    private void Awake()
    {
        controllers = GameObject.Find("Controllers");

        currentTradeOffPair = -1;
        m_familyTradeoffs = new List<(GameObject, GameObject)>();

        // UI references
        leftRepresentationSlider = GameObject.Find("LeftBattlerRepresentationSlider");
        leftCompromiseSlider = GameObject.Find("LeftBattlerCompromiseSlider");

        rightRepresentationSlider = GameObject.Find("RightBattlerRepresentationSlider");
        rightCompromiseSlider = GameObject.Find("RightBattlerCompromiseSlider");

        HideTradeOffUI();

        // TradeOff weight calculation
        tradeOffWeightMatrix = new List<IntermediateTradeOffPair>();
        objectiveWeightsFamilyA = new List<(string, float)>();
        objectiveWeightsFamilyB = new List<(string, float)>();
        objectiveWeightsFamilyC = new List<(string, float)>();
        objectiveWeightsFamilyD = new List<(string, float)>();
        globalWeights = new List<(string, float)>();
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
            var winnerData = controllers.GetComponent<TestingEnvironment>().Objectives[winnerName.ToLower()];
            var loserData = controllers.GetComponent<TestingEnvironment>().Objectives[loserName.ToLower()];
            var sliderWinner = tradeOffLoserUI.transform.GetChild(2).GetComponent<Slider>();

            tradeOffWeightMatrix.Add(new IntermediateTradeOffPair(sliderWinner.value, winnerData, loserData));

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
                tradeOffWeightMatrix.Clear();
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
        GameObject clone = Instantiate(winner2D, tradeOffFinalists.transform);
        // trimming the clone name
        clone.name = clone.name.Remove(10);
        var map = new int[4][];
        map[0] = new[] {0, 1};
        map[1] = new[] {2, 3, 4};
        map[2] = new[] {5, 6, 7};
        map[3] = new[] {8, 9};
        for (int i = 0; i < map.Length; i++)
        {
            if (map[i].Contains(int.Parse(clone.name.Last().ToString())))
                clone.transform.SetSiblingIndex(i);
        }

        clone.SetActive(false);
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

    private void TradeOffSceneForwarding()
    {
        // If all finalists are ready to be weighted
        if (tradeOffFinalists.transform.childCount == 4)
        {
            if (!finals)
            {
                GameEventMessage.SendEvent("GoToFinalsConversation");
                GetComponent<ControllerChapter2_2>().conversationIndex = 1;
                ShowTradeOffBackground(backgroundTradeOff, true);
                HideTradeOffUI();
                finals = true;
                // setting up callback after conversation to move onto finals
                GetComponent<ControllerChapter2_2>().conversationCallback = () =>
                {
                    GameEventMessage.SendEvent("GoToFinals");
                    PrepareTradeOffs(tradeOffFinalists);
                    ShowTradeOffBackground(backgroundTradeOff, false);
                    ShowTradeOffBackground(backgroundTradeOffFinals, true);
                };
            }
            else
            {
                GameEventMessage.SendEvent("GoToResultConversation");
                GetComponent<ControllerChapter2_2>().conversationIndex = 2;
                ShowTradeOffBackground(backgroundTradeOffFinals, false);
                
                // setting up callback after conversation to move onto results screen
                GetComponent<ControllerChapter2_2>().conversationCallback = () =>
                {
                    GameEventMessage.SendEvent("GoToTradeOffResults");
                };
            }
        }
        else
        {
            GameEventMessage.SendEvent("GoToTables");
            HideTradeOffUI();
            ShowTradeOffBackground(backgroundTradeOff, false);
        }
    }

    private void CalculateLocalWeights(List<(string, float)> family)
    {
        if (tradeOffWeightMatrix.Count == 1)
        {
            var pair = tradeOffWeightMatrix[0];
            var R = 1 - pair.winnerSlider / 20;
            pair.loserWeight = 1 / (R + 1);
            pair.winnerWeight = R * pair.loserWeight;

            family.Add((pair.loser.name, pair.loserWeight));
            family.Add((pair.winner.name, pair.winnerWeight));
        }
        else if (tradeOffWeightMatrix.Count == 2)
        {
            var pair = tradeOffWeightMatrix[0];
            var pair1 = tradeOffWeightMatrix[1];
            var R = 1 - pair.winnerSlider / 20;
            var Z = 1 - pair1.winnerSlider / 20;
            pair1.loserWeight = 1 / (R * Z + Z + 1);
            pair1.winnerWeight = Z * pair1.loserWeight;
            pair.winnerWeight = R * pair1.winnerWeight;

            family.Add((pair1.loser.name, pair1.loserWeight));
            family.Add((pair1.winner.name, pair1.winnerWeight));
            family.Add((pair.loser.name, pair.winnerWeight));
        }
        else if (tradeOffWeightMatrix.Count == 3)
        {
            var pair = tradeOffWeightMatrix[0];
            var pair1 = tradeOffWeightMatrix[1];
            var pair2 = tradeOffWeightMatrix[2];
            var R = 1 - pair.winnerSlider / 20;
            var Z = 1 - pair1.winnerSlider / 20;
            var J = 1 - pair2.winnerSlider / 20;
            pair2.loserWeight = 1 / ((R * Z * J) + (Z * J) + J + 1);
            pair2.winnerWeight = J * pair2.loserWeight;
            pair1.winnerWeight = Z * pair2.winnerWeight;
            pair.winnerWeight = R * pair1.winnerWeight;

            family.Add((pair2.loser.name, pair2.loserWeight));
            family.Add((pair2.winner.name, pair2.winnerWeight));
            family.Add((pair1.loser.name, pair1.winnerWeight));
            family.Add((pair.loser.name, pair.winnerWeight));
        }

        family.Sort((x, y) => x.Item2.CompareTo(y.Item2));
    }

    private void CalculateClassification()
    {
        var classification = controllers.GetComponent<TestingEnvironment>().TradeOffClassification;
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
        slider.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = $"{winnerData.best:0.0}";
        slider.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = $"{winnerData.worst:0.0}";
        //    update slider handle label
        slider.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            $"{winnerData.worst:0.0}";
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
    public void ResetTradeOffs()
    {
        GameEventMessage.SendEvent("GoToTitle");
        foreach (var button in tradeOffButtons)
        {
            button.GetComponent<Button>().interactable = true;
            button.GetComponent<CanvasGroup>().alpha = 1;
        }

        foreach (Transform child in tradeOffFinalists.transform)
        {
            Destroy(child.gameObject);
        }

        objectiveWeightsFamilyA.Clear();
        objectiveWeightsFamilyB.Clear();
        objectiveWeightsFamilyC.Clear();
        objectiveWeightsFamilyD.Clear();

        controllers.GetComponent<TestingEnvironment>().TradeOffClassification.Clear();
        finals = false;

        ShowTradeOffBackground(backgroundTradeOffFinals, false);
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

    public void DeactivateButton(GameObject caller)
    {
        caller.GetComponent<Button>().interactable = false;
    }

    public void UpdateUserSelection(GameObject handleLabel)
    {
        var winnerData = controllers.GetComponent<TestingEnvironment>().Objectives[winnerName.ToLower()];
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
        var results = controllers.GetComponent<TestingEnvironment>().TradeOffClassification;
        var objectives = controllers.GetComponent<TestingEnvironment>().Objectives;
        foreach (KeyValuePair<string, float> result in results.OrderByDescending(x => x.Value))
        {
            var resultItem = Instantiate(resultListItemPrefab, resultList.transform);
            var resultData = objectives[result.Key];

            resultItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{resultData.description} ({result.Value*100:0.0}%)";
            var rt = resultItem.transform.GetChild(0).GetComponent<RectTransform>();
            rt.localScale = new Vector3(result.Value, rt.localScale.y, rt.localScale.z);
        }
    }
}