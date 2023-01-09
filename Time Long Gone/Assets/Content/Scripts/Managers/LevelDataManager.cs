using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class LevelDataManager : MonoBehaviour
{
    [SerializeField] protected List<Sound> clips;
    [SerializeField][Tooltip("Each entry is for diffirent difficulty level")] protected List<ListContainer> enemyStateMachines;

    protected static LevelDataManager _i;
    public static LevelDataManager Instance { get; }
    public List<StateMachine> EnemyStateMachines
    {
        get
        {
            return enemyStateMachines[0].stages;
            //TODO
            //return enemyStateMachines[GameManager.Instance.GameDifficulty].stages;
            //or smth like that
        }
    }
    public List<Sound> GetSounds(string name)
    {
        return clips.FindAll(x => x.name == name);
    }


    [Serializable]
    protected class ListContainer
    {
        public List<StateMachine> stages;
    }
    [Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
        public float minPitch;
        public float maxPitch;
    }
}
