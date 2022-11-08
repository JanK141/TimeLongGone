using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Content.Scripts
{
    public class GameManager : MonoBehaviour
    {

        public static GameManager Instance { get; private set; }

        private PlayerInput _playerInput;

        private List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

        Scene mainMenuScene;
        Scene pauseScene;
        Scene HUDScene;
        Scene lvlScene1;

        // Start is called before the first frame update
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            if (Instance != null && Instance != this)
                Destroy(this);
            else
                Instance = this;

            mainMenuScene = SceneManager.GetSceneByBuildIndex(1);
            pauseScene = SceneManager.GetSceneByBuildIndex(2);
            HUDScene = SceneManager.GetSceneByBuildIndex(4);
            lvlScene1 = SceneManager.GetSceneByBuildIndex(3);


            //MainInputActions mainInputActions = new MainInputActions();
            //mainInputActions.Player.Disable();
            //mainInputActions.Menu.Enable();

        
        }

        private void Start()
        {
            //playerInput = GetComponent<PlayerInput>();
            //playerInput.SwitchCurrentActionMap("Menu");
            //SceneManager.LoadScene(1, LoadSceneMode.Additive);
        }

        public void GoToLevel1()
        {
            SceneManager.LoadScene(lvlScene1.buildIndex, LoadSceneMode.Additive);
            SceneManager.LoadScene(HUDScene.buildIndex, LoadSceneMode.Additive);
            SceneManager.LoadScene(pauseScene.buildIndex, LoadSceneMode.Additive);
        }

        public void ExitToMenu() => SceneManager.LoadScene("Main Menu Scene");

        public void LoadLevel(string levelName)
        {
            scenesToLoad.Clear();
            StartCoroutine(LoadLoading(levelName));
            //SceneManager.LoadScene("LoadingScreen", LoadSceneMode.Single);
            /*scenesToLoad.Add(SceneManager.LoadSceneAsync(levelName));
        scenesToLoad.Add(SceneManager.LoadSceneAsync("Pause Menu Scene", LoadSceneMode.Additive));
        scenesToLoad.Add(SceneManager.LoadSceneAsync("HUD Scene", LoadSceneMode.Additive));
        scenesToLoad.ForEach(s=>s.allowSceneActivation=false);
        StartCoroutine(Loading());
        StartCoroutine(DisplayLoading(levelName));*/
        }

        IEnumerator LoadLoading(string levelName)
        {
            var tmp = SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Single);
            while (!tmp.isDone) yield return null;
            //yield return new WaitForSeconds(1f);
            scenesToLoad.Add(SceneManager.LoadSceneAsync(levelName));
            scenesToLoad.Add(SceneManager.LoadSceneAsync("Pause Menu Scene", LoadSceneMode.Additive));
            scenesToLoad.Add(SceneManager.LoadSceneAsync("HUD Scene", LoadSceneMode.Additive));
            scenesToLoad.ForEach(s => s.allowSceneActivation = false);
            StartCoroutine(Loading());
            StartCoroutine(DisplayLoading(levelName));
        }

        public void FinishLoading()
        {
            StopCoroutine(nameof(DisplayLoading));
            scenesToLoad.ForEach(s=>s.allowSceneActivation=true);
        }

        IEnumerator Loading()
        {
            yield return new WaitForSeconds(1f);
            var pressToContinue = GameObject.Find("PressToContinue").GetComponent<PressToContinue>();
            var progressBar = GameObject.Find("LoadingBar").GetComponent<Slider>();
            float totalProgress = 0;
        
            /*while (scenesToLoad[0].progress <= 0.85)
            {
                totalProgress = scenesToLoad[0].progress;
                progressBar.value = totalProgress;
                yield return null;
            }*/

            while (totalProgress<1)
            {
                var randWait = Random.value;
                var randProg = Random.Range(0.01f, 0.1f);
                yield return new WaitForSeconds(randWait);
                totalProgress += randProg;
                progressBar.value = totalProgress;
            }
        

            progressBar.value = 1;
            pressToContinue.IsLoaded = true;
        }

        IEnumerator DisplayLoading(string levelName)
        {
            YieldInstruction displayWait = new WaitForSeconds(0.25f);
            //yield return new WaitForSeconds(1f);
            var levelDisplay = GameObject.Find("LevelDisplay").GetComponent<TextMeshProUGUI>();
            while (true)
            {
                levelDisplay.text = "Loading " + levelName;
                yield return displayWait;
                levelDisplay.text = "Loading " + levelName+".";
                yield return displayWait;
                levelDisplay.text = "Loading " + levelName + "..";
                yield return displayWait;
                levelDisplay.text = "Loading " + levelName + "...";
                yield return displayWait;
            }
        }
    }
}
