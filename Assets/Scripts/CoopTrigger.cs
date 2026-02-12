using UnityEngine;

public class CoopTrigger : MonoBehaviour
{
    public ChickenCatchQuest questRef;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            questRef.SetPlayerNearCoop(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            questRef.SetPlayerNearCoop(false);
        }
    }
}
