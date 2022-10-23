using Content.Scripts.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRewinder : MonoBehaviour
{
    [SerializeField] FloatVariable TimeToRemember;
    [SerializeField] FloatVariable TimeBetweenEntries;
    [SerializeField] BoolVariable IsRewinding;

    private int maxentries;
    private int entries;
    private LinkedList<Vector3> positions = new LinkedList<Vector3>();
    private LinkedList<Quaternion> rotations = new LinkedList<Quaternion>();
    private LinkedList<Vector3> scales = new LinkedList<Vector3>();

    void Start()
    {
        entries = 0;
        maxentries = (int)(TimeToRemember.Value / TimeBetweenEntries.Value);
        StartCoroutine(Cycle());
    }

    IEnumerator Cycle()
    {
        YieldInstruction waitBetween = new WaitForSeconds(TimeBetweenEntries.Value);
        while (true)
        {
            if (IsRewinding.Value)
            {
                if (entries>0)
                {
                    var currPos = transform.position;
                    var currRot = transform.rotation;
                    var currScale = transform.localScale;
                    float time = 0f;
                    while (time < TimeBetweenEntries.Value)
                    {
                        transform.position = Vector3.Lerp(currPos, positions.Last.Value, time / TimeBetweenEntries.Value);
                        transform.rotation = Quaternion.Lerp(currRot, rotations.Last.Value, time / TimeBetweenEntries.Value);
                        transform.localScale = Vector3.Lerp(currScale, scales.Last.Value, time / TimeBetweenEntries.Value);
                        time += Time.deltaTime;
                        yield return null;
                    }
                    positions.RemoveLast();
                    rotations.RemoveLast();
                    scales.RemoveLast();
                    entries--;
                }
                else yield return waitBetween;
            }
            else
            {
                positions.AddLast(transform.position);
                rotations.AddLast(transform.rotation);
                scales.AddLast(transform.localScale);
                entries++;
                if (entries > maxentries)
                {
                    positions.RemoveFirst();
                    rotations.RemoveFirst();
                    scales.RemoveFirst();
                    entries--;
                }
                yield return waitBetween;
            }
        }
    }
}
