using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TownViewController : MonoBehaviour
{
    [SerializeField]
    private Button btnContinue;

    private int notifications;

    void Awake()
    {
        if (btnContinue == null)
        {
            btnContinue = GameObject.Find("btnContinue").GetComponent<Button>();
        }
    }

    public void UpdateObjectiveButton(GameObject caller)
    {
        caller.transform.GetChild(1).gameObject.SetActive(false);
        notifications = 0;
        foreach (Transform objective in transform.GetChild(0))
        {
            GameObject notification = objective.GetChild(1).gameObject;
            if (!notification.activeSelf)
            {
                notifications += 1;
                if (notifications == 10)
                {
                    btnContinue.interactable = true;
                }
            }
        }
    }
    
}
