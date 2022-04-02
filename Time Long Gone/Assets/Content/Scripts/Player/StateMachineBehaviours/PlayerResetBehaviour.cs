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
        player.movementScript.RotateSlow = false;
        player.transform.DOKill();
        player.movementScript.ResetSpeed();
        player.movementScript.CanMove = true;
        player.movementScript.CanRotate = true;
        player.combat.CanAttack = true;
    }
}
