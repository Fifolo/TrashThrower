using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : Singleton<OptionsMenu>
{
    [SerializeField] private Button backButton;
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
        backButton.onClick.AddListener(UIManager.Instance.BackFromOptions);
        backButton.onClick.AddListener(SoundManager.Instance.ButtonPressed);
    }

}
