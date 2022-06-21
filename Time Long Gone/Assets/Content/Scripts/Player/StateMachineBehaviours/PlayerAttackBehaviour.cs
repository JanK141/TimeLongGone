using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Player;
using DG.Tweening;
using UnityEngine;

public class PlayerAttackBehaviour : StateMachineBehaviour
{
    [SerializeField] [Tooltip("Put 0 to not affect move speed")] private float moveSpeed = 0;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerScript player = PlayerScript.Instance;
        player.movementScript.rotateSlow = true;
        if(moveSpeed!=0)player.movementScript.Speed = moveSpeed;
    }
}
