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
    private Button submitPasswordChangeButton;

    private Dictionary<string, object> api_params;
    
    private bool isUsernameValid;
    private bool doesUserExist;
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
        
        api_params = new Dictionary<string, object>();
        
        confirmButton = GameObject.Find("btnConfirmCreate").GetComponent<Button>();
        submitPasswordChangeButton = GameObject.Find("btnConfirmChange").GetComponent<Button>();
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
        submitPasswordChangeButton.interactable = doesUserExist && password == passwordConfirmation;
    }

    // Checking usernames for availability on creation or existence on login
    public void CheckUsername(GameObject caller)
    {
        api_params.Clear();
        api_params.Add("username", username);
        m_api.ApiList("is_user", response =>
        {
            JSONNode nodeResponse = JSON.Parse(response);
            doesUserExist = DoesUsernameExist(nodeResponse);
            ChangeColor(caller, doesUserExist? Color.green: Color.red);
            loginButton.interactable = doesUserExist;
            userId = doesUserExist ? (string) nodeResponse["data"][0]["id"] : "";
        }, api_params);
    }
    public void CheckUsernameAvailability(GameObject caller)
    {
        api_params.Clear();
        api_params.Add("username", username);
        m_api.ApiList("is_user", response =>
        {
            JSONNode nodeResponse = JSON.Parse(response);
            isUsernameValid = !DoesUsernameExist(nodeResponse);
            ChangeColor(caller, isUsernameValid? Color.green: Color.red);
        }, api_params);
    }
    public void Login()
    {
        HashPassword();
        api_params.Clear();
        api_params.Add("username", username);
        api_params.Add("userpass", password);
        m_api.ApiPost("login_user", api_params, response =>
        {
            JSONNode nodeResponse = JSON.Parse(response);
            if (nodeResponse["error"]["code"] == 200 && nodeResponse["error"]["message"] == "ok")
            {
                SceneManager.LoadScene("Chapter2.1");
            }
            else
            {
                ChangeColor(GameObject.Find("inptPassword"),Color.red);
            }
        });
    }
    
    public void CreateAccount()
    {
        passwordConfirmation = "";
        HashPassword();
        api_params.Clear();
        api_params.Add("language_preference", GetComponent<LanguageHandler>().GetCurrentLanguage());
        api_params.Add("username", username);
        api_params.Add("userpass", password);
        m_api.ApiPost("crud_user", api_params, null); 
    }
    
    public void ChangePassword()
    {
        passwordConfirmation = "";
        HashPassword();
        api_params.Clear();
        api_params.Add("userpass", password);
        m_api.ApiPut("crud_user/" + userId, api_params, null); 
    }
}
