﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageHandler : MonoBehaviour
{
    private string currentLanguage;

    private UITranslator m_UItranslator;
    //private ConversationTranslator m_ConTranslator;

    private void Awake()
    {
        m_UItranslator = GameObject.Find("MasterCanvas").GetComponent<UITranslator>();
    }

    public void ChangeLanguage(string language)
    {
        currentLanguage = language;
        m_UItranslator.FetchTranslation(currentLanguage, "test_menu");
    }

}
