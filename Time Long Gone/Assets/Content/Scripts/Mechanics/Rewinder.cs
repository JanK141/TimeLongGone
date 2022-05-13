using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Variables;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.Mechanics
{
    public class Rewinder : MonoBehaviour
    {
        [SerializeField] private FloatVariable saveInterval;
        [SerializeField] private FloatVariable maxRememberedTime;


        private YieldInstruction _saveTimeYieldInstruction;
        private int _maxEntries;
        private float _interval;
        private bool _rewinding;
        private Animator _animator;

        private LinkedList<TimeEntry> _savedTimeLinkedList;


        #region Unity Event Functions

        private void Awake()
        {
            _savedTimeLinkedList = new LinkedList<TimeEntry>();
            _interval = saveInterval.Value;
            _saveTimeYieldInstruction = new WaitForSeconds(saveInterval.Value);
            _maxEntries = (int)(maxRememberedTime.Value / saveInterval.Value);
            _animator = GetComponentInChildren<Animator>();
            ManaBarHUD.OnRewindChange += Switch;
            Controller.OnRewind += Switch;
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
                    if (_savedTimeLinkedList.Count > 0)
                    {
                        _savedTimeLinkedList.Last?.Value.Apply(gameObject);

                        var t = transform;
                        var startPos = t.position;
                        var startRot = t.rotation;
                        for (var i = 1; i <= 30; i++)
                        {
                            var lastValue = _savedTimeLinkedList.Last!.Value;
                            var interpolationPoint = (float)i / 30;
                            transform.position = Vector3.Lerp(startPos, lastValue.Pos, interpolationPoint);
                            transform.rotation = Quaternion.Lerp(startRot, lastValue.Rot, interpolationPoint);
                        }

                        if (!_animator.GetCurrentAnimatorStateInfo(0).fullPathHash
                                .Equals(_savedTimeLinkedList.Last!.Value.Anim.fullPathHash))
                            _animator.Play(_savedTimeLinkedList.Last.Value.Anim.fullPathHash, -1);

                        _savedTimeLinkedList.Last?.List.RemoveLast();
                    }

                    _savedTimeLinkedList.Last?.Value.Apply(gameObject, _interval);
                    yield return _saveTimeYieldInstruction;
                }
                else
                {
                    var t = transform;
                    _savedTimeLinkedList.AddLast(new TimeEntry
                        {
                            Pos = t.position,
                            Rot = t.rotation,
                            Anim = _animator.GetCurrentAnimatorStateInfo(0)
                        }
                    );
                    if (_savedTimeLinkedList.Count >= _maxEntries) _savedTimeLinkedList.RemoveFirst();
                    yield return _saveTimeYieldInstruction;
                }
            }
        }

        #endregion

        private void Switch(bool rewind)
        {
            _rewinding = rewind;
            if (_savedTimeLinkedList.Count > 0)
                _animator.Play(_savedTimeLinkedList.Last.Value.Anim.fullPathHash,
                    -1,
                    1 - _savedTimeLinkedList.Last.Value.Anim.normalizedTime);
        }


        private class TimeEntry
        {
            public Vector3 Pos { get; set; }
            public Quaternion Rot { get; set; }
            public AnimatorStateInfo Anim { get; set; }

            public void Apply(GameObject gObj, float time)
            {
                gObj.transform.DOMove(Pos, time);
                gObj.transform.DORotateQuaternion(Rot, time);
            }

            public void Apply(GameObject gObj)
            {
                gObj.transform.position = Pos;
                gObj.transform.rotation = Rot;
            }
        }
    }
}