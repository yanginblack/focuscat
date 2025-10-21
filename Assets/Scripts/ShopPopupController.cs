using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using static ItemManager;

public class ShopPopupController : MonoBehaviour {
    [Header("Root")]
    public GameObject popup;          // ShopPopup
    public Button closeButton;
    public TMP_Text moneyText;

    [Header("Tabs")]
    public Transform tabsParent;      // contains TabButton instances (or make them at runtime)
    public TabButton tabPrefab;

    [Header("Grid")]
    public Transform gridContent;     // ScrollView/Viewport/Content
    public ItemCell cellPrefab;

    [Header("Detail")]
    public ItemDetailPanel detailPanel;   // reference to other overlay/panel

    private readonly List<TabButton> _tabs = new();
    private string _activeCategory;

    void Start() {
        popup.SetActive(false);
        closeButton.onClick.AddListener(Hide);
    }

    public void Show() {
        RebuildTabs();
        SetCategory(FirstCategory());
        RefreshMoney();
        popup.SetActive(true);
    }

    public void Hide() {
        popup.SetActive(false);
        ClearGrid();
    }

    private void RebuildTabs() {
        foreach (Transform c in tabsParent) Destroy(c.gameObject);
        _tabs.Clear();

        foreach (var cat in SharedData.I.itemDatabase.GetCategories()) {
            var t = Instantiate(tabPrefab, tabsParent);
            t.category = cat;
            t.label.text = cat;
            t.onClick = SetCategory;
            _tabs.Add(t);
        }
    }

    private string FirstCategory() {
        foreach (var c in SharedData.I.itemDatabase.GetCategories()) return c;
        return null;
    }

    private void SetCategory(string cat) {
        _activeCategory = cat;
        foreach (var t in _tabs) t.SetActive(t.category == cat);
        RebuildGrid(cat);
    }

    private void RebuildGrid(string cat) {
        ClearGrid();
        foreach (var item in SharedData.I.itemDatabase.GetByCategory(cat)) {
            var cell = Instantiate(cellPrefab, gridContent);
            cell.Bind(item, OnItemClicked);
        }
    }

    private void ClearGrid() {
        for (int i = gridContent.childCount - 1; i >= 0; --i)
            Destroy(gridContent.GetChild(i).gameObject);
    }

    private void RefreshMoney() => moneyText.text = $"$ {SharedData.I.userData.money:n0}";

    private void OnItemClicked(ItemData item) {
        // open detail
        detailPanel.Show(item, OnBuy, OnCancel);
    }

    private void OnBuy(ItemData item) {
        if (SharedData.I.userData.SpendMoney(item.price))
        {
            SharedData.I.userData.AddItem(item.id);
            RefreshMoney();
            detailPanel.ShowToast("Purchased!", /*hideAfter=*/true);
        }
        else
        {
            detailPanel.ShowToast("Not enough money!");
        }
    }

    private void OnCancel() {
        // default to hide
    }
}
