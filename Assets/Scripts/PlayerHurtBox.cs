using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHurtBox : MonoBehaviour
{
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Enemy"))
    //    {
    //        var health = GetComponentInParent<PlayerHealth>();
    //        if (health != null && !Player.instance.isRolling && health.canTakeDamage)
    //        {
    //            //tính chỉ số dmg scale theo level kẻ địch
    //            int damage = (int) Mathf.Floor((float)EnemyStats.Instance.damage /10);
    //            damage = Mathf.Max(damage, 1);

    //            //áp dụng chỉ số dmg
    //            health.TakeDamage(damage, collision.transform);
    //        }
    //    }
    //}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Enemy"))
    //    {
    //        var health = GetComponentInParent<PlayerHealth>();
    //        if (health != null && !Player.instance.isRolling && health.canTakeDamage)
    //        {
    //            //tính chỉ số dmg scale theo level kẻ địch
    //            int damage = (int)Mathf.Floor((float)EnemyStats.Instance.damage / 10);
    //            damage = Mathf.Max(damage, 1);

    //            Debug.Log(damage);
    //            //áp dụng chỉ số dmg
    //            health.TakeDamage(damage, collision.transform);
    //        }
    //    }
    //}
}
