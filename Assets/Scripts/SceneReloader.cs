using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReloader : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerHealth.OnPlayerDied += ReloadScene;
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDied -= ReloadScene;
    }

    public void ReloadScene()
    {
        Debug.Log("🔄 Player chết — Load lại scene hiện tại");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
