
using System.Collections;
using UnityEngine;

public class TransparentFlicker : MonoBehaviour
{
    Color zeroTransparentSpriteColor;
    public Color defaultColor;
    public SpriteRenderer sr;
    public float restoreToDefaultTime = 0.1f;
    private void Awake()
    {
        if(sr != null)
            defaultColor = sr.color;
        zeroTransparentSpriteColor = new Color(1, 1, 1, 0);
    }

    public IEnumerator FlickerRoutine()
    {
        while (true)
        {
            sr.color = zeroTransparentSpriteColor;
            yield return new WaitForSeconds(restoreToDefaultTime);
            sr.color = defaultColor;
            yield return new WaitForSeconds(restoreToDefaultTime);
        }
    }

}
