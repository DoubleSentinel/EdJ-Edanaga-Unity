using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Doozy.Engine;
using Doozy.Engine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Swing : MonoBehaviour
{
    [SerializeField] private GameObject SwingUIPrefab;
    [SerializeField] private GameObject SwingConversationBubble;
    [SerializeField] private GameObject CurrentSwingUI;
    [SerializeField] private GameObject SwingFinalists;
    public GameObject ValidateSwingButton;

    // flags
    private bool finals = false;
    private bool isPlural = false;

    // local variables
    private GameObject currentSwingFamily;
    private Dictionary<GameObject, Dictionary<GameObject, float>> userInputValues;
    

    // linking ui to characters
    private Dictionary<GameObject, GameObject> characterToUIMap;
    
    private GameObject controllers;

    private void Awake()
    {
        characterToUIMap = new Dictionary<GameObject, GameObject>();
        userInputValues = new Dictionary<GameObject, Dictionary<GameObject, float>>();
        controllers = GameObject.Find("Controllers");
        ToggleValidationButton();
    }

    public void PrepareSwingWith(GameObject family)
    {
        currentSwingFamily = family;
    }

    public void StartSwing()
    {
        isPlural = currentSwingFamily.transform.childCount > 2;
        PrepareCharacters(currentSwingFamily);

        // This isn't great but due to time constraints I had to generate the string here instead of creating a proper 
        // structure that handles these associations
        string title = !finals ? $"2.2.8.2_Swing_fam{currentSwingFamily.name.Last()}" : $"2.2.9_Swing_round2";

        SwingConversationBubble.GetComponent<ConversationHandler>().GenerateConversation(title);
        SwingConversationBubble.GetComponent<ConversationHandler>().NextConversationSnippet();
        SwingConversationBubble.GetComponent<ConversationHandler>().callback = () => { ToggleSwingUI(true); };
    }

    private void ToggleSwingUI(bool isShown)
    {
        foreach (KeyValuePair<GameObject, GameObject> keyValuePair in characterToUIMap)
        {
            keyValuePair.Value.SetActive(isShown);
        }
    }

    private void AlternateSwingUI()
    {
        foreach (KeyValuePair<GameObject, GameObject> keyValuePair in characterToUIMap)
        {
            var isActive = keyValuePair.Value.transform.GetChild(0).gameObject.activeSelf;
            keyValuePair.Value.transform.GetChild(0).gameObject.SetActive(!isActive);
            keyValuePair.Value.transform.GetChild(1).gameObject.SetActive(isActive);
        }
    }

    private void PrepareCharacters(GameObject family)
    {
        float height = Screen.height * 0.9f / 2f;
        float depth = -1f;
        float step = Screen.width / family.transform.childCount;
        float offset = step / 2;

        int i = 0;
        foreach (Transform child in family.transform)
        {
            Vector3 screenPos = new Vector3(i * step + offset, height);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

            PrepareUI(child.gameObject, screenPos);

            child.position = new Vector3(worldPos.x, worldPos.y, depth);
            child.gameObject.SetActive(true);
            child.GetComponent<CharacterSpriteController>().MaximizeSymbol(false);
            i++;
        }
    }

    private void PrepareUI(GameObject character, Vector3 screenPosition)
    {
        var ui = Instantiate(SwingUIPrefab, CurrentSwingUI.transform);
        ui.transform.position = screenPosition;
        ui.SetActive(false);
        ui.transform.GetChild(1).gameObject.GetComponent<UIButton>().OnClick.OnTrigger.Action = delegate(GameObject swingSelect)
        {
            SetSwingUIButtonTrigger(swingSelect);
        };
        
        // Setting slider labels
        Objective objective = controllers.GetComponent<TestingEnvironment>().Objectives[character.name.ToLower()];
        var labelGroup = ui.transform.GetChild(0).GetChild(3);
        labelGroup.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{objective.best:0.0}";
        labelGroup.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{objective.worst:0.0}";
        labelGroup.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"{objective.unit}";
        
        characterToUIMap.Add(character, ui);
    }

    private void SetSwingUIButtonTrigger(GameObject swingSelect)
    {
        var swingUI = swingSelect.transform.parent.gameObject;
        swingUI.transform.GetChild(0).GetComponent<Slider>().value = 20;
        swingUI.transform.GetChild(0).GetComponent<Slider>().interactable = false;
        
        var winner = Instantiate(characterToUIMap.FirstOrDefault(x => x.Value == swingUI).Key, SwingFinalists.transform);
        winner.name = winner.name.Remove(10);
        winner.SetActive(false);
        
        ToggleSwingUI(false);
        SwingConversationBubble.GetComponent<ConversationHandler>()
            .GenerateConversation(isPlural
                ? "2.2.8.2_Swing_Explanation_Plural"
                : "2.2.8.2_Swing_Explanation_Singular");
        SwingConversationBubble.GetComponent<ConversationHandler>().NextConversationSnippet();
        SwingConversationBubble.GetComponent<ConversationHandler>().callback = SetSwingConversationCallback;
    }

    private void SetSwingConversationCallback()
    {
        ToggleValidationButton();
        // if this is the before last swing event, change the way the validate button works to
        // chain to a grouped conversation instead of back to tables which is it's default behavior
        if (userInputValues.Count == 3)
        {
            ValidateSwingButton.GetComponent<UIButton>().OnClick.OnTrigger.GameEvents.Clear();
            ValidateSwingButton.GetComponent<UIButton>().OnClick.OnTrigger.GameEvents
                .Add("GoToGroupedConversation");
        }

        ValidateSwingButton.GetComponent<UIButton>().OnClick.OnTrigger.Action =
            delegate(GameObject validateButton)
            {
                SetSwingValidateButtonTrigger();
            };
        ToggleSwingUI(true);
        AlternateSwingUI();
    }

    private void SetSwingValidateButtonTrigger()
    {
        CalculateLocalWeights();
        // once all swings have been made set up for final swing
        if (userInputValues.Count == 4)
        {
            foreach (Transform child in SwingFinalists.transform)
            {
                Instantiate(child.gameObject,
                    GetComponent<ControllerChapter2_2>().ConversationGroup.transform);
            }
            GetComponent<ControllerChapter2_2>().groupedConversationIndex = 2;
            GetComponent<ControllerChapter2_2>().groupedConversationCallback = () =>
            {
                GameEventMessage.SendEvent("GoToSwingFinals");
                foreach (Transform child in GetComponent<ControllerChapter2_2>().ConversationGroup.transform)
                {
                    Destroy(child.gameObject);
                }
                currentSwingFamily = SwingFinalists;
                finals = true;
            };
        }

        if (finals)
        {
            // TODO: Load next scene on button press
        }

        ToggleValidationButton();
        foreach (Transform child in CurrentSwingUI.transform)
        {
            Destroy(child.gameObject);
        }
        characterToUIMap.Clear();
    }

    private void CalculateLocalWeights()
    {
        var objectiveValues = new Dictionary<GameObject, float>();
        foreach (Transform child in CurrentSwingUI.transform)
        {
            var objective = characterToUIMap.FirstOrDefault(x => x.Value == child.gameObject).Key;
            var objVal = child.GetChild(0).GetComponent<Slider>().value;
            objectiveValues.Add(objective, objVal);
        }
        userInputValues.Add(currentSwingFamily, objectiveValues);
    }
    private void CalculateFinalWeights()
    {
        //controllers.GetComponent<TestingEnvironment>().SwingClassification.Add(objName, sliderVal);
    }

    private void ToggleValidationButton()
    {
        var button = ValidateSwingButton.GetComponent<Button>();
        var canvasGroup = ValidateSwingButton.GetComponent<CanvasGroup>();
        
        button.interactable = !button.interactable;
        canvasGroup.DOFade(Math.Abs(canvasGroup.alpha) < 0.01? 1f: 0f, 0.2f);
    }
}