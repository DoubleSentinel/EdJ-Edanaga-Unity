using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BackendAPI : MonoBehaviour 
{
    public static string BASEAPIURL = "localhost:5001";

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void ApiPull(string endpoint, Dictionary<string, string> filters, Action<string> callbackSuccess)
    {
        string uri = BASEAPIURL + "/" + endpoint + "/?";
        foreach(KeyValuePair<string, string> filter in filters)
        {
            uri+=filter.Key + "=" + filter.Value + "&";  
        }
        StartCoroutine(GetRequest(uri, callbackSuccess));
    }

    IEnumerator GetRequest(string uri, Action<string> callbackSuccess)
    {
        using(UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if(webRequest.isNetworkError || webRequest.isHttpError)
            {
                // failed to access api error handling
                yield break;
            } else {
                callbackSuccess(webRequest.downloadHandler.text);
                yield break;
            }
        }
    }

}
