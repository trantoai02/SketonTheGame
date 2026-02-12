using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelInfoPopup : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI levelNameText;
    public Image[] stars;               // mảng hình sao
    public Sprite goldenStarSprite;
    public Sprite grayStarSprite; //levelObjects[i].stars[j].sprite

    public TextMeshProUGUI coinText;
    public Image coinImage;

    public TextMeshProUGUI isCompletedText;

    [Header("Buttons")]
    public Button playButton;
   // public Button closeButton;

    private int levelIndex;
    private string sceneName;

    public void Show(int index, string scene, int reward)
    {
        levelIndex = index;
        sceneName = scene;

        // ===== Level name =====
        levelNameText.text = "Level " + (index + 1);

        // ===== Coin reward =====
        coinText.text = reward.ToString();

        // ===== Stars =====
        int earnedStars = 0;

        for (int i = 0; i < stars.Length; i++)
        {
            int starKey = PlayerPrefs.GetInt("stars" + index.ToString()+ i.ToString(), 0);
            if (starKey == 1)
            {
                stars[i].sprite = goldenStarSprite;
                earnedStars++;
            }
            else
            {
                stars[i].sprite = grayStarSprite;
            }
        }

        // ===== Completed text ===== sửa logic lại
        if (LevelSelection.instance.levelIndex < LevelSelection.instance.unlockedLevel)
        {
            isCompletedText.text = "Đã hoàn thành!";
            isCompletedText.color = Color.green;
        }
        else if (LevelSelection.instance.levelIndex == LevelSelection.instance.unlockedLevel)
        {
            isCompletedText.text = "Chưa hoàn thành!";
            isCompletedText.color = Color.red;
        }

        // ===== Enable / Disable Play =====
        playButton.interactable = true;
       // closeButton.interactable = true;

        gameObject.SetActive(true);
    }

    public void Play()
    {
        //LevelSelection.instance.LevelIndex(levelIndex);
        LevelSelection.instance.LoadLevel(sceneName);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
