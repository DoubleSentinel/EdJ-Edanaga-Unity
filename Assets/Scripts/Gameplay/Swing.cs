using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine.UI;
using Shapes2D;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Swing : MonoBehaviour
{
    [SerializeField] private GameObject SwingUIPrefab;
    [SerializeField] private GameObject SwingConversationBubble;

    // flags
    private bool finals = false;
    private bool isPlural = false;

    // local variables
    private GameObject currentSwingFamily;
    private GameObject swingWinner;

    // linking ui to characters
    private Dictionary<GameObject, GameObject> characterToUIMap;

    private void Awake()
    {
        characterToUIMap = new Dictionary<GameObject, GameObject>();
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
            keyValuePair.Value.transform.GetChild(isActive?1:0).gameObject.SetActive(isActive);
            keyValuePair.Value.transform.GetChild(isActive?0:1).gameObject.SetActive(!isActive);
        }
    }

    private void PrepareCharacters(GameObject family)
    {
        float height = Screen.height * 1f / 2f;
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
        var ui = Instantiate(SwingUIPrefab, SwingConversationBubble.transform.parent);
        ui.transform.position = screenPosition;
        ui.SetActive(false);
        ui.transform.GetChild(1).gameObject.GetComponent<UIButton>().OnClick.OnTrigger.Action = delegate(GameObject o)
        {
            var swingUI = o.transform.parent.gameObject;
            swingWinner = characterToUIMap.FirstOrDefault(x => x.Value == swingUI).Key;
            swingUI.transform.GetChild(0).GetComponent<Slider>().value = 20;
            ToggleSwingUI(false);
            SwingConversationBubble.GetComponent<ConversationHandler>()
                .GenerateConversation(isPlural
                    ? "2.2.8.2_Swing_Explanation_Plural"
                    : "2.2.8.2_Swing_Explanation_Singular");
            SwingConversationBubble.GetComponent<ConversationHandler>().NextConversationSnippet();
            SwingConversationBubble.GetComponent<ConversationHandler>().callback = () =>
            {
                ToggleSwingUI(true);
                AlternateSwingUI();
            };
        };
        characterToUIMap.Add(character, ui);
    }
}