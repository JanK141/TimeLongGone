using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Enemy;
using Content.Scripts.Player;
using DG.Tweening;
using UnityEngine;

public class FatBossExtras : MonoBehaviour
{

    public void LookAtPlayer()
    {
        Transform player = PlayerScript.Instance.transform;
        transform.DOLookAt(new Vector3(player.position.x, transform.position.y, player.position.z), 0.5f);
    }

    public void WalkToPlayer()
    {
        EnemyScript.Instance.move.WalkTo(PlayerScript.Instance.transform.position);
    }

    public void WalkForward()
    {
        var enemy = EnemyScript.Instance;
        enemy.move.WalkTo(enemy.transform.position + enemy.transform.forward*2);
    }
}
