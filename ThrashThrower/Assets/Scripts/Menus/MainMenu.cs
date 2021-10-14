using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Singleton<MainMenu>
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;

    protected override void Awake()
    {
        base.Awake();
        AddButtonActions();
    }

    private void AddButtonActions()
    {
        playButton.onClick.AddListener(GameManager.Instance.StartGame);
        optionsButton.onClick.AddListener(UIManager.Instance.LoadOptions);
        exitButton.onClick.AddListener(GameManager.Instance.ExitGame);
    }
}
