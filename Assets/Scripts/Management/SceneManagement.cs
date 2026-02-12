using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public static SceneManagement instance;
    [SerializeField] string sceneToLoad;

    float waitToLoadTime = 1f;
    private void Awake()
    {
        if (instance == null)
            instance = this;


}
    public string SceneTransitionName {get; private set;}

    public void SetTransitionName(string sceneTransitionName)
    {
        this.SceneTransitionName = sceneTransitionName;
    }
    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1f;
        UIFade.instance.FadeOut();
        StartCoroutine(LoadScreenRoutine(sceneName));

    }
    public void LoadScene()
    {
        Time.timeScale = 1f;
        UIFade.instance.FadeOut();
        StartCoroutine(LoadScreenRoutine(sceneToLoad));
    }

    public void LoadSceneFast(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator LoadScreenRoutine(string sceneName)
    {
        while (waitToLoadTime >= 0)
        {
            waitToLoadTime -= Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadScene(sceneName);

    }
}
