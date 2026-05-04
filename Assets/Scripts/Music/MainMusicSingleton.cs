using UnityEngine;

public class MainMusicSingleton : MonoBehaviour
{
    private static MainMusicSingleton musicInstance;

    private AudioSource audioSource;

    private void Awake()
    {
        if (musicInstance != null && musicInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        musicInstance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        
    }
}
