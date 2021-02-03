using System.Collections;
using UnityEngine;
using Doozy.Engine.Soundy;
using UnityEngine.UI;

//Used to set the sounds to play and make transition between sounds
public class SoundsController : MonoBehaviour
{
    //Sliders on the SoundsView
    public Slider masterSlider;
    public Slider uiSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    //The sound category
    private string mySoundDatabaseName;
    
    private Slider slider;
    private float duration = 0.5f; //fade sound speed
    private float targetVolume = 0.0f; //Default volume intensity
    public bool isFinished = false; //fade sound state

    private void Start()
    {
        SoundyManager.Init();
    }

    //Set FadeSound targetVolume value
    private void FadeSoundParam(bool UpToDown)
    {
        targetVolume = UpToDown ? targetVolume = masterSlider.minValue+40 : targetVolume = 0; //Slow down to -40dB or default value
    }
    
    //Fade sound using master slider volume values
    private IEnumerator StartFade(Slider slider, float duration, float targetVolume)
    {
        isFinished = false;
        float currentTime = 0;
        float start = slider.value;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            slider.value = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        isFinished = true;
        yield break;
    }

    //Wait the end of the StartFade coroutine and start next sound
    IEnumerator NextFade(string mySoundDatabaseName, string mySoundName)
    {
        while (!isFinished)
            yield return new WaitForSeconds(0.1f);

        SoundyManager.StopAllSounds();
        FadeSoundParam(false);
        StartCoroutine(StartFade(masterSlider, duration, targetVolume));
        SoundyManager.Play(mySoundDatabaseName, mySoundName);
        yield break;
    }

    //Play specific UI sound
    public void PlaySoundUI(string mySoundName)
    {
        mySoundDatabaseName = "UI";
        FadeSoundParam(true);
        StartCoroutine(StartFade(masterSlider, duration, targetVolume));
        StartCoroutine(NextFade(mySoundDatabaseName, mySoundName));
    }

    //Play specific Ambiance sound
    public void PlaySoundAmbiance(string mySoundName)
    {
        mySoundDatabaseName = "Ambiance";
        FadeSoundParam(true);
        StartCoroutine(StartFade(masterSlider, duration, targetVolume));
        StartCoroutine(NextFade(mySoundDatabaseName, mySoundName));
    }

    //Play specific Music sound
    public void PlaySoundMusic(string mySoundName)
    {
        mySoundDatabaseName = "Music";
        FadeSoundParam(true);
        StartCoroutine(StartFade(masterSlider, duration, targetVolume));
        StartCoroutine(NextFade(mySoundDatabaseName, mySoundName));
    }
}