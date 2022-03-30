using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VideoSettings : MonoBehaviour
{
    [SerializeField]
    public MenuGameEventSystem EventManager;
    [SerializeField]
    public GameObject FirstButtonInVideo;

    public void BackToSettings()
    {
        MakeInactive();
        EventManager.SettingsMenu.MakeActive();
    }


    public void MakeActive()
    {
        gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);    //validation
        EventSystem.current.SetSelectedGameObject(FirstButtonInVideo);
    }

    public void MakeInactive()
    {
        gameObject.SetActive(false);
    }
}
