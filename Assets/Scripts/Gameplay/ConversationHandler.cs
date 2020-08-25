using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

public class ConversationHandler : MonoBehaviour
{
    [SerializeField] private GameObject controllers;
    [SerializeField] private string[] conversationTitles;
    [SerializeField] private bool isVertical = false;

    private BackendAPI m_api;

    private Dictionary<string, JSONNode> conversations;
    private Dictionary<string, object> api_params;
    
    // Unity calls Awake after all active GameObjects in the Scene are initialized
    void Awake()
    {
        conversations = new Dictionary<string, JSONNode>();
        api_params = new Dictionary<string, object>();
        if (controllers == null)
        {
            controllers = GameObject.Find("Controllers");
        }
        m_api = controllers.GetComponent<BackendAPI>();
    }

    public void FetchConversations()
    {
        foreach (string title in conversationTitles)
        {
            api_params.Clear();
            api_params.Add("scene", title);
            api_params.Add("language", controllers.GetComponent<LanguageHandler>().GetCurrentLanguage());
            
            m_api.ApiList("scenes", response =>
            {
                conversations.Add(title, JSONNode.Parse(response));
            }, api_params);
        }
    }

    public void GenerateConversation(string title)
    {
        
    }

}
