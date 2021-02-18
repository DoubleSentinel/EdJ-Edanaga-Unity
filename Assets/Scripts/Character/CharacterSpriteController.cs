using UnityEngine;

public class CharacterSpriteController : MonoBehaviour
{
    [SerializeField]
    private GameObject symbolChest;
    [SerializeField]
    private GameObject symbolComplete;
    [SerializeField]
    private GameObject fullBody;

    // Local variables
    private GameObject controllers;
    private string head_to_show;

    private void Awake()
    {
        controllers = GameObject.Find("Controllers");

        // by default show the character
        symbolChest.SetActive(true); 
        symbolComplete.SetActive(false);
        fullBody.SetActive(true);
               
        head_to_show = controllers.GetComponent<TestingEnvironment>().Characters[this.name];
        
        foreach (GameObject child in this.gameObject.transform.GetChild(1).GetChild(2)) //Character Heads
		{
		    child.gameObject.SetActive(child.name != head_to_show);
		}
    }

    public void MaximizeSymbol(bool isMaximized)
    {
        symbolChest.SetActive(!isMaximized); 
        symbolComplete.SetActive(isMaximized);
        fullBody.SetActive(!isMaximized);
    }
}
