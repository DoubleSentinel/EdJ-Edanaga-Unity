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
        return $"name: {this.name}, description: {this.description}, unit: {this.unit}, worst: {this.worst}, best: {this.best}, global: {this.global_weight}";
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
        TradeOffClassification = new Dictionary<string, float>();
        SwingClassification = new Dictionary<string, float>();

        isInnerLoopFirstRun = true;
        SkipSwing = false;
        SkipTradeOff = false;
        SceneCallback = "Chapter2.3";
        
        //Testing purpose!
        
        AlternativesUninformed = new int[6] { 0, 1, 2, 3, 4, 5 };
        AlternativesMCDA = new int[6] { 1, 0, 2, 3, 4, 5 };
        AlternativesInformed = new int[6] { 1, 0, 2, 3, 4, 5 };
        
    }

    public Dictionary<string, Objective> Objectives { get; set; }
    public Dictionary<string, float> TradeOffClassification { get; set; }
    public Dictionary<string, float> SwingClassification { get; set; }
    public Dictionary<string, float> UsersSelectedClassification { get; set; }

    public bool SkipSwing;
    public bool SkipTradeOff;
    public string SceneCallback;

    public bool isInnerLoopFirstRun;
    public int[] AlternativesUninformed { get; set; }
    public int[] AlternativesMCDA { get; set; }
    public int[] AlternativesInformed { get; set; }

    public bool ConsistentFirst = true;
    public string UserPreference = "";
}
