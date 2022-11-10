using Content.Scripts.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidBodyRewinder : MonoBehaviour
{
    FloatVariable TimeToRemember;
    FloatVariable TimeBetweenEntries;
    BoolVariable IsRewinding;

    private Rigidbody rb;
    private int maxentries;
    private int entries;
    private LinkedList<RBEntry> rbentries = new LinkedList<RBEntry>();

    private void Awake()
    {
        IsRewinding = Resources.Load<BoolVariable>("Rewind/IsRewinding");
        TimeToRemember = Resources.Load<FloatVariable>("Rewind/TimeToRemember");
        TimeBetweenEntries = Resources.Load<FloatVariable>("Rewind/TimeBetweenEntries");
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        entries = 0;
        maxentries = (int)(TimeToRemember.Value / TimeBetweenEntries.Value);
        StartCoroutine(Cycle());
    }

    private void OnEnable() => IsRewinding.OnValueChange += HandleRewind;
    private void OnDisable() => IsRewinding.OnValueChange -= HandleRewind;

    private void HandleRewind()
    {
        if (IsRewinding.Value)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
        else
        {
            rb.isKinematic = rbentries.Last.Value.isKinematic;
            rb.detectCollisions = rbentries.Last.Value.isDetectingCollisions;
            rb.velocity = rbentries.Last.Value.velocity;
            rb.angularVelocity = rbentries.Last.Value.angularVelocity;
        }
    }
    float interpolationTime = 0f;
    IEnumerator Cycle()
    {
        YieldInstruction waitBetween = new WaitForSeconds(TimeBetweenEntries.Value);
        while (true)
        {
            if (IsRewinding.Value)
            {
                if (entries > 1)
                {
                    var currentVel = rb.velocity;
                    var currentAngVel = rb.angularVelocity;
                    var entry = rbentries.Last.Value;
                    while(interpolationTime < TimeBetweenEntries.Value)
                    {
                        if (!IsRewinding.Value)
                        {
                            rb.velocity = entry.velocity;
                            rb.angularVelocity = entry.angularVelocity;
                            rb.isKinematic = entry.isKinematic;
                            rb.detectCollisions = entry.isDetectingCollisions;
                            yield return new WaitForSeconds(TimeBetweenEntries.Value - interpolationTime);
                            interpolationTime = TimeBetweenEntries.Value;
                        }
                        else
                        {
                            rb.velocity = Vector3.Lerp(currentVel, entry.velocity, interpolationTime / TimeBetweenEntries.Value);
                            rb.angularVelocity = Vector3.Lerp(currentAngVel, entry.angularVelocity, interpolationTime / TimeBetweenEntries.Value);
                            interpolationTime += Time.deltaTime;
                            yield return null;
                        }
                    }
                    interpolationTime -= TimeBetweenEntries.Value;
                    rbentries.RemoveLast();
                    entries--;
                }
                else yield return waitBetween;
            }
            else
            {
                rbentries.AddLast(new RBEntry(rb.velocity, rb.angularVelocity, rb.isKinematic, rb.detectCollisions));
                entries++;
                if(entries > maxentries)
                {
                    rbentries.RemoveFirst();
                    entries--;
                }
                yield return waitBetween;
            }
        }
    }

    private struct RBEntry
    {
        public Vector3 velocity;
        public Vector3 angularVelocity;
        public bool isKinematic;
        public bool isDetectingCollisions;

        public RBEntry(Vector3 velocity, Vector3 angularVelocity, bool isKinematic, bool isDetectingCollisions)
        {
            this.velocity = velocity;
            this.angularVelocity = angularVelocity;
            this.isKinematic = isKinematic;
            this.isDetectingCollisions = isDetectingCollisions;
        }
    }
}
