using Content.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PausingScript : MonoBehaviour
{
    public static PausingScript Instance { get; private set; }

    public GameObject canvas;
    public PauseMenu menu;


    private PlayerScript playerScript;
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
    }

    private void Start()
    {
        playerScript = PlayerScript.Instance;
    }

    public void Pausing()
    {
        Time.timeScale = 0;
        playerScript.GetComponent<PlayerInput>().enabled = false;
        canvas.SetActive(true);
        menu.MakeActive();
    }

    public void Unpausing()
    {
        playerScript.GetComponent<PlayerInput>().enabled = true;
        canvas.SetActive(false);
        Time.timeScale = 1;
    }
}
