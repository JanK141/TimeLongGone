using System;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Mechanics;
using Content.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;
using Time = UnityEngine.Time;

public class ManaBarHUD : MonoBehaviour
{
    public static ManaBarHUD Instance;
    public static float TickRate;

    public static event Action<bool> OnRewindChange; 

    //SERIALIZED FIELDS
    [SerializeField] [Min(1)] private float maxMana = 100;
    [SerializeField] private float startingMana;
    [SerializeField] private Slider slider;
    [SerializeField] [Tooltip("How many points of mana are generated per second")] private float genRate = 5f;
    [SerializeField] [Tooltip("Time for lerping slider value")] private float DoValueTime;
    [SerializeField] [Tooltip("Tick speed for Mana updates")] private float tickRate;
    [SerializeField] [Tooltip("Mana cost per second while using slow mo")] private float slowMoCost;
    [SerializeField] [Tooltip("Flat amount of mana used to start rewinding time")] private float flatRewindCost;
    [SerializeField] [Tooltip("Mana cost per second while rewinding")] public float rewindCost;

    //PUBLIC VARIABLES
    private float currMana;

    //PRIVATE VARIABLES
    private bool generating;
    private bool rewinding;
    private bool slowing;


    //PROPERTIES
    public float CurrMana
    {
        get => currMana;
        set { currMana = value; UpdateMana(); }
    }

    void Awake()
    {
        TickRate = tickRate;
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        CurrMana = startingMana;
        generating = true;
        rewinding = false;
        slowing = false;
    }

    void Update()
    {
        if (CurrMana <= maxMana && generating)
        {
            CurrMana += genRate * Time.deltaTime;
        }
        Mathf.Clamp(CurrMana, 0, maxMana);

        if (slowing)
        {
            CurrMana -= slowMoCost * Time.unscaledDeltaTime;
            if (CurrMana <= 0)
            {
                CurrMana = 0;
                StopSlowingTime();
            }
        }

        if (rewinding)
        {
            CurrMana -= rewindCost * Time.unscaledDeltaTime;
            if (CurrMana <= 0)
            {
                CurrMana = 0;
                StopRewindingTime();
            }
        }
    }
    
    void UpdateMana()
    {
        slider.value = CurrMana / maxMana;
    }

    //method triggered by pressing the RewindTime button
    void StartRewindingTime()
    {
        if (CurrMana >= flatRewindCost)
        {
            rewinding = true;
            generating = false;
            CurrMana -= flatRewindCost;
            OnRewindChange?.Invoke(true);
        }
    }

    //method triggered by releasing the RewindTime button
    void StopRewindingTime()
    {
        rewinding = false;
        generating = true;
        OnRewindChange?.Invoke(false);
    }

    //method triggered by pressing the SlowTime button
    public void StartSlowingTime()
    {
        if (!PlayerScript.Instance.IsAlive && !rewinding)
        {
            PlayerScript.Instance.IsAlive = true;
            StartRewindingTime();
            return;
        }
        if (CurrMana >= slowMoCost && !slowing)
        {
            slowing = true;
            generating = false;
            Controller.Instance.ProcessSlowMo(true);
        }
    }

    //method triggered by releasing the SlowTime button
    public void StopSlowingTime()
    {
        if (rewinding)
        {
            StopRewindingTime();
            return;
        }
        if(!slowing) return;
        slowing = false;
        generating = true;
        Controller.Instance.ProcessSlowMo(false);
    }

}
