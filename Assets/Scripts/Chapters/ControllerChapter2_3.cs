using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControllerChapter2_3 : MonoBehaviour
{
    [Header("2D Scene References")]
    [SerializeField] private GameObject sceneHost;
    [SerializeField] private GameObject scenePlayer;
    
    
    [Header("Conversation References")]
    [HideInInspector] public int hostConversationIndex = 0;
    public GameObject[] ConversationBubbles;
    public ConversationHandler.ConversationEnd hostConversationCallback;

    [Header("UI References")] 
    public GameObject TradeOffList;
    public GameObject SwingList;
    public GameObject ListItemPrefab;
    public GameObject SwingSelectButton;
    public GameObject TradeOffSelectButton;
    public GameObject ResultContinueButton;
    public GameObject Choice4;
    
    private GameObject controllers;
    private bool hadConsistencyCheck;
    
    
    void Awake()
    {
        controllers = GameObject.Find("Controllers");
        hostConversationCallback = () => { GameEventMessage.SendEvent("GoToResults"); };
        hadConsistencyCheck = false;
    }
    private void Start()
    {
        controllers.GetComponent<LanguageHandler>().translateUI();
    }

    private bool IsConsistent()
    {
        var tradeOffList = controllers.GetComponent<TestingEnvironment>().TradeOffClassification
            .OrderByDescending(x => x.Value).Take(5).ToList();
        var swingList = controllers.GetComponent<TestingEnvironment>().TradeOffClassification
            .OrderByDescending(x => x.Value).Take(5).ToList();
        if (tradeOffList.All(swingList.Contains))
        {
            if (tradeOffList.SequenceEqual(swingList))
            {
                return true;
            }

            return tradeOffList.All(tradeoff =>
                !swingList.Where(swing =>
                    tradeoff.Key.ToLower() == swing.Key.ToLower())
                    .Any(swing =>
                        Math.Abs(tradeoff.Value - swing.Value) > 0.05f));
        }

        return false;
    }

    // ------------------ UI Callables ------------------
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

    public void UpdateInnerLoopChoices()
    {
        var first = controllers.GetComponent<TestingEnvironment>().isInnerLoopFirstRun;
        if (first)
        {
            first = false;
            Choice4.SetActive(false);
        }
        else
        {
            
        }
    }

    public void UpdateResultLists()
    {
        var objectives = controllers.GetComponent<TestingEnvironment>().Objectives;
        var TradeOffData = controllers.GetComponent<TestingEnvironment>().TradeOffClassification;
        var SwingData = controllers.GetComponent<TestingEnvironment>().SwingClassification;

        BuildList(TradeOffData, TradeOffList.transform);
        BuildList(SwingData, SwingList.transform);

        if (!hadConsistencyCheck)
        {
            ToggleSelectionButtons(false);
            ToggleContinueButton(true);
            if (IsConsistent())
            {
                hostConversationIndex = 2;
                hostConversationCallback = () => { GameEventMessage.SendEvent("GoToResults"); };
            }
            else
            {
                hostConversationIndex = 1;
                hostConversationCallback = () =>
                {
                    GameEventMessage.SendEvent("GoToInnerLoopSelect");
                };
            }
            hadConsistencyCheck = true;
        }
        else
        {
            ToggleSelectionButtons(true);
            ToggleContinueButton(false);
        }

        void BuildList(Dictionary<string, float> listData, Transform parent)
        {
            foreach (Transform child in parent)
            {
                Destroy(child.gameObject);
            }
            
            foreach (KeyValuePair<string, float> objective in listData.OrderByDescending(x => x.Value))
            {
                var listItem = Instantiate(ListItemPrefab, parent);
                var objectiveData = objectives[objective.Key];
                var objectiveRef = GameObject.Find(ConversationHandler.FirstLetterToUpper(objective.Key));
                
                listItem.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
                    $"{objectiveData.description} ({objective.Value * 100:0.0}%)";

                // background color
                listItem.GetComponent<Image>().color = objectiveRef.GetComponent<Coloration>().fond;
                // fill color
                listItem.transform.GetChild(0).GetComponent<Image>().color = objectiveRef.GetComponent<Coloration>().contour;

                var rt = listItem.transform.GetChild(0).GetComponent<RectTransform>();
                rt.localScale = new Vector3(objective.Value, rt.localScale.y, rt.localScale.z);
            }
        }
    }

    private void ToggleSelectionButtons(bool isVisible)
    {
        SwingSelectButton.SetActive(isVisible);
        TradeOffSelectButton.SetActive(isVisible);
    }

    private void ToggleContinueButton(bool isVisible)
    {
        ResultContinueButton.SetActive(isVisible);
    }
}
