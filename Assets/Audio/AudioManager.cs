using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public DuplicateSoundSetting duplicateSoundSetting = DuplicateSoundSetting.Randomize;
    public Sound[] sounds;

    public static AudioManager instance;
    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioSound;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.mixerGroup;
            if (s.playOnAwake)
                s.source.Play();
        }
    }
    private Dictionary<string, int> soundIndex = new Dictionary<string, int>();
    public void Play(string soundName)
    {
        Sound[] s = Array.FindAll(sounds, sound => sound.name == soundName);
        if(s.Count() == 0)
        {
            Debug.LogWarning("Sound, " + soundName + ", was not found.");
            return;
        }
        if (s.Count() > 1)
        {
            switch (duplicateSoundSetting)
            {
                case DuplicateSoundSetting.Randomize:
                    s[UnityEngine.Random.Range(0, s.Count())].source.Play();
                    break;
                case DuplicateSoundSetting.PlayAll:
                    foreach (Sound sound in s)
                    {
                        sound.source.Play();
                    }
                    break;
                case DuplicateSoundSetting.PlayInOrder:
                    if (soundIndex.TryGetValue(soundName, out int index)){
                        if (index >= s.Count())
                        {
                            index = 0;
                        }
                        soundIndex[soundName] = index + 1;
                        s[index].source.Play();
                    }
                    else
                    {
                        soundIndex.Add(soundName, 1);
                        s[0].source.Play();
                    }
                    break;
            }
        }
        else
        {
            s[0].source.Play();
        }
    }
    public void StopPlaying (string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound, " + soundName + ", was not found.");
            return;
        }
        s.source.Stop();
    }
    public float GetLengthOf(string soundName)
    {
        Sound s = Array.Find(sounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound, " + soundName + ", was not found.");
            return 0;
        }
        return s.source.clip.length;
    }
}
[System.Serializable]
public enum DuplicateSoundSetting
{
    Randomize,
    PlayAll,
    PlayInOrder
}