using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImageCarousel : MonoBehaviour
{
    [SerializeField] private Image imagePrefab;
    [SerializeField] private RectTransform content;
    [SerializeField] private Image background;
    private List<Sprite> imagesList = new();
    private const float dragThreshold = 150f;
    private const float snapDuration = 0.5f;
    private const float returnDuration = 0.2f;
    private float itemWidth;
    private int imagesCount;
    private bool isDragging;
    private int currentIndex;
    private int CurrentIndex
    {
        get => currentIndex;
        set
        {
            background.sprite = imagesList[value];
            currentIndex = value;
        }
    }

    private void Awake() { itemWidth = content.rect.width; }

    public void Initialize(List<Sprite> images)
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);

        imagesList = images;

        foreach (Sprite sprite in images)
        {
            Image newImage = Instantiate(imagePrefab, content);
            newImage.sprite = sprite;
            newImage.preserveAspect = true;
        }
        imagesCount = images.Count;

        if (imagesCount > 0) Canvas.ForceUpdateCanvases();

        CurrentIndex = 0;
        content.anchoredPosition = Vector2.zero;
    }

    public void OnBeginDrag(BaseEventData eventData) => isDragging = true;

    public void OnEndDrag(BaseEventData eventData)
    {
        if (isDragging)
        {
            StartCoroutine(Return(DragOffset < 0));
            isDragging = false;
        }
    }

    public void OnDrag(BaseEventData eventData)
    {
        if (!isDragging) return;

        content.anchoredPosition += Vector2.right * ((PointerEventData)eventData).delta.x;
        float dragOffset = DragOffset;
        if (Mathf.Abs(dragOffset) > dragThreshold)
        {
            bool isPositive = dragOffset < 0;
            int nextIndex = NextIndex(isPositive);
            isDragging = false;
            if (nextIndex >= 0 && nextIndex < imagesCount)
                StartCoroutine(Move(isPositive));
            else StartCoroutine(Return(isPositive));
        }
    }

    private int NextIndex(bool isPositive) => CurrentIndex + (isPositive ? 1 : -1);
    private float DragOffset => content.anchoredPosition.x - (-CurrentIndex * itemWidth);

    private IEnumerator Move(bool isPositive)
    {
        CurrentIndex += (isPositive ? 1 : -1);
        Vector2 startPos = content.anchoredPosition;
        Vector2 targetPos = Vector2.right * -CurrentIndex * itemWidth;
        float elapsed = 0f;

        while (elapsed < snapDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / snapDuration);
            content.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        content.anchoredPosition = targetPos;
    }

    private IEnumerator Return(bool isPositive)
    {
        Vector2 startPos = content.anchoredPosition;
        Vector2 targetPos = Vector2.right * -CurrentIndex * itemWidth;
        float elapsed = 0f;

        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / returnDuration);
            content.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        content.anchoredPosition = targetPos;
    }
    
    private void OnDestroy() => StopAllCoroutines();
}
