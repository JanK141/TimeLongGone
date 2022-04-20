using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PausingScript : MonoBehaviour
{
    public static PausingScript Instance { get; private set; }

    public GameObject canvas;
    public PauseMenu menu;

    private PlayerInput playerInput;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        playerInput = GetComponent<PlayerInput>();
    }

    public void Pausing()
    {
        Time.timeScale = 0;
        //MainInputActions mainInputActions = new MainInputActions();
        //mainInputActions.Player.Disable();
        //mainInputActions.Menu.Enable();

        playerInput.SwitchCurrentActionMap("Menu");

        canvas.SetActive(true);
        menu.MakeActive();
    }

    public void Unpausing()
    {
        //MainInputActions mainInputActions = new MainInputActions();
        //mainInputActions.Player.Enable();
        //mainInputActions.Menu.Disable();

        playerInput.SwitchCurrentActionMap("Player");

        canvas.SetActive(false);
        menu.MakeInactive();
        Time.timeScale = 1;
    }
}
