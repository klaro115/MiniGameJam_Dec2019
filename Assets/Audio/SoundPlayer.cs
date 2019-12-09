using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip[] audioClips;

    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play()
    {
        //Randomize Clip >

        int randomAudioClipIndex = GetRamdomIndex(0, audioClips.Length);

        Debug.Log("randomAudioClipIndex " + randomAudioClipIndex);

        audioSource.clip = audioClips[randomAudioClipIndex];

        //Randomize Clip <

        //Randomize Pitch >
        float randomPitch = Random.Range(minPitch, maxPitch);

        Debug.Log("randomPitch " + randomPitch);

        audioSource.pitch = randomPitch;

        //Randomize Pitch <

        audioSource.Play();
    }

    public int GetRamdomIndex(int min, int max)
    {
        int returnIndex = Random.Range(min, max);
        return Random.Range(min, max);
    }
}
