using UnityEngine;

public class EggController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip noteClip;

    public EggPuzzleController puzzle;

    [Header("Spotlight của quả trứng (GameObject con)")]
    public GameObject spotlight;

    [Header("Hiệu ứng nổ (Prefab)")]
    public GameObject explosionEffect;

    private void Awake()
    {
        if (spotlight != null)
            spotlight.SetActive(false);
    }

    public void Sing()
    {
        if (spotlight != null)
            StartCoroutine(BlinkSpotlight());

        if (audioSource != null && noteClip != null)
            audioSource.PlayOneShot(noteClip);
    }

    private System.Collections.IEnumerator BlinkSpotlight()
    {
        spotlight.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        spotlight.SetActive(false);
    }

    public void Explode()
    {
        // Spawn hiệu ứng nổ
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Tắt spotlight
        if (spotlight != null)
            spotlight.SetActive(false);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<DamageSource>() != null)
        {
            // Khi bị đánh → Sing() luôn
            Sing();

            if (puzzle != null)
                puzzle.OnEggHit(this);
        }
    }
}
