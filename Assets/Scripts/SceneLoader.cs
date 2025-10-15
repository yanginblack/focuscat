using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] string sceneName = "Fishing";
    bool isLoading;
    static SceneLoader instance;

    void Awake()
    {
        // Keep SceneLoader alive between scenes to prevent coroutine interruption
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadScene()
    {
        if (isLoading) return;
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("[SceneLoader] sceneName is empty.");
            return;
        }
        StartCoroutine(LoadWithFade(sceneName));
    }

    IEnumerator LoadWithFade(string target)
    {
        isLoading = true;
        Debug.Log($"[SceneLoader] Start loading '{target}'");
        Debug.Log($"[SceneLoader] ScreenFader instance ID before: {ScreenFader.I?.GetInstanceID()}");

        // Store reference to ScreenFader before scene transition
        ScreenFader fader = ScreenFader.I;

        // 1) Fade to black
        if (fader != null)
        {
            Debug.Log("[SceneLoader] FadeToBlack...");
            yield return fader.FadeToBlack();
        }
        else
        {
            Debug.LogWarning("[SceneLoader] No ScreenFader found — loading without fade.");
        }

        // 2) Async load
        AsyncOperation op = SceneManager.LoadSceneAsync(target);
        if (op == null)
        {
            Debug.LogError("[SceneLoader] LoadSceneAsync returned null. Check scene name & Build Settings.");
            if (fader != null) yield return fader.FadeFromBlack();
            isLoading = false;
            yield break;
        }

        op.allowSceneActivation = false;
        
        // Wait for scene to load to 90%
        while (op.progress < 0.9f)
        {
            Debug.Log($"[SceneLoader] Loading progress: {op.progress * 100}%");
            yield return null;
        }

        Debug.Log("[SceneLoader] Activating scene...");
        op.allowSceneActivation = true;
        
        // Wait for scene to be completely done
        while (!op.isDone)
        {
            Debug.Log($"[SceneLoader] Waiting for scene activation... isDone: {op.isDone}");
            yield return null;
        }
        
        Debug.Log($"[SceneLoader] Scene activated. Active scene is now: {SceneManager.GetActiveScene().name}");
        
        // Small delay to ensure everything is initialized
        yield return new WaitForEndOfFrame();
        yield return null;
        
        // Check if ScreenFader still exists, use stored reference or find new one
        if (fader == null)
        {
            Debug.Log("[SceneLoader] Stored fader reference was null, trying to find ScreenFader.I");
            fader = ScreenFader.I;
        }
        
        Debug.Log($"[SceneLoader] ScreenFader instance ID after: {fader?.GetInstanceID()}");
        Debug.Log($"[SceneLoader] ScreenFader.I is null? {ScreenFader.I == null}");
        Debug.Log($"[SceneLoader] fader is null? {fader == null}");

        // 3) Fade back in
        if (fader != null)
        {
            Debug.Log("[SceneLoader] Starting FadeFromBlack...");
            yield return fader.FadeFromBlack();
            Debug.Log("[SceneLoader] FadeFromBlack complete!");
        }
        else
        {
            Debug.LogError("[SceneLoader] No ScreenFader available for fade in!");
        }

        isLoading = false;
        Debug.Log("[SceneLoader] Load complete.");
    }

    // Optional: Public method to load any scene
    public void LoadSceneByName(string newSceneName)
    {
        if (isLoading) return;
        sceneName = newSceneName;
        LoadScene();
    }
}