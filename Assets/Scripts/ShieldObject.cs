using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldObject : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Knockback>().GetKnockedBack(transform, 10f);
            if (Player.instance.parryCollider.gameObject.activeSelf && PlayerShieldManager.instance.currentShield >= 2.5f)
            {
                Debug.Log("đã được bảo vệ");
                PlayerShieldManager.instance.ShieldConsume(2.5f);
            }
        }
      
    }
}
