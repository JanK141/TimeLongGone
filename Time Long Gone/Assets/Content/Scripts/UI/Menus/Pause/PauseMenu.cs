using System.Collections;
using System.Collections.Generic;
using Content.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    public MenuGameEventSystem EventManager;
    [SerializeField]
    public GameObject FirstButtonInPause;
    [SerializeField]
    public PausingScript pausingScript;

    public void Resume()
    {
        MakeInactive();
        pausingScript.Unpausing();
    }

    public void GoToSettings()
    {
        MakeInactive();
        EventManager.SettingsMenu.MakeActive();
    }

    public void BackToMainMenu()
    {
        //unload lvl?
    }

    public void ExitGame()
    {
        //Application.Quit();
        GameManager.Instance.ExitToMenu();
    }


    public void MakeActive()
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<PlayerInput>().actions.FindActionMap("Menu").Enable();
        EventSystem.current.SetSelectedGameObject(null);    //validation
        EventSystem.current.SetSelectedGameObject(FirstButtonInPause);
    }

    public void MakeInactive()
    {
        gameObject.SetActive(false);
    }
}
