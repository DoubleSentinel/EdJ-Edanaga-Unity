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
    private string m_currentLanguageID;

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
                m_currentLanguageID = languages[getLanguage()];
            }
            catch (EntryPointNotFoundException)
            {
                m_currentLanguageID = languages["EN"];
            }
        }, null);
    }

    public void translateUI()
    {
        translator.FetchTranslation(m_currentLanguageID, SceneManager.GetActiveScene().name);
    }

    public string GetCurrentLanguage()
    {
        return languages.FirstOrDefault(lang => lang.Value == m_currentLanguageID).Key;
    }
}