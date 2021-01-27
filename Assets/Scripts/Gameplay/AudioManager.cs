using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer _MasterMixer;

    public void SetMasterVolume(Slider volume)
    {
        _MasterMixer.SetFloat("VolumeMaster", -volume.value);
    }

    public void SetUIVolume(Slider volume)
    {
        _MasterMixer.SetFloat("VolumeUI", -volume.value);
    }

    public void SetMusicVolume(Slider volume)
    {
        _MasterMixer.SetFloat("VolumeMusic", -volume.value);
    }

    public void SetSFXVolume(Slider volume)
    {
        _MasterMixer.SetFloat("VolumeSFX", -volume.value);
    }
}