using System.Collections;
using UnityEngine;

namespace Content.Scripts.tim
{
    public class Controller : MonoBehaviour
    {
        public static Controller Instance;

        [SerializeField] [Range(0, 1)] [Tooltip("How low time scale can get during slow mo")]
        private float minSlowMo;

        [SerializeField] [Tooltip("How long it gets to bring time scale to it's minimum value")]
        private float slowMoTime;

        [SerializeField] [Tooltip("How much faster time should slow down when player is killed")]
        private float deathSlowMoMulti = 3;

        [SerializeField] private float maxRewindTime;
        //[SerializeField] private CinemachineVirtualCamera cam;

        private bool _isRewinding;
        private bool _isPlayerDead;
        private bool _isSlowMo;

        public bool PlayerPressTime
        {
            set
            {
                if (value) ProcessSlowMo();
            }
        }

        public bool IsPlayerDead
        {
            get => _isPlayerDead;
            set
            {
                _isPlayerDead = value;
                DeadOrAlive(value);
            }
        }

        public float RewindTime => maxRewindTime;

        public bool IsRewinding => _isRewinding;

        private void Awake() => Instance = this;

        private void DeadOrAlive(bool dead)
        {
            if (dead)
            {
                //       Mana.Instance.Generating = false;
                StartCoroutine(PlayerDead());
            }
            else
            {
                //      Mana.Instance.Generating = true;
                Time.timeScale = minSlowMo;
                StartCoroutine(StopSlowMo());
            }
        }

        private void ProcessSlowMo()
        {
            if (!_isSlowMo)
            {
                _isSlowMo = true;
                StartCoroutine(StartSlowMo());
            }
            else
            {
                _isSlowMo = false;
                StartCoroutine(StopSlowMo());
            }

            Debug.Log(_isSlowMo);
            PlayerPressTime = false;
        }

        private IEnumerator StartSlowMo()
        {
            //    var composer = cam.GetCinemachineComponent<CinemachineGroupComposer>();
            var time = (1 - Time.timeScale) / (1 - minSlowMo) * slowMoTime;
            while (_isSlowMo && Time.timeScale > minSlowMo)
            {
                time += Time.unscaledDeltaTime;
                //       composer.m_MinimumFOV = Mathf.Lerp(88f, 60f, time / SlowMoTime);
                Time.timeScale = Mathf.Lerp(1f, minSlowMo, time / slowMoTime);
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
                yield return null;
            }
        }

        //TODO change loop conditions in coroutines for safety
        private IEnumerator StopSlowMo()
        {
            //  var composer = cam.GetCinemachineComponent<CinemachineGroupComposer>();
            var time = (Time.timeScale - minSlowMo) / (1 - minSlowMo) * slowMoTime;
            while (!_isSlowMo && Time.timeScale < 1f)
            {
                time += Time.unscaledDeltaTime;
                //    composer.m_MinimumFOV = Mathf.Lerp(60f, 88f, time / SlowMoTime);
                Time.timeScale = Mathf.Lerp(minSlowMo, 1f, time / slowMoTime);
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
                yield return null;
            }
        }

        private IEnumerator PlayerDead()
        {
            //  var composer = cam.GetCinemachineComponent<CinemachineGroupComposer>();
            var time = (1 - Time.timeScale) * (slowMoTime / deathSlowMoMulti);
            while (Time.timeScale > 0.01f)
            {
                time += Time.unscaledDeltaTime;
                //     composer.m_MinimumFOV = Mathf.Lerp(88f, 40f, time / (SlowMoTime / DeathSlowMoMulti));
                Time.timeScale = Mathf.Lerp(1f, 0.01f, time / (slowMoTime / deathSlowMoMulti));
                //Time.fixedDeltaTime = Time.timeScale * 0.02f;
                yield return null;
            }
        }
    }
}