using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject continueButton;
    private void Start()
    {
        CheckContinueCondition();
        Debug.Log("cotinue con checked!");


    }

    //private void Awake()
    //{
    //    CheckContinueCondition();
    //    Debug.Log("cotinue con checked!");
    //}
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game!");

        Application.Quit();
    }
    public void GoToPlayingLevel()
    {
        //"playingLevel" được set ở LevelSelection, các GO ExitArea, completeLevel,...
        string playingLevelName = PlayerPrefs.GetString("playingLevel", "");
        if (playingLevelName != "")
        {
            SceneManager.LoadScene(playingLevelName);
        }
       // LevelSelection.currentLevel = PlayerPrefs.GetInt("playingLevelIndex");
        for (int i = 0; i < 3; i++)
        {
            LevelStarManager.instance.starIndexList[i] = PlayerPrefs.GetInt("stars" + PlayerPrefs.GetInt("playingLevelIndex") + i.ToString(), 0);
        }
    }
 
    public void CheckContinueCondition()
    {
        Debug.Log("cotinue con checked!");
        string playingLevelName = PlayerPrefs.GetString("playingLevel", "");
        if (playingLevelName != "")
        {
            continueButton.SetActive(true);
            continueButton.gameObject.GetComponentInChildren<TMP_Text>().text = "TIẾP TỤC (LEVEL " + (PlayerPrefs.GetInt("playingLevelIndex") + 1) + ")";
        }
        else 
        { 
            continueButton.SetActive(false);
        }
    }
}
