using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{

    [SerializeField] private AudioMixer mainMixer;


    // Allows player to change master volume in settings
    public void SetMasterVolume(float level)
    {

        mainMixer.SetFloat("MasterVolume", Mathf.Log10(level) * 20f);

    }

    // Allows player to change music volume in settings
    public void SetMusicVolume(float level)
    {

        mainMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);

    }

    // Allows player to change soundFX volume in settings
    public void SetSoundFXVolume(float level)
    {

        mainMixer.SetFloat("SoundFXVolume", Mathf.Log10(level) * 20f);

    }
}