using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using TMPro;
using UnityEngine;

public class ConversationHandler : MonoBehaviour
{
    [SerializeField] private string[] conversationTitles;
    [SerializeField] private bool isVertical = false;

    private GameObject controllers;
    private BackendAPI m_api;

    private Dictionary<string, JSONNode> conversations;
    
    // Unity calls Awake after all active GameObjects in the Scene are initialized
    void Awake()
    {
        if (controllers == null)
        {
            controllers = GameObject.Find("Controllers");
        }
        m_api = controllers.GetComponent<BackendAPI>();
        
        conversations = new Dictionary<string, JSONNode>();
    }

    public void FetchConversations()
    {
        foreach (string title in conversationTitles)
        {
            m_api.parameters.Clear();
            m_api.parameters.Add("conversation_title", title);
            m_api.parameters.Add("language", controllers.GetComponent<LanguageHandler>().GetCurrentLanguage());
            
            m_api.ApiList("scenes", response =>
            {
                conversations.Add(title, JSONNode.Parse(response)["data"][0]);
            }, m_api.parameters);
        }
    }

    public void GenerateConversation(string title)
    {
        string decoded =
            BackendAPI.DecodeEncodedNonAsciiCharacters(conversations[title]["conversation_content"][2]["text"]);
        ;
        GetComponentInChildren<TextMeshProAnimated>().text = decoded;
        
        StartCoroutine(GetComponentInChildren<TextMeshProAnimated>().ReadAllPages());
    }


    public void NextConversationItem()
    {
        //conversationBubble.GetComponentInChildren<TextMeshProAnimated>().ReadText()
    }
}
