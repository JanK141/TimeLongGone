using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeGPS : MonoBehaviour
{
    #region Inspector Fields
    [SerializeField] [Tooltip("How often/s will the info be saved")] [Range(0, 1)] 
    private float tickRate = 0.1f;
    [SerializeField] [Tooltip("Maximum number of seconds past remembered")] 
    private float memoryExtent = 5.0f;
    [SerializeField]
    [Tooltip("Rate at which the time will be rewinded")]
    [Range(0, 1)]
    private float rewindRate = 0.5f;
    private float rewindSpeed = 1;
    #endregion

    Transform target;

    [HideInInspector]
    private LinkedList<TimeInfo> TimeInfoList;

    void Start()
    {
        target = GetComponent<Transform>();
        rewindSpeed = tickRate * (1 / rewindRate);
        StartSaving();

        /*
        Animator animator = GetComponent<Animator>();
        Animator a1 = GetComponent<Animator>();
        AnimatorOverrideController a2 = GetComponent<AnimatorOverrideController>();
        animator.runtimeAnimatorController = a2;
        animator = a1;
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartSaving()
    {
        StopAllCoroutines();
        StartCoroutine(SavingRotation());
    }

    IEnumerator SavingRotation()
    {
        while(true)
        {
            TimeInfoList.AddFirst(new TimeInfo(target.position, target.rotation));
            if (TimeInfoList.Count > tickRate * memoryExtent)
            {
                TimeInfoList.RemoveLast();
            }
            yield return new WaitForSeconds(tickRate);
        }
    }

    public void StartRewinding()
    {
        StopAllCoroutines();
        StartCoroutine(RewindingRotation());
    }

    IEnumerator RewindingRotation()
    {
        while (true)
        {
            target.position = TimeInfoList.First.Value.pos;
            target.rotation = TimeInfoList.First.Value.rot;
            TimeInfoList.RemoveFirst();
            if (TimeInfoList.Count == 0)
            {
                StartSaving();
            }
            yield return new WaitForSeconds(rewindSpeed);
        }
    }

    
}
