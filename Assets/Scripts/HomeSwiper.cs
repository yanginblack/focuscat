using UnityEngine;
using UnityEngine.InputSystem; // ← 新输入系统

[RequireComponent(typeof(Camera))]
public class HomeSwiper : MonoBehaviour
{
    [Header("Assign your Background SpriteRenderer here")]
    public SpriteRenderer background;

    [Header("Swipe Settings")]
    [Range(0.02f, 0.5f)] public float swipeThresholdScreen = 0.12f;
    public float snapSpeed = 6f;
    public float dragSensitivity = 1.0f;

    private Camera cam;
    private float camHalfHeight, camHalfWidth;
    private Bounds bgBounds;

    private float leftCenterX, midCenterX, rightCenterX;
    private int currentPage = 1; // 0=左,1=中,2=右

    private bool dragging = false;
    private Vector2 dragStartScreenPos;
    private Vector3 dragStartCamPos;

    private bool inited = false;

    void Start()
    {
        cam = GetComponent<Camera>();
        InitIfPossible();
    }

    void InitIfPossible()
    {
        if (background == null || background.sprite == null) return;

        bgBounds = background.bounds;

        camHalfHeight = cam.orthographicSize;
        camHalfWidth = camHalfHeight * cam.aspect;

        float bgLeft = bgBounds.min.x;
        float bgRight = bgBounds.max.x;
        float bgWidth = bgBounds.size.x;

        float thirdWidth = bgWidth / 3f;
        leftCenterX  = bgLeft + thirdWidth * 0.5f;
        midCenterX   = bgLeft + thirdWidth * 1.5f;
        rightCenterX = bgLeft + thirdWidth * 2.5f;

        float neededWidth = camHalfWidth * 2f;
        if (thirdWidth < neededWidth)
        {
            Debug.LogWarning($"[CameraSwiper] PageWidth({thirdWidth:0.##}) < CameraWidth({neededWidth:0.##}). Consider smaller orthographicSize or larger background.");
        }

        Vector3 p = transform.position;
        p.x = Mathf.Clamp(midCenterX, bgLeft + camHalfWidth, bgRight - camHalfWidth);
        transform.position = p;
        currentPage = 1;

        inited = true;
    }

    void Update()
    {
        if (!inited)
        {
            InitIfPossible();
            if (!inited) return;
        }

        HandleInputSystem();

        if (!dragging)
        {
            float targetX = GetPageCenterX(currentPage);
            float clamped = ClampCamX(targetX);
            Vector3 pos = transform.position;
            pos.x = Mathf.Lerp(pos.x, clamped, Time.deltaTime * snapSpeed);
            transform.position = pos;
        }
    }

    float GetPageCenterX(int page)
    {
        switch (page)
        {
            case 0: return leftCenterX;
            case 1: return midCenterX;
            case 2: return rightCenterX;
        }
        return midCenterX;
    }

    float ClampCamX(float x)
    {
        float minX = bgBounds.min.x + camHalfWidth;
        float maxX = bgBounds.max.x - camHalfWidth;
        return Mathf.Clamp(x, minX, maxX);
    }

    // ========= 新输入系统处理 =========
    void HandleInputSystem()
    {
        bool hasTouchscreen = Touchscreen.current != null;
        bool hasMouse = Mouse.current != null;

        // Pointer "down"
        bool down =
            (hasTouchscreen && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) ||
            (hasMouse && Mouse.current.leftButton.wasPressedThisFrame);

        // Pointer "held"
        bool held =
            (hasTouchscreen && Touchscreen.current.primaryTouch.press.isPressed) ||
            (hasMouse && Mouse.current.leftButton.isPressed);

        // Pointer "up"
        bool up =
            (hasTouchscreen && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame) ||
            (hasMouse && Mouse.current.leftButton.wasReleasedThisFrame);

        Vector2 pointerPos = ReadPointerPosition();

        if (down)
        {
            dragging = true;
            dragStartScreenPos = pointerPos;
            dragStartCamPos = transform.position;
        }
        else if (held && dragging)
        {
            Vector2 delta = pointerPos - dragStartScreenPos;
            float worldDeltaX = ScreenToWorldDeltaX(delta.x) * dragSensitivity;
            Vector3 pos = dragStartCamPos;
            // 左拖(屏幕dx<0) => 视角右移(世界x增加)
            pos.x = ClampCamX(dragStartCamPos.x - worldDeltaX);
            transform.position = pos;
        }
        else if (up && dragging)
        {
            Vector2 delta = pointerPos - dragStartScreenPos;
            EndDragDecidePage(delta);
            dragging = false;
        }
    }

    Vector2 ReadPointerPosition()
    {
        if (Touchscreen.current != null)
        {
            // 使用 primary touch 的当前位置
            return Touchscreen.current.primaryTouch.position.ReadValue();
        }
        if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }
        return Vector2.zero;
    }

    void EndDragDecidePage(Vector2 screenDelta)
    {
        float dx = screenDelta.x;
        float threshold = Screen.width * swipeThresholdScreen;

        if (Mathf.Abs(dx) < threshold)
        {
            float x = transform.position.x;
            float dL = Mathf.Abs(x - leftCenterX);
            float dM = Mathf.Abs(x - midCenterX);
            float dR = Mathf.Abs(x - rightCenterX);
            int nearest = 0;
            float best = dL;
            if (dM < best) { best = dM; nearest = 1; }
            if (dR < best) { best = dR; nearest = 2; }
            currentPage = nearest;
        }
        else
        {
            // 向左滑（dx<0）=> 视角右移 => 下一页
            if (dx < 0) currentPage = Mathf.Min(currentPage + 1, 2);
            else        currentPage = Mathf.Max(currentPage - 1, 0);
        }
    }

    float ScreenToWorldDeltaX(float screenDx)
    {
        float worldWidth = camHalfWidth * 2f;
        return (screenDx / Screen.width) * worldWidth;
    }

    void OnDrawGizmosSelected()
    {
        if (background != null)
        {
            var b = background.bounds;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(new Vector3(b.min.x, b.min.y, 0), new Vector3(b.min.x, b.max.y, 0));
            Gizmos.DrawLine(new Vector3(b.max.x, b.min.y, 0), new Vector3(b.max.x, b.max.y, 0));
        }
    }
}