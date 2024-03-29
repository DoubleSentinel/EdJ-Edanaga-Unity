﻿using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using TMPro;

public class UITranslator : MonoBehaviour
{
    [SerializeField] private GameObject controllers;

    private BackendAPI m_api;

    private Dictionary<string, List<GameObject>> referenceMap;
    
    // Unity calls Awake after all active GameObjects in the Scene are initialized
    void Awake()
    {
        referenceMap = new Dictionary<string, List<GameObject>>();
        if (controllers == null)
        {
            controllers = GameObject.Find("Controllers");
        }
        m_api = controllers.GetComponent<BackendAPI>();
        controllers.GetComponent<LanguageHandler>().UiTranslator = this;
    }

    private void UpdateUITranslationReferences()
    {
        referenceMap.Clear();
        foreach (GameObject translatableObject in GameObject.FindGameObjectsWithTag("Translatable"))
        {
            if (referenceMap.ContainsKey(translatableObject.name))
            {
                referenceMap[translatableObject.name].Add(translatableObject);
            }
            else
            {
                referenceMap.Add(translatableObject.name, new List<GameObject> {translatableObject});
            }
        }
    }

    public void FetchTranslation(string language, string scene)
    {
        UpdateUITranslationReferences();
        m_api.parameters.Clear();
        m_api.parameters.Add("language", language);
        m_api.parameters.Add("scene", scene);
        m_api.ApiList("ui", TranslateUI, m_api.parameters);
    }
    
    private void TranslateUI(string json)
    {
        JSONNode translation = JSON.Parse(json);
        foreach (JSONNode element in translation["data"][0]["elements"])
        {
            try
            {
                foreach (GameObject go in referenceMap[element["gameobject_id"]])
                {
                    try
                    {
                        // edge case where the label is a child of the element
                        var label = go.transform.GetChild(0);
                        var labelTMP = label.gameObject.GetComponent<TextMeshProUGUI>();
                        if (labelTMP != null)
                            labelTMP.SetText(element["text_value"]);
                        else
                            go.GetComponent<TextMeshProUGUI>().SetText(element["text_value"]);
                    }
                    catch (NullReferenceException)
                    {
                        go.GetComponent<TextMeshProUGUI>().SetText(element["text_value"]);
                    }
                    catch (UnityException)
                    {
                        go.GetComponent<TextMeshProUGUI>().SetText(element["text_value"]);
                    }
                }
            }
            catch (KeyNotFoundException e)
            {
                print(e.Message);
                print("The scene doesn't contain: " + element["gameobject_id"]);
            }
        }
    }
}
