using UnityEngine;

namespace Content.Scripts.Camera
{
    public class CinemachineSwitcher : MonoBehaviour
    {
        private Animator _animator;
        private CameraScript _camera;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _camera = CameraScript.Instance;
        }


        private void Update()
        {
            if (Input.GetKey("1"))
            {
                _animator.Play("ArenaCamera");
                _camera.ActiveView = CameraScript.View.Arena;
            }

            else if (Input.GetKey("2"))
            {
                _animator.Play("PlayerCamera");
                _camera.ActiveView = CameraScript.View.Player;
            }
        }
    }
}