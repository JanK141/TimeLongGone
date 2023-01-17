using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] public GameObject Canvas;
    [SerializeField] public GameObject Title;
    [SerializeField] public GameObject Flavour;
    [SerializeField] public GameObject Instruction;
    [SerializeField] public GameObject Buttons;

    [SerializeField, Tooltip("json format")]
    public TextAsset dataFile;

    private List<TutorialData> _tutorials;
    private int _currentTutorial;

    private Button _playButton;
    private Button _skipButton;
    private bool _playButtonPressed;
    private bool _skipButtonPressed;
    private Text _title;
    private Text _flavour;
    private Text _instruction;

    public string GetInstruction(int nr) => _tutorials[nr].instruction;

    private void Start()
    {
        _tutorials = JsonUtility.FromJson<JsonTutorialList>(dataFile.text).tutorialsData;
        _title = Title.GetComponent<Text>();
        _flavour = Flavour.GetComponent<Text>();
        _instruction = Instruction.GetComponent<Text>();

        _playButton = GameObject.Find("T_Play_Button").GetComponent<Button>();
        _skipButton = GameObject.Find("T_Skip_Button").GetComponent<Button>();

        _playButton.onClick.AddListener(OnNextButtonPressed);
        Canvas.SetActive(true);
        RunTutorialWindow(0);
    }

    public bool IsClicked()
    {
        if (!_playButtonPressed) return false;

        _playButtonPressed = false;
        return true;
    }

    public void RunTutorialWindow(int nrTutorial)
    {
        Canvas.SetActive(true);
        DisplayText(nrTutorial);

        if (nrTutorial == 0)
            Buttons.SetActive(true);
        else
        {
            Buttons.SetActive(true);
        }
    }

    public void CloseTutorial()
    {
        Buttons.SetActive(false);
        Canvas.SetActive(false);
    }

    private void OnNextButtonPressed()
    {
        CloseTutorial();
        _playButtonPressed = true;
    }

    public void OnSkipButtonPressed()
    {
        // TODO: make skip action.
        if (_currentTutorial == 0)
        {
            print("Skip");
            Destroy(FindObjectOfType<TutorialScript>());
        }
    }


    private void DisplayText(int i)
    {
        _title.text = _tutorials[i].title;
        _flavour.text = _tutorials[i].flavour;
        _instruction.text = _tutorials[i].instruction;
    }

}