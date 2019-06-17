using System.Collections;
using Hellmade.Sound;
using UnityEngine;

public class BackgroundSoundController : MonoBehaviour
{
    public AudioClip backgroundMusic;
    public AudioClip backgroundSound;

    public float secondsUntilBGSoundStarts;

    public bool turnOff;

    private bool _isOff;

    private int _bgMusicId;
    
    [Tooltip("A range from 0 to 1.0")]
    [Range(0, 1)]
    public float bgMusicVolume;

    void Awake()
    {
        _isOff = !turnOff;
        _bgMusicId = EazySoundManager.PrepareMusic(backgroundMusic, bgMusicVolume, true, false, 1, 1);
        EazySoundManager.GetMusicAudio(_bgMusicId).Stop();
    }

    void Update()
    {
        if (turnOff == _isOff) return;

        var bgMusic = EazySoundManager.GetMusicAudio(_bgMusicId);
        if (turnOff)
        {
            bgMusic.Stop();
        }
        else
        {
            bgMusic.Play();
        }
        
        _isOff = turnOff;
    }

    IEnumerator WaitToPlayBGSound()
    {
        yield return new WaitForSeconds(secondsUntilBGSoundStarts);
    }
}