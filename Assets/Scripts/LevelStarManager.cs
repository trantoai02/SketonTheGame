
using UnityEngine;


public class LevelStarManager : MonoBehaviour
{
    public static LevelStarManager instance;

    public int[] starIndexList;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        starIndexList = new int[3];
    }

    //public void Update()
    //{
    //    Debug.Log(LevelSelection.currentLevel.ToString());
    //}
    public void GetStarStatus()
    {
        
        for (int i = 0; i < 3; i++)
        {
            starIndexList[i] = PlayerPrefs.GetInt("stars" + LevelSelection.currentLevel.ToString() + i.ToString(), 0);
           // starIndexList[i] = PlayerPrefs.GetInt("stars" + PlayerPrefs.GetInt("playingLevelIndex") + i.ToString(), 0);
        }
    }

    public void ClearStarList()
    {
        for (int i = 0; i < 3; i++)
        {
            starIndexList[i] = 0;
        }
    }
}

