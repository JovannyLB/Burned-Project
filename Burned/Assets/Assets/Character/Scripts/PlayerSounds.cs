using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSounds : MonoBehaviour{

    private AudioSource characterAS;
    public AudioClip[] audioClips;

    void Start(){
        characterAS = GetComponent<AudioSource>();
    }

    public void PlaySound(){
        characterAS.pitch = Random.Range(0.75f, 1.25f);
        characterAS.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
    }
}
