using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public GameObject CustomOrderingButton;
    public GameObject ResultContinueButton;
    public GameObject Choice4;
    
    private GameObject controllers;
    private TestingEnvironment env;
    private bool hadConsistencyCheck;
    
    
    void Awake()
    {
        controllers = GameObject.Find("Controllers");
        env = controllers.GetComponent<TestingEnvironment>();
        hostConversationCallback = () => { GameEventMessage.SendEvent("GoToResults"); };
        hadConsistencyCheck = false;
    }
    private void Start()
    {
        controllers.GetComponent<LanguageHandler>().translateUI();
    }

    private bool IsConsistent()
    {
        var tradeOffList = env.TradeOffClassification
            .OrderByDescending(x => x.Value).Take(5).ToList();
        var swingList = env.TradeOffClassification
            .OrderByDescending(x => x.Value).Take(5).ToList();
        
        // if tradeOffList contains the same objectives as swinglist regardless of order (checking their names [Key])
        if (AreListsEqual(tradeOffList.OrderBy(e => e.Key).ToList(), swingList.OrderBy(e => e.Key).ToList()))
        {
            // if tradeOffList contains the same objectives as swing list in the same order
            if (AreListsEqual(tradeOffList, swingList))
            {
                return true;
            }

            var result =tradeOffList.All(tradeoff =>
                !swingList.Where(swing => tradeoff.Key.ToLower() == swing.Key.ToLower())
                    .Any(swing => Math.Abs(tradeoff.Value - swing.Value) > 0.05f));
            return result;
        }
        return false;

        // TODO: verify this again
        bool AreListsEqual(List<KeyValuePair<string, float>> list1, List<KeyValuePair<string, float>> list2)
        {
            return !(from list1Data in list1 from list2Data in list2 where list1Data.Key != list2Data.Key select list1Data).Any();
        }
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
        if (env.isInnerLoopFirstRun)
        {
            env.isInnerLoopFirstRun = false;
            Choice4.SetActive(false);
        }
        else
        {
            Choice4.SetActive(true);
        }
    }

    public void InconsistentUserChoice(int choice)
    {
        switch (choice)
        {
            case 1:
                env.SkipSwing = true;
                env.SkipTradeOff = false;
                break;
            case 2:
                env.SkipSwing = false;
                env.SkipTradeOff = true;
                break;
            case 3:
                env.SkipSwing = false;
                env.SkipTradeOff = false;
                break;
        }
    }

    public void ResultsUserChoice(bool isTradeOff)
    {
        env.UsersSelectedClassification = isTradeOff ? env.TradeOffClassification : env.SwingClassification;
        hostConversationIndex = 3;
        hostConversationCallback = () =>
        {
            GameEventMessage.SendEvent("GoToChapter6");
        };
    }

    public void UpdateResultLists()
    {
        var objectives = env.Objectives;
        var TradeOffData = env.TradeOffClassification;
        var SwingData = env.SwingClassification;

        BuildList(TradeOffData, TradeOffList.transform);
        BuildList(SwingData, SwingList.transform);

        if (!hadConsistencyCheck)
        {
            print("consistency check");
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
        CustomOrderingButton.SetActive(isVisible);
    }

    private void ToggleContinueButton(bool isVisible)
    {
        ResultContinueButton.SetActive(isVisible);
    }
}
