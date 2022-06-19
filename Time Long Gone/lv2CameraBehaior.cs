using UnityEngine;

public class lv2CameraBehavior : MonoBehaviour
{
    [serializedfield]private float activationDistance;
    TargetGroup targetGroup;
     GameObject player;
     GameObject enemy;
    void Start()
    {
        targetGroup = GameObject.getCompoment
            player
        
    }
    void Update()
    {
        var distance = Vector3.Distance(player, enemy);
        if(distance>=activationDistance)
            targetGruop.
        
    }
}
