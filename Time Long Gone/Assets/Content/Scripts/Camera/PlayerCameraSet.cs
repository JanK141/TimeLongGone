using Cinemachine;
using UnityEngine;

namespace Content.Scripts.Camera
{
    public class PlayerCameraSet : MonoBehaviour
    {
        private CinemachineVirtualCamera _cvm;
        private CinemachineComposer _composer;
        private GameObject _player;

        private GameObject _enemy;

        // this variables change also camera aim :(
        [Tooltip("distance > 0"), SerializeField]
        private float distanceFromPlayer = 6;

        [SerializeField] private float height = 4;

        private void Start()
        {
            _cvm = GetComponent<CinemachineVirtualCamera>();
            _player = GameObject.FindGameObjectWithTag("Player");
            _enemy = GameObject.FindGameObjectWithTag("Enemy");
            _composer = _cvm.AddCinemachineComponent<CinemachineComposer>();
            _cvm.LookAt = _enemy.transform;
        }

        private void Update() => SetupCamera();

        private void SetupCamera()
        {
            PositioningCamera();
            AimingCamera();
        }

        private void PositioningCamera()
        {
            var playerPosition = _player.transform.position;
            var enemyPosition = _enemy.transform.position;
            var direction = (playerPosition - enemyPosition).normalized;

            _cvm.transform.position =
                playerPosition + (distanceFromPlayer * direction + Vector3.up * height);
        }

        private void AimingCamera()
        {
            var x = Vector3.Distance(_enemy.transform.position, _cvm.transform.position);
            const float a = (float)-0.36, b = (float)2.77;
         //   Debug.Log("distance " + x);
            _composer.m_TrackedObjectOffset.y = a * x + b;
        }
    }
}