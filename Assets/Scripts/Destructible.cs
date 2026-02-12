
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject destroyEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.GetComponent<DamageSource>() || collision.gameObject.GetComponent<Projectile>() || (collision.gameObject.CompareTag("Player") && Player.instance.isRolling))
            && gameObject.tag =="DestructableObject")
        {
           
            BeDestroyed();
        }
    }

    public void BeDestroyed()
    {
        GetComponent<PickupSpawner>().DropHeartOrCoin();
        Instantiate(destroyEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
