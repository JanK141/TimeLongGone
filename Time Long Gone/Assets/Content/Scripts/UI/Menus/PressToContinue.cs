using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PressToContinue : MonoBehaviour
{
    private bool isLoaded = false;
    public bool IsLoaded
    {
        get => isLoaded;
        set { isLoaded = value;  StartFading();}
    }

    private TextMeshProUGUI text;

    void Start()
    {
        SetText();
    }

    void StartFading()
    {
        if(IsLoaded) GetComponent<CanvasGroup>().DOFade(1, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void Continue(InputAction.CallbackContext context)
    {
        //GameManager.Instance
        if (IsLoaded && context.started)
        {
            print("Continue");
            GameManager.Instance.FinishLoading();
        }
    }

    public void SetText()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = "Press " + ((GetComponent<PlayerInput>().currentControlScheme.Equals("gamepad"))
                                 ? Between(GetComponent<PlayerInput>().actions.FindAction("Submit").bindings[2].ToString(),"/","[")
                                 : Between(GetComponent<PlayerInput>().actions.FindAction("Submit").bindings[0].ToString(),"/","["))
                             + " to continue";
    }

    private string Between(string STR, string FirstString, string LastString)
    {
        string FinalString;
        int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
        int Pos2 = STR.IndexOf(LastString);
        FinalString = STR.Substring(Pos1, Pos2 - Pos1);
        return FinalString;
    }
}
