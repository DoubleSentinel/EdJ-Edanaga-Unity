using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControllerChapter0 : MonoBehaviour
{
    private int selectedCharacterIndex = 7;
    private int characterCount;
    private Color desiredColor;

    [Header("2D Scene References")]
    [SerializeField] private GameObject scenePlayer;

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

    void Awake()
    {
        controllers = GameObject.Find("Controllers");
    }

    private void Start()
    {
        controllers.GetComponent<LanguageHandler>().translateUI();
        characterCount = scenePlayer.gameObject.transform.GetChild(1).GetChild(2).childCount; //Number of possible characters
        UpdateCharacterSelectionUI();
    }
    private void Update()
    {
        backgroundColor.color = Color.Lerp(backgroundColor.color, desiredColor, Time.deltaTime * backgroudColorTransitionSpeed);
    }

    public void LeftArrow()
    {
        scenePlayer.gameObject.transform.GetChild(1).GetChild(2).GetChild(selectedCharacterIndex).gameObject.SetActive(false);

        selectedCharacterIndex--;
        if (selectedCharacterIndex < 0)
            selectedCharacterIndex = characterCount - 1;

        print("selectedCharacterIndex = " + selectedCharacterIndex); 
        scenePlayer.gameObject.transform.GetChild(1).GetChild(2).GetChild(selectedCharacterIndex).gameObject.SetActive(true);
        UpdateCharacterSelectionUI();
    }

    public void RightArrow()
    {
        scenePlayer.gameObject.transform.GetChild(1).GetChild(2).GetChild(selectedCharacterIndex).gameObject.SetActive(false);

        selectedCharacterIndex++;
        if (selectedCharacterIndex == characterCount)
            selectedCharacterIndex = 0;
        
        print("selectedCharacterIndex = " + selectedCharacterIndex);
        scenePlayer.gameObject.transform.GetChild(1).GetChild(2).GetChild(selectedCharacterIndex).gameObject.SetActive(true);
        UpdateCharacterSelectionUI();
    }

    public void Confirm()
    {
        //Save character choice and load Chapter1
    }

    private void UpdateCharacterSelectionUI()
    {
        characterName.text = scenePlayer.gameObject.transform.GetChild(1).GetChild(2).GetChild(selectedCharacterIndex).gameObject.name;

        SetupPlayer();
    }

    private void SetupPlayer()
    {
        float height = Screen.height * 0.75f / 2f;
        float depth = -1f;
        Vector3 player = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, height));

        scenePlayer.transform.position = new Vector3(player.x, player.y, depth);
        scenePlayer.SetActive(true);
    }

    [System.Serializable]
    public class CharacterSelectObject
    {
        public Sprite splash;
        public string characterName;
        public Color characterColor;
    }
}
