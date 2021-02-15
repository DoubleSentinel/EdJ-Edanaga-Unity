using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SimpleJSON;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class ConversationHandler : MonoBehaviour
{
    [SerializeField] private GameObject talkingCharacterPointer;
    [SerializeField] private string[] conversationTitles;
    [SerializeField] private Transform LeftCursorPosition;
    [SerializeField] private Transform CenterCursorPosition;
    [SerializeField] private Transform RightCursorPosition;
    
    private GameObject controllers;
    private BackendAPI m_api;

    private Dictionary<string, JSONNode> conversations;
    private string currentConversationTitle;
    private int currentConversationPage;
    private int currentConversationSnippet;
    private TextMeshProAnimated conversationBubble;
    private string[] conversationToRead;

    public delegate void ConversationEnd();

    public ConversationEnd callback;

    // parsing markers
    private const string ObjectiveLoser = "objectiveloser";
    private const string ObjectiveWinner = "objectivewinner";

    [HideInInspector] public string[] tradeOffWinnerLoserReplacement = new string[2];

    // Unity calls Awake after all active GameObjects in the Scene are initialized
    void Awake()
    {
        if (controllers == null)
        {
            controllers = GameObject.Find("Controllers");
        }

        m_api = controllers.GetComponent<BackendAPI>();

        conversations = new Dictionary<string, JSONNode>();

        conversationBubble = GetComponentInChildren<TextMeshProAnimated>();

        currentConversationPage = 1;
        currentConversationSnippet = 0;
    }

    private void Start()
    {
        FetchConversations();
    }

    public void FetchConversations()
    {
        foreach (string title in conversationTitles)
        {
            m_api.parameters.Clear();
            m_api.parameters.Add("conversation_title", title);
            m_api.parameters.Add("language", controllers.GetComponent<LanguageHandler>().GetCurrentLanguageID());

            m_api.ApiList("scenes", response => { conversations.Add(title, JSONNode.Parse(response)["data"][0]); },
                m_api.parameters);
        }
    }

    public void GenerateConversation(int conversationId)
    {
        ToggleConversation(true);
        currentConversationSnippet = 0;
        currentConversationPage = 1;
        currentConversationTitle = conversationTitles[conversationId];
        conversationToRead = new string[conversations[currentConversationTitle]["conversation_content"].Count];
        int iterator = 0;
        foreach (JSONNode conversationExchange in conversations[currentConversationTitle]["conversation_content"])
        {
            var decoded = BackendAPI.DecodeEncodedNonAsciiCharacters(conversationExchange["text"]);
            conversationToRead[iterator] = ReplaceCustomMarkers(decoded);
            iterator++;
        }

        conversationBubble.StopAllCoroutines();
        conversationBubble.ParseText(conversationToRead[currentConversationSnippet]);
    }

    public void GenerateConversation(string title)
    {
        GenerateConversation(Array.IndexOf(conversationTitles, title));
    }

    // Method used on button click and to start the conversation
    public void NextConversationSnippet()
    {
        if (conversationBubble.isWriting)
        {
            StopAllCoroutines();
            conversationBubble.ShowCurrentPage();
        }
        else
        {
            // while there still are conversation snippets
            if (currentConversationSnippet < conversations[currentConversationTitle]["conversation_content"].Count)
            {
                MoveAndRotateCharacterPointer();
                // as long as we're not at the end of all the pages, the button will show the next page
                if (currentConversationPage <= conversationBubble.textInfo.pageCount)
                {
                    StartCoroutine(conversationBubble.ReadPage(currentConversationPage));
                    currentConversationPage++;
                }
                // when out of pages for the current snippet, reset page number and move to the next snippet
                else
                {
                    currentConversationPage = 1;
                    currentConversationSnippet++;

                    if(currentConversationSnippet>=conversationToRead.Length)
                        EndConversation();
                    else
                    {
                        conversationBubble.ParseText(conversationToRead[currentConversationSnippet]);
                        NextConversationSnippet();
                    }
                }
            }
        }
    }

    public void EndConversation()
    {
        currentConversationSnippet = 0;
        currentConversationPage = 1;
        conversationBubble.text = string.Empty;
        callback?.Invoke();
        ToggleConversation(false);
    }
    
    // Private utility functions
    private string ReplaceCustomMarkers(string text)
    {
        // this pattern recognizes value tags
        const string tag = @"val\(\w+:\w+\)";
        return Regex.Replace(text, tag, (match) =>
        {
            // extracts the parameters in the parentheses
            string[] parameters = match.ToString().Split('(', ')')[1].Split(':');
            var replacementObjective = controllers.GetComponent<TestingEnvironment>().Objectives;

            var conditional = ConditionalObjectiveValueReplacement(parameters, replacementObjective);
            if (conditional != null)
                return conditional;
            var value = Convert.ToDecimal(replacementObjective[parameters[0].ToLower()].GetValue(parameters[1].ToLower()).ToString());
            return $"{Math.Round(value)}";
        });
    }

    private void ToggleConversation(bool showConversation)
    {
        if (showConversation)
        {
            var parent = conversationBubble.transform.parent;
            parent.gameObject.SetActive(true);
            parent.GetComponent<CanvasGroup>().DOFade(1f, 0.2f);
        }
        else
        {
            conversationBubble.transform.parent.GetComponent<CanvasGroup>().DOFade( 0f, 0.2f).OnComplete(
                () =>
                {
                    conversationBubble.transform.parent.gameObject.SetActive(false);
                });
        }
    }

    private string ConditionalObjectiveValueReplacement(string[] parameters,
        Dictionary<string, Objective> replacementObjective)
    {
        if (tradeOffWinnerLoserReplacement != null)
        {
            switch (parameters[0].ToLower())
            {
                case ObjectiveWinner:
                    return replacementObjective[tradeOffWinnerLoserReplacement[0].ToLower()].GetValue(parameters[1].ToLower()).ToString();
                case ObjectiveLoser:
                    return replacementObjective[tradeOffWinnerLoserReplacement[1].ToLower()].GetValue(parameters[1].ToLower()).ToString();
            }
        }

        return null;
    }

    private void MoveAndRotateCharacterPointer()
    {
        var tgtName = conversations[currentConversationTitle]["conversation_content"][currentConversationSnippet][
            "target_character"];
        GameObject tgtGameObject = null;

        var replacement = ConditionalObjectiveValueReplacement(new string[] {tgtName, "name"},
            controllers.GetComponent<TestingEnvironment>().Objectives);
        tgtGameObject = replacement != null
            ? GameObject.Find(FirstLetterToUpper(replacement))
            : GameObject.Find(FirstLetterToUpper(tgtName));

        MoveCloserToTarget(talkingCharacterPointer, tgtGameObject);
        LookAt2D(talkingCharacterPointer, tgtGameObject, 180);
    }

    private void MoveCloserToTarget(GameObject cursor, GameObject worldTarget)
    {
        cursor.SetActive(true);
        var rt = cursor.GetComponent<RectTransform>();
        var tgt = Camera.main.WorldToScreenPoint(worldTarget.transform.position);
        var screenThird = Screen.width * 1/3;
        
        if (tgt.x <= screenThird)
            rt.position = LeftCursorPosition.position;
        else if (tgt.x <= screenThird * 2)
            rt.position = CenterCursorPosition.position;
        else if(tgt.x <= screenThird * 3)
            rt.position = RightCursorPosition.position;
        else
            rt.gameObject.SetActive(false);
    }

    private void LookAt2D(GameObject source2D, GameObject worldTarget, float angleOffset)
    {
        var diff = Camera.main.WorldToScreenPoint(worldTarget.transform.position) - source2D.transform.position;
        float angle = Mathf.Atan2(diff.x, diff.y) * Mathf.Rad2Deg;
        source2D.transform.rotation = Quaternion.Euler(0, 0, -angle + angleOffset);
    }

    public static string FirstLetterToUpper(string str)
    {
        if (str == null)
            return null;

        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);

        return str.ToUpper();
    }
}