using UnityEngine;

namespace Enemy
{
    public class ProjectileHit : MonoBehaviour
    {
        [SerializeField] private GameObject fracturedPrefab;
        [SerializeField] private LayerMask mask;
        [SerializeField] [Tooltip("If false renderer and collider will be disabled instead")] private bool destroyOnHit = false;

        private SoundPlayer sound;

        private void Start()
        {
            sound = GetComponent<SoundPlayer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & mask) == 0) return;

            if (other.tag == "Player")
                sound.Play("WoodParry");
            else
                sound.Play("WoodCrash");

            Vector3 vel = Vector3.zero;
            if(TryGetComponent<Rigidbody>(out var rb))
            {
                vel = rb.velocity;
                rb.velocity = Vector3.zero;
                rb.detectCollisions = false;
                rb.isKinematic = true;
            }

            GameObject go = Instantiate(fracturedPrefab, transform.position, transform.rotation);
            foreach(var g in go.GetComponentsInChildren<Rigidbody>())
                g.velocity = vel * Random.Range(0.5f, 1f);

            if (destroyOnHit)
                Destroy(gameObject);
            else
            {
                GetComponent<Collider>().enabled = false;
                GetComponent<Renderer>().enabled = false;
            }
        }


    }
}
