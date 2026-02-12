
using UnityEngine;
using UnityEngine.UI;

public class StarUI : MonoBehaviour
{


    public Image[] stars;
    public Sprite goldenStar;
    public Sprite emptyStar;

    void Start()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            //if (PlayerPrefs.GetInt("stars" + LevelSelection.instance.levelIndex.ToString() + i.ToString()) == 1)
            if (PlayerPrefs.GetInt("stars" + PlayerPrefs.GetInt("playingLevelIndex") + i.ToString()) == 1)
            {
                stars[i].sprite = goldenStar;
            }
            else
            {
                stars[i].sprite = emptyStar;
            }
        }
    }

    void Update()

    {
        UpdateStarUI();

    }

    public void UpdateStarUI()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            if ((LevelStarManager.instance.starIndexList[i] == 1))
            {
                stars[i].sprite = goldenStar;
            }
            else
            {
                stars[i].sprite = emptyStar;
            }
        }
    }
}
