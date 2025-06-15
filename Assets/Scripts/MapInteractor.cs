using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MapInteractor : MonoBehaviour
{
    [SerializeField] private RectTransform map;
    [SerializeField] private Graph graph;
    [SerializeField] private TMP_Text scopeText;
    private int scaleLevelMap = 0;
    private Vector2 maskSize;
    private static float[] ScaleLevels { get; } = { 1f, 1.5f, 3f, 6f, 9f, 18f };

    private void Start() => maskSize = (map.parent as RectTransform).rect.size;

    public void ScaleTheMap(float scaleLevel)
    {
        int scaleLevelInt = Mathf.RoundToInt(scaleLevel);
        float rescaler = ScaleLevels[scaleLevelInt];
        map.localScale = Vector2.one * rescaler;

        graph.Zoom(rescaler);

        float rescalerPrev = ScaleLevels[scaleLevelMap];
        scaleLevelMap = scaleLevelInt;

        ClampByBorders
        (
            map.anchoredPosition / rescalerPrev * rescaler,
            rescaler
        );

        scopeText.text = (900 / rescaler).ToString() + " м";
    }

    public void Drag(BaseEventData eventData)
    {
        ClampByBorders
        (
            map.anchoredPosition + ((PointerEventData)eventData).delta,
            ScaleLevels[scaleLevelMap]
        );
    }

    private void ClampByBorders(Vector2 newPos, float rescaler)
    {
        Vector2 max = (map.rect.size * rescaler - maskSize) * 0.5f;
        float maxX = max.x;
        float maxY = max.y;
        newPos.x = Mathf.Clamp(newPos.x, -maxX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, -maxY, maxY);
        map.anchoredPosition = newPos;
    }
}
