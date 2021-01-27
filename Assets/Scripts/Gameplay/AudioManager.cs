using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer masterMixer;
    public Slider masterSlider;
    public Slider uiSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    public Toggle toggle;
    private float masterVolume = 0;

    //Mute/Unmute toogle
    public void ToggleMute(Toggle mute)
    {
        if (mute.isOn)
        {
            masterMixer.SetFloat("VolumeMaster", -80.0f);
            masterSlider.interactable = false;
            uiSlider.interactable = false;
            musicSlider.interactable = false;
            sfxSlider.interactable = false;
        }
        else
        {
            masterSlider.interactable = true;
            uiSlider.interactable = true;
            musicSlider.interactable = true;
            sfxSlider.interactable = true;
            masterMixer.SetFloat("VolumeMaster", masterVolume);
        }
    }

    //Set Master Mixer volume from the slider value
    public void SetMasterVolume()
    {
        masterVolume = masterSlider.value;
        masterMixer.SetFloat("VolumeMaster", masterSlider.value);
    }

    //Set UI Mixer volume from the slider value

    public void SetUIVolume()
    {
        masterMixer.SetFloat("VolumeUI", uiSlider.value);
    }

    //Set Music Mixer volume from the slider value

    public void SetMusicVolume()
    {
        masterMixer.SetFloat("VolumeMusic", musicSlider.value);
    }

    //Set SFX Mixer volume from the slider value
    public void SetSFXVolume()
    {
        masterMixer.SetFloat("VolumeSFX", sfxSlider.value);
    }
}