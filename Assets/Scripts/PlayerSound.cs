using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private AudioMixerGroup stepsMixer;
    [SerializeField] private AudioMixerGroup hurtMixer;
    [SerializeField] private AudioMixerGroup deathMixer;
    [SerializeField] private AudioMixerGroup attackMixer;
    [SerializeField] private AudioMixerGroup rollMixer;
    [SerializeField] private AudioClip[] footsteps;
    [SerializeField] private AudioClip[] hurt;
    [SerializeField] private AudioClip death;
    [SerializeField] private AudioClip[] attack;
    [SerializeField] private AudioClip[] roll;

    private AudioSource audioSource;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    public void PlayStep()
    {
        AudioClip chosenClip = footsteps[Random.Range(0, footsteps.Length)];


        audioSource.outputAudioMixerGroup = stepsMixer;
        audioSource.pitch = Random.Range(0.85f,1.15f);
        audioSource.PlayOneShot(chosenClip);
    }
    
    public void PlayAttack()
    {
        AudioClip chosenClip = attack[Random.Range(0, attack.Length)];


        audioSource.outputAudioMixerGroup = attackMixer;
        audioSource.pitch = Random.Range(0.8f,1.2f);
        audioSource.PlayOneShot(chosenClip);
    }
    
    public void PlayHurt()
    {
        AudioClip chosenClip = hurt[Random.Range(0, hurt.Length)];


        audioSource.outputAudioMixerGroup = hurtMixer;
        audioSource.pitch = Random.Range(0.9f,1.1f);
        audioSource.PlayOneShot(chosenClip);
    }
    
    public void PlayDeath()
    {
        audioSource.outputAudioMixerGroup = deathMixer;
        audioSource.pitch = 1.0f;
        audioSource.PlayOneShot(death);
    }
    
    public void PlayRoll()
    {
        AudioClip chosenClip = roll[Random.Range(0, roll.Length)];


        audioSource.outputAudioMixerGroup = rollMixer;
        audioSource.pitch = Random.Range(0.9f,1.1f);
        audioSource.PlayOneShot(chosenClip);
    }
}
