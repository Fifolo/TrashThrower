using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Singleton<MainMenu>
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;

    private void Start()
    {
        AddButtonActions();
    }
    private void AddButtonActions()
    {
        playButton.onClick.AddListener(GameManager.Instance.StartGame);
        playButton.onClick.AddListener(SoundManager.Instance.ButtonPressed);

        optionsButton.onClick.AddListener(UIManager.Instance.LoadOptions);
        optionsButton.onClick.AddListener(SoundManager.Instance.ButtonPressed);

        exitButton.onClick.AddListener(GameManager.Instance.ExitGame);
        exitButton.onClick.AddListener(SoundManager.Instance.ButtonPressed);
    }
}
