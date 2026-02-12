using UnityEngine.Events;
using UnityEngine;

public class SubDoorTeleport : MonoBehaviour
{
    public UnityEvent eventAfterPlayerGetIn;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            eventAfterPlayerGetIn?.Invoke();
        }
    }
}
