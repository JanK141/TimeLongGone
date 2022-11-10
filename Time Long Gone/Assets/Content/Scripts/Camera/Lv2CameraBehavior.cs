using Cinemachine;
using UnityEngine;

namespace Content.Scripts.Camera
{
    public class Lv2CameraBehavior : MonoBehaviour
    {
        [SerializeField] private float activationDistance;

        private CinemachineVirtualCamera _cameraOnPlayer;
        private CinemachineVirtualCamera _cameraOnEnemyAndPlayer;

        private GameObject _player;
        private GameObject _enemy;

        private bool _locksOnEnemy;

        private void Start()
        {
            _cameraOnPlayer = GameObject.Find("CameraOnPlayer").GetComponent<CinemachineVirtualCamera>();
            _cameraOnEnemyAndPlayer = GameObject.Find("arena cam").GetComponent<CinemachineVirtualCamera>();
            _player = GameObject.FindGameObjectWithTag("Player");
            _enemy = GameObject.FindGameObjectWithTag("Enemy");
        }

        private void Update()
        {
            var distance = Vector3.Distance(
                _player.transform.position,
                _enemy.transform.position
            );

            if (distance < activationDistance)
                LookAtEnemyAndPlayer();
            else
                LookAtPlayer();
        }

        private void LookAtPlayer()
        {
            if (!_locksOnEnemy) return;
            SwapCamerasPriority();
            _locksOnEnemy = false;
        }

        private void LookAtEnemyAndPlayer()
        {
            if (_locksOnEnemy) return;
            SwapCamerasPriority();
            _locksOnEnemy = true;
        }

        private void SwapCamerasPriority() =>
            (_cameraOnPlayer.Priority, _cameraOnEnemyAndPlayer.Priority) = 
            (_cameraOnEnemyAndPlayer.Priority, _cameraOnPlayer.Priority);
    }
}