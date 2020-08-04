using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class BackendAPI : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern string getAPIHost();

    public string BASEAPIURL;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (string.IsNullOrEmpty(BASEAPIURL))
        {
            BASEAPIURL = getAPIHost();
        }
    }

    public void ApiPull(string endpoint, Dictionary<string, string> filters, Action<string> callbackSuccess)
    {
        string uri = BASEAPIURL + "/" + endpoint + "/?";
        foreach (KeyValuePair<string, string> filter in filters)
        {
            uri += filter.Key + "=" + filter.Value + "&";
        }

        StartCoroutine(GetRequest(uri, callbackSuccess));
    }

    public void ApiPost(string endpoint, Dictionary<string, string> formFields, Action<string> callbackSuccess)
    {
        string url = BASEAPIURL + "/" + endpoint;
        WWWForm form = new WWWForm();

        foreach (KeyValuePair<string, string> field in formFields)
        {
            form.AddField(field.Key, field.Value);
        }

        StartCoroutine(PostRequest(url, form, callbackSuccess));
    }

    private IEnumerator PostRequest(string url, WWWForm form, Action<string> callbackSuccess = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, form))
        {
            yield return webRequest.SendWebRequest();
            
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                // failed to access api error handling
                print(url);
                print(webRequest.error);
            }
            else
            {
                callbackSuccess?.Invoke(webRequest.downloadHandler.text);
            }
        }
    }

    private IEnumerator GetRequest(string uri, Action<string> callbackSuccess = null)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                // failed to access api error handling
                print(uri);
                print(webRequest.error);
            }
            else
            {
                callbackSuccess?.Invoke(webRequest.downloadHandler.text);
            }
        }
    }
}