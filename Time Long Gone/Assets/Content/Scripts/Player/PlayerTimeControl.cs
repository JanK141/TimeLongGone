using Content.Scripts.Variables;
using Enemy;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Player
{
    public class PlayerTimeControl : MonoBehaviour
    {
        private BoolVariable IsRewinding;
        [SerializeField] private float targetSlowMoScale;
        [SerializeField] private float targetDeathScale;
        [SerializeField] private float rewindTimeScale;
        [SerializeField] private float timeToInterpolate;

        private float mana;
        public float Mana { get => mana; set{ mana = Mathf.Clamp(value, 0, MaxMana); } }
        public float MaxMana { get; private set; }
        public bool WantsToTimeControl { get; set; } = false;
        public bool ActiveTime { get; set; } = true;

        private Player player;
        private PlayerVariables variables;
        private TimeState currState;

        private void Awake()
        {
            IsRewinding = Resources.Load<BoolVariable>("Rewind/IsRewinding");
        }
        void Start()
        {
            player = GetComponent<Player>();
            variables = player.variables;
            MaxMana = variables.mana;
            Mana = variables.mana;
            currState = new NormalFlow(true);
        }

        void Update()
        {
            if (!ActiveTime) return;
            currState.Tick(this);
            var state = currState.Evaluate(this);
            if(state != null)
            {
                currState.Exit(this);
                currState = state;
                currState.Enter(this);
            }
        }

        private interface TimeState {
            public void Enter(PlayerTimeControl control);
            public void Tick(PlayerTimeControl control);
            public TimeState Evaluate(PlayerTimeControl control);
            public void Exit(PlayerTimeControl control);
        }
        private class NormalFlow : TimeState
        {
            private bool CanTimeControl;
            private float time;
            public NormalFlow(bool canTimeControl)
            {
                CanTimeControl = canTimeControl;
                time = 0f;
            }
            public NormalFlow()
            {
                CanTimeControl = false;
                time = 0f;
            }
            public void Enter(PlayerTimeControl control) => Time.timeScale = control.targetSlowMoScale;

            public TimeState Evaluate(PlayerTimeControl control)
            {
                if (control.player.CurrentState == control.player.DEAD_STATE) return new DeathFlow();
                if (CanTimeControl && control.WantsToTimeControl) return new SlowFlow();
                return null;
            }

            public void Exit(PlayerTimeControl control)
            {
            }

            public void Tick(PlayerTimeControl control)
            {
                if (Time.timeScale < 1)
                {
                    time += Time.unscaledDeltaTime;
                    Time.timeScale = Mathf.Lerp(control.targetSlowMoScale, 1, time / control.timeToInterpolate);
                    if (time >= control.timeToInterpolate)
                    {
                        Time.timeScale = 1;
                        CanTimeControl = true;
                    }
                }
                if(control.Mana < control.MaxMana)
                {
                    control.Mana += control.variables.manaRegPerSecond * Time.unscaledDeltaTime;
                }
            }
        }
        private class SlowFlow : TimeState
        {
            private float time = 0f;
            public void Enter(PlayerTimeControl control) => Time.timeScale = 1;

            public TimeState Evaluate(PlayerTimeControl control)
            {
                if (control.player.CurrentState == control.player.DEAD_STATE) return new DeathFlow();
                if (control.Mana <= 0 || !control.WantsToTimeControl) return new NormalFlow();
                return null;
            }

            public void Exit(PlayerTimeControl control)
            {
            }

            public void Tick(PlayerTimeControl control)
            {
                if(Time.timeScale > control.targetSlowMoScale)
                {
                    time += Time.unscaledDeltaTime;
                    Time.timeScale = Mathf.Lerp(1, control.targetSlowMoScale, time / control.timeToInterpolate);
                    if (time >= control.timeToInterpolate) Time.timeScale = control.targetSlowMoScale;
                }
                control.Mana -= control.variables.slowmoCostPerSecond * Time.unscaledDeltaTime;
            }
        }
        private class DeathFlow : TimeState
        {
            private float time = 0f;
            private bool CanTimeControl = false;
            public void Enter(PlayerTimeControl control) => Time.timeScale = 1;

            public TimeState Evaluate(PlayerTimeControl control)
            {
                if(control.Mana < control.variables.rewindFlatCost || time >= 10f)
                {
                    print("GAME OVER");
                    return null;
                }else if(CanTimeControl && control.WantsToTimeControl)
                {
                    return new Rewind();
                }
                return null;
            }

            public void Exit(PlayerTimeControl control)
            {
            }

            public void Tick(PlayerTimeControl control)
            {
                time += Time.unscaledDeltaTime;
                if(Time.timeScale > control.targetDeathScale)
                {
                    Time.timeScale = Mathf.Lerp(1, control.targetDeathScale , time / control.timeToInterpolate);
                    if (time >= control.timeToInterpolate) { 
                        Time.timeScale = control.targetDeathScale; 
                        CanTimeControl = true;
                    }
                }
            }
        }
        private class Rewind : TimeState
        {
            private float needsMoreTime = 0f;
            private float additionalTime = 0f;
            public void Enter(PlayerTimeControl control)
            {
                Time.timeScale = control.rewindTimeScale;
                control.IsRewinding.Value = true;
                control.Mana -= control.variables.rewindFlatCost;
            }

            public TimeState Evaluate(PlayerTimeControl control)
            {
                if(needsMoreTime == 0)
                {
                    if(control.Mana <= 0 || !control.WantsToTimeControl)
                    {
                        needsMoreTime = control.player.combat.enemy.RewindTimeNeeded();
                        if (needsMoreTime == 0)
                        {
                            if (control.player.CurrentState == control.player.DEAD_STATE) return new DeathFlow();
                            else return new NormalFlow();
                        }
                        else return null;
                    }
                }
                else
                {
                    if(additionalTime >= needsMoreTime)
                    {
                        if (control.player.CurrentState == control.player.DEAD_STATE) return new DeathFlow();
                        else return new NormalFlow();
                    }
                }
                return null;
            }

            public void Exit(PlayerTimeControl control)
            {
                control.IsRewinding.Value = false;
            }

            public void Tick(PlayerTimeControl control)
            {
                if (needsMoreTime == 0)
                {
                    control.Mana -= control.variables.rewindCostPerSecond * Time.unscaledDeltaTime;
                }
                else
                {
                    additionalTime += Time.deltaTime;
                }
            }
        }
    }

}