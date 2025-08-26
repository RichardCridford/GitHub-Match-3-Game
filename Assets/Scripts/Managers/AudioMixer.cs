using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundEffects
{ 
    land,
    swap,
    resolve,
    upgrade,
    powerup,
    score

}
    

[RequireComponent (typeof(AudioSource))]
public class AudioMixer : Singleton<AudioMixer>
{
    [SerializeField]
    private AudioSource music,
                        soundEffects;

    [   
        // could also use Tooltip instead of Header if you wanted
        Header
        (
            "0 = land\n" +
            "1 = swap\n" +
            "2 = resolve\n" +
            "3 = upgrade\n" +
            "4 = powerup\n" +
            "5 = score\n" 
        )
    ]

    
    [SerializeField]
    private AudioClip[] sounds;

    protected override void Init()
    {
        soundEffects = GetComponent<AudioSource>();
    }
    // Play background music
    public void PlayMusic()
    { 
        music.Play();
    }

    // Pause/unpause backgroudn music
    public void PauseMusic(bool pause)
    { 
        if(pause)
            music.Pause();
        else
            music.UnPause();
    }

    // Play a sound effect
    public void PlaySound(SoundEffects effect)
    {
        soundEffects.PlayOneShot(sounds[ (int) effect]);
    }

    // Play sound effect after time delay
    public IEnumerator PlayDelayedSound(SoundEffects effect, float t)
    { 
        yield return new WaitForSeconds(t);
        PlaySound(effect);
    }

}
