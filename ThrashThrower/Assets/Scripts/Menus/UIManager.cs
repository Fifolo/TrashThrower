using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
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
        if(GameManager.Instance != null) GameManager.Instance.OnGameStateChange -= Instance_OnGameStateChange;
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
        /*
        if (optionsMenu != null)
        {
            optionsMenu = Instantiate(optionsMenu, UImanagerTransform);
            optionsMenu.SetActive(false);
        }
        else Debug.LogWarning("Options menu prefab not set!");
        */
        if (inGameUI != null)
        {
            inGameUI = Instantiate(inGameUI, UImanagerTransform);
            inGameUI.SetActive(false);
        }
        else Debug.LogWarning("Options menu prefab not set!");
    }

    private void Instance_OnGameStateChange(GameManager.GameState from, GameManager.GameState to)
    {
        //going from boot
        if (from == GameManager.GameState.Boot) mainMenu.SetActive(true);

        ////going from pause
        else if (from == GameManager.GameState.Pause)
        {
            //to main menu
            if (to == GameManager.GameState.MainMenu)
            {
                pauseMenu.SetActive(false);
                mainMenu.SetActive(true);
                inGameUI.SetActive(false);
            }
            //to playing
            else if (to == GameManager.GameState.Playing) pauseMenu.SetActive(false);
        }
        // to pause
        else if (to == GameManager.GameState.Pause) pauseMenu.SetActive(true);

        //to playing
        else if (to == GameManager.GameState.Playing)
        {
            pauseMenu.SetActive(false);
            mainMenu.SetActive(false);
            inGameUI.SetActive(true);
        }
    }
    #endregion
}
