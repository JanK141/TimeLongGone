using Content.Scripts.Camera;
using UnityEngine;

namespace Content.Scripts.Enemy
{
    public class EnemyHealth : MonoBehaviour
    {
        #region Inspector fields
        [SerializeField] [Min(1)] private int maxHealth = 100;
        [SerializeField] private IntVariable hp;
        [SerializeField] [Min(1)] [Tooltip("How many combat stages boss has (divided by % of hp) ")] private int stages = 1;
        #endregion

        #region Private variables
        private int _currStage;
        private float[] _stageChangers;
        private EnemyScript _enemy;
        private static readonly int Stage = Animator.StringToHash("Stage");

        #endregion

        #region Properties
        public int CurrHealth
        {
            get => hp.Value;
            set
            {
                hp.Value = value;
                UpdateHealth();
            }
        }

        public int MaxHealth => maxHealth;
        public int CurrStage => _currStage;
        #endregion


        private void Start()
        {
            _enemy = EnemyScript.Instance;
            hp.Value = maxHealth;
            hp.Reset();
            _currStage = 1;
            _enemy.anim.SetInteger(Stage, _currStage);
            _stageChangers = new float[stages];
            for (var i = 0; i < stages; i++)
                _stageChangers[i] = i + 1 == stages ? 0 : maxHealth - (maxHealth / stages) * (i + 1);
        }

        private void UpdateHealth()
        {
            if (hp.Value <= 0) Death();
            else if (hp.Value <= _stageChangers[_currStage - 1])
            {
                _currStage++;
                _enemy.anim.Play("SwitchStage");
                _enemy.anim.SetInteger(Stage, _currStage);
                CinemachineSwitcher.Instance.Switch(false);
            }
        }

        private void Death()
        {
            print("You won the level!");
            _enemy.anim.Play("Death");
            CinemachineSwitcher.Instance.Switch(true);
        }
    }
}