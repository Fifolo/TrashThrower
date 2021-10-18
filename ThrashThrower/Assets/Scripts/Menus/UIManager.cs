using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject inGameUI;

    private Transform UImanagerTransform;
    protected override void Awake()
    {
        base.Awake();
        UImanagerTransform = transform;
        InitializeMenus();
        GameManager.Instance.OnGameStateChange += Instance_OnGameStateChange;
    }

    protected override void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnGameStateChange -= Instance_OnGameStateChange;
        base.OnDestroy();
    }

    #region Methods
    private void InitializeMenus()
    {
        if (mainMenu != null)
        {
            mainMenu = Instantiate(mainMenu, UImanagerTransform);
            mainMenu.SetActive(false);
        }
        else Debug.LogWarning("Main menu prefab not set!");

        if (pauseMenu != null)
        {
            pauseMenu = Instantiate(pauseMenu, UImanagerTransform);
            pauseMenu.SetActive(false);
        }
        else Debug.LogWarning("Pause menu prefab not set!");

        if (optionsMenu != null)
        {
            optionsMenu = Instantiate(optionsMenu, UImanagerTransform);
            optionsMenu.SetActive(false);
        }
        else Debug.LogWarning("Options menu prefab not set!");

        if (inGameUI != null)
        {
            inGameUI = Instantiate(inGameUI, UImanagerTransform);
            inGameUI.SetActive(false);
        }
        else Debug.LogWarning("Options menu prefab not set!");

        if (gameOverMenu != null)
        {
            gameOverMenu = Instantiate(gameOverMenu, UImanagerTransform);
            gameOverMenu.SetActive(false);
        }
        else Debug.LogWarning("Game over menu prefab not set!");
    }
    public void LoadOptions()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }
    public void BackFromOptions()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }
    private void Instance_OnGameStateChange(GameManager.GameState from, GameManager.GameState to)
    {
        if (from == GameManager.GameState.Boot) mainMenu.SetActive(true);

        else if (from == GameManager.GameState.MainMenu)
        {
            mainMenu.SetActive(false);
            inGameUI.SetActive(true);
        }

        else if (from == GameManager.GameState.Playing)
        {
            inGameUI.SetActive(false);

            if (to == GameManager.GameState.Pause)
                pauseMenu.SetActive(true);

            else if (to == GameManager.GameState.GameOver)
                gameOverMenu.SetActive(true);
        }

        else if (from == GameManager.GameState.Pause)
        {
            pauseMenu.SetActive(false);

            if (to == GameManager.GameState.Playing)
                inGameUI.SetActive(true);

            else if (to == GameManager.GameState.MainMenu)
                mainMenu.SetActive(true);
        }

        else if (from == GameManager.GameState.GameOver)
        {
            gameOverMenu.SetActive(false);
            mainMenu.SetActive(true);
        }


    }
    #endregion
}
