using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using TMPro;
using UnityEngine.UI;

public class Authentication : MonoBehaviour
{
    private BackendAPI m_api;

    private GameObject controllers;
    
    // Unity calls Awake after all active GameObjects in the Scene are initialized
    void Awake()
    {
        if (controllers == null)
        {
            controllers = GameObject.Find("Controllers");
        }
        m_api = controllers.GetComponent<BackendAPI>();
    }

    private bool DoesUsernameExist(string username)
    {
        bool isValid = false;
        Dictionary<string, string> filters = new Dictionary<string, string>();
        filters.Add("username", username);
        m_api.ApiPull("is_user", filters, response =>
        {
            JSONNode nodeResponse = JSON.Parse(response);
            isValid = !string.IsNullOrEmpty(nodeResponse["data"][0]["username"]);
        });
        return isValid;
    }

    private void CreateUser(string username, string userpass)
    {
        //TODO: create user with uname and pass with an ApiPost call if ConfirmPassword
        // is ok and username doesn't exist
    }

    /*
     * Public methods for UI calls
     */
    public void CheckUsername(GameObject caller)
    {
        string inUsername = caller.transform.GetChild(0).GetComponentsInChildren<TextMeshProUGUI>()[1].text;
        ColorBlock block = ColorBlock.defaultColorBlock;
        block.normalColor = DoesUsernameExist(inUsername)? Color.green: Color.red;
        caller.GetComponent<TMP_InputField>().colors = block;
    }

    public void ConfirmPassword(GameObject caller)
    {
        //TODO: confirm password input field checks that password input field texts are the same
        //if they don't match set both fields normalColor to red
    }

}
