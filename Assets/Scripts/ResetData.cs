using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetData : MonoBehaviour
{
    public void ResetStars()
    {
        for (int i = 0; i<3; i++)
        {
            PlayerPrefs.SetInt("stars" + LevelSelection.currentLevel.ToString() + i, 0);
            PlayerPrefs.Save();
        }
       
    }
    public void ResetLevelsUnlock()
    {
        PlayerPrefs.SetInt("unlockedLevels", 0);
        PlayerPrefs.Save();
    }
}
