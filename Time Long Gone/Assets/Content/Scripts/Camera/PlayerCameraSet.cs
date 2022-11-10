using Cinemachine;
using UnityEngine;

namespace Content.Scripts.Camera
{
    [ExecuteAlways]
    public class PlayerCameraSet : MonoBehaviour
    {
        private CinemachineVirtualCamera _cvm;

        private Transform _player;
        private Transform _enemy;

        // this variables change also camera aim :(
        [Tooltip("distance > 0"), SerializeField]
        private float distanceFromPlayer = 6;

        [SerializeField] private float height = 4;
        [SerializeField] private float sideOffset = 2;
        private Vector3 velocity = Vector3.zero;

        private void Start()
        {
            _cvm = GetComponent<CinemachineVirtualCamera>();
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            _enemy = GameObject.FindGameObjectWithTag("Enemy").transform;
        }

        private void Update()
        {
            var direction = (_player.position - _enemy.position).normalized;
            var desiredPos = _player.position + (distanceFromPlayer * direction + Vector3.up * height);
            desiredPos += Quaternion.Euler(0, -90, 0) * direction * sideOffset;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, 0.25f);
            transform.LookAt(_enemy);
        }

    }
}