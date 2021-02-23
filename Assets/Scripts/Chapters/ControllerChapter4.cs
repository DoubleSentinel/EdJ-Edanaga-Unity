using TMPro;
using UnityEngine;

public class ControllerChapter4 : MonoBehaviour
{
    [Header("2D Scene References")]
    [SerializeField] private GameObject scenePlayer;
    [SerializeField] private GameObject sceneJournalist;
    [SerializeField] private GameObject sceneEngineer;
    [SerializeField] private GameObject sceneHost;
    [SerializeField] private GameObject[] sceneObjectives;

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
        SetupPictureFinsish();
    }

    private void SetupPictureFinsish()
    {
        float height = Screen.height * 0.75f / 2f;
        float depth = -1f;

        Vector3 player = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f,
            height / 2f));
        Vector3 journalist = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 1f / 10f,
           height));
        Vector3 engineer = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 9f / 10f,
            height));
        Vector3 host = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2f,
            height * 1.25f));
       
        Vector3 objectives0 = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 3 / 10f,
            height));

        sceneObjectives[0].transform.position = new Vector3(objectives0.x, objectives0.y, depth);
        sceneObjectives[0].transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        sceneObjectives[0].transform.rotation = Quaternion.Euler(0, -180, 0);

        scenePlayer.transform.position = new Vector3(player.x, player.y, depth);
        sceneJournalist.transform.position = new Vector3(journalist.x, journalist.y, depth);
        sceneEngineer.transform.position = new Vector3(engineer.x, engineer.y, depth);
        sceneHost.transform.position = new Vector3(host.x, host.y, depth);

        scenePlayer.SetActive(true);
        sceneJournalist.SetActive(true);
        sceneEngineer.SetActive(true);
        sceneHost.SetActive(true);
    }

    //Prepare texts for the introduction
    public void PrepareText(GameObject TextsBox)
    {
        //Get texts from TextBox
        texts = "\n\n";
        for (int i = 0; i < TextsBox.transform.childCount; i++)
        {
            texts += TextsBox.gameObject.transform.GetChild(i).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text;
            texts += "\n\n"; //new line characters
        }
        TextsBox.gameObject.SetActive(false); //Disable TextsBox
    }

    //Add the texts to the scroll view prefab
    public void SetText(GameObject TargetText)
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

    //Enable or disable options wheel
    public void EnableOptions(bool enable)
    {
        controllers.GetComponent<AudioManager>().EnableOptionWheel(enable); //options allowed or not allowed
    }
}