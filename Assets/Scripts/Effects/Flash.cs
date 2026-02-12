using System.Collections;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [SerializeField] Material whiteFlashMat;
    [SerializeField] float restoreDefaultMatTime = 0.2f;

    Material defaultMat;
    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null )
            spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        defaultMat = spriteRenderer.material;
    }

    public float GetRestoreMatTime()
    {
        return restoreDefaultMatTime;
    }

    public IEnumerator FlashRoutine()
    {
        spriteRenderer.material = whiteFlashMat;
        yield return new WaitForSeconds(restoreDefaultMatTime); 
        spriteRenderer.material = defaultMat;
       
    }
}
