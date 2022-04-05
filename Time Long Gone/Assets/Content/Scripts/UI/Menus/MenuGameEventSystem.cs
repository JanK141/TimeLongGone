using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGameEventSystem : MonoBehaviour
{
    [SerializeField]
    public MenuGameEventSystem Instance;
    [SerializeField]
    public MainMenu MainMenu;
    [SerializeField]
    public SettingsMenu SettingsMenu;
    [SerializeField]
    public GameplaySettings GameplaySettings;
    [SerializeField]
    public AudioSettings AudioSettings;
    [SerializeField]
    public VideoSettings VideoSettings;
    [SerializeField]
    public PauseMenu PauseMenu;


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
