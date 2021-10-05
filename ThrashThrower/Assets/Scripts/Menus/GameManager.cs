using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject[] managerPrefabs;

    #region Instance Variables
    private List<GameObject> instantiatedManagers;
    public delegate void StateGameChange(GameState from, GameState to);
    public event StateGameChange OnGameStateChange;
    private string currentSceneName = "Boot";
    private string previousSceneName;
    public GameState currentState { get; private set; }
    private Transform gameManagerTransform;
    public enum GameState
    {
        Boot,
        MainMenu,
        Pause,
        Playing
    }
    #endregion

    #region MonoBehaviour
    protected override void Awake()
    {
        Application.targetFrameRate = 144;
        currentState = GameState.Boot;
        DontDestroyOnLoad(gameObject);
        base.Awake();
        gameManagerTransform = transform;
        InitializeManagers();
        LoadScene("MainMenu");
    }
    #endregion

    #region Methods
    private void InitializeManagers()
    {
        instantiatedManagers = new List<GameObject>();

        foreach (GameObject manager in managerPrefabs)
        {
            Instantiate(manager, gameManagerTransform);
            manager.SetActive(true);
            instantiatedManagers.Add(manager);
        }
    }
    private void ChangeGameState(GameState nextState)
    {
        GameState previousState = currentState;

        switch (nextState)
        {
            case GameState.MainMenu:
                //Time.timeScale = 0f;
                break;
            case GameState.Pause:
                Time.timeScale = 0f;
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
        }

        currentState = nextState;
        OnGameStateChange?.Invoke(previousState, currentState);
    }

    public void StartGame() => LoadScene("MyCity");
    public void ExitGame() => Application.Quit();
    public void ResumeGame() => ChangeGameState(GameState.Playing);
    public void PauseGame() => ChangeGameState(GameState.Pause);
    public void GoToMainMenu() => LoadScene("MainMenu");

    #region Scene Management
    private void LoadScene(string newSceneName)
    {
        if (SceneManager.GetSceneByName(newSceneName) != null)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
            ao.completed += OnSceneLoadComplete;
            previousSceneName = currentSceneName;
            currentSceneName = newSceneName;
        }
    }
    private void OnSceneLoadComplete(AsyncOperation obj)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentSceneName));

        switch (currentSceneName)
        {
            case "MyCity":
                ChangeGameState(GameState.Playing);
                break;
            case "MainMenu":
                ChangeGameState(GameState.MainMenu);
                break;
        }
        UnloadScene(previousSceneName);
    }
    private void UnloadScene(string sceneName)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(previousSceneName);
        ao.completed += OnSceneUnloadComplete;
    }
    private void OnSceneUnloadComplete(AsyncOperation obj)
    {
        //Debug.Log($"{previousSceneName} succesfully unloaded");
    }
    #endregion

    #endregion
}
