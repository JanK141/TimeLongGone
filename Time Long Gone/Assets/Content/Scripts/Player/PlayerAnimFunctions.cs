using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Player;
using UnityEngine;

public class PlayerAnimFunctions : MonoBehaviour
{
    private PlayerScript player;
    private float initialMoveSpeed;
    void Start()
    {
        player = GetComponentInParent<PlayerScript>();
        initialMoveSpeed = player.movementScript.Speed;
    }

    public void Hit() => player.combat.Hit();
    public void LastHit() => player.combat.LastHit();

    public void AttackStart()
    {
        player.movementScript.Speed *= 0.1f;
        player.movementScript.CanRotate = false;
    }

    public void AttackEnd()
    {
        player.movementScript.Speed = initialMoveSpeed;
        player.movementScript.CanRotate = true;
    }
}
