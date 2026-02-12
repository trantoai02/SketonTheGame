using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ResultPopup : MonoBehaviour
{
    public static ResultPopup instance;

    [Header("UI")]
    public TextMeshProUGUI titleText;
    public Image[] stars;
    public Sprite goldenStarSprite;
    public Sprite grayStarSprite;

    public TextMeshProUGUI coinText;
    public Image coinImage;

    [Header("Button")]
    public Button continueButton;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        gameObject.SetActive(false);
    }

    public void Show()
    {
        titleText.text = "Hoàn thành!";

        // int levelIndex = LevelSelection.currentLevel;

        // ===== Stars =====

        for (int i = 0; i < stars.Length; i++)
        {
            int savedStar = PlayerPrefs.GetInt(
                "stars" + LevelSelection.instance.levelIndex.ToString() + i.ToString(), 0);

            int currentStar = LevelStarManager.instance.starIndexList[i];

            int finalStar = Mathf.Max(savedStar, currentStar);

            stars[i].sprite = finalStar == 1
                ? goldenStarSprite
                : grayStarSprite;
        }

        // ===== Coin Reward =====
    
        int rewardCoin = LevelSelection.instance.coinReward; // gợi ý
        coinText.text = rewardCoin.ToString();

        EconomyManager.instance.AddCoins(rewardCoin);

        gameObject.SetActive(true);
       // Time.timeScale = 0f; // pause game khi hiện popup
    }

    public void Continue()
    {
        Time.timeScale = 1f;

        LevelComplete.instance.OnLevelComplete();
        gameObject.SetActive(false);
    }
}
