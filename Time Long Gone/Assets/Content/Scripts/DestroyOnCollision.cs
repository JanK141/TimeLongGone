using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    [SerializeField] private GameObject fracturedPrefab;
    [SerializeField] private LayerMask mask;

    /*private void OnCollisionEnter(Collision collision)
    {
        print("");
        if (((1 << collision.gameObject.layer) & mask) != 0)
        {
            print("test2");
            Instantiate(fracturedPrefab, transform.position, transform.rotation);
            //Instantiate(fracturedPrefab, transform.parent).transform.localPosition = transform.localPosition;

            Destroy(this.gameObject);
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & mask) != 0)
        {
            //print("test");
            Instantiate(fracturedPrefab, transform.position, transform.rotation);
            //Instantiate(fracturedPrefab, transform.parent).transform.localPosition = transform.localPosition;

            Destroy(this.gameObject);
        }
    }


}
