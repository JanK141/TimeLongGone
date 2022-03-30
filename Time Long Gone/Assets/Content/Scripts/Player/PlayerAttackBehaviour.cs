using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Player;
using DG.Tweening;
using UnityEngine;

public class PlayerAttackBehaviour : StateMachineBehaviour
{
    [SerializeField] private bool lockMove;
    [SerializeField] private bool lockRotation;
    [SerializeField] private bool rotateSlow;
    [SerializeField] private bool lockDash;
    [SerializeField] [Tooltip("Put 0 to not affect move speed")] private float moveSpeed = 0;

    private PlayerScript player;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = PlayerScript.Instance;
        if (lockDash) player.movementScript.CanDash = false;
        if (lockMove) player.movementScript.CanMove = false;
        if (lockRotation) player.movementScript.CanRotate = false;
        else if (rotateSlow) player.movementScript.RotateSlow = true;
        if (moveSpeed > 0)
        {
            player.movementScript.Speed = moveSpeed;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (lockDash) player.movementScript.CanDash = false;
        if (lockMove) player.movementScript.CanMove = false;
        if (lockRotation) player.movementScript.CanRotate = false;
        else if (rotateSlow) player.movementScript.RotateSlow = true;
        if (moveSpeed > 0)
        {
            player.movementScript.Speed = moveSpeed;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (lockDash) player.movementScript.CanDash = true;
        if (lockMove) player.movementScript.CanMove = true;
        if (lockRotation) player.movementScript.CanRotate = true;
        else if (rotateSlow)
        {
            player.movementScript.RotateSlow = false;
            player.transform.DOKill();
        }
        if (moveSpeed > 0)
        {
            player.movementScript.ResetSpeed();
        }
    }

}
