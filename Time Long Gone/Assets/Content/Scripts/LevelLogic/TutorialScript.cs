using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enemy;
using Player;
using Player.States;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
    #region player

    private GameObject _playerObject;
    private Player.Player _player;
    private PlayerTimeControl _playerTimeControl;
    private PlayerHitHandler _playerHitHandler;
    private Animator _playerAnimator;

    #endregion

    #region enemy

    private GameObject _enemyObject;
    private Enemy1 _enemy;
    private IEnemy _iEnemy;
    private StateMachine _enemyStateMachine;
    private Animator _enemyAnimator;
    private float _currentEnemyHealth;

    #endregion

    #region gui

    private string _faze;
    private string _textInstruction;

    #endregion

    #region Coroutines

    private int _currentCoroutine;

    private readonly List<string> _tutorialCoroutines = new List<string>
    {
        "Movement",
        "Attack",
        "CombosAndFinishers",
        "ChargeAttack",
        "Blocks",
        "DodgeAndParry",
        "Kicks",
        "Finisher",
        "GoFourth",
        "TimeDilution",
        "EndTutorial"
    };

    #endregion

    #region event Function

    private void Start()
    {
        _enemyObject = GameObject.FindGameObjectWithTag("Enemy");
        _enemy = _enemyObject.GetComponent<Enemy1>();
        _enemyAnimator = GameObject.Find("Boss").GetComponent<Animator>();
        _currentEnemyHealth = _enemy.GetMaxHealth;
        _iEnemy = FindObjectsOfType<MonoBehaviour>().OfType<IEnemy>().SingleOrDefault();

        _playerObject = GameObject.FindGameObjectWithTag("Player");
        _player = _playerObject.GetComponent<Player.Player>();
        _playerAnimator = GameObject.Find("model").GetComponent<Animator>();
        _playerTimeControl = _playerObject.GetComponent<PlayerTimeControl>();
        _playerHitHandler = _player.GetComponent<PlayerHitHandler>();

        _player.hitHandler.ReceiveHitActive = false;
        _enemy.StopAgent();
        SetEnemyAI(false);
        StartCoroutine(Movement());
    }

    // for debugging 
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
            Pausing(false);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            ChangeCoroutine(step: 1);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            ChangeCoroutine(step: -1);
    }

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

    #endregion

    private void SetEnemyAI(bool value) => _iEnemy.ActiveAI = value;

    private void Pausing(bool pausing)
    {
        if (pausing)
        {
            _playerTimeControl.ActiveTime = false;
            Time.timeScale = 0;
        }
        else
        {
            _playerTimeControl.ActiveTime = true;
            Time.timeScale = 1;
        }
    }

    private bool EnemyIsHitedWhen(IPlayerState playerState)
    {
        if (_player.CurrentState != playerState ||
            !(_enemy.Health < _currentEnemyHealth)) return false;

        _currentEnemyHealth = _enemy.Health;
        return true;
    }

    private bool IsPlayerNearBoss() =>
        Vector3.Distance(
            _enemyObject.transform.position,
            _playerObject.transform.position
        )
        <= 3;


    #region ChangingCoroutine(Develop)

    private void ChangeCoroutine(int step)
    {
        // StopCoroutine(_tutorialCoroutines[_currentCoroutine]);
        StopAllCoroutines();
        var index = _currentCoroutine + step;
        if (index < 0 || index >= _tutorialCoroutines.Count)
            print("index out of the range " + index);
        else
        {
            _currentCoroutine += step;
            StartCoroutine(_tutorialCoroutines[_currentCoroutine]);
            Pausing(true);
            if (_currentCoroutine == 0) return;

            _player.hitHandler.ReceiveHitActive = false;
            SpawnBossNearPlayer();
            _player.CurrentState = _player.IDLE_STATE;

            if (_tutorialCoroutines[_currentCoroutine] != "EndTutorial")
                SetEnemyAI(false);
        }
    }

    private void SpawnBossNearPlayer()
    {
        var playerPosition = _playerObject.transform.position;
        _enemyObject.transform.position = playerPosition + new Vector3(0, 0, 2);
        _enemy.LookAtPlayer(0);
    }

    #endregion

    #region TutorialFazes(coroutines)

    private IEnumerator Intro()
    {
        _faze = "intro";
        _textInstruction = "Welcome to “Time Long Gone”! If it’s your first time playing," +
                           " we recommend you follow the Banished in a quick review of his skills and " +
                           "capabilities during the tutorial phase. If you already know all the basics and intricacies of combat, " +
                           "feel free to skip it to get straight into the action!";

        yield return new WaitForSeconds(1);
    }

    private IEnumerator Movement()
    {
        _faze = "Movement";
        _textInstruction = "Move to Enemy by WSAD";

        yield return new WaitUntil(IsPlayerNearBoss);

        _enemy.LookAtPlayer(4);
        _currentCoroutine++;
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        _faze = "Attack";
        _textInstruction = "Use LMB/X/Square to perform an attack.\n" +
                           "Press the Attack button now on Enemy";

        yield return new WaitUntil(IsPlayerNearBoss);

        yield return new WaitUntil(
            () => EnemyIsHitedWhen(_player.ATTACK_STATE)
        );
        _currentCoroutine++;
        StartCoroutine(CombosAndFinishers());
    }

    private IEnumerator CombosAndFinishers()
    {
        _faze = "ComboAndFinisher";
        _textInstruction = "You can press it quickly or hold it \n " +
                           "to perform a set sequence of them, with the last attack dealing bonus  damage \n";

        yield return new WaitUntil(IsPlayerNearBoss);

        var attackCount = 0;

        while (attackCount < 3)
        {
            yield return new WaitUntil(() => EnemyIsHitedWhen(_player.ATTACK_STATE));
            attackCount++;
            _textInstruction = "attack done " + attackCount + " \n";
        }

        _currentCoroutine++;
        StartCoroutine(ChargeAttack());
    }

    private IEnumerator ChargeAttack()
    {
        _faze = "ChargeAttack";
        _textInstruction = "hold LPM";

        yield return new WaitUntil(IsPlayerNearBoss);

        _enemyAnimator.Play("Repulse");
        var direction = _playerObject.transform.position - _enemy.transform.position;
        direction.y = 0f;
        float time = 0;
        const float pushingFactor = 1f;
        var initVel = _player.variables.pushVelocity * pushingFactor;
        var pushCurve = _playerHitHandler.GetPushCurve;
        while (time < _player.variables.pushTime)
        {
            _player.velocity += direction * (pushCurve.Evaluate(time / _player.variables.pushTime) * initVel);
            if (time < _player.variables.pushTime / 6)
                _player.velocity.y = pushCurve.Evaluate(time / _player.variables.pushTime) * direction.y;
            time += Time.deltaTime;
            yield return null;
        }

        yield return new WaitUntil(() => _enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Repulse")
                                         && _enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f
        );
        Pausing(true);
        yield return new WaitUntil(() => _player.inputContext == InputIntermediary.InputContext.ChargeCanceled);
        Pausing(false);

        _currentCoroutine++;
        StartCoroutine(Blocks());
    }

    private IEnumerator Blocks()
    {
        _faze = "Blocks";
        _textInstruction = "Use block to block enemy attack";
        yield return new WaitUntil(IsPlayerNearBoss);
        _enemy.BasicAttack();
        yield return new WaitUntil(() => _enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("BasicAttack1")
                                         && _enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f
        );

        Pausing(true);
        yield return new WaitUntil(() => _player.inputContext == InputIntermediary.InputContext.BlockStarted);
        Pausing(false);

        _currentCoroutine++;
        StartCoroutine(DodgeAndParry());
    }

    private IEnumerator DodgeAndParry()
    {
        _faze = "DodgeAndParry";
        _textInstruction = "parry enemy attack";
        yield return new WaitUntil(IsPlayerNearBoss);
        _enemy.BasicAttack();

        yield return new WaitUntil(() => _enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("BasicAttack2")
                                         && _enemyAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f
        );

        Pausing(true);
        print("pause");
        yield return new WaitUntil(() => _player.inputContext == InputIntermediary.InputContext.BlockStarted);
        print("block pressed");
        Pausing(false);

        _currentCoroutine++;
        StartCoroutine(Kicks());
    }

    private IEnumerator Kicks()
    {
        _faze = "Kicks";
        _textInstruction = "press E to kick";

        yield return new WaitUntil(IsPlayerNearBoss);

        _enemy.ChangeEnemyStatus(EnemyStatus.Vulnerable);

        yield return new WaitUntil(() =>
            _player.CurrentState == _player.STUN_STATE && IsPlayerNearBoss()
        );

        _enemy.ReceiveStun();
        _enemyAnimator.Play("StunStart");
        _currentCoroutine++;
        StartCoroutine(Finisher());
    }

    private IEnumerator Finisher()
    {
        _faze = "Finisher";
        _textInstruction = "Build up your combo meter up to 5 and";
        var attackCount = 0;
        while (attackCount < 5)
        {
            yield return new WaitUntil(
                () => EnemyIsHitedWhen(_player.ATTACK_STATE)
            );
            attackCount++;
            _textInstruction = "attack done " + attackCount + " \n";
        }

        _textInstruction = "then use the Q key to perform the Finisher and deal extra damage!";
        _player.hitHandler.ReceiveHitActive = true;
        _enemyAnimator.Play("StunEnd");

        yield return new WaitUntil(() => EnemyIsHitedWhen(_player.FINISHER_STATE));
        _enemyAnimator.Play("StunEnd");


        _currentCoroutine++;
        StartCoroutine(GoFourth());
    }

    private IEnumerator GoFourth()
    {
        _faze = "GoFourth";
        _textInstruction = "press r to rewind";
        _enemy.BasicAttack();
        _player.hitHandler.ReceiveHitActive = true;

        yield return new WaitUntil(() => _player.inputContext == InputIntermediary.InputContext.RewindCanceled);
        _currentCoroutine++;
        StartCoroutine(TimeDilution());
    }

    private IEnumerator TimeDilution()
    {
        _faze = "Time Dilution";
        _textInstruction = "hold the time button";

        SetEnemyAI(true);
        _playerTimeControl.Mana = _playerTimeControl.MaxMana;

        yield return new WaitUntil(() => _player.IsRewinding.Value);
        _currentCoroutine++;
        StartCoroutine(EndTutorial());
    }

    private IEnumerator EndTutorial()
    {
        _faze = " end tutorial";
        _textInstruction = "Press any key to finish the tutorial and face the mysterious warrior in all his might! ";
        _playerTimeControl.Mana = _playerTimeControl.MaxMana;
        yield return new WaitUntil(() => Input.anyKey);
    }

    #endregion
}