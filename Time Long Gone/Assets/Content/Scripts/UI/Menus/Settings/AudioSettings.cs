using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AudioSettings : MonoBehaviour
{
    [SerializeField]
    public MenuGameEventSystem EventManager;
    [SerializeField]
    public GameObject FirstButtonInAudio;

    public void BackToSettings()
    {
        MakeInactive();
        EventManager.SettingsMenu.MakeActive();
    }


    public void MakeActive()
    {
        gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);    //validation
        EventSystem.current.SetSelectedGameObject(FirstButtonInAudio);
    }

    public void MakeInactive()
    {
        gameObject.SetActive(false);
    }
}
