using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SnapshotManager : MonoBehaviour
{

    [Header("Mixer & Snapshots")]
    [SerializeField] private List<AudioMixerSnapshot> snapshots;
    [SerializeField] private AudioMixer mainMixer;

    [SerializeField]
    [Range(0f, 1f)]
    private float transitionDelay = 0.5f;

    private void Start()
    {
        

    }


    // Calling this can transition to other snapshots
    // Use to change audio along with game state
    public void TransitionToSnapshot(int snapshot)
    {

        snapshots[snapshot].TransitionTo(transitionDelay);

    }


    // Still working on below code, currently doesnt work



    // Sets a master lowpass when pause menu is on
    public void LowPassPause(float wetVol)
    {


        mainMixer.SetFloat("Lowpass", wetVol);

    }

    // Turns off lowpass when pause menu is off
    public void LowPassUnPause(float wetVol)
    {


        mainMixer.SetFloat("Lowpass", wetVol);

    }

    // Sets an extreme lowpass for when the player takes damage
    // Overtime the lowpass will filter back to 22000
    public void LowPassHitstop(float wetVol)
    {


        mainMixer.SetFloat("Lowpass", wetVol);

    }

}