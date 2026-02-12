using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BlackOut : MonoBehaviour
{

    //tham chieu tu electicGenerator co trong scene
    [SerializeField]
    public Light2D light2D;

    IEnumerator TatDien()
    {
        yield return new WaitForSeconds(.1f);
        light2D.intensity = .1f;
        yield return new WaitForSeconds(.2f);
        light2D.intensity = 1;
        yield return new WaitForSeconds(.1f);
        light2D.intensity = .1f;
        yield return new WaitForSeconds(.1f);
        light2D.intensity = 1;
        yield return new WaitForSeconds(.1f);
        light2D.intensity = .1f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("EnemyBoss"))
        {
            StartCoroutine(TatDien());

            Debug.Log("slime boss triggered");

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("EnemyBoss"))
            StartCoroutine(TatDien());
    }
}
