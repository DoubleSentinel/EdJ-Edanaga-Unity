using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControllerChapter0 : MonoBehaviour
{
    private int selectedCharacterIndex = 0;
    private int characterCount;
    private Color desiredColor;

    [Header("2D Scene References")]
    [SerializeField] private GameObject scenePlayerCh0;
    [SerializeField] private GameObject scenePlayerCh1;

    /*
    [Header("List of characters")]
    [SerializeField] private List<CharacterSelectObject> characterList = new List<CharacterSelectObject>();
    */
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private Image characterImage;
    [SerializeField] private Image backgroundColor;

    [Header("Tweaks")]
    [SerializeField] private float backgroudColorTransitionSpeed = 10f;

    // Local variables
    private GameObject controllers;

    private string texts;

    void Awake()
    {
        controllers = GameObject.Find("Controllers");
    }

    private void Start()
    {
        controllers.GetComponent<LanguageHandler>().translateUI();

        scenePlayerCh1 = scenePlayerCh1.gameObject.transform.GetChild(0).GetChild(2).gameObject;
        scenePlayerCh0 = scenePlayerCh0.gameObject.transform.GetChild(0).GetChild(2).gameObject;

        characterCount = scenePlayerCh1.gameObject.transform.GetChild(1).GetChild(2).childCount - 3; //Number of possible characters

        int i = 0;
        int randomVal = Random.Range(0, characterCount);
        while (i < characterCount)
        {
            /*
            if (scenePlayerCh0.gameObject.transform.GetChild(1).GetChild(2).GetChild(i).gameObject.activeSelf)
            {
                selectedCharacterIndex = i;
            }
            */
            if( i == randomVal)
            {
                scenePlayerCh0.gameObject.transform.GetChild(1).GetChild(2).GetChild(i).gameObject.SetActive(true);
                scenePlayerCh1.gameObject.transform.GetChild(1).GetChild(2).GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                scenePlayerCh0.gameObject.transform.GetChild(1).GetChild(2).GetChild(i).gameObject.SetActive(false);
                scenePlayerCh1.gameObject.transform.GetChild(1).GetChild(2).GetChild(i).gameObject.SetActive(false);
            }

            i++;
        }
        selectedCharacterIndex = randomVal;
        UpdateCharacterSelectionUI();
    }
    private void Update()
    {
        backgroundColor.color = Color.Lerp(backgroundColor.color, desiredColor, Time.deltaTime * backgroudColorTransitionSpeed);
    }

    public void LeftArrow()
    {
        scenePlayerCh1.gameObject.transform.GetChild(1).GetChild(2).GetChild(selectedCharacterIndex).gameObject.SetActive(false);
        scenePlayerCh0.gameObject.transform.GetChild(1).GetChild(2).GetChild(selectedCharacterIndex).gameObject.SetActive(false);

        selectedCharacterIndex--;
        if (selectedCharacterIndex < 0)
            selectedCharacterIndex = characterCount - 1;

        scenePlayerCh1.gameObject.transform.GetChild(1).GetChild(2).GetChild(selectedCharacterIndex).gameObject.SetActive(true);
        scenePlayerCh0.gameObject.transform.GetChild(1).GetChild(2).GetChild(selectedCharacterIndex).gameObject.SetActive(true);
        UpdateCharacterSelectionUI();
    }

    public void RightArrow()
    {
        scenePlayerCh1.gameObject.transform.GetChild(1).GetChild(2).GetChild(selectedCharacterIndex).gameObject.SetActive(false);
        scenePlayerCh0.gameObject.transform.GetChild(1).GetChild(2).GetChild(selectedCharacterIndex).gameObject.SetActive(false);

        selectedCharacterIndex++;
        if (selectedCharacterIndex == characterCount)
            selectedCharacterIndex = 0;

        scenePlayerCh1.gameObject.transform.GetChild(1).GetChild(2).GetChild(selectedCharacterIndex).gameObject.SetActive(true);
        scenePlayerCh0.gameObject.transform.GetChild(1).GetChild(2).GetChild(selectedCharacterIndex).gameObject.SetActive(true);
        UpdateCharacterSelectionUI();
    }

    public void Confirm()
    {
        //Save character choice and load Chapter1
    }

    private void UpdateCharacterSelectionUI()
    {
        characterName.text = scenePlayerCh1.gameObject.transform.GetChild(1).GetChild(2).GetChild(selectedCharacterIndex).gameObject.name;

        SetupPlayer();
    }

    public void SetupPlayer()
    {
        float height = Screen.height * 0.75f / 2f;
        float depth = -1f;
        Vector3 player = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, height));

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


    [System.Serializable]
    public class CharacterSelectObject
    {
        public Sprite splash;
        public string characterName;
        public Color characterColor;
    }
}
