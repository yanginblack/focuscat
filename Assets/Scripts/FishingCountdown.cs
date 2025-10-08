using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FishingCountdown : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI countdownText;
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;

    [Header("Scene Objects")]
    public GameObject fishObject;   // ← drag your fish GameObject here

    [Header("Config")]
    public int durationSeconds = 20;

    float remaining;
    bool running;

    void OnEnable()
    {
        StartCountdown();
    }

    public void StartCountdown()
    {
        remaining = durationSeconds;
        running = true;
        if (resultPanel) resultPanel.SetActive(false);
        if (fishObject) fishObject.SetActive(true);  // ensure fish is visible again
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        UpdateCountdownLabel();
    }

    void Update()
    {
        if (!running) return;

        remaining -= Time.unscaledDeltaTime;
        if (remaining <= 0f)
        {
            remaining = 0f;
            running = false;
            OnCountdownSuccess();
        }
        UpdateCountdownLabel();
    }

    void UpdateCountdownLabel()
    {
        if (!countdownText) return;
        int secs = Mathf.CeilToInt(remaining);
        int m = secs / 60;
        int s = secs % 60;
        countdownText.text = $"{m:00}:{s:00}";
    }

    void OnCountdownSuccess()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
        if (resultText) resultText.text = "You succeeded!";
        if (resultPanel) resultPanel.SetActive(true);
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (running && !hasFocus) ShowRestPanel();
    }

    void OnApplicationPause(bool paused)
    {
        if (running && paused) ShowRestPanel();
    }

    void ShowRestPanel()
    {
        running = false;
        Screen.sleepTimeout = SleepTimeout.SystemSetting;

        if (resultText) resultText.text = "Having a rest?";
        if (resultPanel) resultPanel.SetActive(true);
        if (fishObject) fishObject.SetActive(false);  // hide the fish
    }

    public void OnClickContinue() => StartCountdown();
    public void OnClickBackHome() => SceneManager.LoadScene("Home");
}