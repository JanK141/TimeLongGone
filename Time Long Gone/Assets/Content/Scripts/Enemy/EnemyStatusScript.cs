using UnityEngine;

namespace Content.Scripts.Enemy
{
    public class EnemyStatusScript : MonoBehaviour
    {
    

        public static Statuses CurrStatus;

        // Start is called before the first frame update
        private void Start() => MakeEnemyRegular();

        private void Update() => StatusDebug.Instance.UpdateText(CurrStatus.ToString());

        public void MakeEnemyStunned()
        {
            CurrStatus = Statuses.Stunned;
        }

        public void MakeEnemyVulnerable()
        {
            CurrStatus = Statuses.Vulnerable;
        }

        public void MakeEnemyRegular()
        {
            CurrStatus = Statuses.Regular;
        }

        public void MakeEnemyUnblockable()
        {
            CurrStatus = Statuses.Unblockable;
        }

        public void MakeEnemyUnavoidable()
        {
            CurrStatus = Statuses.Unavoidable;
        }

        public void MakeEnemyInvulnerable()
        {
            CurrStatus = Statuses.Unavoidable;
        }
    }
    public enum Statuses
    {
        Stunned = -2,       //can take (more?) dmg, cannot move
        Vulnerable = -1,    //can take dmg, can be blocked, can be parried, receiving a stun attack will interrupt and transfer to Stunned
        Regular = 0,        //DEFAULT, can take dmg, can be blocked, can be parried
        Unblockable = 1,        //can take dmg, can be parried, CAN'T be blocked
        Unavoidable = 2,        //can take dmg, CAN'T be blocked or parried
        Invulnerable = 999        //immune to everything
    }
}