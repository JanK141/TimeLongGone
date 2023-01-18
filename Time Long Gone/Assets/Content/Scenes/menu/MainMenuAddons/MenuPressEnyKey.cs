using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Playables;
using DG.Tweening;

public class MenuPressEnyKey : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] PlayableAsset animation;
    [SerializeField] private PlayableDirector playableDirector;


    void Start()
    {
        Time.timeScale = 1;
        text.DOFade(0, 1).SetLoops(-1, LoopType.Yoyo);
        //playableDirector = GetComponent<PlayableDirector>();

        InputSystem.onAnyButtonPress.CallOnce((action) => {
            playableDirector.gameObject.SetActive(true);
            //playableDirector.time = 0;
            //playableDirector.Stop();
            //playableDirector.Evaluate();
            playableDirector.Play();
            //playableDirector.Resume();
            //text.DOKill();
            text.gameObject.SetActive(false);
            //enabled = false; 
            print("MenuStart");
        });
    }

}
