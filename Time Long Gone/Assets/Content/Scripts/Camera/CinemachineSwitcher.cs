using UnityEngine;

namespace Content.Scripts.Camera
{
    public class CinemachineSwitcher : MonoBehaviour
    {
        private Animator _animator;


        private void Awake()
            => _animator = GetComponent<Animator>();


        private void Update()
        {
            if (Input.GetKey("1"))
            {
                _animator.Play("ArenaCamera");
            }
            else if (Input.GetKey("2"))
            {
                _animator.Play("PlayerCamera");
            }
        }
    }
}