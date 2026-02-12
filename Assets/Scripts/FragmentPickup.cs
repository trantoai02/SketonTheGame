using UnityEngine;

public class FragmentPickup : MonoBehaviour
{
    [Range(0, 2)]
    public int fragmentID;

    private void Start()
    {
        // Nếu đã nhặt rồi thì biến mất ngay
        //if (MirrorFragmentManager.Instance.IsCollected(fragmentID))
        //{
        //    gameObject.SetActive(false);
        //}
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        MirrorFragmentManager.Instance.CollectFragment(fragmentID);
        gameObject.SetActive(false);
    }
}
