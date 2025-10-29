using UnityEngine;

public class ItemSwitcher : MonoBehaviour
{
    [Header("Hat Settings")]
    public Transform hatSlot;
    public GameObject[] hats;
    private int currentHatIndex = -1;
    private GameObject currentHat;

    [Header("Body Renderer")]
    public SpriteRenderer bodyRenderer;     // 拖角色的 Body 这个 SpriteRenderer

    [Header("Body Variants (Sprites)")]
    public Sprite[] bodySkins;  

    [Header("Rod Settings")]
    public Transform rodSlot;
    public GameObject[] rods;
    private int currentRodIndex = -1;
    private GameObject currentRod;

    private int currentBodyIndex = 0;

    void Start()
    {
        // 初始化装备（可选）
        if (hats.Length > 0) SwitchHat(0);
        if (bodySkins != null && bodySkins.Length > 0)
            Apply(currentBodyIndex);        
        if (rods.Length > 0) SwitchRod(0);
    }

    // -------------------
    // 🎩 Hat 控制
    // -------------------
    public void NextHat()
    {
        if (hats.Length == 0) return;
        currentHatIndex = (currentHatIndex + 1) % hats.Length;
        SwitchHat(currentHatIndex);
    }

    public void PreviousHat()
    {
        if (hats.Length == 0) return;
        currentHatIndex = (currentHatIndex - 1 + hats.Length) % hats.Length;
        SwitchHat(currentHatIndex);
    }

    private void SwitchHat(int index)
    {
        if (currentHat != null) Destroy(currentHat);
        currentHat = Instantiate(hats[index], hatSlot);
        ResetTransform(currentHat);
    }

    // -------------------
    // 🧥 Jacket 控制
    // -------------------

    public void NextBody()
    {
        if (bodySkins == null || bodySkins.Length == 0) return;
        currentBodyIndex = (currentBodyIndex + 1) % bodySkins.Length;
        Apply(currentBodyIndex);
    }

    public void PrevBody()
    {
        if (bodySkins == null || bodySkins.Length == 0) return;
        currentBodyIndex = (currentBodyIndex - 1 + bodySkins.Length) % bodySkins.Length;
        Apply(currentBodyIndex);
    }

    private void Apply(int index)
    {
        bodyRenderer.sprite = bodySkins[index];
        // 若衣服边缘要盖住手臂，调下 sortingOrder（例如 Body=5，Arm=4 或用 Sorting Group）
    }

    // -------------------
    // 🎣 Rod 控制
    // -------------------
    public void NextRod()
    {
        if (rods.Length == 0) return;
        currentRodIndex = (currentRodIndex + 1) % rods.Length;
        SwitchRod(currentRodIndex);
    }

    public void PreviousRod()
    {
        if (rods.Length == 0) return;
        currentRodIndex = (currentRodIndex - 1 + rods.Length) % rods.Length;
        SwitchRod(currentRodIndex);
    }

    private void SwitchRod(int index)
    {
        if (currentRod != null) Destroy(currentRod);
        currentRod = Instantiate(rods[index], rodSlot);
        ResetTransform(currentRod);
    }

    // -------------------
    // 通用重置函数
    // -------------------
    private void ResetTransform(GameObject obj)
    {
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
        obj.transform.localScale = Vector3.one;
    }
}