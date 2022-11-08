using Content.Scripts.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRewinder : MonoBehaviour
{
    FloatVariable TimeToRemember;
    FloatVariable TimeBetweenEntries;
    BoolVariable IsRewinding;
    [SerializeField] bool rewindTransformParent = true;
    [SerializeField] bool UseLossyScale = true;

    private int maxentries;
    private int entries;
    private LinkedList<TimeEntry> timeEntries = new LinkedList<TimeEntry>();

    private void Awake()
    {
        IsRewinding = Resources.Load<BoolVariable>("Rewind/IsRewinding");
        TimeToRemember = Resources.Load<FloatVariable>("Rewind/TimeToRemember");
        TimeBetweenEntries = Resources.Load<FloatVariable>("Rewind/TimeBetweenEntries");
    }
    void Start()
    {
        entries = 0;
        maxentries = (int)(TimeToRemember.Value / TimeBetweenEntries.Value);
        StartCoroutine(Cycle());
    }
    float interpolationTime = 0f;
    IEnumerator Cycle()
    {
        YieldInstruction waitBetween = new WaitForSeconds(TimeBetweenEntries.Value);
        while (true)
        {
            if (IsRewinding.Value)
            {
                if (entries > 0)
                {
                    var currPos = transform.position;
                    var currRot = transform.rotation;
                    var currScale = transform.localScale;
                    var timeEntry = timeEntries.Last.Value;
                    if (rewindTransformParent) { 
                        transform.SetParent(timeEntry.parent, true);
                        currScale = transform.localScale;
                    }
                    var targetscale = !UseLossyScale ? timeEntry.scale : CalcLocalScaleByLossy(timeEntry.scale);
                    while (interpolationTime < TimeBetweenEntries.Value)
                    {
                        if (!IsRewinding.Value)
                        {
                            transform.position = timeEntry.position;
                            transform.rotation = timeEntry.rotation;
                            transform.localScale = !UseLossyScale ? timeEntry.scale : CalcLocalScaleByLossy(timeEntry.scale);
                            yield return new WaitForSeconds(TimeBetweenEntries.Value - interpolationTime);
                            interpolationTime = TimeBetweenEntries.Value;
                        }
                        else
                        {
                            transform.position = Vector3.Lerp(currPos, timeEntry.position, interpolationTime / TimeBetweenEntries.Value);
                            transform.rotation = Quaternion.Lerp(currRot, timeEntry.rotation, interpolationTime / TimeBetweenEntries.Value);
                            transform.localScale = 
                                Vector3.Lerp(currScale, targetscale, interpolationTime / TimeBetweenEntries.Value);
                            interpolationTime += Time.deltaTime;
                            yield return null;
                        }
                    }
                    interpolationTime -= TimeBetweenEntries.Value;
                    timeEntries.RemoveLast();
                    entries--;
                }
                else yield return waitBetween;
            }
            else
            {
                timeEntries.AddLast(new TimeEntry(transform.position, transform.rotation, UseLossyScale?transform.lossyScale:transform.localScale, transform.parent));
                entries++;
                if (entries > maxentries)
                {
                    timeEntries.RemoveFirst();
                    entries--;
                }
                yield return waitBetween;
            }
        }
    }
    private Vector3 CalcLocalScaleByLossy(Vector3 scale)
    {
        Transform parent = transform.parent;
        while (parent != null)
        {
            scale = Vector3.Scale(scale, InvertVector3(parent.localScale));
            parent = parent.parent;
        }
        return scale;
    }

    public Vector3 InvertVector3(Vector3 vec)
    {
        return new Vector3(1 / vec.x, 1 / vec.y, 1 / vec.z);
    }

    private struct TimeEntry
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public Transform parent;

        public TimeEntry(Vector3 position, Quaternion rotation, Vector3 scale, Transform parent)
        {
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;
            this.parent = parent;
        }

    }

}
