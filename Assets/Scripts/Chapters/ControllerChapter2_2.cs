using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.UI;
using UnityEngine;

public class ControllerChapter2_2 : MonoBehaviour
{
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
    
    // Button Callables
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
