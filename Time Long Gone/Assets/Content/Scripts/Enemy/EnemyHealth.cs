using System;
using Content.Scripts.Enemy;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    #region Inspector fields
    [SerializeField] [Min(1)] private int maxHealth = 100;
    [SerializeField] [Min(1)] [Tooltip("How many combat stages boss has (divided by % of hp) ")] private int stages = 1;
    #endregion

    #region Private variables
    private int _currHealth;
    private int _currStage;
    private float[] _stageChangers;
    private EnemyScript _enemy;
    #endregion

    #region Properties
    public int CurrHealth
    {
        get => _currHealth;
        set
        {
            _currHealth = value;
            UpdateHealth();
        }
    }

    public int MaxHealth => maxHealth;
    public int CurrStage => _currStage;
    #endregion


    private void Start()
    {
        _enemy = EnemyScript.Instance;
        _currHealth = maxHealth;
        _currStage = 1;
        _stageChangers = new float[stages];
        for (var i = 0; i < stages; i++)
        {
            _stageChangers[i] = i + 1 == stages ? 0 : maxHealth - (maxHealth / stages) * (i + 1);
        }
    }

    private void UpdateHealth()
    {
        if (_currHealth <= 0) Death();
        else if (_currHealth <= _stageChangers[_currStage - 1])
        {
            _currStage++;
            //TODO next combat stage sequence
        }
    }

    private void Death()
    {
        print("You won the level!");
        //TODO start level end sequence
        Destroy(gameObject);
    }
}