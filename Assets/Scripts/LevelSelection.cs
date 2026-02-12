//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class LevelSelection : MonoBehaviour
//{
//    public static LevelSelection instance;

//    public static int currentLevel;
//    public static string currentLevelName;
//    public int levelIndex;
//    public Sprite goldenStarSprite;

//    public LevelObject[] levelObjects;

//    // Lưu index cao nhất level đã unlock (persistent)
//    public static int unlockedLevels;
//    public int unlockedLevel;

//    // Tổng số sao (từ tất cả level)
//    public int totalStars = 0;

//    private void Awake()
//    {
//        if (instance == null)
//        {
//            instance = this;
//            // nếu muốn giữ object giữa scene có thể uncomment:
//            // DontDestroyOnLoad(gameObject);
//        }
//        else if (instance != this)
//        {
//            Destroy(gameObject);
//        }
//    }

//    public void LevelIndex(int levelNum)
//    {
//        levelIndex = levelNum;
//    }

//    public void LoadLevel(string sceneToLoad)
//    {
//        currentLevel = levelIndex;
//        // lưu tên scene đang chơi
//        PlayerPrefs.SetString("playingLevel", sceneToLoad);
//        PlayerPrefs.SetInt("playingLevelIndex", levelIndex);
//        SceneManager.LoadScene(sceneToLoad);

//        // giữ nguyên dòng này nếu bạn có LevelStarManager
//        if (LevelStarManager.instance != null)
//            LevelStarManager.instance.GetStarStatus();
//    }

//    private void Start()
//    {
//        // 1) Đọc unlockedLevels từ PlayerPrefs (nếu có)
//        unlockedLevels = PlayerPrefs.GetInt("unlockedLevels", 0);
//        unlockedLevel = unlockedLevels;

//        // 2) Tính totalStars chính xác bằng cách đọc các key "stars{i}{j}"
//        totalStars = 0;
//        for (int i = 0; i < levelObjects.Length; i++)
//        {
//            int starsForLevel = 0;

//            if (levelObjects[i].stars != null)
//            {
//                for (int j = 0; j < levelObjects[i].stars.Length; j++)
//                {
//                    // key per-star (theo cách bạn đã lưu trước đó)
//                    if (PlayerPrefs.GetInt("stars" + i.ToString() + j.ToString(), 0) == 1)
//                    {
//                        starsForLevel++;
//                        // set icon thành vàng nếu có
//                        levelObjects[i].stars[j].sprite = goldenStarSprite;
//                    }
//                    else
//                    {
//                        // nếu muốn, có thể set sprite mặc định ở đây
//                    }
//                }
//            }

//            // cộng vào tổng
//            totalStars += starsForLevel;

//            // (Tùy chọn) lưu tổng sao của level vào PlayerPrefs để tương thích cũ
//            PlayerPrefs.SetInt("stars" + i.ToString(), starsForLevel);
//        }

//        PlayerPrefs.Save(); // lưu aggregate nếu có

//        // 3) Dùng totalStars để check unlock cho từng level
//        for (int i = 0; i < levelObjects.Length; i++)
//        {
//            bool meetsStarRequirement = totalStars >= levelObjects[i].startRequired;
//            bool previouslyUnlocked = i <= unlockedLevels; // nếu trước đó đã unlock (legacy)

//            if (meetsStarRequirement || previouslyUnlocked)
//            {
//                levelObjects[i].levelBtn.interactable = true;

//                // cập nhật unlockedLevel để lưu lại nếu cần
//                if (i > unlockedLevel)
//                    unlockedLevel = i;
//            }
//            else
//            {
//                levelObjects[i].levelBtn.interactable = false;
//            }

//            // Nếu bạn muốn hiển thị số sao on label (nếu từng dùng):
//            // int starsCount = PlayerPrefs.GetInt("stars" + i.ToString(), 0);
//            // (hiển thị starsCount nếu cần)
//        }

//        // 4) Nếu có level mới được unlock bằng sao → lưu lại unlockedLevels
//        if (unlockedLevel > unlockedLevels)
//        {
//            unlockedLevels = unlockedLevel;
//            PlayerPrefs.SetInt("unlockedLevels", unlockedLevels);
//            PlayerPrefs.Save();
//        }
//    }
//}

using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelection : MonoBehaviour
{
    public static LevelSelection instance;
    public static int currentLevel;
    public static string currentLevelName;

    public int levelIndex;
    public Sprite goldenStarSprite;
    public LevelObject[] levelObjects;

    public static int unlockedLevels;
    public int unlockedLevel;
    public int totalStars = 0;

    public int coinReward;
    public LevelInfoPopup levelInfoPopup;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        // Tính tổng số sao
        for (int i = 0; i < levelObjects.Length; i++)
        {
            for (int j = 0; j < levelObjects[i].stars.Length; j++)
            {
                if (PlayerPrefs.GetInt("stars" + i.ToString() + j.ToString(), 0) == 1)
                {
                    totalStars++;
                }
            }
        }
    }

    public void OnClickLevel( string sceneName)
    {
        levelInfoPopup.Show(levelIndex, sceneName, coinReward);
    }

    public void SetLevelReward(int reward)
    {
        coinReward = reward;
    }

    public void LevelIndex(int levelNum)
    {
        levelIndex = levelNum;
    }

    public void SetLevelInfo(string sceneToLoad) 
    {
        currentLevel = levelIndex;

        // Lưu lại level đang chơi
        PlayerPrefs.SetString("playingLevel", sceneToLoad);
        PlayerPrefs.SetInt("playingLevelIndex", levelIndex);
    }
        
    public void LoadLevel(string sceneToLoad)
    {
        currentLevel = levelIndex;

        // Lưu lại level đang chơi
        PlayerPrefs.SetString("playingLevel", sceneToLoad);
        PlayerPrefs.SetInt("playingLevelIndex", levelIndex);

        SceneManager.LoadScene(sceneToLoad);

        LevelStarManager.instance.GetStarStatus();
    }

    public void ChangeCurrentLevelIndex(int levelIndex)
    {
        currentLevel = levelIndex;
    }

    private void Start()
    {
        unlockedLevels = PlayerPrefs.GetInt("unlockedLevels", 0);
        unlockedLevel = unlockedLevels;

        for (int i = 0; i < levelObjects.Length; i++)
        {
            // Mở khóa level nếu đủ số sao yêu cầu
            if (levelObjects[i].startRequired <= totalStars && i <= unlockedLevels)
            {
                levelObjects[i].levelBtn.interactable = true;

                // Hiển thị sao đã đạt được
                for (int j = 0; j < levelObjects[i].stars.Length; j++)
                {
                    if (PlayerPrefs.GetInt("stars" + i.ToString() + j.ToString(), 0) == 1)
                    {
                        levelObjects[i].stars[j].sprite = goldenStarSprite;
                    }
                }
            }
        }
    }
}

