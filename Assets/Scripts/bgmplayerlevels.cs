using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    public AudioClip[] songs;          // Add your 3 songs here
    private AudioSource audioSource;
    private int currentSongIndex = 0;

    void Awake()
    {
        // Keep music playing even after scene changes
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Safety check
        if (songs.Length == 0)
        {
            Debug.LogError("No songs assigned in BGMPlayer!");
            return;
        }

        PlaySong();
    }

    void Update()
    {
        // When current song ends, play next one
        if (!audioSource.isPlaying)
        {
            NextSong();
        }
    }

    void PlaySong()
    {
        audioSource.clip = songs[currentSongIndex];
        audioSource.Play();
    }

    void NextSong()
    {
        currentSongIndex++;

        // If last song finished, start again from first
        if (currentSongIndex >= songs.Length)
        {
            currentSongIndex = 0;
        }

        PlaySong();
    }
}
