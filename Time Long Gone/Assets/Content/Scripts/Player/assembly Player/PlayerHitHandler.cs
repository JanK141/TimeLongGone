using System;
using System.Collections;
using Content.Scripts.Enemy;
using UnityEngine;

namespace Content.Scripts.Player.assembly_Player
{
    public class PlayerHitHandler : MonoBehaviour
    {
        [SerializeField] private PlayerVariables variables;

        [SerializeField] private AnimationCurve pushCurve;


        void ProcessHit(Statuses enemyStatus)
        {
            if (enemyStatus == Statuses.Vulnerable)
            {
            }
        }

        IEnumerator NoCollision()
        {
            throw new InvalidOperationException();
        }

        IEnumerator PushPlayer(float velocity, float time)
        {
            throw new InvalidOperationException();
        }
    }
}