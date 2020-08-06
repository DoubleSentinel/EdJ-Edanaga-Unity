using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using TMPro;
using Doozy.Engine;

public class UITranslator : MonoBehaviour
{
    [SerializeField] private GameObject controllers;

    private BackendAPI m_api;

    private Dictionary<string, GameObject> referenceMap;
    
    // Unity calls Awake after all active GameObjects in the Scene are initialized
    void Awake()
    {
        if (controllers == null)
        {
            controllers = GameObject.Find("Controllers");
        }
        m_api = controllers.GetComponent<BackendAPI>();
        controllers.GetComponent<LanguageHandler>().m_translator = this;
        
        // build local reference map
        referenceMap = new Dictionary<string, GameObject>();
        foreach (GameObject translatableObject in GameObject.FindGameObjectsWithTag("Translatable"))
        {
            referenceMap.Add(translatableObject.name, translatableObject);
        }
    }

    public void FetchTranslation(string language, string scene)
    {
        Dictionary<string, string> filters = new Dictionary<string, string>();
        filters.Add("language", language);
        filters.Add("scene", scene);
        m_api.ApiList("ui", filters, TranslateUI);
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
