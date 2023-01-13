using DG.Tweening;
using UnityEngine;

namespace Player.States
{
    public class Attack : Interruptible
    {
        public float chainTime { get; set; }
        public float airborne { get; set; }

        private float time;

        public override void OnStateEnter(bool playAnimation)
        {
            if(Vector3.Distance(player.transform.position, (player.combat.enemy as MonoBehaviour).transform.position) < player.variables.attackDistance + player.variables.attackRadius*2)
            {
                var t = (player.combat.enemy as MonoBehaviour).transform.position;
                player.transform.LookAt(new Vector3(t.x, player.transform.position.y, t.z));
            }
            time = 0;
            player.move = player.MoveSlow;
            player.rotate = player.SlowRotate;
            player.velocity = Vector3.zero;
            player.Gravity = 0;
            if(playAnimation)player.animator.SetTrigger("Attack");
            player.ResetAttack();
            player.animator.ResetTrigger("EndChain");
        }

        public override void OnStateExit()
        {
            player.move = player.MoveNormal;
            player.rotate = player.InstaRotate;
            player.Gravity = player.variables.initialGravity;
            player.transform.DOKill();
        }

        public override void Tick()
        {
            if (player.Gravity == 0 && time >= airborne * player.SpeedFactor) player.Gravity = player.variables.initialGravity;
            time += Time.deltaTime;
        }

        public override IPlayerState Evaluate()
        {
            var state = base.Evaluate();
            if (state != null) return state;
            if (player.CanAttack && player.inputContext == InputIntermediary.InputContext.Attack)
            {
                player.inputContext = InputIntermediary.InputContext.Nothing;
                return player.ATTACK_STATE;
            }

            if (time >= chainTime && player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime>=0.95)
            {
                player.animator.SetTrigger("EndChain");
                return player.IDLE_STATE;
            }
            return null;
        }
    }
}