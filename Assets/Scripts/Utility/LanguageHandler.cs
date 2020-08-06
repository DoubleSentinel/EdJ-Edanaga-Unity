using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LanguageHandler : MonoBehaviour
{
    private string currentLanguage;

    [HideInInspector]
    public UITranslator m_translator;

    [DllImport("__Internal")]
    private static extern string getLanguage();
    private void Awake()
    {
        try
        {
            currentLanguage = getLanguage();
        } catch (EntryPointNotFoundException)
        {
            currentLanguage = "EN";
        }
    }

    public void translateUI()
    {
        m_translator.FetchTranslation(currentLanguage, SceneManager.GetActiveScene().name);
    }
}
