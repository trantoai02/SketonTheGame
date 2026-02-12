using UnityEngine;

public class WolfAction : MonoBehaviour
{
    public WolfSatanManager manager;

    public void TakeHit()
    {
        manager.OnWolfHit();
        QuestManager.Instance.OnEnemyKilled(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "GlowSword")
        {
            TakeHit();

        }
    }
}
