using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        
        if (instance == null)
        {

            instance = this;

        }

    }



    // 1 is full volume, use to call sound effects in individual scripts

    //[SerializeField] private AudioClip neededAudioClip;
    //AudioManager.instance.SoundFXPlay(audioclip, transform, 1f);

    //[SerializeField] private AudioClip[] neededAudioClips;
    //AudioManager.instance.RandomSoundFXPlay(audiocliparray, transform, 1f);


    // Calling this instantiates a sound clip, plays it, then deletes it
    public void SoundFXPlay(AudioClip audioClip, Transform spawnTransform, float volume)
    {

        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);

    }


    // Calling this instantiates a random sound clip from an arrray, plays it, then deletes it
    public void RandomSoundFXPlay(AudioClip[] audioClip, Transform spawnTransform, float volume)
    {

        int rand = Random.Range(0, audioClip.Length);

        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip[rand];

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);

    }

}