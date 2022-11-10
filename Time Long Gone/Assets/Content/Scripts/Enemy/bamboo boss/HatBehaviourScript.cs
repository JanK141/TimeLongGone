using UnityEngine;

namespace Content.Scripts.Enemy.bamboo_boss
{
    public class HatBehaviourScript : MonoBehaviour
    {
        [SerializeField] private Animator bossAnimator;
        private static readonly int SomethingOnMyHat = Animator.StringToHash("somethingOnMyHat");

        private void OnCollisionStay()
            => bossAnimator.SetBool(SomethingOnMyHat, true);

        private void OnCollisionExit()
            => bossAnimator.SetBool(SomethingOnMyHat, false);
    }
}