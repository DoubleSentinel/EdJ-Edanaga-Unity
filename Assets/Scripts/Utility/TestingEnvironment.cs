using System;
using System.Collections;
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

    public string GetValue(string parameterName)
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
                return worst.ToString();
            case "best":
                return best.ToString();
            case "global_weight":
                return global_weight.ToString();
            default:
                throw new Exception($"Parameter {parameterName} is not a valid parameter.");
        }
    }
}

public class TestingEnvironment : MonoBehaviour
{
    public Dictionary<string, Objective> Objectives { get; set; }
    public Dictionary<string, object[]> TradeOffResults { get; set; }
}
