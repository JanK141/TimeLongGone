using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    public MenuGameEventSystem EventManager;
    [SerializeField]
    public GameObject FirstButtonInPause;

    public void Resume()
    {

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
        Application.Quit();
    }


    public void MakeActive()
    {
        gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);    //validation
        EventSystem.current.SetSelectedGameObject(FirstButtonInPause);
    }

    public void MakeInactive()
    {
        gameObject.SetActive(false);
    }
}
