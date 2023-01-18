using Content.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class LevelDataManager : MonoBehaviour
{
    [SerializeField] protected List<Sound> clips;
    [SerializeField][Tooltip("Each entry is for diffirent difficulty level")] protected List<ListContainer> enemyStateMachines = new List<ListContainer> {new ListContainer(), new ListContainer(), new ListContainer(), new ListContainer(), new ListContainer()};
    [SerializeField] protected List<float> enemyHealthPoints = new List<float> {0, 0 ,0 ,0, 0};

    protected static LevelDataManager _i;
    public static LevelDataManager Instance { get; }
    public float EnemyHealth { get => (enemyHealthPoints[GameManager.Instance.DifficultyLevel] == 0) ? 
            enemyHealthPoints[0] : enemyHealthPoints[GameManager.Instance.DifficultyLevel]; }
    public List<StateMachine> EnemyStateMachines
    {
        get
        {
            return (enemyStateMachines[GameManager.Instance.DifficultyLevel].stages == null)?
                enemyStateMachines[0].stages:
                enemyStateMachines[GameManager.Instance.DifficultyLevel].stages;
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
