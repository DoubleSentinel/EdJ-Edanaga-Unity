using System.Collections;
using System.Collections.Generic;
using System.Threading;
using SimpleJSON;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class ConversationHandler : MonoBehaviour
{
    [SerializeField] private string[] conversationTitles;
    [SerializeField] private bool isVertical = false;

    private GameObject controllers;
    private BackendAPI m_api;

    private Dictionary<string, JSONNode> conversations;
    private string currentConversationTitle;
    private int currentConversationPage;
    private int currentConversationSnippet;
    private TextMeshProAnimated conversationBubble;

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

    public void FetchConversations()
    {
        foreach (string title in conversationTitles)
        {
            m_api.parameters.Clear();
            m_api.parameters.Add("conversation_title", title);
            m_api.parameters.Add("language", controllers.GetComponent<LanguageHandler>().GetCurrentLanguage());

            m_api.ApiList("scenes", response => { conversations.Add(title, JSONNode.Parse(response)["data"][0]); },
                m_api.parameters);
        }
    }

    public void GenerateConversation(string title)
    {
        currentConversationTitle = title;
        foreach (JSONNode conversationExchange in conversations[title]["conversation_content"])
        {
            conversationExchange["text"] = BackendAPI.DecodeEncodedNonAsciiCharacters(conversationExchange["text"]);
        }

        conversationBubble.text = conversations[currentConversationTitle]
            ["conversation_content"][currentConversationSnippet]["text"];
        conversationBubble.maxVisibleCharacters = 0;
    }


    // Method used on button click and to start the conversation
    public void NextConversationSnippet()
    {
        // while there still are conversation snippets
        if (currentConversationSnippet <= conversations[currentConversationTitle]["conversation_content"].Count)
        {
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

                conversationBubble.text = conversations[currentConversationTitle]
                    ["conversation_content"][currentConversationSnippet]["text"];
                conversationBubble.maxVisibleCharacters = 0;
                // This currently doesn't queue the first page of the next snippet until user presses button
                // figure it out
            }
        }
        else
        {
            // end conversation
            // TODO: remove dialog ui
            currentConversationSnippet = 0;
        }
    }
}