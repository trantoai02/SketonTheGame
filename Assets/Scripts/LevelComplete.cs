
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelComplete : MonoBehaviour
{
    public static LevelComplete instance;
    private bool isInitialized = false;

    public int starsAquired;
    public int[] starsTracking;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
           
           // DontDestroyOnLoad(gameObject);
            InitializeData();
        }
        else
        {
            Destroy(gameObject);
            
        }

    }
    
    public void OnLevelComplete()
    {
        //unlock the next level
        if (LevelSelection.currentLevel == LevelSelection.unlockedLevels)
        {
            LevelSelection.unlockedLevels++;
            PlayerPrefs.SetInt("unlockedLevels", LevelSelection.unlockedLevels);
            PlayerPrefs.Save();
        }

        // lưu stars từ danh sách tạm LevelStarManager.starIndexList vào PlayerPrefs

        for(int i = 0; i < 3; i++)
        {
            PlayerPrefs.SetInt("stars" + LevelSelection.currentLevel.ToString() + i.ToString(), LevelStarManager.instance.starIndexList[i]);
        }

        // làm mới danh sách tạm
        //for (int i = 0; i < 3; i++)
        //{
        //    LevelStarManager.instance.starIndexList[i] = 0;
        //}



        //sum up stars
        for (int i = 0;i<starsTracking.Length;i++) {
            if (starsTracking[i] > PlayerPrefs.GetInt("stars" + LevelSelection.currentLevel.ToString() + i.ToString(),0) ) {
                PlayerPrefs.SetInt("stars" + LevelSelection.currentLevel.ToString() + i.ToString(), starsTracking[i]);
                PlayerPrefs.Save();
            }
        }

        PlayerPrefs.Save();

        // lưu quest (chỉ khi hoàn thành level)
        if(QuestManager.Instance != null)
        {
            QuestManager.Instance.SaveAllQuests();

        }

        //load another levels
        SceneManager.LoadScene("LevelSelection");
    }
    // Start is called before the first frame update
    void InitializeData()
    {
        if (!isInitialized)
        {
            starsAquired = 0;
            starsTracking = new int[3];
            isInitialized = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
