using Content.Scripts.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationRewinder : MonoBehaviour
{
    [SerializeField] FloatVariable TimeToRemember;
    [SerializeField] FloatVariable TimeBetweenEntries;
    [SerializeField] BoolVariable IsRewinding;
    [SerializeField] List<string> AnimatorSpeedParams;

    private Animator anim;
    private int maxentries;
    private int entries;
    private LinkedList<AnimatorStateInfo> animators = new LinkedList<AnimatorStateInfo>();
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
        if (IsRewinding.Value)
        {
            foreach(string par in AnimatorSpeedParams)
                anim.SetFloat(par, -1);
        }
        else
        {
            foreach (string par in AnimatorSpeedParams)
                anim.SetFloat(par, 1);
        }
    }
    IEnumerator Cycle()
    {
        YieldInstruction waitBetween = new WaitForSeconds(TimeBetweenEntries.Value);
        while (true)
        {
            if (IsRewinding.Value)
            {
                if(entries > 0)
                {
                    anim.CrossFade(animators.Last.Value.fullPathHash, 0.1f, 0, animators.Last.Value.normalizedTime);
                    animators.RemoveLast();
                    entries--;
                    yield return waitBetween;
                }else yield return waitBetween;
            }
            else
            {
                animators.AddLast(anim.GetCurrentAnimatorStateInfo(0));
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
}
