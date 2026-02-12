using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheForestSpike : MonoBehaviour
{
   
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            PlayerHealth.Instance.TakeDamage(1, transform);
        }
       
    }
}
