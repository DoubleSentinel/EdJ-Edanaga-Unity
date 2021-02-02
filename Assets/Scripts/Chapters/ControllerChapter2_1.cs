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
    [SerializeField] private Camera cam;
    [SerializeField] private Animator animator;
    private int state;

    [SerializeField] private GameObject scenePlayer;
    [SerializeField] private GameObject[] sceneObjectives;
    [SerializeField] private GameObject[] sceneBuildings;
    [SerializeField] private GameObject[] sceneButtons;
    [SerializeField] private GameObject sceneObjective;
    [SerializeField] private Button btnContinue;

    private int notifications;
    private int userSelectedObjectiveNumber = 0;
    [Header("Conversation References")]
    [SerializeField] private GameObject[] ConversationBubbles;

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
        conversationCallback = () => 
        {
            //Change state of animator
            animator.SetBool("isVisited", true);
            GameEventMessage.SendEvent("ContinueToTown");
            StartCoroutine(UpdateObjectiveButton());
            //Allow the buildings selection after the end of the conversation
            AllowBuildingsSelection(true);
        };
    }

    private void Start()
    {
        controllers.GetComponent<LanguageHandler>().translateUI();
    }

    //Button continue appears when all the objectives have been read
    IEnumerator UpdateObjectiveButton()
    {
        notifications = 0;

        yield return new WaitForSeconds(2); //Wait 2s

        for (int i = 0; i < sceneBuildings.Length; i++)
        {
            //Check state of animator
            if (sceneBuildings[i].gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("BatVisited"))
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

    private void SetupCallConversation()
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

    private void ClearCharacters()
    {
        foreach (GameObject character in GameObject.FindGameObjectsWithTag("Character"))
        {
            character.transform.position = new Vector3(11, 0, 1);
            character.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
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

    //Set buttons position to the buildings position
    private void SetupButtons()
    {
        //Set buttons position to the buildings postion
        for (int i = 0; i < sceneBuildings.Length; i++)
        {
            sceneButtons[i].gameObject.transform.position = cam.WorldToScreenPoint(sceneBuildings[i].transform.position);
        }
    }

    //Block the other buildings selection when one during the conversation
    private void AllowBuildingsSelection(bool actionEnable)
    {
        for (int i = 0; i < sceneBuildings.Length; i++)
        {
            if (actionEnable)
            {
                //Allow the selection of all the buildings
                if (sceneButtons[i].gameObject.GetComponent<UIButton>().Interactable == false)
                    sceneButtons[i].gameObject.GetComponent<UIButton>().Interactable = true;
            }
            else
            {
                //Don't allow buildings selection when one building is already selected 
                if (i != userSelectedObjectiveNumber)
                    sceneButtons[i].gameObject.GetComponent<UIButton>().Interactable = false;
            }
        }
    }

    //Get objective number from building
    private int GetObjectiveNumer(GameObject building)
    {
        for (int i = 0; i < sceneBuildings.Length; i++)
        {
            if (building == sceneBuildings[i])
            {
                return userSelectedObjectiveNumber = i;
            }
        }
        return userSelectedObjectiveNumber;
    }

    // --------------------  UI Callables  --------------------------------

    public void StartCall(GameObject building)
    {
        GameEventMessage.SendEvent("GoToPhoneCall");
        userSelectedObjectiveNumber = GetObjectiveNumer(building);
        sceneObjective = sceneObjectives[userSelectedObjectiveNumber];
        SetupCallConversation();
        Call(userSelectedObjectiveNumber);
    }

    // set state of the animation
    public void SetAnimatorState(int stateButton)
    {
        state = stateButton;
    }

    //Set the correct animation of the gameobject
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

        //State: BuildingIdle -> BuildingHover
        if (state == 1)
        {
            animator.SetBool("isHover", true);
        }

        //State: BuildingHover -> BuildingIdle
        if (state == 2)
        {
            animator.SetBool("isHover", false);
        }

        //State:  BuildingHover -> BuildingVisted : Not used yet
        if (state == 3)
        {
            //Add action if necessary 
        }

        //State:  BuildingHover -> BuildingAcivited
        if (state == 4)
        {
            //Don't allow the others buildings selection when one building is already selected
            AllowBuildingsSelection(false);
            animator.Play("BatActivated");
        }

        //State: BuildingAcivited -> BuildingVisted
        if (state == 5)
        {
            animator.Play("BatVisited");
        }
    }

    //Play specific UI sound
    public void PlaySoundUI(string mySoundName)
    {
        controllers.GetComponent<SoundsController>().PlaySoundUI(mySoundName);
    }


    //Play specific Ambiance sound
    public void PlaySoundAmbiance(string mySoundName)
    {
        controllers.GetComponent<SoundsController>().PlaySoundAmbiance(mySoundName);
    }

    //Play specific Music sound
    public void PlaySoundMusic(string mySoundName)
    {
        controllers.GetComponent<SoundsController>().PlaySoundMusic(mySoundName);
    }
}
