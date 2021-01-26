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
        //SoundyController controller = SoundyManager.GetController();
        SoundyManager.Init();
    }

    //Play specific sound
    public void PlaySound(string MySoundDatabaseName, string MySoundName)
    {
	    SoundyManager.Play(MySoundDatabaseName, MySoundName);
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