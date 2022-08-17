using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts.UI.HUD
{
    public class BossHealthbarHUD : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private IntVariable hp;

        private void Start() => hp.OnValueChange += UpdateHealth;

        private void UpdateHealth()
        {
            slider.DOValue((float)hp.Value/ hp.OriginalValue, 0.5f);
            slider.transform.DOShakePosition(0.2f);
        }

        private void OnDestroy() => hp.OnValueChange -= UpdateHealth;
    }
}
