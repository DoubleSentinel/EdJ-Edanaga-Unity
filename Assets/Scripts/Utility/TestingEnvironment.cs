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
        AlternativesUninformed = new List<string> { "Alternative0", "Alternative1", "Alternative2", "Alternative3", "Alternative4", "Alternative5" };
        AlternativesMCDA = new List<string> { "Alternative1", "Alternative0", "Alternative2", "Alternative3", "Alternative4", "Alternative5" };
        AlternativesInformed = new List<string> { "Alternative1", "Alternative0", "Alternative2", "Alternative3", "Alternative4", "Alternative5" };
    }

    public Dictionary<string, Objective> Objectives { get; set; }
    public Dictionary<string, float> TradeOffClassification { get; set; }
    public Dictionary<string, float> SwingClassification { get; set; }
    public List<string> AlternativesUninformed { get; set; }
    public List<string> AlternativesMCDA { get; set; }
    public List<string> AlternativesInformed { get; set; }

    public bool SkipSwing = false;
    public bool SkipTradeOff = false;
    public string SceneCallback = "Chapter3";

    public bool ConsistentFirst = true;
    public string UserPreference = "";
}
