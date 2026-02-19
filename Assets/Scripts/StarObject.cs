
using UnityEngine;

public class StarObject : MonoBehaviour
{
    public int starIDValue = 0;
    int starID;
    public enum StarState
    {
        New,
        Collected,
    }
    public StarState state;
    public Sprite newStar;
    public Sprite collectedStar;

    //public LevelComplete LevelComplete;

    public static StarObject instance;
    private void Awake()
    {
        
        instance = this;
        starID = starIDValue;
    }
    private void Start()
    {
        //if(LevelStarManager.instance.starIndexList[starID] == 1)
        if (PlayerPrefs.GetInt("stars" + PlayerPrefs.GetInt("playingLevelIndex") + starID) == 1)
        {
            state = StarState.Collected;
        }
        else
        {
            state = StarState.New;
        }

        switch(state)
        {
            case StarState.New:
                GetComponent<SpriteRenderer>().sprite = newStar;
                break; 
            case StarState.Collected:
                GetComponent<SpriteRenderer>().sprite = collectedStar;
                break;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag=="Player")
        {
            LevelComplete.instance.starsTracking[starID] = 1;
            LevelStarManager.instance.starIndexList[starID] = 1;
            

            GameManager.instance.starUI.UpdateStarUI();

            Destroy(gameObject);
        }
    }

}
