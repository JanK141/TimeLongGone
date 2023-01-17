using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.Camera;
using DG.Tweening;
using Enemy;
using Player;
using Player.States;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialScript : MonoBehaviour
{
    #region player

    private Player.Player _player;
    private GameObject _playerObject;
    private Transform _playerBeginTransform;
    private PlayerTimeControl _playerTimeControl;
    private PlayerHitHandler _playerHitHandler;
    private Animator _playerAnimator;
    private PlayerInput _playerInput;

    #endregion

    #region enemy

    private Enemy1 _enemy;
    private IEnemy _iEnemy;
    private GameObject _enemyObject;
    private Transform _enemyBeginTransform;
    private Animator _enemyAnimator;
    private float _currentEnemyHealth;

    #endregion

    #region gui

    private TutorialUI _tutorialUI;

    #endregion

    #region Coroutines

    private int _currentCoroutine;
    private bool _isSwitchedInEditor;

    private readonly List<string> _tutorialCoroutines = new List<string>
    {
        "Intro",
        "Movement",
        "Attack",
        "CombosAndFinishers",
        "ChargeAttack",
        "Blocks",
        "DodgeAndParry",
        "Kicks",
        "Finisher",
        "TimeRewind",
        "TimeDilution",
        "EndTutorial"
    };

    #endregion

    #region event Function

    private void Start()
    {
        _tutorialUI = GameObject.FindGameObjectWithTag("Screen").GetComponent<TutorialUI>();

        _enemyObject = GameObject.FindGameObjectWithTag("Enemy");
        _enemyBeginTransform = _enemyObject.transform;
        _enemy = _enemyObject.GetComponent<Enemy1>();
        _enemyAnimator = GameObject.Find("Boss1").GetComponent<Animator>();
        _currentEnemyHealth = _enemy.GetMaxHealth;
        _iEnemy = FindObjectsOfType<MonoBehaviour>().OfType<IEnemy>().SingleOrDefault();

        _playerObject = GameObject.FindGameObjectWithTag("Player");
        _player = _playerObject.GetComponent<Player.Player>();
        _playerBeginTransform = _playerObject.transform;
        _playerTimeControl = _playerObject.GetComponent<PlayerTimeControl>();
        _playerHitHandler = _player.GetComponent<PlayerHitHandler>();
        _playerAnimator = GameObject.Find("MainHero").GetComponent<Animator>();
        _playerInput = _playerObject.GetComponent<PlayerInput>();

        _enemy.StopAgent();
        SetEnemyAi(false);

        StartCoroutine(Intro());
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

    #endregion

    # region Tutorial functions

    private void SetEnemyAi(bool value) => _iEnemy.ActiveAI = value;

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

    private bool IsEnemyHitedWhenPlayer(IPlayerState state)
    {
        if (_player.CurrentState != state ||
            !(_enemy.Health < _currentEnemyHealth)) return false;

        _currentEnemyHealth = _enemy.Health;
        return true;
    }

    private bool IsPlayerNearBoss(float distance = 3) =>
        Vector3.Distance(
            _enemyObject.transform.position,
            _playerObject.transform.position
        )
        <= distance;

    private static bool IsObjectFacedTo(Transform character, Transform target, int fieldOfView = 90)
    {
        var directionToTarget = target.position - character.position;
        var angle = Vector3.Angle(character.forward, directionToTarget);
        return angle < fieldOfView;
    }

    private static bool IsAnimationFinish(Animator animator, string animationName, float moment = 1) =>
        animator.GetCurrentAnimatorStateInfo(0).IsName(animationName)
        && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= moment;

    #region IEnumerator functions

    private IEnumerator ShowingWindow()
    {
        Pausing(true);
        _playerInput.enabled = false;
        _tutorialUI.RunTutorialWindow(_currentCoroutine);
        yield return new WaitUntil(_tutorialUI.IsClicked);
        Pausing(false);
        _playerInput.enabled = true;
    }

    private IEnumerator WaitingGameForPlayerInput(InputIntermediary.InputContext input)
    {
        Pausing(true);
        yield return new WaitUntil(() => _player.inputContext == input);
        Pausing(false);
    }

    #endregion

    #endregion

// Not Working
    private void ChangeCoroutine(int step)
    {
        // StopCoroutine(_tutorialCoroutines[_currentCoroutine]);
        StopAllCoroutines();
        var index = _currentCoroutine + step;
        if (index >= 0 && index < _tutorialCoroutines.Count)
        {
            _currentCoroutine += step;

            var tutorialFaze = _tutorialCoroutines[_currentCoroutine];
            _tutorialUI.CloseTutorial();
            StartCoroutine(tutorialFaze);
            _isSwitchedInEditor = true;
            print("current faze " + tutorialFaze + " -> " + _tutorialUI.GetInstruction(_currentCoroutine));
            Pausing(true);

            void ReSpawnCharacters()
            {
                // BUG: player cannot change position
                if (tutorialFaze == "Intro")
                    _playerObject.transform.position = _playerBeginTransform.position;
                else
                    _playerObject.transform.position = _enemyBeginTransform.position - new Vector3(0, 0, 2);

                _enemyObject.transform.position = _enemyBeginTransform.position;
                _enemy.LookAtPlayer(0);
            }

            ReSpawnCharacters();

            _player.CurrentState = _player.IDLE_STATE;
            CinemachineSwitcher.Instance.Switch(false);

            switch (tutorialFaze)
            {
                case "TimeRewind":
                case "TimeDilution":
                case "EndTutorial":
                    SetEnemyAi(true);
                    _player.hitHandler.ReceiveHitActive = true;
                    break;
                default:
                    SetEnemyAi(false);
                    _player.hitHandler.ReceiveHitActive = false;
                    break;
            }
        }
        else
            print("index out of the range " + index);
    }

    #region TutorialFazes(coroutines)

    private IEnumerator Intro()
    {
        // intro window run in TutorialUI in start() 
        _playerInput.enabled = false;
        yield return new WaitUntil(_tutorialUI.IsClicked);
        _playerInput.enabled = true;

        _currentCoroutine++;
        StartCoroutine(Movement());
    }

    private IEnumerator Movement()
    {
        yield return ShowingWindow();

        yield return new WaitUntil(() => IsPlayerNearBoss(distance: 15));

        _enemy.LookAtPlayer(4);

        _currentCoroutine++;
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        yield return ShowingWindow();

        yield return new WaitUntil(() => IsPlayerNearBoss());

        yield return new WaitUntil(
            () => IsEnemyHitedWhenPlayer(_player.ATTACK_STATE)
        );

        yield return new WaitUntil(() => IsAnimationFinish(_playerAnimator, "Attack1"));

        _currentCoroutine++;
        StartCoroutine(CombosAndFinishers());
    }

    private IEnumerator CombosAndFinishers()
    {
        yield return ShowingWindow();

        yield return new WaitUntil(() => IsPlayerNearBoss());

        var attackCount = 0;
        while (attackCount < 3)
        {
            _enemy.LookAtPlayer(2);

            yield return new WaitUntil(() =>
                IsEnemyHitedWhenPlayer(_player.ATTACK_STATE)
                && IsObjectFacedTo(
                    _playerObject.transform,
                    _enemyObject.transform,
                    60)
            );
            attackCount++;
        }

        _enemy.LookAtPlayer(1);

        void RotatePlayerToEnemy(float timeToRotate)
        {
            var enemyPosition = _enemyObject.transform.position;
            _playerObject.transform.DOLookAt(
                new Vector3(enemyPosition.x, _playerObject.transform.position.y, enemyPosition.z), timeToRotate);
        }

        RotatePlayerToEnemy(1);

        _currentCoroutine++;
        StartCoroutine(ChargeAttack());
    }

    private IEnumerator ChargeAttack()
    {
        _enemyAnimator.Play("Repulse");
        yield return new WaitUntil(() => IsAnimationFinish(_enemyAnimator, "Repulse"));

        yield return ShowingWindow();

        yield return WaitingGameForPlayerInput(InputIntermediary.InputContext.ChargeCanceled);

        // yield return new WaitUntil(() => IsAnimationFinish(_playerAnimator, "ChargeAttack"));
        yield return new WaitUntil(() => IsPlayerNearBoss());

        _currentCoroutine++;
        StartCoroutine(Blocks());
    }

    private IEnumerator Blocks()
    {
        const string enemyAttackAnimationName = "BasicAttack1";
        _enemy.LookAtPlayer(0.5f);
        _enemyAnimator.Play(enemyAttackAnimationName);
        yield return new WaitUntil(() => IsAnimationFinish(_enemyAnimator, enemyAttackAnimationName, 0.6f));

        yield return ShowingWindow();

        yield return WaitingGameForPlayerInput(InputIntermediary.InputContext.BlockPerformed);

        yield return new WaitUntil(() => IsAnimationFinish(_enemyAnimator, enemyAttackAnimationName));


        _currentCoroutine++;
        StartCoroutine(DodgeAndParry());
    }

    private IEnumerator DodgeAndParry()
    {
        const string enemyAttackAnimationName = "BasicAttack2";

        yield return new WaitUntil(() => IsPlayerNearBoss());

        _enemyAnimator.Play(enemyAttackAnimationName);
        yield return new WaitUntil(() => IsAnimationFinish(_enemyAnimator, enemyAttackAnimationName, 0.5f));

        yield return ShowingWindow();

        yield return WaitingGameForPlayerInput(InputIntermediary.InputContext.BlockStarted);

        yield return new WaitUntil(() => IsAnimationFinish(_enemyAnimator, enemyAttackAnimationName));

        _currentCoroutine++;
        StartCoroutine(Kicks());
    }

    private IEnumerator Kicks()
    {
        _enemyAnimator.Play("Parried");
        yield return new WaitUntil(() => IsAnimationFinish(_enemyAnimator, "Parried", 0.3f));

        yield return ShowingWindow();
        yield return
            WaitingGameForPlayerInput(InputIntermediary.InputContext.Stun); // BUG: no stun animation after input 
        print("kicked");

        yield return new WaitUntil(() => IsAnimationFinish(_playerAnimator, "Stun", 0.4f));

        _currentCoroutine++;
        StartCoroutine(Finisher());
    }

    private IEnumerator Finisher()
    {
        _enemyAnimator.Play("StunStart");
        CinemachineSwitcher.Instance.Switch(true);

        yield return new WaitUntil(() => IsAnimationFinish(_enemyAnimator, "StunStart", 0.9f));

        yield return ShowingWindow();

        var attackCount = 0;
        while (attackCount < 5)
        {
            yield return new WaitUntil(
                () => IsEnemyHitedWhenPlayer(_player.ATTACK_STATE)
            );
            attackCount++;
        }

        _player.hitHandler.ReceiveHitActive = true;

        yield return new WaitUntil(() => IsAnimationFinish(_playerAnimator, "HeavyAttack", 0.1f)
                                         && IsPlayerNearBoss()
        );

        CinemachineSwitcher.Instance.Switch(false);

        _enemy.PlayAnimation("StunEnd", 0.1f);
        yield return new WaitUntil(() => IsAnimationFinish(_enemyAnimator, "StunEnd"));


        _currentCoroutine++;
        StartCoroutine(TimeRewind());
    }

    private IEnumerator TimeRewind()
    {
        _enemy.BasicAttack();

        _player.hitHandler.ReceiveHitActive = true;

        SetEnemyAi(true);

        yield return new WaitUntil(() => IsAnimationFinish(_playerAnimator, "Death", 0.5f));

        yield return ShowingWindow();

        yield return new WaitUntil(() => _player.IsRewinding.Value);

        yield return new WaitUntil(() => !_player.IsRewinding.Value);

        _currentCoroutine++;
        StartCoroutine(TimeDilution());
    }

    private IEnumerator TimeDilution()
    {
        yield return ShowingWindow();

        SetEnemyAi(true);

        _playerTimeControl.Mana = _playerTimeControl.MaxMana;

        yield return WaitingGameForPlayerInput(InputIntermediary.InputContext.RewindStarted);

        yield return new WaitUntil(() =>  Time.timeScale > _playerTimeControl.getTargetSlowMoScale);
        _currentCoroutine++;
        StartCoroutine(EndTutorial());
    }

    private IEnumerator EndTutorial()
    {
        yield return ShowingWindow();

        _playerTimeControl.Mana = _playerTimeControl.MaxMana;

        yield return new WaitUntil(() => Input.anyKey);
    }

    #endregion
}