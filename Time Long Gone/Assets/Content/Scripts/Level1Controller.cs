using Cinemachine;
using Content.Scripts;
using Content.Scripts.Camera;
using DG.Tweening;
using Enemy;
using Player;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Rendering;

public class Level1Controller : MonoBehaviour
{
    [SerializeField] private PlayableDirector cutscene1;
    [SerializeField] Player.Player player;
    [SerializeField] Enemy1 enemy;
    [SerializeField] CinemachineVirtualCamera cutscenecam;
    [SerializeField] TextMeshProUGUI skipText;
    [SerializeField] PlayableAsset finisher;
    [SerializeField] PlayableAsset end;
    [SerializeField] CinemachineVirtualCamera executionCam;
    [SerializeField] Volume DeathVolume;
    [SerializeField] TextMeshProUGUI GameOverText;

    bool _isPlaying = true;
    void Start()
    {
        player.enabled = false;
        player.GetComponent<PlayerTimeControl>().ActiveTime = false;
        enemy.ActiveAI = false;
        cutscene1.Play();

        skipText.text = "Press " + ((player.GetComponent<PlayerInput>().currentControlScheme.Equals("gamepad"))
                                 ? Between(player.GetComponent<PlayerInput>().actions.FindAction("Jump").bindings[2].ToString(), "/", "[")
                                 : Between(player.GetComponent<PlayerInput>().actions.FindAction("Jump").bindings[0].ToString(), "/", "["))
                             + " to skip";
    }

    // Update is called once per frame
    void Update()
    {
        if(_isPlaying && player.inputContext == Player.InputIntermediary.InputContext.Jump)
        {
            player.inputContext = InputIntermediary.InputContext.Nothing;
            cutscene1.time = cutscene1.duration;
            cutscene1.Evaluate();
        }
    }

    public void GameOver() => StartCoroutine(GameOverCorutine());

    public IEnumerator GameOverCorutine()
    {
        var cam = (Camera.main.GetComponent<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCamera);
        cam.LookAt = player.transform;
        yield return new WaitForSecondsRealtime(0.5f);
        float time = 0f;
        while (time < 3f)
        {
            time += Time.unscaledDeltaTime;
            DeathVolume.weight = Mathf.Lerp(0, 1, time / 3f);
            cam.m_Lens.FieldOfView = Mathf.Lerp(80, 60, time / 3f);
            yield return null;
        }
        GameOverText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1);
        time = 0;
        while (time < 0.3)
        {
            time += Time.unscaledDeltaTime;
            cam.m_Lens.FieldOfView = Mathf.Lerp(60, 1, time / 0.3f);
            yield return null;
        }
        Time.timeScale = 1;
        GameManager.Instance.LoadLevel("Level 1 map");
    }

    public void EndCutscene1() => StartCoroutine(ActivatePlayer());
    IEnumerator ActivatePlayer()
    {
        GameManager.Instance.LoadHUD(true);
        cutscenecam.enabled = false;
        yield return null;
        _isPlaying = false;
        player.enabled = true;
        enemy.ActiveAI = true;
        player.GetComponent<PlayerTimeControl>().ActiveTime = true;
    }

    public void EnemyDead()
    {
        CinemachineSwitcher.Instance.Switch(true);
        executionCam.GetComponent<PlayerCameraSet>().enabled = false;
        player.animator.Play("Idle");
        enemy.PlayAnimation("Idle", 0);
        GameManager.Instance.LoadHUD(false);
        cutscenecam.enabled = true;
        _isPlaying = true;
        player.enabled = false;
        player.GetComponent<PlayerTimeControl>().ActiveTime = false;
        enemy.ActiveAI = false;

        var playerpos = new Vector3(player.transform.position.x, player.transform.position.y - 0.5f * player.GetComponent<CharacterController>().height, player.transform.position.z);
        var midpos = Vector3.Lerp(playerpos, enemy.transform.position, 0.5f);
        var midrot = Vector3.Lerp(player.transform.rotation.eulerAngles, enemy.transform.rotation.eulerAngles, 0.5f);

        enemy.transform.DOMove(midpos, 0.5f);
        enemy.transform.DORotate(midrot, 0.5f);
        midpos.y += 0.5f * player.GetComponent<CharacterController>().height;
        player.transform.DOMove(midpos + Quaternion.Euler(midrot) * Vector3.forward *2, 0.5f);
        player.transform.DORotate(midrot + 180f * Vector3.up, 0.5f).OnComplete(()=> {
            player.transform.position = midpos;
            player.transform.rotation = Quaternion.Euler(midrot);
            cutscene1.Play(finisher); 
        });


    }
    public void PlayEnd()
    {
        cutscenecam.gameObject.SetActive(true);
        cutscenecam.enabled = true;
        StartCoroutine(PlayEndCorutine());
    }
    IEnumerator PlayEndCorutine()
    {
        yield return new WaitForSeconds(2f);
        _isPlaying = true;
        cutscene1.Play(end);
    }
    public void EndLevel()
    {
        GameManager.Instance.ExitToMenu();
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
