using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using SimpleJSON;

public class LanguageHandler : MonoBehaviour
{
    [SerializeField]
    private string defaultLanguage = "EN";
    
    private string m_currentLanguageId;

    private Dictionary<string, string> languages;

    [HideInInspector]
    public UITranslator translator;

    [DllImport("__Internal")]
    private static extern string getLanguage();

    private void Awake()
    {
        languages = new Dictionary<string, string>();
        GetComponent<BackendAPI>().ApiList("languages", response =>
        {
            JSONNode nodeResponse = JSON.Parse(response);
            foreach (JSONNode language in nodeResponse["data"])
            {
                languages.Add(language["name"], language["id"]);
            }
            try
            {
                m_currentLanguageId = languages[getLanguage()];
            }
            catch (EntryPointNotFoundException)
            {
                m_currentLanguageId = languages[defaultLanguage];
            }
            translateUI();
        }, null);
    }

    public void translateUI()
    {
        translator.FetchTranslation(m_currentLanguageId, SceneManager.GetActiveScene().name);
    }

    public string GetCurrentLanguage()
    {
        return languages.FirstOrDefault(lang => lang.Value == m_currentLanguageId).Key;
    }
}