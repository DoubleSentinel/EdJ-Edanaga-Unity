using Doozy.Engine;
using Doozy.Engine.UI;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ControllerChapter2_1 : MonoBehaviour
{
    [Header("2D Scene References")]
    public Camera cam;
    Animator animator;
    int state;
    bool batHover = false;

    [SerializeField] private GameObject scenePlayer;
    [SerializeField] private GameObject[] sceneObjectives;
    [SerializeField] private GameObject[] sceneBats;
    [SerializeField] private GameObject[] sceneButtons;
    [SerializeField] private GameObject sceneObjective;
    [SerializeField] private Button btnContinue;

    private int notifications;
    private int objectiveNumber = 0;
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
        conversationCallback = () => {
            //Change state of animator
            animator.SetBool("isVisited", true);
            GameEventMessage.SendEvent("ContinueToTown");
            StartCoroutine(UpdateObjectiveButton());
            //Block the others buttons action
            ControlButtons(true);
        };
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
    IEnumerator UpdateObjectiveButton()
    {
         notifications = 0;

        yield return new WaitForSeconds(2); //Wait 2s

        for (int i = 0; i < sceneBats.Length; i++)
        {
            //Check state of animator
            if (sceneBats[i].gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("BatVisited"))
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
        objectiveNumber = Convert.ToInt32($"{objective.name.Last()}");
        sceneObjective = sceneObjectives[objectiveNumber];
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

    public void ControlButtons(bool actionEnable)
    {
        //Set buttons position to the batiments postion
        for (int i = 0; i < sceneBats.Length; i++)
        {
            if (actionEnable)
            {
                if (sceneButtons[i].gameObject.GetComponent<UIButton>().Interactable == false)
                    sceneButtons[i].gameObject.GetComponent<UIButton>().Interactable = true;
            }
            else
            {
                if (i != objectiveNumber)
                    sceneButtons[i].gameObject.GetComponent<UIButton>().Interactable = false;
            }
        }
    }

    public void SetAnimatorState(int stateButton)
    {
        state = stateButton;
    }

    public void SetAnimatorParameter(GameObject obj)
    {
        animator = obj.gameObject.GetComponent<Animator>();

        //Default state
        if (state == 0)
        {
            animator.SetBool("isHover", false);
            animator.SetBool("isVisited", false);
            animator.SetBool("isActivated", false);
        }

        //State: BatIdle -> BatHover
        if (state == 1)
        {
            animator.SetBool("isHover", true);
        }

        //State: BatHover -> BatIdle
        if (state == 2)
        {
            animator.SetBool("isHover", false);
        }

        //State:  BatHover -> BatVisted : Not used yet
        if (state == 3)
        {
            //Add action if necessary 
        }

        //State:  BatHover -> BatAcivited
        if (state == 4)
        {
            //Block the others buttons action
            ControlButtons(false);
            animator.Play("BatActivated");
        }

        //State: BatAcivited -> BatVisted
        if (state == 5)
        {
            animator.Play("BatVisited");
        }
    }
}
