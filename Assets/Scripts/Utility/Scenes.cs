using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class Scenes
{
    private static Dictionary<string, object> parameters;

    public static void Load(string sceneName, Dictionary<string, object> parameters = null)
    {
        Scenes.parameters = parameters;
        SceneManager.LoadScene(sceneName);
    }

    public static void Load(string sceneName, string paramKey, object paramValue)
    {
        parameters = new Dictionary<string, object>
        {
            { paramKey, paramValue }
        };
        SceneManager.LoadScene(sceneName);
    }

    public static void LoadIndex(int index, Dictionary<string, object> parameters = null)
    {
        Scenes.parameters = parameters;
        SceneManager.LoadScene(index);
    }

    public static void LoadIndex(int index, string paramKey, object paramValue)
    {
        parameters = new Dictionary<string, object>();
        parameters.Add(paramKey, paramValue);
        SceneManager.LoadScene(index);
    }

    public static Dictionary<string, object> getSceneParameters()
    {
        return parameters;
    }

    public static object getParam(string paramKey)
    {
        if (parameters == null) return "";
        return parameters[paramKey];
    }

    public static void setParam(string paramKey, object paramValue)
    {
        if (parameters == null)
            parameters = new Dictionary<string, object>();
        parameters.Add(paramKey, paramValue);
    }

}

