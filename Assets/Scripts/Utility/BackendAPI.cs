﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SimpleJSON;
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
        string url = BASEAPIURL + "/" + endpoint + "/";
        StartCoroutine(PostRequest(url, FlatDictToJSON(formFields), callbackSuccess));
    }

    private IEnumerator PostRequest(string url, string JSONbody, Action<string> callbackSuccess = null)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(JSONbody);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer(); 
            webRequest.SetRequestHeader("Content-Type", "application/json");
            
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
                print(webRequest.error);
            }
            else
            {
                callbackSuccess?.Invoke(webRequest.downloadHandler.text);
            }
        }
    }
    
    private string FlatDictToJSON(Dictionary<string,string> dict)
    {
        var entries = dict.Select(d =>
            string.Format("\"{0}\": \"{1}\"", d.Key, d.Value));
        return "{" + string.Join(",", entries) + "}";
    }
}