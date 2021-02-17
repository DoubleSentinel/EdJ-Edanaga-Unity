using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    private int selectedCharacterIndex;
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

    private void Start()
    {
        UpdateCharacterSelectionUI();
    }
    private void Update()
    {
        backgroundColor.color = Color.Lerp(backgroundColor.color, desiredColor, Time.deltaTime * backgroudColorTransitionSpeed);
    }

    public void LeftArrow()
    {
        UpdateCharacterSelectionUI();
    }

    public void RightArrow()
    {
        UpdateCharacterSelectionUI();
    }

    public void Confirm()
    {

    }

    private void UpdateCharacterSelectionUI()
    {
        SetupPlayer();
    }

    private void SetupPlayer()
    {
        float height = Screen.height * 0.75f / 2f;
        float depth = -1f;
        Vector3 player = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 4, height));

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
