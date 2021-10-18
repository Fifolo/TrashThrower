using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameOverMenu : Singleton<GameOverMenu>
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button exitButton;

    [SerializeField] private TextMeshProUGUI score;
    [SerializeField] private TextMeshProUGUI damageDealt;
    [SerializeField] private TextMeshProUGUI damageTaken;
    [SerializeField] private TextMeshProUGUI trashInCansAmount;
    [SerializeField] private TextMeshProUGUI badNpcAmount;
    [SerializeField] private TextMeshProUGUI goodNpcAmount;

    private void Start()
    {
        mainMenuButton.onClick.AddListener(GameManager.Instance.GoToMainMenu);
        exitButton.onClick.AddListener(GameManager.Instance.ExitGame);

        //GameManager.Instance.OnGameStateChange += Instance_OnGameStateChange;
    }
    private void OnEnable()
    {
        if (GameManager.Instance.currentState == GameManager.GameState.GameOver) SetNumbers();
    }
    public void SetNumbers()
    {
        score.text = InGameUI.Instance.Score.ToString();
        damageDealt.text = ((int)StatisticsCounter.Instance.TotalDamageDealt).ToString();
        damageTaken.text = ((int)StatisticsCounter.Instance.TotalDamageTaken).ToString();
        trashInCansAmount.text = ((int)StatisticsCounter.Instance.TrashPutInCans).ToString();
        badNpcAmount.text = ((int)StatisticsCounter.Instance.BadNpcsKilled).ToString();
        goodNpcAmount.text = ((int)StatisticsCounter.Instance.GoodNpcsKilled).ToString();
    }
}
