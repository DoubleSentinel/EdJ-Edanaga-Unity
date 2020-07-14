using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using TMPro;

public class UITranslator : MonoBehaviour
{
    [SerializeField] private GameObject controllers;

    private BackendAPI m_api;

    private JSONNode translation;
    // Unity calls Awake after all active GameObjects in the Scene are initialized
    void Awake()
    {
        // fetch translation data
        m_api = controllers.GetComponent<BackendAPI>();
        Dictionary<string, string> filters = new Dictionary<string, string>();
        filters.Add("language", "EN");
        filters.Add("scene", "test_menu");
        m_api.ApiPull("ui", filters, ConvertJSONToObject);
    }

    private void ConvertJSONToObject(string json)
    {
        translation = JSON.Parse(json);
        TranslateUI();
    }

    private void TranslateUI()
    {
        //TODO: YOU fookin retard, why do you think with your ass?
        foreach (GameObject translatableObject in GameObject.FindGameObjectsWithTag("Translatable"))
        {
            foreach (JSONNode translatedUIElement in translation["data"][0]["elements"])
            {
                try
                {
                    Transform label = translatableObject.transform.GetChild(0);
                    label.gameObject.GetComponent<TextMeshProUGUI>().SetText(translatedUIElement["text_value"]);
                }
                catch (UnityException e)
                {
                    translatableObject.GetComponent<TextMeshProUGUI>().SetText(translatedUIElement["text_value"]);
                }
            }
        }
    }
}
