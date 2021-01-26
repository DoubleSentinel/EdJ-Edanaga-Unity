using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Engine.Soundy;

public class SoundsController : MonoBehaviour
{
    //The sound category
    public string MySoundDatabaseName;
    //Sound Name
    public string MySoundName;

    public SoundyController MyController;
    //The AudioClip to play
    public AudioClip Clip;
    //The volume of the audio source (0.0 to 1.0)
    public float Volume;
    //The pitch of the audio source
    public float Pitch;
    //Is the audio clip looping?
    public bool Loop;
    //Sets how much this AudioSource is affected by 3D spatialisation calculations (attenuation, doppler etc). 0.0 makes the sound full 2D, 1.0 makes it full 3D
    public float SpatialBlend = 0.0f; //Default

    private void Start()
    {
        SoundyManager.Init();
    }

    //Play specific UI sound
    public void PlaySoundUI(string MySoundName)
    {
        MySoundDatabaseName = "UI"; 
	    SoundyManager.Play(MySoundDatabaseName, MySoundName);
    }

    //Play specific Ambiance sound
    public void PlaySoundAmbiance(string MySoundName)
    {
        MySoundDatabaseName = "Ambiance";
        SoundyManager.Play(MySoundDatabaseName, MySoundName);
        print("PlayAmbiance : " + MySoundName);
    }

    public void PlaySoundMusic(string MySoundName)
    {
        MySoundDatabaseName = "Music";
        SoundyManager.Play(MySoundDatabaseName, MySoundName);
        print("PlayMusic : " + MySoundName);
    }

    //Stop all sounds
    public void StopSounds()
    {
        SoundyManager.StopAllSounds();
    }

    //Mute all sounds
    public void MuteSounds()
    {
        SoundyManager.MuteAllSounds();
    }

    //Set the given settings to the target AudioSource
    public void SetAudio()
    { 
	    //Sets how much this AudioSource is affected by 3D spatialisation calculations (attenuation, doppler etc). 0.0 makes the sound full 2D, 1.0 makes it full 3D        public float SpatialBlend;
        MyController.SetSourceProperties(Clip, Volume, Pitch, Loop, SpatialBlend);
    }
}