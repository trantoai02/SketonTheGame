using UnityEngine.Events;
using UnityEngine;

public class TheBigDoor : MonoBehaviour
{
    public UnityEvent eventAfterPlayerGetIn;

    public GameObject theDoorGFX;
    private void Start()
    {
        if (MirrorFragmentManager.Instance.HasFullSet())
        {
            theDoorGFX.SetActive(true);
        }
        else
        {
            theDoorGFX.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && MirrorFragmentManager.Instance.HasFullSet())
        {
            eventAfterPlayerGetIn?.Invoke();
        }
    }
}
