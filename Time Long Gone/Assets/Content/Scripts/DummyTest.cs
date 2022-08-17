using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Player;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DummyTest : MonoBehaviour
{
    public static DummyTest Instance;

    [SerializeField] private TextMeshProUGUI text;
    private Tweener t;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        text.text = "";
        t = text.transform.DOLocalMoveY(2f, 1f).OnComplete(Reset).SetAutoKill(false).SetRecyclable(true);
    }

    public void Damage(float value)
    {
        transform.DOPunchPosition(-(PlayerScript.Instance.transform.position - transform.position).normalized*0.3f, 0.15f);
        text.text = $"{value} DAMAGE!";
        t.Restart();
    }

    public void Stun()
    {
        transform.DOShakePosition(0.5f, transform.right);
        text.text = "STUNED!";
        t.Restart();
    }

    private void Reset()
    {
        text.text = "";
        text.transform.position = Vector3.zero;
    }
}
