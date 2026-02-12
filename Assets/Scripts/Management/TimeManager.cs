using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    bool isSlowmotion = false;
    public float normalTimeScale = 1f;

    [Range(0f, 1f)]
    public float timeScaleFactor = 0.5f;
    

    public void DoSlowmotion()
    {
        if (isSlowmotion)
        {
            Time.timeScale = timeScaleFactor;
          
        }
        else
        {
            Time.timeScale = normalTimeScale;
        }
        Time.fixedDeltaTime = Time.timeScale * 0.02f;


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            isSlowmotion = !isSlowmotion;
            DoSlowmotion();
        }
    }
}
