using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject[] managerPrefabs;
    [SerializeField] private GameDifficulty gameDifficulty = GameDifficulty.Easy;
    public GameDifficulty Game_Difficulty { get { return gameDifficulty; } }

    #region Instance Variables
    private List<GameObject> instantiatedManagers;
    public delegate void StateGameChange(GameState from, GameState to);
    public event StateGameChange OnGameStateChange;

    private int currentSceneIndex = 0;
    private int previousSceneIndex;
    public GameState currentState { get; private set; }
    private Transform gameManagerTransform;
    public enum GameDifficulty
    {
        Easy,
        Medium,
        Hard
    }
    public enum GameState
    {
        Boot,
        MainMenu,
        Pause,
        GameOver,
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
        LoadScene(1);

        Trash.OnMaxContaminationReach += Trash_OnMaxContaminationReach;
        HealthController.OnPlayerDeath += HealthController_OnPlayerDeath;
    }

    private void HealthController_OnPlayerDeath(float damage) => ChangeGameState(GameState.GameOver);

    private void Trash_OnMaxContaminationReach(float contamination) => ChangeGameState(GameState.GameOver);

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
                Trash.CurrentContamination = 0;
                break;
            case GameState.Pause:
                Time.timeScale = 0f;
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
        }
        currentState = nextState;
        OnGameStateChange?.Invoke(previousState, currentState);
    }

    public void StartGame()
    {
        LoadScene(2);
    }
    public void ExitGame() => Application.Quit();
    public void ResumeGame() => ChangeGameState(GameState.Playing);
    public void PauseGame() => ChangeGameState(GameState.Pause);
    public void GoToMainMenu() => LoadScene(1);

    #region Scene Management
    private void LoadScene(int sceneIndex)
    {
        if (SceneManager.GetSceneByBuildIndex(sceneIndex) != null)
        {
            AsyncOperation ao = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            ao.completed += OnSceneLoadComplete;

            previousSceneIndex = currentSceneIndex;
            currentSceneIndex = sceneIndex;
        }
    }
    private void OnSceneLoadComplete(AsyncOperation obj)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(currentSceneIndex));

        switch (currentSceneIndex)
        {
            //boot loaded
            case 0:
                break;
            //main menu loaded
            case 1:
                ChangeGameState(GameState.MainMenu);
                break;
            //play scene loaded
            default:
                ChangeGameState(GameState.Playing);
                break;
        }

        UnloadScene(previousSceneIndex);
    }
    private void UnloadScene(int sceneIndex)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(sceneIndex);
        ao.completed += OnSceneUnloadComplete;
    }
    private void OnSceneUnloadComplete(AsyncOperation obj)
    {
        //Debug.Log($"{previousSceneName} succesfully unloaded");
    }
    #endregion

    #endregion
}
