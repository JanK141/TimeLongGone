using UnityEngine;

namespace Content.Scripts.Camera
{
    public class CinemachineSwitcher : MonoBehaviour
    {
        public static CinemachineSwitcher Instance;

        private Animator _animator;
        private CameraScript _camera;

        private void Awake()
        {
            Instance = this;
            _animator = GetComponent<Animator>();
            _camera = CameraScript.Instance;
        }


        public void Switch(bool x)
        {
            if (!x)
            {
                _animator.Play("ArenaCamera");
                _camera.ActiveView = CameraScript.View.Arena;
            }
            else
            {
                _animator.Play("PlayerCamera");
                _camera.ActiveView = CameraScript.View.Player;
            }
        }
    }
}