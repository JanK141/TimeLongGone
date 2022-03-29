using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuGameEventSystem : MonoBehaviour
{
    [SerializeField]
    public MainMenuGameEventSystem Instance;
    [SerializeField]
    public MainMenu MainMenu;
    [SerializeField]
    public SettingsMainMenu SettingsMenu;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            //Destroy(this); jesli nie dziala
        }
    }


}
