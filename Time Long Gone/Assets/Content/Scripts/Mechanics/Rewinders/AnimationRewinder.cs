using Content.Scripts.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationRewinder : MonoBehaviour
{
    FloatVariable TimeToRemember;
    FloatVariable TimeBetweenEntries;
    BoolVariable IsRewinding;
    [SerializeField] List<string> AnimatorSpeedParams;

    private Animator anim;
    private int maxentries;
    private int entries;
    private LinkedList<TimeEntry> animators = new LinkedList<TimeEntry>();

    private void Awake()
    {
        IsRewinding = GameLogic.Instance.IsRewinding;
        TimeToRemember = GameLogic.Instance.TimeToRemember;
        TimeBetweenEntries = GameLogic.Instance.TimeBetweenEntries;
    }
    void Start()
    {
        anim = GetComponent<Animator>();
        entries = 0;
        maxentries = (int)(TimeToRemember.Value / TimeBetweenEntries.Value);
        StartCoroutine(Cycle());
    }
    private void OnEnable()
    {
        IsRewinding.OnValueChange += InvertParams;
    }
    private void OnDisable()
    {
        IsRewinding.OnValueChange -= InvertParams;
    }

    private void InvertParams()
    {
        foreach (string par in AnimatorSpeedParams)
            anim.SetFloat(par, -1 * anim.GetFloat(par));
    }
    IEnumerator Cycle()
    {
        YieldInstruction waitBetween = new WaitForSeconds(TimeBetweenEntries.Value);
        while (true)
        {
            if (IsRewinding.Value)
            {
                if (entries > 0)
                {
                    anim.CrossFade(animators.Last.Value.fullPathHash, 0.3f, 0, animators.Last.Value.normalizedTime);
                    animators.RemoveLast();
                    entries--;
                    yield return waitBetween;
                }
                else yield return waitBetween;
            }
            else
            {
                var stateinfo = anim.GetCurrentAnimatorStateInfo(0);
                animators.AddLast(new TimeEntry(stateinfo.fullPathHash, stateinfo.normalizedTime));
                entries++;
                if (entries > maxentries)
                {
                    animators.RemoveFirst();
                    entries--;
                }
                yield return waitBetween;
            }
        }
    }

    private struct TimeEntry
    {
        public int fullPathHash;
        public float normalizedTime;

        public TimeEntry(int fullPathHash, float normalizedTime)
        {
            this.fullPathHash = fullPathHash;
            this.normalizedTime = normalizedTime;
        }
    }
}
