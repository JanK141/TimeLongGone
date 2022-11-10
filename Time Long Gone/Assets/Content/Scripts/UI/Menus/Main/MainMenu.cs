using System;
using System.Collections;
using System.Collections.Generic;
using Content.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    public MenuGameEventSystem EventManager;
    [SerializeField]
    public GameObject FirstButtonInMain;

    private PlayerInput playerInput;

    private void OnEnable() => gameObject.GetComponent<PlayerInput>().actions.FindActionMap("Menu").Enable();

    public void Continue(){}

    [Obsolete("Obsolete")]
    public void NewGame()
    {
        GameManager.Instance.LoadLevel("Level 1 prototype");
        //playerInput.SwitchCurrentActionMap("Player");

        //SceneManager.UnloadScene(1);
        //SceneManager.LoadScene(3);

        //hud
        //SceneManager.LoadScene(4, LoadSceneMode.Additive);

        //pause
        //SceneManager.LoadScene(2, LoadSceneMode.Additive);

        //MainInputActions mainInputActions = new MainInputActions();
        //mainInputActions.Menu.Disable();
        //mainInputActions.Player.Enable();
    }

    public void GoToSettings()
    {
        MakeInactive();
        EventManager.SettingsMenu.MakeActive();
    }

    public void ExitGame() => Application.Quit();


    public void MakeActive()
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<PlayerInput>().actions.FindActionMap("Menu").Enable();
        EventSystem.current.SetSelectedGameObject(null);    //validation
        EventSystem.current.SetSelectedGameObject(FirstButtonInMain);
    }

    public void MakeInactive() => gameObject.SetActive(false);
}
