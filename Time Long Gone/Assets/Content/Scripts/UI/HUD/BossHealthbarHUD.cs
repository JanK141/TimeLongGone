using Content.Scripts.Enemy;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.UI.HUD
{
    public class BossHealthbarHUD : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private IntVariable hp;

        void Start()
        {
            hp.OnValueChange += UpdateHealth;
        }

        void UpdateHealth()
        {
            slider.DOValue((float)hp.Value/ hp.OriginalValue, 0.5f);
            slider.transform.DOShakePosition(0.2f);
        }

        void OnDestroy()
        {
            hp.OnValueChange -= UpdateHealth;
        }
    }
}
