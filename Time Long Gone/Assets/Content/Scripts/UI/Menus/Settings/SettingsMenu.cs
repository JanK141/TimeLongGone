using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField]
    public MenuGameEventSystem EventManager;
    [SerializeField]
    public GameObject FirstButtonInSettings;

    public void BackToMainMenu()
    {
        MakeInactive();
        EventManager.MainMenu.MakeActive();
        
    }

    public void GoToGameplay()
    {
        MakeInactive();
        EventManager.GameplaySettings.MakeActive();
    }

    public void GoToAudio()
    {
        MakeInactive();
        EventManager.AudioSettings.MakeActive();
    }

    public void GoToVideo()
    {
        MakeInactive();
        EventManager.VideoSettings.MakeActive();
    }

    public void BackToPauseMenu()
    {
        MakeInactive();
        EventManager.PauseMenu.MakeActive();
    }

    public void MakeActive()
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<PlayerInput>().actions.FindActionMap("Menu").Enable();
        EventSystem.current.SetSelectedGameObject(null);    //validation
        EventSystem.current.SetSelectedGameObject(FirstButtonInSettings);
    }

    public void MakeInactive()
    {
        gameObject.SetActive(false);
    }
}
