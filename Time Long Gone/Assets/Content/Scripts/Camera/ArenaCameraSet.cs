using UnityEngine;

namespace Content.Scripts.Camera
{
    public class ArenaCameraSet : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float height = 7.5f;
        [SerializeField] private float distance = -10;
        private void Update() => Setting();

        private void Setting()
        {
            var position = target.position;
            transform.position = new Vector3(transform.position.x,
                position.y + height, position.z + distance);
        }
    }
}