using Cinemachine;
using UnityEngine;

namespace Content.Scripts.Camera
{
    public class CameraPositioning : MonoBehaviour
    {
        private CinemachineVirtualCamera _cvm;
        private CinemachineComposer _composer;
        private GameObject _player;
        private GameObject _enemy;

        [SerializeField] private float distanceFromPlayer;
        [SerializeField] private float cameraHeight;

        private void Start()
        {
            _cvm = GetComponent<CinemachineVirtualCamera>();
            _player = GameObject.FindGameObjectWithTag("Player");
            _enemy = GameObject.FindGameObjectWithTag("Enemy");
            _composer = _cvm.AddCinemachineComponent<CinemachineComposer>();
        }

        private void Update() => SetupCamera();

        private void SetupCamera()
        {
            PositioningCamera();
            AimingCamera();
        }

        private void PositioningCamera()
        {
            var direction = (_player.transform.position - _enemy.transform.position).normalized;
            _cvm.transform.position =
                _player.transform.position + (distanceFromPlayer * direction + Vector3.up * cameraHeight);
            _cvm.LookAt = _enemy.transform;
        }

        private void AimingCamera()
        {
            var x = Vector3.Distance(_enemy.transform.position, _cvm.transform.position);
            const float a = -0.36f, b = 2.77f;
            _composer.m_TrackedObjectOffset.y = a * x + b;
        }
    }
}