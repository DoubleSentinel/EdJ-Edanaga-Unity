using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer _MasterMixer;

    //Set Master Mixer volume from the slider value
    public void SetMasterVolume(Slider volume)
    {
        _MasterMixer.SetFloat("VolumeMaster", volume.value);
    }

    //Set UI Mixer volume from the slider value

    public void SetUIVolume(Slider volume)
    {
        _MasterMixer.SetFloat("VolumeUI", volume.value);
    }

    //Set Music Mixer volume from the slider value

    public void SetMusicVolume(Slider volume)
    {
        _MasterMixer.SetFloat("VolumeMusic", volume.value);
    }

    //Set SFX Mixer volume from the slider value
    public void SetSFXVolume(Slider volume)
    {
        _MasterMixer.SetFloat("VolumeSFX", volume.value);
    }
}