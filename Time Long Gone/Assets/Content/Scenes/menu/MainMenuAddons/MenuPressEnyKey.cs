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
    private PlayableDirector playableDirector;

    void Start()
    {
        text.DOFade(0, 1).SetLoops(-1, LoopType.Yoyo);
        playableDirector = GetComponent<PlayableDirector>();

        InputSystem.onAnyButtonPress.CallOnce((action) => { 
            playableDirector.Play();
            text.DOKill();
            text.gameObject.SetActive(false);
            enabled = false; 
        });
    }

  
}
