using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.UI;

public class PopupChapter1_Test : MonoBehaviour
{
    [Header("Popup Values")]
    public string PopupName = "Popup1";
    public string Title = "Title";
    public string Message = "Popup message for player";

    public void ShowPopup()
    {
        //get a clone of the UIPopup, with the given PopupName, from the UIPopup Database 
        UIPopup popup = UIPopup.GetPopup(PopupName);

        //make sure that a popup clone was actually created
        if (popup == null)
            return;

        popup.Data.SetLabelsTexts(Title, Message);

        popup.Show(); //show the popup
    }

    /*
    private void ClosePopup()
    {
        UIPopup.HidePopup(PopupName);
    }
    */
}