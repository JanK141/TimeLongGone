using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Player;
using UnityEngine;

public class WeaponHitBox : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Player")))
        {
            PlayerScript.Instance.hit.ReceiveHit();
            GetComponent<Collider>().enabled = false;
        }
    }

}
