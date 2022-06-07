using System;
using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Enemy;
using Content.Scripts.Player;
using Content.Scripts.Variables;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.Mechanics
{
    public class Rewinder : MonoBehaviour
    {
        private enum RewindObjType
        {
            Object,
            Player,
            Enemy
        }

        [SerializeField] private RewindObjType Type;
        [SerializeField] private FloatVariable saveInterval;
        [SerializeField] private FloatVariable maxRememberedTime;


        private YieldInstruction _saveTimeYieldInstruction;
        private int _maxEntries;
        private float _interval;
        private bool _rewinding;
        private Animator _animator;
        private PlayerScript player;
        private EnemyHealth hp;

        private LinkedList<TimeEntry> _savedTimeLinkedList;

        private Action AddEntry = delegate {};
        private Action RewindEnd = delegate {};
        private Action ApplyEntry = delegate {};



        private void Awake()
        {
            _savedTimeLinkedList = new LinkedList<TimeEntry>();
            _interval = saveInterval.Value;
            _saveTimeYieldInstruction = new WaitForSeconds(saveInterval.Value);
            _maxEntries = (int)(maxRememberedTime.Value / saveInterval.Value);

            ManaBarHUD.OnRewindChange += Switch;
            Controller.OnRewind += Switch;

            switch (Type)
            {
                case RewindObjType.Object:
                    AddEntry = AddObject;
                    ApplyEntry = delegate { _savedTimeLinkedList.Last.Value.Apply(gameObject, _interval); };
                    break;
                case RewindObjType.Enemy:
                    _animator = GetComponentInChildren<Animator>();
                    hp = GetComponent<EnemyHealth>();
                    AddEntry = AddEnemy;
                    ApplyEntry = ApplyEnemy;
                    RewindEnd = RestoreEnemy;
                    break;
                case RewindObjType.Player:
                    _animator = GetComponentInChildren<Animator>();
                    player = GetComponent<PlayerScript>();
                    AddEntry = AddPlayer;
                    RewindEnd = RestorePlayer;
                    ApplyEntry = delegate { _savedTimeLinkedList.Last.Value.Apply(gameObject, _interval); };
                    break;
            }
        }

        private void OnDestroy()
        {
            ManaBarHUD.OnRewindChange -= Switch;
            Controller.OnRewind -= Switch;
        }

        private IEnumerator Start()
        {
            while (true)
            {
                if (_rewinding)
                {
                    if (_savedTimeLinkedList.Count > 1)
                    {
                        ApplyEntry();
                        _savedTimeLinkedList.Last.List.RemoveLast();
                    }
                    yield return _saveTimeYieldInstruction;
                }
                else
                {
                    AddEntry();
                    if (_savedTimeLinkedList.Count > _maxEntries) _savedTimeLinkedList.RemoveFirst();
                    yield return _saveTimeYieldInstruction;
                }
            }
        }
        private void Switch(bool rewind)
        {
            _rewinding = rewind;
            if (!_rewinding) RewindEnd();
        }


        #region Delegate fillers

        void AddPlayer() => _savedTimeLinkedList.AddLast(new PlayerTimeEntry
        {
            Pos = transform.position,
            Rot = transform.rotation,
            Alive = player.IsAlive,
            Anim = _animator.GetCurrentAnimatorStateInfo(0)
        });
        void AddEnemy() => _savedTimeLinkedList.AddLast(new EnemyTimeEntry
        {
            Pos = transform.position,
            Rot = transform.rotation,
            HP = hp.CurrHealth,
            Anim = _animator.GetCurrentAnimatorStateInfo(0)
        });

        void AddObject() =>
            _savedTimeLinkedList.AddLast(new TimeEntry {Pos = transform.position, Rot = transform.rotation});

        void RestoreEnemy()
        {
            _animator.enabled = true;
            _animator.Update(0f);
            var state = ((EnemyTimeEntry) _savedTimeLinkedList.Last.Value).Anim;
            _animator.Play(state.fullPathHash, -1, state.normalizedTime);
            _animator.Update(0f);
            _savedTimeLinkedList.RemoveLast();
        }

        void RestorePlayer()
        {
            _animator.enabled = true;
            var state = ((PlayerTimeEntry)_savedTimeLinkedList.Last.Value).Anim;
            _animator.Play(state.fullPathHash, -1, state.normalizedTime);
            _animator.Update(0f);
            _animator.Update(0f);
            if (((PlayerTimeEntry) _savedTimeLinkedList.Last.Value).Alive)
                player.IsAlive = true;
            else
            {
                //TODO stay dead logic
            }
            _savedTimeLinkedList.RemoveLast();
        }

        void ApplyEnemy()
        {
            var entry = (EnemyTimeEntry) _savedTimeLinkedList.Last.Value;
            hp.CurrHealth = entry.HP;
            entry.Apply(gameObject, _interval);
        }


        #endregion
        


        private class TimeEntry
        {
            public Vector3 Pos { get; set; }
            public Quaternion Rot { get; set; }

            public void Apply(GameObject gObj, float time)
            {
                gObj.transform.DOMove(Pos, time);
                gObj.transform.DORotateQuaternion(Rot, time);
            }

        }
        private class PlayerTimeEntry : TimeEntry
        {
            public bool Alive { get; set; }
            public AnimatorStateInfo Anim { get; set; }
        }
        private class EnemyTimeEntry : TimeEntry
        {
            public AnimatorStateInfo Anim { get; set; }
            public int HP { get; set; }
        }
    }
}