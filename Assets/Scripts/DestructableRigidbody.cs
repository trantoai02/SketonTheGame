using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableRigidbody : MonoBehaviour
{
    [SerializeField] Vector2 forceDirection;

    [SerializeField]
    int torque;
    int forceX;
    int forceY;

    Rigidbody2D rb;

    [SerializeField] GameObject explosionEffectPrefab;
    void Start()
    {
        if(torque == 0)
        {
            torque = Random.Range(-200, 200);

        }
        if(forceX == 0)
        {
            forceX = Random.Range(-400, 400);

        }
        if (forceY == 0)
        {
            forceY = Random.Range(-400, 400);

        }


        rb = GetComponent<Rigidbody2D>();

        if (explosionEffectPrefab != null)
        {
            GameObject explosionEff = Instantiate(explosionEffectPrefab);
            explosionEff.transform.position = transform.position;

        }

        rb.AddForce(new Vector2(forceX, forceY));
        rb.AddTorque(torque);

        Invoke("DestroySelf", Random.Range(2, 5));
    }


    void DestroySelf()
    {
        Destroy(gameObject);
    }
}
