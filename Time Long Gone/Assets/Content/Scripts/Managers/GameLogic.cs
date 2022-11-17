using Content.Scripts;
using Content.Scripts.Variables;
using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// This class provides global access to data that dictates how game behaves as a whole (not level specific data).
/// </summary>
public class GameLogic : MonoBehaviour
{
    [SerializeField] private FloatVariable timeToRemember;
    [SerializeField] private FloatVariable timeBetweenEntries;
    [SerializeField] private BoolVariable isRewinding;
    [SerializeField][Tooltip("Easy, Normal, Hard, Very Hard")] private PlayerVariables[] playerVariables = { null, null, null, null };

    private static GameLogic _i;

    public static GameLogic Instance
    {
        get
        {
            if (_i == null) _i = (Instantiate(Resources.Load("Managers/Game Logic")) as GameObject).GetComponent<GameLogic>();
            return _i;
        }
    }
    public FloatVariable TimeToRemember => timeToRemember;
    public FloatVariable TimeBetweenEntries => timeBetweenEntries;
    public BoolVariable IsRewinding => isRewinding;
    public PlayerVariables PlayerVariables { get{
            return playerVariables[0];
            //TODO
            // return playerVariables[GameManager.Instance.GameDifficulty];
            //or smth like that
        }}

    private void Awake()
    {
        timeToRemember.ResetToOrigin();
        timeBetweenEntries.ResetToOrigin();
        isRewinding.ResetToOrigin();
    }
    private void OnValidate()
    {
        if(playerVariables.Length != 4)
        {
            Array.Resize(ref playerVariables, 4);
        }
    }
}
