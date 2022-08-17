using System.Collections;
using Content.Scripts.Enemy;
using Content.Scripts.Player;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

namespace Content.Scripts
{
    public class Level1Manager : MonoBehaviour
    {
        [SerializeField] private PlayableDirector cutscene1;
        [SerializeField] private PlayableDirector cutscene2;

        public void Start()
        {
            PlayerScript.Instance.MechanicsOnOff(false);
            EnemyScript.Instance.MechanicsOnOff(false);
            EnemyHealth.enemyDeath += EndLevel;
            StartCoroutine(WaitForCutscene1End());
        }
    

        IEnumerator WaitForCutscene1End()
        {
            yield return new WaitForSeconds(0.1f);
            foreach (var canvas in FindObjectsOfType<Canvas>(true))
            {
                if(canvas.gameObject.scene == SceneManager.GetSceneByName("HUD Scene"))canvas.gameObject.SetActive(false);
            }
            yield return new WaitForSeconds((float) cutscene1.duration - 0.1f);
            foreach (var canvas in FindObjectsOfType<Canvas>(true))
            {
                if (canvas.gameObject.scene == SceneManager.GetSceneByName("HUD Scene")) canvas.gameObject.SetActive(true);
            }
            PlayerScript.Instance.MechanicsOnOff(true);
            EnemyScript.Instance.MechanicsOnOff(true);
        }

        void EndLevel() => StartCoroutine(PlayCutscene2());

        IEnumerator PlayCutscene2()
        {
            yield return new WaitForSeconds(2f);
            PlayerScript.Instance.MechanicsOnOff(false);
            EnemyScript.Instance.MechanicsOnOff(false);
            foreach (var canvas in FindObjectsOfType<Canvas>(true))
                if (canvas.gameObject.scene == SceneManager.GetSceneByName("HUD Scene"))
                    canvas.gameObject.SetActive(false);
            cutscene2.Play();
            yield return new WaitForSeconds((float) cutscene2.duration);
            GameManager.Instance.LoadLevel("Level 2 prototype");
        }

        void OnDestroy() => EnemyHealth.enemyDeath -= EndLevel;
    }
}
