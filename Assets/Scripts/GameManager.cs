using Inventory;
using Inventory.Model;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Singleton
    public static GameManager instance;

    //Vị trí checkpoint cuối cùng được ghi nhận
    public Vector2 lastCheckpointPosition;

    //Khai báo Input System
    CustomInput input;

    //Dành cho Menu Game Settings
    public GameObject gameSettingsMenu;

    public StarUI starUI;

    //Tham chiếu đến GameObject có gắn script Tab Manager trên Inspector để quản lý việc chuyển tab
    TabManager tabManager;

    public InventoryController InventoryController;
    public InventorySO inventorySO;
    public float timeRemain;

    public string playerName;
    private void Awake()
    {
        //timeRemain = 5;

        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(instance);
        }
        //else
        //{
        //    Destroy(instance);
        //}

        if (gameSettingsMenu != null)
        {
            tabManager = gameSettingsMenu.GetComponent<TabManager>();
        }
        //input - open game settings menu
        input = new CustomInput();
        input.Inventory.OpenGameMenu.performed += OpenGameMenu_performed;

        if (Player.instance != null)
        {
            lastCheckpointPosition = Player.instance.transform.position;

        }

        playerName = PlayerPrefs.GetString("playerName", "Sketon");

    }
    //public void Update()
    //{
      
    //    if (LevelSelection.instance.levelIndex == 1)
    //    {
    //        timeRemain -= Time.deltaTime;
    //        Debug.Log(timeRemain);
    //        Debug.Log(LevelSelection.instance.levelIndex);
    //        if (timeRemain <= 0)
    //        {
    //            SceneManager.LoadScene("MainMenu");
    //        }

    //    }
    //}
    private void OpenGameMenu_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!gameSettingsMenu.activeSelf)
        {
            gameSettingsMenu.SetActive(true);
            tabManager.SwitchToTab(0);
        }
        else
        {
            gameSettingsMenu.SetActive(false);
        }

    }

    //Dành cho Input System
    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();

    }
    public ResultPopup resultPopup;
    public void CompleteLevel()
    {
        //Lưu EXP của Player khi hoàn thành Level
        PlayerXPLevelUpManager.instance.SaveXP();
        int rewardCoin = LevelSelection.instance.coinReward; // gợi ý
      

        EconomyManager.instance.AddCoins(rewardCoin);

        LevelComplete.instance.OnLevelComplete();

        //resultPopup.Show();
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game!");

        Application.Quit();
    }

    //Điều chỉnh Time Scale để dừng - chạy thời gian khi mở Menu Game
    public void PauseTheGame()
    {
        Time.timeScale = 0;
    }

    public void ContinueTheGame()
    {
        Time.timeScale = 1;

    }

    public void ResetAllPlayerPrefsData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }
   
    public void ResetAllPlayerPrefsDataAndGoToMainMenu()
    {
        if(InventoryController != null && inventorySO != null)
        {
            inventorySO.Initialize();
            InventoryController.Save();
        }
       
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;

    }



}

