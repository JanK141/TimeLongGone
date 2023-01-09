using Content.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LevelDataManager;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField][Tooltip("Reference exact names of sounds to try download from data manager")] private List<string> Sounds;

    private Dictionary<string, List<SoundContainer>> _audioSources;

    private void Awake()
    {
        _audioSources = new Dictionary<string, List<SoundContainer>>();
        foreach (string s in Sounds)
        {
            var clips = GameManager.Instance.CurrentDataManager.GetSounds(s);
            if (clips.Count > 0)
            {
                List<SoundContainer> list = new List<SoundContainer>();
                foreach (Sound sound in clips)
                {
                    var tmp = new SoundContainer();
                    tmp.source = gameObject.AddComponent<AudioSource>();
                    tmp.source.clip = sound.clip;
                    tmp.source.loop = false;
                    tmp.source.playOnAwake = false;
                    tmp.source.volume = sound.volume;
                    tmp.minPitch = sound.minPitch;
                    tmp.maxPitch = sound.maxPitch;
                    list.Add(tmp);
                }
                _audioSources.Add(s, list);
            }
        }
    }

    public void Play(string sound)
    {
        if(_audioSources.TryGetValue(sound, out var clips))
        {
            SoundContainer choosenSound;

            if (clips.Count > 1) choosenSound = clips[Random.Range(0, clips.Count)];
            else choosenSound = clips[0];

            choosenSound.source.pitch = Random.Range(choosenSound.minPitch, choosenSound.maxPitch);
            choosenSound.source.Play();
        }
    }


     private class SoundContainer
    {
        public AudioSource source;
        public float minPitch;
        public float maxPitch;
    }
}
