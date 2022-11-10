using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Player;
using UnityEngine;

public class HeavyAttackReset : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerScript.Instance.combat.StickToGround(true);
    }
    
}
