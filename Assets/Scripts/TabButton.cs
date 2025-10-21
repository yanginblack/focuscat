using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class TabButton : MonoBehaviour {
    public TMP_Text label;
    public Button button;
    public string category;
    public Action<string> onClick;

    void Awake() {
        button.onClick.AddListener(() => onClick?.Invoke(category));
    }

    public void SetActive(bool active) {
        // update visual state (colors/bold/underline)
        var colors = button.colors;
        colors.normalColor = active ? new Color(1,1,1,1) : new Color(1,1,1,0.7f);
        button.colors = colors;
        label.fontStyle = active ? FontStyles.Bold : FontStyles.Normal;
    }
}
