using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BGScaler : MonoBehaviour
{
   
    void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Vector2 tempScale = transform.localScale;

        float height = spriteRenderer.bounds.size.y;
        float width = spriteRenderer.bounds.size.x;

        float worldHeight = Camera.main.orthographicSize * 2f;
        float worldWidth = worldHeight*Screen.width/Screen.height;

        tempScale.x = worldWidth / width;
        tempScale.y = worldHeight / height;

        transform.localScale = tempScale;
    }

}
