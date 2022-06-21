using Content.Scripts.Enemy;
using Content.Scripts.Player;
using DG.Tweening;
using UnityEngine;

public class FatBossExtras : MonoBehaviour
{

    public void LookAtPlayer()
    {
        var player = PlayerScript.Instance.transform;
        var position = player.position;
        transform.DOLookAt(new Vector3(position.x, transform.position.y, position.z), 0.5f);
    }

    public void WalkToPlayer() => EnemyScript.Instance.move.WalkTo(PlayerScript.Instance.transform.position);
    
    public void WalkForward()
    {
        var enemy = EnemyScript.Instance;
        var enemyTransform = enemy.transform;
        enemy.move.WalkTo(enemyTransform.position + enemyTransform.forward*2);
    }

    public void StartWalkigForward()
    {
        var enemy = EnemyScript.Instance;
        var enemyTransform = enemy.transform;
        enemy.move.WalkTo(enemyTransform.position + enemyTransform.forward * 100);
    }

    public void StopWalkigForward()
    {
        var enemy = EnemyScript.Instance;
        enemy.move.WalkTo(enemy.transform.position);
    }
}
