using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Engine;
using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ControllerChapter2_2 : MonoBehaviour
{
    [Header("2D Scene References")] [SerializeField]
    private GameObject sceneHost;

    [SerializeField] private GameObject scenePlayer;
    [SerializeField] private GameObject[] sceneFamilies;

    [Header("Conversation References")] 
    public GameObject HostConversationBubble;
    public GameObject GroupedConversationBubble;
    public GameObject ConversationGroup;

    // Local variables
    private GameObject controllers;
    
    // Flags
    public bool isTradeOff = true;

    // BargainConversation vars
    [HideInInspector] public int hostConversationIndex = 0;
    [HideInInspector] public int groupedConversationIndex = 0;
    public ConversationHandler.ConversationEnd hostConversationCallback;
    public ConversationHandler.ConversationEnd groupedConversationCallback;

    // Unity calls Awake after all active GameObjects in the Scene are initialized
    void Awake()
    {
        controllers = GameObject.Find("Controllers");
        hostConversationCallback = () => { GameEventMessage.SendEvent("GoToTables"); };
    }

    private void Start()
    {
        controllers.GetComponent<LanguageHandler>().translateUI();
    }

    private int frames = 0;

    private void Update()
    {
        if (frames != 30)
        {
            GameEventMessage.SendEvent(controllers.GetComponent<TestingEnvironment>().SkipTradeOff
                ? "StartChapter4"
                : "StartChapter3");
            frames++;
        }
    }

    // --------------------  UI Callables  --------------------------------
    public void HideBackground(GameObject background)
    {
        background.SetActive(false);
    }
    public void ShowBackground(GameObject background)
    {
        background.SetActive(true);
    }

    public void ClearCharacters()
    {
        foreach (GameObject character in GameObject.FindGameObjectsWithTag("Character"))
        {
            character.transform.position = new Vector3(50, 0, 1);
            character.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
    }

    public void SetupHostConversation()
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

        var ch =  HostConversationBubble.GetComponent<ConversationHandler>();
        ch.callback = hostConversationCallback;
        ch.GenerateConversation(hostConversationIndex);
        ch.NextConversationSnippet();
    }

    public void SetupGroupedConversation()
    {
        float height = Screen.height * 0.75f / 2f;
        float depth = -1f;
        float step = Screen.width / 8;
        
        Vector3 host = Camera.main.ScreenToWorldPoint(new Vector3(2*step, height));
        sceneHost.transform.position = new Vector3(host.x, host.y, depth);
        sceneHost.SetActive(true);
        
        for (int i = 0; i < ConversationGroup.transform.childCount; i++)
        {
            var go = ConversationGroup.transform.GetChild(i).gameObject;
            Vector3 uiPos = Camera.main.ScreenToWorldPoint(new Vector3((4 + i)*step, height));
            go.transform.position = new Vector3(uiPos.x, uiPos.y, depth);
            go.SetActive(true);
        }

        var ch =  GroupedConversationBubble.GetComponent<ConversationHandler>();
        ch.callback = groupedConversationCallback;
        ch.GenerateConversation(groupedConversationIndex);
        ch.NextConversationSnippet();
    }
    
    public void PrepareTradeOffResultsConversation()
    {
        var results = controllers.GetComponent<TestingEnvironment>().TradeOffClassification;
        var winningFamilyMember = results.OrderByDescending(x => x.Value).First();
        var winningFamily = GameObject.Find(ConversationHandler.FirstLetterToUpper(winningFamilyMember.Key)).transform.parent;
        
        ClearCharacterConversationGroup();
        
        // Add Family members of the winning family
        foreach (Transform objective in winningFamily)
        {
            Instantiate(objective.gameObject, ConversationGroup.transform);
        }

        groupedConversationIndex = 1;
        groupedConversationCallback = () =>
        {
            ClearCharacterConversationGroup();
            isTradeOff = false;
            hostConversationIndex = 2;
            hostConversationCallback = () =>
            {
                GameEventMessage.SendEvent("GoToTables");
            };
            if(!controllers.GetComponent<TestingEnvironment>().SkipSwing)
                GameEventMessage.SendEvent("GoToTitleChapter4");
            else
            {
                EndScene();
            }
        };
    }

    public void EndScene()
    {
        SceneManager.LoadScene(controllers.GetComponent<TestingEnvironment>().SceneCallback);
    }

    public void PrepareSwingResultsConversation()
    {
        var results = controllers.GetComponent<TestingEnvironment>().TradeOffClassification;
        var winningFamilyMember = results.OrderByDescending(x => x.Value).First();
        var winningFamily = GameObject.Find(ConversationHandler.FirstLetterToUpper(winningFamilyMember.Key)).transform.parent;
        
        ClearCharacterConversationGroup();
        
        // Add Family members of the winning family
        foreach (Transform objective in winningFamily)
        {
            Instantiate(objective.gameObject, ConversationGroup.transform);
        }

        groupedConversationIndex = 1;
        groupedConversationCallback = () =>
        {
            GameEventMessage.SendEvent("GoToTitleChapter4");
            ClearCharacterConversationGroup();
            isTradeOff = false;
            hostConversationIndex = 2;
            hostConversationCallback = () =>
            {
                GameEventMessage.SendEvent("GoToTables");
            };
        };
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

    public void SetupChapter3()
    {
        hostConversationIndex = 0;
        isTradeOff = true;
    }

    public void SetupChapter4()
    {
        isTradeOff = false;
        hostConversationIndex = 2;
        HostConversationBubble.GetComponent<ConversationHandler>().callback = () => { GameEventMessage.SendEvent("GoToTables"); };
        
        GetComponent<Swing>().ValidateSwingButton.GetComponent<UIButton>().OnClick.OnTrigger.GameEvents.Clear();
        GetComponent<Swing>().ValidateSwingButton.GetComponent<UIButton>().OnClick.OnTrigger.GameEvents.Add("GoToTables");
    }

    // View - TradeOff - 2.2.3/5 - Battle
    public void StartActivityWithFamily(GameObject family)
    {
        if (isTradeOff)
        {
            GameEventMessage.SendEvent("GoToTradeOffs");
            GetComponent<TradeOff>().PrepareTradeOffs(family);
        }
        else
        {
            GetComponent<Swing>().PrepareSwingWith(family);
            GameEventMessage.SendEvent("GoToSwing");
        }
    }

    // Utility UI methods
    public void DeactivateFamilySelector(GameObject caller)
    {
        caller.GetComponent<EventTrigger>().enabled = false;
    }

    public void ReactivateFamilySelectors(GameObject DoozyUIView)
    {
        foreach (Transform familySelector in DoozyUIView.transform)
        {
            familySelector.GetChild(0).GetComponent<EventTrigger>().enabled = true;
        }
    }

    private void ClearCharacterConversationGroup()
    {
        foreach (Transform child in ConversationGroup.transform)
        {
            Destroy(child.gameObject);
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