using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Objective
{
    public string name;
    public string description;
    public string unit;
    public float worst;
    public float best;
    public float global_weight;

    public static Objective CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Objective>(jsonString);
    }

    public override string ToString()
    {
        return
            $"name: {this.name}, description: {this.description}, unit: {this.unit}, worst: {this.worst}, best: {this.best}, global: {this.global_weight}";
    }

    public object GetValue(string parameterName)
    {
        switch (parameterName)
        {
            case "name":
                return name;
            case "desc":
                return description;
            case "unit":
                return unit;
            case "worst":
                return worst;
            case "best":
                return best;
            case "global_weight":
                return global_weight;
            default:
                throw new Exception($"Parameter {parameterName} is not a valid parameter.");
        }
    }
}

public class TestingEnvironment : MonoBehaviour
{
    private void Awake()
    {
        TradeOffClassification = new Dictionary<string, double>();
        SwingClassification = new Dictionary<string, double>();
        Characters = new Dictionary<string, string>();

        isInnerLoopFirstRun = true;
        SkipSwing = false;
        SkipTradeOff = false;
        ConsistentFirst = true;
        SceneCallback = "Chapter2.3";
        UserPreference = "";
        AlternativesUninformed = new int[6] { 0, 1, 2, 3, 4, 5 };
        AlternativesMCDA = new int[6] { 0, 1, 2, 3, 4, 5 };
        AlternativesInformed = new int[6] { 0, 1, 2, 3, 4, 5 };
    }

    // Objective Definitions
    public Dictionary<string, Objective> Objectives { get; set; }
    // TradeOff and Swing results
    public Dictionary<string, double> TradeOffClassification { get; set; }
    public Dictionary<string, double> SwingClassification { get; set; }
    public Dictionary<string, double> UsersSelectedClassification { get; set; }

    public Dictionary<string, string> Characters { get; set; }

    // Alternative lists by category
    public int[] AlternativesUninformed { get; set; }
    public int[] AlternativesMCDA { get; set; }
    public int[] AlternativesInformed { get; set; }
    
    // Testing flags
    [HideInInspector]public bool SkipSwing;
    [HideInInspector]public bool SkipTradeOff;
    [HideInInspector]public bool isInnerLoopFirstRun;
    [HideInInspector]public bool ConsistentFirst;
    
    [HideInInspector]public string UserPreference = "";
    [HideInInspector]public string SceneCallback;
}