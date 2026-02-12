
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class OnTimelineFinished : MonoBehaviour
{
    public string nextSceneName; 

    private void Update()
    {
        if (GetComponent<PlayableDirector>().state != PlayState.Playing)
        {
            LoadNextScene();
        }
    }
    public void LoadNextScene()
    {
        // Chỉ gọi LoadScene khi game đang chạy trong chế độ play
        if (Application.isPlaying)
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("LoadNextScene should only be called during play mode.");
        }
    }
}
