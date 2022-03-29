using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SettingsMainMenu : MonoBehaviour
{
    [SerializeField]
    public MainMenuGameEventSystem EventManager;
    [SerializeField]
    public GameObject FirstButtonInSettings;

    public void BackToMainMenu()
    {
        MakeInactive();
        EventManager.MainMenu.MakeActive();
    }


    public void MakeActive()
    {
        gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);    //validattion
        EventSystem.current.SetSelectedGameObject(FirstButtonInSettings);
    }

    public void MakeInactive()
    {
        gameObject.SetActive(false);
    }
}
