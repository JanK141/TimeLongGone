using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Player;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ComboTestUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private float _timeout;
    private bool _scaleDown = false;
    private Vector3 _currentScale;

    void Start()
    {
        _timeout = PlayerScript.Instance.combat.comboTimeout;
        text.text = "";
        PlayerScript.OnComboContinue += UpdateCombo;
    }

    void Update()
    {
        if (_scaleDown)
        {
            text.rectTransform.localScale -= _currentScale/_timeout*Time.deltaTime;
        }
    }

    void UpdateCombo(int combo)
    {
        if (combo <= 0) text.text = "";
        else
        {
            text.text = combo.ToString();
            _currentScale = Vector3.one * Mathf.Min(1 + 0.05f * combo, 2f);
            text.rectTransform.localScale = _currentScale;
            _scaleDown = false;
            text.rectTransform.DOPunchScale(Vector3.one * Mathf.Min(0.05f*combo, 1.5f), 0.2f).OnComplete(()=>_scaleDown=true);
        }
    }

    void OnDestroy() => PlayerScript.OnComboContinue -= UpdateCombo;
}
