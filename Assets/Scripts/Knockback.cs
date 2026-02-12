using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{

    public bool gettingKnockback { get;  set; }

    [SerializeField] float knockBackTime = 0.2f;

    Rigidbody2D rb;

    private void Awake()
    {
      
        rb = GetComponent<Rigidbody2D>();
    }

    public void GetKnockedBack(Transform damageSource, float knockBackThrust)
    {
        if (GetComponent<Player>())
        {
            if(PlayerHealth.Instance.isDead || Player.instance.isRolling)
            {
                return;
            }
        }
        gettingKnockback = true;
        Vector2 difference = (transform.position - damageSource.position).normalized * knockBackThrust * rb.mass;
        rb.AddForce(difference, ForceMode2D.Impulse);
        StartCoroutine(KnockRoutine());

    }
    
    IEnumerator KnockRoutine()
    {
        yield return new WaitForSeconds(knockBackTime);
        rb.velocity = Vector2.zero;

        gettingKnockback = false;
    }

    
}
