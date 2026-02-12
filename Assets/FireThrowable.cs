using UnityEngine;

public class FireThrowable : MonoBehaviour
{
    public float burnRadius = 1.5f;
    public LayerMask burnableLayer;

    bool hasHit = false;

    [SerializeField] float lifeTime = 5f;

    private void Awake()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        // ❌ Gặp vật không phá được → dừng
        if (collision.GetComponent<Indestructible>() != null)
        {
            hasHit = true;
            StopMoving();
            return;
        }

        // 🔥 Gặp vật cháy được
        if (((1 << collision.gameObject.layer) & burnableLayer) != 0)
        {
            Burnable burnable = collision.GetComponent<Burnable>();
            if (burnable != null)
            {
                burnable.Ignite();
                hasHit = true;
                StopMoving();
            }
        }
    }

    void StopMoving()
    {
        FakeHeightObject f = GetComponent<FakeHeightObject>();
        if (f != null)
            f.Stick();
    }
}
