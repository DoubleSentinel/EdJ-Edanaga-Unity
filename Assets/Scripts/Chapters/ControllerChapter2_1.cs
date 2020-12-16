using DG.Tweening;
using Doozy.Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ControllerChapter2_1 : MonoBehaviour
{
    public Camera cam;

    [Header("2D Scene References")]
    private GameObject sceneObjective;

    [SerializeField] private GameObject scenePlayer;
    [SerializeField] private GameObject[] sceneObjectives;
    [SerializeField] private GameObject[] sceneBats;
    [SerializeField] private GameObject[] sceneButtons;
    [SerializeField] private Button btnContinue;

    private int notifications;
    public Color newColor = new Color(0f, 0f, 0f, 1f); // Set to opaque black;
    [Header("Conversation References")] public GameObject[] ConversationBubbles;

    // Local variables
    private GameObject controllers;

    // BargainConversation vars
    [HideInInspector] public int conversationIndex = 0;
    public ConversationHandler.ConversationEnd conversationCallback;

    void Awake()
    {
        if (btnContinue == null)
        {
            btnContinue = GameObject.Find("btnContinue").GetComponent<Button>();
        }
        controllers = GameObject.Find("Controllers");
        conversationCallback = () => { GameEventMessage.SendEvent("ContinueToTown"); };
    }

    private void Start()
    {
        controllers.GetComponent<LanguageHandler>().translateUI();
        //sceneObjectives = new GameObject[10];
    }

    private void Call(int conversationIndex)
    {
        string title = "";
        title = $"2.1.2_Dialogue_objective{conversationIndex}";
        var ch = ConversationBubbles[0].GetComponent<ConversationHandler>();
        ch.callback = conversationCallback;
        ch.GenerateConversation(conversationIndex);
        ch.NextConversationSnippet();
    }

    //Button continue appears when all the objectivves have been read 
    public void UpdateObjectiveButton(GameObject caller)
    {
        //caller.transform.GetChild(1).gameObject.SetActive(false);
        //caller.transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
        caller.transform.GetComponent<UnityEngine.UI.Image>().color = Color.red;
        notifications = 0;
        foreach (GameObject objective in sceneObjectives)
        {
            //GameObject notification = objective.GetChild(1).gameObject;
            //if (!notification.activeSelf) //Si désactiver
            //if(objective.GetChild(0).GetComponent<SpriteRenderer>().color == new Color(1, 0, 0, 1))
            if(caller.transform.GetComponent<UnityEngine.UI.Image>().color == Color.red)
            {
                notifications += 1;
                if (notifications == 10)
                {
                    btnContinue.gameObject.SetActive(true);
                    btnContinue.interactable = true;
                }
            }
        }
    }

    public void ClearCharacters()
    {
        foreach (GameObject character in GameObject.FindGameObjectsWithTag("Character"))
        {
            character.transform.position = new Vector3(11, 0, 1);
            character.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

    public void SetupCallConversation()
    {
        ClearCharacters();

        float height = Screen.height * 0.75f / 2f;
        float depth = -1f;
        Vector3 player = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 2 / 3,
            height));
        Vector3 host = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 4,
            height));
        scenePlayer.transform.position = new Vector3(player.x, player.y, depth);
        sceneObjective.transform.position = new Vector3(host.x, host.y, depth);
        scenePlayer.SetActive(true);
        sceneObjective.SetActive(true);
    }

    public void StartCall(GameObject objective)
    {
        //UpdateObjectiveButton(objective);
        GameEventMessage.SendEvent("GoToPhoneCall");
        string objectiveName = objective.name;
        int objectiveNumber = Convert.ToInt32($"{objective.name.Last()}");
        sceneObjective = sceneObjectives[objectiveNumber];
        SetupCallConversation();
        Call(objectiveNumber);
    }

    public void SetupButtons()
    {
        //Set buttons position to the batiments postion
        for (int i = 0; i < sceneBats.Length; i++)
        {
            sceneButtons[i].gameObject.transform.position = cam.WorldToScreenPoint(sceneBats[i].transform.position);

        }
    }
}
