using System;
using System.Collections;
using UnityEngine;

namespace Content.Scripts.Mechanics
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
        
        #region Event rewind

        public static event Action<bool> OnRewind;
        private static void RewindInvoker(bool value) => OnRewind?.Invoke(value);

        #endregion

        private void Awake() => Instance = this;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                RewindInvoker(true);
                print(" START REWIND");
            }
            else if (Input.GetKeyDown(KeyCode.CapsLock))
            {
                RewindInvoker(false);
                print("STOP REWIND");
            }
        } // FOR TESTING

        public void ProcessSlowMo(bool state) 
        {
            _isSlowMo = state;

            StartCoroutine(_isSlowMo ? StartSlowMo() : StopSlowMo());
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
            StopSlowMo() 
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

        public IEnumerator PlayerDead() 
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