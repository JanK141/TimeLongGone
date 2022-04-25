using System;
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

        private bool _isSlowMo;


        //public static event Action<bool> OnRewind; //Tak jak zaproponowa�em.
        //TODO Invoke it somewhere ofc

        private void Awake() => Instance = this;


        public void ProcessSlowMo(bool state) //TODO ze skryptu z man� wywo�ujesz t� funkcj� i �miga
        {
            _isSlowMo = state;

            if (_isSlowMo)
                StartCoroutine(StartSlowMo());
            else
                StartCoroutine(StopSlowMo());
        }

        private IEnumerator StartSlowMo()
        {
            var time = (1 - Time.timeScale) / (1 - minSlowMo) * slowMoTime;
            while (_isSlowMo && Time.timeScale > minSlowMo)
            {
                time += Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Lerp(1f, minSlowMo, time / slowMoTime);
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
                yield return null;
            }
        }

        private IEnumerator
            StopSlowMo() //TODO after rewinding time it needs to be invoked as well, as it generally smoothly sets time scale to normal (todo later)
        {
            var time = (Time.timeScale - minSlowMo) / (1 - minSlowMo) * slowMoTime;
            while (!_isSlowMo && Time.timeScale < 1f)
            {
                time += Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Lerp(minSlowMo, 1f, time / slowMoTime);
                Time.fixedDeltaTime = Time.timeScale * 0.02f;
                yield return null;
            }
        }

        private IEnumerator PlayerDead() //TODO invoke it through some event like OnPlayerDeath (todo later)
        {
            var time = (1 - Time.timeScale) * (slowMoTime / deathSlowMoMulti);
            while (Time.timeScale > 0.01f)
            {
                time += Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Lerp(1f, 0.01f, time / (slowMoTime / deathSlowMoMulti));
                //Time.fixedDeltaTime = Time.timeScale * 0.02f;
                yield return null;
            }
        }
    }
}