using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField]
    public MenuGameEventSystem EventManager;
    [SerializeField]
    public GameObject FirstButtonInSettings;
    [SerializeField]
    public IntVariable DifficultyIndicator;

    public void BackToMainMenu()
    {
        MakeInactive();
        EventManager.MainMenu.MakeActive();
        
    }

    public void SetDifficulty(int val)
    {
        switch (val)
        {
            case 0:    //easy
                Debug.Log("Chosen: 0");
                DifficultyIndicator.Value = 1;
                Debug.Log("Difficulty is set to: " + DifficultyIndicator.Value);
                break;

            case 1:   //normal
                Debug.Log("Chosen: 1");
                DifficultyIndicator.Value = 2;
                Debug.Log("Difficulty is set to: " + DifficultyIndicator.Value);
                break;
            case 2:   //hard
                Debug.Log("Chosen: 2");
                DifficultyIndicator.Value = 3;
                Debug.Log("Difficulty is set to: " + DifficultyIndicator.Value);
                break;
            case 3:   //very hard
                Debug.Log("Chosen: 3");
                DifficultyIndicator.Value = 4;
                Debug.Log("Difficulty is set to: " + DifficultyIndicator.Value);
                break;
            case 4:   //impossible
                Debug.Log("Chosen: 4");
                DifficultyIndicator.Value = 5;
                Debug.Log("Difficulty is set to: " + DifficultyIndicator.Value);
                break;
        }
    }

    public void GoToGameplay()
    {
        MakeInactive();
        EventManager.GameplaySettings.MakeActive();
    }

    public void GoToAudio()
    {
        MakeInactive();
        EventManager.AudioSettings.MakeActive();
    }

    public void GoToVideo()
    {
        MakeInactive();
        EventManager.VideoSettings.MakeActive();
    }

    public void BackToPauseMenu()
    {
        MakeInactive();
        EventManager.PauseMenu.MakeActive();
    }

    public void MakeActive()
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<PlayerInput>().actions.FindActionMap("Menu").Enable();
        EventSystem.current.SetSelectedGameObject(null);    //validation
        EventSystem.current.SetSelectedGameObject(FirstButtonInSettings);
    }

    public void MakeInactive()
    {
        gameObject.SetActive(false);
    }
}
