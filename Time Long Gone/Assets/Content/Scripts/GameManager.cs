using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    private PlayerInput playerInput;


    Scene mainMenuScene;
    Scene pauseScene;
    Scene HUDScene;
    Scene lvlScene1;

    // Start is called before the first frame update
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

        mainMenuScene = SceneManager.GetSceneByBuildIndex(1);
        pauseScene = SceneManager.GetSceneByBuildIndex(2);
        HUDScene = SceneManager.GetSceneByBuildIndex(4);
        lvlScene1 = SceneManager.GetSceneByBuildIndex(3);


        //MainInputActions mainInputActions = new MainInputActions();
        //mainInputActions.Player.Disable();
        //mainInputActions.Menu.Enable();

        
    }

    private void Start()
    {
        //playerInput = GetComponent<PlayerInput>();
        //playerInput.SwitchCurrentActionMap("Menu");
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
    }

    public void GoToLevel1()
    {
        SceneManager.LoadScene(lvlScene1.buildIndex, LoadSceneMode.Additive);
        SceneManager.LoadScene(HUDScene.buildIndex, LoadSceneMode.Additive);
        SceneManager.LoadScene(pauseScene.buildIndex, LoadSceneMode.Additive);
    }

}
