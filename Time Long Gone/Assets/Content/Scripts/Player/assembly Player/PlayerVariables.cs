using UnityEngine;

namespace Content.Scripts.Player.assembly_Player
{
    public class PlayerVariables : ScriptableObject
    {

        public struct Movement
        {
            float initialGravity;
            float normalSpeed;
            float slowSpeed;
            float rotationTime;
            float jumpHeight;
        }

        public struct Dash
        {
            float cooldown;
            float distance;
            float Time;
        }

        public struct Attack
        {
            float baseDamage;
            float cooldown;
            float radius;
            float distance;
            float chainTime;
        }

        public struct Block
        {
            float cooldown;
            private float parryWindow;
        }

        public struct Combo
        {
            float multiplayer;
            float timeout;
            float dmgCap;
        }
    }
}