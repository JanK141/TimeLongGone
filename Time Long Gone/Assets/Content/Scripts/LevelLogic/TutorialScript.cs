using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enemy;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    [SerializeField] private float setDistance = 10;
    private GameLogic _gl;
    private Level1DataManager _l1Dm;
    private PausingScript _pausingScript;


    #region player

    private GameObject _playerObject;
    private Player.Player _player;

    #endregion

    #region enemy

    private GameObject _enemyObject;
    private Enemy1 _enemy1;
    private IEnemy _ienemy;
    private List<StateMachine> _enemyStateMachine;

    #endregion

    #region gui

    private string _faze;
    private string _textInstruction;

    #endregion

    private void Awake() => _pausingScript = GetComponent<PausingScript>();

    private void Start()
    {
        _enemyObject = GameObject.FindGameObjectWithTag("Enemy");
        _enemy1 = _enemyObject.GetComponent<Enemy1>();
        _ienemy = FindObjectsOfType<MonoBehaviour>().OfType<IEnemy>().SingleOrDefault();
        _playerObject = GameObject.FindGameObjectWithTag("Player");
        _l1Dm = Level1DataManager.Instance;
        _gl = GameLogic.Instance;
        _enemyStateMachine = _l1Dm.Enemy1StateMachines;

        SetEnemyAI(false);
        StartCoroutine(Movement());
    }

    private void SetEnemyAI(bool value) => _ienemy.ActiveAI = value;

    private void OnGUI()
    {
        GUI.Label(
            new Rect(30, 30, 200, 200),
            text: _faze
        );

        GUI.Label(
            new Rect(50, 50, 200, 200),
            text: _textInstruction
        );
    }


    private IEnumerator Movement()
    {
        _faze = "Movement";
        _textInstruction = "Move to Enemy by WSAD";

        yield return new WaitUntil(() => Vector3.Distance(
                                             _enemyObject.transform.position,
                                             _playerObject.transform.position
                                         )
                                         <= setDistance);

        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        _faze = "Attack";
        _textInstruction = "Use LMB/X/Square to perform an attack.\n" +
                           "Press the Attack button now on Enemy";

        yield return new WaitUntil(
            () => _enemy1.Health < _enemy1.GetMaxHealth
        );

        StartCoroutine(CombosAndFinishers());
    }

    private IEnumerator CombosAndFinishers()
    {
        var attackCount = 0;
        const int maxCount = 3;
        _faze = "ComboAndFinisher";
        _textInstruction = "You can press it quickly or hold it \n " +
                           "to perform a set sequence of them, with the last attack dealing bonus  damage \n";


        while (attackCount < maxCount)
        {
            var currHealth =  _enemy1.Health;
            yield return new WaitUntil(
                () => _enemy1.Health < currHealth
            );
            attackCount++;
            _textInstruction =  "attack done " + attackCount + " \n";
        }

        StartCoroutine(Blocks());
    }


    private IEnumerator Blocks()
    {
        _faze = "Blocks";
        _textInstruction = "Use block to block enemy attack";
        _enemy1.LookAtPlayer(1);
        _enemy1.FindEscapeTarget();

        // // todo: gra pauzuje do momentu kiedy gracz przytrzyma długo blok
        //
        // _pausingScript.Pausing();
        //
        // yield return new WaitUntil(() => _playerScript.combat.IsBlocking
        //                                  && _playerScript.combat.BlockPressTime >= 3);
        // _pausingScript.Unpausing();
        // StartCoroutine(DodgeAndParry);
        yield return null;
    }

    // private IEnumerator DodgeAndParry()
    // {
    //     _faze = "DodgeAndParry";
    //     _level1DataManager.move.WalkTo(_enemy.transform.position);
    //     _level1DataManager.anim.Play("Attack");
    //
    //     yield return new WaitUntil(() => _playerScript.combat.IsBlocking);
    //     StartCoroutine(Kicks());
    // }
    //
    // private IEnumerator Kicks()
    // {
    //     _faze = "Kicks";
    //     yield return new WaitUntil(() => _playerScript.combat.IsBlocking
    //     );
    //     StartCoroutine(ChargeAttack());
    // }
    //
    // private IEnumerator ChargeAttack()
    // {
    //     _faze = "ChargeAttack";
    //     yield return new WaitUntil(() => _playerScript.combat.IsCharging);
    //     StartCoroutine(GoFourth());
    // }
    //
    // private IEnumerator GoFourth()
    // {
    //     _faze = "GoFourth";
    //     yield return new WaitUntil(() => Input.anyKey);
    //     SetEnemyAI(true);
    // }
}