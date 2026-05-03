using UnityEngine;

public class MainMusicSingleton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    MainMusicSingleton musicInstance;

    private void Awake()
    {
        if (musicInstance == null)
        {
            musicInstance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(musicInstance);
        }
        
    }
}
