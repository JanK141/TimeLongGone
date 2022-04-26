using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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
        gameObject.GetComponent<PlayerInput>().actions.FindActionMap("Menu").Enable();
        EventSystem.current.SetSelectedGameObject(null);    //validation
        EventSystem.current.SetSelectedGameObject(FirstButtonInAudio);
    }

    public void MakeInactive()
    {
        gameObject.SetActive(false);
    }
}
