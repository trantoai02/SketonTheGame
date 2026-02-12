using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    public GameManager gameManager;

     void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            gameManager.CompleteLevel();
        }
    }

    public void GoToPlayerPos()
    {
        transform.position = Player.instance.transform.position;
    }
}
