
using TMPro;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager instance;
    TMP_Text coinText;
    public int currentCoin = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if (coinText == null)
            {
                coinText = GameObject.Find("Coin Text").GetComponent<TMP_Text>();
            }
        }
            

        LoadCoins();
    }

    private void Update()
    {

    }

    private void Start()
    {
        coinText.text = currentCoin.ToString("D3");
    }
    public bool CanAfford(int amount)
    {
        return currentCoin >= amount;
       
    }
    public void SpendMoney(int amount)
    {
        if (CanAfford(amount))
        {
            currentCoin -= amount;
            if (currentCoin < 0)
            {
                currentCoin = 0;

            }
            UpdateCoinText();
            SaveCoins();
        }
    }

    public void AddCoins(int amount)
    {
        if(currentCoin >= 99999)
        {
            currentCoin = 99999;
        }
        else
        {
            currentCoin += amount;
            UpdateCoinText();
            SaveCoins();
        }
      
    }

 
    public void UpdateCurrentCoin()
    {
        AddCoins(1);

        coinText.text = currentCoin.ToString("D3");
        SaveCoins();
    }

    public void UpdateCoinText()
    {
        coinText.text = currentCoin.ToString("D3");
    }

    public void SaveCoins()
    {
        PlayerPrefs.SetInt("playerCoin", currentCoin);
        PlayerPrefs.Save();
    }

    public void LoadCoins()
    {
        currentCoin = PlayerPrefs.GetInt("playerCoin", 0);

        if (currentCoin < 0) currentCoin = 0;
        if (currentCoin > 99999) currentCoin = 99999;
    }
}
