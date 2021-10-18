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
    }
    private void Start()
    {
        AddButtonActions();
    }
    private void AddButtonActions()
    {
        resumeButton.onClick.AddListener(GameManager.Instance.ResumeGame);
        resumeButton.onClick.AddListener(SoundManager.Instance.ButtonPressed);

        mainMenuButton.onClick.AddListener(GameManager.Instance.GoToMainMenu);
        mainMenuButton.onClick.AddListener(SoundManager.Instance.ButtonPressed);

        exitButton.onClick.AddListener(GameManager.Instance.ExitGame);
        exitButton.onClick.AddListener(SoundManager.Instance.ButtonPressed);
    }
}
