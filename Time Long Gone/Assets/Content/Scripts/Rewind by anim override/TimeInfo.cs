using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeInfo
{
    public Vector3 pos { get; set; }
    public Quaternion rot { get; set; }

    public TimeInfo(Vector3 pos, Quaternion rot)
    {
        this.pos = pos;
        this.rot = rot;
    }
}
