using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip[] music;
    public bool persistant = true;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayNextSong();

        if (persistant)
            DontDestroyOnLoad(this);
    }

    void PlayNextSong(){
        audioSource.clip = music[Random.Range(0,music.Length)];
        audioSource.Play();
        Invoke("PlayNextSong", audioSource.clip.length);
    }
}
