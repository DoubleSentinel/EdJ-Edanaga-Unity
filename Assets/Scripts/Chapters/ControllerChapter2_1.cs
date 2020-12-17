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
    [Header("2D Scene References")]
    public Camera cam;
    Animator animator;
    int state;

    [SerializeField] private GameObject scenePlayer;
    [SerializeField] private GameObject[] sceneObjectives;
    [SerializeField] private GameObject[] sceneBats;
    [SerializeField] private GameObject[] sceneButtons;
    [SerializeField] private GameObject sceneObjective;
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

    //Button continue appears when all the objectives have been read
    public void UpdateObjectiveButton(int objectiveNumber)
    {
        //RuntimeAnimatorController ac = sceneBats[objectiveNumber].GetComponent<RuntimeAnimatorController>();
        //sceneBats[objectiveNumber].GetComponent<RuntimeAnimatorController>();
        
        //Change state of animator

        sceneBats[objectiveNumber].gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;
        notifications = 0;
        
        for (int i = 0; i < sceneBats.Length; i++)
        {
            if(sceneBats[i].gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color == Color.red)
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
        Vector3 host = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 2 / 3,
            height));
        Vector3 player = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 4,
            height));
        scenePlayer.transform.position = new Vector3(player.x, player.y, depth);
        sceneObjective.transform.position = new Vector3(host.x, host.y, depth);
        scenePlayer.SetActive(true);
        sceneObjective.SetActive(true);
    }

    public void StartCall(GameObject objective)
    {
        GameEventMessage.SendEvent("GoToPhoneCall");
        string objectiveName = objective.name;
        int objectiveNumber = Convert.ToInt32($"{objective.name.Last()}");
        sceneObjective = sceneObjectives[objectiveNumber];
        UpdateObjectiveButton(objectiveNumber);
        SetupCallConversation();
        Call(objectiveNumber);
    }

    //Started when the scene is shown
    public void SetupButtons()
    {
        //Set buttons position to the batiments postion
        for (int i = 0; i < sceneBats.Length; i++)
        {
            sceneButtons[i].gameObject.transform.position = cam.WorldToScreenPoint(sceneBats[i].transform.position);

        }
    }

    public void SetAnimatorState(int stateButton)
    {
        state = stateButton;
    }

    public void SetAnimatorParameter(GameObject obj)
    {
        animator = obj.gameObject.GetComponent<Animator>();
        ///animator.runtimeAnimatorController = Resources.Load("Assets/Animation/Map/Map") as RuntimeAnimatorController;

        //BatIdle
        //if (animator.GetCurrentAnimatorStateInfo(0).IsName("BatIdle"))

        if (state == 0)
        {
            //animator.Play("BatIdle");
            animator.SetBool("isHover", false);
            animator.SetBool("isVisited", false);
            animator.SetBool("isActivited", false);
        }

        //BatIdle -> BatHover
        if (state == 1)
        {
            if (animator != null)
            {
                animator.Play("BatIdle");
            }
            print("IS OVER!!!");
            //animator.SetTrigger("BatIdle");
            animator.SetBool("isHover", true);
            //animator.SetBool("isVisited", false);
            //animator.SetBool("isActivited", false);
        }
        //BatHover -> BatIdle
        if (state == 2)
        {
            //animator.SetTrigger("BatIdle");
            animator.SetBool("isHover", false);
            //animator.SetBool("isVisited", false);
            //animator.SetBool("isActivited", false);
        }
        //BatHover -> BatVisted
        if (state == 3)
        {
            //animator.SetTrigger("BatIdle");
            //animator.SetBool("isHover", false);
            animator.SetBool("isVisited", true);
            //animator.SetBool("isActivited", false);
        }
        //BatHover -> BatAcivited
        if (state == 4)
        {
            //animator.SetTrigger("BatActiveted");
            //animator.SetBool("isHover", false);
            //animator.SetBool("isVisited", true);
            animator.SetBool("isActivited", true);
        }
        //BatAcivited -> BatVisted
        if (state == 5)
        {
            //animator.SetTrigger("BatVisited");
            //animator.SetBool("isHover", false);
            animator.SetBool("isVisited", true);
            //animator.SetBool("isActivited", true);
            //animator.SetTrigger("BatVisited");
        }


    }
}
