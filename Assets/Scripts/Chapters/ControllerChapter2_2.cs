using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.UI;
using UnityEngine;

public class ControllerChapter2_2 : MonoBehaviour
{
    // Scene references
    [SerializeField] private GameObject sceneMC;
    [SerializeField] private GameObject scenePlayer;
    [SerializeField] private GameObject[] sceneObjectives;

    // Local variables
    private GameObject controllers;

    private GameObject tradeoffBattleView;
    private GameObject swingBattleView;

    private BackendAPI m_api;

    private List<(GameObject, GameObject)> tradeoffs;
    private int currentTradeOff;

    // Unity calls Awake after all active GameObjects in the Scene are initialized
    void Awake()
    {
        controllers = GameObject.Find("Controllers");

        tradeoffBattleView = GameObject.Find("TradeOff - 2.2.3/5 - Battle");
        swingBattleView = GameObject.Find("Swing - 2.2.8 Share Drinks");

        currentTradeOff = -1;
        tradeoffs = new List<(GameObject, GameObject)>();

        m_api = controllers.GetComponent<BackendAPI>();
        GameObject.Find("Controllers").GetComponent<LanguageHandler>().translateUI();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // --------------------  UI Callables  --------------------------------
    public void ClearCharacters()
    {
        foreach (GameObject character in GameObject.FindGameObjectsWithTag("Character"))
        {
            character.transform.position = Vector3.forward;
            character.SetActive(false);
        }
    }
    // View - 2.2.1/6/8 - Bargain conversation setup
    public void SetupBargainConversation()
    {
        float height = Screen.height * 0.8f / 2f;
        float depth = 1f;
        scenePlayer.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 2 / 3,
            height,
            depth));
        sceneMC.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 4,
            height,
            depth));
        scenePlayer.SetActive(true);
        sceneMC.SetActive(true);
    }

    public void SetupBargainTables()
    {
        
    }

    // TradeOff
    public void PrepareTradeOffs(GameObject objectives)
    {
        currentTradeOff = -1;
        tradeoffs.Clear();
        tradeoffs.Add((objectives.transform.GetChild(0).gameObject, objectives.transform.GetChild(1).gameObject));
        if (objectives.transform.childCount == 3)
        {
            tradeoffs.Add((objectives.transform.GetChild(1).gameObject, objectives.transform.GetChild(2).gameObject));
        }
    }

    public void NextTradeOff()
    {
        currentTradeOff++;
    }
}