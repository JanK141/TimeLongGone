using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Variables;
using DG.Tweening;
using UnityEngine;

public class DebrisDisolve : MonoBehaviour
{
    [SerializeField] Vector3 targetScale = new Vector3(0.1f,0.1f,0.1f);
    [SerializeField] private float initialForce = 1f;
    [SerializeField] private float minTime;
    [SerializeField] private float maxTime;
    [SerializeField][Tooltip("If false renderer and collider will be disabled instead")] private bool destroyOnEnd = false;
    BoolVariable IsRewinding;

    private float timeToScale;
    private float interpolationTime;
    private Vector3 originalScale;
    private bool hasEnded = false;

    private void Awake()
    {
        IsRewinding = Resources.Load<BoolVariable>("Rewind/IsRewinding");
    }
    private void Start()
    {
        GetComponent<Rigidbody>().AddForce(new Vector3(Random.value, Random.value, Random.value).normalized * initialForce, ForceMode.Impulse);
        timeToScale = Random.Range(minTime, maxTime);
        interpolationTime = 0f;
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (!IsRewinding.Value && interpolationTime < timeToScale && !hasEnded)
        {
            interpolationTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, interpolationTime/timeToScale);
        }
        if(!hasEnded && interpolationTime >= timeToScale)
        {
            hasEnded = true;
            if (destroyOnEnd)
            {
                Destroy(gameObject);
            }
            else
            {
                GetComponent<Collider>().enabled = false;
                GetComponent<Renderer>().enabled = false;
            }
        }
    }

    void OnEnable() => IsRewinding.OnValueChange += HandleRewind;
    void OnDisable() => IsRewinding.OnValueChange -= HandleRewind;

    private void HandleRewind()
    {
        if (IsRewinding.Value)
            hasEnded = true;
        else
            StartCoroutine(HandleRewindW8OneFrame());
    }
    IEnumerator HandleRewindW8OneFrame()
    {
        yield return null;
        hasEnded = false;
        interpolationTime = timeToScale *
            (Vector3.Dot(transform.localScale - originalScale, targetScale - originalScale) / Vector3.Dot(targetScale - originalScale, targetScale - originalScale));
        if (interpolationTime < timeToScale) hasEnded = false;
    }

}
