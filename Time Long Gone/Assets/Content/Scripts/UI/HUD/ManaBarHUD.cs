using UnityEngine;
using UnityEngine.UI;
using Player;

namespace HUD
{
    public class ManaBarHUD : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        private PlayerTimeControl player;

        private void Start()
        {
            player = FindObjectOfType<PlayerTimeControl>();
        }

        private void Update()
        {
            slider.value = player.Mana / player.MaxMana;
        }
    }
}
