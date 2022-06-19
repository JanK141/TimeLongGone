using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Enemy;
using Content.Scripts.Player;
using UnityEngine;
using UnityEngine.Playables;

public class Level1Manager : MonoBehaviour
{
    [SerializeField] private PlayableDirector cutscene1;
    [SerializeField] private PlayableDirector cutscene2;
    // Start is called before the first frame update
    void Start()
    {
        PlayerScript.Instance.MechanicsOnOff(false);
        EnemyScript.Instance.MechanicsOnOff(false);
        EnemyHealth.enemyDeath += EndLevel;
        StartCoroutine(WaitForCutscene1End());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitForCutscene1End()
    {
        yield return new WaitForSeconds((float) cutscene1.duration);
        PlayerScript.Instance.MechanicsOnOff(true);
        EnemyScript.Instance.MechanicsOnOff(true);
    }

    void EndLevel()
    {
        StartCoroutine(PlayCutscene2());
    }

    IEnumerator PlayCutscene2()
    {
        yield return new WaitForSeconds(2f);
        PlayerScript.Instance.MechanicsOnOff(false);
        EnemyScript.Instance.MechanicsOnOff(false);
        cutscene2.Play();
    }

    void OnDestroy()
    {
        EnemyHealth.enemyDeath -= EndLevel;
    }
}
