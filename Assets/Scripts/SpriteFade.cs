using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFade : MonoBehaviour
{
    [SerializeField] private float fadeTime = .4f;

    SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public IEnumerator SlowFadeRoutine()
    {
        float elapseTime = 0;
        float startValue = spriteRenderer.color.a;

        while (elapseTime < fadeTime)
        {
            elapseTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startValue, 0f, elapseTime / fadeTime);
            yield return null;

        }
        Destroy(gameObject);
    }
}
