using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using TMPro;

public class UITranslator : MonoBehaviour
{
    [SerializeField] private GameObject controllers;

    private BackendAPI m_api;

    private Dictionary<string, GameObject> referenceMap;
    
    // Unity calls Awake after all active GameObjects in the Scene are initialized
    void Awake()
    {
        referenceMap = new Dictionary<string, GameObject>();
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
            referenceMap.Add(translatableObject.name, translatableObject);
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
                // edge case where the label is a child of the element
                Transform label = referenceMap[element["gameobject_id"]].transform.GetChild(0);
                label.gameObject.GetComponent<TextMeshProUGUI>().SetText(element["text_value"]);
            }
            catch (UnityException)
            {
                referenceMap[element["gameobject_id"]].GetComponent<TextMeshProUGUI>().SetText(element["text_value"]);
            }
        }
    }
}
