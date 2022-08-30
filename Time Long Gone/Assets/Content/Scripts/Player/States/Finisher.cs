using DG.Tweening;

namespace Player.States
{
    public class Finisher : IPlayerState
    {
        public Player player { get; set; }

        public virtual void OnStateEnter()
        {
            player.move = player.MoveGravityOnly;
            player.rotate = player.SlowRotate;
            player.animator.Play("HeavyAttack");
        }

        public virtual void OnStateExit()
        {
            player.move = player.MoveNormal;
            player.rotate = player.InstaRotate;
            player.transform.DOKill();
        }

        public virtual void Tick()
        {
        }

        public virtual IPlayerState Evaluate()
        {
            if (player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95) return player.IDLE_STATE;
            if (player.inputContext == InputIntermediary.InputContext.FinisherCanceled)
            {
                player.animator.SetTrigger("FinisherCanceled");
                return player.IDLE_STATE;
            }
            return null;
        }
    }
}