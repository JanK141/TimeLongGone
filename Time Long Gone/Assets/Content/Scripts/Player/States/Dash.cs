using UnityEngine;

namespace Content.Scripts.Player.States
{
    public class Dash : IPlayerState
    {
        private Vector3 direction;
        private float distance;
        private float dashTime;
        private float time;
        private float[] iframesTime;


        public void OnstateEnter()
        {
            throw new System.NotImplementedException();
        }

        public void OnStateExit()
        {
            throw new System.NotImplementedException();
        }

        public void Tick()
        {
            throw new System.NotImplementedException();
        }

        public IPlayerState Evalueate()
        {
            throw new System.NotImplementedException();
        }
    }
}