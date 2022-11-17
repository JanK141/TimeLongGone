using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class provides global access to data specific for Level 1
/// </summary>
public class Level1DataManager : MonoBehaviour
{
    [SerializeField][Tooltip("Each entry is for diffirent difficulty level")] private List<ListContainer> enemyStateMachines;

    private static Level1DataManager _i;

    public static Level1DataManager Instance
    {
        get
        {
            if (_i == null) _i = (Instantiate(Resources.Load("Managers/Level 1 Data Manager")) as GameObject).GetComponent<Level1DataManager>();
            return _i;
        }
    }

    public List<StateMachine> Enemy1StateMachines { get
        {
            return enemyStateMachines[0].stages;
            //TODO
            //return enemyStateMachines[GameManager.Instance.GameDifficulty].stages;
            //or smth like that
        } }

    [Serializable]
    private class ListContainer
    {
        public List<StateMachine> stages;
    }
}
