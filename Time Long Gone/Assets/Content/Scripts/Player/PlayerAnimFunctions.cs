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

    public void Hit() => player.combat.Hit(false);

    public void Finisher() => player.combat.Hit(true);
}
