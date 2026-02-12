
using UnityEngine;

public class lightningStriker : MonoBehaviour
{
    public int strikeDamage = 6;
    Collider2D collider;
    private void Start()
    {
        collider = GetComponent<Collider2D>();

        collider.enabled = false;
    }

    public void DestroyLightning()
    {
        Destroy(gameObject);
    }

    public void SettingStrike()
    {
        //bật âm thanh những tia sét
    }

    public void On_LightningStrikes()
    {
        //bật âm thanh sét đánh

        //bật collider damage
        collider.enabled = true;
   
       

    }

    public void Off_LightningStrikes()
    {
        //bật âm thanh sét đánh

        //tắt collider damage
        collider.enabled = false;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy" )
        {
            collision.gameObject.GetComponent<EnemyHealth>().TakeDamageWithoutKnockedBack(strikeDamage);
        } 

        if(collision.gameObject.GetComponent<BreakApartObject>() != null)
        {
            collision.gameObject.GetComponent<BreakApartObject>().TakeDamage();
        } 
        
        if(collision.gameObject.GetComponent<Explosion2D>() != null)
        {
            collision.gameObject.GetComponent<Explosion2D>().TakeDamage();
        }


        
    }
}
