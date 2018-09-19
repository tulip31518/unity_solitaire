using UnityEngine;
using System.Collections;
using System;

public class SoundManager : MonoBehaviour {

    private AudioSource audioSource;
    public static SoundManager instance;
    public AudioClip reverseCardSound;
    public AudioClip dropCardSound;
    public AudioClip pickCardSound;
    public AudioClip btnClickSound;

    private bool locked = false;

    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayReverseCardSound()
    {
        PlaySound(reverseCardSound);
    }

    public void PlayPickCardSound()
    {
        PlaySound(pickCardSound);
    }

    public void PlayDropCardSound()
    {
        PlaySound(dropCardSound);
    }

    public void PlayBtnClickSound()
    {
        PlaySound(btnClickSound, true);
    }


    private void PlaySound(AudioClip audioClip, bool isPrioritySound = false)
    {
        if (!locked || isPrioritySound)
        {
            locked = true;
            audioSource.PlayOneShot(audioClip);
            Invoke("Unlock", Constants.SOUND_LOCK_TIME);
        }
    }

    void Unlock()
    {
        locked = false;
    }
}
