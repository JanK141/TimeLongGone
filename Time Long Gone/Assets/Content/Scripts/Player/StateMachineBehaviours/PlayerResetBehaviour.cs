using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Player;
using DG.Tweening;
using UnityEngine;

public class PlayerResetBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerScript player = PlayerScript.Instance;
        player.movementScript.rotateSlow = false;
        player.transform.DOKill();
        player.movementScript.ResetSpeed();
        player.movementScript.canMove = true;
        player.movementScript.canRotate = true;
        player.combat.CanAttack = true;
        player.playerInput.CanStun = true;
    }
}
