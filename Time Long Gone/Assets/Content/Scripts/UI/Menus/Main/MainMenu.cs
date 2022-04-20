using System.Collections;
using System.Collections.Generic;
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

    //private PlayerInput playerInput;

    void Awake()
    {
        //playerInput = GetComponent<PlayerInput>();
        //playerInput.SwitchCurrentActionMap("Menu");
    }

    public void Continue()
    {

    }

    public void NewGame()
    {
        SceneManager.UnloadScene(1);
        SceneManager.LoadScene(3);

        //hud
        SceneManager.LoadScene(4, LoadSceneMode.Additive);

        //pause
        SceneManager.LoadScene(2, LoadSceneMode.Additive);

        //MainInputActions mainInputActions = new MainInputActions();
        //mainInputActions.Menu.Disable();
        //mainInputActions.Player.Enable();
    }

    public void GoToSettings()
    {
        MakeInactive();
        EventManager.SettingsMenu.MakeActive();
    }

    public void ExitGame()
    {
        Application.Quit();
    }


    public void MakeActive()
    {
        gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);    //validation
        EventSystem.current.SetSelectedGameObject(FirstButtonInMain);
    }

    public void MakeInactive()
    {
        gameObject.SetActive(false);
    }
}
