using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// This manager script allows all AudioClips to be stored into an array via class inheritance,
/// eliminating the need to manually apply an AudioSource component to every gameobject via the inspector.
/// </summary>

public class SoundEffects : MonoBehaviour
{
    public Sound[] sounds;
    //public static SoundEffects instance = null;

    void Awake()
    {
        
        foreach(Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();

            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
        }
    }

    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.PlayOneShot(s.clip, 0.7f);
    }

    
}
