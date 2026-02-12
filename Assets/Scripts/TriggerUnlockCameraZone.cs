
using UnityEngine;
using UnityEngine.Events;

public class TriggerUnlockCameraZone : MonoBehaviour
{
    public UnityEvent afterCollisionTrigger;
    public bool destroyAfterTrigger = true;
    public void OnTriggerEnter2D(Collider2D collision)
    {
       

        if (!collision.CompareTag("Player"))
            return;

        afterCollisionTrigger?.Invoke();

        if (destroyAfterTrigger)
            Destroy(gameObject);
    }
}
