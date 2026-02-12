using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class ParallaxLayer
{
    public Transform layer;          // Background layer
    [Range(0f, 1f)] public float parallaxAmount = 0.3f; // Độ sâu của lớp
}

public class ParallaxDragController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Danh sách các layer background")]
    public ParallaxLayer[] layers;

    [Header("Giới hạn kéo (nếu cần)")]
    public float limitLeft = -5f;
    public float limitRight = 5f;

    private bool isDragging;
    private float dragOffsetX;
    private Vector2 lastDragDelta;

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        lastDragDelta = Vector2.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        lastDragDelta = eventData.delta;
        dragOffsetX += eventData.delta.x;

        // Giới hạn vùng kéo
        dragOffsetX = Mathf.Clamp(dragOffsetX, limitLeft, limitRight);

        // Cập nhật vị trí tất cả layer
        foreach (var layer in layers)
        {
            if (layer.layer == null) continue;

            float offset = dragOffsetX * layer.parallaxAmount;
            layer.layer.localPosition = new Vector3(offset, layer.layer.localPosition.y, layer.layer.localPosition.z);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

    private void Update()
    {
        // Hiệu ứng trôi mượt trở về vị trí gốc khi thả tay (tuỳ chọn)
        if (!isDragging)
        {
            dragOffsetX = Mathf.Lerp(dragOffsetX, 0f, Time.deltaTime * 2f);

            foreach (var layer in layers)
            {
                if (layer.layer == null) continue;
                float offset = dragOffsetX * layer.parallaxAmount;
                layer.layer.localPosition = new Vector3(offset, layer.layer.localPosition.y, layer.layer.localPosition.z);
            }
        }
    }
}
