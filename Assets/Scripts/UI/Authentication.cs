using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using SimpleJSON;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Doozy.Engine;

public class Authentication : MonoBehaviour
{
    private BackendAPI m_api;

    [DllImport("__Internal")]
    private static extern string getToken();

    private GameObject controllers;

    private Button confirmButton;
    private Button loginButton;

    private bool isUsernameValid;
    private bool doesUserExist;
    private string tokenId;
    private string userId;
    private string username;
    private string password;
    private string passwordConfirmation;

    // Unity calls Awake after all active GameObjects in the Scene are initialized
    void Awake()
    {
        controllers = GameObject.Find("Controllers");

        m_api = controllers.GetComponent<BackendAPI>();

        confirmButton = GameObject.Find("btnConfirmCreate").GetComponent<Button>();
        loginButton = GameObject.Find("btnLogin").GetComponent<Button>();
    }

    private void Start()
    {
        GetComponent<LanguageHandler>().translateUI();
    }

    private bool DoesUsernameExist(JSONNode apiResponse)
    {
        return apiResponse["data"].Count == 1;
    }

    private bool IsPasswordValid()
    {
        return password.Length >= 6;
    }

    private void ChangeColor(GameObject inputTextField, Color color)
    {
        inputTextField.GetComponent<Image>().color = color;
    }

    private void HashPassword()
    {
        using (SHA256 sha256 = new SHA256Managed())
        {
            byte[] hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            StringBuilder hex = new StringBuilder(hash.Length * 2);
            foreach (byte bit in hash)
            {
                hex.AppendFormat("{0:x2}", bit);
            }

            password = hex.ToString();
        }
    }

    private void LinkToToken()
    {
        string token = "";
        try
        {
            token = getToken();
        }
        catch (Exception)
        {
            token = "testToken";
        }
        m_api.parameters.Clear();
        m_api.parameters.Add("token_url", token);
        m_api.parameters.Add("user_id", userId);
        m_api.ApiPost("update_invite", m_api.parameters, response =>
        {
            JSONNode nodeResponse = JSON.Parse(response);
            if (nodeResponse["error"]["code"] == 200 && nodeResponse["error"]["message"] == "ok")
            {
                tokenId = nodeResponse["error"]["id"];
            }
        });
    }

    /*
     * Public methods for UI calls
     */
    // Value updates
    public void UpdateUsername(GameObject caller)
    {
        username = caller.GetComponent<TMP_InputField>().text;
    }

    public void UpdatePassword(GameObject caller)
    {
        password = caller.GetComponent<TMP_InputField>().text;
    }

    public void UpdatePasswordValidation(GameObject caller)
    {
        password = caller.GetComponent<TMP_InputField>().text;
        ChangeColor(caller, IsPasswordValid() ? Color.green : Color.red);
    }

    public void UpdatePasswordConfirmation(GameObject caller)
    {
        passwordConfirmation = caller.GetComponent<TMP_InputField>().text;
        ChangeColor(caller, password == passwordConfirmation ? Color.green : Color.red);
        confirmButton.interactable = isUsernameValid && password == passwordConfirmation;
    }

    // Checking usernames for availability on login
    public void CheckUsername(GameObject caller)
    {
        m_api.parameters.Clear();
        m_api.parameters.Add("username", username);
        m_api.ApiList("is_user", response =>
        {
            JSONNode nodeResponse = JSON.Parse(response);
            doesUserExist = DoesUsernameExist(nodeResponse);
            ChangeColor(caller, doesUserExist ? Color.green : Color.red);
            loginButton.interactable = doesUserExist;
            userId = doesUserExist ? (string) nodeResponse["data"][0]["id"] : "";
        }, m_api.parameters);
    }

    //Checking usernames for availability on creation 
    public void CheckUsernameAvailability(GameObject caller)
    {
        m_api.parameters.Clear();
        m_api.parameters.Add("username", username);
        m_api.ApiList("is_user", response =>
        {
            JSONNode nodeResponse = JSON.Parse(response);
            isUsernameValid = !DoesUsernameExist(nodeResponse);
            ChangeColor(caller, isUsernameValid ? Color.green : Color.red);
        }, m_api.parameters);
    }

    public void Login(string sceneToLoad)
    {
        HashPassword();
        m_api.parameters.Clear();
        m_api.parameters.Add("username", username);
        m_api.parameters.Add("userpass", password);
        m_api.ApiPost("login_user", m_api.parameters, response =>
        {
            JSONNode nodeResponse = JSON.Parse(response);
            if (nodeResponse["error"]["code"] == 200 && nodeResponse["error"]["message"] == "ok")
            {
                Dictionary<string, Objective> objectives = new Dictionary<string, Objective>();
                foreach (JSONNode constant in nodeResponse["error"]["constants"])
                {
                    Objective objectified = Objective.CreateFromJSON(constant.ToString());
                    objectives.Add(objectified.name, objectified);
                }

                GetComponent<TestingEnvironment>().Objectives = objectives;
                SceneManager.LoadScene(sceneToLoad);
            }
            else
            {
                ChangeColor(GameObject.Find("inptPassword"), Color.red);
            }
        });
    }

    public void CreateAccount(string sceneToLoad)
    {
        passwordConfirmation = "";
        HashPassword();
        m_api.parameters.Clear();
        m_api.parameters.Add("language_preference", GetComponent<LanguageHandler>().GetCurrentLanguage());
        m_api.parameters.Add("username", username);
        m_api.parameters.Add("userpass", password);
        m_api.ApiPost("crud_user", m_api.parameters, (response) =>
        {
            JSONNode nodeResponse = JSON.Parse(response);
            userId = nodeResponse["id"];
            LinkToToken();
            Login(sceneToLoad);
        });
    }

    // Could be useful some other time, currently unused
    public void ChangePassword()
    {
        passwordConfirmation = "";
        HashPassword();
        m_api.parameters.Clear();
        m_api.parameters.Add("userpass", password);
        m_api.ApiPut("crud_user/" + userId, m_api.parameters, null);
    }
}