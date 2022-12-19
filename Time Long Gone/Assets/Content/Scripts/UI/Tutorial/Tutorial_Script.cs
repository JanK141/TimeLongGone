using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial_Script : MonoBehaviour
{
    [SerializeField]
    public GameObject Canvas;
    [SerializeField]
    public GameObject Title;
    [SerializeField]
    public GameObject Flavour;
    [SerializeField]
    public GameObject Instruction;
    [SerializeField]
    public GameObject Buttons;
    [SerializeField]
    public TextAsset TxtFile;

    private int N_tuts;
    private int N_fields;
    private string[,] source;

    void Start()
    {
        N_tuts = 12;
        N_fields = 3;
        source = new string[N_tuts, N_fields];

        string[] s = TxtFile.text.Split('/');
        for (int i = 0; i < N_tuts; i++)
        {
            string[] temp = s[i].Split('\\');
            for (int j = 0; j < N_fields; j++)
            {
                source[i, j] = temp[j];
            }
        }
    }

    public void DisplayText(int i)
    {
        Title.gameObject.GetComponent<Text>().text = source[i, 0];
        Flavour.gameObject.GetComponent<Text>().text = source[i, 1];
        Instruction.gameObject.GetComponent<Text>().text = source[i, 2];
    }

    public void Run_T0_Intro()
    {
        Canvas.SetActive(true);
        DisplayText(0);

        Buttons.SetActive(true);
    }

    public void Close_T0_Intro()
    {
        Buttons.SetActive(false);
        Canvas.SetActive(false);
    }
    public void Close_T()
    {
        Canvas.SetActive(false);
    }

    public void Run_T1_Movement()
    {
        Canvas.SetActive(true);
        DisplayText(1);
    }

    public void Run_T2_Attacks()
    {
        Canvas.SetActive(true);
        DisplayText(2);
    }

    public void Run_T3_CombosAndFinishers()
    {
        Canvas.SetActive(true);
        DisplayText(3);
    }

    public void Run_T4_ChargeAttack()
    {
        Canvas.SetActive(true);
        DisplayText(4);
    }

    public void Run_T5_Blocks()
    {
        Canvas.SetActive(true);
        DisplayText(5);
    }

    public void Run_T6_DodgesAndParries()
    {
        Canvas.SetActive(true);
        DisplayText(6);
    }

    public void Run_T7_Kicks()
    {
        Canvas.SetActive(true);
        DisplayText(7);
    }

    public void Run_T8_Finishers()
    {
        Canvas.SetActive(true);
        DisplayText(8);
    }

    public void Run_T9_TimeRewind()
    {
        Canvas.SetActive(true);
        DisplayText(9);
    }

    public void Run_T10_TimeDilution()
    {
        Canvas.SetActive(true);
        DisplayText(10);
    }

    public void Run_T11_End()
    {
        Canvas.SetActive(true);
        DisplayText(11);
    }
}
