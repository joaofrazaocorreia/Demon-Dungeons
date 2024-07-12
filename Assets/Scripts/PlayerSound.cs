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

    private AudioSource stepsAudioSource;
    private AudioSource attackAudioSource;
    private AudioSource hurtAudioSource;
    private AudioSource deathAudioSource;
    private AudioSource rollAudioSource;


    private void Start()
    {
        stepsAudioSource = gameObject.AddComponent<AudioSource>();
        stepsAudioSource.outputAudioMixerGroup = stepsMixer;

        attackAudioSource = gameObject.AddComponent<AudioSource>();
        attackAudioSource.outputAudioMixerGroup = attackMixer;

        hurtAudioSource = gameObject.AddComponent<AudioSource>();
        hurtAudioSource.outputAudioMixerGroup = hurtMixer;

        deathAudioSource = gameObject.AddComponent<AudioSource>();
        deathAudioSource.outputAudioMixerGroup = deathMixer;

        rollAudioSource = gameObject.AddComponent<AudioSource>();
        rollAudioSource.outputAudioMixerGroup = rollMixer;
    }
    
    public void PlayStep()
    {
        AudioClip chosenClip = footsteps[Random.Range(0, footsteps.Length)];


        stepsAudioSource.pitch = Random.Range(0.85f, 1.15f);
        stepsAudioSource.PlayOneShot(chosenClip);
    }
    
    public void PlayAttack()
    {
        AudioClip chosenClip = attack[Random.Range(0, attack.Length)];


        attackAudioSource.pitch = Random.Range(0.8f, 1.2f);
        attackAudioSource.PlayOneShot(chosenClip);
    }
    
    public void PlayHurt()
    {
        AudioClip chosenClip = hurt[Random.Range(0, hurt.Length)];


        hurtAudioSource.pitch = Random.Range(0.9f, 1.1f);
        hurtAudioSource.PlayOneShot(chosenClip);
    }
    
    public void PlayDeath()
    {
        deathAudioSource.pitch = Random.Range(0.9f, 1.1f);
        deathAudioSource.PlayOneShot(death);
    }
    
    public void PlayRoll()
    {
        AudioClip chosenClip = roll[Random.Range(0, roll.Length)];


        rollAudioSource.pitch = Random.Range(0.95f, 1.15f);
        rollAudioSource.PlayOneShot(chosenClip);
    }
}
