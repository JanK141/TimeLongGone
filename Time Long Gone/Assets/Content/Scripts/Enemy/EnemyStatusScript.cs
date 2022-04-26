using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatusScript : MonoBehaviour
{
    public enum Statuses
    {
        Stunned = -2,       //can take (more?) dmg, cannot move
        Vulnerable = -1,    //can take dmg, can be blocked, can be parried, receiving a stun attack will interrupt and transfer to Stunned
        Regular = 0,        //DEFAULT, can take dmg, can be blocked, can be parried
        Unblockable = 1,        //can take dmg, can be parried, CAN'T be blocked
        Unavoidable = 2,        //can take dmg, CAN'T be blocked or parried
        Invulnerable = 999        //immune to everything
    }

    public Statuses currStatus;

    // Start is called before the first frame update
    void Start()
    {
        MakeEnemyRegular();
    }

    public void MakeEnemyStunned()
    {
        currStatus = Statuses.Stunned;
    }

    public void MakeEnemyVulnerable()
    {
        currStatus = Statuses.Vulnerable;
    }

    public void MakeEnemyRegular()
    {
        currStatus = Statuses.Regular;
    }

    public void MakeEnemyUnblockable()
    {
        currStatus = Statuses.Unblockable;
    }

    public void MakeEnemyUnavoidable()
    {
        currStatus = Statuses.Unavoidable;
    }

    public void MakeEnemyInvulnerable()
    {
        currStatus = Statuses.Unavoidable;
    }
}
