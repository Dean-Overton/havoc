using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private MusicType _currentMusicType = MusicType.Menu;
    public MusicType currentMusicType {
        get {
            return _currentMusicType;
        }
        set {
            if (_currentMusicType == value) return;
            _currentMusicType = value;
            StopMusicLoop();
            StartMusicLoop();
        }
    }

    public MusicLoop[] musicLoops;
    public static MusicManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start() {
        StartMusicLoop();
    }
    public void StartMusicLoop()
    {
        MusicLoop[] musicLoop = GetMusicLoop(currentMusicType);
        StartCoroutine(MusicLoopCoroutine(musicLoop));
    }
    public void StopMusicLoop()
    {
        foreach (MusicLoop musicLoop in musicLoops)
        {
            AudioManager.instance.StopPlaying(musicLoop.songAudioName);
        }
        StopAllCoroutines();
    }
    private MusicLoop[] GetMusicLoop(MusicType musicType)
    {
        Debug.Log("Getting Music Loops of type: " + musicType);
        List<MusicLoop> currentTypeLoops = new List<MusicLoop>();
        foreach (MusicLoop musicLoop in musicLoops)
        {
            if (musicLoop.musicType == musicType)
            {
                currentTypeLoops.Add(musicLoop);
            }
        }
        Debug.Log("Music Loops: " + currentTypeLoops.Count);
        return currentTypeLoops.ToArray();
    }
    private IEnumerator MusicLoopCoroutine(MusicLoop[] musicLoops)
    {
        Debug.Log("Music Loop Coroutine Started");
        while(true) {
            foreach (MusicLoop musicLoop in musicLoops)
            {
                yield return new WaitForSeconds(musicLoop.songDelay);
                Debug.Log("Playing song: " + musicLoop.songAudioName);
                AudioManager.instance.Play(musicLoop.songAudioName);
                yield return new WaitForSeconds(AudioManager.instance.GetLengthOf(musicLoop.songAudioName) * musicLoop.songLoopCount);
                AudioManager.instance.StopPlaying(musicLoop.songAudioName);
            }
        }
    }

}
[System.Serializable]
public class MusicLoop {
    public string songAudioName;
    public float songDelay;
    public int songLoopCount;
    public MusicType musicType;
}
public enum MusicType
{
    Menu,
    GameMediumTempo,
    GameFastTempo
}