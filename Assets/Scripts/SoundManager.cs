using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource levelAudioSource;
    [SerializeField] private AudioSource safeAudioSource;
    [SerializeField] private AudioSource bossAudioSource;

    public void InitSafeRoomAudio()
    {
        bossAudioSource.Stop();
        levelAudioSource.Stop();

        safeAudioSource.Play();
    }

    public void InitLevelAudio()
    {
        bossAudioSource.Stop();
        safeAudioSource.Stop();

        levelAudioSource.Play();
    }

    public void InitBossRoomAudio()
    {
        safeAudioSource.Stop();
        levelAudioSource.Stop();

        bossAudioSource.Play();
    }
}
