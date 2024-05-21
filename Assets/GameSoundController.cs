using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundController : MonoBehaviour
{
    public enum AudioType
    {
        Match,
        Mismatch,
        Victory
    }

    public AudioSource AudioSource;
    public List<AudioClip> AudioClips = new List<AudioClip>();

    public void PlayAudio(AudioType audioType)
    {
        int idx = (int)audioType;
        AudioSource.PlayOneShot(AudioClips[idx]);
    }
}
