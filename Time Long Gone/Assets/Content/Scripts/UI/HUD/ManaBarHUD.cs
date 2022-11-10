using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Player;
using Content.Scripts.tim;
using UnityEngine;
using UnityEngine.UI;
using Time = UnityEngine.Time;

public class ManaBarHUD : MonoBehaviour
{
    public static ManaBarHUD Instance;
    public static float TickRate;

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

        if (slowing) CurrMana -= slowMoCost * Time.unscaledDeltaTime;
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
            //doRewind(1s);
            InvokeRepeating("RewindOneTick", 0, tickRate);
        }
    }

    //method triggered by releasing the RewindTime button
    void StopRewindingTime()
    {
        rewinding = false;
        generating = true;
        CancelInvoke("RewindOneTick");
    }

    //method rewinding time continuously, while the RewindTime button is pressed
    void RewindOneTick()
    {
        if (rewinding)
        {
            if (CurrMana >= rewindCost * tickRate)
            {
                CurrMana -= rewindCost * tickRate;
                //doRewind();
            }
            else
            {
                rewinding = false;
                generating = true;
            }
        }
    }

    //method triggered by pressing the SlowTime button
    public void StartSlowingTime()
    {
        if (!PlayerScript.Instance.IsAlive)
        {
            PlayerScript.Instance.IsAlive = true;
            StartRewindingTime();
            return;
        }
        if (CurrMana >= slowMoCost)
        {
            slowing = true;
            generating = false;
            Controller.Instance.ProcessSlowMo(true);
        }
    }

    //method triggered by releasing the SlowTime button
    public void StopSlowingTime()
    {
        if (!PlayerScript.Instance.IsAlive)
        {
            StopRewindingTime();
            return;
        }
        slowing = false;
        generating = true;
        Controller.Instance.ProcessSlowMo(false);
    }

    //method slowing time continuously, while the SlowTime button is pressed
    void SlowOneTick()
    {
        if (slowing)
        {
            if (CurrMana >= slowMoCost * tickRate)
            {
                CurrMana -= slowMoCost * tickRate;
                //doSlow(TickRate);
            }
            else
            {
                slowing = false;
                generating = true;
            }
        }
    }

    //TODO Slow- mo nie dzia³a per-tick ani nic w tym stylu. Czas zostaje raz spowolniony i taki zostaje, albo raz zresetowany i taki zostaje.  Jest to single action process
    //TODO w zwi¹zku z czym mamy tu sporo totalnie zbêdnego kodu. Mana mo¿e siê regenerowaæ per-frame w Update(), nie ma ¿adnych przeciwstawieñ ku temu.
    //TODO Tak samo to za³o¿enie "per-tick" jest zbyt przekombinowane i zdecydowanie nie optymalne zwa¿aj¹c na iloœæ yieldów. Równie dorbze mo¿na to zast¹piæ pojedynczym eventem przekazuj¹cym boola (start/finish)

    //TODO Po za tym za du¿o odwo³añ wszêdzie siê robi. ManaBarHUD powinien odpowiadaæ jak nazwa mówi jednak tylko za ManaBarHUD a nie manê sam¹ w sobie te¿
    //TODO przepakowaæ czêœæ zwi¹zan¹ z man¹ do skrpytu odpowiedzialnego za sam¹ manê i niech informuje HUD jedynie o zmianach wartoœci eventem. Read more at PlayerScipt.cs oraz PlayerInput.cs
    //??????
}
