using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DebrisDisolve : MonoBehaviour
{
    [SerializeField] private float initialForce = 1f;

    // Start is called before the first frame update
    private void Start()
    {
        GetComponent<Rigidbody>().AddForce(new Vector3(Random.value, Random.value, Random.value).normalized * initialForce, ForceMode.Impulse);
        StartCoroutine(Disolve());
    }

    private IEnumerator Disolve()
    {
        yield return new WaitForSeconds(Random.Range(4, 10));
        /*while (transform.localScale.magnitude > 0.1)
        {
            transform.localScale -= Vector3.one * 0.2f * Time.deltaTime;
            yield return null;
        }*/

        transform.DOScale(Vector3.one * 0.1f, 4f).OnComplete(() => Destroy(this.gameObject));
    }
}
