using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextScreenPlay : MonoBehaviour
{
    public TMP_Text text;
    public float initialDelayTime = 1f;
    public float timeBetweenSentences = 0.5f;
    public float delayTime = 0.5f;
    public float typeSpeed = 0.1f;
    public float fadeInSpeed = 1f;
    public float fadeOutSpeed = 1f;
    public string[] lines;

    public string sceneToLoad;
    public float waitToLoadTime =1f;

    IEnumerator Start()
    {
 
        yield return new WaitForSeconds(initialDelayTime);



        yield return TypeText(lines[0]);
        yield return new WaitForSeconds(1);
        yield return FadeTextOut(1);

        yield return TypeText(lines[1]);
        yield return new WaitForSeconds(3);
        yield return FadeTextOut(0.1f);

        yield return TypeText(lines[2]);
        yield return new WaitForSeconds(4);
        yield return FadeTextOut(1);

        yield return TypeText(lines[3]);
        yield return new WaitForSeconds(1);
        yield return FadeTextOut(1);

        yield return TypeText(lines[4]);
        yield return new WaitForSeconds(1);
        yield return FadeTextOut(3);

        SceneManager.LoadScene(sceneToLoad);
        StartCoroutine(LoadScreenRoutine());


    }

    IEnumerator LoadScreenRoutine()
    {
        while (waitToLoadTime >= 0)
        {
            waitToLoadTime -= Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene(sceneToLoad);
    }

    IEnumerator TypeText(string str)
    {
        
        text.text = "";
        foreach( char c in str)
        {
            yield return FadeTextIn(fadeInSpeed);
            text.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
        
    }

    IEnumerator FadeTextOut(float fadeOutTime)
    {
        float elapsedTime = 0f;
        Color startColor = text.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // Màu mục tiêu có alpha là 0

        while (elapsedTime < fadeOutTime)
        {
            float t = elapsedTime / fadeOutTime;
            text.color = Color.Lerp(startColor, targetColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        text.color = targetColor; // Đảm bảo màu cuối cùng là màu mục tiêu
    }

    IEnumerator FadeTextIn(float fadeInTime)
    {
        float elapsedTime = 0f;
        Color startColor = text.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f); // Màu mục tiêu có alpha là 1

        while (elapsedTime < fadeInTime) // Mặc định thời gian fade in là 1 giây
        {
            float t = elapsedTime / fadeInTime; // 1 giây
            text.color = Color.Lerp(startColor, targetColor, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        text.color = targetColor; // Đảm bảo màu cuối cùng là màu mục tiêu
    }

}
