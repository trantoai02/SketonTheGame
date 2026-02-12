using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour
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
        torque = Random.Range(-200, 200);
        forceX = Random.Range(400, 900);
        forceY = Random.Range(-200, 200);


        rb = GetComponent<Rigidbody2D>();


        rb.bodyType = RigidbodyType2D.Kinematic;

    }
    public void Explode()
    {

        rb.AddForce(new Vector2(forceX, forceY));
        rb.AddTorque(torque);

        Invoke("DestroySelf", Random.Range(2, 5));

    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "EnemyBoss")
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            Explode();
           // Debug.Log("slime boss triggered");
        }
    }
}
