using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader I;
    [Range(0.05f, 3f)] public float fadeDuration = 0.5f;

    CanvasGroup cg;

    void Awake()
    {
        Debug.Log($"[ScreenFader] Awake called. GameObject: {gameObject.name}, Scene: {gameObject.scene.name}, Instance exists: {I != null}");
        
        if (I && I != this) 
        { 
            Debug.LogWarning($"[ScreenFader] Duplicate found! Destroying this one in scene: {gameObject.scene.name}");
            Destroy(gameObject); 
            return; 
        }
        
        I = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log($"[ScreenFader] Set as singleton. Instance ID: {GetInstanceID()}");
        
        cg = GetComponent<CanvasGroup>();
        if (!cg) 
        {
            cg = gameObject.AddComponent<CanvasGroup>();
            Debug.Log("[ScreenFader] Added CanvasGroup component");
        }
        
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;
        
        Debug.Log($"[ScreenFader] Initialization complete. Alpha: {cg.alpha}");
    }

    void OnDestroy()
    {
        Debug.LogWarning($"[ScreenFader] OnDestroy called! Instance ID: {GetInstanceID()}");
        if (I == this)
        {
            Debug.LogWarning("[ScreenFader] Singleton instance is being destroyed!");
            I = null;
        }
    }

    public Coroutine FadeToBlack()
    {
        Debug.Log($"[ScreenFader] FadeToBlack called. Current alpha: {cg.alpha}");
        return StartCoroutine(Fade(1f));
    }
    
    public Coroutine FadeFromBlack()
    {
        Debug.Log($"[ScreenFader] FadeFromBlack called. Current alpha: {cg.alpha}");
        return StartCoroutine(Fade(0f));
    }

    IEnumerator Fade(float target)
    {
        Debug.Log($"[ScreenFader] Starting fade to {target}");
        cg.blocksRaycasts = true;
        float start = cg.alpha;
        float t = 0f;
        
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }
        
        cg.alpha = target;
        cg.blocksRaycasts = target > 0.99f;
        Debug.Log($"[ScreenFader] Fade complete. Final alpha: {cg.alpha}, blocksRaycasts: {cg.blocksRaycasts}");
    }

    public void SetInstant(float a) 
    { 
        if (cg == null)
        {
            Debug.LogError("[ScreenFader] SetInstant called but CanvasGroup is null!");
            return;
        }
        cg.alpha = a; 
        cg.blocksRaycasts = a >= 1f;
        Debug.Log($"[ScreenFader] SetInstant to {a}");
    }
}