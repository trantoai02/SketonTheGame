using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public static Checkpoint instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }
    private void Start()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //player's XP
            PlayerXPLevelUpManager.instance.SaveXP();

            //player's location
            GameManager.instance.lastCheckpointPosition = transform.position;
        }
    }
}
