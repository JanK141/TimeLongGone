using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public interface IEnemy
    {
        public float Health { get;}
        public EnemyStatus Status { get;}
        public int Stage { get;}
        public bool ActiveAI { get; set; }


        public void ReceiveHit(float damage);
        public void ReceiveStun();
        public void ReceiveParry();
        public float RewindTimeNeeded();

    }
    public enum EnemyStatus
    {
        Passive,
        Attacking,
        Chasing,
        Fleeing,
        Vulnerable,
        Parried,
        Stunned,
        Untouchable
    }
    public enum AttackStatus
    {
        Regular,
        Unblockable,
        Unstoppable,
        Force,
        Sequence
    }
}
