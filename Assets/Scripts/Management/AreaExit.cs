using System.Collections;

using UnityEngine.SceneManagement;
using UnityEngine;


public class AreaExit : MonoBehaviour
{
    [SerializeField] string sceneToLoad;
    [SerializeField] string sceneTransitionName;

    //float waitToLoadTime = 1f;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerXPLevelUpManager.instance.SaveXP();
            SceneManagement.instance.SetTransitionName(sceneTransitionName);
            SceneManagement.instance.LoadScene(sceneToLoad);
            PlayerPrefs.SetString("playingLevel", sceneToLoad);

        }
    }
}

