using UnityEngine;
using System.Collections;

public class Burnable : MonoBehaviour
{
    [Header("Burn Settings")]
    public bool canBurn = true;
    public float burnDuration = 3f;
    public GameObject fireVFX;

    GameObject currentFireVFX;

    [Header("For non-enemy objects")]
 

    [Header("Fire Spread")]
    public float spreadRadius = 1.5f;
    public LayerMask burnableLayer;

  
    bool isBurning = false;



    public void Ignite()
    {
        if (!canBurn || isBurning) return;

        isBurning = true;
        Debug.Log(name + " is burning!");

        // ✅ VFX lửa nằm TRÊN sprite vật
        if (fireVFX != null)
        {
            currentFireVFX = Instantiate(fireVFX, transform.position, Quaternion.identity, transform);

            SpriteRenderer parentSR = GetComponent<SpriteRenderer>();

            if (parentSR != null)
            {
                var renderers = currentFireVFX.GetComponentsInChildren<Renderer>();

                foreach (var r in renderers)
                {
                    r.sortingLayerID = parentSR.sortingLayerID;
                    r.sortingOrder = parentSR.sortingOrder + 5;
                }
            }
        }

        StartCoroutine(BurnRoutine());
    }

    IEnumerator BurnRoutine()
    {
        EnemyHealth hp = GetComponent<EnemyHealth>();

        Explosion2D explosion2D = GetComponent<Explosion2D>();

        Destructible destructible = GetComponent<Destructible>();

        float elapsed = 0f;

        while (elapsed < burnDuration)
        {
            elapsed += 1f;

            if (hp != null)
            {
                // 🔥 Enemy → trừ máu mỗi giây
                hp.TakeDamageWithoutKnockedBack(1);
            }

            if (explosion2D != null)
            {
                explosion2D.TakeDamage();
            }

          

            SpreadFire();

            yield return new WaitForSeconds(1f);
        }

        // 🔥 Hết thời gian cháy
        isBurning = false;

       

        if (currentFireVFX != null)
            Destroy(currentFireVFX);

        if (destructible != null)
        {
            destructible.BeDestroyed();
        }
        // 🌿 Nếu KHÔNG phải enemy → cháy rụi
        if (hp == null && explosion2D ==null)
        {
            Destroy(gameObject);
        }

    }



    void SpreadFire()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, spreadRadius, burnableLayer);

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject) continue;

            Burnable other = hit.GetComponent<Burnable>();
            if (other != null && !other.isBurning)
            {
                StartCoroutine(IgniteWithDelay(other));
            }
        }
    }

    IEnumerator IgniteWithDelay(Burnable target)
    {
        yield return new WaitForSeconds(0.5f); // ⏳ thời gian lửa lan

        if (target != null && !target.isBurning)
            target.Ignite();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        // nếu game đang chạy thì dùng vị trí thật, còn không thì dùng transform
        Vector3 pos = Application.isPlaying ? transform.position : transform.position;

        Gizmos.DrawWireSphere(pos, spreadRadius);
    }

}
