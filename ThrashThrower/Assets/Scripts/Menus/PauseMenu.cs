using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : Singleton<PauseMenu>
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button exitButton;

    protected override void Awake()
    {
        base.Awake();
        AddButtonActions();
    }

    private void AddButtonActions()
    {
        resumeButton.onClick.AddListener(GameManager.Instance.ResumeGame);
        mainMenuButton.onClick.AddListener(GameManager.Instance.GoToMainMenu);
        exitButton.onClick.AddListener(GameManager.Instance.ExitGame);
    }
}
