using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Rewinder : MonoBehaviour
{
    [SerializeField] private FloatVariable SaveInterval;
    [SerializeField] private FloatVariable MaxRememberedTime;


    private YieldInstruction _saveTime;
    private int _maxEntries;
    private float _interval;
    private bool _rewinding = false;
    private Animator _animator;

    private LinkedList<TimeEntry> savedTime;


    void Awake()
    {
        savedTime = new LinkedList<TimeEntry>();
        _interval = SaveInterval.Value;
        _saveTime = new WaitForSeconds(SaveInterval.Value);
        _maxEntries = (int)(MaxRememberedTime.Value / SaveInterval.Value);
        _animator = GetComponentInChildren<Animator>();
        ManaBarHUD.OnRewindChange += Switch;
    }

    void Switch(bool rewind)
    {
        _rewinding = rewind;
        if (savedTime.Count > 0)
        {
            _animator.Play(savedTime.Last.Value.anim.fullPathHash, -1,
                1 - savedTime.Last.Value.anim.normalizedTime);
        }
    }

    void OnDestroy() => ManaBarHUD.OnRewindChange -= Switch;

    IEnumerator Start()
    {
        while (true)
        {
            if (!_rewinding)
            {
                savedTime.AddLast(new TimeEntry{pos = transform.position, rot = transform.rotation, anim = _animator.GetCurrentAnimatorStateInfo(0)});
                if(savedTime.Count>=_maxEntries) savedTime.RemoveFirst();
                yield return _saveTime;
            }
            else
            {
                if (savedTime.Count > 0)
                {
                    savedTime.Last?.Value.Apply(gameObject);

                    if (!_animator.GetCurrentAnimatorStateInfo(0).fullPathHash
                            .Equals(savedTime.Last.Value.anim.fullPathHash))
                    {
                        _animator.Play(savedTime.Last.Value.anim.fullPathHash, -1);
                    }

                    savedTime.Last?.List.RemoveLast();
                }

                savedTime.Last?.Value.Apply(gameObject, _interval);
                yield return _saveTime;
            }
        }
    }

    private class TimeEntry
    {
        public Vector3 pos { get; set; }
        public Quaternion rot { get; set; }
        public AnimatorStateInfo anim { get; set; }

        public void Apply(GameObject t, float time)
        {
            t.transform.DOMove(pos, time);
            t.transform.DORotateQuaternion(rot, time);
        }
        public void Apply(GameObject t)
        {
            t.transform.position = pos;
            t.transform.rotation = rot;
        }
    }
}
