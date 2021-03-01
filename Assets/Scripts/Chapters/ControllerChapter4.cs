using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ControllerChapter4 : MonoBehaviour
{
    [Header("2D Scene References")]
    [SerializeField] private GameObject scenePlayer;
    [SerializeField] private GameObject sceneJournalist;
    [SerializeField] private GameObject sceneEngineer;
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
        float heightMiddle = Screen.height * 0.75f / 2f;
        float heightBottom = heightMiddle / 2f;
        float heightTop = heightMiddle * 1.25f;
        float widthSpace = Screen.width / 10; 

        float depth = -1f;
        int index, i = 0;

        //Default position of the Player, Journalist, engineer
        Vector3 player = Camera.main.ScreenToWorldPoint(new Vector3(widthSpace*5, heightBottom));
        Vector3 journalist = Camera.main.ScreenToWorldPoint(new Vector3(widthSpace, heightMiddle));
        Vector3 engineer = Camera.main.ScreenToWorldPoint(new Vector3(widthSpace*9, heightMiddle));
        
        scenePlayer.transform.position = new Vector3(player.x, player.y, depth);
        sceneJournalist.transform.position = new Vector3(journalist.x, journalist.y, depth);
        sceneEngineer.transform.position = new Vector3(engineer.x, engineer.y, depth);
        
        scenePlayer.SetActive(true);
        sceneJournalist.SetActive(true);
        sceneEngineer.SetActive(true);


        //10 posiible positions for 10 objectives
        Vector3[] positions = new[] { new Vector3(widthSpace*3, heightTop),
                                      new Vector3(widthSpace*7, heightTop),
                                      new Vector3(widthSpace*7, heightTop),
                                      new Vector3(widthSpace*1, heightTop),
                                      new Vector3(widthSpace*3, heightTop),
                                      new Vector3(widthSpace*7, heightTop),
                                      new Vector3(widthSpace*9, heightTop),
                                      new Vector3(widthSpace*2, heightTop),
                                      new Vector3(widthSpace*4, heightTop),
                                      new Vector3(widthSpace*6, heightTop) };

        //Swing classification results
        var results = controllers.GetComponent<TestingEnvironment>().SwingClassification;
        
        // creating the visual list with the given prefab
        foreach (KeyValuePair<string, double> result in results.OrderByDescending(x => x.Value))
        {
            index = int.Parse(result.Key.Last().ToString());

            //Take the 2 first objectives in the Swing results
            if (i < 2)
            {
                //Set position depending of the Swing classification
                Vector3 position = Camera.main.ScreenToWorldPoint(positions[index]);
                sceneObjectives[index].gameObject.transform.position = new Vector3(position.x, position.y, depth);

                if (positions[i].x < widthSpace*5) //On the left of the Player position
                {
                    sceneObjectives[index].gameObject.transform.rotation = Quaternion.Euler(0, 180, 0); //Turn the body and head
                }

                //Adapt size of the objectives
                sceneObjectives[index].gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                sceneObjectives[index].gameObject.SetActive(true);
            }
            else
            {
                sceneObjectives[index].gameObject.SetActive(false);
            }
            i++;
        }
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