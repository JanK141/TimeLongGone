using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Player;
using UnityEngine;

public class PlayerAnimFunctions : MonoBehaviour
{
    private PlayerScript player;
    void Start()
    {
        player = GetComponentInParent<PlayerScript>();
    }

    public void Hit() => player.combat.Hit();
    public void LastHit() => player.combat.LastHit();

}
