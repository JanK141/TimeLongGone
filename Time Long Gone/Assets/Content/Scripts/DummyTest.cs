using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DummyTest : MonoBehaviour
{
    public static DummyTest Instance;

    [SerializeField] private TextMeshProUGUI text;
    private Tweener t;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }
    void Start()
    {
        text.text = "";
        t = text.transform.DOLocalMoveY(2f, 1f).OnComplete(Reset).SetAutoKill(false).SetRecyclable(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(float value)
    {
        transform.DOShakePosition(0.5f);
        text.text = $"{value} DAMAGE!";
        t.Restart();
    }

    void Reset()
    {
        text.text = "";
        text.transform.position = Vector3.zero;
    }
}
