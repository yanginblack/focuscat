using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // load a scene by name
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // optional helper: quit the app
    public void QuitApp()
    {
        Application.Quit();
        Debug.Log("Game closed");
    }
}
