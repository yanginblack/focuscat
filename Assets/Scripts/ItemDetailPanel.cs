using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static ItemManager;

public class ItemDetailPanel : MonoBehaviour {
    public GameObject panel;            // ItemDetailPanel
    public Image largeIcon;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public TMP_Text descText;
    public Button buyButton;
    public Button cancelButton;
    public TMP_Text toastText;          // small fading text (optional)

    private ItemData _item;
    private Action<ItemData> _onBuy;
    private Action _onCancel;
    private AsyncOperationHandle<Sprite>? _handle;

    void Awake() { Hide(); }

    public void Show(ItemData item, Action<ItemData> onBuy, Action onCancel) {
        _item = item; _onBuy = onBuy; _onCancel = onCancel;

        nameText.text = item.name;
        priceText.text = $"$ {item.price}";
        descText.text = item.description;

        panel.SetActive(true);
        HideToast();

        if (_handle.HasValue) Addressables.Release(_handle.Value);
        // Change to iconPathLarge to load the large icon
        _handle = Addressables.LoadAssetAsync<Sprite>(item.iconPathSmall);
        _handle.Value.Completed += h => {
            if (h.Status == AsyncOperationStatus.Succeeded) largeIcon.sprite = h.Result;
        };

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => _onBuy?.Invoke(_item));

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() => { Hide(); _onCancel?.Invoke(); });
    }

    public void Hide() {
        panel.SetActive(false);
    }

    public void ShowToast(string msg, bool hideAfter = false)
    {
        if (!toastText) return;
        toastText.text = msg;
        toastText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideToast));
        Invoke(nameof(HideToast), 1.2f);
        if (hideAfter)
        {
            Invoke(nameof(Hide), 1.2f);
        }
    }

    private void HideToast() { if (toastText) toastText.gameObject.SetActive(false); }

    void OnDestroy() {
        if (_handle.HasValue) Addressables.Release(_handle.Value);
    }
}
