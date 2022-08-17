using Content.Scripts.Player;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Content.Scripts
{
    public class ComboTestUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        private float _timeout;
        private bool _scaleDown;
        private Vector3 _currentScale;

        private void Start()
        {
            _timeout = PlayerScript.Instance.combat.comboTimeout;
            text.text = "";
            PlayerScript.OnComboContinue += UpdateCombo;
        }

        private void Update()
        {
            if (_scaleDown) 
                text.rectTransform.localScale -= _currentScale / _timeout * Time.deltaTime;
        }

        void UpdateCombo(int combo)
        {
            if (combo > 0)
            {
                text.text = combo.ToString();
                _currentScale = Vector3.one * Mathf.Min(1 + 0.05f * combo, 2f);
                text.rectTransform.localScale = _currentScale;
                _scaleDown = false;
                text.rectTransform.DOPunchScale(Vector3.one * Mathf.Min(0.05f * combo, 1.5f), 0.2f)
                    .OnComplete(() => _scaleDown = true);
            }
            else
                text.text = "";
        }

        void OnDestroy() => PlayerScript.OnComboContinue -= UpdateCombo;
    }
}
