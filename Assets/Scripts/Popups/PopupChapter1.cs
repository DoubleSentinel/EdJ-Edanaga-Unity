using Doozy.Engine.UI;
using UnityEngine;
using UnityEngine.UI;

public class PopupChapter1 : MonoBehaviour
{
    [Header("Popup Values")]
    public string PopupName = "Popup1";
    //public GameObject TitleObject;
    public string Title = "Title";
    public GameObject MessageObject;
    public string Message = "Popup message for player";

    public void ShowPopup()
    {
        //get a clone of the UIPopup, with the given PopupName, from the UIPopup Database 
        UIPopup popup = UIPopup.GetPopup(PopupName);

        //make sure that a popup clone was actually created
        if (popup == null)
            return;

        //popup.Data.SetLabelsTexts(Title, Message);
        //Title = TitleObject.GetComponent<Text>().ToString();
        //Message = MessageObject.GetComponent<Text>().ToString();
        Title = "Consignes";
        Message = "Cliquez sur l�alternative que vous pr�f�rez, maintenez le bouton de la souris press� et d�placez l�image en haut du classement, avant de rel�cher le bouton de la souris. De la m�me mani�re, glissez-d�posez votre deuxi�me alternative pr�f�r�e � la seconde place, et ainsi de suite jusqu�� obtenir le classement de votre choix !";
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