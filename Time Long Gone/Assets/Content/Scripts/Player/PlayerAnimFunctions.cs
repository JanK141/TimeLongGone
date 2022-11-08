using UnityEngine;

namespace Player
{
    public class PlayerAnimFunctions : MonoBehaviour
    {
        private Player player;
        void Start()
        {
            player = GetComponentInParent<Player>();
        }

        public void Hit() => player.combat.Hit();

        public void LastHit() => player.combat.Hit(true);

        public void Finisher() => player.combat.Finisher();
    }
}
