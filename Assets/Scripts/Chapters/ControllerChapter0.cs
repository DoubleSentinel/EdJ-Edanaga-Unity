using Doozy.Engine;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControllerChapter0 : MonoBehaviour
{
    private int selectedCharacterIndex = 0;
    
    private Color desiredColor;

    [Header("2D Scene References")]
    [SerializeField] private GameObject scenePlayerCh0;
    [SerializeField] private Transform heads;

    // Local variables
    private GameObject controllers;
    private string texts;
    private List<string> listHeads;
    private int headCount;

    void Awake()
    {
        controllers = GameObject.Find("Controllers");
        headCount = heads.childCount;
    }

    private void Start()
    {
        selectedCharacterIndex = 10; //Otarie as head default
        SetupPlayer();
    }

    public void ChangeHead(int direction)
    {
        var totalSize = headCount - 3;
        heads.GetChild(selectedCharacterIndex).gameObject.SetActive(false);
        selectedCharacterIndex = (selectedCharacterIndex + direction + totalSize) % totalSize;
        heads.GetChild(selectedCharacterIndex).gameObject.SetActive(true);
        SetupPlayer();
    }

    public void Confirm()
    {
        listHeads = new List<string>();
        controllers.GetComponent<TestingEnvironment>().Characters.Clear();

        //List of the possible heads for the Objectives
        for (int i = 0; i < headCount -3; i++)
        {
            listHeads.Add(heads.GetChild(i).gameObject.name);
        }
        
        //get currently selected head as player
        controllers.GetComponent<TestingEnvironment>().Characters.Add("Player", listHeads[selectedCharacterIndex]);
        controllers.GetComponent<TestingEnvironment>().Characters.Add("Host", "Bernard");
        controllers.GetComponent<TestingEnvironment>().Characters.Add("Journalist", "Mouse");
        controllers.GetComponent<TestingEnvironment>().Characters.Add("Engineer", "Hibou");


        for (int i = 0; i < 10; i++) //Objective0 to Objective9 
        {
            int index = Random.Range(0, listHeads.Count);
            controllers.GetComponent<TestingEnvironment>().Characters.Add($"Objective{i}", listHeads[index]);
            listHeads.RemoveAt(index); //Adapt listHeads
        }
    }

    public void SetupPlayer()
    {
        float height = Screen.height * 0.75f / 2f;
        float depth = -1f;
        Vector3 player = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 4, height*1.2f));

        scenePlayerCh0.transform.position = new Vector3(player.x, player.y, depth);
        scenePlayerCh0.SetActive(true);
    }

    //Prepare texts for the detailled description of the alternative
    public void PrepareTextAlternative(GameObject TextsBox)
    {
        //Get texts from TextBox
        texts = "";
        for (int i = 0; i < TextsBox.transform.childCount; i++)
        {
            texts += TextsBox.gameObject.transform.GetChild(i).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text;
            texts += "\n\n"; //new line characters
        }
        TextsBox.gameObject.SetActive(false); //Disable TextsBox
    }

    //Add the texts to the scroll view prefab
    public void SetTextAlternative(GameObject TargetText)
    {
        TargetText.gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = texts;
    }

    //Play specific UI sound
    public void PlaySoundUI(string mySoundName)
    {
        controllers.GetComponent<SoundsController>().PlaySoundUI(mySoundName);
    }

    //Play specific Ambiance sound
    public void PlaySoundAmbiance(string mySoundName)
    {
        controllers.GetComponent<SoundsController>().PlaySoundAmbiance(mySoundName);
    }

    //Play specific Music sound
    public void PlaySoundMusic(string mySoundName)
    {
        controllers.GetComponent<SoundsController>().PlaySoundMusic(mySoundName);
    }

    //Load Ctreate Account View
    public void LoadAccountCreation()
    {
        GameEventMessage.SendEvent("LoadCreateAccount");
    }
}