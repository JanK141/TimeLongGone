using UnityEngine;
using UnityEngine.InputSystem;

public class PausingScript : MonoBehaviour
{
    public static PausingScript Instance { get; private set; }

    public GameObject canvas;
    public PauseMenu menu;


    private Player.InputIntermediary playerInput;
    private Player.PlayerTimeControl playerControl;
    private float restoreTimeScale = 1;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    private void Start()
    {
        playerInput = FindObjectOfType<Player.InputIntermediary>();
        playerControl = playerInput.GetComponent<Player.PlayerTimeControl>();
    }

    public void Pausing()
    {
        restoreTimeScale = Time.timeScale;
        Time.timeScale = 0;
        playerInput.enabled = false;
        playerInput.GetComponent<PlayerInput>().enabled = false;
        playerControl.WantsToTimeControl = false;
        playerControl.ActiveTime = false;
        canvas.SetActive(true);
        menu.MakeActive();
    }

    public void Unpausing()
    {
        canvas.SetActive(false);
        Time.timeScale = restoreTimeScale;
        playerInput.enabled = true;
        playerInput.GetComponent<PlayerInput>().enabled = true;
        playerControl.ActiveTime = true;
    }
}
