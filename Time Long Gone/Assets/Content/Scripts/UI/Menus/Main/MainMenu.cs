using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    public MenuGameEventSystem EventManager;
    [SerializeField]
    public GameObject FirstButtonInMain;

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
        EventSystem.current.SetSelectedGameObject(null);    //validation
        EventSystem.current.SetSelectedGameObject(FirstButtonInMain);
    }

    public void MakeInactive()
    {
        gameObject.SetActive(false);
    }
}
