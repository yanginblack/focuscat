using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using static ItemManager;

public class ItemCell : MonoBehaviour {
    public Image icon;
    public TMP_Text priceText;
    public Button button;

    private ItemData _item;
    private AsyncOperationHandle<Sprite>? _handle;

    public void Bind(ItemData item, Action<ItemData> onClick) {
        _item = item;
        priceText.text = $"$ {item.price}";

        // async load sprite
        _handle = Addressables.LoadAssetAsync<Sprite>(item.iconPathSmall);
        _handle.Value.Completed += h => {
            if (h.Status == AsyncOperationStatus.Succeeded && icon)
                icon.sprite = h.Result;
        };

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(_item));
    }

    void OnDestroy() {
        if (_handle.HasValue) Addressables.Release(_handle.Value);
    }
}
