using Doozy.Engine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControllerChapter0 : MonoBehaviour
{
    private int selectedCharacterIndex = 0;
    
    private Color desiredColor;

    [Header("2D Scene References")]
    [SerializeField] private GameObject scenePlayerCh0;
    [SerializeField] private Transform heads;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private Image characterImage;
    [SerializeField] private Image backgroundColor;

    [Header("Tweaks")]
    [SerializeField] private float backgroudColorTransitionSpeed = 10f;

    // Local variables
    private GameObject controllers;
    private string texts;
    private List<string> listHeads;
    private int headCount;

    void Awake()
    {
        controllers = GameObject.Find("Controllers");

        headCount = heads.childCount;

        //scenePlayerCh0 = scenePlayerCh0.gameObject.transform.GetChild(0).GetChild(2).gameObject;
        //heads = scenePlayerCh0.transform.GetChild(1).GetChild(2).childCount - 3; //Number of possible characters
    }

    private void Start()
    {
        int i = 0;
        /*
        int randomVal = Random.Range(0, headCount -3);

        while (i < headCount -3)
        {
            heads.GetChild(i).gameObject.SetActive(i==randomVal);
            i++;
        }
        */
        selectedCharacterIndex = 10; //Otarie as head default
        UpdateCharacterSelectionUI();
    }
    private void Update()
    {
        //backgroundColor.color = Color.Lerp(backgroundColor.color, desiredColor, Time.deltaTime * backgroudColorTransitionSpeed);
    }

    public void LeftArrow()
    {
        heads.GetChild(selectedCharacterIndex).gameObject.SetActive(false);
        
        selectedCharacterIndex--;
        if (selectedCharacterIndex < 0)
            selectedCharacterIndex = headCount - 4;

        heads.GetChild(selectedCharacterIndex).gameObject.SetActive(true);
        UpdateCharacterSelectionUI();
    }

    public void RightArrow()
    {
        heads.GetChild(selectedCharacterIndex).gameObject.SetActive(false);
        
        selectedCharacterIndex++;
        if (selectedCharacterIndex == headCount -3)
            selectedCharacterIndex = 0;

        heads.GetChild(selectedCharacterIndex).gameObject.SetActive(true);
        UpdateCharacterSelectionUI();
    }

    public void Confirm()
    {
        listHeads = new List<string>();

        //get currently selected head as player
        controllers.GetComponent<TestingEnvironment>().Characters.Add("Player", characterName.text);
        controllers.GetComponent<TestingEnvironment>().Characters.Add("Host", "Bernard");
        controllers.GetComponent<TestingEnvironment>().Characters.Add("Journalist", "Mouse");
        controllers.GetComponent<TestingEnvironment>().Characters.Add("Engineer", "Hibou");

        //List of the possible heads for the Objectives
        for (int i = 0; i < headCount -3; i++)
        {
            listHeads.Add(heads.GetChild(i).gameObject.name);
        }
        listHeads.Remove(characterName.text); //Remove Player choice from the list

        for (int i = 0; i < 10; i++) //Objective0 to Objective9 
        {
            int index = Random.Range(0, listHeads.Count);
            controllers.GetComponent<TestingEnvironment>().Characters.Add($"Objective{i}", listHeads[index]);
            listHeads.RemoveAt(index); //Adapt listHeads
        }
        foreach (KeyValuePair<string, string> character in controllers.GetComponent<TestingEnvironment>().Characters)
        {
            print($"Char: {character.Key} Head: {character.Value}");
        }         
    }

    private void UpdateCharacterSelectionUI()
    {
        characterName.text = scenePlayerCh0.gameObject.transform.GetChild(1).GetChild(2).GetChild(selectedCharacterIndex).gameObject.name;
        SetupPlayer();
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
            texts += TextsBox.gameObject.transform.GetChild(i).GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text;
            texts += "\n\n"; //new line characters
        }
        TextsBox.gameObject.SetActive(false); //Disable TextsBox
    }

    //Add the texts to the scroll view prefab
    public void SetTextAlternative(GameObject TargetText)
    {
        TargetText.gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = texts;
    }

    //Load Ctreate Account View
    public void LoadAccountCreation()
    {
        GameEventMessage.SendEvent("LoadCreateAccount");
    }

    [System.Serializable]
    public class CharacterSelectObject
    {
        public Sprite splash;
        public string characterName;
        public Color characterColor;
    }
}