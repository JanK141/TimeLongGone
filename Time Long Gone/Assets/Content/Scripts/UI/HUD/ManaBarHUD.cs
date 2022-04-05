using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBarHUD : MonoBehaviour
{
    public static ManaBarHUD Instance;

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
        //what is the purpose of the above line? this does NOT trigger every time currMana value is changed, as shown in current form of the script
        //left the momented commented out wherever it will be needed, ready to be brought back
    }

    void Awake() => Instance = this;

    // Start is called before the first frame update
    void Start()
    {
        CurrMana = startingMana;
        generating = true;
        rewinding = false;
        slowing = false;
        //UpdateMana();
    }

    void Update()
    {
        if (CurrMana <= maxMana && generating)
        {
            CurrMana += genRate * Time.unscaledDeltaTime;
            //UpdateMana();
        }
        Mathf.Clamp(CurrMana, 0, maxMana);
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
            //UpdateMana();
            //doRewind(1s);
            InvokeRepeating("RewindOneTick", 0, tickRate);
        }
    }

    //method triggered by releasing the RewindTime button
    void StopRewindingTime()
    {
        rewinding = false;
        generating = true;
    }

    //method rewinding time continuously, while the RewindTime button is pressed
    void RewindOneTick()
    {
        if (rewinding)
        {
            if (CurrMana >= rewindCost * tickRate)
            {
                CurrMana -= rewindCost * tickRate;
                //UpdateMana();
                //doRewind(TickRate);
            }
            else
            {
                rewinding = false;
                generating = true;
            }
        }
    }

    //method triggered by pressing the SlowTime button
    void StartSlowingTime()
    {
        if (CurrMana >= slowMoCost)
        {
            slowing = true;
            generating = false;
            //doSlow(1s);
            InvokeRepeating("SlowOneTick", 0, tickRate);
        }
    }

    //method triggered by releasing the SlowTime button
    void StopSlowingTime()
    {
        slowing = false;
        generating = true;
    }

    //method slowing time continuously, while the SlowTime button is pressed
    void SlowOneTick()
    {
        if (slowing)
        {
            if (CurrMana >= slowMoCost * tickRate)
            {
                CurrMana -= slowMoCost * tickRate;
                //UpdateMana();
                //doSlow(TickRate);
            }
            else
            {
                slowing = false;
                generating = true;
            }
        }
    }

    
}
