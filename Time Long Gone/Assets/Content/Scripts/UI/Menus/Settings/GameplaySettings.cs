using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameplaySettings : MonoBehaviour
{
    [SerializeField]
    public MenuGameEventSystem EventManager;
    [SerializeField]
    public GameObject FirstButtonInGameplay;

    public void BackToSettings()
    {
        MakeInactive();
        EventManager.SettingsMenu.MakeActive();
    }


    public void MakeActive()
    {
        gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);    //validation
        EventSystem.current.SetSelectedGameObject(FirstButtonInGameplay);
    }

    public void MakeInactive()
    {
        gameObject.SetActive(false);
    }
}
