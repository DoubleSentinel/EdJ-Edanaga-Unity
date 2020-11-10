using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Doozy.Engine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControllerChapter2_2 : MonoBehaviour
{
    [Header("2D Scene References")] [SerializeField]
    private GameObject sceneHost;

    [SerializeField] private GameObject scenePlayer;
    [SerializeField] private GameObject[] sceneFamilies;


    [Header("Bargain Conversation References")]
    public GameObject HostBargainConversationBubble;

    // Local variables
    private GameObject controllers;

    // BargainConversation vars
    [HideInInspector] public int conversationIndex = 0;
    public ConversationHandler.ConversationEnd conversationCallback;

    // Unity calls Awake after all active GameObjects in the Scene are initialized
    void Awake()
    {
        controllers = GameObject.Find("Controllers");
        conversationCallback = () => { GameEventMessage.SendEvent("GoToTables"); };
    }

    private void Start()
    {
        controllers.GetComponent<LanguageHandler>().translateUI();
        foreach (GameObject o in GameObject.FindGameObjectsWithTag("DialogBubble"))
        {
            o.GetComponent<ConversationHandler>().FetchConversations();
        }
    }

    // --------------------  UI Callables  --------------------------------
    public void ToggleBackground(GameObject background)
    {
        background.SetActive(!background.activeSelf);
    }

    public void ClearCharacters()
    {
        foreach (GameObject character in GameObject.FindGameObjectsWithTag("Character"))
        {
            character.transform.position = new Vector3(11, 0, 1);
            character.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

    // View - 2.2.1/6/8 - Bargain conversation setup
    public void SetupBargainConversation()
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

        HostBargainConversationBubble.GetComponent<ConversationHandler>().callback = conversationCallback;
        HostBargainConversationBubble.GetComponent<ConversationHandler>().GenerateConversation(conversationIndex);
        HostBargainConversationBubble.GetComponent<ConversationHandler>().NextConversationSnippet();
    }


    // View - 2.2.2/7 - Tables
    public void SetupBargainTables()
    {
        float depth = 1f;
        float offset = 20f;
        foreach (GameObject family in sceneFamilies)
        {
            int passage = 1;
            foreach (Transform objective in family.transform)
            {
                Vector3 tablePosition = GameObject.Find("UI" + family.name).transform.position;
                objective.position = Camera.main.ScreenToWorldPoint(new Vector3(
                    tablePosition.x + passage * offset * (passage % 2 < 0.01 ? 1 : -1),
                    tablePosition.y + passage * offset * (passage % 2 < 0.01 ? 1 : -1),
                    depth));
                objective.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                objective.gameObject.SetActive(true);
                objective.GetComponent<CharacterSpriteController>().MaximizeSymbol(true);
                passage++;
            }
        }
    }
}