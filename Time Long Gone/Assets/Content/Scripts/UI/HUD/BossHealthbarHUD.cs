using Content.Scripts.Enemy;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.UI.HUD
{
    public class BossHealthbarHUD : MonoBehaviour
    {
        [SerializeField] private Slider slider;

        void Start()
        {
            EnemyScript.OnEnemyHeatlhChange += UpdateHealth;
        }

        void UpdateHealth(int max, int curr)
        {
            slider.DOValue((max / curr), 0.5f);
            slider.transform.DOShakePosition(0.2f);
        }

        void OnDestroy()
        {
            EnemyScript.OnEnemyHeatlhChange -= UpdateHealth;
        }
    }
}
