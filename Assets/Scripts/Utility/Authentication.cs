using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using SimpleJSON;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Authentication : MonoBehaviour
{
    private BackendAPI m_api;

    private GameObject controllers;

    private Button confirmButton;
    private Button loginButton;

    private bool isUsernameValid;
    private string userId;
    private string username;
    private string password;
    private string passwordConfirmation;
    
    // Unity calls Awake after all active GameObjects in the Scene are initialized
    void Awake()
    {
        if (controllers == null)
        {
            controllers = GameObject.Find("Controllers");
        }
        m_api = controllers.GetComponent<BackendAPI>();
        confirmButton = GameObject.Find("btnConfirmCreate").GetComponent<Button>();
        loginButton = GameObject.Find("btnLogin").GetComponent<Button>();
    }

    private bool DoesUsernameExist(JSONNode apiResponse)
    {
        return apiResponse["data"].Count == 1;
    }
    
    private bool IsPasswordValid()
    {
        return password.Length >= 6;
    }
    
    private void ChangeColor (GameObject inputTextField, Color color)
    {
        inputTextField.GetComponent<Image>().color = color;
    }

    private void HashPassword()
    {
        using (SHA256 sha256 = new SHA256Managed())
        {
            byte[] hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            StringBuilder hex = new StringBuilder(hash.Length*2);
            foreach (byte bit in hash)
            {
                hex.AppendFormat("{0:x2}", bit);
            }
            password = hex.ToString();
        }
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
        ChangeColor(caller, IsPasswordValid()? Color.green: Color.red);
    }

    public void UpdatePasswordConfirmation(GameObject caller)
    {
        passwordConfirmation = caller.GetComponent<TMP_InputField>().text;
        ChangeColor(caller, password == passwordConfirmation? Color.green: Color.red );
        confirmButton.interactable = isUsernameValid && password == passwordConfirmation;
    }

    // Checking usernames for availability on creation or existence on login
    public void CheckUsername(GameObject caller)
    {
        Dictionary<string, string> filters = new Dictionary<string, string>();
        filters.Add("username", username);
        m_api.ApiList("is_user", filters, response =>
        {
            JSONNode nodeResponse = JSON.Parse(response);
            bool doesUserExist = DoesUsernameExist(nodeResponse);
            ChangeColor(caller, doesUserExist? Color.green: Color.red);
            loginButton.interactable = doesUserExist;
            userId = doesUserExist ? (string) nodeResponse["data"][0]["id"] : "";
        });
    }
    public void CheckUsernameAvailability(GameObject caller)
    {
        Dictionary<string, string> filters = new Dictionary<string, string>();
        filters.Add("username", username);
        m_api.ApiList("is_user", filters, response =>
        {
            JSONNode nodeResponse = JSON.Parse(response);
            isUsernameValid = !DoesUsernameExist(nodeResponse);
            ChangeColor(caller, isUsernameValid? Color.green: Color.red);
        });
    }
    public void Login()
    {
        HashPassword();
        m_api.ApiFetch("create_auth_user", userId, response =>
        {
            JSONNode nodeResponse = JSON.Parse(response);
            if (password == nodeResponse["userpass"])
            {
                print("login success");
                //SceneManager.LoadScene("Chapter2.1");
            }
            else
            {
                print("login failed");
                ChangeColor(GameObject.Find("inptPassword"),Color.red);
            }
        });
    }
    
    public void CreateAccount()
    {
        passwordConfirmation = "";
        HashPassword();
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        // TODO get the language preference from the LanguageHandler
        parameters.Add("language_preference", "EN");
        parameters.Add("username", username);
        parameters.Add("userpass", password);
        m_api.ApiPost("create_auth_user", parameters, response =>
        {
            //Login();
        }); 
    }
}
