using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    public MainMenuGameEventSystem EventManager;
    [SerializeField]
    public GameObject FirstButtonInMain;

    void Start()
    {
        
    }

    public void Continue()
    {

    }

    public void NewGame()
    {

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
        EventSystem.current.SetSelectedGameObject(null);    //validattion
        EventSystem.current.SetSelectedGameObject(FirstButtonInMain);
    }

    public void MakeInactive()
    {
        gameObject.SetActive(false);
    }
}
