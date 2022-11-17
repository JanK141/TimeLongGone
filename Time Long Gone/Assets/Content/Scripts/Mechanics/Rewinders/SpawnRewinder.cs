using Content.Scripts.Variables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class SpawnRewinder : MonoBehaviour
{
    FloatVariable TimeToRemember;
    FloatVariable TimeBetweenEntries;
    BoolVariable IsRewinding;

    private int maxentries;
    private int entries;
    private LinkedList<TimeEntry> timeEntries = new LinkedList<TimeEntry>();
    private Renderer gorenderer;
    private Collider gocollider;

    private void Awake()
    {
        IsRewinding = GameLogic.Instance.IsRewinding;
        TimeToRemember = GameLogic.Instance.TimeToRemember;
        TimeBetweenEntries = GameLogic.Instance.TimeBetweenEntries;
    }
    void Start()
    {
        gorenderer = GetComponent<Renderer>();
        gocollider = GetComponent<Collider>();
        entries = 1;
        maxentries = (int)(TimeToRemember.Value / TimeBetweenEntries.Value);
        timeEntries.AddFirst(new TimeEntry(null));
        StartCoroutine(Cycle());
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
                    if (timeEntries.Last.Value.exists == null) Destroy(gameObject);
                    else { 
                        gorenderer.enabled = (bool)timeEntries.Last.Value.exists;
                        gocollider.enabled = (bool)timeEntries.Last.Value.exists;
                    }
                    timeEntries.RemoveLast();
                    entries--;
                    yield return waitBetween;
                }else yield return waitBetween;
            }
            else
            {
                timeEntries.AddLast(new TimeEntry(gorenderer.enabled));
                if(timeEntries.First.Value.exists == false) Destroy(gameObject);
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

    private struct TimeEntry
    {
        public bool? exists;

        public TimeEntry(bool? exists)
        {
            this.exists = exists;
        }
    }
}
